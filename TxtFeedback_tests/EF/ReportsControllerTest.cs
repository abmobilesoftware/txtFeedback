using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmsFeedback_Take4.Controllers;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Utilities;

namespace TxtFeedback_tests.EF
{
     [TestFixture]
    class ReportsControllerTest
    {
        ReportsController reportsController;
        EFInteraction efInteraction;

        [TestFixtureSetUp]
        public void Initialise()
        {
            reportsController = new ReportsController();
            efInteraction = new EFInteraction();
        }

    }
}
