using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmsFeedback_Take4.Utilities;
using SmsFeedback_EFModels;
using System.Globalization;
namespace TxtFeedback_tests
{
   [TestFixture]
    public class EFInteractionTests
    {
      //first we test the "success" scenarios as those are the most important once we refactor the db
       EFInteraction mEFinter;
       smsfeedbackEntities context;

      [TestFixtureSetUp]
       public void Initialise()
       {
          mEFinter = new EFInteraction();
         context =  new smsfeedbackEntities();
       }
      [TestFixtureTearDown]
      public void TearDown()
      {
         context.Dispose();
      }

       //[Test]
       //public void AddTagToDb_addValidParameters_TagIsAddedAndReturned()
       //{
       //   string tagName = "testTag1";
       //   string tagDescription = "testDescription1";
       //   string userName = "ando";
       //   /*var newlyAddedTag = mContext.AddTagToDB(tagName, tagDescription, userName);
       //   Assert.IsNotNull(newlyAddedTag);
       //   Assert.AreEqual(tagName, newlyAddedTag.Name);
       //   Assert.AreEqual(tagDescription, newlyAddedTag.Description);*/
       //   //the tagcompany will be based on the userName
       //}
       
       //public void AddMessage_addValidParameters_MessagesIsAddedAndReturned()
       //{ 

       //}

      //[Test]
      //public void UpdateAddConversation_AddNewConversation_idOfTheNewConversationIsReturned()
      //{
      //   //this will add a tag - it up to someone else to clean up the db afterwards
      //   string from = "1000000000";
      //   string to = "1000000000";
      //   string conversationID = "1000000000-0751569436";
      //   string text = "what's up";
      //   bool readStatus = false;
      //   DateTime? updateTime = DateTime.Now;
      //   bool markAsRead = false;
      //   /*var convIDofNewConv = mContext.UpdateAddConversation(from, to, conversationID, text, readStatus, updateTime, markAsRead);
      //   Assert.IsNotNull(convIDofNewConv);*/
      //}

      [Test]
      public void UpdateSMSstatusForCompany_Update_timestampIsModifiedAfterUpdate()
      {         
         var wpID = "19802294808";
         var wp = context.WorkingPoints.Find(wpID);
         var sd = wp.Users.FirstOrDefault().Company.SubscriptionDetail;
         var initialTimestamp = sd.TimeStamp;
         var price = "0.1";
         mEFinter.UpdateSMSstatusForCompany(wpID,price, context);
         context.Entry(sd).Reload();
         var resultTimestamp = sd.TimeStamp;
         Assert.AreNotEqual(initialTimestamp, resultTimestamp);        
      }

      [Test]
      public void UpdateSMSForSubscription_2ContextsChangePriceTwiceNoRemainingSMS_PriceHasCorrectValueAtTheEnd()
      {         
         var wpID = "19802294808";//Wallmart only sms
         var price = "0.1";
         
         var wp = context.WorkingPoints.Find(wpID);
         var sd1 = wp.Users.FirstOrDefault().Company.SubscriptionDetail;
         //make sure that remaining sms is 0 so that we nibble through our spent amount
         sd1.RemainingSMS = 0;
         context.SaveChanges();
         context.Entry(sd1).Reload();
         smsfeedbackEntities altContext = new smsfeedbackEntities();
         var wp2 = altContext.WorkingPoints.Find(wpID);
         var sd2 = wp2.Users.FirstOrDefault().Company.SubscriptionDetail;
         //make sure that we are starting from the same point
         var sd1BeforeTimestamp = sd1.TimeStamp;
         var sd2BeforeTimestamp = sd2.TimeStamp;
         Assert.AreEqual(sd1BeforeTimestamp, sd2BeforeTimestamp, "The initial timestamp should be the same as we start from the same starting point");
         var initialSA = sd1.SpentThisMonth;
         mEFinter.UpdateSMSForSubscription(price, sd1, context);
         context.Entry(sd1).Reload();
         var sd1Timestamp = sd1.TimeStamp;
         mEFinter.UpdateSMSForSubscription(price, sd2, altContext);
         var sd2Timestamp = sd2.TimeStamp;
         
         Assert.AreNotEqual(sd2Timestamp, sd1Timestamp,"The subscription should have been modified twice (timestamp wise" );
         
         var finalSA = sd2.SpentThisMonth;
         var expectedPrice = initialSA + 2 * Decimal.Parse(price, CultureInfo.InvariantCulture);
         Assert.AreEqual(expectedPrice, finalSA, "The price should have been modified twice");
         altContext.Dispose();
      }

