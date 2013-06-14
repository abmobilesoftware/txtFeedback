<%@ Page Title="SmsFeedback" Language="C#" MasterPageFile="~/Views/Shared/Site.Master"
   Inherits="System.Web.Mvc.ViewPage<dynamic>"  %>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   <%: Resources.Global.messagesPageTitle %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="IncludesCssArea" runat="server">
   <%: Styles.Render("~/Content/homeCss") %>      
</asp:Content>
<asp:Content ID="Content15" ContentPlaceHolderID="IncludesJsArea" runat="server">   
   <%:Scripts.Render("~/bundles/homejs")%> 
   <script type="text/javascript">
      $(function () {
         var newGUI = new InitializeGUI();
      });
   </script>
</asp:Content>    
<asp:Content ID="Content3" ContentPlaceHolderID="TemplatesArea" runat="server">
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
            <img title="<%: Resources.Global.tooltipWpImg %>" src="<%: Url.Content("~/Content/images/check-white.svg") %>" class="wpItem wpSelectorIcon deletePhoneNumberIconSelected" width="17px" height="17px"/>		
			<span class="wpItem" >{{ Name }}</span>						
		</span>
   </script>
   <script type="text/template" id="conversation-template">
            <div class="conversation {% if (Read) { %} readConversation {% }else{ %} unreadConversation {% } %}
                        {% if (ClientIsSupportBot) { %} supportConversation {% }else{ %} normalConversation {% } %}">
                {% if (IsSmsBased) { %}
               <img class="isSms" src="<%: Url.Content("~/Content/images/sms_white.png") %>"/>
                 {% } %}

            <div class="leftLiDiv convColumn noShowOnTablet">
                    {% if (Read) { %}
                            <img src="<%: Url.Content("~/Content/images/check-grey.png") %>" class="images conversationImageRead" height="28" width="40" />
                    {% } else {
                            var fromTo = getFromToFromConversation(ConvID);
                            if (comparePhoneNumbers(fromTo[0],From)) {
                            %}
                                <img src="<%: Url.Content("~/Content/images/exclamation-blue.png") %>" class="images conversationImageUnread" height="28" width="40" />
                            {% } else { %}
                                <img src="<%: Url.Content("~/Content/images/exclamation-green.png") %>" class="images conversationImageUnread" height="28" width="40" />
                            {% }    
                    }  %}
                </div>
                <div class="rightLiDiv convColumn">    
                   
                   <div class="spanClassFrom rightSideMembers">                    
                            {% 
                                var clientDisplayName = cleanupPhoneNumber(ClientDisplayName);                                                                      
                            %} 
                            {%
                                 if (ClientIsSupportBot) {             
                                %}
                                    <img class="conversationHeaderImg" src="<%: Url.Content("~/Content/images/Help-16.png") %>"/>
                                {%
                                }
                             %}                         
                            <span class="conversationFrom" title="{{ clientDisplayName }}" >{{ clientDisplayName }} </span> 
                           <span class='conversationArrows'> >> </span>
                           <span class="conversationTo">{{ window.app.workingPointsNameDictionary[getFromToFromConversation(ConvID)[1]] }}</span>      
                    </div>
                   <div class='clear'></div>
                    <div class="spanClassText rightSideMembers">
                        <span>{{ Text }}</span>
                    </div>
                    <% if ((bool)ViewData["messageOrganizer"] )
                       { %>
                          {%
                                 if (!ClientIsSupportBot) {             
                           %}
                              <div class="deleteConv ignoreElementOnSelection">
                                 <img title="<%: Resources.Global.tooltipDeleteConversation %>" src="<%: Url.Content("~/Content/images/trash_white.png") %>" class="deleteConvImg" />
                              </div>
                            {%
                                }
                             %}     
                        
                    <% } %>
       
                    <div class="favoriteConversation">
                        <img class="star starred {% if (!Starred) { %} hide {% } %}" title="<%: Resources.Global.tooltipMarkAsFavouriteImg %>" src="<%: Url.Content("~/Content/images/star-selected_orange.png") %>" height="32" width="36"/>
                        <img class="star unstarred {% if (Starred) { %} hide {% } %}" title="<%: Resources.Global.tooltipMarkAsFavouriteImg %>" src="<%: Url.Content("~/Content/images/star.png") %>" height="32" width="36"/> 
                    </div>
                </div>                         
                <div class="clear"></div>
            </div>
   </script>
   <script type="text/template" id="message-template">
      <div class="textMessage">
         <span class="textMessageContent">{{ Text }} </span> 
         <div class="clear"></div>
         {% 
            var dateComponents = TimeReceived.toTimeString().split(" ");
            var time = dateComponents[0];               
            var displayPattern = 'DD, MM d, yy';
            if (window.app.calendarCulture == "ro") displayPattern = 'DD, d MM, yy';       
            var timeReceivedLocal = $.datepicker.formatDate(displayPattern, TimeReceived, 
                                                            {dayNamesShort: $.datepicker.regional[window.app.calendarCulture].dayNamesShort, dayNames: $.datepicker.regional[window.app.calendarCulture].dayNames,
                                                             monthNamesShort: $.datepicker.regional[window.app.calendarCulture].monthNamesShort, monthNames: $.datepicker.regional[window.app.calendarCulture].monthNames}); 
            var timeReceivedLocal = timeReceivedLocal + " " + time;
       %}
            
            {% if (Direction == "to") { %}
                <div class="sendAndReceivedChecks">
                    {% if (ClientAcknowledge) { %}
                        <img title="<%: Resources.Global.tooltipCheckMessageReceived %>" src="/Content/images/doubleCheck.png" class="check checkNo{{ Id }}" />
                     {%  } else if (WasSuccessfullySent) { %}             
                        <img title="<%: Resources.Global.tooltipCheckMessageSent %>" src="/Content/images/singleCheck.png" class="check checkNo{{ Id }}" />            
                     {% } %}  
                </div>    
            {% } %}
            <span class="timeReceived">{{ timeReceivedLocal }} </span>  
           
       {%
            var encodedImgUrl = encodeURIComponent("http://txtfeedback.net/wp-content/uploads/2012/07/txtfeedback_logo_small.png");
            var encodedUrl = encodeURIComponent("http://localhost:4631/ro-RO");
            var encodedTxtUrl = encodeURIComponent("http://www.txtfeedback.net");
            var encodedTextWithQuotationMarks = encodeURIComponent("\"" + Text + "\"");
            var encodedText = encodeURIComponent(Text);
            var encodedTitle = encodeURIComponent($("#linkedInTitle").val());
       %}
            <div class="messageMenu">           
                <div class="msgButtons sendEmailButton">
                    <img title="<%: Resources.Global.tooltipSendEmailImg %>" src="<%: Url.Content("~/Content/images/em16x16.png") %>" />
                </div>
                <div class="msgButtons">
                   <a class="shareBtn" title="<%: Resources.Global.shareOnFacebook %>" href="http://www.facebook.com/sharer/sharer.php?s=100&p[title]={{ encodedTitle }}&p[url]={{ encodedTxtUrl }}&p[images][0]={{ encodedImgUrl }}&p[summary]={{ encodedText }}" target="_blank"><img src="<%: Url.Content("~/Content/images/fb16x16.png") %>" /></a>
                </div>
                <div class="msgButtons">
                   <a class="shareBtn" title="<%: Resources.Global.shareOnTwitter %>" href="http://twitter.com/share?text={{ encodedTextWithQuotationMarks }}&url={{ encodedUrl }}&via=txtfeedback" target="_blank"><img src="<%: Url.Content("~/Content/images/tw16x16.png") %>" /></a>
                </div>
                <div class="msgButtons">
                   <a class="shareBtn" title="<%: Resources.Global.shareOnLinkedin %>" href="http://www.linkedin.com/shareArticle?mini=true&url={{ encodedUrl }}&title={{ encodedTitle }}&summary={{ encodedText }}" target="_blank"><img src="<%: Url.Content("~/Content/images/in16x16.png") %>" /></a>
                </div> 
               <% if ((bool)ViewData["messageOrganizer"]) { %>
                   <div class="msgButtons deleteMessage"> 
                         <img title="<%: Resources.Global.deleteMessage %>" src="<%: Url.Content("~/Content/images/trash20x20.png") %>" />    
                   </div>     
               <% } %>
            </div>
            
      </div>
      
      <div class="clear"></div>
      </div>
   </script>

    <script type="text/template" id="voucher">
        <a href="#">
            <div class="voucherItem">
                {{ code }} - {{ description }}
            </div>
        </a>
    </script>
    <script type="text/template" id="vouchersPanel">
        <div id="panelTitle">
            <span><%: Resources.Global.messageListOfVouchers %></span>
            <a class="panelClose" href="#"><img src="<%: Url.Content("~/Content/images/arrow_down_16.png") %>" /></a>
        </div>
        <div id="panelContent">

        </div>
    </script>
    <script type="text/template" id="button">
        <div id="vPanel"></div>
        <a class="button" href="#">
            <div class="buttonContent">
                <span class="buttonTitle">{{ Title }}</span>
                <img class="loader displayNone" src="<%: Url.Content("~/Content/images/ajax-loader.gif") %>" />      
            </div>
        </a>
        
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FilterArea" runat="server">
    <div class="grid_4_custom filterStripElement">
            <div id="supportFilterArea" class="filterLabel">
                <img id="includeSupportInFilter" title="<%: Resources.Global.tooltipIncludeSupportFilter %>" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
                   src="<%: Url.Content("~/Content/images/transparent.gif") %>" width="17px" height="17px"/>
                <span style="vertical-align: middle">
                   <%: Resources.Global.supportLabel %></span>
           </div>
         
           <div id="starredFilterArea" class="filterLabel">
                <img id="includeStarredInFilter" title="<%: Resources.Global.tooltipIncludeStarredInFilter %>" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
                   src="<%: Url.Content("~/Content/images/transparent.gif") %>" width="17px" height="17px"/>
                <span style="vertical-align: middle">
                   <%: Resources.Global.starredLabel %></span>
           </div>
            <div id="unreadFilterArea" class="filterLabel">
                <img id="includeUnreadInFilter" title="<%: Resources.Global.tooltipIncludeUnreadInFilter %>" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
                   src="<%: Url.Content("~/Content/images/transparent.gif") %>" width="17px" height="17px"/>
                <span style="vertical-align: middle">
                   <%: Resources.Global.readLabel %></span>
           </div>           
      </div>
      <div class="grid_6 filterStripElement tagFilterArea">
         <div id="tagsLabel" class="filterLabel">
            <img id="includeTagsInFilter" title="<%: Resources.Global.tooltipIncludeTagsInFilter %>" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
               src="<%: Url.Content("~/Content/images/transparent.gif") %>" width="17px" height="17px"/>
            <span style="vertical-align: middle">
               <%: Resources.Global.tagsLabel %></span>
         </div>
         <div id="tagFiltering" class="filterInputBox">
            <input name="filterTag" id="filterTag" />
            <input type="hidden" value="<%: Resources.Global.addATagLabel %>" class="filterLabel"/>
         </div>        
      </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server" ContentType="text/xml">      
   
   <div id="conversationsArea" class="grid_convs">
      <div id="scrollableconversations" class="conversationbox scrollablebox">
         <div id="conversations" class="conversationbox">
         </div>
         <div id="loadMoreConversations" class="readable">
            <%: Resources.Global.loadMoreConversations %>
         </div>
      </div>
       <div id="convOverlay"></div> 
   </div>
   <div id="messagesArea" class="grid_messages">
      <input type="hidden" value="<%: Resources.Global.shareOnLinkedinTitle %>" id="linkedInTitle"/>
      <div id="scrollablemessagebox" class="messagesboxcontainerclass scrollablebox">
         <div id="messagesbox" class="messagesboxclass">
            <span id="noConversationsLoaded"><%: Resources.Global.lblNoConversationSelected%></span>
         </div>
      </div>
      <div id="messageTagsSeparator"></div>
      <div id="tagsContainer" class="invisible">
         <div id="tagsPoolWrapper" class="tagArea">
            <div id="tagsPool" class="tagsPhoneNumbers"></div>
            <input name="tags" id="tags" />         
         </div>
         <div id="specialTagsWrapper" class="triangle-isosceles left">
             <div id="specialTags">
             <a href="#" id='thumbsUp' title="<%: Resources.Global.thumbsUpTooltip %>" class='specialTag' tagType="positiveFeedback"></a>
             <a href="#" id='thumbsDown' title="<%: Resources.Global.thumbsDownTooltip %>" class='specialTag' tagType="negativeFeedback"></a>
            </div>
       </div>
        <div class="clear"></div>
      </div>
      
      <div id="quickActionBtns" class="hidden"></div>
      <div id="textareaContainer" class="invisible">
          <div class="voucherAlert">
              <%: Resources.Global.messageVoucherNotInserted %>
          </div>
          <div id="replyFormArea">
            <form id="replyToMessageForm">
            <div id="inputTextContainer">
               <textarea id="limitedtextarea" class="textarea160"
                   dir="ltr"></textarea>
               <br>
               <div class="clear"></div>
               <span><font size="0.5"><input readonly type="text" name="countdown" size="2" value="160"> </font>
               &nbsp;</span></div>
            </form>
         </div>
         <div id="replyButtonArea">
            <input type="hidden" value="<%: Resources.Global.errorCannotSendMessage %>" id="msgMessageNotSent"/>
            <button title="<%: Resources.Global.tooltipReplyBtn %>" id="replyBtn"> <%: Resources.Global.sendButton %></button>
         </div>
         <div class="clear"></div>
      </div>
   </div>
       
   <input type="hidden" value="<%: ViewData["currentCulture"] %>" class="currentCulture" />
   <input type="hidden" value="<%: Resources.Global.lblNoConversationSelected %>" id="noConversationSelectedMessage" />
   <input type="hidden" value="<%: Resources.Global.messagesAddTagPlaceHolder %>" id="messagesAddTagPlaceHolderMessage" />
   <input type="hidden" value="<%: Resources.Global.messagesRemoveTagPlaceHolder %>"
      id="messagesRemoveTagPlaceHolderMessage" />
   <input type="hidden" value="<%: Resources.Global.filteringAddFilterTag %>" id="filteringAddFilterTagMessage" />
   <input type="hidden" value="<%: Resources.Global.confirmDeleteMessage %>" id="confirmDeleteMessage" />
   <input type="hidden" value="<%: Resources.Global.confirmDeleteConversation %>" id="confirmDeleteConversation" />
   <input type="hidden" value="<%: Resources.Global.errorMessageNotSentReasonUnknown %>" id="messageNotSentReasonUnknown" />
   <input type="hidden" value="<%: Resources.Global.errorMessageNotSentInsufficientCredits %>" id="messageNotSentInsufficientCredits" />
   <input type="hidden" value="<%: Resources.Global.warningMessageSentSpendingLimitReached %>" id="messageSentSpendingLimitReached" />
    <input type="hidden" value="<%: Resources.Global.messageChooseVoucher %>" id="messageChooseVoucher" />
    <input type="hidden" value="<%: Resources.Global.btnGiveVoucher %>" id="titleBtnGiveVoucher" />
    <input type="hidden" value="<%: Resources.Global.messageNoVouchers %>" id="messageNoVouchers" />
    <input type="hidden" value="<%: User.IsInRole("VoucherOperator") ? "yes" : "no" %>" id="hasVoucherRole" />
</asp:Content>