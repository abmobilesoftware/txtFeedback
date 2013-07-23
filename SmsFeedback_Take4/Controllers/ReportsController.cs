using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Models.Helpers;
using SmsFeedback_EFModels;
using System.Linq.Expressions;
using LinqKit;
using SmsFeedback_Take4.Utilities;


namespace SmsFeedback_Take4.Controllers
{
    [CustomAuthorizeAtribute]
    public class ReportsController : BaseController
    {
       #region Export raw data
       public const string cExporterOfRawData = "ExportRawData";
       #endregion
        private const int cConvsMenuContainerID = 10;
        private const int cConvsOverviewMenuID = 11;
        private const int cConvsIncomingVsOutgoingID = 12;
        private const int cConvsPosVsNegID = 13;
        private const int cConvsTagsOverviewID = 14;
        private const int cClientsOverviewID = 20;
        private const int cClientsNewVsReturningID = 21;
        private const int cSectionID1 = 1;
        private const int cSectionID2 = 2;
        private const int cSectionID3 = 3;
        private const int cSectionID4 = 4;
        private const int cSectionID5 = 5;
        private const int cSectionID6 = 6;

        private const String cDateFormat = "yyyy-MM-dd H:mm:ss";
        private const String cDateFormat1 = "dd-mm";
        private const String cDateFormat2 = "dd/mm/yyyy";
        private const String cDateFormat3 = "dd/mm";
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private EFInteraction mEFInterface = new EFInteraction();
        smsfeedbackEntities context = new smsfeedbackEntities();

        public ActionResult Index()
        {
            ViewData["currentCulture"] = getCurrentCulture();
            ViewBag.DefaultFilterTag = GetDefaultTagForTagReport(context);
            return View();
        }

