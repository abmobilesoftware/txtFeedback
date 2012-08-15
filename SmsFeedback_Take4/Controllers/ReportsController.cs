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

                Dictionary<DateTime, int> daysInterval = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => 0);
                foreach (var wp in workingPoints)
                {
                    var conversations = from conv in wp.Conversations select conv;
                    foreach (var conv in conversations)
                    {
                        var msgsToFrom = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                        foreach (var entry in msgsToFrom)
                        {
                            daysInterval[entry.date] += entry.count;
                        }
                    }
                }

                List<RepDataRow> content = new List<RepDataRow>();
                var hashTable = new Dictionary<DateTime, int>();
                foreach (var entry in daysInterval)
                {
                    // Incoming
                    var totalSmsText = entry.Value + " sms - " + entry.Key.ToShortDateString();
                    content.Add(new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(entry.Key.ToShortDateString(), entry.Key.ToShortDateString()), new RepDataRowCell(entry.Value, totalSmsText) }));
                }

                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Total sms") }, content);
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

                Dictionary<DateTime, int> daysIntervalIncoming = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => 0);
                Dictionary<DateTime, int> daysIntervalOutgoing = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => 0);
                foreach (var wp in workingPoints)
                {
                    var conversations = from conv in wp.Conversations select conv;
                    foreach (var conv in conversations)
                    {
                        var msgsTo = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                        foreach (var entry in msgsTo)
                        {
                            daysIntervalIncoming[entry.date] += entry.count;
                        }

                        var msgsFrom = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                        foreach (var entry in msgsFrom)
                        {
                            daysIntervalOutgoing[entry.date] += entry.count;
                        }

                        List<RepDataRow> content = new List<RepDataRow>();
                        var hashTable = new Dictionary<DateTime, int>();
                        foreach (var entry in daysIntervalIncoming)
                        {
                            var totalIncomingText = entry.Value + " sms - " + entry.Key.ToShortDateString();
                            var totalOutgoingText = daysIntervalOutgoing[entry.Key] + " sms - " + entry.Key.ToShortDateString();
                            content.Add(new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(entry.Key.ToShortDateString(), entry.Key.ToShortDateString()), new RepDataRowCell(entry.Value, totalIncomingText), new RepDataRowCell(daysIntervalOutgoing[entry.Key], totalOutgoingText) }));
                        }

                        RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Total incoming sms"), new RepDataColumn("18", "number", "Total outgoing sms") }, content);
                        return Json(chartSource, JsonRequestBehavior.AllowGet);
                    }
                }
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

                Dictionary<DateTime, int> daysIntervalNewClients = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => 0);
                Dictionary<DateTime, int> daysIntervalReturningClients = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => 0);
                foreach (var wp in workingPoints)
                {
                    var newClients = from conv in wp.Conversations where conv.StartTime > new DateTime(2012, 8, 1) & conv.StartTime < new DateTime(2012, 8, 15) group conv by conv.StartTime.Date into convGroup select new { date = convGroup.Key, count = convGroup.Count() };
                    foreach (var newClient in newClients)
                    {
                        daysIntervalNewClients[newClient.date] += newClient.count;
                    }

                    var returningClients = from conv in wp.Conversations where conv.StartTime < intervalStart select (from msg in conv.Messages where msg.TimeReceived > intervalStart & msg.TimeReceived < intervalEnd group conv by new { msg.TimeReceived.Date } into convGroup select new { date = convGroup.Key, count = convGroup.Count() });
                    foreach (var conv in returningClients)
                    {
                        foreach (var day in conv)
                        {
                            DateTime a = day.date.Date;
                            daysIntervalReturningClients[a] += 1;
                        }
                    }
                }

                // Prepare Json result
                List<RepDataRow> content = new List<RepDataRow>();
                var hashTable = new Dictionary<DateTime, int>();
                foreach (var entry in daysIntervalNewClients)
                {
                    var totalNewClientsText = entry.Value + " clients - " + entry.Key.ToShortDateString();
                    var totalReturningText = daysIntervalReturningClients[entry.Key] + " sms - " + entry.Key.ToShortDateString();
                    content.Add(new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(entry.Key.ToShortDateString(), entry.Key.ToShortDateString()), new RepDataRowCell(entry.Value, totalNewClientsText), new RepDataRowCell(daysIntervalReturningClients[entry.Key], totalReturningText) }));
                }

                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "New clients"), new RepDataColumn("18", "number", "Returning clients") }, content);
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
                    rowContent.Add(new RepDataRowCell(tagEntry.Value, tagEntry.Value.ToString()));
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
                var totalNoOfSms = 0;

                foreach (var wp in workingPoints)
                {
                    var conversations = from conv in wp.Conversations select conv;
                    foreach (var conv in conversations)
                    {
                        var msgsFromTo = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
                        totalNoOfSms += msgsFromTo;
                    }
                }
                return Json(new RepInfoBox(totalNoOfSms, "sms'"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetTotalNoOfSmsInfo", e);
            }
            return Json(new RepInfoBox("Request failed", "sms'"), JsonRequestBehavior.AllowGet);
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

                var incomingNoOfSms = 0;
                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        var msgsTo = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) select msg).Count();
                        incomingNoOfSms += msgsTo;
                    }
                }
                return Json(new RepInfoBox(incomingNoOfSms, "sms"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetIncomingSmsInfo", e);
            }
            return Json(new RepInfoBox("Request failed", "sms"), JsonRequestBehavior.AllowGet);
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

                var outgoingNoOfSms = 0;
                foreach (var wp in workingPoints)
                {
                    var conversations = from conv in wp.Conversations select conv;
                    foreach (var conv in conversations)
                    {
                        var msgsFrom = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) select msg).Count();
                        outgoingNoOfSms += msgsFrom;
                    }
                }
                return Json(new RepInfoBox(outgoingNoOfSms, "sms"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetOutgoingSmsInfo", e);
            }
            return Json(new RepInfoBox("Request failed", "sms"), JsonRequestBehavior.AllowGet);
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

                var totalNoOfSms = 0;
                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        var msgsToFrom = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                        foreach (var entry in msgsToFrom)
                        {
                            totalNoOfSms += entry.count;
                        }
                    }
                }
                TimeSpan interval = intervalEnd - intervalStart;
                return Json(new RepInfoBox(Math.Round(totalNoOfSms / interval.TotalDays), "sms'/day"), JsonRequestBehavior.AllowGet);
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

                int noOfClients = 0;
                int noOfIncomingMessages = 0;
                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        var msgsTo = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) select msg).Count();
                        noOfIncomingMessages += msgsTo;
                        noOfClients += 1;
                    }
                }
                return Json(new RepInfoBox(Math.Round((double)noOfIncomingMessages / noOfClients), "sms'/client"), JsonRequestBehavior.AllowGet);
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

                int noOfClients = 0;
                int noOfOutgoingMessages = 0;
                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        var msgsFrom = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) select msg).Count();
                        noOfOutgoingMessages += msgsFrom;
                        noOfClients += 1;
                    }
                }
                return Json(new RepInfoBox(Math.Round((double)noOfOutgoingMessages / noOfClients), "sms'/client"), JsonRequestBehavior.AllowGet);
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
                return Json(new RepInfoBox(String.Join(", ", mostUsedTags), "tag"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetMostUsedTagsInfo", e);
            }
            return Json("Request failed", JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAverageNoOfTagsPerConversationInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
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
                    noOfConversations += wp.Conversations.Count;
                    foreach (var conv in wp.Conversations)
                    {
                        noOfTags += conv.Tags.Count;
                    }
                }
                return Json(new RepInfoBox(Math.Round((double)noOfTags / noOfConversations), "tags/conversation"), JsonRequestBehavior.AllowGet);
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
                    var conversations = from conv in wp.Conversations where conv.StartTime >= intervalStart && conv.StartTime <= intervalEnd select conv;
                    noOfNewClients += conversations.ToList<Conversation>().Count();
                }
                return Json(new RepInfoBox(noOfNewClients, "clients"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error("GetNoOfNewClientsInfo", e);
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
                    var returningClients = from conv in wp.Conversations where conv.StartTime < intervalStart select (from msg in conv.Messages where msg.TimeReceived > intervalStart & msg.TimeReceived < intervalEnd group conv by new { msg.TimeReceived.Date } into convGroup select new { date = convGroup.Key, count = convGroup.Count() });
                    noOfReturningClients += returningClients.Count();
                }
                return Json(new RepInfoBox(noOfReturningClients, "clients"), JsonRequestBehavior.AllowGet);
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

                int noOfClients = 0;
                int noOfMessages = 0;
                foreach (var wp in workingPoints)
                {
                    foreach (var conv in wp.Conversations)
                    {
                        var msgsToOrFrom = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
                        noOfMessages += msgsToOrFrom;
                        noOfClients += 1;
                    }
                }
                return Json(new RepInfoBox(Math.Round((double)noOfMessages / noOfClients), "sms'/client"), JsonRequestBehavior.AllowGet);
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
                TimeSpan avgResponseTime = new TimeSpan((long)(totalResponseTime / counter));
                if (avgResponseTime.TotalMinutes < 1)
                {
                    return Json(new RepInfoBox(Math.Round(avgResponseTime.TotalSeconds, 2), "seconds"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new RepInfoBox(Math.Round(avgResponseTime.TotalMinutes, 2), "minutes"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e) {
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

                var incomingNoOfSms = 0;
                var outgoingNoOfSms = 0;
                foreach (var wp in workingPoints)
                {
                    var conversations = from conv in wp.Conversations select conv;
                    foreach (var conv in conversations)
                    {
                        var msgsTo = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) select msg).Count();
                        var msgsFrom = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) select msg).Count();
                        incomingNoOfSms += msgsTo;
                        outgoingNoOfSms += msgsFrom;
                    }
                }

                // Prepare Json result
                var row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("Incoming", "Incoming"), new RepDataRowCell(incomingNoOfSms, incomingNoOfSms + " sms - from customers") });
                var row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("Outgoing", "Outgoing"), new RepDataRowCell(outgoingNoOfSms, outgoingNoOfSms + " sms - to customers") });
                List<RepDataRow> content = new List<RepDataRow>();
                content.Add(row1);
                content.Add(row2);
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Type"), new RepDataColumn("18", "number", "Number") }, content);
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
                                                                         new ReportsMenuItem(3, Resources.Global.RepIncomingVsOutgoing, true, 1), new ReportsMenuItem(4, Resources.Global.RepTags, true, 1),
                                                                         new ReportsMenuItem(5, Resources.Global.RepClients, false, 0), new ReportsMenuItem(6, Resources.Global.RepNewVsReturning, true, 5), 
                                                                        };
            // new ReportsMenuItem(7, Resources.Global.RepNewVsReturning , true, 5)
            return Json(reportsMenuItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportById(int reportId)
        {
            var hashTable = new Dictionary<int, Report>();
            var report2 = new Report(2, Resources.Global.RepOverview, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Total number of sms with granularity", iSource: "/Reports/GetTotalNoOfSmsChartSource") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "/Reports/GetTotalNoOfSmsInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfSmsPerDay, iSource: "/Reports/GetAvgNoOfSmsPerDayInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfNewClients, iSource: "/Reports/GetNoOfNewClientsInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfSmsPerClient, iSource: "/Reports/GetAvgNoOfSmsPerClientInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgResponseTime, iSource: "/Reports/GetAvgResponseTimeInfo")
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetIncomingOutgoingThirdArea") 
                                                                                                                                                          }),
                                                                                    });
            var report3 = new Report(3, Resources.Global.RepIncomingVsOutgoing, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms with granularity", iSource: "/Reports/GetIncomingOutgoingSmsChartSource") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfIncomingSms, iSource: "/Reports/GetIncomingSmsInfo"),  
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfOutgoingSms, iSource: "/Reports/GetOutgoingSmsInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfNewClients, iSource: "/Reports/GetNoOfNewClientsInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfOutgoingSmsPerConversation, iSource: "/Reports/GetAvgNoOfOutgoingSmsPerClientInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfIncomingSmsPerConversation, iSource: "/Reports/GetAvgNoOfIncomingSmsPerClientInfo")
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetIncomingOutgoingThirdArea") 
                                                                                                                                                          }),
                                                                                    });
            var report4 = new Report(4, Resources.Global.RepTags, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("No of conversations by tags", iSource: "/Reports/GetNoOfConversationsByTagsChartSource", iOptions: new ReportResourceOptions(Constants.BARS_CHART_STYLE)) 
                                                                                                                                                            
                                                                                                                                                             
                                                                                                                                                            }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepMostUsedTag, iSource: "/Reports/GetMostUsedTagsInfo"),     
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfTagsPerConversation, iSource: "/Reports/GetAverageNoOfTagsPerConversationInfo")                                                                                                                                                  
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report6 = new Report(6, Resources.Global.RepNewVsReturning, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms with granularity", iSource: "/Reports/GetNewVsReturningClientsChartSource") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfNewClients, iSource: "/Reports/GetNoOfNewClientsInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfReturningClients, iSource: "/Reports/GetNoOfReturningClientsInfo")
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
            hashTable.Add(6, report6);
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
                            foreach (var tag in conv.Tags)
                            {

                                if (tagsHash.ContainsKey(tag.Name))
                                {
                                    tagsHash[tag.Name] += 1;
                                }
                                else
                                {
                                    tagsHash.Add(tag.Name, 1);
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
        #endregion
    }
}