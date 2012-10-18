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
//#endregion
window.app = window.app || {};
window.app.globalMessagesRep = {};
window.app.receivedMsgID = 12345;
var gSelectedMessage = null;
var gSelectedConversationID = null;
var gSelectedElement = null;

var timer; //this will be responsible for triggering the "mark conversation as read event"
var timer_is_on = 0;

//#region Mark Conversation as Read function
function markConversationAsRead()
{
    $(gSelectedElement).removeClass("unreadconversation");
    $(gSelectedElement).addClass("readconversation");
    window.app.selectedConversation.set({"Read": true});
    //call the server to mark the conversation as read
    $.getJSON('Messages/MarkConversationAsRead',
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
      IsSmsBased: false
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

//#region Receive message
//TODO DA move this somewhere else :)
window.app.handleIncommingMessage = function (msgContent, isIncomming) {
   window.app.receivedMsgID++;
   var xmlDoc;
   if (window.DOMParser) {
      var parser = new DOMParser();
      xmlDoc = parser.parseFromString(msgContent, "text/xml");
   }
   else {
      xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
      xmlDoc.async = false;
      xmlDoc.loadXML(msgContent);
   }
   var xmlMsgToBeDecoded = xmlDoc.getElementsByTagName("msg")[0];
   var rawFromID = xmlMsgToBeDecoded.getElementsByTagName('from')[0].textContent;
   var rawToID = xmlMsgToBeDecoded.getElementsByTagName('to')[0].textContent;
   var toID = cleanupPhoneNumber(rawToID);
   var fromID = cleanupPhoneNumber(rawFromID);
   var extension;
   /*
   DA: the following line seems weird and it actually is :)
   Right now a Working Point XMPP address is shortID@moderator.txtfeedback.net
   In order not to hard code the @ prefix we try to retrieve it from SuffixDictionary
   The issue is that the WP's address might be the from address or the to address (depending on different factors)
   But for sure the WP is either the to or the from -> we will find it in the suffix dictionary
   To avoid complicated logic we test both from and to in the suffix dictionary and one of them will hit :)
   */
   extension = window.app.workingPointsSuffixDictionary[toID] || window.app.workingPointsSuffixDictionary[fromID];
   //decide if we are dealing with a message coming from another WorkingPoint
   var isFromWorkingPoint = isWorkingPoint(rawFromID, extension);
   var dateReceived = xmlMsgToBeDecoded.getElementsByTagName('datesent')[0].textContent;
   var isSmsBasedAsString = xmlMsgToBeDecoded.getElementsByTagName('sms')[0].textContent;
   var isSmsBased = false;
   if (isSmsBasedAsString === "true") {
      isSmsBased = true;
   }
   var convID;
   if (isFromWorkingPoint && isIncomming) {
      convID = buildConversationID(fromID, toID);
   } else {
      convID = xmlMsgToBeDecoded.getElementsByTagName("convID")[0].textContent;
   }

   var newText = xmlMsgToBeDecoded.getElementsByTagName("body")[0].textContent;
   var readStatus = false; //one "freshly received" message is always unread
   window.app.receivedMsgID++;
   $(document).trigger('msgReceived', {
      fromID: fromID,
      toID: toID,
      convID: convID,
      msgID: window.app.receivedMsgID,
      dateReceived: dateReceived,
      text: newText,
      readStatus: readStatus,
      isSmsBased: isSmsBased
   });

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
   //set the filter to make on the top div (conversation) selectable
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
            self.messagesView.getMessages(convId);
            self.tagsArea.getTags(convId);
       },
       cancel: ".conversationStarIconImg"
    });

    var id = 412536; //this should be unique
    var sendMessageToClient = function () {
       var inputBox = $("#limitedtextarea");
       id++;
       //add it to the visual list
       //I should set values to all the properties
       var msgContent = inputBox.val();

       /*
       inside a conversationID
       for SMS messages
       from - tel number
       to - self tel number
       for XMPP messages
       from - clientXMPP id without @
       to - selfXMPP id without @
       */
       var fromTo = getFromToFromConversation(self.currentConversationId);
       var from = fromTo[0];
       var to = fromTo[1];
       //decide if this should be handled via SMS or not
       var isSmsBased = window.app.selectedConversation.get("IsSmsBased");
       if (isSmsBased) {
          //send it to the server
         // $.getJSON('Messages/SendMessage',
         //         {
         //            from: to,
         //            to: from,
         //            convId: self.currentConversationId,
         //            text: msgContent
         //         },
         //         function (data) {
         //            //delivered successfully? if yes - indicate this
         //         }
         //);
       }
   
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
          messageIsSent: true,
          isSmsBased: isSmsBased
       });
       //reset the input form
       $("#replyToMessageForm")[0].reset();

       //sendToSupport you send to support or to another Staff web client
       var sendToSupport = window.app.selectedConversation.get("ClientIsSupportBot");

       if (!isSmsBased) {
          if (sendToSupport) {
             //we assume that the TxtFeedback support runs on the same component
             from = from + window.app.workingPointsSuffixDictionary[to];
          } else {
             //TODO @txtfeedback.net is now hardcoded
             from = from + "@txtfeedback.net";
          }
          to = to + window.app.workingPointsSuffixDictionary[to];
       }

       //signal all the other "listeners/agents"       
       var storeStaffAddress = "";
       var txtFeedbackSupportAddress = "";
       if (sendToSupport) {
          /*
          for a support conversation the id is in the form of supportID-storeStaffID, where the from is TxtFeedback support ID and the to is store staff ID
          We need to send a message to TxtFeedback support - which has its own Staff website, so 
          staff has to be true
          to has to be the supportID
          from has to be the storeStaffID
          Note: support conversations goes through XMPP
          */
          txtFeedbackSupportAddress = from;
          storeStaffAddress = to;
          window.app.xmppHandlerInstance.send_reply(storeStaffAddress, txtFeedbackSupportAddress, timeSent, self.currentConversationId, msgContent, txtFeedbackSupportAddress, isSmsBased, sendToSupport);
       } else {
          /*
          we are dealing with a non-support conversation 
          For SMS
          We send a message to trigger carbons
          For XMPP
          We send a message and then the component redirects that message to the client
          */
          var clientToRespondToAddress = from
          storeStaffAddress = to;
          txtFeedbackSupportAddress = self.wpsArea.wpPoolView.phoneNumbersPool.getWorkingPointXmppAddress(cleanupPhoneNumber(to)) || self.wpsArea.wpPoolView.phoneNumbersPool.getWorkingPointXmppAddress(cleanupPhoneNumber(from));          
          window.app.xmppHandlerInstance.send_reply(storeStaffAddress, clientToRespondToAddress, timeSent, self.currentConversationId, msgContent, txtFeedbackSupportAddress, isSmsBased, sendToSupport);
       }

       
    };

    $("#replyBtn").click(function () {
        sendMessageToClient();
    });

    $("#limitedtextarea").keydown(function (event) {
        if (event.which === 13 && event.shiftKey) {
            sendMessageToClient();
            event.preventDefault();
        }
    });   

    _.templateSettings = {
        interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
        evaluate: /\{%([\s\S]+?)%\}/g,   // excute code: {% code_to_execute %}
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
            if (this.model.attributes.Direction === "to") {
               direction = "messageto";                              
               arrowInnerMenuLeft = "arrowInnerRight";
               extraMenuWrapperSide = "extraMenuWrapperRight";
            }            
            this.$el.addClass("message");
            this.$el.addClass(direction);
                   
            $(".innerExtraMenu", this.$el).addClass(arrowInnerMenuLeft);
            $(".extraMenuWrapper", this.$el).addClass(extraMenuWrapperSide);

            var sendEmail = $("div.sendEmailButton img", this.$el);
            setTooltipOnElement(sendEmail, sendEmail.attr('tooltiptitle'),'dark');
            return this;
        },
        updateView: function () {           
           return this;
        }
    });

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
               "newMessageReceived");// to solve the this issue
            this.messages = new window.app.MessagesList();
            this.messages.bind("reset", this.render);
        },
        resetViewToDefault: function () {
           var noConversationLoadedMessage = $("#noConversationSelectedMessage").val();
           $('#messagesbox').html('<span id="noConversationsLoaded">'+noConversationLoadedMessage+'</span>');
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
                //we have already loaded this conversation
                performFadeIn = false;
                spinner.stop();
                startTimer(3000);
                this.render();
                $("#textareaContainer").removeClass("invisible");
                $("textareaContainer").fadeIn("slow");
                $("#tagsContainer").removeClass("invisible");
                $("#tagsContainer").fadeIn("slow");
                //if (window.app.globalMsgHistoryNotLoaded[convID] === true) {
                   //the conversation has no history - load also the history

                //}
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
        newMessageReceived: function (fromID, convID, msgID, dateReceived, text, isSmsBased) {
           //var innerSelf = this;
           var newMsg = new window.app.Message({
              Id: msgID,              
              From: fromID,
              Text: text,
              ConvID: convID,
              ClientDisplayName: fromID,
              ClientIsSupportBot: false,
              IsSmsBased: isSmsBased
           });
            //decide if this is a from or to message
            var fromTo = getFromToFromConversation(convID);
            var from = fromTo[0];
            var direction = "from";
            if (!comparePhoneNumbers(fromID, from)) {
                direction = "to";
            }
            newMsg.set("Direction", direction);            
            //we receive the date as RFC 822 string - we need to convert it to a valid Date
            newMsg.set("TimeReceived", new Date(Date.parse(dateReceived)));            
            //we add the message only if are in correct conversation
            if (window.app.globalMessagesRep[convID] !== undefined) {
               window.app.globalMessagesRep[convID].add(newMsg);
            }
            else {
               //this conversation has not been opened so for, so no history is available
               //window.app.globalMsgHistoryNotLoaded[convID] = true;
               //var msgs = new window.app.MessagesList();
               //msgs.identifier = convID;
               //msgs.bind("reset", innerSelf.render);
               //msgs.bind('add', innerSelf.appendMessage);
               //msgs.add(newMsg);
               //window.app.globalMessagesRep[convID] = msgs;
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
            if (scrollToBottomParam) {
                var messagesEl = $("#scrollablemessagebox");
                messagesEl.animate({ scrollTop: messagesEl.prop("scrollHeight") }, 3000);
            }
        }
    });
       
          
    this.messagesView = new MessagesView();
}

function limitText(limitField, limitCount, limitNum) {
    if (limitField.value.length > limitNum) {
        limitField.value = limitField.value.substring(0, limitNum);
    } else {
        limitCount.value = limitNum - limitField.value.length;
    }
}