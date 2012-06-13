using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SmsFeedback_EFModels;


namespace SmsFeedback_Take4.Services
{
   // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IConversations" in both code and config file together.
   [ServiceContract(Name="TxtConversationsService")]   
   public interface IConversations
   {
      [OperationContract]
      [WebInvoke(Method = "POST",
               BodyStyle = WebMessageBodyStyle.Wrapped,
               ResponseFormat = WebMessageFormat.Json
    )]
      System.Linq.IQueryable GetConversationsList(bool showAll, bool showTagged, string[] tags);

      [OperationContract]
      [WebInvoke(Method = "POST",
               BodyStyle = WebMessageBodyStyle.Wrapped,
               ResponseFormat = WebMessageFormat.Json
    )]
      string DoWork();
   }
}
