//#region Defines to stop jshint from complaining about "undefined objects"
/*global window */
/*global Strophe */
/*global document */
/*global console */
/*global $pres */
/*global $iq */
/*global $msg */
/*global Persist */
/*global DOMParser */
/*global ActiveXObject */
/*global Backbone */
/*global _ */
/*global Spinner */
/*global buildConversationID */
/*global cleanupPhoneNumber */
/*global comparePhoneNumbers */
/*global getFromToFromConversation */
/*global setTooltipOnElement */
/*global clearTimeout */
/*global confirm */
/*global setTimeout */
/*global isWorkingPoint */
//#endregion
window.app = window.app || {};
window.app.globalMessagesRep = {};

var gSelectedMessage = null;
var gSelectedMessageItem = null;
var gSelectedConversationID = null;
var gSelectedElement = null;
var gDateOfSelectedMessage = null;
var gDateDisplayPattern = 'DD, MM d, yy';

var timer; //this will be responsible for triggering the "mark conversation as read event"
var timer_is_on = 0;

//#region UUID generator, rfc4122 compliant, details http://www.ietf.org/rfc/rfc4122.txt
function generateUUID() {
   var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
   });
   return uuid;
}
//#endregion

//#region Mark Conversation as Read function
function markConversationAsRead() {
   $(gSelectedElement).removeClass("unreadconversation");
   $(gSelectedElement).addClass("readconversation");
   window.app.selectedConversation.set({ "Read": true });
   //call the server to mark the conversation as read
   $.getJSON('Conversations/MarkConversationAsRead',
               { conversationId: gSelectedConversationID },
               function (data) {
                  //conversation marked as read
                  window.app.updateNrOfUnreadConversations(false);
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

//#region Delete message
function deleteMessage(element, messageText, timeReceived, convId) {
   $(element).remove();
   var messagePosition = 0;
   for (var i = 0; i < window.app.globalMessagesRep[convId].models.length; ++i) {
      var currentModel = window.app.globalMessagesRep[convId].models[i];
      var timeDifference = timeReceived.getTime() - currentModel.attributes.TimeReceived.getTime();
      if (currentModel.attributes.Text.trim() === messageText && Math.abs(timeDifference) < 100 && currentModel.attributes.ConvID === convId) {
         messagePosition = i;
      }
   }
   window.app.globalMessagesRep[convId].remove(window.app.globalMessagesRep[convId].at(messagePosition));
}
//#endregion

//#region Helpers
function convertDateTimeStringToObject(dateTimeString) {
   var date = dateTimeString.substr(0, dateTimeString.length - 10);
   var time = dateTimeString.substr(dateTimeString.length - 9, 8);
   var hours = time.substr(0, 2);
   var minutes = time.substr(3, 2);
   var seconds = time.substr(6, 2);
   // default date display pattern is 'DD, MM d, yy'
   if (window.app.calendarCulture === "ro") {
      gDateDisplayPattern = 'DD, d MM, yy';
   }
   var resultedDateTime = $.datepicker.parseDate(gDateDisplayPattern, date,
       {
          dayNamesShort: $.datepicker.regional[window.app.calendarCulture].dayNamesShort, dayNames: $.datepicker.regional[window.app.calendarCulture].dayNames,
          monthNamesShort: $.datepicker.regional[window.app.calendarCulture].monthNamesShort, monthNames: $.datepicker.regional[window.app.calendarCulture].monthNames
       });
   resultedDateTime.setHours(hours, minutes, seconds);
   return resultedDateTime;
}

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

//#region Message model
window.app.Message = Backbone.Model.extend({
   defaults: {
      From: "0752345678",
      To: "0751569435",
      Text: "defaulttext",
      //DateTimeInTicks: (new Date()).valueOf(),
      TimeReceived: Date.now(),
      ConvID: 1,
      Direction: "from",
      Read: false,
      Starred: false,
      IsSmsBased: false,
      WasSuccessfullySent: false,
      ClientAcknowledge: false
   },
   parse: function (data, xhc) {
      //a small hack: the TimeReceived will be something like: "\/Date(1335790178707)\/" which is not something we can work with
      //in the TimeReceived property we have the same info coded as ticks, so we replace the TimeReceived value with a value build from the ticks value      
      data.TimeReceived = new Date(Date.UTC(data.Year, data.Month - 1, data.Day, data.Hours, data.Minutes, data.Seconds));
      //we have to determine the direction
      var dir = cleanupPhoneNumber(data.From) + "-" + cleanupPhoneNumber(data.To);
      if (dir === data.ConvID) {
         dir = "from";
      }
      else {
         dir = "to";
      }
      if (!isNaN(data.Id)) data.WasSuccessfullySent = true;
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


//#region MessageView
window.app.MessageView = Backbone.View.extend({
   model: window.app.Message,
   tagName: "div",   
   events: { "click div.deleteMessage": "deleteMessage" },
   initialize: function () {
      this.messageTemplate= _.template($('#message-template').html()),
      _.bindAll(this, 'render', 'updateView','deleteMessage');
      this.model.on("change", this.updateView);
      this.model.on("change:WasSuccessfullySent", this.render);
      this.model.on("change:ClientAcknowledge", this.render);
      return this.render;
   },   
   render: function () {
      var messageModel = this.model; // offers access to model in deleteMessage function

      this.$el.html(this.messageTemplate(this.model.toJSON()));
      var direction = "messagefrom";
      var arrowInnerMenuLeft = "arrowInnerLeft";
      if (this.model.attributes.Direction === "to") {
         direction = "messageto";
         arrowInnerMenuLeft = "arrowInnerRight";
      }
      this.$el.addClass("message");
      this.$el.addClass(direction);

      $(".innerExtraMenu", this.$el).addClass(arrowInnerMenuLeft);

      var checkIconEl = $($(this.$el).find(".checkNo" + this.model.get("Id")));
      var messageMenuEl = $(this.$el).find("div.messageMenu")[0];
      var messageEl = this.$el;
      // first state
      $(checkIconEl).hide();
      $(messageMenuEl).hide();

      $(this.$el).hover(function () {
         $(messageMenuEl).fadeIn(100);
         $(checkIconEl).show();
      }, function () {
         $(messageMenuEl).fadeOut(100);         
         $(checkIconEl).hide();
      });
      return this;
   },
   deleteMessage: function (e) {
      var messageModel = this.model     
      var messageEl = this.$el;
      e.preventDefault();
      var msgText = messageModel.get("Text");
      var msgConvId = messageModel.get("ConvID");
      var msgTimeRcv = messageModel.get("TimeReceived");
      if (confirm($("#confirmDeleteMessage").val() + " \"" + msgText + "\" ?")) {
         $.ajax({
            url: "Messages/DeleteMessage",
            data: { 'messageText': msgText, 'convId': msgConvId, 'timeReceived': msgTimeRcv.toUTCString() },
            success: function (data) {
               if (data === "success") {
                  deleteMessage(messageEl, msgText, msgTimeRcv, msgConvId);
               } else if (data === "lastMessage") {
                  var previousItem = $(messageEl).prev();
                  deleteMessage(messageEl, msgText, msgTimeRcv, msgConvId);
                  if (previousItem.length !== 0) {
                     var lastMessage = $($(previousItem).find(".textMessage").find("span")).html().trim();
                     var lastMessageDate = convertDateTimeStringToObject($($(previousItem).find(".timeReceived")[0]).html());
                     // update the conversation
                     $.ajax({
                        url: "Conversations/UpdateConversation",
                        data: { 'convId': msgConvId, 'newText': lastMessage, 'newTextReceivedDate': lastMessageDate.toUTCString() },
                        success: function (data) {
                           $(gSelectedElement).find(".spanClassText").find("span").html(lastMessage);
                        }
                     });
                  }
               }
            }
         });
      }
   },
   updateView: function () {
      return this;
   }
});
//#endregion


//#region Send message
window.app.sendMessageToClient = function (text, conversationID, selectedConv, msgID, wpPool) {
   /*
   inside a conversationID
   SMS messages
     from - tel number
     to - self tel number
   XMPP messages
     from - clientXMPP id without @
     to - selfXMPP id without @
   */
   var fromTo = getFromToFromConversation(conversationID);
   var from = fromTo[0];
   var to = fromTo[1];
   //decide if this should be handled via SMS or not
   var isSmsBased = selectedConv.get("IsSmsBased");
   //the component will send/save the message on the server so we don't have to trigger this ourselves        
   //TODO should be RFC822 format
   var timeSent = new Date();
   $(document).trigger('msgReceived', {
      fromID: to,
      toID: from,
      convID: conversationID,
      msgID: msgID,
      dateReceived: timeSent,
      text: text,
      readStatus: true,
      messageIsSent: true,
      isSmsBased: isSmsBased
   });
   //sendToSupport you send to support or to another Staff web client
   var sendToSupport = selectedConv.get("ClientIsSupportBot");
   var storeStaffAddress = to; // defaults to the conversation's to

   var clientToRespondToAddress = from; //defaults to the conversation's from
   if (!isSmsBased) {
      if (sendToSupport) {
         //we assume that the TxtFeedback support runs on the same component
         clientToRespondToAddress = from + window.app.workingPointsSuffixDictionary[to];
      } else {
         //TODO @txtfeedback.net is now hardcoded             
         clientToRespondToAddress = from + "@txtfeedback.net";
      }
      //build the store@moderator.txtfeedback.net
      storeStaffAddress = to + window.app.workingPointsSuffixDictionary[to];
   }
   var storeXMPPcomponentAddress = "";
   if (sendToSupport) {
      /*          
      We need to send a message to TxtFeedback support - which has its own Staff website, so 
      staff has to be true, sms false
      to = should normally be storeID, BUT because we are sending not to our own component (supportID@moderator.txtfeedback.net) but to the one of the store (store@moderator.txtfeedback.net)
      we make some "adjustments" (NOTE: support conversations are treated differently that other conversations)
      convID remains the same: storeID-supportID
      from: support (staff in this case) ID
      to: 
      XMPPto = storeID
      */
      storeXMPPcomponentAddress = clientToRespondToAddress;
   } else {
      /*
      we are dealing with a non-support conversation 
      For SMS
      We send a message to trigger carbons
      For XMPP
      We send a message and then the component redirects that message to the client
      */
      storeXMPPcomponentAddress = wpPool.getWorkingPointXmppAddress(cleanupPhoneNumber(to)) || wpPool.getWorkingPointXmppAddress(cleanupPhoneNumber(from));
   }
   window.app.xmppHandlerInstance.send_reply(storeStaffAddress, clientToRespondToAddress, timeSent, conversationID, text, storeXMPPcomponentAddress, isSmsBased, sendToSupport, msgID);
};
//#endregion

//#region MessagesArea default properties
window.app.defaultMessagesOptions = {
   messagesRep: {},
   currentConversationId: ""
};
//#endregion
function MessagesArea(convView, tagsArea, wpsArea) {
   "use strict";
   var self = this;

   var replyButton = $("#replyBtn");
   replyButton.qtip({
      content: replyButton.attr('tooltiptitle'),
      position: {
         corner: {
            target: 'leftMiddle',
            tooltip: 'rightMiddle'
         }
      },
      style: 'dark'
   });
   $.extend(this, window.app.defaultMessagesOptions);

   this.convView = convView;
   this.tagsArea = tagsArea;
   this.wpsArea = wpsArea;
   //set the filter to make only the top div (conversation) selectable
   // in the absence of the filter option all elements within the conversation are made "selectable"
   $("#conversations").selectable({
      filter: ".conversation",
      selected: function (event, ui) {
         //prepare to mark the conversation as read in 3 seconds - once the messages have been loaded
         //for now make sure to other timers are active
         gSelectedElement = ui.selected;
         // If the selected conversation is the support conversation than add a special class. Else remove the ui-selectedSupport
         if ($(gSelectedElement).hasClass("supportConversation")) {
            if (!$(gSelectedElement).hasClass("ui-selectedSupport")) {
               $(gSelectedElement).addClass("ui-selectedSupport");
            }
         } else {
            // SEARCH for ui-selectedSupport and remove that class;
            $(gSelectedElement).parent().children(".ui-selectedSupport").removeClass("ui-selectedSupport");
         }
         resetTimer();
         var convId = ui.selected.getAttribute("conversationid");
         gSelectedConversationID = convId;

         window.app.selectedConversation = self.convView.convsList.get(convId);
         $(document).trigger("conversationSelected", {convID:convId});
         //self.messagesView.getMessages(convId);
         //self.tagsArea.getTags(convId);
      },
      cancel: ".ignoreElementOnSelection"
   });

   $("#replyBtn").click(function () {      
      self.sendMessageTriggered();      
   });

   self.limitText = function (limitField, limitCount, limitNum) {
      if (limitField.val().length > limitNum) {
         limitField.val(limitField.val().substring(0, limitNum));
      } else {
         limitCount.val(limitNum - limitField.val().length);
      }
   };

   var countdown = $('input[name="countdown"]');
   $("#limitedtextarea").keydown(function (event) {      
      if (event.which === 13 && event.shiftKey) {
         event.preventDefault();
         self.sendMessageTriggered();
         return;
      }
      self.limitText($(this), countdown, 160);
   });
   $("#limitedtextarea").keyup(function (event) {      
      self.limitText($(this), countdown, 160);
   });

   self.sendMessageTriggered = function () {
      //DA you should not be able to send an SMS message if no credit available (or spending limit reached)
      var inputBox = $("#limitedtextarea");
      var msgContent = inputBox.val();

      if (window.app.selectedConversation.get("IsSmsBased")) {
         //if can send SMS messages
         if (window.app.canSendSmS) {
            $("#replyToMessageForm")[0].reset();
            window.app.sendMessageToClient(msgContent, self.currentConversationId, window.app.selectedConversation, generateUUID(), self.wpsArea.wpPoolView.phoneNumbersPool);
         } else {
            window.app.NotifyArea.show($('#msgMessageNotSent').val(), function () {
               window.location.href = "mailto:contact@txtfeedback.net?subject=Increase spending limit or Buy Credit";

            }, true);
            //alert("Cannot send sms message");
         }

      }
      else {
         //reset the input form
         $("#replyToMessageForm")[0].reset();
         window.app.sendMessageToClient(msgContent, self.currentConversationId, window.app.selectedConversation, generateUUID(), self.wpsArea.wpPoolView.phoneNumbersPool);
      }
      
      
      
   };
   
   _.templateSettings = {
      interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
      evaluate: /\{%([\s\S]+?)%\}/g,   // execute code: {% code_to_execute %}
      escape: /\{%-([\s\S]+?)%\}/g
   }; // escape HTML: {%- <script> %} prints &lt

   var opts = {
      lines: 13, // The number of lines to draw
      length: 7, // The length of each line
      width: 4, // The line thickness
      radius: 10, // The radius of the inner circle
      rotate: 0, // The rotation offset
      color: '#000', // #rgb or #rrggbb
      speed: 1, // Rounds per second
      trail: 60, // Afterglow percentage
      shadow: true, // Whether to render a shadow
      hwaccel: false, // Whether to use hardware acceleration
      className: 'spinner', // The CSS class to assign to the spinner
      zIndex: 2e9, // The z-index (defaults to 2000000000)
      top: 'auto', // Top position relative to parent in px
      left: 'auto' // Left position relative to parent in px
   };
   var spinner = new Spinner(opts);
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
            "newMessageReceived",
            "messageSuccessfullySent",
            "setAcknowledgeFromClient");// to solve the this issue
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
         $("#messagesbox").html('');
         var target = document.getElementById('scrollablemessagebox');
         spinner.spin(target);

         self.currentConversationId = conversationId;
         if (self.currentConversationId in window.app.globalMessagesRep) {
            //we have already loaded this conversation so display the cached messages 
            //this should be a realistic view due to the fact that we are listening to new messages
            performFadeIn = false;
            spinner.stop();
            startTimer(3000);
            this.render();
            $("#textareaContainer").removeClass("invisible");
            $("textareaContainer").fadeIn("slow");
            $("#tagsContainer").removeClass("invisible");
            $("#tagsContainer").fadeIn("slow");
         }
         else {
            var messages1 = new window.app.MessagesList();
            messages1.identifier = conversationId;
            messages1.bind("reset", this.render);
            messages1.bind('add', this.appendMessage);
            performFadeIn = true;
            messages1.fetch({
               data: { "conversationId": messages1.identifier },
               success: function () {
                  //we have loaded the conversation - give the user some time to read it and then mark it as read
                  startTimer(3000);
                  spinner.stop();
                  $("#textareaContainer").removeClass("invisible");
                  $("textareaContainer").fadeIn("slow");
                  $("#tagsContainer").removeClass("invisible");
                  $("#tagsContainer").fadeIn("slow");
               }
            });
            window.app.globalMessagesRep[self.currentConversationId] = messages1;
            $.each(messages1, function (index, value) {
               value.set("Starred", window.app.selectedConversation.get("Starred"));
            });
         }
      },
      render: function () {
         $("#messagesbox").html('');
         var selfMessageView = this;
         window.app.globalMessagesRep[self.currentConversationId].each(function (msg) {
            //don't scroll to bottom as we will do it when loading is done
            selfMessageView.appendMessageToDiv(msg, performFadeIn, false);
         });
         spinner.stop();
         //scroll to bottom
         //var messagesEl = $("#scrollablemessagebox");
         //messagesEl.animate({ scrollTop: messagesEl.prop("scrollHeight") }, 3000);
         return this;
      },
      appendMessage: function (msg) {
         //append only if the current view is the one in focus
         if (msg.get('ConvID') === self.currentConversationId) {
            //when appending a new message always scroll to bottom
            this.appendMessageToDiv(msg, true, true);
         }
      },
      newMessageReceived: function (fromID, convID, msgID, dateReceived, text, read, isSmsBased) {
         var newMsg = new window.app.Message({
            Id: msgID,
            From: fromID,
            Text: text,
            ConvID: convID,
            ClientDisplayName: fromID,
            ClientIsSupportBot: false,
            Read: read,
            IsSmsBased: isSmsBased,
            TimeReceived: dateReceived
         });
         //decide if this is a from or to message
         var fromTo = getFromToFromConversation(convID);
         var from = fromTo[0];
         var direction = "from";
         if (!comparePhoneNumbers(fromID, from)) {
            direction = "to";
         }
         newMsg.set("Direction", direction);
         //we add the message only if are in correct conversation
         if (window.app.globalMessagesRep[convID] !== undefined) {
            window.app.globalMessagesRep[convID].add(newMsg);
         }
      },
      appendMessageToDiv: function (msg, performFadeIn, scrollToBottomParam) {
         var msgView = new window.app.MessageView({ model: msg });
         var item = msgView.render().el;
         $(this.el).append(item);
         // Messages from db have positive IDs and recently sent messages have negative messages
         if (performFadeIn) {
            $(item).hide().fadeIn("2000");
         }
         //var helperDiv = $(this).find("div")[0];
         //$(helperDiv).css)
         if (scrollToBottomParam) {
            var messagesEl = $("#scrollablemessagebox");
            messagesEl.animate({ scrollTop: messagesEl.prop("scrollHeight") }, 3000);
         }
      },
      /* Change the WasSuccessfullySent attribute */
      messageSuccessfullySent: function (message) {
         setMsgWasSuccessfullySentValue(message.convID, message.msgID, true);
      },
      setAcknowledgeFromClient: function (message) {
         setMsgClientAcknowledgeValue(message.convID, message.msgID, true);
      }
   });
   this.messagesView = new MessagesView();

   $(document).bind('conversationSelected', function (ev, data) {
      self.messagesView.getMessages(data.convID);
   });
   // The attachment of the handler for this type of event is done only once
}