      [Test]
      public void UpdateSMSForSubscription_2ContextsChangeWithMoreThan2RemainingSMS_RemainingSMSHasCorrectValueAtTheEnd()
      {
         var wpID = "19802294808";//Wallmart only sms
         var price = "0.1";
         var initialRS = 200;
         var wp = context.WorkingPoints.Find(wpID);
         var sd1 = wp.Users.FirstOrDefault().Company.SubscriptionDetail;
         //make sure that RemainingSMS is not 0 so that we have where to subtract from
         bool saved = false;
         do
         {
            saved = true;
            try
            {
               sd1.RemainingSMS = initialRS;
               context.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {
               saved = false;
               ex.Entries.Single().Reload();
            }
         } while (!saved);         
        
         context.Entry(sd1).Reload();
         //initialize second context
         smsfeedbackEntities altContext = new smsfeedbackEntities();
         var wp2 = altContext.WorkingPoints.Find(wpID);
         var sd2 = wp2.Users.FirstOrDefault().Company.SubscriptionDetail;
         //update the subscriptions details twice
         mEFinter.UpdateSMSForSubscription(price, sd1, context);
         mEFinter.UpdateSMSForSubscription(price, sd2, altContext);
         //get final RemainingSMS (RS)
         var finalRS = sd2.RemainingSMS;
         var expectedRS = initialRS - 2;
         Assert.AreEqual(expectedRS, finalRS, "After the dust has settled the RemainingSMS should be decreased by 2");
         altContext.Dispose();
      }

      [Test]
      public void UpdateSMSForSubscription_2ContextsChangeRemainingSMSisJust1_RemainingSMSAndSpentAmountAreCorrectlyUpdated()
      {
         var wpID = "19802294808";//Wallmart only sms
         var price = "0.1";
         var initialRS = 1;
         var wp = context.WorkingPoints.Find(wpID);
         var sd1 = wp.Users.FirstOrDefault().Company.SubscriptionDetail;
         //make sure that RemainingSMS is exactly 1 and that the spent amount has a known value
        
         var initialSA = 1.34m;      
         bool saved = false;
         do
         {
            saved = true;
            try
            {
               sd1.RemainingSMS = initialRS;
               sd1.SpentThisMonth = initialSA;
               context.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {
               saved = false;
               ex.Entries.Single().Reload();
            }
         } while (!saved);    
         context.Entry(sd1).Reload();

         //initialize second context
         smsfeedbackEntities altContext = new smsfeedbackEntities();
         var wp2 = altContext.WorkingPoints.Find(wpID);
         var sd2 = wp2.Users.FirstOrDefault().Company.SubscriptionDetail;
         //update the subscriptions details twice
         mEFinter.UpdateSMSForSubscription(price, sd1, context);
         mEFinter.UpdateSMSForSubscription(price, sd2, altContext);
         //get final RemainingSMS (RS)         
         var finalRS = sd2.RemainingSMS;
         var expectedRS = 0;
         Assert.AreEqual(expectedRS, finalRS, "After the dust has settled the RemainingSMS should be 0");
         var finalSA = sd2.SpentThisMonth;
         var expectedSA = initialSA + Decimal.Parse(price,CultureInfo.InvariantCulture );
         Assert.AreEqual(expectedSA, finalSA, "The spent amount should have been increased with the price of 1 SMS");
         altContext.Dispose();
      }

      
    } 
}