        #region Overview
        public JsonResult GetReportOverviewData(String iIntervalStart, String iIntervalEnd, Dictionary<string, string> dataToBeSaved, String[] iScope = null)
        {
           try
           {
              DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
              DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);

              iScope = iScope != null ? iScope : new string[0];
              Dictionary<DateTime, ChartValue> resultInterval = InitializeInterval(intervalStart, intervalEnd, Constants.DAY_GRANULARITY);

              IEnumerable<Message> msgs = GetMessages(intervalStart, intervalEnd, User.Identity.Name, iScope, context);

              var msgsGrByDay = from msg in msgs
                                group msg by new { msg.TimeReceived.Date } into g
                                select new { date = g.Key, count = g.Count() };
              foreach (var entry in msgsGrByDay)
                 resultInterval[entry.date.Date].value = entry.count;

              TimeSpan interval = intervalEnd - intervalStart;
              Int32 noOfClients = msgs.GroupBy(msg => msg.ConversationId).Count();
              Int32 totalNoOfMsgs = 0;
              long totalResponseTime = 0;
              var counter = 0;
              foreach (var msg in msgs)
              {
                 ++totalNoOfMsgs;
                 if (!msg.Conversation.Client.isSupportClient && msg.ResponseTime.HasValue)
                 {
                    totalResponseTime += msg.ResponseTime.Value;
                    ++counter;
                 }
              }

              TimeSpan avgResponseTime = (counter == 0) ? new TimeSpan(0) :
                  avgResponseTime = new TimeSpan((long)(totalResponseTime / counter));

              List<Dictionary<DateTime, ChartValue>> chartOverviewContent = new List<Dictionary<DateTime, ChartValue>>();
              chartOverviewContent.Add(resultInterval);
              RepChartData chartContentWrapper = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("17", Constants.STRING_COLUMN_TYPE), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepTotalSmsChart) 
                }, PrepareJson(chartOverviewContent, Resources.Global.RepSmsUnit));
              RepInfoBox ibTotalNoOfSms = new RepInfoBox(totalNoOfMsgs, Resources.Global.RepSmsUnit);
              RepInfoBox ibAvgNoOfSmsPerDay = (interval.TotalDays == 0) ? new RepInfoBox(totalNoOfMsgs, Resources.Global.RepSmsPerDayUnit) :
                  new RepInfoBox(Math.Round(totalNoOfMsgs / interval.TotalDays, 2), Resources.Global.RepSmsPerDayUnit);
              RepInfoBox ibTotalNoOfClients = new RepInfoBox(noOfClients, Resources.Global.RepClientsUnit);
              RepInfoBox ibAvgNoOfSmsPerClient = (noOfClients == 0) ?
                  new RepInfoBox(0, Resources.Global.RepSmsPerClient) :
                  new RepInfoBox(Math.Round((double)totalNoOfMsgs / noOfClients, 2), Resources.Global.RepSmsPerClient);
              RepInfoBox ibAvgResponseTime = (avgResponseTime.TotalMinutes < 2) ?
                  new RepInfoBox(Math.Round(avgResponseTime.TotalSeconds, 2), Resources.Global.RepSecondsUnit)
                  : (avgResponseTime.TotalMinutes < 120) ? new RepInfoBox(Math.Round(avgResponseTime.TotalMinutes, 2), Resources.Global.RepMinutesUnit)
                  : (avgResponseTime.TotalHours < 48) ? new RepInfoBox(Math.Round(avgResponseTime.TotalHours, 2), Resources.Global.RepHoursUnit)
                  : new RepInfoBox(Math.Round(avgResponseTime.TotalDays, 2), Resources.Global.RepDaysUnit);

              new RepInfoBox(Math.Round(avgResponseTime.TotalHours, 2), Resources.Global.RepMinutesUnit);

              var sideBySide = OverviewSideBySideInternal(intervalStart, intervalEnd, Constants.DAY_GRANULARITY, ref iScope);
              var repData = new ReportData(new List<RepChartData>() { sideBySide, chartContentWrapper },
              new List<RepInfoBox>() { ibTotalNoOfSms, ibAvgNoOfSmsPerDay,
                        ibTotalNoOfClients, ibAvgNoOfSmsPerClient, ibAvgResponseTime });
              var result = new { repData = repData, restoreData = dataToBeSaved };
              return Json(result, JsonRequestBehavior.AllowGet);
           }
           catch (Exception e)
           {
              logger.Error("GetReportOverviewData", e);
           }
           return Json("Request failed", JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetReportOverviewGrData(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
                 

                Dictionary<DateTime, ChartValue> resultInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                iScope = iScope != null ? iScope : new string[0];
                IEnumerable<Message> msgs = GetMessages(intervalStart, intervalEnd, User.Identity.Name, iScope, context);

                if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                {
                    var msgsToFrom = from msg in msgs
                                     group msg by new { msg.TimeReceived.Date } into g
                                     select new { date = g.Key, count = g.Count() };
                    foreach (var entry in msgsToFrom)
                        resultInterval[entry.date.Date].value = entry.count;
                }
                else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                {
                    var msgsToFrom = from msg in msgs
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
                    var msgsToFrom = from msg in msgs
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

                List<Dictionary<DateTime, ChartValue>> overviewChartContent = new List<Dictionary<DateTime, ChartValue>>();
                overviewChartContent.Add(resultInterval);
                RepChartData chartContentWrapper = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("17", Constants.STRING_COLUMN_TYPE), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepTotalSmsChart)
                }, PrepareJson(overviewChartContent, Resources.Global.RepSmsUnit));
                return Json(chartContentWrapper, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetReportOverviewDataGr", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportOverviewGrDataSideBySide(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
        {
           try
           {
              DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
              DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
              RepChartData chartSource = OverviewSideBySideInternal(intervalStart, intervalEnd, iGranularity, ref iScope);
              return Json(chartSource, JsonRequestBehavior.AllowGet);
           }
           catch (Exception e)
           {
              logger.Error("GetReportOverviewDataGr", e);
           }
           return Json("Request failed", JsonRequestBehavior.AllowGet);
        }

        private RepChartData OverviewSideBySideInternal(DateTime intervalStart, DateTime intervalEnd, String iGranularity, ref String[] iScope)
        {
           //DA the convention is that no scope equals all is in scope
           string userName = User.Identity.Name;
           if (iScope.Length == 0)
           {
              iScope = (from u in context.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp.TelNumber)).SelectMany(x => x).ToArray();
           }
           var resIntervals = new Dictionary<string, Dictionary<DateTime, ChartValue>>();           
           foreach (var location in iScope)
           {
              resIntervals.Add(location, InitializeInterval(intervalStart, intervalEnd, iGranularity));
           }
     
         
           Dictionary<string, IEnumerable<Message>> msgs = GetMessagesSideBySide(intervalStart, intervalEnd, userName, iScope, context);

           if (iGranularity.Equals(Constants.DAY_GRANULARITY))
           {
              foreach (var gr in msgs)
              {
                 var msgsToFrom = from msg in gr.Value
                                  group msg by new { msg.TimeReceived.Date } into g
                                  select new { date = g.Key, count = g.Count() };
                 foreach (var entry in msgsToFrom)
                 {
                    resIntervals[gr.Key][entry.date.Date].value = entry.count;
                 }
              }
           }
           else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
           {
              foreach (var gr in msgs)
              {
                 var msgsToFrom = from msg in gr.Value
                                  group msg by new { msg.TimeReceived.Month, msg.TimeReceived.Year } into g
                                  select new { date = g.Key, count = g.Count() };
                 foreach (var entry in msgsToFrom)
                 {
                    var monthDateTime = new DateTime(entry.date.Year, entry.date.Month, 1);
                    if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                       resIntervals[gr.Key][intervalStart].value += entry.count;
                    else
                       resIntervals[gr.Key][monthDateTime].value += entry.count;
                 }
              }
           }
           else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
           {
              foreach (var gr in msgs)
              {
                 var msgsToFrom = from msg in gr.Value
                                  group msg by new { firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(msg.TimeReceived) } into g
                                  select new { date = g.Key, count = g.Count() };
                 foreach (var entry in msgsToFrom)
                 {
                    var firstDayOfTheWeek = entry.date.firstDayOfTheWeek;
                    if (DateTime.Compare(firstDayOfTheWeek, intervalStart) < 0)
                       resIntervals[gr.Key][intervalStart].value += entry.count;
                    else
                       resIntervals[gr.Key][firstDayOfTheWeek].value += entry.count;
                 }
              }
           }

           List<Dictionary<DateTime, ChartValue>> overviewChartContent = new List<Dictionary<DateTime, ChartValue>>();
           foreach (var item in resIntervals)
           {
              overviewChartContent.Add(item.Value);
           }
           List<RepDataColumn> columnDefinition = new List<RepDataColumn> { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date") };
           foreach (var location in iScope)
           {
              columnDefinition.Add(new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, location));

           }
           RepChartData chartSource = new RepChartData(
              columnDefinition,
                 PrepareJson(overviewChartContent, Resources.Global.RepSmsUnit));
           return chartSource;
        }
        #endregion

        #region Incoming vs Outgoing
        public JsonResult GetReportIncomingOutgoingData(String iIntervalStart, String iIntervalEnd, String[] iScope,Dictionary<string, string> dataToBeSaved)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
                 

                Dictionary<DateTime, ChartValue> resultIncomingInterval = InitializeInterval(intervalStart, intervalEnd, Constants.DAY_GRANULARITY);
                Dictionary<DateTime, ChartValue> resultOutgoingInterval = InitializeInterval(intervalStart, intervalEnd, Constants.DAY_GRANULARITY);

                iScope = iScope != null ? iScope : new string[0];
                var msgs = GetMessages(intervalStart, intervalEnd, User.Identity.Name, iScope, context);
                // Group messages by day and type incoming or outgoing
                var msgsGrouped = from msg in GroupIncomingOutgoingMsgs(intervalStart, 
                                      intervalEnd, 
                                      iScope, 
                                      context) 
                                  group msg by new { 
                                      msg.Key.TimeReceived.Date, 
                                      incoming = msg.Key.incoming 
                                  } into g 
                                  select new { g.Key, count = g.Count() };
                Int32 noOfIncomingMsgs = 0;
                Int32 noOfOutgoingMsgs = 0;
                foreach (var msg in msgsGrouped)
                {
                    if (msg.Key.incoming)
                    {
                        resultIncomingInterval[msg.Key.Date].value = msg.count;
                        noOfIncomingMsgs += msg.count;
                    }
                    else
                    {
                        resultOutgoingInterval[msg.Key.Date].value = msg.count;
                        noOfOutgoingMsgs += msg.count;
                    }
                }

                Int32 noOfClients = msgs.GroupBy(msg => msg.ConversationId).Count();

                var row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(Resources.Global.RepIncomingSmsChart, Resources.Global.RepIncomingSmsChart), new RepDataRowCell(noOfIncomingMsgs, noOfIncomingMsgs + " sms") });
                var row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(Resources.Global.RepOutgoingSmsChart, Resources.Global.RepOutgoingSmsChart), new RepDataRowCell(noOfOutgoingMsgs, noOfOutgoingMsgs + " sms") });
                List<RepDataRow> pieChartContent = new List<RepDataRow>();
                pieChartContent.Add(row1);
                pieChartContent.Add(row2);
                RepChartData pieChartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, Resources.Global.RepTypeTable), new RepDataColumn("18", Constants.STRING_COLUMN_TYPE, Resources.Global.RepValueTable) }, pieChartContent);

                List<Dictionary<DateTime, ChartValue>> content = new List<Dictionary<DateTime, ChartValue>>();
                content.Add(resultIncomingInterval);
                content.Add(resultOutgoingInterval);
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepIncomingSmsChart), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepOutgoingSmsChart) }, PrepareJson(content, Resources.Global.RepSmsUnit));
                RepInfoBox ibNoOfIncomingMsgs = new RepInfoBox(noOfIncomingMsgs, Resources.Global.RepSmsUnit); // TODO: Refactor name of static variable RepSmsUnit 
                RepInfoBox ibNoOfOutgoingMsgs = new RepInfoBox(noOfOutgoingMsgs, Resources.Global.RepSmsUnit);
                RepInfoBox ibTotalNoOfClients = new RepInfoBox(noOfClients, Resources.Global.RepClientsUnit);
                RepInfoBox ibAvgNoOfIncomingMsgsPerClient = (noOfClients == 0) ? new RepInfoBox(0, Resources.Global.RepSmsPerClient) :
                    new RepInfoBox(Math.Round((double)noOfIncomingMsgs / noOfClients, 2), Resources.Global.RepSmsPerClient);
                RepInfoBox ibAvgNoOfOutgoingSmsPerClient = (noOfClients == 0) ? new RepInfoBox(0, Resources.Global.RepSmsPerClient)
                    : new RepInfoBox(Math.Round((double)noOfOutgoingMsgs / noOfClients, 2), Resources.Global.RepSmsPerClient);
                var repData = new ReportData(new List<RepChartData>() { chartSource, pieChartSource },
                        new List<RepInfoBox>() { ibNoOfIncomingMsgs, ibNoOfOutgoingMsgs,
                        ibTotalNoOfClients, ibAvgNoOfIncomingMsgsPerClient, ibAvgNoOfOutgoingSmsPerClient });
                var result = new { repData = repData, restoreData = dataToBeSaved };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetReportIncomingOutgoingData", e);
                return Json("request failed", JsonRequestBehavior.AllowGet);
            }
        }

        private IEnumerable<MsgInfoWrapper> GroupIncomingOutgoingMsgs(DateTime iIntervalStart, DateTime iIntervalEnd, String[] iScope, smsfeedbackEntities dbContext)
        {
            var msgs = GetMessages(iIntervalStart, iIntervalEnd, User.Identity.Name, iScope, dbContext);
            List<MsgInfoWrapper> msgsGrouped = (from msg in msgs
                                                                                   group msg by new MsgInfo
                                                                                   {
                                                                                       incoming = msg.ConversationId.StartsWith(msg.IsSmsBased ?
                                                                                           ConversationUtilities.CleanUpPhoneNumber(msg.From) :
                                                                                           ConversationUtilities.ExtractUserFromAddress(msg.From)),
                                                                                       Id = msg.Id,
                                                                                       TimeReceived = msg.TimeReceived
                                                                                   } into g
                                                                                   select new MsgInfoWrapper { Key = g.Key }).ToList();
            return msgsGrouped;
        }
        
        public JsonResult GetReportIncomingOutgoingGrData(String iIntervalStart, String iIntervalEnd, String iGranularity,String[] iScope = null)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
                 

                Dictionary<DateTime, ChartValue> resultIncomingInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultOutgoingInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
               iScope = iScope != null ? iScope : new string[0];
                if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                {
                    var incomingMsgsGr = from msg in GroupIncomingOutgoingMsgs(intervalStart,
                                             intervalEnd,
                                             iScope,
                                             context)
                                         group msg by new
                                         {
                                             msg.Key.TimeReceived.Date,
                                             incoming = msg.Key.incoming
                                         } into g
                                         select new { key = g.Key, count = g.Count() };
                    foreach (var entry in incomingMsgsGr)
                    {
                        if (entry.key.incoming)
                            resultIncomingInterval[entry.key.Date].value += entry.count;
                        else
                            resultOutgoingInterval[entry.key.Date].value += entry.count;
                    }
                }
                else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                {
                    var incomingMsgsGr = from msg in GroupIncomingOutgoingMsgs(intervalStart,
                                             intervalEnd,
                                             iScope,
                                             context)
                                         group msg by new
                                         {
                                             msg.Key.TimeReceived.Month,
                                             msg.Key.TimeReceived.Year,
                                             incoming = msg.Key.incoming
                                         } into g
                                         select new { key = g.Key, count = g.Count() };
                    foreach (var entry in incomingMsgsGr)
                    {
                        var monthDateTime = new DateTime(entry.key.Year, entry.key.Month, 1);
                        if (entry.key.incoming)
                        {
                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                            {
                                resultIncomingInterval[intervalStart].value += entry.count;
                            }
                            else
                            {
                                resultIncomingInterval[monthDateTime].value += entry.count;
                            }

                        }
                        else
                        {
                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                resultOutgoingInterval[intervalStart].value += entry.count;
                            else
                                resultOutgoingInterval[monthDateTime].value += entry.count;
                        }
                    }
                }
                else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                {
                    var incomingMsgsGr = from msg in GroupIncomingOutgoingMsgs(intervalStart,
                                             intervalEnd,
                                             iScope,
                                             context)
                                         group msg by new
                                         {
                                             firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(msg.Key.TimeReceived),
                                             incoming = msg.Key.incoming
                                         } into g
                                         select new { key = g.Key, count = g.Count() };
                    foreach (var entry in incomingMsgsGr)
                    {
                        var weekDateTime = entry.key.firstDayOfTheWeek;
                        if (entry.key.incoming)
                        {
                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                            {
                                resultIncomingInterval[intervalStart].value += entry.count;
                            }
                            else
                            {
                                resultIncomingInterval[weekDateTime].value += entry.count;
                            }

                        }
                        else
                        {
                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                resultOutgoingInterval[intervalStart].value += entry.count;
                            else
                                resultOutgoingInterval[weekDateTime].value += entry.count;
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
                logger.Error("GetReportIncomingOutgoingGrData", e);
                return Json("request failed", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region TagReports
        public JsonResult GetReportTagsData(
           String iIntervalStart,
           String iIntervalEnd,
           Dictionary<string,string> dataToBeSaved,
           String[] iScope = null
           )
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
                 

                String DEFAULT_TAGS = "defaultTags";
                iScope = iScope != null ? iScope : new string[0];
                var useAllWps = iScope.Length == 0;
                var tags = (from u in context.Users
                            where u.UserName.Equals(User.Identity.Name)
                            select (from wp in u.WorkingPoints
                                    where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                    select (from conv in wp.Conversations
                                            where conv.Messages.Where(msg => msg.TimeReceived >= intervalStart
                                                && msg.TimeReceived <= intervalEnd).Count() > 0
                                            select (from convTag in conv.ConversationTags
                                                    select
                                                        new { tag = convTag, wp = wp })))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);

                var tagsGr = from tag in tags
                             group tag by tag.tag.Tag.TagTagTypes.Count == 0 ? tag.tag.Tag.Name : DEFAULT_TAGS
                                 into g
                                 select new { key = g.Key, count = g.Count() };

                // Build the chart source and compute the "Most used tag"
                Int32 columnCounter = 0;
                String tagInterval = intervalStart.ToShortDateString() + " - " + intervalEnd.ToShortDateString();
                var headerContent = new List<RepDataColumn>();
                var rowContent = new List<RepDataRowCell>();
                rowContent.Add(new RepDataRowCell(tagInterval, tagInterval));
                headerContent.Add(new RepDataColumn(columnCounter.ToString(), Constants.STRING_COLUMN_TYPE));
                String mostUsedTag = Resources.Global.RepNoneDefaultValue;
                Int32 tagMaxUsage = -1;
                Int32 noOfTags = 0;

                foreach (var tagGr in tagsGr)
                {
                    if (!tagGr.key.Equals(DEFAULT_TAGS))
                    {
                        if (tagGr.count > tagMaxUsage)
                        {
                            tagMaxUsage = tagGr.count;
                            mostUsedTag = tagGr.key;
                        }
                        ++noOfTags;
                        ++columnCounter;
                        rowContent.Add(new RepDataRowCell(tagGr.count, tagGr.count + " " + Resources.Global.RepConversationsUnit));
                        headerContent.Add(new RepDataColumn(columnCounter.ToString(), Constants.NUMBER_COLUMN_TYPE, tagGr.key.ToString()));
                    }
                }

                if (rowContent.Count() == 1)
                {
                    rowContent.Add(new RepDataRowCell(0, Resources.Global.RepNoDataToDisplay));
                    headerContent.Add(new RepDataColumn("15", "number", Resources.Global.RepNoDataToDisplay));
                }

                RepChartData chartSource = new RepChartData(headerContent, new RepDataRow[] { new RepDataRow(rowContent) });                
                var noOfConversations = ((from u in context.Users
                                          where u.UserName.Equals(User.Identity.Name)
                                          select (from wp in u.WorkingPoints
                                                  select (from conv in wp.Conversations
                                                          where
                                                              conv.Messages.Where(msg => msg.TimeReceived >= intervalStart
                                                              && msg.TimeReceived <= intervalEnd).Count() > 0
                                                          select conv)))
                                                          .SelectMany(x => x).SelectMany(x => x)).Count();
                RepInfoBox IbMostUsedTag = new RepInfoBox(mostUsedTag, "");
                RepInfoBox IbAvgNoOfTagsPerConversation = (noOfConversations == 0) ? new RepInfoBox(0, Resources.Global.RepTagsPerConversationUnit) :
                    new RepInfoBox(Math.Round((double)noOfTags / noOfConversations, 2), Resources.Global.RepTagsPerConversationUnit);
                var user = (from u in context.Users where u.UserName.Equals(User.Identity.Name) select u).FirstOrDefault();

               //decide on the passed save data if we have any tags or not
               //DA for the time being the dataToBeSaved only contains the tag values
               RepChartData tagReport;
                if (dataToBeSaved != null && dataToBeSaved.Keys.Count != 0)
                {
                   var asarray = dataToBeSaved.Keys.ToArray();
                   tagReport = GetTagReportData(intervalStart, intervalEnd, iScope, Constants.DAY_GRANULARITY, asarray, user);
                }
                else
                {
                   var defaultTags = new string[1] { GetDefaultTagForTagReport(context) };                   
                   tagReport = GetTagReportData(intervalStart, intervalEnd, iScope, Constants.DAY_GRANULARITY, defaultTags, user);
                }
                var repData = new ReportData(new List<RepChartData>() { tagReport, chartSource},
                        new List<RepInfoBox>() { IbMostUsedTag, IbAvgNoOfTagsPerConversation });
                var result = new { repData = repData, restoreData = dataToBeSaved};
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetReportTagsData", e);
                return Json("request failed", JsonRequestBehavior.AllowGet);
            }
        }      

        public JsonResult GetTagReportDataGrid(String iIntervalStart, String iIntervalEnd, String[] iScope, String iGranularity = "day", string[] tags=null)
        {
           //report on conversations with activity in that period
           DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
           DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
           //var iGranularity = Constants.WEEK_GRANULARITY;

           var user = (from u in context.Users where u.UserName.Equals(User.Identity.Name) select u).FirstOrDefault();
            tags = tags ?? new string[0];
            iScope = iScope != null ? iScope : new string[0];            
            var res = GetTagReportData(intervalStart, intervalEnd, iScope, iGranularity, tags, user);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        private string GetDefaultTagForTagReport(smsfeedbackEntities context)
        {
            SmsFeedback_Take4.Models.AggregateSmsRepository iSmsRepository = SmsFeedback_Take4.Models.AggregateSmsRepository.GetInstance(User.Identity.Name);
            var specialTags = iSmsRepository.GetSpecialTags(User.Identity.Name, context);
            return specialTags.Where(t => t.TagType == Constants.NEGATIVE_FEEDBACK).FirstOrDefault().Name;
        }

        private RepChartData GetTagReportData(         
           DateTime intervalStart,
           DateTime intervalEnd,
           String[] iScope,
           string iGranularity,
           string[] tags,
           SmsFeedback_EFModels.User user)
        {
           List<Dictionary<DateTime, ChartValue>> content = new List<Dictionary<DateTime, ChartValue>>();
           Dictionary<DateTime, ChartValue> tagsRepData = InitializeInterval(intervalStart, intervalEnd, iGranularity);
           var useAllWps = iScope.Length == 0;
           if (tags.Length != 0)
           {
              switch (iGranularity)
              {
                 case Constants.WEEK_GRANULARITY:
                    var byweeks = (from wp in user.WorkingPoints
                                   where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                   select (from conv in wp.Conversations
                                           where (conv.StartTime >= intervalStart && conv.StartTime <= intervalEnd)
                                           && !tags.Except(conv.ConversationTags.Select(tag => tag.TagName)).Any()
                                           select conv)).SelectMany(x => x)
                                .GroupBy(c => new
                                {
                                   FirstDayInWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(c.StartTime),
                                }, c => c, (key, g) => new
                                {
                                   key = key,
                                   count = g.Count()
                                }).OrderBy(x => x.key.FirstDayInWeek);
                    foreach (var item in byweeks)
                    {
                       var weekDateTime = item.key.FirstDayInWeek;
                       if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                          tagsRepData[intervalStart].value = item.count;
                       else
                          tagsRepData[weekDateTime].value = item.count;
                    }
                    break;
                 case Constants.MONTH_GRANULARITY:
                    var bymonths = (from wp in user.WorkingPoints
                                    where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                    select (from conv in wp.Conversations
                                            where (conv.StartTime >= intervalStart && conv.StartTime <= intervalEnd)
                                            && !tags.Except(conv.ConversationTags.Select(tag => tag.TagName)).Any()
                                            select conv)).SelectMany(x => x)
                                .GroupBy(c => new
                                {
                                   Month = c.StartTime.Month,
                                   Year = c.StartTime.Year
                                }, c => c, (key, g) => new
                                {
                                   key = key,
                                   count = g.Count()
                                }).OrderBy(x => x.key.Year).ThenBy(x => x.key.Month);
                    foreach (var item in bymonths)
                    {
                       var monthDateTime = new DateTime(item.key.Year, item.key.Month, 1);

                       if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                          tagsRepData[intervalStart].value = item.count;
                       else
                          tagsRepData[monthDateTime].value = item.count;
                    }
                    break;
                 case Constants.DAY_GRANULARITY:
                 default:
                    var bydays = (from wp in user.WorkingPoints
                                  where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                  select (from conv in wp.Conversations
                                          where (conv.StartTime >= intervalStart && conv.StartTime <= intervalEnd)
                                          && !tags.Except(conv.ConversationTags.Select(tag => tag.TagName)).Any()
                                          select conv)).SelectMany(x => x)
                               .GroupBy(c => new
                               {
                                  Date = c.StartTime.Date,
                               }, c => c, (key, g) => new
                               {
                                  key = key,
                                  count = g.Count()
                               }).OrderBy(x => x.key.Date);
                    foreach (var item in bydays)
                    {
                       tagsRepData[item.key.Date].value = item.count;
                    }
                    break;
              }
           }
           
           content.Add(tagsRepData);
           //DA build legend
           string title;
           if (tags.Length != 0)
           {
              title = Resources.Global.repTagsLegendDescription + String.Join("& ", tags);
           }
           else
           {
              title = Resources.Global.repTagsLegendDescriptionNoTags;
           }
           RepChartData chartSource = new RepChartData(
            new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, title) },
            PrepareJson(content, Resources.Global.RepSmsUnit));
           return chartSource;
        }
        #endregion
        #region Report Positive and Negative
        public JsonResult GetReportPosNegData(String iIntervalStart, String iIntervalEnd, Dictionary<string, string> dataToBeSaved,String[] iScope= null)
        {
            try
            {
                var iGranularity = Constants.DAY_GRANULARITY;
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
                 

                Dictionary<DateTime, ChartValue> resultPositiveTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegativeTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultRemovePositiveTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultRemoveNegativeTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultPositiveTagsEvInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegativeTagsEvInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultPosToNegTransitionsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegToPosTransitionsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                KeyAndCount posToNegTransitions = new KeyAndCount(Constants.POS_TO_NEG_EVENT, 0);
                KeyAndCount negToPosTransitions = new KeyAndCount(Constants.NEG_TO_POS_EVENT, 0);

                iScope = iScope != null ? iScope : new string[0];
                var useAllWps = iScope.Length == 0;
                // GLOBAL SCOPE
                IEnumerable<ConversationHistory> convEvents = (from u in context.Users
                                                               where u.UserName.Equals(User.Identity.Name)
                                                               select (from wp in u.WorkingPoints
                                                                       where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                                                       select (from conv in wp.Conversations
                                                                               where !conv.Client.isSupportClient &&
                                                                               conv.Messages.Where(msg => msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd).Count() > 0
                                                                               select (from convEvent in conv.ConversationEvents
                                                                                       where (convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT)) &&
                                                                                           (convEvent.Date <= intervalEnd)
                                                                                       select convEvent)))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);

                /*
                 * All events are grouped by occur date and event name/type, which can be POS_ADD, NEG_ADD, etc.
                 * This groups of events are than grouped by occur date. And the new formed groups are ordered ascending by occur date.
                 * For Activity and Transitions I count for each type of event the number of appearances.
                 * For Evolution I count the previous events (occur date < intervalStart) and then count the events for each day.
                 */

                var posFeedback = 0;
                var negFeedback = 0;
                DateTime lastEvDate = DateTime.MinValue;
                int prevPosFeedback = 0;
                int prevNegFeedback = 0;
                int lastPosFeedback = -1;
                int lastNegFeedback = -1;
                var convEventsGr = (from convEvent in convEvents
                                    group convEvent by new { evOccurDate = convEvent.Date.Date, eventType = convEvent.EventTypeName }
                                        into g
                                        select new { key = g.Key, count = g.Count(), elements = g.ToList() }).OrderBy(convEventGr => convEventGr.key.evOccurDate);

                foreach (var convEvent in convEventsGr)
                {
                    if (convEvent.key.evOccurDate >= intervalStart && convEvent.key.evOccurDate <= intervalEnd)
                    {
                        lastNegFeedback = negFeedback;
                        lastPosFeedback = posFeedback;
                        if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                        {
                            resultPositiveTagsInterval[convEvent.key.evOccurDate].value += convEvent.count;
                            posFeedback += convEvent.count;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
                        {
                            resultNegativeTagsInterval[convEvent.key.evOccurDate].value += convEvent.count;
                            negFeedback += convEvent.count;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                        {
                            resultRemovePositiveTagsInterval[convEvent.key.evOccurDate].value -= convEvent.count;
                            posFeedback -= convEvent.count;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                        {
                            resultRemoveNegativeTagsInterval[convEvent.key.evOccurDate].value -= convEvent.count;
                            negFeedback -= convEvent.count;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                        {
                            resultPosToNegTransitionsInterval[convEvent.key.evOccurDate].value += convEvent.count;
                            posToNegTransitions.count += convEvent.count;
                            posFeedback -= convEvent.count;
                            negFeedback += convEvent.count;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                        {
                            resultNegToPosTransitionsInterval[convEvent.key.evOccurDate].value += convEvent.count;
                            negToPosTransitions.count += convEvent.count;
                            negFeedback -= convEvent.count;
                            posFeedback += convEvent.count;
                        }

                        /* To achieve: Fill the empty entries -- Obs: Maybe someday i'll figure out a smarter solution
                        * How the chart data look: The evolution chart is continuous. Lets say ch[d1] = x and ch[d6] = y, d1, d6 - dates, d1 < d6
                        *  x,y = no of positive fdbks, x!=y. The entries ch[d2], ch[d3], .., ch[d5] must have the x value.
                        *  
                        * How the events data look: Array<Entry> ordered asc by key.occurDate, Entry = {key, int count}
                        * Key = {type = {POS_ADD,etc}, DateTime occurDate}. Multiple Entry instances with the same occurDate.
                        * I convert the events data TO chart data
                        * 
                        * Solution: I group the events that occur on the same date. 
                        * To figure out when I finished to group different types of events for a certain date I use lastEvDate.
                        * Case lastEvDate == null fill the entries between startInterval and lastEvDate - 1 day
                        *      convEvent.key.evOccurDate > lastDate fill the entries between lastEvDate and evOccurDate - 1 day
                        */
                        if (lastEvDate.Equals(DateTime.MinValue))
                        {
                            for (var i = intervalStart; i <= convEvent.key.evOccurDate.AddDays(-1); i = i.AddDays(1))
                            {
                                resultPositiveTagsEvInterval[i].value = lastPosFeedback;
                                resultNegativeTagsEvInterval[i].value = lastNegFeedback;
                            }
                            lastEvDate = convEvent.key.evOccurDate;
                            lastPosFeedback = posFeedback;
                            lastNegFeedback = negFeedback;
                        }
                        else
                        {
                            if (convEvent.key.evOccurDate > lastEvDate)
                            {
                                for (var i = lastEvDate; i <= convEvent.key.evOccurDate.AddDays(-1); i = i.AddDays(1))
                                {
                                    resultPositiveTagsEvInterval[i].value = lastPosFeedback;
                                    resultNegativeTagsEvInterval[i].value = lastNegFeedback;
                                }
                                lastEvDate = convEvent.key.evOccurDate;
                                lastPosFeedback = posFeedback;
                                lastNegFeedback = negFeedback;
                            }
                            else
                            {
                                lastEvDate = convEvent.key.evOccurDate;
                                lastPosFeedback = posFeedback;
                                lastNegFeedback = negFeedback;
                            }
                        }
                    }
                    else
                    {
                        if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                        {
                            ++posFeedback;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
                        {
                            ++negFeedback;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                        {
                            --posFeedback;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                        {
                            --negFeedback;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                        {
                            ++negFeedback;
                            --posFeedback;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                        {
                            ++posFeedback;
                            --negFeedback;
                        }
                    }
                }
                if (lastEvDate.Equals(DateTime.MinValue))
                {
                    // No events found in interval intervalStart - intervalEnd
                    lastEvDate = intervalStart;
                    lastPosFeedback = prevPosFeedback;
                    lastNegFeedback = prevNegFeedback;
                }
                for (var i = lastEvDate; i <= intervalEnd; i = i.AddDays(1))
                {
                    resultPositiveTagsEvInterval[i].value = lastPosFeedback;
                    resultNegativeTagsEvInterval[i].value = lastNegFeedback;
                }

                List<Dictionary<DateTime, ChartValue>> evolutionChartContent = new List<Dictionary<DateTime, ChartValue>>();
                evolutionChartContent.Add(resultPositiveTagsEvInterval);
                evolutionChartContent.Add(resultNegativeTagsEvInterval);
                RepChartData evolutionChartSource = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("17", Constants.STRING_COLUMN_TYPE), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPositiveFeedback), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegativeFeedback) },
                    PrepareJson(evolutionChartContent, Resources.Global.RepConversationsUnit));

                List<Dictionary<DateTime, ChartValue>> activityChartContent = new List<Dictionary<DateTime, ChartValue>>();
                activityChartContent.Add(resultPositiveTagsInterval);
                activityChartContent.Add(resultNegativeTagsInterval);
                activityChartContent.Add(resultRemovePositiveTagsInterval);
                activityChartContent.Add(resultRemoveNegativeTagsInterval);
                RepChartData activityChartSource = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("16", Constants.STRING_COLUMN_TYPE), 
                    new RepDataColumn("17", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPosFeedbackAdded), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegFeedbackAdded),
                    new RepDataColumn("19", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPosFeedbackRemoved), 
                    new RepDataColumn("20", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegFeedbackRemoved) },
                    PrepareJson(activityChartContent, Resources.Global.RepConversationsUnit));

                List<Dictionary<DateTime, ChartValue>> transitionsChartContent = new List<Dictionary<DateTime, ChartValue>>();
                transitionsChartContent.Add(resultNegToPosTransitionsInterval);
                transitionsChartContent.Add(resultPosToNegTransitionsInterval);
                RepChartData transitionsChartSource = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("17", Constants.STRING_COLUMN_TYPE), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegToPosFeedback), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPosToNegFeedback) },
                PrepareJson(transitionsChartContent, Resources.Global.RepConversationsUnit));

                List<RepDataRow> transitionsPieChartContent = new List<RepDataRow>();
                RepDataRow row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(Resources.Global.RepPosToNegFeedback, Resources.Global.RepPosToNegFeedback), new RepDataRowCell(posToNegTransitions.count, posToNegTransitions.count + " " + Resources.Global.RepPosToNegFeedback) });
                RepDataRow row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(Resources.Global.RepNegToPosFeedback, Resources.Global.RepNegToPosFeedback), new RepDataRowCell(negToPosTransitions.count, negToPosTransitions.count + " " + Resources.Global.RepNegToPosFeedback) });
                transitionsPieChartContent.Add(row1);
                transitionsPieChartContent.Add(row2);
                RepChartData transitionsPieChartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, Resources.Global.RepTypeTable), new RepDataColumn("18", Constants.STRING_COLUMN_TYPE, Resources.Global.RepValueTable) }, transitionsPieChartContent);

                ReportData repData = new ReportData(new List<RepChartData>() { evolutionChartSource, transitionsChartSource, transitionsPieChartSource, activityChartSource }, null);
                var result = new { repData = repData, restoreData = dataToBeSaved };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetReportPosNegData", e);
                return Json("request failed", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetReportPosNegActivityGr(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
                 

                Dictionary<DateTime, ChartValue> resultPositiveTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegativeTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultRemovePositiveTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultRemoveNegativeTagsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultPositiveTagsEvInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegativeTagsEvInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);

                iScope = iScope != null ? iScope : new string[0];
                var useAllWps = iScope.Length == 0;
                // GLOBAL SCOPE
                IEnumerable<ConversationHistory> convEvents = (from u in context.Users
                                                               where u.UserName.Equals(User.Identity.Name)
                                                               select (from wp in u.WorkingPoints
                                                                       where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                                                       select (from conv in wp.Conversations
                                                                               where !conv.Client.isSupportClient &&
                                                                               conv.Messages.Where(msg => msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd).Count() > 0
                                                                               select (from convEvent in conv.ConversationEvents
                                                                                       where (convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT)) &&
                                                                                           (convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd)
                                                                                       select convEvent)))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);

                /*
                 * All events are grouped by occur date and event name/type, which can be POS_ADD, NEG_ADD, etc.
                 * This groups of events are than grouped by occur date. And the new formed groups are ordered ascending by occur date.
                 * For Activity I count for each type of event the number of appearances.
                 */


                if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                {
                    var convEventsGr = (from convEvent in convEvents
                                        group convEvent by new { evOccurDate = convEvent.Date.Date, eventType = convEvent.EventTypeName }
                                            into g
                                            select new { key = g.Key, count = g.Count(), elements = g.ToList() });

                    foreach (var convEvent in convEventsGr)
                    {
                        if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                        {
                            resultPositiveTagsInterval[convEvent.key.evOccurDate].value += convEvent.count;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
                        {
                            resultNegativeTagsInterval[convEvent.key.evOccurDate].value += convEvent.count;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                        {
                            resultRemovePositiveTagsInterval[convEvent.key.evOccurDate].value -= convEvent.count;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                        {
                            resultRemoveNegativeTagsInterval[convEvent.key.evOccurDate].value -= convEvent.count;
                        }
                    }

                }
                else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                {
                    var convEventsGr = (from convEvent in convEvents
                                        group convEvent by new
                                        {
                                            Month = convEvent.Date.Month,
                                            Year = convEvent.Date.Year,
                                            eventType = convEvent.EventTypeName
                                        } into g
                                        select new { key = g.Key, count = g.Count(), elements = g.ToList() });

                    foreach (var convEvent in convEventsGr)
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
                else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                {
                    var convEventsGr = (from convEvent in convEvents
                                        group convEvent by new
                                        {
                                            firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(convEvent.Date),
                                            eventType = convEvent.EventTypeName
                                        }
                                            into g
                                            select new { key = g.Key, count = g.Count(), elements = g.ToList() });

                    foreach (var convEvent in convEventsGr)
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

                List<Dictionary<DateTime, ChartValue>> activityChartContent = new List<Dictionary<DateTime, ChartValue>>();
                activityChartContent.Add(resultPositiveTagsInterval);
                activityChartContent.Add(resultNegativeTagsInterval);
                activityChartContent.Add(resultRemovePositiveTagsInterval);
                activityChartContent.Add(resultRemoveNegativeTagsInterval);
                RepChartData activityChartSource = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("16", Constants.STRING_COLUMN_TYPE, "Date"), 
                    new RepDataColumn("17", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPosFeedbackAdded), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegFeedbackAdded),
                    new RepDataColumn("19", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPosFeedbackRemoved), 
                    new RepDataColumn("20", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegFeedbackRemoved) },
                    PrepareJson(activityChartContent, Resources.Global.RepConversationsUnit));

                return Json(activityChartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetReportPosNegActivityData", e);
                return Json("request failed", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetReportPosNegTransitionsGr(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope=null)
        {
            try
            {
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
                 

                Dictionary<DateTime, ChartValue> resultPosNegTagsTrInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegPosTagsTrInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);

                iScope = iScope != null ? iScope : new string[0];
                var useAllWps = iScope.Length == 0;
                // GLOBAL SCOPE
                IEnumerable<ConversationHistory> convEvents = (from u in context.Users
                                                               where u.UserName.Equals(User.Identity.Name)
                                                               select (from wp in u.WorkingPoints
                                                                       where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                                                       select (from conv in wp.Conversations
                                                                               where !conv.Client.isSupportClient &&
                                                                               conv.Messages.Where(msg => msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd).Count() > 0
                                                                               select (from convEvent in conv.ConversationEvents
                                                                                       where (convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT)) &&
                                                                                           (convEvent.Date >= intervalStart && convEvent.Date <= intervalEnd)
                                                                                       select convEvent)))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);

                /*
                 * All events are grouped by occur date and event name/type, which can be POS_ADD, NEG_ADD, etc.
                 * This groups of events are than grouped by occur date. And the new formed groups are ordered ascending by occur date.
                 * For Activity I count for each type of event the number of appearances.
                 */


                if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                {
                    var convEventsGr = (from convEvent in convEvents
                                        group convEvent by new { evOccurDate = convEvent.Date.Date, eventType = convEvent.EventTypeName }
                                            into g
                                            select new { key = g.Key, count = g.Count(), elements = g.ToList() });

                    foreach (var convEvent in convEventsGr)
                    {
                        if (convEvent.key.evOccurDate >= intervalStart && convEvent.key.evOccurDate <= intervalEnd)
                        {
                            if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                            {
                                resultPosNegTagsTrInterval[convEvent.key.evOccurDate].value += convEvent.count;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                            {
                                resultNegPosTagsTrInterval[convEvent.key.evOccurDate].value += convEvent.count;
                            }
                        }
                    }
                }
                else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                {
                    var convEventsGr = (from convEvent in convEvents
                                        group convEvent by new
                                        {
                                            Month = convEvent.Date.Month,
                                            Year = convEvent.Date.Year,
                                            eventType = convEvent.EventTypeName
                                        }
                                            into g
                                            select new { key = g.Key, count = g.Count(), elements = g.ToList() });

                    foreach (var convEvent in convEventsGr)
                    {
                        var monthDateTime = new DateTime(convEvent.key.Year, convEvent.key.Month, 1);
                        if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                        {
                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                resultPosNegTagsTrInterval[intervalStart].value += convEvent.count;
                            else
                                resultPosNegTagsTrInterval[monthDateTime].value += convEvent.count;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                        {
                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                resultNegPosTagsTrInterval[intervalStart].value += convEvent.count;
                            else
                                resultNegPosTagsTrInterval[monthDateTime].value += convEvent.count;
                        }
                    }
                }
                else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                {
                    var convEventsGr = (from convEvent in convEvents
                                        group convEvent by new
                                        {
                                            firstDayOfTheWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(convEvent.Date),
                                            eventType = convEvent.EventTypeName
                                        }
                                            into g
                                            select new { key = g.Key, count = g.Count(), elements = g.ToList() });

                    foreach (var convEvent in convEventsGr)
                    {
                        var weekDateTime = convEvent.key.firstDayOfTheWeek;
                        if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                        {
                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                resultPosNegTagsTrInterval[intervalStart].value += convEvent.count;
                            else
                                resultPosNegTagsTrInterval[weekDateTime].value += convEvent.count;
                        }
                        else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                        {
                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                resultNegPosTagsTrInterval[intervalStart].value += convEvent.count;
                            else
                                resultNegPosTagsTrInterval[weekDateTime].value += convEvent.count;
                        }
                    }
                }

                List<Dictionary<DateTime, ChartValue>> transitionsChartContent = new List<Dictionary<DateTime, ChartValue>>();
                transitionsChartContent.Add(resultNegPosTagsTrInterval);
                transitionsChartContent.Add(resultPosNegTagsTrInterval);
                RepChartData transitionsChartSource = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegToPosFeedback), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPosToNegFeedback) },
                PrepareJson(transitionsChartContent, Resources.Global.RepConversationsUnit));

                return Json(transitionsChartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetReportPosNegTransitionsData", e);
                return Json("request failed", JsonRequestBehavior.AllowGet);
            }
        }

       // public RepChartData PosEvolutionSideBySideInternal(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
       //{
       //    DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
       //    DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);

       //    var resIntervals = new Dictionary<string, Dictionary<DateTime, ChartValue>>();
       //    foreach (var location in iScope)
       //    {
       //       resIntervals.Add(location, InitializeInterval(intervalStart, intervalEnd, iGranularity));
       //    }

       //    var grConvEvents = GetSideBySidePositiveConvEventsInternal(iGranularity, iScope, intervalStart, intervalEnd);
       //    if (iGranularity.Equals(Constants.DAY_GRANULARITY))
       //    {
       //       foreach (var gr in grConvEvents)
       //       {
       //          var convEventsGr = (from convEvent in gr.Value
       //                              group convEvent by new { evOccurDate = convEvent.Date.Date, eventType = convEvent.EventTypeName }
       //                                 into g
       //                                 select new { key = g.Key, count = g.Count(), elements = g.ToList() }).OrderBy(c =>c.key.evOccurDate);

       //          foreach (var convEvent in convEventsGr)
       //          {
       //             var posFeedback = 0;                    

       //             if (convEvent.key.evOccurDate >= intervalStart && convEvent.key.evOccurDate <= intervalEnd)
       //             {
       //                if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
       //                {
       //                   posFeedback += convEvent.count;
       //                }
       //                else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT) || convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
       //                {
       //                   posFeedback -= convEvent.count;
       //                }
                       

       //                /* To achieve: Fill the empty entries -- Obs: Maybe someday i'll figure out a smarter solution
       //                * How the chart data look: The evolution chart is continuous. Lets say ch[d1] = x and ch[d6] = y, d1, d6 - dates, d1 < d6
       //                *  x,y = no of positive fdbks, x!=y. The entries ch[d2], ch[d3], .., ch[d5] must have the x value.
       //                *  
       //                * How the events data look: Array<Entry> ordered asc by key.occurDate, Entry = {key, int count}
       //                * Key = {type = {POS_ADD,etc}, DateTime occurDate}. Multiple Entry instances with the same occurDate.
       //                * I convert the events data TO chart data
       //                * 
       //                * Solution: I group the events that occur on the same date. 
       //                * To figure out when I finished to group different types of events for a certain date I use lastEvDate.
       //                * Case lastEvDate == null fill the entries between startInterval and lastEvDate - 1 day
       //                *      convEvent.key.evOccurDate > lastDate fill the entries between lastEvDate and evOccurDate - 1 day
       //                */

       //                if (lastEvDate.Equals(DateTime.MinValue))
       //                {
       //                   lastEvDate = convEvent.key.evOccurDate;
       //                   lastPosFeedback = posFeedback;
       //                   lastNegFeedback = negFeedback;
       //                }
       //                else
       //                {
       //                   if (convEvent.key.evOccurDate > lastEvDate)
       //                   {
       //                      for (var i = lastEvDate; i <= convEvent.key.evOccurDate.AddDays(-1); i = i.AddDays(1))
       //                      {
       //                         resultPositiveTagsEvInterval[i].value = lastPosFeedback;
       //                         resultNegativeTagsEvInterval[i].value = lastNegFeedback;
       //                      }
       //                      lastEvDate = convEvent.key.evOccurDate;
       //                      lastPosFeedback = posFeedback;
       //                      lastNegFeedback = negFeedback;
       //                   }
       //                   else
       //                   {
       //                      lastEvDate = convEvent.key.evOccurDate;
       //                      lastPosFeedback = posFeedback;
       //                      lastNegFeedback = negFeedback;
       //                   }
       //                }
       //             }
       //             else
       //             {
       //                if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
       //                {
       //                   ++posFeedback;
       //                }
       //                else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
       //                {
       //                   ++negFeedback;
       //                }
       //                else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
       //                {
       //                   --posFeedback;
       //                }
       //                else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
       //                {
       //                   --negFeedback;
       //                }
       //                else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
       //                {
       //                   ++negFeedback;
       //                   --posFeedback;
       //                }
       //                else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
       //                {
       //                   ++posFeedback;
       //                   --negFeedback;
       //                }
       //             }

       //          }
       //          if (lastEvDate.Equals(DateTime.MinValue))
       //          {
       //             // No events found in interval intervalStart - intervalEnd
       //             lastEvDate = intervalStart;
       //             lastPosFeedback = prevPosFeedback;
       //             lastNegFeedback = prevNegFeedback;
       //          }
       //          for (var i = lastEvDate; i <= intervalEnd; i = i.AddDays(1))
       //          {
       //             resultPositiveTagsEvInterval[i].value = lastPosFeedback;
       //             resultNegativeTagsEvInterval[i].value = lastNegFeedback;
       //          }
       //       }
       //    }
       //    else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
       //    {
             
       //    }
       //    else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
       //    {
             
       //    }

       //    List<Dictionary<DateTime, ChartValue>> posEvolutionChartContent = new List<Dictionary<DateTime, ChartValue>>();
       //    foreach (var item in resIntervals)
       //    {
       //       posEvolutionChartContent.Add(item.Value);
       //    }
       //    List<RepDataColumn> columnDefinition = new List<RepDataColumn> { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date") };
       //    foreach (var location in iScope)
       //    {
       //       columnDefinition.Add(new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, location));

       //    }
       //    RepChartData chartSource = new RepChartData(
       //       columnDefinition,
       //          PrepareJson(posEvolutionChartContent, Resources.Global.RepSmsUnit));
       //    return chartSource;
       //}

       private Dictionary<string, IEnumerable<ConversationHistory>> GetSideBySidePositiveConvEventsInternal(String iGranularity, String[] iScope, DateTime intervalStart, DateTime intervalEnd)
       {
          var resIntervals = new Dictionary<string, Dictionary<DateTime, ChartValue>>();
          foreach (var location in iScope)
          {
             resIntervals.Add(location, InitializeInterval(intervalStart, intervalEnd, iGranularity));
          }
          bool workOnAllWps = iScope.Length == 0 ? true : false;
          var convEvents = (from u in context.Users
                            where u.UserName.Equals(User.Identity.Name)
                            select (from wp in u.WorkingPoints
                                    where workOnAllWps ? true : iScope.Contains(wp.TelNumber)
                                    select (from conv in wp.Conversations
                                            where !conv.Client.isSupportClient &&
                                            conv.Messages.Where(msg => msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd).Count() > 0
                                            select (from convEvent in conv.ConversationEvents
                                                    where (convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                        convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                        convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                        convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT) ||
                                                        convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT) ||
                                                        convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT)) &&
                                                        (convEvent.Date <= intervalEnd)
                                                    select new { WpId = wp.TelNumber, convEvent = convEvent })))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);
          var convEventsGr = convEvents.GroupBy(c => new
          {
             c.WpId
          }, c => c, (key, g) => new
          {
             key = key,
             convEvents = from item in g select item.convEvent
          });
          var result = new Dictionary<string, IEnumerable<ConversationHistory>>();
          foreach (var gr in convEventsGr)
          {
             result.Add(gr.key.WpId, gr.convEvents);
          }
          return result;
       }

        public JsonResult GetReportPosNegEvolutionGr(String iIntervalStart, String iIntervalEnd,  String iGranularity,String[] iScope = null )
        {
            try
            {
               DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
               DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
                 

                Dictionary<DateTime, ChartValue> resultPositiveTagsEvInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegativeTagsEvInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);

                KeyAndCount posToNegTransitions = new KeyAndCount(Constants.POS_TO_NEG_EVENT, 0);
                KeyAndCount negToPosTransitions = new KeyAndCount(Constants.NEG_TO_POS_EVENT, 0);

                iScope = iScope != null ? iScope : new string[0];
                var useAllWps = iScope.Length == 0;
                // GLOBAL SCOPE
                IEnumerable<ConversationHistory> convEvents = (from u in context.Users
                                                               where u.UserName.Equals(User.Identity.Name)
                                                               select (from wp in u.WorkingPoints
                                                                       where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                                                       select (from conv in wp.Conversations
                                                                               where !conv.Client.isSupportClient &&
                                                                               conv.Messages.Where(msg => msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd).Count() > 0
                                                                               select (from convEvent in conv.ConversationEvents
                                                                                       where (convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT)) &&
                                                                                           (convEvent.Date <= intervalEnd)
                                                                                       select convEvent)))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);

                /*
                 * All events are grouped by occur date and event name/type, which can be POS_ADD, NEG_ADD, etc.
                 * This groups of events are than grouped by occur date. And the new formed groups are ordered ascending by occur date.
                 * For Activity and Transitions I count for each type of event the number of appearances.
                 * For Evolution I count the previous events (occur date < intervalStart) and then count the events for each day.
                 */

                var posFeedback = 0;
                var negFeedback = 0;
                DateTime lastEvDate = DateTime.MinValue;
                int prevPosFeedback = 0;
                int prevNegFeedback = 0;
                int lastPosFeedback = -1;
                int lastNegFeedback = -1;

                if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                {
                    var convEventsGr = (from convEvent in convEvents
                                        group convEvent by new
                                        {
                                            evOccurDate = convEvent.Date.Date,
                                            eventType = convEvent.EventTypeName
                                        }
                                            into g
                                            select new
                                            {
                                                key = g.Key,
                                                count = g.Count(),
                                                elements = g.ToList()
                                            }).OrderBy(convEventGr =>
                                                convEventGr.key.evOccurDate);

                    foreach (var convEvent in convEventsGr)
                    {
                        if (convEvent.key.evOccurDate >= intervalStart && convEvent.key.evOccurDate <= intervalEnd)
                        {
                            if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                            {
                                posFeedback += convEvent.count;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
                            {
                                negFeedback += convEvent.count;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                            {
                                posFeedback -= convEvent.count;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                            {
                                negFeedback -= convEvent.count;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                            {
                                posFeedback -= convEvent.count;
                                negFeedback += convEvent.count;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                            {
                                negFeedback -= convEvent.count;
                                posFeedback += convEvent.count;
                            }

                            /* To achieve: Fill the empty entries -- Obs: Maybe someday i'll figure out a smarter solution
                            * How the chart data look: The evolution chart is continuous. Lets say ch[d1] = x and ch[d6] = y, d1, d6 - dates, d1 < d6
                            *  x,y = no of positive fdbks, x!=y. The entries ch[d2], ch[d3], .., ch[d5] must have the x value.
                            *  
                            * How the events data look: Array<Entry> ordered asc by key.occurDate, Entry = {key, int count}
                            * Key = {type = {POS_ADD,etc}, DateTime occurDate}. Multiple Entry instances with the same occurDate.
                            * I convert the events data TO chart data
                            * 
                            * Solution: I group the events that occur on the same date. 
                            * To figure out when I finished to group different types of events for a certain date I use lastEvDate.
                            * Case lastEvDate == null fill the entries between startInterval and lastEvDate - 1 day
                            *      convEvent.key.evOccurDate > lastDate fill the entries between lastEvDate and evOccurDate - 1 day
                            */

                            if (lastEvDate.Equals(DateTime.MinValue))
                            {
                                lastEvDate = convEvent.key.evOccurDate;
                                lastPosFeedback = posFeedback;
                                lastNegFeedback = negFeedback;
                            }
                            else
                            {
                                if (convEvent.key.evOccurDate > lastEvDate)
                                {
                                    for (var i = lastEvDate; i <= convEvent.key.evOccurDate.AddDays(-1); i = i.AddDays(1))
                                    {
                                        resultPositiveTagsEvInterval[i].value = lastPosFeedback;
                                        resultNegativeTagsEvInterval[i].value = lastNegFeedback;
                                    }
                                    lastEvDate = convEvent.key.evOccurDate;
                                    lastPosFeedback = posFeedback;
                                    lastNegFeedback = negFeedback;
                                }
                                else
                                {
                                    lastEvDate = convEvent.key.evOccurDate;
                                    lastPosFeedback = posFeedback;
                                    lastNegFeedback = negFeedback;
                                }
                            }
                        }
                        else
                        {
                            if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                            {
                                ++posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
                            {
                                ++negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                            {
                                --posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                            {
                                --negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                            {
                                ++negFeedback;
                                --posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                            {
                                ++posFeedback;
                                --negFeedback;
                            }
                        }

                    }
                    if (lastEvDate.Equals(DateTime.MinValue))
                    {
                        // No events found in interval intervalStart - intervalEnd
                        lastEvDate = intervalStart;
                        lastPosFeedback = prevPosFeedback;
                        lastNegFeedback = prevNegFeedback;
                    }
                    for (var i = lastEvDate; i <= intervalEnd; i = i.AddDays(1))
                    {
                        resultPositiveTagsEvInterval[i].value = lastPosFeedback;
                        resultNegativeTagsEvInterval[i].value = lastNegFeedback;
                    }

                }
                else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                {
                    var convEventsGr = (from convEvent in convEvents
                                        group convEvent by new
                                        {
                                            evOccurDate = convEvent.Date,
                                            eventType = convEvent.EventTypeName
                                        }
                                            into g
                                            select new
                                            {
                                                key = g.Key
                                            })
                                            .OrderBy(convEvent => convEvent.key.evOccurDate);

                    foreach (var convEvent in convEventsGr)
                    {
                        if (convEvent.key.evOccurDate >= intervalStart && convEvent.key.evOccurDate <= intervalEnd)
                        {
                            var monthDateTime = new DateTime(convEvent.key.evOccurDate.Year, convEvent.key.evOccurDate.Month, 1);
                            lastPosFeedback = posFeedback;
                            lastNegFeedback = negFeedback;

                            if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                            {
                                ++posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
                            {
                                ++negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                            {
                                --posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                            {
                                --negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                            {
                                --posFeedback;
                                ++negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                            {
                                --negFeedback;
                                ++posFeedback;
                            }

                            if (lastEvDate.Equals(DateTime.MinValue))
                            {
                                if (monthDateTime > intervalStart)
                                {
                                    resultPositiveTagsEvInterval[intervalStart].value = lastPosFeedback;
                                    resultNegativeTagsEvInterval[intervalStart].value = lastNegFeedback;

                                    var nextMonth = intervalStart.AddMonths(1);
                                    var nextMonthFirst = new DateTime(nextMonth.Year, nextMonth.Month, 1);
                                    for (var i = nextMonthFirst; i < monthDateTime; i = i.AddMonths(1))
                                    {
                                        resultNegativeTagsEvInterval[i].value = lastNegFeedback;
                                        resultPositiveTagsEvInterval[i].value = lastPosFeedback;
                                    }
                                }
                                lastEvDate = monthDateTime;
                                lastPosFeedback = posFeedback;
                                lastNegFeedback = negFeedback;
                            }
                            else
                            {
                                if (monthDateTime > lastEvDate)
                                {

                                    for (var i = lastEvDate; i < monthDateTime; i = i.AddMonths(1))
                                    {
                                        resultPositiveTagsEvInterval[monthDateTime].value = lastPosFeedback;
                                        resultNegativeTagsEvInterval[monthDateTime].value = lastNegFeedback;
                                    }
                                    lastEvDate = monthDateTime;
                                    lastPosFeedback = posFeedback;
                                    lastNegFeedback = negFeedback;
                                }
                                else
                                {
                                    lastEvDate = monthDateTime;
                                    lastPosFeedback = posFeedback;
                                    lastNegFeedback = negFeedback;
                                }
                            }
                        }
                        else
                        {
                            if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                            {
                                ++prevPosFeedback;
                                ++posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
                            {
                                ++prevNegFeedback;
                                ++negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                            {
                                --prevPosFeedback;
                                --posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                            {
                                --prevNegFeedback;
                                --negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                            {
                                ++prevNegFeedback;
                                ++negFeedback;
                                --prevPosFeedback;
                                --posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                            {
                                ++prevPosFeedback;
                                ++posFeedback;
                                --prevNegFeedback;
                                --negFeedback;
                            }
                        }

                    }
                    if (lastEvDate.Equals(DateTime.MinValue))
                    {
                        // No events found in interval intervalStart - intervalEnd
                        lastEvDate = intervalStart;
                        lastPosFeedback = prevPosFeedback;
                        lastNegFeedback = prevNegFeedback;
                    }
                    for (var i = lastEvDate; i <= intervalEnd; i = i.AddMonths(1))
                    {
                        resultPositiveTagsEvInterval[i].value = lastPosFeedback;
                        resultNegativeTagsEvInterval[i].value = lastNegFeedback;
                    }

                }
                else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                {
                    var convEventsGr = (from convEvent in convEvents
                                        group convEvent by new
                                        {
                                            evOccurDate = convEvent.Date,
                                            eventType = convEvent.EventTypeName
                                        }
                                            into g
                                            select new
                                            {
                                                key = g.Key
                                            })
                                            .OrderBy(convEvent => convEvent.key.evOccurDate);

                    foreach (var convEvent in convEventsGr)
                    {
                        if (convEvent.key.evOccurDate >= intervalStart && convEvent.key.evOccurDate <= intervalEnd)
                        {
                            var weekDateTime = FirstDayOfWeekUtility.GetFirstDayOfWeek(convEvent.key.evOccurDate);
                            lastPosFeedback = posFeedback;
                            lastNegFeedback = negFeedback;

                            if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                            {
                                ++posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
                            {
                                ++negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                            {
                                --posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                            {
                                --negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                            {
                                --posFeedback;
                                ++negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                            {
                                --negFeedback;
                                ++posFeedback;
                            }
                                                      
                            if (lastEvDate.Equals(DateTime.MinValue))
                            {
                                if (weekDateTime > intervalStart)
                                {
                                    resultPositiveTagsEvInterval[intervalStart].value = lastPosFeedback;
                                    resultNegativeTagsEvInterval[intervalStart].value = lastNegFeedback;

                                    var nextWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(intervalStart).AddDays(7);
                                    for (var i = nextWeek; i < weekDateTime; i = i.AddDays(7))
                                    {
                                        resultNegativeTagsEvInterval[i].value = lastNegFeedback;
                                        resultPositiveTagsEvInterval[i].value = lastPosFeedback;
                                    }
                                }
                                lastEvDate = weekDateTime;
                                lastPosFeedback = posFeedback;
                                lastNegFeedback = negFeedback;
                            }
                            else
                            {
                                if (weekDateTime > lastEvDate)
                                {

                                    for (var i = lastEvDate; i < weekDateTime; i = i.AddDays(7))
                                    {
                                       //TODO DA  - fix this logic
                                       var iInWeek = FirstDayOfWeekUtility.GetFirstDayOfWeek(i);
                                       iInWeek = intervalStart > iInWeek ? intervalStart : iInWeek;
                                       //
                                       resultPositiveTagsEvInterval[iInWeek].value = lastPosFeedback;
                                       resultNegativeTagsEvInterval[iInWeek].value = lastNegFeedback;
                                    }
                                    lastEvDate = weekDateTime;
                                    lastPosFeedback = posFeedback;
                                    lastNegFeedback = negFeedback;
                                }
                                else
                                {
                                    lastEvDate = weekDateTime;
                                    lastPosFeedback = posFeedback;
                                    lastNegFeedback = negFeedback;
                                }
                            }
                        }
                        else
                        {
                            if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT))
                            {
                                ++prevPosFeedback;
                                ++posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT))
                            {
                                ++prevNegFeedback;
                                ++negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_REMOVE_EVENT))
                            {
                                --prevPosFeedback;
                                --posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_REMOVE_EVENT))
                            {
                                --prevNegFeedback;
                                --negFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                            {
                                ++prevNegFeedback;
                                ++negFeedback;
                                --prevPosFeedback;
                                --posFeedback;
                            }
                            else if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                            {
                                ++prevPosFeedback;
                                ++posFeedback;
                                --prevNegFeedback;
                                --negFeedback;
                            }
                        }

                    }
                    if (lastEvDate.Equals(DateTime.MinValue))
                    {
                        // No events found in interval intervalStart - intervalEnd
                        lastEvDate = intervalStart;
                        lastPosFeedback = prevPosFeedback;
                        lastNegFeedback = prevNegFeedback;
                    }
                    for (var i = lastEvDate; i <= intervalEnd; i = i.AddDays(7))
                    {
                        resultPositiveTagsEvInterval[i].value = lastPosFeedback;
                        resultNegativeTagsEvInterval[i].value = lastNegFeedback;
                    }
                }

                List<Dictionary<DateTime, ChartValue>> evolutionChartContent = new List<Dictionary<DateTime, ChartValue>>();
                evolutionChartContent.Add(resultPositiveTagsEvInterval);
                evolutionChartContent.Add(resultNegativeTagsEvInterval);
                RepChartData evolutionChartSource = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepPositiveFeedback), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNegativeFeedback) },
                    PrepareJson(evolutionChartContent, Resources.Global.RepConversationsUnit));

                return Json(evolutionChartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetReportPosNegEvolutionData", e);
                return Json("request failed", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Client report
        public JsonResult GetReportClientsData(String iIntervalStart, String iIntervalEnd, String[] iScope, Dictionary<string, string> dataToBeSaved)
        {
            try
            {
                var iGranularity = Constants.DAY_GRANULARITY;
                DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
                DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
                                                
                Dictionary<DateTime, ChartValue> resultNewClientsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultReturningClientsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);

                iScope = iScope != null ? iScope : new string[0];
                var useAllWps = iScope.Length == 0;
                var clients = (from u in context.Users
                               where u.UserName.Equals(User.Identity.Name)
                               select (from wp in u.WorkingPoints
                                       where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                       select (from conv in wp.Conversations
                                               where conv.Messages.Where(msg => msg.TimeReceived >= intervalStart &&
                                                   msg.TimeReceived <= intervalEnd).Count() > 0
                                               select (from msg in conv.Messages
                                                       where
                                                           msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd
                                                       group msg by new
                                                       {
                                                           msg.TimeReceived.Day,
                                                           msg.TimeReceived.Month,
                                                           msg.TimeReceived.Year,
                                                           conv.ConvId,
                                                           returning = conv.Messages.Where(convMsg => convMsg.TimeReceived < intervalStart).Count() > 0
                                                       }
                                                           into msgGr
                                                           select (new { key = msgGr.Key })))))
                                                           .SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);
                Int32 noOfNewClients = 0;
                Int32 noOfReturningClients = 0;

                foreach (var client in clients)
                {
                    if (client.key.returning)
                    {
                        ++resultReturningClientsInterval[new DateTime(client.key.Year, client.key.Month, client.key.Day)].value;
                        ++noOfReturningClients;
                    }
                    else
                    {
                        ++resultNewClientsInterval[new DateTime(client.key.Year, client.key.Month, client.key.Day)].value;
                        ++noOfNewClients;
                    }
                }

                List<Dictionary<DateTime, ChartValue>> content = new List<Dictionary<DateTime, ChartValue>>();
                content.Add(resultNewClientsInterval);
                content.Add(resultReturningClientsInterval);
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNewClientsChart), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepReturningClientsChart) }, PrepareJson(content, Resources.Global.RepClientsUnit));
                RepInfoBox IbTotalNoOfClients = new RepInfoBox(noOfNewClients + noOfReturningClients, Resources.Global.RepClientsUnit);
                RepInfoBox IbNoOfNewClients = new RepInfoBox(noOfNewClients, Resources.Global.RepClientsUnit);
                RepInfoBox IbNoOfReturningClients = new RepInfoBox(noOfReturningClients, Resources.Global.RepClientsUnit);
                LinkedList<RepInfoBox> repInfoBoxArray = new LinkedList<RepInfoBox>();
                var repData = new ReportData(new List<RepChartData>() { chartSource },
                        new List<RepInfoBox>() { IbTotalNoOfClients, IbNoOfNewClients, IbNoOfReturningClients });
                var result = new { repData = repData, restoreData = dataToBeSaved };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetReportClientsData", e);
                return Json("request failed", JsonRequestBehavior.AllowGet);
            }
        }
  

        public JsonResult GetReportClientsGrData(String iIntervalStart, String iIntervalEnd,  String iGranularity,String[] iScope =null)
        {
            try
            {
               DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
               DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
               
                Dictionary<DateTime, ChartValue> resultNewClientsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultReturningClientsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);

                Int32 noOfNewClients = 0;
                Int32 noOfReturningClients = 0;

                iScope = iScope != null ? iScope : new string[0];
                var useAllWps = iScope.Length == 0;
                if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                {
                   var clients = (from u in context.Users
                                   where u.UserName.Equals(User.Identity.Name)
                                   select (from wp in u.WorkingPoints
                                           where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                           select (from conv in wp.Conversations
                                                   where conv.Messages.Where(msg => msg.TimeReceived >= intervalStart &&
                                                       msg.TimeReceived <= intervalEnd).Count() > 0
                                                   select (from msg in conv.Messages
                                                           where
                                                               msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd
                                                           group msg by new
                                                           {
                                                               msg.TimeReceived.Day,
                                                               msg.TimeReceived.Month,
                                                               msg.TimeReceived.Year,
                                                               conv.ConvId,
                                                               returning = conv.Messages.Where(convMsg => convMsg.TimeReceived < intervalStart).Count() > 0
                                                           }
                                                               into msgGr
                                                               select (new { key = msgGr.Key })))))
                                                       .SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);

                    foreach (var client in clients)
                    {
                        if (client.key.returning)
                        {
                            ++resultReturningClientsInterval[new DateTime(client.key.Year, client.key.Month, client.key.Day)].value;
                            ++noOfReturningClients;
                        }
                        else
                        {
                            ++resultNewClientsInterval[new DateTime(client.key.Year, client.key.Month, client.key.Day)].value;
                            ++noOfNewClients;
                        }
                    }
                }
                else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                {
                   var clients = (from u in context.Users
                                   where u.UserName.Equals(User.Identity.Name)
                                   select (from wp in u.WorkingPoints
                                           where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                           select (from conv in wp.Conversations
                                                   where conv.Messages.Where(msg => msg.TimeReceived >= intervalStart &&
                                                       msg.TimeReceived <= intervalEnd).Count() > 0
                                                   select (from msg in conv.Messages
                                                           where
                                                               msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd
                                                           group msg by new
                                                           {
                                                               msg.TimeReceived.Day,
                                                               msg.TimeReceived.Month,
                                                               msg.TimeReceived.Year,
                                                               conv.ConvId,
                                                               returning = conv.Messages.Where(convMsg => convMsg.TimeReceived < intervalStart).Count() > 0
                                                           }
                                                               into msgGr
                                                               select (new { key = msgGr.Key })))))
                                                       .SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);


                    var clientsGrByMonth = from client in clients
                                           group client by new { client.key.Month, client.key.Year, client.key.returning } into gr
                                           select new { gr.Key, count = gr.Count() };
                    foreach (var entry in clientsGrByMonth)
                    {
                        var monthDateTime = new DateTime(entry.Key.Year, entry.Key.Month, 1);
                        if (entry.Key.returning)
                        {
                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                resultReturningClientsInterval[intervalStart].value += entry.count;
                            else
                                resultReturningClientsInterval[monthDateTime].value += entry.count;
                        }
                        else
                        {
                            if (DateTime.Compare(monthDateTime, intervalStart) < 0)
                                resultNewClientsInterval[intervalStart].value += entry.count;
                            else
                                resultNewClientsInterval[monthDateTime].value += entry.count;
                        }
                    }
                }
                else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                {
                   var clients = (from u in context.Users
                                   where u.UserName.Equals(User.Identity.Name)
                                   select (from wp in u.WorkingPoints
                                           where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                           select (from conv in wp.Conversations
                                                   where conv.Messages.Where(msg => msg.TimeReceived >= intervalStart &&
                                                       msg.TimeReceived <= intervalEnd).Count() > 0
                                                   select (from msg in conv.Messages
                                                           where
                                                               msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd
                                                           group msg by new
                                                           {
                                                               msg.TimeReceived.Day,
                                                               msg.TimeReceived.Month,
                                                               msg.TimeReceived.Year,
                                                               conv.ConvId,
                                                               returning = conv.Messages.Where(convMsg => convMsg.TimeReceived < intervalStart).Count() > 0
                                                           }
                                                               into msgGr
                                                               select (new { key = msgGr.Key })))))
                                                       .SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);

                    foreach (var entry in clients)
                    {
                        var weekDateTime = FirstDayOfWeekUtility.GetFirstDayOfWeek(
                                                  new DateTime(entry.key.Year,
                                                      entry.key.Month,
                                                      entry.key.Day));
                        if (entry.key.returning)
                        {
                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                ++resultReturningClientsInterval[intervalStart].value;
                            else
                                ++resultReturningClientsInterval[weekDateTime].value;
                        }
                        else
                        {
                            if (DateTime.Compare(weekDateTime, intervalStart) < 0)
                                ++resultNewClientsInterval[intervalStart].value;
                            else
                                ++resultNewClientsInterval[weekDateTime].value;
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
                logger.Error("GetReportClientsGrData", e);
                return Json("request failed", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion // 230 rows
        #region RawData export
        [CustomAuthorizeAtribute(Roles = cExporterOfRawData)]
        public JsonResult GetActivityReport(String iIntervalStart, String iIntervalEnd, String[] iScope = null)
        {
           //filter on start date, end date and scope (all stores or one in particular)
           DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
           DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);

           iScope = iScope != null ? iScope : new string[0];
           var useAllWps = iScope.Length == 0;
           var user = context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
           var allmsg = (from wp in user.WorkingPoints
                         where useAllWps ? true : iScope.Contains(wp.TelNumber)
                         select
                            (from c in wp.Conversations orderby c.StartTime descending
                             where !c.Client.isSupportClient
                             select
                                (from msg in c.Messages
                                 where msg.TimeReceived >= intervalStart &&
                                  msg.TimeReceived <= intervalEnd
                                 orderby msg.TimeReceived
                                 select new
                                 {
                                    Store = wp.Name,
                                    ConvID = msg.ConversationId,
                                    From = msg.From,
                                    To = msg.To,
                                    Text = msg.Text,
                                    ReceivedTime = msg.TimeReceived.ToString()
                                 }))).SelectMany(x => x).SelectMany(x => x);

           //define the headers (first row)
           var header = new string[] { 
              Resources.Global.repRawWpName,
              Resources.Global.repRawConversationId,
              Resources.Global.repRawFrom,
              Resources.Global.repRawTo,
              Resources.Global.repRawText,
              Resources.Global.repRawTimeReceived
           };
           var result = new {
              reportname= Resources.Global.repExportRawRepName,
              header = header,
              rows = allmsg };
           return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Reports page left side menus
        public JsonResult GetReportsMenuItems()
        {
            ReportsMenuItem[] reportsMenuItems = new ReportsMenuItem[] {
               new ReportsMenuItem(cConvsMenuContainerID, Resources.Global.RepConversations, false, 0, "getReportById", "Conversations"),
               new ReportsMenuItem(cConvsOverviewMenuID, Resources.Global.RepOverview, true, cConvsMenuContainerID, "getReportById","ConversationsOverview"), 
               new ReportsMenuItem(cConvsIncomingVsOutgoingID, Resources.Global.RepIncomingVsOutgoing, true, cConvsMenuContainerID,"getReportById","ConversationsIncomingVsOutgoing"),
               new ReportsMenuItem(cConvsPosVsNegID, Resources.Global.RepPositiveAndNegative, true, cConvsMenuContainerID, "getReportById","ConversationsPositiveAndNegative"),
               new ReportsMenuItem(cConvsTagsOverviewID, Resources.Global.RepTags, true, cConvsMenuContainerID, "getReportById","ConversationsTagsOverview"),
               new ReportsMenuItem(cClientsOverviewID, Resources.Global.RepClients, false, 0, "getReportById","Clients"),
               new ReportsMenuItem(cClientsNewVsReturningID, Resources.Global.RepNewVsReturning, true, cClientsOverviewID,"getReportById","CustomersNewVsReturning")};

            return Json(reportsMenuItems, JsonRequestBehavior.AllowGet);
        }
        #endregion
              
        public JsonResult GetReportById(int reportId)
        {
            var hashTable = new Dictionary<int, Report>();

            var report2 = new Report(cConvsOverviewMenuID, Resources.Global.RepOverview, "/Reports/GetReportOverviewData",
                new ReportSection[] {
                                        new ReportSection("FirstSection", "Side by side: Total Number of Messages", 0,iChartSource: "/Reports/GetReportOverviewGrDataSideBySide"),
                                        new ReportSection("FirstSection", Resources.Global.RepOverviewChartTitle, 1, iChartSource: "/Reports/GetReportOverviewGrData", iHasExportRawData: User.IsInRole(cExporterOfRawData)),
                                        new ReportSection("SecondSection", Resources.Global.RepTotalNoOfSms, 0),
                                        new ReportSection("SecondSection", Resources.Global.RepAvgNoOfSmsPerDay, 1),
                                        new ReportSection("SecondSection", Resources.Global.RepTotalNoOfClients, 2),
                                        new ReportSection("SecondSection", Resources.Global.RepAvgNoOfSmsPerClient, 3),
                                        new ReportSection("SecondSection", Resources.Global.RepAvgResponseTime, 4)
                                      });
            var report3 = new Report(cConvsIncomingVsOutgoingID, Resources.Global.RepIncomingVsOutgoing, "/Reports/GetReportIncomingOutgoingData",
                new ReportSection[] { 
                                        new ReportSection("FirstSection", iGroupId: "7", iTitle:Resources.Global.RepIncomingOutgoingChartTitle, iDataIndex: 0, iChartSource: "/Reports/GetReportIncomingOutgoingGrData"), 
                                        new ReportSection("SecondSection", Resources.Global.RepNoOfIncomingSms, 0),
                                        new ReportSection("SecondSection", Resources.Global.RepNoOfOutgoingSms, 1),
                                        new ReportSection("SecondSection", Resources.Global.RepTotalNoOfClients, 2),                                                                                        
                                        new ReportSection("SecondSection", Resources.Global.RepAvgNoOfIncomingSmsPerConversation, 3),
                                        new ReportSection("SecondSection", Resources.Global.RepAvgNoOfOutgoingSmsPerConversation, 4),
                                        new ReportSection("ThirdSection", Resources.Global.RepIncomingOutgoingChartTitle, iGroupId: "7", iDataIndex: 1)
                                       });
            var report4 = new Report(cConvsPosVsNegID, Resources.Global.RepPositiveAndNegativeTitle, "/Reports/GetReportPosNegData",
                new ReportSection[] {  
                                       
                                        new ReportSection("FirstSection", iDataIndex: 0, iChartSource:"/Reports/GetReportPosNegEvolutionGr", iSectionId:"1", iTitle:Resources.Global.RepPositiveNegativeEvolutionChartTitle, iTooltip: Resources.Global.RepTooltipPosNegFeedbackEvolution),                                                                                                                                                            
                                        new ReportSection("FirstSection", iChartSource:"/Reports/GetReportPosNegTransitionsGr", iDataIndex: 1, iSectionId: "2", iGroupId: "7",  iTitle:Resources.Global.RepPositiveNegativeTransitionsChartTitle, iTooltip: Resources.Global.RepTooltipPosNegFeedbackTransitions),
                                        new ReportSection("ThirdSection", iDataIndex: 2, iSectionId: "3", iGroupId: "7", iTitle: Resources.Global.RepPositiveNegativeTransitionsChartTitle),
                                        new ReportSection("FirstSection", iChartSource:"/Reports/GetReportPosNegActivityGr", iDataIndex: 3, iSectionId: "4", iTitle: Resources.Global.RepPositiveNegativeActivityChartTitle, iTooltip: Resources.Global.RepTooltipPosNegFeedbackActivity, 
                                        iOptions: new ReportResourceOptions(iColors: new List<String>{"#3366cc", "#dc3912", "#667189", "#b48479"}))
            });
            var report5 = new Report(cConvsTagsOverviewID, Resources.Global.RepTags, "/Reports/GetReportTagsData",
                new ReportSection[] { 
                                        new ReportSection("TagsReportSection", Resources.Global.repTagFilteringReportTitle,iDataIndex: 0, iGroupId: "5",iChartSource:"/Reports/GetTagReportDataGrid"),
                                        new ReportSection("FirstSection",iGroupId: "7", iDataIndex: 1,
                                           iTitle:Resources.Global.RepNoOfConversationsByTagsChartTitle,
                                           iOptions: new ReportResourceOptions(iSeriesType : Constants.BARS_CHART_STYLE)                                           
                                           ),
                                        new ReportSection("SecondSection", iGroupId: "7", iDataIndex: 0, iTitle:Resources.Global.RepMostUsedTag),
                                        new ReportSection("SecondSection",iGroupId: "7", iDataIndex: 1, iTitle: Resources.Global.RepAvgNoOfTagsPerConversation)
                                        
                                     });
            var report7 = new Report(cClientsNewVsReturningID, Resources.Global.RepNewVsReturning, "/Reports/GetReportClientsData",
                new ReportSection[] { 
                                        new ReportSection("FirstSection", iDataIndex: 0, iChartSource: "/Reports/GetReportClientsGrData", iTitle: Resources.Global.RepNewReturningClientsChartTitle),                                                                                                                                                            
                                        new ReportSection("SecondSection", iDataIndex: 0, iTitle: Resources.Global.RepTotalNoOfClients),
                                        new ReportSection("SecondSection", iDataIndex: 1, iTitle: Resources.Global.RepNoOfNewClients),
                                        new ReportSection("SecondSection", iDataIndex: 2, iTitle: Resources.Global.RepNoOfReturningClients)                                                                                                                                                    
                                   });

            hashTable.Add(cConvsOverviewMenuID, report2);
            hashTable.Add(cConvsIncomingVsOutgoingID, report3);
            hashTable.Add(cConvsPosVsNegID, report4);
            hashTable.Add(cConvsTagsOverviewID, report5);
            hashTable.Add(cClientsNewVsReturningID, report7);
            //hashTable.Add(7, report7);

            return Json(hashTable[reportId], JsonRequestBehavior.AllowGet);

        }
        
        #region Helper methods
        private String TransformDate(DateTime iDate, String pattern)
        {
            // TODO: Look for a library to convert to different local formats
            var transformedDate = "";
            if (pattern.Equals(cDateFormat1))
            {
                var day = (iDate.Day < 10) ? "0" + iDate.Day.ToString() : iDate.Day.ToString();
                var month = (iDate.Month < 10) ? "0" + iDate.Month.ToString() : iDate.Month.ToString();
                transformedDate = day + "-" + month;
            }
            else if (pattern.Equals(cDateFormat2))
            {
                var day = (iDate.Day < 10) ? "0" + iDate.Day.ToString() : iDate.Day.ToString();
                var month = (iDate.Month < 10) ? "0" + iDate.Month.ToString() : iDate.Month.ToString();
                transformedDate = day + "/" + month + "/" + iDate.Year;
            }
            else if (pattern.Equals(cDateFormat3))
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
                    totalNoOfSms += (from msg in conv.Messages where (msg.TimeReceived >= iIntervalStart && msg.TimeReceived <= iIntervalEnd) select msg).Count();
                }
            }
            return totalNoOfSms;
        }

        public int ComputeTotalNoOfClients(DateTime iIntervalStart, DateTime intervalEnd, IEnumerable<WorkingPoint> iWorkingPoints)
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

        private IEnumerable<Message> GetMessages(DateTime iIntervalStart, DateTime iIntervalEnd, String iUser, String[] scope, smsfeedbackEntities dbContext)
        {
            if (scope.Length == 0)
            {
                var msgsCollectionOfCollections = from u in dbContext.Users
                                                  where u.UserName == iUser
                                                  select (from wp in u.WorkingPoints
                                                          select (
                                                          from conv in wp.Conversations
                                                          select (from msg in conv.Messages
                                                                  where (msg.TimeReceived >= iIntervalStart && msg.TimeReceived <= iIntervalEnd)
                                                                  select msg)
                                                                  )
                                                           );
                var msgs = msgsCollectionOfCollections.SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);
                return msgs;
            }
            else
            {
                var msgsCollectionOfCollections = from u in dbContext.Users
                                                  where u.UserName == iUser
                                                  select (from wp in u.WorkingPoints
                                                          where scope.Contains(wp.TelNumber)
                                                          select (
                                                          from conv in wp.Conversations
                                                          select (from msg in conv.Messages
                                                                  where (msg.TimeReceived >= iIntervalStart && msg.TimeReceived <= iIntervalEnd)
                                                                  select msg)
                                                                  )
                                                           );
                var msgs = msgsCollectionOfCollections.SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);
                return msgs;
            }


        }

        private Dictionary<string,IEnumerable<Message>> GetMessagesSideBySide(DateTime iIntervalStart, DateTime iIntervalEnd, String iUser, String[] scope, smsfeedbackEntities dbContext)
        {
           bool workOnAllWps = scope.Length == 0 ? true : false;
           var result = new Dictionary<string,IEnumerable<Message>>();
           var msgsCollectionOfCollections = from u in dbContext.Users
                                             where u.UserName == iUser
                                             select (from wp in u.WorkingPoints
                                                     where workOnAllWps ? true : scope.Contains(wp.TelNumber)
                                                     select (
                                                     from conv in wp.Conversations
                                                     select (from msg in conv.Messages
                                                             where (msg.TimeReceived >= iIntervalStart && msg.TimeReceived <= iIntervalEnd)
                                                             select new { WpId = wp.TelNumber, Msg =msg })));
           var msgs = msgsCollectionOfCollections.SelectMany(x => x).SelectMany(x => x).SelectMany(x => x).GroupBy(c => new
           {
              c.WpId
           }, c => c, (key, g) => new
           {
              key = key,
              msgs = from item in g select item.Msg
           });
           foreach (var gr in msgs)
           {
              result.Add(gr.key.WpId, gr.msgs);
           }              
          return result;
        }

        private int ComputeNoOfIncomingSms(DateTime iIntervalStart, DateTime iIntervalEnd, IEnumerable<WorkingPoint> iWorkingPoints)
        {
            var incomingNoOfSms = 0;
            foreach (var wp in iWorkingPoints)
            {
                foreach (var conv in wp.Conversations)
                {
                    var msgsTo = (from msg in conv.Messages where (msg.TimeReceived >= iIntervalStart & msg.TimeReceived <= iIntervalEnd & (msg.To == wp.TelNumber || msg.To.StartsWith(wp.ShortID))) select msg).Count();
                    incomingNoOfSms += msgsTo;
                }
            }
            return incomingNoOfSms;
        }

        private int ComputeNoOfOutgoingSms(DateTime iIntervalStart, DateTime iIntervalEnd, IEnumerable<WorkingPoint> iWorkingPoints)
        {
            var outgoingNoOfSms = 0;
            foreach (var wp in iWorkingPoints)
            {
                var conversations = from conv in wp.Conversations select conv;
                foreach (var conv in conversations)
                {
                    var msgsFrom = (from msg in conv.Messages where (msg.TimeReceived >= iIntervalStart & msg.TimeReceived <= iIntervalEnd & (msg.From == wp.TelNumber || msg.From.StartsWith(wp.ShortID))) select msg).Count();
                    outgoingNoOfSms += msgsFrom;
                }
            }
            return outgoingNoOfSms;
        }

        private Dictionary<DateTime, ChartValue> InitializeInterval(DateTime intervalStart, DateTime intervalEnd, string iGranularity)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar calendar = dfi.Calendar;
            Dictionary<DateTime, ChartValue> resultInterval = null;

            if (iGranularity.Equals(Constants.DAY_GRANULARITY))
            {
                resultInterval = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => new ChartValue(0, TransformDate(d, "dd/mm")));
            }
            else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
            {
                resultInterval = new Dictionary<DateTime, ChartValue>();
                for (DateTime i = intervalStart; i < intervalEnd; i = i.AddMonths(1))
                {
                    var currentDate = (DateTime.Compare(new DateTime(i.Year, i.Month, 1), intervalStart) <= 0) ? intervalStart : new DateTime(i.Year, i.Month, 1);
                    var endOfTheMonth = (DateTime.Compare(new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month)), intervalEnd) > 0) ?
                                                                    intervalEnd : new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
                    resultInterval.Add(currentDate, new ChartValue(0, TransformDate(currentDate, "dd/mm/yyyy") + " » " + TransformDate(endOfTheMonth, "dd/mm/yyyy")));
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
                    resultInterval.Add(currentDate, new ChartValue(0, TransformDate(currentDate, "dd/mm/yyyy") + " » " + TransformDate(endOfTheWeek, "dd/mm/yyyy")));
                    // executed just first time
                    i = (DateTime.Compare(i, intervalStart) == 0) ? FirstDayOfWeekUtility.GetFirstDayOfWeek(i) : i;
                }
            }

            return resultInterval;
        }

        private List<RepDataRow> PrepareJson(List<Dictionary<DateTime, ChartValue>> source, String unitOfMeasurement)
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

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }

    }

    class KeyAndCount
    {
        public string key { get; set; }
        public int count { get; set; }

        public KeyAndCount(string iKey, int iCount)
        {
            key = iKey;
            count = iCount;
        }
    }

    class MsgInfoWrapper
    {
        public MsgInfo Key;
    }

    class MsgInfo
    {
        public bool incoming;
        public DateTime TimeReceived;
        public int Id;
    }
}