
/*global window */
/*global Backbone */
/*global document */
/*global console */
window.app = window.app || {};
window.app.globalMessagesRep = {};
window.app.msgView = {};
window.app.calendarCulture = "en-GB";

function newMessageReceivedGUI(msgView, fromID, toId, convID, msgID, dateReceived, text, readStatus) {
   //the conversations window expects that the toID be a "name" and not a telephone number   
   msgView.newMessageReceived(fromID, convID, msgID, dateReceived, text);
}
//#region Message model
window.app.Message = Backbone.Model.extend({
   defaults: {
      From: window.app.defaultFrom,
      To: window.app.defaultTo,
      Text: window.app.defaultMessage,      
      TimeReceived: new Date(),
      ConvID: window.app.defaultConversationID,
      Direction: "from",
      Read: false,
      Starred: false
   },
   parse: function (data, xhc) {
      //a small hack: the TimeReceived will be something like: "\/Date(1335790178707)\/" which is not something we can work with
      //in the TimeReceived property we have the same info coded as ticks, so we replace the TimeReceived value with a value build from the ticks value      
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
   model: window.app.Message,
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
   "use strict";
   var self = this;

   $.extend(this, window.app.defaultMessagesOptions);  
  
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
         fromID: from,
         toID: to,
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
      window.app.xmppHandlerInstance.send_reply(from, to, timeSent, self.currentConversationId, msgContent, window.app.suffixedMessageModeratorAddress);
   };

      _.templateSettings = {
      interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
      evaluate: /\{%([\s\S]+?)%\}/g,   // execute code: {% code_to_execute %}
      escape: /\{%-([\s\S]+?)%\}/g
   }; // excape HTML: {%- <script> %} prints &lt

   var MessageView = Backbone.View.extend({
      model: window.app.Message,
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
         if (this.model.attributes.Direction === "to") {
            direction = "messageto";
            arrowInnerMenuLeft = "arrowInnerRight";
            extraMenuWrapperSide = "extraMenuWrapperRight";
            //arrowInnerExtraMenu = "arrowInnerExtraMenuTo";
         }
         this.$el.addClass("message");
         this.$el.addClass(direction);

         //$(".innerExtraMenu", this.$el).addClass(arrowInnerMenuLeft);
         //$(".extraMenuWrapper", this.$el).addClass(extraMenuWrapperSide);

         //var sendEmail = $("div.sendEmailButton img", this.$el);
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
         this.messages = new window.app.MessagesList();
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
         //console.log("get messages");
         $("#messagesbox").html('');         
         var messages = new window.app.MessagesList();
         messages.identifier = conversationId;
         messages.bind("reset", this.render);
         messages.bind('add', this.appendMessage);
         performFadeIn = true;
         
         self.currentConversationId = conversationId;
         window.app.globalMessagesRep[self.currentConversationId] = messages;

         var msgWelcome = new window.app.Message({
            From: window.app.defaultTo,
            To: window.app.defaultFrom,
            Text: $("#welcomeMessage").text(),
            TimeReceived: new Date(),
            ConvID: conversationId,
            Direction: "from",
            Read: false,
            Starred: false
         });
         messages.add(msgWelcome);
         //var msgTo = new app.Message({ Text: "Do you still have Zuzu milk?", Direction : "to" });
         //messages.add(msgTo);
         //var msgFromReply = new app.Message({ Text: "Yes, we will bring more to the aile in 5 minutes" });
         //messages.add(msgFromReply);        
      },
      render: function () {
         $("#messagesbox").html('');
         var selfMessageView = this;
         window.app.globalMessagesRep[self.currentConversationId].each(function (msg) {            
            selfMessageView.appendMessageToDiv(msg, performFadeIn, false);
         });         
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
         var newMsg = new window.app.Message({ Id: msgID });
         //decide if this is a from or to message
         var fromTo = getFromToFromConversation(convID);
         var selfTelNo = fromTo[0];
         var direction = "to";
         if (!comparePhoneNumbers(fromID, selfTelNo)) {
            direction = "from";
         }
         newMsg.set("Direction", direction);
         newMsg.set("From", fromID);
         newMsg.set("ConvID", convID);
         newMsg.set("Text", text);
         //we receive the date as RFC 822 string - we need to convert it to a valid Date
         newMsg.set("TimeReceived", new Date(Date.parse(dateReceived)));
         newMsg.set("ClientDisplayName", selfTelNo);
         newMsg.set("ClientIsSupportBot", false);
         //we add the message only if are in correct conversation
         if (window.app.globalMessagesRep[convID] !== undefined) {
            window.app.globalMessagesRep[convID].add(newMsg);
         }
      },
      appendMessageToDiv: function (msg, performFadeIn, scrollToBottomParam) {
         var msgView = new MessageView({ model: msg });
         var item = msgView.render().el;
         $(this.el).append(item);         
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
   window.app.initializeBasedOnLocation();

   window.app.msgView = new MessagesArea();
   $("[data-role=header]").fixedtoolbar({ tapToggle: true });
   $("[data-role=footer]").fixedtoolbar({ tapToggle: false });

   $(document).bind('msgReceived', function (ev, data) {
      newMessageReceivedGUI(window.app.msgView.messagesView, data.fromID, data.toID, data.convID, data.msgID, data.dateReceived, data.text, false);
   });

   $(window).unload(function () {
      window.app.saveLoginDetails();
   });
   window.app.loadLoginDetails();
});



