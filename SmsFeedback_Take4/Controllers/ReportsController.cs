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
                var row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("15.07.2012"), new RepDataRowCell(18) });
                var row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("16.07.2012"), new RepDataRowCell(19, "19") });
                var row3 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("17.07.2012"), new RepDataRowCell(10) });
                var row4 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("18.07.2012"), new RepDataRowCell(12) });
                var row5 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("19.07.2012"), new RepDataRowCell(7) });
                var row6 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("20.07.2012"), new RepDataRowCell(20) });
                List<RepDataRow> content = new List<RepDataRow>();
                content.Add(row1);
                content.Add(row2);
                content.Add(row3);
                content.Add(row4);
                content.Add(row5);
                content.Add(row6);

                RepChartData chartSource = new RepChartData(new RepDataColumn[] {new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Total no of sms")}, content);

                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            else if (iGranularity == Constants.weekGranularity) 
            {
                var row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("15.07.2012"), new RepDataRowCell(18) });
                var row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("16.07.2012"), new RepDataRowCell(15) });
                var row3 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("16.07.2012"), new RepDataRowCell(24) });

                List<RepDataRow> content = new List<RepDataRow>();
                content.Add(row1);
                content.Add(row2);
                content.Add(row3);

                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Total no of sms") }, content);
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var row1 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("15.07.2012"), new RepDataRowCell(18) });
                var row2 = new RepDataRow(new RepDataRowCell[] { new RepDataRowCell("16.07.2012"), new RepDataRowCell(19, "19") });
                List<RepDataRow> content = new List<RepDataRow>();
                content.Add(row1);
                content.Add(row2);

                RepChartData chartSource = new RepChartData(new RepDataColumn[] { new RepDataColumn("17", "string", "Date"), new RepDataColumn("18", "number", "Total no of sms") }, content);
                return Json(chartSource, JsonRequestBehavior.AllowGet);
            }

        }

    }

   public class ReportMaxNrOfSms {       
   }}
