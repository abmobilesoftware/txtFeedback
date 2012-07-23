<%@ Page Title="SmsFeedback" Language="C#" MasterPageFile="~/Views/Shared/Site.Master"
   Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   <%: ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" ContentType="text/xml">
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/phonenumbers.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/messages.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/conversations.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/conversations_mb.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/filtersStrip.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/tags.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/jquery.tagsinput.css") %>" />

   <script src="<%: Url.UpdatedResourceLink("~/Scripts/spin.js") %>" type="application/javascript" ></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.cookie.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.simplemodal.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Utilities.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/WorkingPoints.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.tagsinput.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Messages.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/contact.js") %>" type="application/javascript"></script>
   
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Conversations.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Filtering.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/ConversationTags.js") %>" type="application/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/Strophe/strophe.js") %>" type="application/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/flxhr/flXHR.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/flxhr/strophe.flxhr.js") %>" type="text/javascript"></script>

   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/XMPP.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Facade.js") %>" type="application/javascript"></script>
   
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
         <img class="wpItem wpSelectorIcon deletePhoneNumberIconSelected" src="<%: Url.Content("~/Content/images/check-white.svg") %>"/>			
			<span class="wpItem" >{{ Name }}</span>						
		</span>
   </script>
   <script type="text/template" id="conversation-template">
           <div class="leftLiDiv convColumn">
                {% if (Read) { %}
                        <img src="<%: Url.Content("~/Content/images/check-grey.svg") %>" class="images conversationImageRead" />
                {% } else { %}
                        <img src="<%: Url.Content("~/Content/images/exclamation-green.svg") %>" class="images conversationImageUnread" />
                {% }  %}
            </div>
            <div class="rightLiDiv convColumn">    
                   
               <div class="spanClassFrom rightSideMembers">
                    <span>{% countryPrefix = From.substring(0,3); localPrefix = From.substring(3, 5); group1 = From.substring(5, 9); group2 = From.substring(9,13); %} {{ countryPrefix }} ({{ localPrefix }}) {{ group1 }} {{ group2 }} <span class='conversationArrows'> >> </span>  {{ To }} </span>
                </div>
               <div class='clear'></div>
                <div class="spanClassText rightSideMembers">
                    <span>{{ Text }}</span>
                </div>
                <div class="conversationStarIcon">
                    {% if (Starred) { %}
                            <img src="<%: Url.Content("~/Content/images/star-selected.svg") %>" class="conversationStarIconImg" />
                    {% } else { %}
                            <img src="<%: Url.Content("~/Content/images/star.svg") %>" class="conversationStarIconImg" /> 
                    {% } %}
                </div>
            </div>                         
        <div class="clear"></div>
   </script>
   <script type="text/template" id="message-template">
      <div class="textMessage">
         <span>{{ Text }} </span> 
         <div class="clear"></div>
         <span class="timeReceived">{{ TimeReceived }} </span>
      </div>
      <div class="clear"></div>
      <div class="extramenu" hoverID="{{ Id }}">
         <div class="innerExtraMenu">
            <div class="actionButtons">
               <img class="favConversation" src="<%: Url.Content("~/Content/images/star.svg") %>" />                
            </div>
            <div class="actionButtons sendEmailButton">
               <img src="<%: Url.Content("~/Content/images/mail.png") %>" />
            </div>
         <div class="clear"></div>                       
         </div>               
      </div>
        <div class="arrow">
         <div class="arrowInner"> </div>
       </div>
   </script>
   <script type="text/javascript">
      $(function () {
        var newGUI = new InitializeGUI();
      });
   </script>

   <div id="filtersStrip">

      <div class="grid_4_custom filterStripElement">
         <div id="dateFilterArea">
            <div id="dateLabel" class="filterLabel">
               <img id="includeDateInFilter" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
                  src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
               <span style="vertical-align: middle">
                  <%: Resources.Global.dateLabel %></span>
            </div>
            <div id="datePickersArea">
               <input type="dateTimePicker" id="startDateTimePicker" class="filterDate filterInputBox"
                  value="<%: Resources.Global.fromDate %>"> </input>
               <input type="dateTimePicker" id="endDateTimePicker" class="filterDate filterInputBox"
                  value="<%: Resources.Global.toDate %>"> </input>
            </div>
         </div>
         <div id="starredFilterArea" class="filterLabel">
            <img id="includeStarredInFilter" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
               src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
            <span style="vertical-align: middle">
               <%: Resources.Global.starredLabel %></span>
         </div>
         <div id="unreadFilterArea" class="filterLabel">
            <img id="includeUnreadInFilter" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
               src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
            <span style="vertical-align: middle">
               <%: Resources.Global.readLabel %></span>
         </div>
      </div>
      <div class="grid_6 filterStripElement tagFilterArea">
         <div id="tagsLabel" class="filterLabel">
            <img id="includeTagsInFilter" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
               src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
            <span style="vertical-align: middle">
               <%: Resources.Global.tagsLabel %></span>
         </div>
         <div id="tagFiltering" class="filterInputBox">
            <input name="filterTag" id="filterTag" />
         </div>        
      </div>
   </div>
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
      <div id="tagsContainer" class="tagArea invisible">
         <div id="tagsPool" class="tagsPhoneNumbers"></div>
         <input name="tags" id="tags" />
      </div>
      <div id="textareaContainer" class="invisible">
         <div id="replyFormArea">
            <form id="replyToMessageForm">
            <div id="inputTextContainer">
               <textarea id="limitedtextarea" onkeydown="limitText(this.form.limitedtextarea,this.form.countdown,160);"
                  onkeyup="limitText(this.form.limitedtextarea,this.form.countdown,160);" dir="ltr"></textarea>
               <br>
               <div class="clear"></div>
               <span><font size="0.5"><input readonly type="text" name="countdown" size="2" value="160"> </font>
               </span>
            </div>
            </form>
         </div>
         <div id="replyButtonArea">
            <button id="replyBtn"> <%: Resources.Global.sendButton %></button>
         </div>
      </div>
   </div>
</asp:Content>
