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
        public string scope;
        public IEnumerable<ReportSection> sections;
        public string source;

        public Report(int iReportId, string iTitle, string iScope, String iSource, IEnumerable<ReportSection> iSections)
        {
            reportId = iReportId;
            title = iTitle;
            scope = iScope;
            source = iSource;
            sections = iSections;            
        }
    }
}