using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class ReportSection
    {
        public string identifier;
        public bool visibility;
        public IEnumerable<ReportResource> resources;

        public ReportSection(string iIdentifier, bool iVisibility, IEnumerable<ReportResource> iResources)
        {
            identifier = iIdentifier;
            visibility = iVisibility;
            resources = iResources;
        }
    }
}