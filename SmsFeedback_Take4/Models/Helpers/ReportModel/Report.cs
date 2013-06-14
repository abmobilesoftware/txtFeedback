using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class Report
    {
        public int reportId;
        public string title;
        public IEnumerable<ReportSection> sections;
        public string source;

        public Report(int iReportId, string iTitle, String iSource, IEnumerable<ReportSection> iSections)
        {
            reportId = iReportId;
            title = iTitle;
            source = iSource;
            sections = iSections;            
        }
    }
}