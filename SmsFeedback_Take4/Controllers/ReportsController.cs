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
            foreach (var entry in daysInterval) {
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
            TimeSpan noOfDays = intervalEnd - intervalStart;

            return Json(new RepInfoBox(totalNoOfSms/noOfDays.Days, "sms"), JsonRequestBehavior.AllowGet);
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

        public JsonResult GetAvgResponseTimeInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            intervalEnd = intervalEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            return Json(new RepInfoBox(7, "min"), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetNoOfNewClients(String iIntervalStart, String iIntervalEnd, String culture, String scope)
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
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfNewClients, iSource: "/Reports/GetNoOfNewClients")
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
                                                                                                                                                    new ReportResource(Resources.Global.RepNoOfNewClients, iSource: "/Reports/GetNoOfNewClients")
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report4 = new Report(4, Resources.Global.RepTags, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms with granularity", iSource: "/Reports/GetSmsIncomingOutgoingDetailed") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "/Reports/GetSmsTotalInfo"),                                                                                                                                                    
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "/Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report6 = new Report(6, Resources.Global.RepOverview, "Global", new ReportSection[] { 
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

        public List<RepDataRow> BogusDataGenerator(int intervalEnd, string workingPoint) {
                
                var hashTable = new Dictionary<String, RepDataRow>();
                var keyPrefix = "row";               

                int[] incomingSmsGlobal = new int[31] {17, 15, 16, 15, 17, 16, 14, 13, 14, 15, 16, 17, 15, 14, 11, 13, 16, 15, 14, 13, 13, 12, 11, 12, 13, 14, 15, 16, 15, 16, 14};
                int[] incomingSmsWP1 = new int[31] {18, 19, 21, 22, 21,18, 16, 19, 20, 21, 23, 25, 26, 27, 27,29, 28, 27, 25, 24, 23, 20, 19, 19, 21, 23, 24, 27, 30, 31, 32};
                int[] incomingSmsWP2 = new int[31] {23, 19, 18, 17, 15, 12, 11, 9, 8, 7, 11, 10, 11, 13, 15, 16, 17, 17,19, 18, 23, 25, 24, 23, 20, 19, 19, 21, 23, 24, 27};

                int[] outgoingSmsGlobal = new int[31] {15, 14, 15, 15, 14, 15, 13, 12, 13, 14, 15, 16, 14, 13, 10, 12, 15, 14, 13, 12, 12, 11, 10, 11, 12, 13, 14, 15, 14, 15, 13};
                int[] outgoingSmsWP1 = new int[31] {17, 18, 20, 21, 20, 17, 15, 18, 19, 18, 17, 16, 15, 14, 13, 15, 14, 11, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13};
                int[] outgoingSmsWP2 = new int[31] {22, 18, 17, 16, 14, 11, 10, 8, 7, 6, 10, 9, 10, 12, 14, 15, 16, 16, 18, 17, 22, 24, 23, 22, 19, 18, 18, 20, 22, 23, 26};

                int[] incoming = incomingSmsWP2;
                int[] outgoing = outgoingSmsWP2; 

                if (workingPoint.Equals("Global")) {
                    incoming = incomingSmsGlobal;
                    outgoing = outgoingSmsGlobal;
                } else if (workingPoint.Equals("WP1")) {
                    incoming = incomingSmsWP1;
                    outgoing = outgoingSmsWP1;
                } else {
                    incoming = incomingSmsWP2;
                    outgoing = outgoingSmsWP2;
                }
                
                for (int i=0; i < intervalEnd; ++i) {
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

