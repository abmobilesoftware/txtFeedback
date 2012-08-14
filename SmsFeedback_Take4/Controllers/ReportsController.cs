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

        public ActionResult Index()
        {
            ViewData["currentCulture"] = getCurrentCulture();
            return View();
        }

        public JsonResult GetSmsTotalDetailed(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);


            Dictionary<DateTime, int> daysInterval = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => 0);
            smsfeedbackEntities dbContext = new smsfeedbackEntities();


            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            foreach (var wp in incommingMsgs)
            {
                var conversations = from conv in wp.Conversations select conv;
                foreach (var conv in conversations)
                {
                    var msgsTo = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                    foreach (var entry in msgsTo)
                    {
                        daysInterval[entry.date] += entry.count;
                    }

                    var msgsFrom = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                    foreach (var entry in msgsFrom)
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

        public JsonResult GetSmsIncomingOutgoingDetailed(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            Dictionary<DateTime, int> daysIntervalIncoming = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => 0);
            Dictionary<DateTime, int> daysIntervalOutgoing = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => 0);
            smsfeedbackEntities dbContext = new smsfeedbackEntities();


            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            foreach (var wp in incommingMsgs)
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
                }
            }

            List<RepDataRow> content = new List<RepDataRow>();
            var hashTable = new Dictionary<DateTime, int>();
            foreach (var entry in daysIntervalIncoming)
            {
                // Incoming
                var totalIncomingText = entry.Value + " sms - " + entry.Key.ToShortDateString();
                var totalOutgoingText = daysIntervalOutgoing[entry.Key] + " sms - " + entry.Key.ToShortDateString();
                content.Add(new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(entry.Key.ToShortDateString(), entry.Key.ToShortDateString()), new RepDataRowCell(entry.Value, totalIncomingText), new RepDataRowCell(daysIntervalOutgoing[entry.Key], totalOutgoingText) }));
            }

            RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Total incoming sms"), new RepDataColumn("18", "number", "Total outgoing sms") }, content);
            return Json(chartSource, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClientsNewVsReturningDetailed(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            Dictionary<DateTime, int> daysIntervalNewClients = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => 0);
            Dictionary<DateTime, int> daysIntervalReturningClients = Enumerable.Range(0, 1 + intervalEnd.Subtract(intervalStart).Days).Select(offset => intervalStart.AddDays(offset)).ToDictionary(d => d.Date, d => 0);
            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            
            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            foreach (var wp in incommingMsgs)
            {
                var newClients = from conv in wp.Conversations where conv.StartTime > new DateTime(2012, 8, 1) & conv.StartTime < new DateTime(2012, 8, 15) group conv by conv.StartTime.Date into convGroup select new { date = convGroup.Key, count = convGroup.Count() };
                foreach (var newClient in newClients)
                {
                    daysIntervalNewClients[newClient.date] += newClient.count;
                }

                var returningClients = from conv in wp.Conversations where conv.StartTime < intervalStart select (from msg in conv.Messages where msg.TimeReceived > intervalStart & msg.TimeReceived < intervalEnd group conv by new { msg.TimeReceived.Date} into convGroup select new { date = convGroup.Key, count = convGroup.Count() });
                foreach (var conv in returningClients)
                {                   
                    foreach (var day in conv)
                    {                        
                        DateTime a = day.date.Date;
                        daysIntervalReturningClients[a] += 1;               
                    }
                }
            }

            List<RepDataRow> content = new List<RepDataRow>();
            var hashTable = new Dictionary<DateTime, int>();
            foreach (var entry in daysIntervalNewClients)
            {
                // Incoming
                var totalNewClientsText = entry.Value + " clients - " + entry.Key.ToShortDateString();
                var totalReturningText = daysIntervalReturningClients[entry.Key] + " sms - " + entry.Key.ToShortDateString();
                content.Add(new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(entry.Key.ToShortDateString(), entry.Key.ToShortDateString()), new RepDataRowCell(entry.Value, totalNewClientsText), new RepDataRowCell(daysIntervalReturningClients[entry.Key], totalReturningText) }));
            }

            RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "New clients"), new RepDataColumn("18", "number", "Returning clients") }, content);

            /*
            List<RepDataRow> content = new List<RepDataRow>();
            var hashTable = new Dictionary<DateTime, int>();
            foreach (var entry in daysInterval)
            {
                // Incoming
                var totalIncomingText = entry.Value + " sms - " + entry.Key.ToShortDateString();
                var totalOutgoingText = daysIntervalOutgoing[entry.Key] + " sms - " + entry.Key.ToShortDateString();
                content.Add(new RepDataRow(new RepDataRowCell[] { new RepDataRowCell(entry.Key.ToShortDateString(), entry.Key.ToShortDateString()), new RepDataRowCell(entry.Value, totalIncomingText), new RepDataRowCell(daysIntervalOutgoing[entry.Key], totalOutgoingText) }));
            }
             */ 

            //RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Total incoming sms"), new RepDataColumn("18", "number", "Total outgoing sms") }, content);
            return Json(chartSource, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSmsTotalInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);


            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            var totalNoOfSms = 0;

            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            foreach (var wp in incommingMsgs)
            {
                var conversations = from conv in wp.Conversations select conv;
                foreach (var conv in conversations)
                {
                    var msgsTo = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                    var msgsFrom = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                    foreach (var entry in msgsTo)
                    {
                        totalNoOfSms += entry.count;
                    }
                    foreach (var entry in msgsFrom)
                    {
                        totalNoOfSms += entry.count;
                    }
                }

            }
            return Json(new RepInfoBox(totalNoOfSms, "sms"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSmsAvgPerDayInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            var totalNoOfSms = 0;

            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            foreach (var wp in incommingMsgs)
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

            
            TimeSpan noOfDays = intervalEnd - intervalStart;

            return Json(new RepInfoBox((int)((double)totalNoOfSms / noOfDays.Days * 100) / (double)100, "sms"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIncomingSmsAvgPerClientInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            int noOfClients = 0;
            int noOfIncomingMessages = 0;

            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            foreach (var wp in incommingMsgs)
            {
                foreach (var conv in wp.Conversations)
                {
                    var msgsTo = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) select msg).Count();
                    noOfIncomingMessages += msgsTo;
                    noOfClients += 1;
                }
            }
            return Json(new RepInfoBox((int)((double)noOfIncomingMessages / noOfClients * 100) / (double)100, "sms/client"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOutgoingSmsAvgPerClientInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            int noOfClients = 0;
            int noOfOutgoingMessages = 0;

            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            foreach (var wp in incommingMsgs)
            {
                foreach (var conv in wp.Conversations)
                {
                    var msgsFrom = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) select msg).Count();
                    noOfOutgoingMessages += msgsFrom;
                    noOfClients += 1;
                }
            }
            return Json(new RepInfoBox((int)((double)noOfOutgoingMessages / noOfClients * 100) / (double)100, "sms/client"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNoOfConversationsByTag(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            int noOfClients = 0;
            int noOfOutgoingMessages = 0;

            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            var tagsHash = new Dictionary<String, int>();
            foreach (var wp in incommingMsgs)
            {
                foreach (var conv in wp.Conversations)
                {
                    var noOfMessagesInThisConversation = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
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
            
            RepChartData chartSource = new RepChartData(headerContent, new RepDataRow[] {new RepDataRow(rowContent)} );
            return Json(chartSource, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMostUsedTagInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            var tagsHash = new Dictionary<String, int>();
            foreach (var wp in incommingMsgs)
            {
                foreach (var conv in wp.Conversations)
                {
                    var noOfMessagesInThisConversation = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
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

        public JsonResult GetAverageNoOfTagsPerConversationInfo(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            var noOfConversations = 0;
            var noOfTags = 0;

            foreach (var wp in incommingMsgs)
            {
                noOfConversations += wp.Conversations.Count;
                foreach (var conv in wp.Conversations)
                {
                    noOfTags += conv.Tags.Count;
                }

            }

            return Json(new RepInfoBox((double)noOfTags/noOfConversations, "tags/conversation"), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSmsIncomingOutgoingTotal(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            var incomingNoOfSms = 0;
            var outgoingNoOfSms = 0;

            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            foreach (var wp in incommingMsgs)
            {
                var conversations = from conv in wp.Conversations select conv;
                foreach (var conv in conversations)
                {
                    var msgsTo = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.To == wp.TelNumber) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                    var msgsFrom = from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd & msg.From == wp.TelNumber) group msg by msg.TimeReceived.Date into g select new { date = g.Key, count = g.Count() };
                    foreach (var entry in msgsTo)
                    {
                        incomingNoOfSms += entry.count;
                    }
                    foreach (var entry in msgsFrom)
                    {
                        outgoingNoOfSms += entry.count;
                    }
                }
            }

            var row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("Incoming", "Incoming"), new RepDataRowCell(incomingNoOfSms, incomingNoOfSms + " sms - from customers") });
            var row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("Outgoing", "Outgoing"), new RepDataRowCell(outgoingNoOfSms, outgoingNoOfSms + " sms - to customers") });

            List<RepDataRow> content = new List<RepDataRow>();
            content.Add(row1);
            content.Add(row2);

            RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Type"), new RepDataColumn("18", "number", "Number") }, content);
            return Json(chartSource, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetNoOfNewClientsInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            int noOfNewClients = 0;

            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            foreach (var wp in incommingMsgs)
            {
                var conversations = from conv in wp.Conversations where conv.StartTime >= intervalStart && conv.StartTime <= intervalEnd select conv;
                noOfNewClients += conversations.ToList<Conversation>().Count();
            }

            return Json(new RepInfoBox(noOfNewClients, "clients"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvgNoOfSmsPerClientInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            int noOfClients = 0;
            int noOfMessages = 0;

            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            foreach (var wp in incommingMsgs)
            {
                foreach (var conv in wp.Conversations)
                {
                    var msgsToOrFrom = (from msg in conv.Messages where (msg.TimeReceived >= intervalStart & msg.TimeReceived <= intervalEnd) select msg).Count();
                    noOfMessages += msgsToOrFrom;
                    noOfClients += 1;
                }
            }
            return Json(new RepInfoBox((int)((double)noOfMessages / noOfClients * 100) / (double)100, "sms/client"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvgResponseTimeInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            smsfeedbackEntities dbContext = new smsfeedbackEntities();
            IEnumerable<WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var userName = User.Identity.Name;
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            long totalResponseTime = 0;
            var counter = 0;
            foreach (var wp in incommingMsgs)
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

            return Json(new RepInfoBox((int)((double)totalResponseTime / counter * 100) / (double)100, "sms/client"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportsMenuItems()
        {
            ReportsMenuItem[] reportsMenuItems = new ReportsMenuItem[] { new ReportsMenuItem(1, Resources.Global.RepConversations, false, 0), new ReportsMenuItem(2, Resources.Global.RepOverview, true, 1), 
                                                                         new ReportsMenuItem(3, Resources.Global.RepIncomingVsOutgoing, true, 1), new ReportsMenuItem(4, Resources.Global.RepTags, true, 1),
                                                                         new ReportsMenuItem(5, Resources.Global.RepClients, false, 0), new ReportsMenuItem(6, Resources.Global.RepOverview, true, 5), 
                                                                         new ReportsMenuItem(7, Resources.Global.RepNewVsReturning , true, 5)};
            return Json(reportsMenuItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportById(int reportId)
        {
            var hashTable = new Dictionary<int, Report>();
            var report2 = new Report(2, Resources.Global.RepOverview, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Total number of sms with granularity", iSource: "/Reports/GetSmsTotalDetailed") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "/Reports/GetSmsTotalInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfSmsPerDay, iSource: "/Reports/GetSmsAvgPerDayInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfNewClients, iSource: "/Reports/GetNoOfNewClientsInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfSmsPerClient, iSource: "/Reports/GetAvgNoOfSmsPerClientInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgResponseTime, iSource: "/Reports/GetAvgResponseTimeInfo")
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report3 = new Report(3, Resources.Global.RepIncomingVsOutgoing, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms with granularity", iSource: "/Reports/GetSmsIncomingOutgoingDetailed") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "/Reports/GetSmsTotalInfo"),  
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfSmsPerDay, iSource: "/Reports/GetSmsAvgPerDayInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfNewClients, iSource: "/Reports/GetNoOfNewClientsInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfOutgoingSmsPerConversation, iSource: "/Reports/GetOutgoingSmsAvgPerClientInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfIncomingSmsPerConversation, iSource: "/Reports/GetIncomingSmsAvgPerClientInfo")
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report4 = new Report(4, Resources.Global.RepTags, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("No of conversations by tags", iSource: "/Reports/GetNoOfConversationsByTag", iOptions: new ReportResourceOptions(Constants.BARS_CHART_STYLE)) 
                                                                                                                                                            
                                                                                                                                                             
                                                                                                                                                            }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepMostUsedTag, iSource: "/Reports/GetMostUsedTagInfo"),     
                                                                                                                                                    new ReportResource(Resources.Global.RepAvgNoOfTagsPerConversation, iSource: "/Reports/GetAverageNoOfTagsPerConversationInfo")                                                                                                                                                  
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report6 = new Report(6, Resources.Global.RepOverview, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms with granularity", iSource: "/Reports/GetClientsNewVsReturningDetailed") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "/Reports/GetSmsTotalInfo"),                                                                                                                                                    
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
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
            hashTable.Add(2, report2);
            hashTable.Add(3, report3);
            hashTable.Add(4, report4);
            hashTable.Add(6, report6);
            hashTable.Add(7, report7);

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

    }

}

