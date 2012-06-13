using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_EFModels;


namespace SmsFeedback_Take4.Models.ViewModels
{
   public class MessagesContext
   {
      private smsfeedbackEntities mContext = new smsfeedbackEntities();
      private static MessagesContext mInstance = null;
      private MessagesContext() { }
      public static MessagesContext getInstance()
      {
         if (mInstance == null)
         {
            mInstance = new MessagesContext();
         }
         return mInstance;
      }
      //private List<Message> myMessages;
      //private List<Message> mMyConversations;

      public int SelectedConversationID { get; set; }

      public IQueryable getMessagesForConversation(string conversationId)
      {
         //get the internal conversation id
         var convIds = from c in mContext.Conversations where c.ConvId == conversationId select c.Id;
         var convId = convIds.First();         
         var records = from c in mContext.Messages where c.ConversationId == convId select
                        new { Id = c.Id, ConvID = conversationId, From = c.From, To = c.To, Text = c.Text, TimeReceived = c.TimeReceived , Read = c.Read  };
         return records;      
         //switch (conversationId)
         //{
         //   case "0751569435-0745103618":
         //   return new List<Message>() 
         //      { 
         //         new Message("0751569435","0745103618","Buna mai aveti clatite",DateTime.Now.Subtract(new TimeSpan(3,2,9,2)),11, "from" ),
         //         new Message("0745103618","0751569435", "Da, imediat aducem - sunt proaspat scoase din cuptor", DateTime.Now.Subtract(new TimeSpan(2,2,9,2)),12, "to"),
         //         new Message("0751569435","0745103618", "Multumesc, imi plac mult clatitele dumneavoastra cu macese", DateTime.Now.Subtract(new TimeSpan(1,1,9,2)),13,"from"),
         //         new Message("0745103618","0751569435", "Cu placere! Sa nu uitati sa incercati si noile clatite cu dulceata de nuci - sunt o nebunie :)", DateTime.Now.Subtract(new TimeSpan(1,9,2)),14,"to")
         //      };
         //   case "0754654213-0745103618":
         //   return new List<Message>() 
         //      { 
         //         new Message("0754654213","0745103618","Servus Cluj, mai sunt numere?",DateTime.Now,11,"from" ),
         //         new Message("0745103618","0754654213", "Dada, cate mai doriti?", DateTime.Now.Subtract(new TimeSpan(2,9,2)),12, "to"),
         //         new Message("0754654213","0745103618", "4", DateTime.Now.Subtract(new TimeSpan(1,9,2)),13, "from")                  
         //      };
         //   default:
         //   return new List<Message>() 
         //      { 
         //         new Message("0754654213","0745103618","Bogus",DateTime.Now,11, "from" ),
         //         new Message("0745103618","0754654213", "Bogus1", DateTime.Now.Subtract(new TimeSpan(2,9,2)),12, "to"),
         //         new Message("0754654213","0745103618", "Bogus2", DateTime.Now.Subtract(new TimeSpan(1,9,2)),13, "from")                  
         //      };
               
         //}         
      }
      

       public IQueryable Conversations
      {
         get
         {
            var records = from c in mContext.Conversations select new { ConvID = c.ConvId, TimeUpdated=c.TimeUpdated, Read=c.Read, Text =c.Text, From = c.From };
            return records;
            //if (mMyConversations == null)
            //{
            //   mMyConversations = new List<Message>() 
            //   { 
            //      new Message("0751569435","0745103618","Buna",DateTime.Now.Subtract(new TimeSpan(4,2,9,2)),1, "from",false ),
            //      new Message("0754654213","0745103618", "Waz up?", DateTime.Now.Subtract(new TimeSpan(3,2,9,2)),2, "from",false),
            //      new Message("0754654232","0745103618", "Lorem ipsun dolor sit", DateTime.Now.Subtract(new TimeSpan(2,1,9,2)),3, "from"),
            //      new Message("0754651321","0745103618", "Acesta va fi un mesaj mai mare de 120 de caractere, asa ca trebuie sa scriu ceva cuvinte care sa faca sens, sau nu", DateTime.Now.Subtract(new TimeSpan(1,9,2)),4, "from")};
            //   for (int i = 0; i <= 9; i++)
            //   {
            //      Message msg = new Message("075156912" + i, "0745103618", "Buna", DateTime.Now.Subtract(new TimeSpan(4, 2, 9, 2)), 1, "from", true);
            //      mMyConversations.Add(msg);
            //   }
            //   mMyConversations.Reverse();
            //}
            //return mMyConversations;
         }
      }
   }
}