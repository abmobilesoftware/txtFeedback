using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmsFeedback_Take4.Utilities;
namespace TxtFeedback_tests
{
   [TestFixture]
    public class EFInteractionTests
    {
      //first we test the "success" scenarios as those are the most important once we refactor the db
       EFInteraction mContext;
       
      [TestFixtureSetUp]
       public void Initialise()
       {
          mContext = new EFInteraction();
       }

       [Test]
       public void AddTagToDb_addValidParameters_TagIsAddedAndReturned()
       {
          string tagName = "testTag1";
          string tagDescription = "testDescription1";
          string userName = "ando";
          /*var newlyAddedTag = mContext.AddTagToDB(tagName, tagDescription, userName);
          Assert.IsNotNull(newlyAddedTag);
          Assert.AreEqual(tagName, newlyAddedTag.Name);
          Assert.AreEqual(tagDescription, newlyAddedTag.Description);*/
          //the tagcompany will be based on the userName
       }
       
       void AddMessage_addValidParameters_MessagesIsAddedAndReturned()
       { 

       }

      [Test]
      void UpdateAddConversation_AddNewConversation_idOfTheNewConversationIsReturned()
      {
         //this will add a tag - it up to someone else to clean up the db afterwards
         string from = "1000000000";
         string to = "1000000000";
         string conversationID = "1000000000-0751569436";
         string text = "what's up";
         bool readStatus = false;
         DateTime? updateTime = DateTime.Now;
         bool markAsRead = false;
         /*var convIDofNewConv = mContext.UpdateAddConversation(from, to, conversationID, text, readStatus, updateTime, markAsRead);
         Assert.IsNotNull(convIDofNewConv);*/
      }
    } 
}
