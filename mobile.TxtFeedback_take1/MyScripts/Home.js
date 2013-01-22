//#region Defined in other documents
/*global window */
/*global Backbone */
/*global document */
/*global console */
/*global getFromToFromConversation */
/*global comparePhoneNumbers */
/*global cleanupPhoneNumber */
/*global _ */
//#endregion
window.app = window.app || {};
window.app.globalMessagesRep = {};
window.app.msgView = {};

//#region Helpers
function getMessagePositionInRepository(convID, msgID) {
   var messagePosition = -1;
   if (window.app.globalMessagesRep[convID] != null && window.app.globalMessagesRep[convID] != undefined) {
      for (var i = 0; i < window.app.globalMessagesRep[convID].models.length; ++i) {
         var currentModel = window.app.globalMessagesRep[convID].models[i];
         if (currentModel.attributes.Id === msgID) {
            return i;
         }
      }
   }
   return messagePosition;
}

function setMsgWasSuccessfullySentValue(convID, msgID, sentStatus) {
   var messagePosition = getMessagePositionInRepository(convID, msgID);
   var msgSent = window.app.globalMessagesRep[convID].at(messagePosition);
   if (msgSent != null) {
      msgSent.set("WasSuccessfullySent", sentStatus);
   }
}

function setMsgClientAcknowledgeValue(convID, msgID, clientAcknowledge) {
   var messagePosition = getMessagePositionInRepository(convID, msgID);
   var msgSent = window.app.globalMessagesRep[convID].at(messagePosition);
   if (msgSent != null) {
      msgSent.set("ClientAcknowledge", clientAcknowledge);
   }
}
//#endregion

//#region UUID generator, rfc4122 compliant, details http://www.ietf.org/rfc/rfc4122.txt
function generateUUID() {
   var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
   });
   return uuid;
}
//#endregion

function newMessageReceivedGUI(msgView, fromID, toId, convID, msgID, dateReceived, text, readStatus) {
   //the conversations window expects that the toID be a "name" and not a telephone number   
   msgView.newMessageReceived(fromID, convID, msgID, dateReceived, text);
}

function messageSuccessfullySent(msgView, message) {
    msgView.messageSuccessfullySent(message);
}

function acknowledgeFromClient(msgView, message) {
   msgView.acknowledgeFromClient(message);
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
      Starred: false,
      WasSuccessfullySent: false,
      ClientAcknowledge: false
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

window.app.sendMessageToClient = function (id, convID) {
   var inputBox = $("#replyText");
   if ($.trim($("#replyText").val()).length > 0) {
      //add it to the visual list
      //I should set values to all the properties
      var msgContent = inputBox.val();

      var fromTo = getFromToFromConversation(convID);
      var from = fromTo[0];
      var to = fromTo[1];
      //TODO should be RFC822 format           
      var timeSent = new Date();
      $(document).trigger('msgReceived', {
         fromID: from,
         toID: to,
         convID: convID,
         msgID: id,
         dateReceived: timeSent,
         text: msgContent,
         readStatus: false,
         messageIsSent: true
      });
      //reset the input form
      inputBox.val('');

      //signal all the other "listeners/agents"
      window.app.xmppHandlerInstance.send_reply(from, to, timeSent, convID, msgContent, window.app.suffixedMessageModeratorAddress, id);
   }
};
window.app.messageID = 412342;
window.app.MessagesArea = function () {
   "use strict";
   var self = this;

   $.extend(this, window.app.defaultMessagesOptions);

   $("#reply").click(function () {
      window.app.sendMessageToClient(window.app.messageID++, self.currentConversationId);
   });
   $("#replyText").keydown(function (event) {
      if (event.which === 13) {
         event.preventDefault();
         window.app.sendMessageToClient(window.app.messageID++, self.currentConversationId);
      }
   });


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
         _.bindAll(this, 'render');
         this.model.on("change:WasSuccessfullySent", this.render);
         this.model.on("change:ClientAcknowledge", this.render);         
         return this.render;
      },
      render: function () {
         this.$el.html(this.messageTemplate(this.model.toJSON()));
         var direction = "messagefrom";         
         if (this.model.attributes.Direction === "to") {
            direction = "messageto";            
         }
         this.$el.addClass("message");
         this.$el.addClass(direction);
         
         var messageId = this.model.get("Id");         
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
         $(this.el).append("<div class='clear'></div>");
         $(this.el).css("margin-bottom", "100px");         
      },
      resetViewToDefault: function () {
         var noConversationLoadedMessage = $("#noConversationSelectedMessage").val();
         $('#messagesbox').html('<span id="noConversationsLoaded">' + noConversationLoadedMessage + '</span>');         
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
      },
      render: function () {
          $("#messagesbox").html('');
          $("#messagesbox").append("<div class='clear'></div>");
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
         $(".debug").empty();
         $(this.el).append(item);
         //if (performFadeIn) {
           // $(item).hide().fadeIn("2000");
         //}
         if (msg.get("WasSuccessfullySent"))
             $(".singleCheckNo" + msg.get("Id")).css("visibility", "visible");
         
         var magicNumber = 50;
         var bodyHeight = $(window).height() - 2 * $(".ui-header").height() - magicNumber;
         var contentHeight = $("#contentArea").height();
         if (contentHeight > bodyHeight) {
            $(document).scrollTop(document.body.scrollHeight - $(".ui-footer").height());
         }        
         $(this.el).append("<div class='clear'></div>");
      },
      messageSuccessfullySent: function (message) {
         setMsgWasSuccessfullySentValue(message.convID, message.msgID, true);         
      },
      acknowledgeFromClient: function (message) {
         setMsgClientAcknowledgeValue(message.convID, message.msgID, true);
      }
   });
   this.messagesView = new MessagesView();
};

$(function () {
   if (window.app.initializeBasedOnLocation()) {
         $('body').bind('touchstart', function (e) {
         });
         window.app.msgView = new window.app.MessagesArea();
         $("[data-role=header]").fixedtoolbar({ tapToggle: true });
         $("[data-role=footer]").fixedtoolbar({ tapToggle: false });

         $(document).bind('msgReceived', function (ev, data) {
            newMessageReceivedGUI(window.app.msgView.messagesView, data.fromID, data.toID, data.convID, data.msgID, data.dateReceived, data.text, false);
         });

         $(document).bind('serverAcknowledge', function (ev, data) {
             messageSuccessfullySent(window.app.msgView.messagesView, data.message);
         });

         $(document).bind('clientAcknowledge', function (ev, data) {
            acknowledgeFromClient(window.app.msgView.messagesView, data.message);
         });

         //DA unfortunately Opera does not implement correctly onUnload - so this will not be triggered when closing opera
         $(window).unload(function () {            
            window.app.saveLoginDetails();
            window.app.disconnectXMPP();
         });
         window.app.loadLoginDetails();
         //DA launch timer only if not on blackberry
         //var blackBerry = false;
         //var ua = window.navigator.userAgent;
         //if (ua.indexOf("BlackBerry") >= 0) {            
         //      blackBerry = true;            
         //}
         //if (!blackBerry) {
            window.app.startReconnectTimer();
         //}
   }
    
   $("#replyText").click(function () {
       if ($("#replyText").is(":focus")) {
           $("#footer").css("position", "relative");
       }
   });
   

   $("#replyText").blur(function () {
       $("#footer").css("position", "fixed");
   });
   
  /*
   $(window).scroll(function () {
       $("#footer").css("top", window.scrollTop + $(window).height - 50);
   });
   */
});



