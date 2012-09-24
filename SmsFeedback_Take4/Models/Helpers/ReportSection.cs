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
        public int uniqueId;

        public ReportSection(string iIdentifier, bool iVisibility, IEnumerable<ReportResource> iResources, int iUniqueId = -1)
        {
            identifier = iIdentifier;
            visibility = iVisibility;
            resources = iResources;
            uniqueId = iUniqueId;
            Random randomNumberGenerator = new Random();
            if (uniqueId == -1) uniqueId = randomNumberGenerator.Next();           
        }
    }
}