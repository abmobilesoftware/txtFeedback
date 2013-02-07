using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmsFeedback_EFModels;
namespace TxtFeedback_tests.EF
{
   [TestFixture]
   class SubscriptionDetailTests
   {
      SubscriptionDetail sd;
      [TestFixtureSetUp]
      public void Initialize()
      {
         sd = new SubscriptionDetail();         
      }

      [Test]
      public void CanSendSMS_existingSMS_returnsTrue() {
         sd.RemainingSMS = 20;
         Assert.IsTrue(sd.CanSendSMS);
      }
      [Test]
      public void CanSendSMS_noMoreSMS_creditAdded_returnsTrue() {
         sd.ExtraAddedCreditThisMonth = 1;
         sd.RemainingSMS = 0;
         sd.RemainingCreditFromPreviousMonth = 0;
         sd.SpentThisMonth = 0;
         sd.SpendingLimit = 0;
         Assert.IsTrue(sd.CanSendSMS);
      }
      [Test]
      public void CanSendSMS_noMoreSMS_creditFromPreviousMonth_returnsTrue()
      {
         sd.ExtraAddedCreditThisMonth = 0;
         sd.RemainingSMS = 0;
         sd.RemainingCreditFromPreviousMonth = 1;
         sd.SpentThisMonth = 0;
         sd.SpendingLimit = 0;
         Assert.IsTrue(sd.CanSendSMS);
      }
      [Test]
      public void CanSendSMS_noMoreSMS_spentMoreAllPossible_returnsFalse()
      {
         sd.RemainingSMS = 0;
         sd.RemainingCreditFromPreviousMonth = 2;
         sd.ExtraAddedCreditThisMonth = 1;
         sd.SpendingLimit = 1;
         sd.SpentThisMonth = 4;
         Assert.IsFalse(sd.CanSendSMS);
      }
      [Test]
      public void CanSendSMS_creditInsufficient_returnsFalse()
      {
         //creditInsufficient = we still have "very little credit"
         sd.RemainingSMS = 0;
         sd.RemainingCreditFromPreviousMonth = 2;
         sd.ExtraAddedCreditThisMonth = 1;
         sd.SpendingLimit = 1;
         sd.SpentThisMonth = 3.9451m;
         Assert.IsFalse(sd.CanSendSMS);
      }
      [Test]
      public void CanSendSMS_creditJustEnoughfor1SMS_returnsTrue()
      {
         //creditInsufficient = we still have "very little credit"
         sd.RemainingSMS = 0;
         sd.RemainingCreditFromPreviousMonth = 2;
         sd.ExtraAddedCreditThisMonth = 1;
         sd.SpendingLimit = 1;
         sd.SpentThisMonth = 3.9449m;
         Assert.IsTrue(sd.CanSendSMS);
      }

