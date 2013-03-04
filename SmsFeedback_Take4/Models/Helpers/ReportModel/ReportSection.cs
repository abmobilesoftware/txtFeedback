using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class ReportSection
    {
        public string type;
        public string id;
        public string groupId;
        public string title;
        public ReportResourceOptions options;
        public string tooltip;
        public int dataIndex;
        public string chartSource;

        public ReportSection(string iType, 
            string iTitle,
            int iDataIndex,
            ReportResourceOptions iOptions = null,
            string iTooltip = "no tooltip", 
            string iSectionId = "-1", 
            string iGroupId = "-1",
            string iChartSource = "no source")
        {
            type = iType;
            title = iTitle;
            options = iOptions;
            tooltip = iTooltip;
            id = iSectionId;
            groupId = iGroupId;
            dataIndex = iDataIndex;
            chartSource = iChartSource;

            Guid groupUUID = Guid.NewGuid();
            Guid sectionUUID = Guid.NewGuid();
            if (id.Equals("-1")) id = sectionUUID.ToString();
            if (groupId.Equals("-1")) groupId = groupUUID.ToString();
        }
    }
}