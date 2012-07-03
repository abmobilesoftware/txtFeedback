using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmsFeedback_Take4.Utilities;

namespace TxtFeedback_tests.Utilities
{
   class Utilities_Tests
   {
      private TestContext testContextInstance;
      /// <summary>
      ///Gets or sets the test context which provides
      ///information about and functionality for the current test run.
      ///</summary>
      public TestContext TestContext
      {
         get
         {
            return testContextInstance;
         }
         set
         {
            testContextInstance = value;
         }
      }

      /// <summary>
      ///A test for BuildConversationIDFromFromAndTo
      ///</summary>      
      [Test]
      public void BuildConversationIDFromFromAndTo_ValidFromTo_returnsValidConvId()
      {
         string from = "442033221909";
         string to = "442033221134";
         string expected = "442033221909-442033221134";
         string actual;
         actual = ConversationUtilities.BuildConversationIDFromFromAndTo(from, to);
         Assert.AreEqual(expected, actual);
      }

      /// <summary>
      ///A test for GetFromAndToFromConversationID
      ///</summary>      
      [Test]
      public void GetFromAndToFromConversationIDTest_ValidConvID_returnsValidArrayWithFromAndTo()
      {
         string conversationID = "442033221909-442033221134"; // TODO: Initialize to an appropriate value
         string[] expected = { "442033221909", "442033221134" }; // TODO: Initialize to an appropriate value
         string[] actual;
         actual = ConversationUtilities.GetFromAndToFromConversationID(conversationID);
         CollectionAssert.AreEqual(expected, actual);
         //Assert.AreEqual(expected, actual);         
      }
   }
}