      #region WarningsRequired
      [Test]
      public void WarningsRequired_StillHaveRemainingSMS_returnsFalse()
      {         
         var sd = new SubscriptionDetail();
         sd.RemainingSMS = 100;
         bool result = sd.WarningsRequired();
         Assert.IsFalse(result,"If we still have subscriptionSMS then no warnings are required");         
      }
      [Test]
      public void WarningsRequired_NoRemainingSMS_stillHaveCreditFromLastMonth_returnsFalse()
      {
         var sd = new SubscriptionDetail();
         sd.RemainingSMS = 0;
         sd.ExtraAddedCreditThisMonth = 0;
         sd.RemainingCreditFromPreviousMonth = 0.1m;
         bool result = sd.WarningsRequired();
         Assert.IsFalse(result, "If we still have Credit no warnings are required");         
      }
      [Test]
      public void WarningsRequired_NoRemainingSMS_creditAddedThisMonth_returnsFalse()
      {
         var sd = new SubscriptionDetail();
         sd.RemainingSMS = 0;
         sd.ExtraAddedCreditThisMonth = 0.1m;
         sd.RemainingCreditFromPreviousMonth = 0;
         bool result = sd.WarningsRequired();
         Assert.IsFalse(result, "If we still have Credit no warnings are required");
      }
      [Test]
      public void WarningsRequired_NoRemainingSMS_noCredit_belowWarningLimit_returnsFalse()
      {
         var sd = new SubscriptionDetail();
         sd.RemainingSMS = 0;
         sd.ExtraAddedCreditThisMonth = 0;
         sd.RemainingCreditFromPreviousMonth = 0;
         sd.SpentThisMonth = 1;
         sd.WarningLimit = 1.3m;
         sd.SpendingLimit = 1.5m;
         bool result = sd.WarningsRequired();
         Assert.IsFalse(result, "No Warnings required if below warning limit");
      }
      [Test]
      public void WarningsRequired_NotEnoughToSendAnotherSMS_returnsTrue()
      {
         var sd = new SubscriptionDetail();
         sd.RemainingSMS = 0;
         sd.ExtraAddedCreditThisMonth = 0;
         sd.RemainingCreditFromPreviousMonth = 0;
         sd.SpentThisMonth = 1;
         sd.WarningLimit = 1.1m;
         bool result = sd.WarningsRequired();
         Assert.IsTrue(result, "If not enough to send another SMS then warnings are required");
      }
      [Test]
      public void WarningsRequired_NoRemainingSMS_noCredit_aboveWarningLimit_returnsTrue()
      {
         var sd = new SubscriptionDetail();
         sd.RemainingSMS = 0;
         sd.ExtraAddedCreditThisMonth = 0;
         sd.RemainingCreditFromPreviousMonth = 0;
         sd.SpentThisMonth = 1;
         sd.WarningLimit = 0.9m;
         bool result = sd.WarningsRequired();
         Assert.IsTrue(result, "Warnings required if above warning limit");
      }
      [Test]
      public void WarningsRequired_NoRemainingSMS_noCredit_atWarningLimit_returnsTrue()
      {
         var sd = new SubscriptionDetail();
         sd.RemainingSMS = 0;
         sd.ExtraAddedCreditThisMonth = 0;
         sd.RemainingCreditFromPreviousMonth = 0;
         sd.SpentThisMonth = 1;
         sd.WarningLimit = 1;
         bool result = sd.WarningsRequired();
         Assert.IsTrue(result, "Warnings required if at warning limit");
      }
      [Test]
      public void WarningsRequired_NoRemainingSMS_noCredit_atWarningLimitAndWarningLimit0_returnsTrue()
      {
         var sd = new SubscriptionDetail();
         sd.RemainingSMS = 0;
         sd.ExtraAddedCreditThisMonth = 0;
         sd.RemainingCreditFromPreviousMonth = 0;
         sd.SpentThisMonth = 0;
         sd.WarningLimit = 0;
         bool result = sd.WarningsRequired();
         Assert.IsTrue(result, "Warnings required if at warning limit");
      }
      #endregion
      #region GetNextBillingDate
      [Test]
      public void GetNextBillingDate_ComparisonAfterBillingDay_DateIsNextMonth()
      {
         var compTime = new DateTime(2012,01,15);
         var billingDay = 14;
         var sd = new SubscriptionDetail() { BillingDay = billingDay };
         var nextBillingDate = sd.GetNextBillingDate(compTime);
         var expectedBillingDate = new DateTime(2012, 02, billingDay);
         Assert.AreEqual(expectedBillingDate.Date, nextBillingDate.Date);
      }
      [Test]
      public void GetNextBillingDate_ComparisonIsBeforeBillingDay_DateIsThisMonth()
      {
         var compTime = new DateTime(2012, 01, 13);
         var billingDay = 14;
         var sd = new SubscriptionDetail() { BillingDay = billingDay };
         var nextBillingDate = sd.GetNextBillingDate(compTime);
         var expectedBillingDate = new DateTime(2012, 01, billingDay);
         Assert.AreEqual(expectedBillingDate.Date, nextBillingDate.Date);
      }
      [Test]
      public void GetNextBillingDate_ComparisonOnBillingDay_AND_NoAutoInvoice_DateIsOnComparisonDay()
      {
         var compTime = new DateTime(2012, 01, 15);
         var billingDay = 15;
         var sd = new SubscriptionDetail() { BillingDay = billingDay };
         sd.Companies.Add(new Company() {Name= "Random"});
         var nextBillingDate = sd.GetNextBillingDate(compTime);
         var expectedBillingDate = compTime;
         Assert.AreEqual(expectedBillingDate.Date, nextBillingDate.Date);
      }
      [Test]
      public void GetNextBillingDate_ComparisonOnBillingDay_AND_AutoInvoiceOnThisDay_DateIsNextMonth()
      {
         var compTime = new DateTime(2012, 01, 15);
         var billingDay = 15;
         var sd = new SubscriptionDetail() { BillingDay = billingDay };
         Company newCompany = new Company() { Name = "Random" };
         newCompany.Invoices.Add(new Invoice() {DateCreated = compTime, AutoGenerated=true });
         newCompany.Invoices.Add(new Invoice() { DateCreated = compTime.Subtract(new TimeSpan(35,0,0,0)), AutoGenerated = true });
         sd.Companies.Add(newCompany);

         var nextBillingDate = sd.GetNextBillingDate(compTime);
         var expectedBillingDate = new DateTime(2012,02,billingDay);
         Assert.AreEqual(expectedBillingDate.Date, nextBillingDate.Date);
      }
      [Test]
      public void GetNextBillingDate_ComparisonOnBillingDay_AND_AutoInvoiceBeforeThisDay_DateIsOnComparisonDay()
      {
         var compTime = new DateTime(2012, 01, 15);
         var billingDay = 15;
         var sd = new SubscriptionDetail() { BillingDay = billingDay };
         Company newCompany = new Company() { Name = "Random" };         
         newCompany.Invoices.Add(new Invoice() { DateCreated = compTime.Subtract(new TimeSpan(35, 0, 0, 0)), AutoGenerated = true });
         sd.Companies.Add(newCompany);

         var nextBillingDate = sd.GetNextBillingDate(compTime);
         var expectedBillingDate = compTime;
         Assert.AreEqual(expectedBillingDate.Date, nextBillingDate.Date);
      }
      [Test]
      public void GetNextBillingDate_ComparisonAfterBillingDay_AND_ComparisonDayIsInDecember_DateIsNextYear()
      {
         var compTime = new DateTime(2012, 12, 17);
         var billingDay = 15;
         var sd = new SubscriptionDetail() { BillingDay = billingDay };
         Company newCompany = new Company() { Name = "Random" };
         newCompany.Invoices.Add(new Invoice() { DateCreated = compTime.Subtract(new TimeSpan(35, 0, 0, 0)), AutoGenerated = true });
         sd.Companies.Add(newCompany);

         var nextBillingDate = sd.GetNextBillingDate(compTime);
         var expectedBillingDate = new DateTime(2013,01,billingDay);
         Assert.AreEqual(expectedBillingDate.Date, nextBillingDate.Date);
      }
      #endregion
   }
}
