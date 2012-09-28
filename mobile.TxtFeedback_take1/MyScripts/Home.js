"use strict";
window.app = window.app || {};
var gSelectedMessage = null;
var gSelectedConversationID = null;
var gSelectedElement = null;
window.app.selectedConversation = {};
window.app.globalMessagesRep = {};
window.app.msgView = {};
window.app.defaultConversationID = "0752345678-0751569435";
window.app.calendarCulture = "ro";

var timer; //this will be responsible for triggering the "mark conversation as read event"
var timer_is_on = 0;

//#region Mark Conversation as Read function
function markConversationAsRead() {
   $(gSelectedElement).removeClass("unreadconversation");
   $(gSelectedElement).addClass("readconversation");
   app.selectedConversation.set({ "Read": true });
   //call the server to mark the conversation as read
   $.getJSON('Messages/MarkConversationAsRead',
               { conversationId: gSelectedConversationID },
               function (data) {
                  //conversation marked as read
                  app.updateNrOfUnreadConversations(false);
               }
       );
}
//#endregion

//#region Timer for marking a conversation as being read
function resetTimer() {
   if (timer_is_on) {
      clearTimeout(timer);
      timer_is_on = false;
   }
}

function startTimer(intervalToWait) {
   if (!timer_is_on) {
      //establish if any action is still required - maybe the conversation is already read
      if (!$(gSelectedElement).hasClass("readconversation")) {
         timer = setTimeout(markConversationAsRead, intervalToWait);
         timer_is_on = true;
      }
   }
}
//#endregion

//#region Message model
window.app.Message = Backbone.Model.extend({
   defaults: {
      From: "0752345678",
      To: "0751569435",
      Text: "defaulttext",
      //DateTimeInTicks: (new Date()).valueOf(),
      TimeReceived: new Date(),
      ConvID: window.app.defaultConversationID,
      Direction: "from",
      Read: false,
      Starred: false
   },
   parse: function (data, xhc) {
      //a small hack: the TimeReceived will be something like: "\/Date(1335790178707)\/" which is not something we can work with
      //in the TimeReceived property we have the same info coded as ticks, so we replace the TimeReceived value with a value build from the ticks value
      var dateInTicks = data.TimeReceived.substring(6, 19);
      data.TimeReceived = (new Date(Date.UTC(data.Year, data.Month - 1, data.Day, data.Hours, data.Minutes, data.Seconds)));
      //we have to determine the direction
      var dir = cleanupPhoneNumber(data.From) + "-" + cleanupPhoneNumber(data.To);
      if (dir === data.ConvID) {
         dir = "from";
      }
      else {
         dir = "to";
      }
      data.Direction = dir;
      return data;
   },
   idAttribute: "Id"
});
//#endregion

//#region MessagesPool
window.app.MessagesList = Backbone.Collection.extend({
   model: app.Message,
   identifier: null,
   url: function () {
      return "Messages/MessagesList";
   }
});
//#endregion

//#region MessagesArea default properties
window.app.defaultMessagesOptions = {
   messagesRep: {},
   currentConversationId: ""
};
//#endregion

