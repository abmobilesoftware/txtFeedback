function MessagesArea() {
    $("#conversations").selectable({
        selected: function (event, ui) {            
            $(ui.selected).removeClass("unreadconversation");            
            $(ui.selected).addClass("readconversation");
            appview.getMessages(ui.selected.getAttribute("conversationid"));

            $("#textareaContainer").removeClass("invisible");
            $("#textareaContainer").addClass("visible");
        }
    });

   //$(document).bind('messageReceived', function (ev, data) {
    //    var fromID = data.id;
    //    //var convID = data.id-;
    //    var newMsg = new Message({ ID: id });
    //    id++;
    //    newMsg.set("Direction", "from");
    //    newMsg.set("From", fromID);
    //    newMsg.set("ConvID", convID);
    //    newMsg.set("Text", "And another thing, I don't like your colors");
    //    //we add the message only if are in correct conversation
    //    if (appview.currentConversationId == convID) {
    //        appview.messagesRep[appview.currentConversationId].add(newMsg);
    //    }
    //});
    var id = 12344; //this should be unique
    var sendMessageToClient = function () {
        var inputBox = $("#limitedtextarea");
        var newMsg = new Message({ ID: id });
        id++;
        //add it to the visual list
        //I should set values to all the properties
        newMsg.set("Direction", "to");
        newMsg.set("Text", inputBox.val());        
        appview.messagesRep[appview.currentConversationId].add(newMsg);
        //reset the input form
        $("form[name=replyToMessageForm]")[0].reset();
        //send it to the server
        $.getJSON('Messages/SendMessage',
                { from: "04154521542", to: "5454645464", text: "test data" },
                function (data) {
            console.log(data);
        }
        );
    };

    $("#replyBtn").click(function () {
        sendMessageToClient();
    });

    $("#limitedtextarea").keydown(function (event) {
        if (event.which == 13 && event.shiftKey) {
            sendMessageToClient();
            event.preventDefault();
        }
    });

    var Message = Backbone.Model.extend({
        defaults: {
            From: "0752345678",
            To: "0751569435",
            Text: "Hey you",            
            DateTimeInTicks: (new Date()).valueOf(),
            TimeReceived: null,
            ID: 1,
            ConvID: 1,
            Direction: "from",
            ReadStatus: "notread",
            TrimmedText: "some data"
        },
        parse: function (data, xhc) {
            //a small hack: the TimeReceived will be something like: "\/Date(1335790178707)\/" which is not something we can work with
            //in the DateTimeInTicks property we have the same info coded as ticks, so we replace the TimeReceived value with a value build from the ticks value
            data["TimeReceived"] = (new Date(data["DateTimeInTicks"])).toUTCString();
            return data;
        }        
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
            if (this.model.attributes["Direction"] == "to") {
                direction = "messageto";
            }
            this.$el.addClass("message");
            this.$el.addClass(direction);
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
            _.bindAll(this, "render", "getMessages","appendMessage","appendMessageToDiv");// to solve the this issue
            this.messagesRep = {};
            this.currentConversationId = '';
            this.messages = new MessagesList();
            this.messages.bind("reset", this.render);
            $("#messagesbox").selectable();
        },
        getMessages: function (conversationId) {
            $("#messagesbox").html('');
            var target = document.getElementById('scrollablemessagebox');
            spinner.spin(target);
            var messages1 = new MessagesList()
            messages1.identifier = conversationId;
            this.currentConversationId = conversationId;
            if (this.currentConversationId in this.messagesRep) {
                //we have already loaded this conversation
                performFadeIn = false;
                spinner.stop();
                this.render();
            }
            else {
                messages1.bind("reset", this.render);
                messages1.bind('add', this.appendMessage)
                performFadeIn = true;
                messages1.fetch({
                    data: { "conversationId": messages1.identifier },
                    success: function () {
                        spinner.stop();
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
            //scroll to bottom
            var messagesEl = $("#scrollablemessagebox");
            messagesEl.animate({ scrollTop: messagesEl.prop("scrollHeight") }, 3000);
            spinner.stop();
            return this;
        },
        appendMessage: function (msg) {
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
            if (appview.currentConversationId == convID) {
                appview.messagesRep[appview.currentConversationId].add(newMsg);
            }
        },
        appendMessageToDiv: function (msg, performFadeIn, scrollToBottomParam) {
            var msgView = new MessageView({ model: msg });
            var item = msgView.render().el;
            $(this.el).append(item);
            $(item).hover(function () {
                var helperDiv = $(this).find("div.extramenu")[0];
               // $(helperDiv).css("visibility", "visible");
                $(helperDiv).fadeIn(400);
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

    var appview = new MessagesView;
    return appview;
};

//function appendMessageToDiv(msg, performFadeInParam, scrollToBottomParam) {
//    var messagesEl = $("#messagesbox");
//    var direction = "messagefrom";
//    if (msg.get("Direction") == "to") {
//        direction = "messageto";
//    }
//    var item = $('<span class="message ' + direction + '"> ' + msg.get("Text") + '</span>')
//    if (performFadeInParam) {
//        item.hide().fadeIn(2000);
//    }

//    messagesEl.append(item);
//    if (scrollToBottomParam) {
//        var messagesEl = $("#scrollablemessagebox");
//        messagesEl.animate({ scrollTop: messagesEl.prop("scrollHeight") }, 3000);
//    }
//};

function limitText(limitField, limitCount, limitNum) {
    if (limitField.value.length > limitNum) {
        limitField.value = limitField.value.substring(0, limitNum);
    } else {
        limitCount.value = limitNum - limitField.value.length;
    }
}
