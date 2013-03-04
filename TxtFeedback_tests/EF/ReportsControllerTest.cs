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

        [Test]
        public void GetNoOfComputedClients()
        {
            DateTime today = DateTime.Now;
            DateTime lastMonth = today.AddDays(-30);
            smsfeedbackEntities context = new smsfeedbackEntities();
            IEnumerable<WorkingPoint> wps = efInteraction.GetWorkingPointsForAUser(Constants.GLOBAL_SCOPE, "ando", context);
            Int32 noOfClients1 = reportsController.ComputeTotalNoOfClients(lastMonth, today, wps);
            Console.Write("No of clients 1 = " + noOfClients1);
            
            Int32 noOfClients2 = reportsController.ComputeTotalNoOfClients(lastMonth, today, wps);
            Console.Write("No of clients 2 = " + noOfClients2);
            Assert.AreEqual(noOfClients1, noOfClients2);
        }
    }
}