function MessagesArea() {
   var self = this;

   $.extend(this, app.defaultMessagesOptions);  
  
   $("#reply").click(function () {
      sendMessageToClient();
   });

   var id = 412536; //this should be unique
   var sendMessageToClient = function () {
      var inputBox = $("#replyText");
      id++;
      //add it to the visual list
      //I should set values to all the properties
      var msgContent = inputBox.val();

      var fromTo = getFromToFromConversation(self.currentConversationId);
      var from = fromTo[0];
      var to = fromTo[1];
      
      //TODO should be RFC822 format
      var timeSent = new Date();
      $(document).trigger('msgReceived', {
         fromID: to,
         toID: from,
         convID: self.currentConversationId,
         msgID: id,
         dateReceived: timeSent,
         text: msgContent,
         readStatus: false,
         messageIsSent: true
      });
      //reset the input form
      inputBox.val('');

      //signal all the other "listeners/agents"
      window.app.xmppHandlerInstance.send_reply(to, from, timeSent, msgContent, window.app.addressOfPhpScripts);
   };

      _.templateSettings = {
      interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
      evaluate: /\{%([\s\S]+?)%\}/g,   // execute code: {% code_to_execute %}
      escape: /\{%-([\s\S]+?)%\}/g
   }; // excape HTML: {%- <script> %} prints &lt

   var MessageView = Backbone.View.extend({
      model: app.Message,
      tagName: "div",
      messageTemplate: _.template($('#message-template').html()),
      initialize: function () {
         _.bindAll(this, 'render', 'updateView');
         this.model.on("change", this.updateView);
         return this.render;
      },
      render: function () {
         this.$el.html(this.messageTemplate(this.model.toJSON()));
         var direction = "messagefrom";
         var arrowInnerMenuLeft = "arrowInnerLeft";
         var extraMenuWrapperSide = "extraMenuWrapperLeft";
         //var arrowExtraMenu="arrowExtraMenuFrom";
         //var arrowInnerExtraMenu = "arrowInnerExtraMenuFrom";
         if (this.model.attributes["Direction"] === "to") {
            direction = "messageto";
            arrowInnerMenuLeft = "arrowInnerRight";
            extraMenuWrapperSide = "extraMenuWrapperRight";
            //arrowInnerExtraMenu = "arrowInnerExtraMenuTo";
         }
         this.$el.addClass("message");
         this.$el.addClass(direction);

         $(".innerExtraMenu", this.$el).addClass(arrowInnerMenuLeft);
         $(".extraMenuWrapper", this.$el).addClass(extraMenuWrapperSide);

         var sendEmail = $("div.sendEmailButton img", this.$el);
         return this;
      },
      updateView: function () {
         return this;
      }
   });

   //we fade in when we first load a conversation, afterwards we just render - no fade in
   var performFadeIn = false;
   var MessagesView = Backbone.View.extend({
      el: $("#messagesbox"),
      initialize: function () {
         _.bindAll(this,
            "render",
            "getMessages",
            "appendMessage",
            "appendMessageToDiv",
            "resetViewToDefault",
            "newMessageReceived");// to solve the this issue
         this.messages = new app.MessagesList();
         this.messages.bind("reset", this.render);
      },
      resetViewToDefault: function () {
         var noConversationLoadedMessage = $("#noConversationSelectedMessage").val();
         $('#messagesbox').html('<span id="noConversationsLoaded">' + noConversationLoadedMessage + '</span>');
         $("#textareaContainer").addClass("invisible");
         $("#tagsContainer").addClass("invisible");
         self.currentConversationId = '';
      },
      getMessages: function (conversationId) {
         $("#messagesbox").html('');         
         var messages = new app.MessagesList();
         messages.identifier = conversationId;
         messages.bind("reset", this.render);
         messages.bind('add', this.appendMessage);
         performFadeIn = true;
         
         self.currentConversationId = conversationId;
         app.globalMessagesRep[self.currentConversationId] = messages;

         var msgFrom = new app.Message({Text: "Welcome to Lidl Republicii. How can we be of service?"})
         messages.add(msgFrom);
         //var msgTo = new app.Message({ Text: "Do you still have Zuzu milk?", Direction : "to" });
         //messages.add(msgTo);
         //var msgFromReply = new app.Message({ Text: "Yes, we will bring more to the aile in 5 minutes" });
         //messages.add(msgFromReply);        
      },
      render: function () {
         $("#messagesbox").html('');
         var selfMessageView = this;
         app.globalMessagesRep[self.currentConversationId].each(function (msg) {            
            selfMessageView.appendMessageToDiv(msg, performFadeIn, false);
         });
         spinner.stop();
         return this;
      },
      appendMessage: function (msg) {
         //append only if the current view is the one in focus
         if (msg.get('ConvID') === self.currentConversationId) {
            //when appending a new message always scroll to bottom
            this.appendMessageToDiv(msg, true, true);
         }
      },
      newMessageReceived: function (fromID, convID, msgID, dateReceived, text) {
         var newMsg = new app.Message({ Id: msgID });
         //decide if this is a from or to message
         var fromTo = getFromToFromConversation(convID);
         var from = fromTo[0];
         var direction = "from";
         if (!comparePhoneNumbers(fromID, from)) {
            direction = "to";
         }
         newMsg.set("Direction", direction);
         newMsg.set("From", fromID);
         newMsg.set("ConvID", convID);
         newMsg.set("Text", text);
         //we receive the date as RFC 822 string - we need to convert it to a valid Date
         newMsg.set("TimeReceived", new Date(Date.parse(dateReceived)));
         newMsg.set("ClientDisplayName", from);
         newMsg.set("ClientIsSupportBot", false);
         //we add the message only if are in correct conversation
         if (app.globalMessagesRep[convID] !== undefined) {
            app.globalMessagesRep[convID].add(newMsg);
         }
      },
      appendMessageToDiv: function (msg, performFadeIn, scrollToBottomParam) {
         var msgView = new MessageView({ model: msg });
         var item = msgView.render().el;
         $(this.el).append(item);
         $(item).hover(function () {
            var helperDiv = $(this).find("div.extramenu")[0];
            //make sure to bind the buttons
            // $(helperDiv).css("visibility", "visible");
            gSelectedMessage = $($(this).find("div span")[0]).html();
            //$(helperDiv).fadeIn(400);
            $(helperDiv).show();
            //ContactWindow.init();
         }, function () {
            var helperDiv = $(this).find("div.extramenu")[0];
            //$(helperDiv).fadeOut("fast");
            $(helperDiv).hide();
         });

         if (performFadeIn) {
            $(item).hide().fadeIn("2000");
         }
         //var helperDiv = $(this).find("div")[0];
         //$(helperDiv).css)
         //if (scrollToBottomParam) {
         //var messagesEl = $("#messagesbox");
         //messagesEl.animate({ scrollTop: messagesEl.prop("scrollHeight") }, 3000);
         //}
      }
   });
   this.messagesView = new MessagesView();
}

$(function () {
   window.app.msgView = new MessagesArea(self.convView, self.tagsArea);
   window.app.msgView.messagesView.getMessages(window.app.defaultConversationID);
   $("[data-role=header]").fixedtoolbar({ tapToggle: true });

   $(document).bind('msgReceived', function (ev, data) {
      newMessageReceivedGUI(window.app.msgView.messagesView, data.fromID, data.toID, data.convID, data.msgID, data.dateReceived, data.text, false);
   });
})

function newMessageReceivedGUI( msgView, fromID, toId, convID, msgID, dateReceived, text, readStatus) {
   //the conversations window expects that the toID be a "name" and not a telephone number   
   msgView.newMessageReceived(fromID, convID, msgID, dateReceived, text);
}

