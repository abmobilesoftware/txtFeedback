<%@ Page Title="SmsFeedback" Language="C#" MasterPageFile="~/Views/Shared/Site.Master"
   Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   <%: Resources.Global.messagesPageTitle %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" ContentType="text/xml">
    <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/conversations_mb.css") %>" />

   <% if (Html.IsReleaseBuild())      { %>
  <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/Minified/phonenumbers.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/Minified/messages.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/Minified/conversations.css") %>" />   
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/Minified/filtersStrip.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/Minified/tags.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/Minified/jquery.tagsinput.css") %>" />

   <script src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/spin.js") %>" type="application/javascript" ></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/jquery.tagsinput.js") %>" type="application/javascript"></script>

   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-de.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-ro.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-en-GB.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-es.js") %>" type="application/javascript"></script>

   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Minified/WorkingPoints.js") %>" type="application/javascript"></script>   
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Minified/Messages.js") %>" type="application/javascript"></script>
   
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Minified/Conversations.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Minified/Filtering.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Minified/ConversationTags.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Minified/Facade.js") %>" type="application/javascript"></script>

   <% } else { %>

    <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/phonenumbers.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/messages.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/conversations.css") %>" />   
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/filtersStrip.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/tags.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/jquery.tagsinput.css") %>" />    
   

   <script src="<%: Url.UpdatedResourceLink("~/Scripts/spin.js") %>" type="application/javascript" ></script>
   
   
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.tagsinput.js") %>" type="application/javascript"></script>
   
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-de.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-ro.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-en-GB.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-es.js") %>" type="application/javascript"></script>

   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/WorkingPoints.js") %>" type="application/javascript"></script>   
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Messages.js") %>" type="application/javascript"></script>
   
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Conversations.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Filtering.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/ConversationTags.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Facade.js")%>" type="application/javascript"></script>         
   <% } %>
   
   
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
            <img tooltiptitle="<%: Resources.Global.tooltipWpImg %>" src="<%: Url.Content("~/Content/images/check-white.svg") %>" class="wpItem wpSelectorIcon deletePhoneNumberIconSelected" />		
			<span class="wpItem" >{{ Name }}</span>						
		</span>
   </script>
   <script type="text/template" id="conversation-template">
            <div class="leftLiDiv convColumn">
                {% if (Read) { %}
                        <embed src="<%: Url.Content("~/Content/images/check-grey.svg") %>" type="image/svg+xml" class="images conversationImageRead" />
                {% } else {
                        var fromTo = getFromToFromConversation(ConvID);
                        if (fromTo[0] == From) {
                        %}
                            <embed src="<%: Url.Content("~/Content/images/exclamation-blue.svg") %>" type="image/svg+xml" class="images conversationImageUnread" />
                        {% } else { %}
                            <embed src="<%: Url.Content("~/Content/images/exclamation-green.svg") %>" type="image/svg+xml" class="images conversationImageUnread" />
                        {% }    
                }  %}
            </div>
            <div class="rightLiDiv convColumn">    
                   
               <div class="spanClassFrom rightSideMembers">
                    <span class="conversationHeader">
                        {% 
                            var clientDisplayName = "defaultClient";
                            if (ClientIsSupportBot) {
                                clientDisplayName = ClientDisplayName;
                            } else {
                                // Currently if is not support, a number will be displayed
                                //var fromTo = getFromToFromConversation(ConvID); 
                                //var FromNumber = fromTo[0]; 
                                var FromNumber = ClientDisplayName;
                                var countryPrefix = FromNumber.substring(0,2);
                                var localPrefix = FromNumber.substring(2, FromNumber.length);
                                clientDisplayName = "(" + countryPrefix + ") " + localPrefix;
                            }                                          
                        %} 
                        
                        <span class="conversationFrom">
                           
                            {{ clientDisplayName }} 
                             {%
                             if (ClientIsSupportBot) {             
                            %}
                                <img class="conversationHeaderImg" src="<%: Url.Content("~/Content/images/Help-16.png") %>"/>
                            {%
                            }
                            %}
                       </span> 
                       <span class='conversationArrows'> >> </span>
                       <span class="conversationTo">{{ window.app.workingPoints[getFromToFromConversation(ConvID)[1]] }}</span> </span>
                </div>
               <div class='clear'></div>
                <div class="spanClassText rightSideMembers">
                    <span>{{ Text }}</span>
                </div>
                <div class="conversationStarIcon">
                    {% if (Starred) { %}
                            <img tooltiptitle="<%: Resources.Global.tooltipMarkAsFavouriteImg %>" src="<%: Url.Content("~/Content/images/star-selected_orange.svg") %>" class="conversationStarIconImg" />
                    {% } else { %}
                            <img tooltiptitle="<%: Resources.Global.tooltipMarkAsFavouriteImg %>" src="<%: Url.Content("~/Content/images/star.svg") %>" class="conversationStarIconImg" /> 
                    {% } %}
                </div>
            </div>                         
        <div class="clear"></div>
   </script>
   <script type="text/template" id="message-template">
      <div class="textMessage">
         <span>{{ Text }} </span> 
         <div class="clear"></div>
         {% 
            var dateComponents = TimeReceived.toString().split(" ");
            var time = dateComponents[4];               
            var displayPattern = 'DD, MM d, yy';
            if (window.app.calendarCulture == "ro") displayPattern = 'DD, d MM, yy';       
            var timeReceivedLocal = $.datepicker.formatDate(displayPattern, TimeReceived, 
                                                            {dayNamesShort: $.datepicker.regional[window.app.calendarCulture].dayNamesShort, dayNames: $.datepicker.regional[window.app.calendarCulture].dayNames,
                                                             monthNamesShort: $.datepicker.regional[window.app.calendarCulture].monthNamesShort, monthNames: $.datepicker.regional[window.app.calendarCulture].monthNames}); 
            var timeReceivedLocal = timeReceivedLocal + " " + time;
       %}
       <span class="timeReceived">{{ timeReceivedLocal }} </span>
      </div>
      
      <div class="clear"></div>
      <div class="extramenu" hoverID="{{ Id }}">
       <div class="extraMenuWrapper"></div>  
       <div class="innerExtraMenu">
            
            <div class="actionButtons sendEmailButton">
               <img tooltiptitle="<%: Resources.Global.tooltipSendEmailImg %>" src="<%: Url.Content("~/Content/images/mail.png") %>" />
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
   
   <div id="filtersStrip" class="headerArea">
       <div class="grid_4_custom filterStripElement">
            <div id="supportFilterArea" class="filterLabel">
                <img id="includeSupportInFilter" tooltiptitle="<%: Resources.Global.tooltipIncludeSupportFilter %>" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
                   src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
                <span style="vertical-align: middle">
                   <%: Resources.Global.supportLabel %></span>
           </div>
           <div id="unreadFilterArea" class="filterLabel">
                <img id="includeUnreadInFilter" tooltiptitle="<%: Resources.Global.tooltipIncludeUnreadInFilter %>" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
                   src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
                <span style="vertical-align: middle">
                   <%: Resources.Global.readLabel %></span>
           </div>
           <div id="starredFilterArea" class="filterLabel">
                <img id="includeStarredInFilter" tooltiptitle="<%: Resources.Global.tooltipIncludeStarredInFilter %>" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
                   src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
                <span style="vertical-align: middle">
                   <%: Resources.Global.starredLabel %></span>
           </div>
           <!--
           <div id="dateFilterArea">
            <div id="dateLabel" class="filterLabel">
               <img tooltiptitle="<%: Resources.Global.tooltipIncludeDateInFilter %>" id="includeDateInFilter" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
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
           -->
         
         
         
      </div>
      <div class="grid_6 filterStripElement tagFilterArea">
         <div id="tagsLabel" class="filterLabel">
            <img id="includeTagsInFilter" tooltiptitle="<%: Resources.Global.tooltipIncludeTagsInFilter %>" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
               src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
            <span style="vertical-align: middle">
               <%: Resources.Global.tagsLabel %></span>
         </div>
         <div id="tagFiltering" class="filterInputBox">
            <input name="filterTag" id="filterTag" />
            <input type="hidden" value="<%: Resources.Global.addATagLabel %>" class="filterLabel"/>
         </div>        
      </div>
   </div>
   <div class="clear"></div>
   <div id="phoneNumbersPool" class="wordwrap tagsPhoneNumbers grid_2 leftSideArea">
   </div>
   <div id="conversationsArea" class="grid_4">
      <div id="scrollableconversations" class="conversationbox scrollablebox">
         <div id="conversations" class="conversationbox">
         </div>
         <div id="loadMoreConversations" class="readable">
            <%: Resources.Global.loadMoreConversations %>
         </div>
      </div>
   </div>
   <div id="messagesArea" class="grid_6">
      <div id="scrollablemessagebox" class="messagesboxcontainerclass scrollablebox">
         <div id="messagesbox" class="messagesboxclass">
            <span id="noConversationsLoaded"><%: Resources.Global.lblNoConversationSelected%></span>
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
               &nbsp;</span></div>
            </form>
         </div>
         <div id="replyButtonArea">
            <button tooltiptitle="<%: Resources.Global.tooltipReplyBtn %>" id="replyBtn"> <%: Resources.Global.sendButton %></button>
         </div>
         <input type="hidden" value="<%: ViewData["currentCulture"] %>" class="currentCulture" />
         <input type="hidden" value="<%: Resources.Global.lblNoConversationSelected %>" id="noConversationSelectedMessage" />
         <input type="hidden" value="<%: Resources.Global.messagesAddTagPlaceHolder %>" id="messagesAddTagPlaceHolderMessage" />
         <input type="hidden" value="<%: Resources.Global.filteringAddFilterTag %>" id="filteringAddFilterTagMessage" />
      </div>
   </div>
</asp:Content>
