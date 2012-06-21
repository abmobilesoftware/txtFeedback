﻿<%@ Page Title="SmsFeedback" Language="C#" MasterPageFile="~/Views/Shared/Site.Master"
   Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   <%: ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/phonenumbers.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/messages.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/conversations.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/filtersStrip.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/tags.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/jquery.tagsinput.css") %>" />
   <script src="<%: Url.Content("~/Scripts/spin.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/jquery.cookie.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/splitter.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/jquery.simplemodal.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/MyScripts/Utilities.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/MyScripts/WorkingPoints.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/jquery.tagsinput.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/MyScripts/Messages.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/MyScripts/contact.js") %>" type="text/javascript"></script>
   
   <script src="<%: Url.Content("~/MyScripts/Conversations.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/MyScripts/Tags.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/Strophe/strophe.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/flxhr/flXHR.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/flxhr/strophe.flxhr.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/MyScripts/XMPP.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/MyScripts/Facade.js") %>" type="text/javascript"></script>
   
     <script type="text/template" id="tag-template">       
		<span class="tag"  >
			<span>{{ Name }}</span>
			<span class="removeTag">
				<img class="removeTagIcon" src="<%: Url.Content("~/Content/images/close14x14.png") %>"/>
			</span>
		</span>
   </script>

   <script type="text/template" id="phoneNumber-template">       
		<span >
			<span>{{ Name }}</span>
			<span class="deletePhoneNumber">
				<img class="deletePhoneNumberIconSelected" src="<%: Url.Content("~/Content/images/checkbox_checked.png") %>"/>
			</span>
		</span>
   </script>

   <script type="text/template" id="conversation-template">
           <div class="leftLiDiv">
                <img src="<%: Url.Content("~/Content/images/delete.ico") %>" class="images" />
            </div>
            <div class="rightLiDiv">                
                <div class="spanClassFrom">
                    <span>{{ From }} for " {{ To }} "</span>
                </div>
                <div class="spanClassText">
                    <span>{{ Text }}</span>
                </div>
            </div>        
            <div class="exclamationcontainer">
                <img class="images" src="<%: Url.Content("~/Content/images/exclamationMark.png") %>"/>
            </div>            
        <div class="clear"></div>
   </script>

   <script type="text/template" id="message-template">
        <div class="textMessage">
           <span>{{ Text }} </span> 
            <div class="clear"></div>
           <span class="timeReceived">{{ TimeReceived }} </span>
        </div>
        <%--<div class="clear"></div>--%>
        <div class="extramenu" hoverID="{{ Id }}">
            <div class="actionButtons">
                <button id="markAsImportant" >Star</button>
            </div>
            <div class="actionButtons sendEmailButton">
                <button id="sendEmail" >Email</button>                 
            </div>
      <div class="clear"></div>
            <div class="actionButtons">
                <button id="copyText" >Copy</button>               
            </div>
            <div class="clear"></div>
         </div>
         <div class="arrow">
         <div class="arrowInner"> </div>
       </div>
   </script>
   <script type="text/javascript">
      $(function () {
         InitializeGUI();
      });
   </script>  
   <div id="filtersStrip"></div>     
   <div class="clear"></div>
   <div id="phoneNumbersPool" class="wordwrap tagsPhoneNumbers grid_2">
   </div>
      <div id="conversationsArea" class="grid_4">
         <div id="scrollableconversations" class="conversationbox scrollablebox">
            <div id="conversations" class="conversationbox">
            </div>
             <div id="loadMoreConversations" class="readable">
             Load More Conversations
                 </div>
         </div>
      </div>
      
      <div id="messagesArea" class="grid_6">        
        
         <div id="scrollablemessagebox" class="messagesboxcontainerclass scrollablebox">
            <div id="messagesbox" class="messagesboxclass">
               <span>No conversation selected, please select one</span>
            </div>
           
         </div>
         <div id="messageTagsSeparator"></div>       
         <div id="tagsContainer">
            <div id="tagsPool" class="tagsPhoneNumbers">
              <%-- <span>Here will be the tags</span>--%>
            </div>
           <input name="tags" id="tags" />
          </div>   
         <div id="textareaContainer" class="invisible">          
            <form id="replyToMessageForm">
               <div id="inputTextContainer">
               <textarea id="limitedtextarea" onkeydown="limitText(this.form.limitedtextarea,this.form.countdown,160);"
                  onkeyup="limitText(this.form.limitedtextarea,this.form.countdown,160);" dir="ltr"></textarea>
               <br>
                  <div class="clear"></div>
               <span>
                  <font size="1"><input readonly type="text" name="countdown" size="3" value="160"> </font>
               </span>               
               </div>
            </form>          
            <button id="replyBtn">Reply</button>
         </div>
      </div>    
   
  <%-- <div class="clear"></div>--%>
</asp:Content>
