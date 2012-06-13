using SmsFeedback_Take4.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace SmsFeedbackTest
{
    /// <summary>
    ///This is a test class for UtilitiesTest and is intended
    ///to contain all UtilitiesTest Unit Tests
    ///</summary>
   [TestClass()]
   public class UtilitiesTest
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

      #region Additional test attributes
      // 
      //You can use the following additional attributes as you write your tests:
      //
      //Use ClassInitialize to run code before running the first test in the class
      //[ClassInitialize()]
      //public static void MyClassInitialize(TestContext testContext)
      //{
      //}
      //
      //Use ClassCleanup to run code after all tests in a class have run
      //[ClassCleanup()]
      //public static void MyClassCleanup()
      //{
      //}
      //
      //Use TestInitialize to run code before running each test
      //[TestInitialize()]
      //public void MyTestInitialize()
      //{
      //}
      //
      //Use TestCleanup to run code after each test has run
      //[TestCleanup()]
      //public void MyTestCleanup()
      //{
      //}
      //
      #endregion


      /// <summary>
      ///A test for BuildConversationIDFromFromAndTo
      ///</summary>      
      [TestMethod()]      
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
      [TestMethod()]     
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
