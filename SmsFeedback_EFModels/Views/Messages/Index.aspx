<%@ Page Title="SmsFeedback" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SmsFeedback_Take4.Models.ViewModels.MessagesContext>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/phonenumbers.css") %>"/> 
    <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/messages.css") %>"/> 
    <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/conversations.css") %>"/> 
         
    <script src="<%: Url.Content("~/Scripts/spin.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/MyScripts/PhoneNumbers.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/MyScripts/Messages.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/MyScripts/Conversations.js") %>" type="text/javascript"></script>
    
    <script src="<%: Url.Content("~/Scripts/Strophe/strophe.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/flxhr/flXHR.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/flxhr/strophe.flxhr.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/MyScripts/XMPP.js") %>" type="text/javascript"></script>

    <script type="text/template" id="phoneNumber-template">       
		<span class="phoneNumber" number="something" >
			<span>{{ label }}</span>
			<span class="deletePhoneNumber">
				<img class="deletePhoneNumberIcon" src="<%: Url.Content("~/Content/images/close14x14.png") %>"/>
			</span>
		</span>
	</script>    

    <script type="text/template" id="conversation-template">
           <div class="leftLiDiv">
                <img src="<%: Url.Content("~/Content/images/delete.ico") %>" class="images" />
            </div>
            <div class="rightLiDiv">                
                <div class="spanClassFrom">
                    <span>{{ From }}</span>
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

    <script  type="text/template" id="message-template">
        <div>
        <span>{{ Text }} </span> 
        <span class="timeReceived">{{ TimeReceived }} </span>
        </div>
        <div class="clear"></div>
        <div class="extramenu" hoverID="{{ ID }}">
            <div class="actionButtons">
                <button id="markAsImportant" >Star</button>
            </div>
            <div class="actionButtons">
                <button id="sendEmail" >Email</button> 
                
            </div>
            <div class="actionButtons">
                <button id="copyText" >Copy</button>               
            </div>
            <div class="clear"></div>
         </div>
    </script>
        
    <script type="text/javascript">

        $(function () {
            if (window.Prototype) {
                delete Object.prototype.toJSON;
                delete Array.prototype.toJSON;
                delete Hash.prototype.toJSON;
                delete String.prototype.toJSON;
            }
            //putting it all together

            //build the areas
            var convView = ConversationArea();
            var msgView = MessagesArea();            

            //get the initial conversations
            convView.getConversations();

            //the xmpp handler for new messages
            var xmppHandler = CreateXMPPHandler(convView,msgView);
            xmppHandler.connect("smsapp@smsfeedback.com/07541237895", "123456");

            var msgID = 1;
            $("#updateMessage").bind("click", function () {                
                var convID = '0754654213-0745103618';
                var newTrimmedText = "And another thing...";
                var fromID = '0754654213';                
                var newText = "And another thing, where are your wine bottles?"
                var dateReceived = "Mon, 15 Aug 2005 15:52:01 +0000";

                newMessageReceived(convView, msgView,fromID, convID, msgID, dateReceived, newText, newTrimmedText);
                msgID++;
            });
            
            $("#addMessage").bind("click", function () {
                var convID = '0753214212-0745103618';
                var fromID = '0753214212';
                var dateReceived = "Mon, 15 Aug 2005 15:52:01 +0000";
                var newTrimmedText = "Something new just got added...";
                var newText = "Something new just got added, aren't you curious?"
                newMessageReceived(convView, msgView, fromID, convID, msgID, dateReceived, newText, newTrimmedText);                               
            });
        });
    </script>

    <div id="navigationMenu" class="grid_1">
            <p>This will be the menu</p>
            <button id="addMessage">Add</button>
            <button id="removeMessage">Remove</button>
            <button id="updateMessage">Update</button>
            <button id="btnSlideUp">SlideUp</button>
            <button id="btnSlideDown">SlideDown</button>
        </div>
        <div id="conversationsArea" class="grid_5">
            <h2>Conversations</h2>
            <div id="phoneNumbersPool" class="wordwrap tagsPhoneNumbers">                
            </div>
            <div id="scrollableconversations" class="conversationbox scrollablebox"> 
                <div id="conversations" class="conversationbox"> 

                </div>
            </div>  
        </div>
        <div id="messagesArea" class="grid_6">
            <h2>Messages</h2>
            <div id="tagsPool" class="tagsPhoneNumbers">               
               <span> Here will be the tags</span>
            </div>
            <div id="scrollablemessagebox" class="messagesboxcontainerclass scrollablebox">
                <div id="messagesbox"  class="messagesboxclass">
                    No conversation selected, please select one
                </div>
            </div>
            <div id="textareaContainer" class="invisible">                
                <form name="replyToMessageForm">
                <textarea id="limitedtextarea" onkeydown="limitText(this.form.limitedtextarea,this.form.countdown,160);"
                    onkeyup="limitText(this.form.limitedtextarea,this.form.countdown,160);" dir="ltr"></textarea>                   
                    <br>
                            <font size="1">(Left: 160)<br>
                            You have <input readonly type="text" name="countdown" size="3" value="160"> characters left.</font>
                  </form>
                 <button id="replyBtn">Reply</button>
            </div>
        </div>
        <div class="clear"></div> 
</asp:Content>
