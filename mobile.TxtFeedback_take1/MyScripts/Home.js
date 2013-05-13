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
window.app.historyLoaded = false;

//#region Helpers
function getMessagePositionInRepository(convID, msgID) {
    var messagePosition = -1;
    if (window.app.globalMessagesRep[convID] !== null && window.app.globalMessagesRep[convID] !== undefined) {
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

function getConversationHistory(msgView, converstionID) {
   $.getJSON("/Messages/GetConversationHistory",
        {
           convId: converstionID,
           top: 4
        },
        function success(data, textResponse, xhr) {
            msgView.addHistoryMessages(data);
            window.app.historyLoaded = true;
            _.each(window.app.highlightMessageQueue, function (value) {
                msgView.highlightMessage(value.msgId, value.convId);
            });
        });
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
        ClientAcknowledge: false,
        IsBusyMessage: false,
        History: false
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

//#region MessageView 
window.app.MessageView = Backbone.View.extend({
    model: window.app.Message,
    tagName: "div",
    initialize: function () {
        _.bindAll(this, 'render', "highlight");
        this.messageTemplate = _.template($('#message-template').html());
        this.model.on("change:WasSuccessfullySent", this.render);
        this.model.on("change:ClientAcknowledge", this.render);
        this.model.on("highlight", this.highlight);
        /* 
        - even number of highlight let the display at the
        end of the animation in the initial state.
        - odd number let in the opposite state. (normal || grayed out)
        */
        this.noOfHighlights = 12; 
        return this.render;
    },
    render: function () {
        this.$el.html(this.messageTemplate(this.model.toJSON()));
        var direction = "messagefrom";
        if (this.model.attributes.Direction === "to") {
            direction = "messageto";
        }
        if (this.model.get("History"))  this.$el.addClass("history");
        this.$el.addClass("message");
        this.$el.addClass(direction);
        this.$el.attr("id", this.model.get("Id"));
        var messageId = this.model.get("Id");
        return this;
    }, 
    highlight: function() {
        var self = this;
        var interval = setInterval(function () {
            if (self.noOfHighlights > 0) {
                self.$el.css("opacity",
                    self.$el.css("opacity") < 1 ? 1 : 0.4)
                self.noOfHighlights-=1;
            } else {
                clearInterval(interval);
            }
        },500);
    }
});
//#endregion

//#region BusyMessageView
window.app.BusyMessageView = Backbone.View.extend({
    model: window.app.Message,
    tagName: "div",
    initialize: function () {
        _.bindAll(this, 'render');
        this.messageTemplate = _.template($('#busy-message-template').html());      
        return this.render;
    },
    render: function () {
        this.$el.html(this.messageTemplate(this.model.toJSON()));     
        this.$el.attr("id", this.model.get("Id"));
        var messageId = this.model.get("Id");
        return this;
    }
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

window.app.busyMessageTimer = {};
window.app.intervalAfterWhichToShowBusy = {};//default value
function showBusyMessageIfRequired() {
    window.app.msgView.messagesView.addBusyMessage();
}
window.app.startBusyMessageTimer = function () {
    clearTimeout(window.app.busyMessageTimer);
    if (_.isEmpty(window.app.intervalAfterWhichToShowBusy)) {
        window.app.intervalAfterWhichToShowBusy = parseInt($("#busyTimerValue").text(), 10);
    }
    window.app.busyMessageTimer = setTimeout(showBusyMessageIfRequired, window.app.intervalAfterWhichToShowBusy);
};

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
        //remove busy message
        window.app.msgView.messagesView.removeBusyMessage();
        //signal all the other "listeners/agents"
        window.app.xmppHandlerInstance.send_reply(from, to, timeSent, convID, msgContent, window.app.suffixedMessageModeratorAddress, id);
        //once the message has been the "busy timer kicks in"
        window.app.startBusyMessageTimer();
    }
};
window.app.messageID = 412342;
window.app.busyMessage = {};
window.app.busyMessageID = -1;

window.app.MessagesArea = function () {
    "use strict";
    var self = this;

    $.extend(this, window.app.defaultMessagesOptions);

    $("#reply").click(function () {
        window.app.sendMessageToClient(generateUUID(), self.currentConversationId);
    });
    $("#replyText").keydown(function (event) {
        if (event.which === 13) {
            event.preventDefault();
            window.app.sendMessageToClient(generateUUID(), self.currentConversationId);
        }
    });


    _.templateSettings = {
        interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
        evaluate: /\{%([\s\S]+?)%\}/g,   // execute code: {% code_to_execute %}
        escape: /\{%-([\s\S]+?)%\}/g
    }; // excape HTML: {%- <script> %} prints &lt

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
               "addBusyMessage",
               "removeBusyMessage");// to solve the this issue
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
            messages.bind('remove', this.removeMessage);
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
                Starred: false,
                Id: generateUUID()
            });        
            messages.add(msgWelcome);                      
        },
        addBusyMessage: function () {         
            if (_.isEmpty(window.app.busyMessage)) {
                window.app.busyMessage = new window.app.Message({
                    From: window.app.defaultTo,
                    To: window.app.defaultFrom,
                    Text: $("#busyMessage").text(),
                    TimeReceived: new Date(),
                    ConvID: self.currentConversationId,
                    Direction: "from",
                    Read: false,
                    Starred: false,
                    Id: window.app.busyMessageID,
                    IsBusyMessage: true
                });
            }         
            this.removeBusyMessage();
            window.app.globalMessagesRep[self.currentConversationId].add(window.app.busyMessage);
        },
        removeBusyMessage:function() {
            window.app.globalMessagesRep[self.currentConversationId].remove(window.app.busyMessage);
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
        removeMessage: function( msg) {
            $('div[id='+msg.get("Id")+']').fadeOut('slow').remove();
        },
        newMessageReceived: function (fromID, convID, msgID, dateReceived, text) {
            clearTimeout(window.app.busyMessageTimer);
            window.app.msgView.messagesView.removeBusyMessage();
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
            var msgView;
            if(msg.get("IsBusyMessage") === false) {
                msgView = new window.app.MessageView({ model: msg });
            }
            else {
                msgView = new window.app.BusyMessageView({ model: msg });
            }
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
        },
        addHistoryMessages: function (messages) {
            _.each(messages, function (value, index, list) {
                var newMsg = new window.app.Message({ Id: value.Id });
                //decide if this is a from or to message
                var fromTo = getFromToFromConversation(value.ConvId);
                var selfTelNo = fromTo[0];
                var direction = "to";
                if (!comparePhoneNumbers(value.From, selfTelNo)) {
                    direction = "from";
                }
                newMsg.set("Direction", direction);
                newMsg.set("From", value.From);
                newMsg.set("ConvID", value.ConvId);
                newMsg.set("Text", value.Text);
                // TODO MB: Check TimeReceived conversion
                newMsg.set("TimeReceived", new Date(Date.parse(value.TimeReceived)));
                newMsg.set("History", true);

                //we add the message only if are in correct conversation
                if (window.app.globalMessagesRep[value.ConvId] !== undefined) {
                    window.app.globalMessagesRep[value.ConvId].models.splice(0, 0, newMsg);
                }
            });
            if (messages.length !== 0) { window.app.globalMessagesRep[messages[0].ConvId].trigger("reset"); }
        },
        highlightMessage: function (msgId, convId) {
            /* TODO MB: Implement backbone collection.get(id). Now it doesn't work
            my guess is because the history messages are not added in collection 
            using add, the collection doesn't know about them. Collection length
            currently is 1.
            */
            if (window.app.globalMessagesRep[convId] != undefined) {
                _.each(window.app.globalMessagesRep[convId].models, function (model) {
                    if (model.get("Id") == msgId) model.trigger("highlight");
                });
                //alert(window.app.globalMessagesRep[convId].models.length);
            }
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

        $(document).bind("getHistory", function (ev, data) {
            getConversationHistory(window.app.msgView.messagesView, data.conversationID);
        });

        $(document).bind("highlightMessage", function (ev, data) {
            window.app.msgView.messagesView.highlightMessage(data.msgId, data.convId);
        });

        //DA unfortunately Opera does not implement correctly onUnload - so this will not be triggered when closing opera
        $(window).unload(function () {            
            window.app.saveLoginDetails();
            window.app.disconnectXMPP();
        });
        window.app.loadLoginDetails();
        window.app.startReconnectTimer();      
    }
    
    $("#replyText").click(function () {
        if ($("#replyText").is(":focus")) {
            $("#footer").css("position", "relative");
        }
    });
   
    $("#replyText").blur(function () {
        $("#footer").css("position", "fixed");
    });  
});



