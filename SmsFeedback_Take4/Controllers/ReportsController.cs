using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Models.Helpers;
using SmsFeedback_Take4.Utilities;


namespace SmsFeedback_Take4.Controllers
{
   public class ReportsController : BaseController
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            ViewData["currentCulture"] = getCurrentCulture();
            return View();
        }

        public JsonResult getTotalNoOfSms(String iIntervalStart, String iIntervalEnd, String iGranularity)
        {
            if (iGranularity == Constants.dayGranularity)
            {
                // a lot of bogus data
                var hashTable = new Dictionary<String, RepDataRow>();
                hashTable.Add("row1", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("15.07.2012"), new RepDataRowCell(18, "18 sms - 15.07.2012"), new RepDataRowCell(15, "15 sms - 15.07.2012") }));
                hashTable.Add("row2", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("16.07.2012"), new RepDataRowCell(19, "19 sms - 16.07.2012"), new RepDataRowCell(14, "14 sms - 16.07.2012") }));
                hashTable.Add("row3", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("17.07.2012"), new RepDataRowCell(21, "21 sms - 17.07.2012"), new RepDataRowCell(16, "16 sms - 17.07.2012") }));
                hashTable.Add("row4", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("18.07.2012"), new RepDataRowCell(22, "22 sms - 18.07.2012"), new RepDataRowCell(19, "19 sms - 18.07.2012") }));
                hashTable.Add("row5", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("19.07.2012"), new RepDataRowCell(21, "23 sms - 19.07.2012"), new RepDataRowCell(20, "20 sms - 18.07.2012") }));
                hashTable.Add("row6", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("20.07.2012"), new RepDataRowCell(18, "18 sms - 20.07.2012"), new RepDataRowCell(17, "17 sms - 20.07.2012") }));
                hashTable.Add("row7", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("21.07.2012"), new RepDataRowCell(16, "16 sms - 21.07.2012"), new RepDataRowCell(16, "16 sms - 21.07.2012") }));
                hashTable.Add("row8", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("22.07.2012"), new RepDataRowCell(19, "19 sms - 22.07.2012"), new RepDataRowCell(18, "18 sms - 22.07.2012") }));
                hashTable.Add("row9", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("23.07.2012"), new RepDataRowCell(20, "20 sms - 23.07.2012"), new RepDataRowCell(19, "19 sms - 23.07.2012") }));
                hashTable.Add("row10", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("24.07.2012"), new RepDataRowCell(21, "21 sms - 24.07.2012"), new RepDataRowCell(20, "20 sms - 24.07.2012") }));
                hashTable.Add("row11", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("25.07.2012"), new RepDataRowCell(23, "23 sms - 25.07.2012"), new RepDataRowCell(22, "22 sms - 25.07.2012") }));
                hashTable.Add("row12", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("26.07.2012"), new RepDataRowCell(25, "25 sms - 26.07.2012"), new RepDataRowCell(24, "24 sms - 26.07.2012") }));
                hashTable.Add("row13", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("27.07.2012"), new RepDataRowCell(26, "26 sms - 27.07.2012"), new RepDataRowCell(25, "25 sms - 27.07.2012") }));
                hashTable.Add("row14", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("28.07.2012"), new RepDataRowCell(27, "27 sms - 28.07.2012"), new RepDataRowCell(26, "26 sms - 28.07.2012") }));
                hashTable.Add("row15", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("29.07.2012"), new RepDataRowCell(27, "27 sms - 29.07.2012"), new RepDataRowCell(25, "25 sms - 29.07.2012") }));
                hashTable.Add("row16", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("30.07.2012"), new RepDataRowCell(29, "29 sms - 30.07.2012"), new RepDataRowCell(28, "28 sms - 30.07.2012") }));
                hashTable.Add("row17", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("31.07.2012"), new RepDataRowCell(28, "28 sms - 31.07.2012"), new RepDataRowCell(28, "28 sms - 31.07.2012") }));
                hashTable.Add("row18", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("01.08.2012"), new RepDataRowCell(27, "27 sms - 01.08.2012"), new RepDataRowCell(25, "25 sms - 01.08.2012") }));
                hashTable.Add("row19", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("02.08.2012"), new RepDataRowCell(25, "25 sms - 02.08.2012"), new RepDataRowCell(23, "23 sms - 02.08.2012") }));
                hashTable.Add("row20", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("03.08.2012"), new RepDataRowCell(24, "24 sms - 03.08.2012"), new RepDataRowCell(23, "23 sms - 02.08.2012") }));
                hashTable.Add("row21", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("04.08.2012"), new RepDataRowCell(23, "23 sms - 04.08.2012"), new RepDataRowCell(22, "22 sms - 04.08.2012") }));
                hashTable.Add("row22", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("05.08.2012"), new RepDataRowCell(20, "20 sms - 05.08.2012"), new RepDataRowCell(19, "20 sms - 05.08.2012") }));
                hashTable.Add("row23", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("06.08.2012"), new RepDataRowCell(19, "19 sms - 06.08.2012"), new RepDataRowCell(18, "18 sms - 06.08.2012") }));
                hashTable.Add("row24", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("07.08.2012"), new RepDataRowCell(19, "19 sms - 07.08.2012"), new RepDataRowCell(18, "18 sms - 07.08.2012") }));
                hashTable.Add("row25", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("08.08.2012"), new RepDataRowCell(21, "21 sms - 08.08.2012"), new RepDataRowCell(17, "17 sms - 08.08.2012") }));
                hashTable.Add("row26", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("09.08.2012"), new RepDataRowCell(23, "23 sms - 09.08.2012"), new RepDataRowCell(22, "22 sms - 09.08.2012") }));
                hashTable.Add("row27", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("10.08.2012"), new RepDataRowCell(24, "24 sms - 10.08.2012"), new RepDataRowCell(23, "23 sms - 10.08.2012") }));
                hashTable.Add("row28", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("11.08.2012"), new RepDataRowCell(27, "27 sms - 11.08.2012"), new RepDataRowCell(26, "26 sms - 11.08.2012") }));
                hashTable.Add("row29", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("12.08.2012"), new RepDataRowCell(30, "30 sms - 12.08.2012"), new RepDataRowCell(29, "29 sms - 12.08.2012") }));
                hashTable.Add("row30", new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("13.08.2012"), new RepDataRowCell(31, "31 sms - 13.08.2012"), new RepDataRowCell(31, "31 sms - 13.08.2012") }));
                List<RepDataRow> content = new List<RepDataRow>();
                for (int i = 1; i < 30; ++i)
                {
                    var key = "row" + i;
                    content.Add(hashTable[key]);                    
                }


                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Outgoing sms"), new RepDataColumn("19", "number", "Incoming sms")}, content);

                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            else if (iGranularity == Constants.weekGranularity) 
            {
                var row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("15.07.2012-21.07.2012"), new RepDataRowCell(196, "196 sms - 15.07.2012-21.07.2012"), new RepDataRowCell(185, "185 sms - 15.07.2012-21.07.2012") });
                var row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("22.07.2012-28.07.2012"), new RepDataRowCell(221, "221 sms - 22.07.2012-28.07.2012"), new RepDataRowCell(205, "205 sms - 22.07.2012-28.07.2012") });
                var row3 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("29.07.2012-04.08.2012"), new RepDataRowCell(178, "178 sms - 29.07.2012-04.08.2012"), new RepDataRowCell(170, "172 sms - 29.07.2012-04.08.2012") });
                var row4 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("05.08.2012-11.08.2012"), new RepDataRowCell(231, "231 sms - 05.08.2012-11.08.2012"), new RepDataRowCell(198, "198 sms - 05.08.2012-11.08.2012") });
                var row5 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("12.07.2012-13.08.2012"), new RepDataRowCell(240, "240 sms - 12.07.2012-13.08.2012"), new RepDataRowCell(212, "212 sms - 12.07.2012-13.08.2012") });

                List<RepDataRow> content = new List<RepDataRow>();
                content.Add(row1);
                content.Add(row2);
                content.Add(row3);
                content.Add(row4);
                content.Add(row5);

                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Outgoing sms"), new RepDataColumn("19", "number", "Incoming sms") }, content);
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("15.08.2012-31.07.2012"), new RepDataRowCell(315, "315 sms - 15.07.2012-31.07.2012"), new RepDataRowCell(278, "278 sms - 15.07.2012-21.07.2012") });
                var row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("01.08.2012-13.08.2012"), new RepDataRowCell(325, "325 sms - 01.08.2012-13.08.2012"), new RepDataRowCell(288, "288 sms - 01.08.2012-13.08.2012") });
                
                List<RepDataRow> content = new List<RepDataRow>();
                content.Add(row1);
                content.Add(row2);

                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Outgoing sms"), new RepDataColumn("19", "number", "Incoming sms") }, content);
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }

        }

    }

   public class ReportMaxNrOfSms {       
   }}
