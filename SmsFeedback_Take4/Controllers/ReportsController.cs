using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Models.Helpers;
using SmsFeedback_Take4.Utilities;


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

        public JsonResult GetSmsIncomingOutgoingDetailed(String iIntervalStart, String iIntervalEnd, String iGranularity, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            // interval start and end dates received as rfc822
            if (iGranularity == Constants.dayGranularity)
            {
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Outgoing sms"), new RepDataColumn("19", "number", "Incoming sms") }, BogusDataGenerator(30, scope));

                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            else if (iGranularity == Constants.weekGranularity)
            {
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Outgoing sms"), new RepDataColumn("19", "number", "Incoming sms") }, BogusDataGenerator(5, scope));
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            else
            {
                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Outgoing sms"), new RepDataColumn("19", "number", "Incoming sms") }, BogusDataGenerator(2, scope));
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetSmsIncomingOutgoingTotal(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("Incoming", "Incoming"), new RepDataRowCell(315, "315 sms - from customers") });
            var row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("Outgoing", "Outgoing"), new RepDataRowCell(136, "136 sms - to customers") });

            List<RepDataRow> content = new List<RepDataRow>();
            content.Add(row1);
            content.Add(row2);

            RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Type"), new RepDataColumn("18", "number", "Number") }, content);
            return Json(chartSource, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetTotalNoOfSmsInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return Json(new RepInfoBox(350, "sms"), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAvgResponseTimeInfo(String iIntervalStart, String iIntervalEnd, String culture, String scope)
        {
            DateTime intervalStart = DateTime.ParseExact(iIntervalStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime intervalEnd = DateTime.ParseExact(iIntervalEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return Json(new RepInfoBox(7, "min"), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetReportsMenuItems()
        {
            ReportsMenuItem[] reportsMenuItems = new ReportsMenuItem[] { new ReportsMenuItem(1, Resources.Global.RepConversations, false, 1), new ReportsMenuItem(2, Resources.Global.RepOverview, true, 1), 
                                                                         new ReportsMenuItem(3, Resources.Global.RepNewVsReturning, true, 1), new ReportsMenuItem(4, Resources.Global.RepClients, false, 4),
                                                                         new ReportsMenuItem(5, Resources.Global.RepOverview, true, 4), new ReportsMenuItem(6, Resources.Global.RepNewVsReturning , true, 4)};
            return Json(reportsMenuItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportById(int reportId)
        {
            var hashTable = new Dictionary<int, Report>();
            var report2 = new Report(2, Resources.Global.RepOverview, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms with granularity", iSource: "Reports/GetSmsIncomingOutgoingDetailed") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "Reports/GetTotalNoOfSmsInfo"),
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "Reports/GetTotalNoOfSmsInfo")
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report3 = new Report(3, Resources.Global.RepIncomingVsOutgoing, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms with granularity", iSource: "Reports/GetSmsIncomingOutgoingDetailed") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "Reports/GetTotalNoOfSmsInfo"),                                                                                                                                                    
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report5 = new Report(5, Resources.Global.RepOverview, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms with granularity", iSource: "Reports/GetSmsIncomingOutgoingDetailed") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "Reports/GetTotalNoOfSmsInfo"),                                                                                                                                                    
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            var report6 = new Report(6, Resources.Global.RepNewVsReturning, "Global", new ReportSection[] { 
                                                                                        new ReportSection("PrimaryChartArea", true, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms with granularity", iSource: "Reports/GetSmsIncomingOutgoingDetailed") 
                                                                                                                                                          }),
                                                                                        new ReportSection("InfoBox", true, new ReportResource[] { 
                                                                                                                                                    new ReportResource(Resources.Global.RepTotalNoOfSms, iSource: "Reports/GetTotalNoOfSmsInfo"),                                                                                                                                                    
                                                                                                                                                }),
                                                                                        new ReportSection("SecondaryChartArea", false, new ReportResource[] { 
                                                                                                                                                            new ReportResource("Incoming vs Outgoing Sms total", iSource: "Reports/GetSmsIncomingOutgoingTotal") 
                                                                                                                                                          }),
                                                                                    });
            hashTable.Add(2, report2);
            hashTable.Add(3, report3);
            hashTable.Add(5, report5);
            hashTable.Add(6, report6);

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

