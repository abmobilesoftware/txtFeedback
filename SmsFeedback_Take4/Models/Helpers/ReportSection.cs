using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class ReportSection
    {
        public string identifier;
        public bool visibility;
        public IEnumerable<ReportResource> resources;
        public string uniqueId;
        public string sectionId;

        public ReportSection(string iIdentifier, bool iVisibility, IEnumerable<ReportResource> iResources, string iUniqueId = "-1", string iSectionId = "-1")
        {
            identifier = iIdentifier;
            visibility = iVisibility;
            resources = iResources;
            uniqueId = iUniqueId;
            sectionId = iSectionId;
            
            Guid guid = Guid.NewGuid();
            if (uniqueId.Equals("-1")) uniqueId = ConversationUtilities.RandomNumberGenerator.Next(30).ToString();
            if (sectionId.Equals("-1")) sectionId = guid.ToString();
        }
    }
}