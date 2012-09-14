using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Models.Helpers;
using SmsFeedback_Take4.Utilities;
using SmsFeedback_EFModels;


namespace SmsFeedback_Take4.Controllers
{
    [CustomAuthorizeAtribute]
    public class ReportsController : BaseController
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private EFInteraction mEFInterface = new EFInteraction();

        public ActionResult Index()
        {
            ViewData["currentCulture"] = getCurrentCulture();
            return View();
        }

        #region First area chart sources

        public JsonResult GetTotalNoOfSmsChartSource(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);
                Dictionary<DateTime, ChartValue> resultInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                foreach (var wp in workingPoints)
                {
                    var conversations = from conv in wp.Conversations select conv;
                    foreach (var conv in conversations)
                    {

                        if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                        {
                            var msgsToFrom = from msg in conv.Messages
                                             where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd)
                                             group msg by msg.TimeReceived.Date into g
                                             select new { date = g.Key, count = g.Count() };
                            foreach (var entry in msgsToFrom)
                                resultInterval[entry.date].value += entry.count;

                        }
                        else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                        {
                            var msgsToFrom = from msg in conv.Messages
                                             where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd)
                                             group msg by new { msg.TimeReceived.Month, msg.TimeReceived.Year } into g
                                             select new { date = g.Key, count = g.Count() };
                            foreach (var entry in msgsToFrom)
                            {
                                var monthDateTime = new DateTime(entry.date.Year, entry.date.Month, 1);
                                if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                    resultInterval[intervalStart].value += entry.count;
                                else
                                    resultInterval[monthDateTime].value += entry.count;
                            }
                        }
                        else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                        {
                            var msgsToFrom = from msg in conv.Messages
                                             where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd)
                                             group msg by new { firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(msg.TimeReceived) } into g
                                             select new { date = g.Key, count = g.Count() };
                            foreach (var entry in msgsToFrom)
                            {
                                var firstDayOfTheWeek = entry.date.firstDayOfTheWeek;
                                if (DateTime.Compare(firstDayOfTheWeek, intervalStart) < 0)
                                    resultInterval[intervalStart].value += entry.count;
                                else
                                    resultInterval[firstDayOfTheWeek].value += entry.count;
                            }
                        }

                    }
                }
                List<Dictionary<DateTime, ChartValue>> content = new List<Dictionary<DateTime, ChartValue>>();
                content.Add(resultInterval);
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepTotalSmsChart) }, PrepareJson(content, Resources.Global.RepSmsUnit));
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetTotalNoOfSmsChartSource", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIncomingOutgoingSmsChartSource(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);
                Dictionary<DateTime, ChartValue> resultIncomingInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultOutgoingInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                foreach (var wp in workingPoints)
                {
                    var conversations = from conv in wp.Conversations select conv;
                    foreach (var conv in conversations)
                    {
                        if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                        {
                            var msgsTo = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                            foreach (var entry in msgsTo)
                            {
                                resultIncomingInterval[entry.date].value += entry.count;
                            }
                            var msgsFrom = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                            foreach (var entry in msgsFrom)
                            {
                                resultOutgoingInterval[entry.date].value += entry.count;
                            }
                        }
                        else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                        {
                            var msgsTo = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) group msg by new { msg.TimeReceived.Month, msg.TimeReceived.Year } into g select new { date = g.Key, count = g.Count() };
                            var msgsFrom = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) group msg by new { msg.TimeReceived.Month, msg.TimeReceived.Year } into g select new { date = g.Key, count = g.Count() };
                            foreach (var entry in msgsTo)
                            {
                                var monthDateTime = new DateTime(entry.date.Year, entry.date.Month, 1);
                                if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                    resultIncomingInterval[intervalStart].value += entry.count;
                                else
                                    resultIncomingInterval[monthDateTime].value += entry.count;
                            }

                            foreach (var entry in msgsFrom)
                            {
                                var monthDateTime = new DateTime(entry.date.Year, entry.date.Month, 1);
                                if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                    resultOutgoingInterval[intervalStart].value += entry.count;
                                else
                                    resultOutgoingInterval[monthDateTime].value += entry.count;
                            }
                        }
                        else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                        {
                            var msgsTo = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) group msg by new { firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(msg.TimeReceived) } into g select new { date = g.Key, count = g.Count() };
                            var msgsFrom = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) group msg by new { firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(msg.TimeReceived) } into g select new { date = g.Key, count = g.Count() };
                            foreach (var entry in msgsTo)
                            {
                                var weekDateTime = entry.date.firstDayOfTheWeek;
                                if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                    resultIncomingInterval[intervalStart].value += entry.count;
                                else
                                    resultIncomingInterval[weekDateTime].value += entry.count;
                            }

                            foreach (var entry in msgsFrom)
                            {
                                var weekDateTime = entry.date.firstDayOfTheWeek;
                                if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                    resultOutgoingInterval[intervalStart].value += entry.count;
                                else
                                    resultOutgoingInterval[weekDateTime].value += entry.count;
                            }
                        }
                    }
                }

                List<Dictionary<DateTime, ChartValue>> content = new List<Dictionary<DateTime, ChartValue>>();
                content.Add(resultIncomingInterval);
                content.Add(resultOutgoingInterval);
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepIncomingSmsChart), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepOutgoingSmsChart) }, PrepareJson(content, Resources.Global.RepSmsUnit));
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetIncomingOutgoingSmsChartSource", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewVsReturningClientsChartSource(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                Dictionary<DateTime, ChartValue> resultNewClientsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultReturningClientsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);

                foreach (var wp in workingPoints)
                {
                    if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                    {
                        var newClients = from conv in wp.Conversations where conv.StartTime > intervalStart & conv.StartTime < intervalEnd group conv by conv.StartTime.Date into convGroup select new { date = convGroup.Key, count = convGroup.Count() };
                        foreach (var newClient in newClients)
                        {
                            resultNewClientsInterval[newClient.date].value += newClient.count;
                        }

                        var returningClients = from conv in wp.Conversations where conv.StartTime < intervalStart select (from msg in conv.Messages where msg.TimeReceived > intervalStart & msg.TimeReceived < intervalEnd group conv by new { msg.TimeReceived.Date } into convGroup select new { date = convGroup.Key, count = convGroup.Count() });
                        foreach (var conv in returningClients)
                        {
                            foreach (var day in conv)
                            {
                                DateTime currentDay = day.date.Date;
                                resultReturningClientsInterval[currentDay].value += 1;
                            }
                        }
                    }
                    else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                    {
                        var newClients = from conv in wp.Conversations where conv.StartTime > intervalStart & conv.StartTime < intervalEnd group conv by new { conv.StartTime.Month, conv.StartTime.Year } into convGroup select new { date = convGroup.Key, count = convGroup.Count() };
                        foreach (var entry in newClients)
                        {
                            var monthDateTime = new DateTime(entry.date.Year, entry.date.Month, 1);
                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                resultNewClientsInterval[intervalStart].value += entry.count;
                            else
                                resultNewClientsInterval[monthDateTime].value += entry.count;
                        }

                        var returningClients = from conv in wp.Conversations where conv.StartTime < intervalStart select (from msg in conv.Messages where msg.TimeReceived > intervalStart & msg.TimeReceived < intervalEnd group conv by new { msg.TimeReceived.Month, msg.TimeReceived.Year } into convGroup select new { date = convGroup.Key, count = convGroup.Count() });
                        foreach (var conv in returningClients)
                        {
                            foreach (var entry in conv)
                            {
                                var monthDateTime = new DateTime(entry.date.Year, entry.date.Month, 1);
                                if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                    resultReturningClientsInterval[intervalStart].value += 1;
                                else
                                    resultReturningClientsInterval[monthDateTime].value += 1;

                            }
                        }
                    }
                    else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                    {
                        var newClients = from conv in wp.Conversations where conv.StartTime >= intervalStart & conv.StartTime <= intervalEnd group conv by new { firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(conv.StartTime) } into convGroup select new { date = convGroup.Key, count = convGroup.Count() };
                        foreach (var entry in newClients)
                        {
                            var weekDateTime = entry.date.firstDayOfTheWeek;
                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                resultNewClientsInterval[intervalStart].value += entry.count;
                            else
                                resultNewClientsInterval[weekDateTime].value += entry.count;
                        }

                        var returningClients = from conv in wp.Conversations where conv.StartTime < intervalStart select (from msg in conv.Messages where msg.TimeReceived > intervalStart & msg.TimeReceived < intervalEnd group conv by new { firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(msg.TimeReceived) } into convGroup select new { date = convGroup.Key, count = convGroup.Count() });
                        foreach (var conv in returningClients)
                        {
                            foreach (var entry in conv)
                            {
                                var weekDateTime = entry.date.firstDayOfTheWeek;
                                if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                    resultReturningClientsInterval[intervalStart].value += 1;
                                else
                                    resultReturningClientsInterval[weekDateTime].value += 1;
                            }
                        }
                    }
                }

                List<Dictionary<DateTime, ChartValue>> content = new List<Dictionary<DateTime, ChartValue>>();
                content.Add(resultNewClientsInterval);
                content.Add(resultReturningClientsInterval);

                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNewClientsChart), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepReturningClientsChart) }, PrepareJson(content, Resources.Global.RepClientsUnit));
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetNewVsReturningClientsChartSource", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNoOfConversationsByTagsChartSource(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                var tagsHash = GetNoOfConversationsByTags(intervalStart, intervalEnd, iGranularity, scope);

                // Prepare Json result
                var columnCounter = 0;
                var tagInterval = intervalStart.ToShortDateString() + " - " + intervalEnd.ToShortDateString();
                var headerContent = new List<RepDataColumn>();
                var rowContent = new List<RepDataRowCell>();

                rowContent.Add(new RepDataRowCell(tagInterval, tagInterval));
                headerContent.Add(new RepDataColumn(columnCounter.ToString(), "string", "Date"));

                foreach (var tagEntry in tagsHash)
                {
                    ++columnCounter;
                    rowContent.Add(new RepDataRowCell(tagEntry.Value, tagEntry.Value.ToString() + " conversations"));
                    headerContent.Add(new RepDataColumn(columnCounter.ToString(), "number", tagEntry.Key));
                }

                RepChartData chartSource = new RepChartData(headerContent, new RepDataRow[] { new RepDataRow(rowContent) });
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetNoOfConversationsByTagsChartSource", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPosAndNegTagActivity(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);
                Dictionary<DateTime, ChartValue> resultPositiveTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegativeTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultRemovePositiveTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultRemoveNegativeTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        if (!conv.Client.isSupportClient)
                        {
                            if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                            {
                                // Test if conversation had activity in that period.
                                var allMsg = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
                                if (allMsg > 0)
                                {
                                    var convEvents = from convEvent in conv.ConversationEvents
                                                     where ((convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT)) &&
                                                     (convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd))
                                                     group convEvent by new { occurDate = convEvent.Date.Date, eventType = convEvent.EventTypeName }
                                                         into g
                                                         select new { key = g.Key, count = g.Count() };
                                    foreach (var convEvent in convEvents)
                                    {
                                        if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                                        {
                                            resultPositiveTagsInterval[convEvent.key.occurDate].value += convEvent.count;
                                        }
                                        else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
                                        {
                                            resultNegativeTagsInterval[convEvent.key.occurDate].value += convEvent.count;
                                        }
                                        else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                                        {
                                            resultRemovePositiveTagsInterval[convEvent.key.occurDate].value -= convEvent.count;
                                        }
                                        else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                                        {
                                            resultRemoveNegativeTagsInterval[convEvent.key.occurDate].value -= convEvent.count;
                                        }
                                    }

                                }
                            }
                            else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                            {
                                // Test if conversation had activity in that period.
                                var allMsg = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
                                if (allMsg > 0)
                                {
                                    var convEvents = from convEvent in conv.ConversationEvents
                                                     where ((convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT)) &&
                                                     (convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd))
                                                     group convEvent by new { Month = convEvent.Date.Month, Year = convEvent.Date.Year, eventType = convEvent.EventTypeName }
                                                         into g
                                                         select new { key = g.Key, count = g.Count() };
                                    foreach (var convEvent in convEvents)
                                    {
                                        var monthDateTime = new DateTime(convEvent.key.Year, convEvent.key.Month, 1);
                                        if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                                        {
                                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                                resultPositiveTagsInterval[intervalStart].value += convEvent.count;
                                            else
                                                resultPositiveTagsInterval[monthDateTime].value += convEvent.count;
                                        }
                                        else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT)) 
                                        {
                                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                                resultNegativeTagsInterval[intervalStart].value += convEvent.count;
                                            else
                                                resultNegativeTagsInterval[monthDateTime].value += convEvent.count;
                                        }
                                        else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                                        {
                                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                                resultRemovePositiveTagsInterval[intervalStart].value -= convEvent.count;
                                            else
                                                resultRemovePositiveTagsInterval[monthDateTime].value -= convEvent.count;
                                        }
                                        else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                                        {
                                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                                resultRemoveNegativeTagsInterval[intervalStart].value -= convEvent.count;
                                            else
                                                resultRemoveNegativeTagsInterval[monthDateTime].value -= convEvent.count;                                            
                                        }
                                    }
                                }
                            }
                            else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                            {
                                // Test if conversation had activity in that period.
                                var allMsg = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
                                if (allMsg > 0)
                                {
                                    var convEvents = from convEvent in conv.ConversationEvents
                                                     where ((convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT)) &&
                                                     (convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd))
                                                     group convEvent by new { firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(convEvent.Date), eventType = convEvent.EventTypeName }
                                                         into g
                                                         select new { key = g.Key, count = g.Count() };
                                    foreach (var convEvent in convEvents)
                                    {
                                        var weekDateTime = convEvent.key.firstDayOfTheWeek;
                                        if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                                        {
                                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                                resultPositiveTagsInterval[intervalStart].value += convEvent.count;
                                            else
                                                resultPositiveTagsInterval[weekDateTime].value += convEvent.count;
                                        }
                                        else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT)) 
                                        {
                                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                                resultNegativeTagsInterval[intervalStart].value += convEvent.count;
                                            else
                                                resultNegativeTagsInterval[weekDateTime].value += convEvent.count;
                                        }
                                        else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                                        {
                                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                                resultRemovePositiveTagsInterval[intervalStart].value -= convEvent.count;
                                            else
                                                resultRemovePositiveTagsInterval[weekDateTime].value -= convEvent.count;
                                        }
                                        else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                                        {
                                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                                resultRemoveNegativeTagsInterval[intervalStart].value -= convEvent.count;
                                            else
                                                resultRemoveNegativeTagsInterval[weekDateTime].value -= convEvent.count;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                List<Dictionary<DateTime, ChartValue>> content = new List<Dictionary<DateTime, ChartValue>>();
                content.Add(resultPositiveTagsInterval);
                content.Add(resultNegativeTagsInterval);
                content.Add(resultRemovePositiveTagsInterval);
                content.Add(resultRemovePositiveTagsInterval);
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("16", Constants.STRING_COLUMN_TYPE, "Date"), 
                    new RepDataColumn("17", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPosFeedbackAdded), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegFeedbackAdded),
                    new RepDataColumn("19", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPosFeedbackRemoved), 
                    new RepDataColumn("20", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegFeedbackRemoved) },
                    PrepareJson(content, Resources.Global.RepConversationsUnit));
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetPosAndNegTagActivity", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPosAndNegTagEvolution(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);
                Dictionary<DateTime, ChartValue> resultPositiveTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegativeTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                int posFeedback = 0;
                int negFeedback = 0;
                int posFeedbackEvolution, negFeedbackEvolution;
                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        if (!conv.Client.isSupportClient)
                        {
                            // Test if conversation had activity in that period.
                            var allMsg = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
                            if (allMsg > 0)
                            {
                                var pastEvents = (from convEvent in conv.ConversationEvents
                                                  where ((convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                  convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                  convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT) ||
                                                  convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT)) &&
                                                  convEvent.Date < intervalStart)
                                                  group convEvent by new { eventDate = convEvent.Date, eventType = convEvent.EventTypeName } into g
                                                  select new { key = g.Key, count = g.Count() }).OrderByDescending(c => c.key.eventDate);

                                if (pastEvents.Count() > 0)
                                {
                                    var pastEvent = pastEvents.First();
                                    if (pastEvent.key.eventType.Equals(Constants.POS_ADD_EVENT)) posFeedback += pastEvent.count;
                                    else if (pastEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT)) negFeedback += pastEvent.count;
                                    else if (pastEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT)) { negFeedback += pastEvent.count; }
                                    else if (pastEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT)) { posFeedback += pastEvent.count; }
                                }
                            }
                        }

                    }
                }
                posFeedbackEvolution = posFeedback;
                negFeedbackEvolution = negFeedback;
                List<EventCounter> allEvents = new List<EventCounter>();

                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        if (!conv.Client.isSupportClient)
                        {
                            var allMsg = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
                            if (allMsg > 0)
                            {
                                if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                                {

                                    var convEvents = (from convEvent in conv.ConversationEvents
                                                      where ((convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                      convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                      convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                      convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT) ||
                                                      convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT) ||
                                                      convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT)) &&
                                                      convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd)
                                                      group convEvent by new { occurDate = convEvent.Date.Date, eventType = convEvent.EventTypeName } into g
                                                      select new EventCounter(new Event(g.Key.occurDate, g.Key.eventType), g.Count()));
                                    if (convEvents.Count() > 0) allEvents.AddRange(convEvents.ToList<EventCounter>());
                                }
                                else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                                {
                                    var convEvents = from convEvent in conv.ConversationEvents
                                                     where ((convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT)) &&
                                                     convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd)
                                                     group convEvent by new { Month = convEvent.Date.Month, Year = convEvent.Date.Year, eventType = convEvent.EventTypeName } into g
                                                     select new EventCounter(new Event((new DateTime(g.Key.Year, g.Key.Month, 1).CompareTo(intervalStart) < 0 ? intervalStart : new DateTime(g.Key.Year, g.Key.Month, 1)), g.Key.eventType), g.Count());
                                    if (convEvents.Count() > 0) allEvents.AddRange(convEvents.ToList<EventCounter>());
                                }
                                else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                                {
                                    var convEvents = from convEvent in conv.ConversationEvents
                                                     where ((convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT)) &&
                                                     convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd)
                                                     group convEvent by new { firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(convEvent.Date), eventType = convEvent.EventTypeName } into g
                                                     select new EventCounter(new Event((g.Key.firstDayOfTheWeek.CompareTo(intervalStart) < 0 ? intervalStart : g.Key.firstDayOfTheWeek), g.Key.eventType), g.Count());
                                    if (convEvents.Count() > 0) allEvents.AddRange(convEvents.ToList<EventCounter>());
                                }
                            }
                        }
                    }
                }

                IEnumerable<EventCounter> allEventsOrdered = allEvents.OrderBy(c => c.eventItem.occurDate);
                foreach (var convEvent in allEventsOrdered)
                {
                    if (convEvent.eventItem.eventType.Equals(Constants.POS_ADD_EVENT)) posFeedbackEvolution += convEvent.counter;
                    else if (convEvent.eventItem.eventType.Equals(Constants.NEG_ADD_EVENT)) negFeedbackEvolution += convEvent.counter;
                    else if (convEvent.eventItem.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                    {
                        negFeedbackEvolution += convEvent.counter;
                        posFeedbackEvolution -= convEvent.counter;
                    }
                    else if (convEvent.eventItem.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                    {
                        posFeedbackEvolution += convEvent.counter;
                        negFeedbackEvolution -= convEvent.counter;
                    }
                    else if (convEvent.eventItem.eventType.Equals(Constants.POS_REMOVE_EVENT)) { posFeedbackEvolution -= convEvent.counter; }
                    else if (convEvent.eventItem.eventType.Equals(Constants.NEG_REMOVE_EVENT)) { negFeedbackEvolution -= convEvent.counter; }
                    resultNegativeTagsInterval[convEvent.eventItem.occurDate].value = negFeedbackEvolution;
                    resultNegativeTagsInterval[convEvent.eventItem.occurDate].changed = true;
                    resultPositiveTagsInterval[convEvent.eventItem.occurDate].value = posFeedbackEvolution;
                    resultPositiveTagsInterval[convEvent.eventItem.occurDate].changed = true;
                }

                if (!resultPositiveTagsInterval[intervalStart].changed)
                    resultPositiveTagsInterval[intervalStart].value = posFeedback;
                if (!resultNegativeTagsInterval[intervalStart].changed)
                    resultNegativeTagsInterval[intervalStart].value = negFeedback;
                if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                {
                    for (var i = intervalStart.AddDays(1); i < intervalEnd; i = i.AddDays(1))
                    {
                        if (!resultPositiveTagsInterval[i].changed)
                            resultPositiveTagsInterval[i].value = resultPositiveTagsInterval[i.AddDays(-1)].value;
                        if (!resultNegativeTagsInterval[i].changed)
                            resultNegativeTagsInterval[i].value = resultNegativeTagsInterval[i.AddDays(-1)].value;

                    }
                }
                else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                {
                    DateTime firstDayOfTheMonth = new DateTime(intervalStart.Year, intervalStart.Month, 1);
                    for (var i = firstDayOfTheMonth.AddMonths(1); i < intervalEnd; i = i.AddMonths(1))
                    {
                        if (i.Equals(firstDayOfTheMonth.AddMonths(1)))
                        {
                            if (!resultPositiveTagsInterval[i].changed)
                                resultPositiveTagsInterval[i].value = resultPositiveTagsInterval[intervalStart].value;
                            if (!resultNegativeTagsInterval[i].changed)
                                resultNegativeTagsInterval[i].value = resultNegativeTagsInterval[intervalStart].value;
                        }
                        else
                        {
                            if (!resultPositiveTagsInterval[i].changed)
                                resultPositiveTagsInterval[i].value = resultPositiveTagsInterval[i.AddMonths(-1)].value;
                            if (!resultNegativeTagsInterval[i].changed)
                                resultNegativeTagsInterval[i].value = resultNegativeTagsInterval[i.AddMonths(-1)].value;
                        }
                    }
                }
                else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                {

                    Calendar calendar = CultureInfo.CurrentUICulture.Calendar;
                    for (var i = calendar.AddWeeks(FirstDayOfWeekUtility.GetFirstDayOfWeek(intervalStart), 1); i < intervalEnd; i = calendar.AddWeeks(i, 1))
                    {
                        if (i.Equals(calendar.AddWeeks(FirstDayOfWeekUtility.GetFirstDayOfWeek(intervalStart), 1)))
                        {
                            if (!resultPositiveTagsInterval[i].changed)
                                resultPositiveTagsInterval[i].value = resultPositiveTagsInterval[intervalStart].value;
                            if (!resultNegativeTagsInterval[i].changed)
                                resultNegativeTagsInterval[i].value = resultNegativeTagsInterval[intervalStart].value;
                        }
                        else
                        {
                            if (!resultPositiveTagsInterval[i].changed)
                                resultPositiveTagsInterval[i].value = resultPositiveTagsInterval[calendar.AddWeeks(i, -1)].value;
                            if (!resultNegativeTagsInterval[i].changed)
                                resultNegativeTagsInterval[i].value = resultNegativeTagsInterval[calendar.AddWeeks(i, -1)].value;
                        }
                    }
                }

                List<Dictionary<DateTime, ChartValue>> content = new List<Dictionary<DateTime, ChartValue>>();
                content.Add(resultPositiveTagsInterval);
                content.Add(resultNegativeTagsInterval);
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPositiveFeedback), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegativeFeedback) }, 
                    PrepareJson(content, Resources.Global.RepConversationsUnit));
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetPosAndNegTagEvolution", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPosNegTransitions(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);
                Dictionary<DateTime, ChartValue> resultPosToNegTransitionsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegToPosTransitionsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        if (!conv.Client.isSupportClient)
                        {
                            if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                            {
                                // Test if conversation had activity in that period.
                                var allMsg = (from msg in conv.Messages
                                              where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd)
                                              select msg).Count();
                                if (allMsg > 0)
                                {
                                    var convEvents = from convEvent in conv.ConversationEvents
                                                     where ((convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT)) &&
                                                     (convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd))
                                                     group convEvent by new { occurDate = convEvent.Date.Date, eventType = convEvent.EventTypeName }
                                                         into g
                                                         select new { key = g.Key, count = g.Count() };
                                    foreach (var convEvent in convEvents)
                                    {
                                        if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                                        {
                                            resultPosToNegTransitionsInterval[convEvent.key.occurDate].value += convEvent.count;
                                        }
                                        else
                                        {
                                            resultNegToPosTransitionsInterval[convEvent.key.occurDate].value += convEvent.count;
                                        }
                                    }

                                }
                            }
                            else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                            {
                                // Test if conversation had activity in that period.
                                var allMsg = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
                                if (allMsg > 0)
                                {
                                    var convEvents = from convEvent in conv.ConversationEvents
                                                     where ((convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT)) &&
                                                     (convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd))
                                                     group convEvent by new { Month = convEvent.Date.Month, Year = convEvent.Date.Year, eventType = convEvent.EventTypeName }
                                                         into g
                                                         select new { key = g.Key, count = g.Count() };
                                    foreach (var convEvent in convEvents)
                                    {
                                        var monthDateTime = new DateTime(convEvent.key.Year, convEvent.key.Month, 1);
                                        if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                                        {
                                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                                resultPosToNegTransitionsInterval[intervalStart].value += convEvent.count;
                                            else
                                                resultPosToNegTransitionsInterval[monthDateTime].value += convEvent.count;
                                        }
                                        else
                                        {
                                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                                resultNegToPosTransitionsInterval[intervalStart].value += convEvent.count;
                                            else
                                                resultNegToPosTransitionsInterval[monthDateTime].value += convEvent.count;
                                        }
                                    }
                                }
                            }
                            else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                            {
                                // Test if conversation had activity in that period.
                                var allMsg = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
                                if (allMsg > 0)
                                {
                                    var convEvents = from convEvent in conv.ConversationEvents
                                                     where ((convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT) ||
                                                     convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT)) &&
                                                     (convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd))
                                                     group convEvent by new { firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(convEvent.Date), eventType = convEvent.EventTypeName }
                                                         into g
                                                         select new { key = g.Key, count = g.Count() };
                                    foreach (var convEvent in convEvents)
                                    {
                                        var weekDateTime = convEvent.key.firstDayOfTheWeek;
                                        if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                                        {
                                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                                resultPosToNegTransitionsInterval[intervalStart].value += convEvent.count;
                                            else
                                                resultPosToNegTransitionsInterval[weekDateTime].value += convEvent.count;
                                        }
                                        else
                                        {
                                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                                resultNegToPosTransitionsInterval[intervalStart].value += convEvent.count;
                                            else
                                                resultNegToPosTransitionsInterval[weekDateTime].value += convEvent.count;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                List<Dictionary<DateTime, ChartValue>> content = new List<Dictionary<DateTime, ChartValue>>();
                content.Add(resultNegToPosTransitionsInterval);
                content.Add(resultPosToNegTransitionsInterval);
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegToPosFeedback), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPosToNegFeedback) }, 
                    PrepareJson(content, Resources.Global.RepConversationsUnit));
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetPosAndNegTagActivity", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Second area info box sources

        public JsonResult GetTotalNoOfSmsInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);
                var totalNoOfSms = ComputeTotalNoOfSms(intervalStart, intervalEnd, workingPoints);

                return Json(new RepInfoBox(totalNoOfSms, Resources.Global.RepSmsUnit), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetTotalNoOfSmsInfo", e);
            }
            return Json(new RepInfoBox("Request failed", Resources.Global.RepSmsUnit), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIncomingSmsInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                var incomingNoOfSms = ComputeNoOfIncomingSms(intervalStart, intervalEnd, workingPoints);
                return Json(new RepInfoBox(incomingNoOfSms, Resources.Global.RepSmsUnit), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetIncomingSmsInfo", e);
            }
            return Json(new RepInfoBox("Request failed", Resources.Global.RepSmsUnit), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOutgoingSmsInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                var outgoingNoOfSms = ComputeNoOfOutgoingSms(intervalStart, intervalEnd, workingPoints);
                return Json(new RepInfoBox(outgoingNoOfSms, Resources.Global.RepSmsUnit), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetOutgoingSmsInfo", e);
            }
            return Json(new RepInfoBox("Request failed", Resources.Global.RepSmsUnit), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvgNoOfSmsPerDayInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                var totalNoOfSms = ComputeTotalNoOfSms(intervalStart, intervalEnd, workingPoints);
                TimeSpan interval = intervalEnd - intervalStart;

                RepInfoBox result = (interval.TotalDays == 0) ? new RepInfoBox(totalNoOfSms, Resources.Global.RepSmsPerDayUnit) :
                    new RepInfoBox(Math.Round(totalNoOfSms / interval.TotalDays, 2), Resources.Global.RepSmsPerDayUnit);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetAvgNoOfSmsPerDayInfo", e);
            }
            return Json(new RepInfoBox("Request failed", "sms"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvgNoOfIncomingSmsPerClientInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                int noOfClients = ComputeTotalNoOfClients(intervalStart, intervalEnd, workingPoints);
                int noOfIncomingMessages = ComputeNoOfIncomingSms(intervalStart, intervalEnd, workingPoints);

                RepInfoBox result = (noOfClients == 0) ? new RepInfoBox(0, Resources.Global.RepSmsPerClient) :
                    new RepInfoBox(Math.Round((double)noOfIncomingMessages / noOfClients, 2), Resources.Global.RepSmsPerClient);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetAvgNoOfIncomingSmsPerClientInfo", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvgNoOfOutgoingSmsPerClientInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                int noOfClients = ComputeTotalNoOfClients(intervalStart, intervalEnd, workingPoints);
                int noOfOutgoingMessages = ComputeNoOfOutgoingSms(intervalStart, intervalEnd, workingPoints);

                RepInfoBox result = (noOfClients == 0) ? new RepInfoBox(0, Resources.Global.RepSmsPerClient) :
                    new RepInfoBox(Math.Round((double)noOfOutgoingMessages / noOfClients, 2), Resources.Global.RepSmsPerClient);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetAvgNoOfOutgoingSmsPerClientInfo", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMostUsedTagsInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                var tagsHash = GetNoOfConversationsByTags(intervalStart, intervalEnd, iGranularity, scope);

                var mostUsedTags = new List<String>();
                var mostUsedTagNoOfConversations = -1;
                if (tagsHash.Count > 0)
                {
                    foreach (var tagEntry in tagsHash)
                    {
                        if (tagEntry.Value > mostUsedTagNoOfConversations)
                        {
                            mostUsedTagNoOfConversations = tagEntry.Value;
                            mostUsedTags = new List<String>();
                            mostUsedTags.Add(tagEntry.Key);
                        }
                        else if (tagEntry.Value >= mostUsedTagNoOfConversations)
                        {
                            mostUsedTagNoOfConversations = tagEntry.Value;
                            mostUsedTags.Add(tagEntry.Key);
                        }
                    }
                    return Json(new RepInfoBox(String.Join(", ", mostUsedTags), ""), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new RepInfoBox(Resources.Global.RepNoneDefaultValue, ""), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                logger.Error("GetMostUsedTagsInfo", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAvgNoOfTagsPerConversationInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                var noOfConversations = 0;
                var noOfTags = 0;
                foreach (var wp in workingPoints)
                {
                    var conversationsStartedInPeriod = from conv in wp.Conversations where conv.StartTime >= intervalStart && conv.StartTime <= intervalEnd select conv;
                    noOfConversations += conversationsStartedInPeriod.Count();
                    foreach (var conv in conversationsStartedInPeriod) noOfTags += conv.ConversationTags.Count;

                    var conversationStartedBefore = from conv in wp.Conversations where conv.StartTime < intervalStart select conv;
                    foreach (var conv in conversationStartedBefore)
                    {
                        var messagesInPeriod = from msg in conv.Messages where msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd select msg;
                        if (messagesInPeriod.Count() > 0)
                        {
                            ++noOfConversations;
                            noOfTags += conv.ConversationTags.Count();
                        }
                    }
                }

                RepInfoBox result = (noOfConversations == 0) ? new RepInfoBox(0, Resources.Global.RepTagsPerConversationUnit) :
                    result = new RepInfoBox(Math.Round((double)noOfTags / noOfConversations, 2), Resources.Global.RepTagsPerConversationUnit);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetAverageNoOfTagsPerConversationInfo", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNoOfNewClientsInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                int noOfNewClients = 0;
                foreach (var wp in workingPoints)
                {
                    noOfNewClients += (from conv in wp.Conversations where conv.StartTime >= intervalStart && conv.StartTime <= intervalEnd select conv).Count();
                }
                return Json(new RepInfoBox(noOfNewClients, Resources.Global.RepClients), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetNoOfNewClientsInfo", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTotalNoOfClientsInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                int noOfClients = ComputeTotalNoOfClients(intervalStart, intervalEnd, workingPoints);
                return Json(new RepInfoBox(noOfClients, Resources.Global.RepClients), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetTotalNoOfClientsInfo", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNoOfReturningClientsInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                int noOfReturningClients = 0;
                foreach (var wp in workingPoints)
                {
                    var conversationsStartedBefore = from conv in wp.Conversations where conv.StartTime < intervalStart select conv;
                    foreach (var conv in conversationsStartedBefore)
                    {
                        var noOfMessagesInThisPeriod = (from msg in conv.Messages where msg.TimeReceived > intervalStart & msg.TimeReceived < intervalEnd select msg).Count();
                        if (noOfMessagesInThisPeriod > 0) ++noOfReturningClients;
                    }
                }
                return Json(new RepInfoBox(noOfReturningClients, Resources.Global.RepClients), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetNoOfReturningClientsInfo", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvgNoOfSmsPerClientInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                int noOfClients = ComputeTotalNoOfClients(intervalStart, intervalEnd, workingPoints);
                int noOfMessages = ComputeTotalNoOfSms(intervalStart, intervalEnd, workingPoints);

                RepInfoBox result = (noOfClients == 0) ? new RepInfoBox(0, Resources.Global.RepSmsPerClient) :
                    new RepInfoBox(Math.Round((double)noOfMessages / noOfClients, 2), Resources.Global.RepSmsPerClient);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetAvgNoOfSmsPerClientInfo", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvgResponseTimeInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                long totalResponseTime = 0;
                var counter = 0;
                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        foreach (var msg in conv.Messages)
                        {
                            if (msg.ResponseTime.HasValue)
                            {
                                totalResponseTime += msg.ResponseTime.Value;
                                ++counter;
                            }
                        }
                    }
                }
                // Avoid division by 0
                TimeSpan avgResponseTime = (counter == 0) ? new TimeSpan(0) :
                    avgResponseTime = new TimeSpan((long)(totalResponseTime / counter));

                if (avgResponseTime.TotalMinutes < 1)
                {
                    return Json(new RepInfoBox(Math.Round(avgResponseTime.TotalSeconds, 2), Resources.Global.RepSecondsUnit), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new RepInfoBox(Math.Round(avgResponseTime.TotalMinutes, 2), Resources.Global.RepMinutesUnit), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                logger.Error("GetAvgResponseTimeInfo", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Third area sources

        public JsonResult GetIncomingOutgoingThirdArea(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                var incomingNoOfSms = ComputeNoOfIncomingSms(intervalStart, intervalEnd, workingPoints);
                var outgoingNoOfSms = ComputeNoOfOutgoingSms(intervalStart, intervalEnd, workingPoints);

                // Prepare Json result
                var row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(Resources.Global.RepIncomingSmsChart, Resources.Global.RepIncomingSmsChart), new RepDataRowCell(incomingNoOfSms, incomingNoOfSms + " sms") });
                var row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(Resources.Global.RepOutgoingSmsChart, Resources.Global.RepOutgoingSmsChart), new RepDataRowCell(outgoingNoOfSms, outgoingNoOfSms + " sms") });
                List<RepDataRow> content = new List<RepDataRow>();
                content.Add(row1);
                content.Add(row2);
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, Resources.Global.RepTypeTable), new RepDataColumn("18", Constants.STRING_COLUMN_TYPE, Resources.Global.RepValueTable) }, content);
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetIncomingOutgoingThirdArea", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);

        }

        #endregion

        public JsonResult GetReportsMenuItems()
        {
            ReportsMenuItem[] reportsMenuItems = new ReportsMenuItem[] { new ReportsMenuItem(1, Resources.Global.RepConversations, false, 0), new ReportsMenuItem(2, Resources.Global.RepOverview, true, 1), 
                                                                         new ReportsMenuItem(3, Resources.Global.RepIncomingVsOutgoing, true, 1), new ReportsMenuItem(4, Resources.Global.RepPositiveAndNegative, true, 1),
                                                                         new ReportsMenuItem(5, Resources.Global.RepTags, true, 1), new ReportsMenuItem(6, Resources.Global.RepClients, false, 0), new ReportsMenuItem(7, Resources.Global.RepNewVsReturning, true, 6), 
                                                                        };
            // new ReportsMenuItem(7, Resources.Global.RepNewVsReturning , true, 5)
            return Json(reportsMenuItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportById(int reportId)
        {
            var hashTable = new Dictionary<int, Report>();
            var report2 = new Report(2, Resources.Global.RepOverview, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource(Resources.Global.RepOverviewChartTitle, iSource: "/Reports/GetTotalNoOfSmsChartSource") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "/Reports/GetTotalNoOfSmsInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepTotalNoOfSmsTooltip),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfSmsPerDay, iSource: "/Reports/GetAvgNoOfSmsPerDayInfo",
                                                                                                                                                    iTooltip: Resources.Global.RepAvgNoOfSmsPerDayTooltip),
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfClients, iSource: "/Reports/GetTotalNoOfClientsInfo",
                                                                                                                                                    iTooltip: Resources.Global.RepTotalNoOfClientsTooltip),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfSmsPerClient, iSource: "/Reports/GetAvgNoOfSmsPerClientInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepAvgNoOfSmsPerClientTooltip),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgResponseTime, iSource: "/Reports/GetAvgResponseTimeInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepAvgResponseTimeTooltip)
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetIncomingOutgoingThirdArea") 
                                                                                                                                                          }),
                                                                                    });
            var report3 = new Report(3, Resources.Global.RepIncomingVsOutgoing, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource(Resources.Global.RepIncomingOutgoingChartTitle, iSource: "/Reports/GetIncomingOutgoingSmsChartSource") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfIncomingSms, iSource: "/Reports/GetIncomingSmsInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepNoOfIncomingSmsTooltip),  
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfOutgoingSms, iSource: "/Reports/GetOutgoingSmsInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepNoOfOutgoingSmsTooltip),
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfClients, iSource: "/Reports/GetTotalNoOfClientsInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepTotalNoOfClientsTooltip),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfIncomingSmsPerConversation, iSource: "/Reports/GetAvgNoOfIncomingSmsPerClientInfo",
                                                                                                                                                    iTooltip: Resources.Global.RepAvgNoOfIncomingSmsPerConversationToolitp),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfOutgoingSmsPerConversation, iSource: "/Reports/GetAvgNoOfOutgoingSmsPerClientInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepAvgNoOfOutgoingSmsPerConversationTooltip)
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetIncomingOutgoingThirdArea") 
                                                                                                                                                          }),
                                                                                    });
            var report4 = new Report(4, Resources.Global.RepPositiveAndNegativeTitle, "Global", new ReportSection[] { 
                                                                                        
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource(Resources.Global.RepPositiveNegativeEvolutionChartTitle, iSource: "/Reports/GetPosAndNegTagEvolution")                                                                                                                                                                                                                                                     
                                                                                                                                                            }),
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource(Resources.Global.RepPositiveNegativeTransitionsChartTitle, iSource: "/Reports/GetPosNegTransitions") 
                                                                                                                                                            }),                                                                                        
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource(Resources.Global.RepPositiveNegativeActivityChartTitle, iSource: "/Reports/GetPosAndNegTagActivity", iOptions: new ReportResourceOptions(iColors: new List<String>{"#3366cc", "#dc3912", "#667189", "#b48479"})) 
                                                                                                                                                                                                                                                                                                                        
                                                                                                                                                            }),
                                                                                        new ReportSection("InfoBox", false, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepMostUsedTag, iSource: "/Reports/GetMostUsedTagsInfo",
                                                                                                                                                    iTooltip: Resources.Global.RepMostUsedTagsTooltip),     
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfTagsPerConversation, iSource: "/Reports/GetAvgNoOfTagsPerConversationInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepAvgNoOfTagsPerConversationTooltip)
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report5 = new Report(5, Resources.Global.RepTags, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource(Resources.Global.RepNoOfConversationsByTagsChartTitle, iSource: "/Reports/GetNoOfConversationsByTagsChartSource", iOptions: new ReportResourceOptions(iSeriesType : Constants.BARS_CHART_STYLE)) 
                                                                                                                                                            
                                                                                                                                                             
                                                                                                                                                            }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepMostUsedTag, iSource: "/Reports/GetMostUsedTagsInfo",
                                                                                                                                                    iTooltip: Resources.Global.RepMostUsedTagsTooltip),     
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfTagsPerConversation, iSource: "/Reports/GetAvgNoOfTagsPerConversationInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepAvgNoOfTagsPerConversationTooltip)
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report7 = new Report(7, Resources.Global.RepNewVsReturning, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource(Resources.Global.RepNewReturningClientsChartTitle, iSource: "/Reports/GetNewVsReturningClientsChartSource") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfClients, iSource: "/Reports/GetTotalNoOfClientsInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepTotalNoOfClients),
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfNewClients, iSource: "/Reports/GetNoOfNewClientsInfo", 
                                                                                                                                                    iTooltip: Resources.Global.RepNoOfNewClients),
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfReturningClients, iSource: "/Reports/GetNoOfReturningClientsInfo",
                                                                                                                                                    iTooltip: Resources.Global.RepNoOfReturningClientsTooltip)
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });

            /* out
         var report7 = new Report(7, Resources.Global.RepNewVsReturning, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms with granularity", iSource: "/Reports/GetSmsIncomingOutgoingDetailed") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "/Reports/GetSmsTotalInfo"),                                                                                                                                                    
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
             */
            hashTable.Add(2, report2);
            hashTable.Add(3, report3);
            hashTable.Add(4, report4);
            hashTable.Add(5, report5);
            hashTable.Add(7, report7);
            //hashTable.Add(7, report7);

            return Json(hashTable[reportId], JsonRequestBehavior.AllowGet);

        }

        public List<RepDataRow> BogusDataGenerator(int intervalEnd, string workingPoint)
        {

            var hashTable = new Dictionary<String, RepDataRow>();
            var keyPrefix = "row";

            int[] incomingSmsGlobal = new int[31] { 17, 15, 16, 15, 17, 16, 14, 13, 14, 15, 16, 17, 15, 14, 11, 13, 16, 15, 14, 13, 13, 12, 11, 12, 13, 14, 15, 16, 15, 16, 14 };
            int[] incomingSmsWP1 = new int[31] { 18, 19, 21, 22, 21, 18, 16, 19, 20, 21, 23, 25, 26, 27, 27, 29, 28, 27, 25, 24, 23, 20, 19, 19, 21, 23, 24, 27, 30, 31, 32 };
            int[] incomingSmsWP2 = new int[31] { 23, 19, 18, 17, 15, 12, 11, 9, 8, 7, 11, 10, 11, 13, 15, 16, 17, 17, 19, 18, 23, 25, 24, 23, 20, 19, 19, 21, 23, 24, 27 };

            int[] outgoingSmsGlobal = new int[31] { 15, 14, 15, 15, 14, 15, 13, 12, 13, 14, 15, 16, 14, 13, 10, 12, 15, 14, 13, 12, 12, 11, 10, 11, 12, 13, 14, 15, 14, 15, 13 };
            int[] outgoingSmsWP1 = new int[31] { 17, 18, 20, 21, 20, 17, 15, 18, 19, 18, 17, 16, 15, 14, 13, 15, 14, 11, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13 };
            int[] outgoingSmsWP2 = new int[31] { 22, 18, 17, 16, 14, 11, 10, 8, 7, 6, 10, 9, 10, 12, 14, 15, 16, 16, 18, 17, 22, 24, 23, 22, 19, 18, 18, 20, 22, 23, 26 };

            int[] incoming = incomingSmsWP2;
            int[] outgoing = outgoingSmsWP2;

            if (workingPoint.Equals("Global"))
            {
                incoming = incomingSmsGlobal;
                outgoing = outgoingSmsGlobal;
            }
            else if (workingPoint.Equals("WP1"))
            {
                incoming = incomingSmsWP1;
                outgoing = outgoingSmsWP1;
            }
            else
            {
                incoming = incomingSmsWP2;
                outgoing = outgoingSmsWP2;
            }

            for (int i = 0; i < intervalEnd; ++i)
            {
                var key = keyPrefix + i;
                var bogusData = i + ".07.2012";
                // Incoming
                var bogusIncomingSmsNo = incoming[i];
                var bogusIncomingSmsText = bogusIncomingSmsNo + " sms - " + bogusData;
                //Outgoing 
                var bogusOutgoingSmsNo = outgoing[i];
                var bogusOutgoingSmsText = bogusOutgoingSmsNo + " sms - " + bogusData;
                hashTable.Add(key, new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(bogusData, bogusData), new RepDataRowCell(bogusIncomingSmsNo, bogusIncomingSmsText), new RepDataRowCell(bogusOutgoingSmsNo, bogusOutgoingSmsText) }));
            }

            List<RepDataRow> content = new List<RepDataRow>();
            for (int i = 0; i < intervalEnd; ++i)
            {
                var key = "row" + i;
                content.Add(hashTable[key]);
            }

            return content;
        }

        #region Helper methods
        private Dictionary<String, int> GetNoOfConversationsByTags(DateTime iIntervalStart, DateTime iIntervalEnd, String iGranularity, String scope)
        {
            try
            {
                smsfeedbackEntities dbContext = new smsfeedbackEntities();
                IEnumerable<WorkingPoint> workingPoints = mEFInterface.GetWorkingPointsForAUser(scope, User.Identity.Name, dbContext);

                var tagsHash = new Dictionary<String, int>();
                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        var noOfMessagesInThisConversation = (from msg in conv.Messages where (msg.TimeReceived >= iIntervalStart & msg.TimeReceived <= iIntervalEnd) select msg).Count();
                        if (noOfMessagesInThisConversation > 0)
                        {
                            foreach (var convTag in conv.ConversationTags)
                            {

                                if (tagsHash.ContainsKey(convTag.Tag.Name))
                                {
                                    tagsHash[convTag.Tag.Name] += 1;
                                }
                                else
                                {
                                    tagsHash.Add(convTag.Tag.Name, 1);
                                }
                            }
                        }
                    }

                }
                return tagsHash;
            }
            catch (Exception e)
            {
                logger.Error("GetNoOfConversationsByTagsChartSource", e);
            }
            return null;
        }

        private String transformDate(DateTime iDate, String pattern)
        {
            // TODO: Look for a library to convert to different local formats
            var transformedDate = "";
            if (pattern.Equals("dd-mm"))
            {
                var day = (iDate.Day < 10) ? "0" + iDate.Day.ToString() : iDate.Day.ToString();
                var month = (iDate.Month < 10) ? "0" + iDate.Month.ToString() : iDate.Month.ToString();
                transformedDate = day + "-" + month;
            }
            else if (pattern.Equals("dd/mm/yyyy"))
            {
                var day = (iDate.Day < 10) ? "0" + iDate.Day.ToString() : iDate.Day.ToString();
                var month = (iDate.Month < 10) ? "0" + iDate.Month.ToString() : iDate.Month.ToString();
                transformedDate = day + "/" + month + "/" + iDate.Year;
            }
            else if (pattern.Equals("dd/mm"))
            {
                var day = (iDate.Day < 10) ? "0" + iDate.Day.ToString() : iDate.Day.ToString();
                var month = (iDate.Month < 10) ? "0" + iDate.Month.ToString() : iDate.Month.ToString();
                transformedDate = day + "/" + month;
            }
            else
            {
                // transform to "dd-mm-yyyy" pattern
                var day = (iDate.Day < 10) ? "0" + iDate.Day.ToString() : iDate.Day.ToString();
                var month = (iDate.Month < 10) ? "0" + iDate.Month.ToString() : iDate.Month.ToString();
                transformedDate = day + "-" + month + "-" + iDate.Year;
            }
            return transformedDate;
        }

        private int ComputeTotalNoOfSms(DateTime iIntervalStart, DateTime iIntervalEnd, IEnumerable<WorkingPoint> iWorkingPoints)
        {
            var totalNoOfSms = 0;
            foreach (var wp in iWorkingPoints)
            {
                foreach (var conv in wp.Conversations)
                {
                    totalNoOfSms += (from msg in conv.Messages where (msg.TimeReceived >= iIntervalStart & msg.TimeReceived <= iIntervalEnd) select msg).Count();
                }
            }
            return totalNoOfSms;
        }

        private int ComputeTotalNoOfClients(DateTime iIntervalStart, DateTime intervalEnd, IEnumerable<WorkingPoint> iWorkingPoints)
        {
            int noOfClients = 0;
            foreach (var wp in iWorkingPoints)
            {
                noOfClients += (from conv in wp.Conversations where conv.StartTime >= iIntervalStart && conv.StartTime <= intervalEnd select conv).Count();
                var conversationStartedBefore = from conv in wp.Conversations where conv.StartTime < iIntervalStart select conv;
                foreach (var conv in conversationStartedBefore)
                {
                    noOfClients += ((from msg in conv.Messages where msg.TimeReceived >= iIntervalStart & msg.TimeReceived <= intervalEnd select msg).Count() > 0) ? 1 : 0;
                }
            }
            return noOfClients;
        }

        public int ComputeNoOfIncomingSms(DateTime iIntervalStart, DateTime iIntervalEnd, IEnumerable<WorkingPoint> iWorkingPoints)
        {
            var incomingNoOfSms = 0;
            foreach (var wp in iWorkingPoints)
            {
                foreach (var conv in wp.Conversations)
                {
                    var msgsTo = (from msg in conv.Messages where (msg.TimeReceived >= iIntervalStart & msg.TimeReceived <= iIntervalEnd & msg.To == wp.TelNumber) select msg).Count();
                    incomingNoOfSms += msgsTo;
                }
            }
            return incomingNoOfSms;
        }

        public int ComputeNoOfOutgoingSms(DateTime iIntervalStart, DateTime iIntervalEnd, IEnumerable<WorkingPoint> iWorkingPoints)
        {
            var outgoingNoOfSms = 0;
            foreach (var wp in iWorkingPoints)
            {
                var conversations = from conv in wp.Conversations select conv;
                foreach (var conv in conversations)
                {
                    var msgsFrom = (from msg in conv.Messages where (msg.TimeReceived >= iIntervalStart & msg.TimeReceived <= iIntervalEnd & msg.From == wp.TelNumber) select msg).Count();
                    outgoingNoOfSms += msgsFrom;
                }
            }
            return outgoingNoOfSms;
        }

        public Dictionary<DateTime, ChartValue> InitializeInterval(DateTime intervalStart, DateTime intervalEnd, string iGranularity)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar calendar = dfi.Calendar;
            Dictionary<DateTime, ChartValue> resultInterval = null;

            if (iGranularity.Equals(Constants.DAY_GRANULARITY))
            {
                resultInterval = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => new ChartValue(0, transformDate(d, "dd/mm")));
            }
            else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
            {
                resultInterval = new Dictionary<DateTime, ChartValue>();
                for (DateTime i = intervalStart; i < intervalEnd; i = i.AddMonths(1))
                {
                    var currentDate = (DateTime.Compare(new DateTime(i.Year, i.Month, 1), intervalStart) <= 0) ? intervalStart : new DateTime(i.Year, i.Month, 1);
                    var endOfTheMonth = (DateTime.Compare(new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month)), intervalEnd) > 0) ?
                                                                    intervalEnd : new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
                    resultInterval.Add(currentDate, new ChartValue(0, transformDate(currentDate, "dd/mm/yyyy") + " » " + transformDate(endOfTheMonth, "dd/mm/yyyy")));
                    i = (DateTime.Compare(i, intervalStart) == 0) ? new DateTime(i.Year, i.Month, 1) : i;
                }
            }
            else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
            {
                resultInterval = new Dictionary<DateTime, ChartValue>();
                for (DateTime i = intervalStart; i < intervalEnd; i = calendar.AddWeeks(i, 1))
                {
                    var currentDate = (DateTime.Compare(FirstDayOfWeekUtility.GetFirstDayOfWeek(i), intervalStart) <= 0) ? intervalStart : FirstDayOfWeekUtility.GetFirstDayOfWeek(i);
                    var lastDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(i).AddDays(6);
                    var endOfTheWeek = (DateTime.Compare(lastDayOfTheWeek, intervalEnd) > 0) ?
                                                                    intervalEnd : lastDayOfTheWeek;
                    resultInterval.Add(currentDate, new ChartValue(0, transformDate(currentDate, "dd/mm/yyyy") + " » " + transformDate(endOfTheWeek, "dd/mm/yyyy")));
                    // executed just first time
                    i = (DateTime.Compare(i, intervalStart) == 0) ? FirstDayOfWeekUtility.GetFirstDayOfWeek(i) : i;
                }
            }

            return resultInterval;
        }

        public List<RepDataRow> PrepareJson(List<Dictionary<DateTime, ChartValue>> source, String unitOfMeasurement)
        {
            List<RepDataRow> content = new List<RepDataRow>();
            var rowsTable = new Dictionary<DateTime, List<RepDataRowCell>>();

            foreach (var row in source.First())
            {
                if (rowsTable.ContainsKey(row.Key))
                {
                    rowsTable[row.Key].Add(new RepDataRowCell(row.Value.description, row.Value.description));
                }
                else
                {
                    rowsTable.Add(row.Key, new List<RepDataRowCell>());
                    rowsTable[row.Key].Add(new RepDataRowCell(row.Value.description, row.Value.description));
                }
            }

            foreach (var column in source)
            {
                foreach (var row in column)
                {
                    if (rowsTable.ContainsKey(row.Key))
                    {
                        rowsTable[row.Key].Add(new RepDataRowCell(row.Value.value, row.Value.value + " " + unitOfMeasurement));
                    }
                }
            }

            foreach (var row in rowsTable)
            {
                var currentRow = new List<RepDataRowCell>();
                foreach (var rowCell in row.Value)
                {
                    currentRow.Add(rowCell);
                }
                content.Add(new RepDataRow(currentRow));
            }

            return content;
        }

        #endregion
    }

    class EventCounter
    {
        public Event eventItem { get; set; }
        public int counter { get; set; }

        public EventCounter(Event iEventItem, int iCounter)
        {
            eventItem = iEventItem;
            counter = iCounter;
        }
    }

    class Event
    {
        public DateTime occurDate { get; set; }
        public String eventType { get; set; }

        public Event(DateTime iOccurDate, String iEventType)
        {
            occurDate = iOccurDate;
            eventType = iEventType;
        }
    }
}