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
              var repData = new ReportData(new List<RepChartData>() { chartContentWrapper, sideBySide },
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
                 
                Dictionary<DateTime, ChartValue> resultPositiveTagsEv1Interval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegativeTagsEv1Interval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultPosNegTagsTrInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultNegPosTagsTrInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);

                iScope = iScope != null ? iScope : new string[0];
                var useAllWps = iScope.Length == 0;                
                IEnumerable<ConversationHistory> convEvents = GetAggregatedConvEventsInternal(iScope, intervalStart, intervalEnd);

                RepChartData evolutionChartSource = GetAggregatedFeedbackEvolutionChart(iGranularity, intervalStart, intervalEnd, resultPositiveTagsEv1Interval, resultNegativeTagsEv1Interval, convEvents);

                int totalNumberOfPosToNegTransitions;
                int totalNumberOfNegToPosTransitions;
                RepChartData transitionsChartSource = GetAggregatedPosNegTransitionsChart(iGranularity, intervalStart, intervalEnd, resultPosNegTagsTrInterval, resultNegPosTagsTrInterval, convEvents,
                   out totalNumberOfNegToPosTransitions, out totalNumberOfPosToNegTransitions);

                List<RepDataRow> transitionsPieChartContent = new List<RepDataRow>();
                RepDataRow row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(Resources.Global.RepPosToNegFeedback, Resources.Global.RepPosToNegFeedback), new RepDataRowCell(totalNumberOfPosToNegTransitions, totalNumberOfPosToNegTransitions + " " + Resources.Global.RepPosToNegFeedback) });
                RepDataRow row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(Resources.Global.RepNegToPosFeedback, Resources.Global.RepNegToPosFeedback), new RepDataRowCell(totalNumberOfNegToPosTransitions, totalNumberOfNegToPosTransitions + " " + Resources.Global.RepNegToPosFeedback) });
                transitionsPieChartContent.Add(row1);
                transitionsPieChartContent.Add(row2);
                RepChartData transitionsPieChartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, Resources.Global.RepTypeTable), new RepDataColumn("18", Constants.STRING_COLUMN_TYPE, Resources.Global.RepValueTable) }, transitionsPieChartContent);

                var posEvolutionSideBySideChart = PosEvolutionSideBySideInternal(iIntervalStart, iIntervalEnd, Constants.DAY_GRANULARITY, iScope);
                var negEvolutionSideBySideChart = NegEvolutionSideBySideInternal(iIntervalStart, iIntervalEnd, Constants.DAY_GRANULARITY, iScope);
                ReportData repData = new ReportData(new List<RepChartData>() {posEvolutionSideBySideChart,negEvolutionSideBySideChart, evolutionChartSource, transitionsChartSource, transitionsPieChartSource}, null);
                var result = new { repData = repData, restoreData = dataToBeSaved };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetReportPosNegData", e);
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
                IEnumerable<ConversationHistory> convEvents = (from u in context.Users
                                                               where u.UserName.Equals(User.Identity.Name)
                                                               select (from wp in u.WorkingPoints
                                                                       where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                                                       select (from conv in wp.Conversations
                                                                               where !conv.Client.isSupportClient
                                                                               select (from convEvent in conv.ConversationEvents
                                                                                       where (convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT) ||
                                                                                           convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT))
                                                                                           &&
                                                                                          (convEvent.Message.TimeReceived >= intervalStart && convEvent.Message.TimeReceived <= intervalEnd)
                                                                                       select convEvent)))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);

                int totalNumberOfPosToNegTransitions;
                int totalNumberOfNegToPosTransitions;
                RepChartData transitionsChartSource = GetAggregatedPosNegTransitionsChart(iGranularity, intervalStart, intervalEnd, resultPosNegTagsTrInterval, resultNegPosTagsTrInterval, convEvents,
                   out totalNumberOfNegToPosTransitions,out  totalNumberOfPosToNegTransitions);

                return Json(transitionsChartSource, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetReportPosNegTransitionsData", e);
                return Json("request failed", JsonRequestBehavior.AllowGet);
            }
        }

        private RepChartData GetAggregatedPosNegTransitionsChart
           (String iGranularity,
           DateTime intervalStart,
           DateTime intervalEnd,
           Dictionary<DateTime, ChartValue> resultPosNegTagsTrInterval,
           Dictionary<DateTime, ChartValue> resultNegPosTagsTrInterval,
           IEnumerable<ConversationHistory> convEvents,
           out int totalNumberOfNegToPosTransitions,
           out int totalNumberOfPosToNegTransitions)
        {
           totalNumberOfNegToPosTransitions = 0;
           totalNumberOfPosToNegTransitions = 0;
           if (iGranularity.Equals(Constants.DAY_GRANULARITY))
           {
              var convEventsGr = (from convEvent in convEvents
                                  group convEvent by new { evOccurDate = convEvent.Message.TimeReceived.Date, eventType = convEvent.EventTypeName }
                                     into g
                                     select new { key = g.Key, count = g.Count(), elements = g.ToList() });

              foreach (var convEvent in convEventsGr)
              {
                 if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                 {                   
                    resultNegPosTagsTrInterval[convEvent.key.evOccurDate].value += convEvent.count;
                    totalNumberOfNegToPosTransitions += convEvent.count;
                 }
                 else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                 {
                    resultPosNegTagsTrInterval[convEvent.key.evOccurDate].value += convEvent.count;
                    totalNumberOfPosToNegTransitions += convEvent.count;
                 }
              }
           }
           else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
           {
              var convEventsGr = (from convEvent in convEvents
                                  group convEvent by new
                                  {
                                     month = convEvent.Message.TimeReceived.Date.Month,
                                     year = convEvent.Message.TimeReceived.Date.Year,
                                     eventType = convEvent.EventTypeName
                                  }
                                     into g
                                     select new { key = g.Key, count = g.Count() });
              foreach (var convEvent in convEventsGr)
              {
                 //DA the interval value is either the calculated month-year combination or the min interval value or the max interval value in case of border cases
                 var intervalDate = new DateTime(convEvent.key.year, convEvent.key.month, 1);
                 intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                 intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                 if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                 {
                    resultNegPosTagsTrInterval[intervalDate].value += convEvent.count;
                 }
                 else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                 {
                    resultPosNegTagsTrInterval[intervalDate].value += convEvent.count;
                 }
              }
           }
           else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
           {
              var convEventsGr = (from convEvent in convEvents
                                  group convEvent by new
                                  {
                                     evOccurDate = FirstDayOfWeekUtility.GetFirstDayOfWeek(convEvent.Message.TimeReceived),
                                     eventType = convEvent.EventTypeName
                                  }
                                     into g
                                     select new { key = g.Key, count = g.Count() });

              foreach (var convEvent in convEventsGr)
              {
                 var intervalDate = convEvent.key.evOccurDate;
                 intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                 intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                 if (convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                 {
                    resultNegPosTagsTrInterval[intervalDate].value += convEvent.count;
                 }
                 else if (convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                 {
                    resultPosNegTagsTrInterval[intervalDate].value += convEvent.count;
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
           return transitionsChartSource;
        }


        public JsonResult GetReportPosNegEvolutionGr(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
        {
           try
           {
              DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
              DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);

              Dictionary<DateTime, ChartValue> resultPositiveTagsEvInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
              Dictionary<DateTime, ChartValue> resultNegativeTagsEvInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);

              IEnumerable<ConversationHistory> convEvents = GetAggregatedConvEventsInternal(iScope, intervalStart, intervalEnd);

              RepChartData evolutionChartSource = GetAggregatedFeedbackEvolutionChart(iGranularity, intervalStart, intervalEnd, resultPositiveTagsEvInterval, resultNegativeTagsEvInterval, convEvents);

              return Json(evolutionChartSource, JsonRequestBehavior.AllowGet);
           }
           catch (Exception e)
           {
              logger.Error("GetReportPosNegEvolutionData", e);
              return Json("request failed", JsonRequestBehavior.AllowGet);
           }
        }

        private RepChartData GetAggregatedFeedbackEvolutionChart(
           String iGranularity,
           DateTime intervalStart,
           DateTime intervalEnd,
           Dictionary<DateTime, ChartValue> resultPositiveTagsEvInterval,
           Dictionary<DateTime, ChartValue> resultNegativeTagsEvInterval,
           IEnumerable<ConversationHistory> convEvents)
        {
           if (iGranularity.Equals(Constants.DAY_GRANULARITY))
           {
              var convEventsGr = (from convEvent in convEvents
                                  group convEvent by new
                                  {
                                     evOccurDate = convEvent.Message.TimeReceived.Date,
                                     eventType = convEvent.EventTypeName
                                  } into g select new { key = g.Key, count = g.Count() });

              foreach (var convEvent in convEventsGr)
              {
                 if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                 {
                    resultPositiveTagsEvInterval[convEvent.key.evOccurDate].value += convEvent.count;
                 }
                 else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                 {
                    resultNegativeTagsEvInterval[convEvent.key.evOccurDate].value += convEvent.count;
                 }
              }

           }
           else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
           {
              var convEventsGr = (from convEvent in convEvents
                                  group convEvent by new
                                  {
                                     month = convEvent.Message.TimeReceived.Date.Month,
                                     year = convEvent.Message.TimeReceived.Date.Year,
                                     eventType = convEvent.EventTypeName
                                  } into g select new { key = g.Key, count = g.Count() });


              foreach (var convEvent in convEventsGr)
              {
                 //DA the interval value is either the calculated month-year combination or the min interval value or the max interval value in case of border cases
                 var intervalDate = new DateTime(convEvent.key.year, convEvent.key.month, 1);
                 intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                 intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                 if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                 {

                    resultPositiveTagsEvInterval[intervalDate].value += convEvent.count;
                 }
                 else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                 {
                    resultNegativeTagsEvInterval[intervalDate].value += convEvent.count;
                 }
              }

           }
           else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
           {
              var convEventsGr = (from convEvent in convEvents
                                  group convEvent by new
                                  {
                                     evOccurDate = FirstDayOfWeekUtility.GetFirstDayOfWeek(convEvent.Message.TimeReceived),
                                     eventType = convEvent.EventTypeName
                                  } into g select new { key = g.Key, count = g.Count() });

              foreach (var convEvent in convEventsGr)
              {
                 var intervalDate = convEvent.key.evOccurDate;
                 intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                 intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                 if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                 {
                    resultPositiveTagsEvInterval[intervalDate].value += convEvent.count;
                 }
                 else if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                 {
                    resultNegativeTagsEvInterval[intervalDate].value += convEvent.count;
                 }
              }
           }

           List<Dictionary<DateTime, ChartValue>> evolutionChartContent = new List<Dictionary<DateTime, ChartValue>>();
           evolutionChartContent.Add(resultPositiveTagsEvInterval);
           evolutionChartContent.Add(resultNegativeTagsEvInterval);
           RepChartData evolutionChartSource = new RepChartData(new RepDataColumn[] { 
                    new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE,"Positive messages"), 
                    new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, "Negative messages") },
               PrepareJson(evolutionChartContent, Resources.Global.RepConversationsUnit));
           return evolutionChartSource;
        }

        private IEnumerable<ConversationHistory> GetAggregatedConvEventsInternal(String[] iScope, DateTime intervalStart, DateTime intervalEnd)
        {
           iScope = iScope != null ? iScope : new string[0];
           var useAllWps = iScope.Length == 0;
           // GLOBAL SCOPE
           IEnumerable<ConversationHistory> convEvents = (from u in context.Users
                                                          where u.UserName.Equals(User.Identity.Name)
                                                          select (from wp in u.WorkingPoints
                                                                  where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                                                  select (from conv in wp.Conversations
                                                                          where !conv.Client.isSupportClient                                                                      
                                                                          select (from convEvent in conv.ConversationEvents
                                                                                  where
                                                                                  (convEvent.Message.TimeReceived >= intervalStart && convEvent.Message.TimeReceived <= intervalEnd) &&
                                                                                      (convEvent.EventTypeName.Equals(Constants.POS_ADD_EVENT) ||
                                                                                      convEvent.EventTypeName.Equals(Constants.NEG_ADD_EVENT) ||
                                                                                      convEvent.EventTypeName.Equals(Constants.POS_REMOVE_EVENT) ||
                                                                                      convEvent.EventTypeName.Equals(Constants.NEG_REMOVE_EVENT) ||
                                                                                      convEvent.EventTypeName.Equals(Constants.POS_TO_NEG_EVENT) ||
                                                                                      convEvent.EventTypeName.Equals(Constants.NEG_TO_POS_EVENT)) 
                                                                                  select convEvent)))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);
           return convEvents;
        }

        private Dictionary<string, IEnumerable<ConversationHistory>> GetSideBySideConvEventsInternal(String iGranularity, String[] iScope, DateTime intervalStart, DateTime intervalEnd)
        {
           var resIntervals = new Dictionary<string, Dictionary<DateTime, ChartValue>>();
           foreach (var location in iScope)
           {
              resIntervals.Add(location, InitializeInterval(intervalStart, intervalEnd, iGranularity));
           }
           bool workOnAllWps = iScope.Length == 0 ? true : false;
           var convEvents = GetAggregatedConvEventsInternal(iScope,intervalStart,intervalEnd);
           var convEventsGr = convEvents.GroupBy(c => new
           {
              c.Conversation.WorkingPoint.TelNumber
           }, c => c, (key, g) => new
           {
              key = key,
              convEvents = from item in g select item
           });
           var result = new Dictionary<string, IEnumerable<ConversationHistory>>();
           foreach (var gr in convEventsGr)
           {
              result.Add(gr.key.TelNumber, gr.convEvents);
           }
           return result;
        }

        public JsonResult PosEvolutionSideBySideInternalGr(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
        {
           iScope = iScope != null? iScope : new string[0];
           var chart = PosEvolutionSideBySideInternal(iIntervalStart, iIntervalEnd, iGranularity, iScope);
           return Json(chart, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NegEvolutionSideBySideInternalGr(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
        {
           iScope = iScope != null ? iScope : new string[0];
           var chart = NegEvolutionSideBySideInternal(iIntervalStart, iIntervalEnd, iGranularity, iScope);
           return Json(chart, JsonRequestBehavior.AllowGet);
        }

        public RepChartData NegEvolutionSideBySideInternal(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
        {
           DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
           DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
           if (iScope.Length == 0)
           {
              iScope = (from u in context.Users where u.UserName == User.Identity.Name select (from wp in u.WorkingPoints select wp.TelNumber)).SelectMany(x => x).ToArray();
           }
           var resIntervals = new Dictionary<string, Dictionary<DateTime, ChartValue>>();
           foreach (var location in iScope)
           {
              resIntervals.Add(location, InitializeInterval(intervalStart, intervalEnd, iGranularity));
           }
           var grConvEvents = GetSideBySideConvEventsInternal(iGranularity, iScope, intervalStart, intervalEnd);
           foreach (var gr in grConvEvents)
           {
              if (iGranularity.Equals(Constants.DAY_GRANULARITY))
              {
                 var convEventsGr = (from convEvent in gr.Value
                                     group convEvent by new
                                     {
                                        evOccurDate = convEvent.Message.TimeReceived.Date,
                                        eventType = convEvent.EventTypeName
                                     } into g
                                     select new { key = g.Key, count = g.Count() });

                 foreach (var convEvent in convEventsGr)
                 {
                    if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                    {
                       resIntervals[gr.Key][convEvent.key.evOccurDate].value += convEvent.count;

                    }
                 }
              }
              else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
              {
                 var convEventsGr = (from convEvent in gr.Value
                                     group convEvent by new
                                     {
                                        month = convEvent.Message.TimeReceived.Date.Month,
                                        year = convEvent.Message.TimeReceived.Date.Year,
                                        eventType = convEvent.EventTypeName
                                     } into g
                                     select new { key = g.Key, count = g.Count() });


                 foreach (var convEvent in convEventsGr)
                 {
                    //DA the interval value is either the calculated month-year combination or the min interval value or the max interval value in case of border cases
                    var intervalDate = new DateTime(convEvent.key.year, convEvent.key.month, 1);
                    intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                    intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                    if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                    {
                       resIntervals[gr.Key][intervalDate].value += convEvent.count;

                    }
                 }

              }
              else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
              {
                 var convEventsGr = (from convEvent in gr.Value
                                     group convEvent by new
                                     {
                                        evOccurDate = FirstDayOfWeekUtility.GetFirstDayOfWeek(convEvent.Message.TimeReceived),
                                        eventType = convEvent.EventTypeName
                                     } into g
                                     select new { key = g.Key, count = g.Count() });

                 foreach (var convEvent in convEventsGr)
                 {
                    var intervalDate = convEvent.key.evOccurDate;
                    intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                    intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                    if (convEvent.key.eventType.Equals(Constants.NEG_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.POS_TO_NEG_EVENT))
                    {
                       resIntervals[gr.Key][intervalDate].value += convEvent.count;

                    }
                 }
              }
           }
           List<Dictionary<DateTime, ChartValue>> posEvolutionChartContent = new List<Dictionary<DateTime, ChartValue>>();
           foreach (var item in resIntervals)
           {
              posEvolutionChartContent.Add(item.Value);
           }
           List<RepDataColumn> columnDefinition = new List<RepDataColumn> { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date") };
           foreach (var location in iScope)
           {
              columnDefinition.Add(new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, location));

           }
           RepChartData chartSource = new RepChartData(
              columnDefinition,
                 PrepareJson(posEvolutionChartContent, Resources.Global.RepSmsUnit));
           return chartSource;
        }

        public RepChartData PosEvolutionSideBySideInternal(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
        {
           DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
           DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);         
           if (iScope.Length == 0)
           {
              iScope = (from u in context.Users where u.UserName == User.Identity.Name select (from wp in u.WorkingPoints select wp.TelNumber)).SelectMany(x => x).ToArray();
           }           
           var resIntervals = new Dictionary<string, Dictionary<DateTime, ChartValue>>();
           foreach (var location in iScope)
           {
              resIntervals.Add(location, InitializeInterval(intervalStart, intervalEnd, iGranularity));
           }
           var grConvEvents = GetSideBySideConvEventsInternal(iGranularity, iScope, intervalStart, intervalEnd);          
           foreach (var gr in grConvEvents)
           {
              if (iGranularity.Equals(Constants.DAY_GRANULARITY))
              {
                 var convEventsGr = (from convEvent in gr.Value
                                     group convEvent by new
                                     {
                                        evOccurDate = convEvent.Message.TimeReceived.Date,
                                        eventType = convEvent.EventTypeName
                                     } into g
                                     select new { key = g.Key, count = g.Count() });

                 foreach (var convEvent in convEventsGr)
                 {
                    if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                    {
                       resIntervals[gr.Key][convEvent.key.evOccurDate].value += convEvent.count;

                    }
                 }
              }
              else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
              {
                 var convEventsGr = (from convEvent in gr.Value
                                     group convEvent by new
                                     {
                                        month = convEvent.Message.TimeReceived.Date.Month,
                                        year = convEvent.Message.TimeReceived.Date.Year,
                                        eventType = convEvent.EventTypeName
                                     } into g
                                     select new { key = g.Key, count = g.Count() });


                 foreach (var convEvent in convEventsGr)
                 {
                    //DA the interval value is either the calculated month-year combination or the min interval value or the max interval value in case of border cases
                    var intervalDate = new DateTime(convEvent.key.year, convEvent.key.month, 1);
                    intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                    intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                    if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                    {
                       resIntervals[gr.Key][intervalDate].value += convEvent.count;

                    }
                 }

              }
              else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
              {
                 var convEventsGr = (from convEvent in gr.Value
                                     group convEvent by new
                                     {
                                        evOccurDate = FirstDayOfWeekUtility.GetFirstDayOfWeek(convEvent.Message.TimeReceived),
                                        eventType = convEvent.EventTypeName
                                     } into g
                                     select new { key = g.Key, count = g.Count() });

                 foreach (var convEvent in convEventsGr)
                 {
                    var intervalDate = convEvent.key.evOccurDate;
                    intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                    intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                    if (convEvent.key.eventType.Equals(Constants.POS_ADD_EVENT) || convEvent.key.eventType.Equals(Constants.NEG_TO_POS_EVENT))
                    {
                       resIntervals[gr.Key][intervalDate].value += convEvent.count;

                    }
                 }
              }
           }
           List<Dictionary<DateTime, ChartValue>> posEvolutionChartContent = new List<Dictionary<DateTime, ChartValue>>();
           foreach (var item in resIntervals)
           {
              posEvolutionChartContent.Add(item.Value);
           }
           List<RepDataColumn> columnDefinition = new List<RepDataColumn> { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date") };
           foreach (var location in iScope)
           {
              columnDefinition.Add(new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, location));

           }
           RepChartData chartSource = new RepChartData(
              columnDefinition,
                 PrepareJson(posEvolutionChartContent, Resources.Global.RepSmsUnit));
           return chartSource;           
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
                var user = User.Identity.Name;
                var items = (from u in context.Users
                       where u.UserName.Equals(user)
                       select (from wp in u.WorkingPoints
                               where useAllWps ? true : iScope.Contains(wp.TelNumber)
                               select (from conv in wp.Conversations
                                       where !conv.Client.isSupportClient                                                                               
                                       select (from msg in conv.Messages
                                               where (msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd) &&
											   (msg.ConversationId.StartsWith(msg.From) || msg.From.EndsWith("@txtfeedback.net"))
											   group msg by new { 											   
												msg.ConversationId,
												msg.TimeReceived.Year,
												msg.TimeReceived.Month,
												msg.TimeReceived.Day
												} into grp
											   select new  {											   
												grp.Key.ConversationId,											   
												grp.Key.Year,
												grp.Key.Month,
												grp.Key.Day,												
												messageBeforeInterval = conv.Messages.Where(convMsg => convMsg.TimeReceived < intervalStart).Count() > 0,
												count = grp.Count()
											   })))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);

                  //DA here we have to leave the LINQ to SQL world and move to LINQ to entities
                var asEntity = items.ToList();
                int newCustomersThisInterval = 0;
                int returningCustomersThisInterval = 0;
                
                var conversationsPerDate = asEntity.GroupBy(x => new DateTime(x.Year, x.Month, x.Day));
               var datesPerConversations = from e in asEntity
                                           group e by new { e.ConversationId }
                                              into datesGroups
                                              select new {
                                                 key = datesGroups.Key.ConversationId,
                                                 dates = datesGroups.Select(x => new DateTime(x.Year, x.Month, x.Day)) };
               int totalNumberOfClientsGivingFeedback = datesPerConversations.Count();
                

                foreach (var date in conversationsPerDate)
                {
                   int newCustomersThisDay = 0;
                   int returningCustomersThisDay = 0;
                   foreach (var conv in date)
                   {
                      //if we have messages before the start of the interval for sure we are dealing with a returning customer
                      if (conv.messageBeforeInterval == true)
                      {
                         returningCustomersThisDay++;
                      }
                      else
                      {                         
                         //if we had message in another day, prior to this -> returning, otherwise new
                         var messagesInEarlierDaysExist = datesPerConversations.Where(x => x.key == conv.ConversationId).Select(x => x.dates).SelectMany(x => x).Any(x => x < date.Key);
                         if (messagesInEarlierDaysExist)
                         {
                            returningCustomersThisDay++;
                         }
                         else
                         {
                            newCustomersThisDay++;
                         }
                      }                      
                   }
                   resultReturningClientsInterval[date.Key].value = returningCustomersThisDay;
                   returningCustomersThisInterval += returningCustomersThisDay;
                   resultNewClientsInterval[date.Key].value = newCustomersThisDay;
                   newCustomersThisInterval += newCustomersThisDay;
                }

                List<Dictionary<DateTime, ChartValue>> content = new List<Dictionary<DateTime, ChartValue>>();
                content.Add(resultNewClientsInterval);
                content.Add(resultReturningClientsInterval);
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date"), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepNewClientsChart), new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, Resources.Global.RepReturningClientsChart) }, PrepareJson(content, Resources.Global.RepClientsUnit));
                RepInfoBox IbTotalNoOfClients = new RepInfoBox(totalNumberOfClientsGivingFeedback, Resources.Global.RepClientsUnit);
                RepInfoBox IbNoOfNewClients = new RepInfoBox(newCustomersThisInterval, Resources.Global.RepClientsUnit);
                RepInfoBox IbNoOfReturningClients = new RepInfoBox(returningCustomersThisInterval, Resources.Global.RepClientsUnit);
                LinkedList<RepInfoBox> repInfoBoxArray = new LinkedList<RepInfoBox>();
                RepChartData sideBySideReturningCustomers = GetReportClientsSideBySideChart(iIntervalStart, iIntervalEnd, iGranularity, iScope, false);
                RepChartData sideBySideNewCustomers = GetReportClientsSideBySideChart(iIntervalStart, iIntervalEnd, iGranularity, iScope, true);
                var repData = new ReportData(new List<RepChartData>() { chartSource,sideBySideNewCustomers, sideBySideReturningCustomers },
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

        public JsonResult GetReportNewClientsSideBySideGrData(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
        {
           RepChartData chartSource = GetReportClientsSideBySideChart(iIntervalStart, iIntervalEnd, iGranularity, iScope, true);
           return Json(chartSource, JsonRequestBehavior.AllowGet);           
        }

        public JsonResult GetReportReturningClientsSideBySideGrData(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope = null)
        {
           RepChartData chartSource = GetReportClientsSideBySideChart(iIntervalStart, iIntervalEnd, iGranularity, iScope, false);
           return Json(chartSource, JsonRequestBehavior.AllowGet);           
        }      

        private RepChartData GetReportClientsSideBySideChart(String iIntervalStart, String iIntervalEnd, String iGranularity, String[] iScope, bool returnNewCustomers)
        {
           DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
           DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
           iScope = iScope != null ? iScope : new string[0];
           if (iScope.Length == 0)
           {
              iScope = (from u in context.Users where u.UserName == User.Identity.Name select (from wp in u.WorkingPoints select wp.TelNumber)).SelectMany(x => x).ToArray();
           }
           //initialize the result intervals
           var newClientsIntervals = new Dictionary<string, Dictionary<DateTime, ChartValue>>();
           foreach (var location in iScope)
           {
              newClientsIntervals.Add(location, InitializeInterval(intervalStart, intervalEnd, iGranularity));
           }
           var returningclientsIntervals = new Dictionary<string, Dictionary<DateTime, ChartValue>>();
           foreach (var location in iScope)
           {
              returningclientsIntervals.Add(location, InitializeInterval(intervalStart, intervalEnd, iGranularity));
           }
           var items = (from u in context.Users
                        where u.UserName.Equals(User.Identity.Name)
                        select (from wp in u.WorkingPoints
                                where iScope.Contains(wp.TelNumber)
                                select (from conv in wp.Conversations
                                        where !conv.Client.isSupportClient
                                        select (from msg in conv.Messages
                                                where (msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd) &&
                                     (msg.ConversationId.StartsWith(msg.From) || msg.From.EndsWith("@txtfeedback.net"))
                                                group msg by new { msg.ConversationId, msg.TimeReceived.Year, msg.TimeReceived.Month, msg.TimeReceived.Day } into grp
                                                select new
                                                {
                                                   grp.Key.ConversationId,
                                                   grp.Key.Year,
                                                   grp.Key.Month,
                                                   grp.Key.Day,
                                                   messageBeforeInterval = conv.Messages.Where(convMsg => convMsg.TimeReceived < intervalStart).Count() > 0,
                                                   count = grp.Count(),
                                                   wp = wp.TelNumber
                                                })))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);
           var asEntity = items.GroupBy(x => x.wp).ToList();
           foreach (var gr in asEntity)
           {
              if (iGranularity.Equals(Constants.DAY_GRANULARITY))
              {
                 var conversationsPerDate = gr.GroupBy(x => new DateTime(x.Year, x.Month, x.Day));
                 var datesPerConversations = from e in gr
                                             group e by new { e.ConversationId }
                                                into datesGroups
                                                select new
                                                {
                                                   key = datesGroups.Key.ConversationId,
                                                   dates = datesGroups.Select(x => new DateTime(x.Year, x.Month, x.Day))
                                                };
                 foreach (var date in conversationsPerDate)
                 {
                    int newCustomersThisDay = 0;
                    int returningCustomersThisDay = 0;
                    foreach (var conv in date)
                    {
                       //if we have messages before the start of the interval for sure we are dealing with a returning customer
                       if (conv.messageBeforeInterval == true)
                       {
                          returningCustomersThisDay++;
                       }
                       else
                       {
                          //if we had message in another day, prior to this -> returning, otherwise new
                          var messagesInEarlierDaysExist = datesPerConversations.Where(x => x.key == conv.ConversationId).Select(x => x.dates).SelectMany(x => x)
                             .Any(x => x < new DateTime(conv.Year, conv.Month, conv.Day));
                          if (messagesInEarlierDaysExist)
                          {
                             returningCustomersThisDay++;
                          }
                          else
                          {
                             newCustomersThisDay++;
                          }
                       }
                    }
                    returningclientsIntervals[gr.Key][date.Key].value = returningCustomersThisDay;
                    newClientsIntervals[gr.Key][date.Key].value = newCustomersThisDay;
                 }
              }
              else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
              {
                 var conversationsPerMonth = gr.GroupBy(x => new { x.Year, x.Month });
                 var datesPerConversations = from e in gr
                                             group e by new { e.ConversationId }
                                                into datesGroups
                                                select new
                                                {
                                                   key = datesGroups.Key.ConversationId,
                                                   dates = datesGroups.Select(x => new DateTime(x.Year, x.Month, x.Day))
                                                };
                 foreach (var date in conversationsPerMonth)
                 {
                    int newCustomersThisMonth = 0;
                    int returningCustomersThisMonth = 0;
                    foreach (var conv in date)
                    {
                       //if we have messages before the start of the interval for sure we are dealing with a returning customer
                       if (conv.messageBeforeInterval == true)
                       {
                          returningCustomersThisMonth++;
                       }
                       else
                       {
                          //if we had message in another day, prior to this -> returning, otherwise new
                          var messagesInEarlierDaysExist = datesPerConversations.Where(x => x.key == conv.ConversationId).Select(x => x.dates).SelectMany(x => x)
                             .Any(x => x < new DateTime(conv.Year, conv.Month, conv.Day));
                          if (messagesInEarlierDaysExist)
                          {
                             returningCustomersThisMonth++;
                          }
                          else
                          {
                             newCustomersThisMonth++;
                          }
                       }
                    }
                    var intervalDate = new DateTime(date.Key.Year, date.Key.Month, 1);
                    intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                    intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                    returningclientsIntervals[gr.Key][intervalDate].value = returningCustomersThisMonth;
                    newClientsIntervals[gr.Key][intervalDate].value = newCustomersThisMonth;
                 }
              }
              else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
              {
                 var conversationsPerWeek = gr.GroupBy(x => FirstDayOfWeekUtility.GetFirstDayOfWeek(new DateTime(x.Year, x.Month, x.Day)));
                 var datesPerConversations = from e in gr
                                             group e by new { e.ConversationId }
                                                into datesGroups
                                                select new
                                                {
                                                   key = datesGroups.Key.ConversationId,
                                                   dates = datesGroups.Select(x => new DateTime(x.Year, x.Month, x.Day))
                                                };
                 foreach (var date in conversationsPerWeek)
                 {
                    int newCustomersThisWeek = 0;
                    int returningCustomersThisWeek = 0;
                    foreach (var conv in date)
                    {
                       //if we have messages before the start of the interval for sure we are dealing with a returning customer
                       if (conv.messageBeforeInterval == true)
                       {
                          returningCustomersThisWeek++;
                       }
                       else
                       {
                          //if we had message in another day, prior to this -> returning, otherwise new
                          var messagesInEarlierDaysExist = datesPerConversations.Where(x => x.key == conv.ConversationId).Select(x => x.dates).SelectMany(x => x)
                             .Any(x => x < new DateTime(conv.Year, conv.Month, conv.Day));
                          if (messagesInEarlierDaysExist)
                          {
                             returningCustomersThisWeek++;
                          }
                          else
                          {
                             newCustomersThisWeek++;
                          }
                       }
                    }
                    var intervalDate = FirstDayOfWeekUtility.GetFirstDayOfWeek(new DateTime(date.Key.Year, date.Key.Month, date.Key.Day));
                    intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                    intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                    returningclientsIntervals[gr.Key][intervalDate].value = returningCustomersThisWeek;
                    newClientsIntervals[gr.Key][intervalDate].value = newCustomersThisWeek;
                 }
              }
           }
           List<Dictionary<DateTime, ChartValue>> clientsChartContent = new List<Dictionary<DateTime, ChartValue>>();
           if (returnNewCustomers)
           {
              foreach (var item in newClientsIntervals)
              {
                 clientsChartContent.Add(item.Value);
              }
           }
           else
           {
              foreach (var item in returningclientsIntervals)
              {
                 clientsChartContent.Add(item.Value);
              }
           }

           List<RepDataColumn> columnDefinition = new List<RepDataColumn> { new RepDataColumn("17", Constants.STRING_COLUMN_TYPE, "Date") };
           foreach (var location in iScope)
           {
              columnDefinition.Add(new RepDataColumn("18", Constants.NUMBER_COLUMN_TYPE, location));

           }
           RepChartData chartSource = new RepChartData(
              columnDefinition,
                 PrepareJson(clientsChartContent, Resources.Global.RepSmsUnit));
           return chartSource;
        }

        public JsonResult GetReportClientsGrData(String iIntervalStart, String iIntervalEnd,  String iGranularity,String[] iScope =null)
        {
            try
            {
               DateTime intervalStart = DateTime.ParseExact(iIntervalStart, cDateFormat, CultureInfo.InvariantCulture);
               DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, cDateFormat, CultureInfo.InvariantCulture);
               
                Dictionary<DateTime, ChartValue> resultNewClientsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);
                Dictionary<DateTime, ChartValue> resultReturningClientsInterval = InitializeInterval(intervalStart, intervalEnd, iGranularity);

                iScope = iScope != null ? iScope : new string[0];
                var useAllWps = iScope.Length == 0;
                var items = (from u in context.Users
                             where u.UserName.Equals(User.Identity.Name)
                             select (from wp in u.WorkingPoints
                                     where useAllWps ? true : iScope.Contains(wp.TelNumber)
                                     select (from conv in wp.Conversations
                                             where !conv.Client.isSupportClient
                                             select (from msg in conv.Messages
                                                     where (msg.TimeReceived >= intervalStart && msg.TimeReceived <= intervalEnd) &&
                                          (msg.ConversationId.StartsWith(msg.From) || msg.From.EndsWith("@txtfeedback.net"))
                                                     group msg by new
                                                     { msg.ConversationId, msg.TimeReceived.Year, msg.TimeReceived.Month,msg.TimeReceived.Day} into grp
                                                     select new
                                                     {grp.Key.ConversationId,grp.Key.Year, grp.Key.Month, grp.Key.Day, messageBeforeInterval = conv.Messages.Where(convMsg => convMsg.TimeReceived < intervalStart).Count() > 0,
                                                        count = grp.Count()
                                                     })))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);
                var asEntity = items.ToList();
                int newCustomersThisInterval = 0;
                int returningCustomersThisInterval = 0;
                if (iGranularity.Equals(Constants.DAY_GRANULARITY))
                {

                   var conversationsPerDate = asEntity.GroupBy(x => new DateTime(x.Year, x.Month, x.Day));
                   var datesPerConversations = from e in asEntity
                                               group e by new { e.ConversationId }
                                                  into datesGroups
                                                  select new
                                                  {
                                                     key = datesGroups.Key.ConversationId,
                                                     dates = datesGroups.Select(x => new DateTime(x.Year, x.Month, x.Day))
                                                  }; 
                   foreach (var convsPerGroup in conversationsPerDate)
                   {
                      int newCustomersThisDay = 0;
                      int returningCustomersThisDay = 0;
                      foreach (var conv in convsPerGroup)
                      {
                         //if we have messages before the start of the interval for sure we are dealing with a returning customer
                         if (conv.messageBeforeInterval == true)
                         {
                            returningCustomersThisDay++;
                         }
                         else
                         {
                            //if we had message in another day, prior to this -> returning, otherwise new
                            var messagesInEarlierDaysExist = datesPerConversations.Where(x => x.key == conv.ConversationId).Select(x => x.dates).SelectMany(x => x)
                               .Any(x => x < new DateTime(conv.Year, conv.Month, conv.Day));
                            if (messagesInEarlierDaysExist)
                            {
                               returningCustomersThisDay++;
                            }
                            else
                            {
                               newCustomersThisDay++;
                            }
                         }
                      }
                      resultReturningClientsInterval[convsPerGroup.Key].value = returningCustomersThisDay;
                      returningCustomersThisInterval += returningCustomersThisDay;
                      resultNewClientsInterval[convsPerGroup.Key].value = newCustomersThisDay;
                      newCustomersThisInterval += newCustomersThisDay;
                   }                    
                }
                else if (iGranularity.Equals(Constants.MONTH_GRANULARITY))
                {
                   var conversationsPerMonth = asEntity.GroupBy(x => new { x.Year, x.Month} );
                   var datesPerConversations = from e in asEntity
                                               group e by new { e.ConversationId }
                                                  into datesGroups
                                                  select new
                                                  {
                                                     key = datesGroups.Key.ConversationId,
                                                     dates = datesGroups.Select(x => new DateTime(x.Year, x.Month, x.Day))
                                                  }; 
                   foreach (var convsPerGroup in conversationsPerMonth)
                   {
                      int newCustomersThisMonth = 0;
                      int returningCustomersThisMonth = 0;
                      foreach (var conv in convsPerGroup)
                      {
                         //if we have messages before the start of the interval for sure we are dealing with a returning customer
                         if (conv.messageBeforeInterval == true)
                         {
                            returningCustomersThisMonth++;
                         }
                         else
                         {
                            //if we had message in another day, prior to this -> returning, otherwise new
                            var messagesInEarlierDaysExist = datesPerConversations.Where(x => x.key == conv.ConversationId).Select(x => x.dates).SelectMany(x => x)
                               .Any(x => x < new DateTime( conv.Year,conv.Month, conv.Day));
                            if (messagesInEarlierDaysExist)
                            {
                               returningCustomersThisMonth++;
                            }
                            else
                            {
                               newCustomersThisMonth++;
                            }
                         }
                      }
                      var intervalDate = new DateTime(convsPerGroup.Key.Year, convsPerGroup.Key.Month, 1);
                      intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                      intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                      resultReturningClientsInterval[intervalDate].value = returningCustomersThisMonth;
                      returningCustomersThisInterval += returningCustomersThisMonth;
                      resultNewClientsInterval[intervalDate].value = newCustomersThisMonth;
                      newCustomersThisInterval += newCustomersThisMonth;
                   } 
                }
                else if (iGranularity.Equals(Constants.WEEK_GRANULARITY))
                {
                   var conversationsPerWeek = asEntity.GroupBy( x => FirstDayOfWeekUtility.GetFirstDayOfWeek(new DateTime(x.Year,x.Month,x.Day)));
                   var datesPerConversations = from e in asEntity
                                               group e by new { e.ConversationId }
                                                  into datesGroups
                                                  select new
                                                  {
                                                     key = datesGroups.Key.ConversationId,
                                                     dates = datesGroups.Select(x => new DateTime(x.Year, x.Month, x.Day))
                                                  };
                   foreach (var date in conversationsPerWeek)
                   {
                      int newCustomersThisWeek = 0;
                      int returningCustomersThisWeek = 0;
                      foreach (var conv in date)
                      {
                         //if we have messages before the start of the interval for sure we are dealing with a returning customer
                         if (conv.messageBeforeInterval == true)
                         {
                            returningCustomersThisWeek++;
                         }
                         else
                         {
                            //if we had message in another day, prior to this -> returning, otherwise new
                            var messagesInEarlierDaysExist = datesPerConversations.Where(x => x.key == conv.ConversationId).Select(x => x.dates).SelectMany(x => x)
                               .Any(x => x < new DateTime(conv.Year, conv.Month, conv.Day));                            
                            if (messagesInEarlierDaysExist)
                            {
                               returningCustomersThisWeek++;
                            }
                            else
                            {
                               newCustomersThisWeek++;
                            }
                         }
                      }
                      var intervalDate =  FirstDayOfWeekUtility.GetFirstDayOfWeek(new DateTime(date.Key.Year, date.Key.Month, date.Key.Day));
                      intervalDate = intervalDate < intervalStart ? intervalStart : intervalDate;
                      intervalDate = intervalDate > intervalEnd ? intervalEnd : intervalDate;
                      resultReturningClientsInterval[intervalDate].value = returningCustomersThisWeek;
                      returningCustomersThisInterval += returningCustomersThisWeek;
                      resultNewClientsInterval[intervalDate].value = newCustomersThisWeek;
                      newCustomersThisInterval += newCustomersThisWeek;
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
                                        new ReportSection("FirstSection", Resources.Global.RepOverviewChartTitle, 0, iChartSource: "/Reports/GetReportOverviewGrData", iHasExportRawData: User.IsInRole(cExporterOfRawData)),
                                        new ReportSection("FirstSection", "Side by side: Total Number of Messages",1 ,iChartSource: "/Reports/GetReportOverviewGrDataSideBySide"),
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
                                       new ReportSection("FirstSection", iDataIndex: 0, iChartSource:"/Reports/PosEvolutionSideBySideInternalGr", iSectionId:"1", iTitle:"Side by side: positive feedback", iTooltip: "Received positive feedback"), 
                                       new ReportSection("FirstSection", iDataIndex: 1, iChartSource:"/Reports/NegEvolutionSideBySideInternalGr", iSectionId:"2", iTitle:"Side by side: negative feedback", iTooltip: "Received negative feedback"), 
                                        new ReportSection("FirstSection", iDataIndex: 2, iChartSource:"/Reports/GetReportPosNegEvolutionGr", iSectionId:"3", iTitle:Resources.Global.RepPositiveNegativeEvolutionChartTitle, iTooltip: Resources.Global.RepTooltipPosNegFeedbackEvolution),                                                                                                                                                            
                                        new ReportSection("FirstSection", iDataIndex: 3, iChartSource:"/Reports/PosEvolutionSideBySideInternal", iSectionId: "4", iGroupId: "7",  iTitle:Resources.Global.RepPositiveNegativeTransitionsChartTitle, iTooltip: Resources.Global.RepTooltipPosNegFeedbackTransitions),
                                        new ReportSection("ThirdSection", iDataIndex: 4, iSectionId: "5", iGroupId: "7", iTitle: Resources.Global.RepPositiveNegativeTransitionsChartTitle)
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
                                        new ReportSection("SecondSection", iDataIndex: 2, iTitle: Resources.Global.RepNoOfReturningClients), 
                                        new ReportSection("FirstSection", iDataIndex: 1, iChartSource: "/Reports/GetReportNewClientsSideBySideGrData", iTitle: "Side by side: new customers"),
                                        new ReportSection("FirstSection", iDataIndex: 2, iChartSource: "/Reports/GetReportReturningClientsSideBySideGrData", iTitle: "Side by side: returning customers ")                                        
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