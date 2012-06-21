"use strict";
var gSelectedMessage = null;
var gSelectedConversationID = null;
var gSelectedElement = null;

var timer; //this will be responsible for triggering the "mark conversation as read event"
var timer_is_on = 0;

function markConversationAsRead()
{
    $(gSelectedElement.selected).removeClass("unreadconversation");
    $(gSelectedElement.selected).addClass("readconversation");
    //call the server to mark the conversation as read
    $.getJSON('Messages/MarkConversationAsRead',
                { conversationId: gSelectedConversationID },
                function (data) {
                    //conversation marked as read
                    console.log(data);
                }
        );
}

function resetTimer() {
    if (timer_is_on) {
        clearTimeout(timer);
        timer_is_on = false;
    }
}

function startTimer(intervalToWait) {
    if (!timer_is_on) {       
        //establish if any action is still required - maybe the conversation is already read
        if (!$(gSelectedElement.selected).hasClass("readconversation")) {
            timer = setTimeout(markConversationAsRead, intervalToWait);
            timer_is_on = true;
        }
    }
}

function MessagesArea(convView, tagsArea) {
    $("#conversations").selectable({
        selected: function (event, ui) {
            //prepare to mark the conversation as read in 3 seconds - once the messages have been loaded
            //for now make sure to other timers are active
            gSelectedElement = ui;
            resetTimer();
            
            var convId = ui.selected.getAttribute("conversationid");
            gSelectedConversationID = convId;
            appview.getMessages(convId);
            tagsArea.getTags(convId);
            
        }
    });

    var id = 12344; //this should be unique
    var sendMessageToClient = function () {
        var inputBox = $("#limitedtextarea");
        var newMsg = new Message({ Id: id });        
        id++;
        //add it to the visual list

        //I should set values to all the properties
        var msgContent = inputBox.val();
        newMsg.set("Direction", "to");
        newMsg.set("Text", msgContent);

        var fromTo = getFromToFromConversation(appview.currentConversationId);
        var from = fromTo[1];
        var to = fromTo[0];
        newMsg.set("From", from);
        newMsg.set("To", to);        
        newMsg.set("TimeReceived", (new Date()).toUTCString());
        appview.messagesRep[appview.currentConversationId].add(newMsg);
       //TODO - here TO is incorrect, as it should be the description 
        convView.newMessageReceived(from, to, appview.currentConversationId, Date.now(), msgContent);
        //reset the input form
        $("#replyToMessageForm")[0].reset();
        //send it to the server
        $.getJSON('Messages/SendMessage',
                { from: from, to: to, text: msgContent },
                function (data) {
                    //delivered successfully? if yes - indicate this
                 console.log(data);
                 }
        );
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

    var Message = Backbone.Model.extend({
        defaults: {
            From: "0752345678",
            To: "0751569435",
            Text: "Hey you",            
            //DateTimeInTicks: (new Date()).valueOf(),
            TimeReceived: null,            
            ConvID: 1,
            Direction: "from",
            Read: false            
        },
        parse: function (data, xhc) {
            //a small hack: the TimeReceived will be something like: "\/Date(1335790178707)\/" which is not something we can work with
            //in the TimeReceived property we have the same info coded as ticks, so we replace the TimeReceived value with a value build from the ticks value
            var dateInTicks = data.TimeReceived.substring(6,19);
            data.TimeReceived = (new Date(parseInt(dateInTicks, 10))).toUTCString();
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

    var MessagesList = Backbone.Collection.extend({
        model: Message,
        identifier: null,
        url: function () {
            return "Messages/MessagesList";
        }
    });

    _.templateSettings = {
        interpolate: /\{\{(.+?)\}\}/g
    };

    var MessageView = Backbone.View.extend({
        model: Message,
        tagName: "div",
        messageTemplate: _.template($('#message-template').html()),        
        initialize: function () {
            _.bindAll(this, 'render');
            return this.render;
        },
        render: function () {
            this.$el.html(this.messageTemplate(this.model.toJSON()));
            var direction = "messagefrom";
            var arrowClass = "arrowFrom";
            var arrowInnerClass = "arrowInnerFrom";
            if (this.model.attributes["Direction"] == "to") {
               direction = "messageto";
               arrowClass= "arrowTo";
                arrowInnerClass = "arrowInnerTo";
            }
            this.$el.addClass("message");
            this.$el.addClass(direction);
            
            $(".arrow", this.$el).addClass(arrowClass);
            $(".arrowInner", this.$el).addClass(arrowInnerClass);
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
           _.bindAll(this, "render", "getMessages", "appendMessage", "appendMessageToDiv", "resetViewToDefault");// to solve the this issue
            this.messagesRep = {};
            this.currentConversationId = '';
            this.messages = new MessagesList();
            this.messages.bind("reset", this.render);
            //$("#messagesbox").selectable();
        },
        resetViewToDefault: function () {
           $('#messagesbox').html(' No conversation selected, please select one');
           $("#textareaContainer").addClass("invisible");
           //$("textareaContainer").hide("slow");
        },
        getMessages: function (conversationId) {
            console.log("getting conversations with id:" + conversationId);
            $("#messagesbox").html('');
            var target = document.getElementById('scrollablemessagebox');
            spinner.spin(target);
            
            this.currentConversationId = conversationId;
            if (this.currentConversationId in this.messagesRep) {
                //we have already loaded this conversation
                performFadeIn = false;
                spinner.stop();
                startTimer(3000);
                this.render();
                $("#textareaContainer").removeClass("invisible");
                //$("#textareaContainer").addClass("visible");
                $("textareaContainer").fadeIn("slow");
            }
            else {
                var messages1 = new MessagesList();
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
                        //$("#textareaContainer").addClass("visible");
                        $("textareaContainer").fadeIn("slow");
                    }
                });
                this.messagesRep[this.currentConversationId] = messages1;
            }
        },
        render: function () {           
            $("#messagesbox").html('');
            var self = this;
            this.messagesRep[this.currentConversationId].each(function (msg) {
                //don't scroll to bottom as we will do it when loading is done
                self.appendMessageToDiv(msg, performFadeIn, false);
             });
            spinner.stop();
            //scroll to bottom
            var messagesEl = $("#scrollablemessagebox");
            messagesEl.animate({ scrollTop: messagesEl.prop("scrollHeight") }, 3000);
        
            return this;
        },
        appendMessage: function (msg) {
            console.log("Adding new message " + msg.get("Text"));
            //when appending a new message always scroll to bottom
            this.appendMessageToDiv(msg, true, true);
        },
        newMessageReceived: function (fromID, convID, msgID, dateReceived, text ) {
            var newMsg = new Message({ ID: msgID });            
            newMsg.set("Direction", "from");
            newMsg.set("From", fromID);
            newMsg.set("ConvID", convID);
            newMsg.set("Text", text);
            newMsg.set("TimeReceived", dateReceived);            
            //we add the message only if are in correct conversation            
            appview.messagesRep[convID].add(newMsg);
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
                $(helperDiv).fadeIn(400);
                //ContactWindow.init();
            }, function () {
                var helperDiv = $(this).find("div.extramenu")[0];
                $(helperDiv).fadeOut("fast");
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

    var appview = new MessagesView();
    return appview;
}

function limitText(limitField, limitCount, limitNum) {
    if (limitField.value.length > limitNum) {
        limitField.value = limitField.value.substring(0, limitNum);
    } else {
        limitCount.value = limitNum - limitField.value.length;
    }
}