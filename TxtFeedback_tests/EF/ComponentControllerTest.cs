using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmsFeedback_Take4.Controllers;
using Newtonsoft.Json;
using System.Web.Mvc;
using SmsFeedback_Take4.Utilities;

namespace TxtFeedback_tests.EF
{
    [TestFixture]
    class ComponentControllerTest
    {
        ComponentController componentController;

        [TestFixtureSetUp]
        public void Initialise()
        {
            componentController = new ComponentController();
        }

        [Test]
        public void SaveMessage_imMessageToStaff_SuccessJsonReturned()
        {
            string from = "xy1234@txtfeedback.net";
            string to = "shop20@moderator.txtfeedback.net";
            string convId = "xy1234-shop20";
            Random rnd = new Random();
            string text = "test+url+decode+" + rnd.Next().ToString();
            string xmppUser = Constants.DONT_ADD_XMPP_USER;
            bool isSms = false;
            JsonResult result = componentController.SaveMessage(from, to, convId, text, xmppUser, isSms);
            Assert.IsNotNull(result);
            Assert.AreEqual("success", result.Data.ToString());
            Assert.AreEqual(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
            //the tagcompany will be based on the userName
        }

        [Test]
        public void SaveMessage_imMessageToClient_SuccessJsonReturned()
        {
            string from = "shop20@moderator.txtfeedback.net";
            string to = "xy1234@txtfeedback.net";
            string convId = "xy1234-shop20";
            Random rnd = new Random();
            string text = "test+url+decode+" + rnd.Next().ToString();
            string xmppUser = "m32@txtfeedback.net";
            bool isSms = false;
            JsonResult result = componentController.SaveMessage(from, to, convId, text, xmppUser, isSms);
            Assert.IsNotNull(result);
            Assert.AreEqual("success", result.Data.ToString());
            Assert.AreEqual(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
            //the tagcompany will be based on the userName
        }

        [Test]
        public void SaveMessage_smsMessageToStaff_SuccessJsonReturned()
        {
            string from = "xy1234@txtfeedback.net";
            string to = "shop20@moderator.txtfeedback.net";
            string convId = "xy1234-shop20";
            Random rnd = new Random();
            string text = "test+url+decode+" + rnd.Next().ToString();
            string xmppUser = Constants.DONT_ADD_XMPP_USER;
            bool isSms = false;
            JsonResult result = componentController.SaveMessage(from, to, convId, text, xmppUser, isSms);
            Assert.IsNotNull(result);
            Assert.AreEqual("success", result.Data.ToString());
            Assert.AreEqual(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
            //the tagcompany will be based on the userName
        }

        [Test]
        public void SaveMessage_invalidWPID_returnsMessageWithInvalidWpID()
        {
            string from = "xy1234@txtfeedback.net";
            string to = "bogusworkingpointid@moderator.txtfeedback.net";
            string convId = "xy1234-bogusWorkingPointId";
            Random rnd = new Random();
            string text = "test+url+decode+" + rnd.Next().ToString();
            string xmppUser = Constants.DONT_ADD_XMPP_USER;
            bool isSms = false;
            JsonResult result = componentController.SaveMessage(from, to, convId, text, xmppUser, isSms);
            Assert.IsNotNull(result);
            Assert.AreEqual(JsonReturnMessages.INVALID_WPID, result.Data.ToString());
            Assert.AreEqual(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
        }

        [Test]
        public void SaveMessage_InvalidConvIdFromCombination_returnsMessageWithInvalidDirection()
        {
            string from = "xy12345@txtfeedback.net";
            string to = "bogusworkingpointid@moderator.txtfeedback.net";
            string convId = "xy1234-shop20";
            Random rnd = new Random();
            string text = "test+url+decode+" + rnd.Next().ToString();
            string xmppUser = Constants.DONT_ADD_XMPP_USER;
            bool isSms = false;
            JsonResult result = componentController.SaveMessage(from, to, convId, text, xmppUser, isSms);
            Assert.IsNotNull(result);
            Assert.AreEqual(JsonReturnMessages.INVALID_DIRECTION, result.Data.ToString());
            Assert.AreEqual(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
        }

        [Test]
        public void SaveMessage_ImAddressAndWrongSmsIndicator_returnsMessageWithInvalidDirection()
        {
            string from = "xy1234@txtfeedback.net";
            string to = "bogusworkingpointid@moderator.txtfeedback.net";
            string convId = "xy1234-shop20";
            string text = "test+url+decode";
            string xmppUser = Constants.DONT_ADD_XMPP_USER;
            bool isSms = true;
            JsonResult result = componentController.SaveMessage(from, to, convId, text, xmppUser, isSms);
            Assert.IsNotNull(result);
            Assert.AreEqual(JsonReturnMessages.INVALID_DIRECTION, result.Data.ToString());
            Assert.AreEqual(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
        }
       /* [Test]
        public void SaveMessage_smsMessageToClient_SuccessJsonReturned()
        {
            string from = "shop20@moderator.txtfeedback.net";
            string to = "xy1234@txtfeedback.net";
            string convId = "xy1234-shop20";
            string text = "test+url+decode";
            string xmppUser = "m32@txtfeedback.net";
            bool isSms = false;
            JsonResult result = componentController.SaveMessage(from, to, convId, text, xmppUser, isSms);
            Assert.IsNotNull(result);
            Assert.AreEqual("success", result.Data.ToString());
            Assert.AreEqual(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
            //the tagcompany will be based on the userName
        }*/
    }
}
