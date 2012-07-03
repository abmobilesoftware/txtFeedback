using    System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedbackTest.EF_tests
{
   [TestClass]
   public class EFInteraction_Tests
   {
      EFInteraction mContext;
      [TestInitialize]
      public void Initialise()
      {
         mContext = new EFInteraction();
      }

      [TestMethod]
      public void AddTagToDb_addValidParameters_TagIsAddedAndReturned()
      {
         string tagName = "testTag1";
         string tagDescription = "testDescription1";
         string userName = "ando";
         var newlyAddedTag = mContext.AddTagToDB(tagName, tagDescription, userName);
         Assert.IsNotNull(newlyAddedTag);
         Assert.AreEqual(tagName, newlyAddedTag.Name);
         Assert.AreEqual(tagDescription, newlyAddedTag.Description);         
      }
   }
}
