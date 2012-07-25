"use strict";
window.app = window.app || {};
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
    app.selectedConversation.set({"Read": true});
    //call the server to mark the conversation as read
    $.getJSON('Messages/MarkConversationAsRead',
                { conversationId: gSelectedConversationID },
                function (data) {
                   //conversation marked as read
                   app.updateNrOfUnreadConversations(false);
                   console.log("MarkConversationAsRead done");
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
      Starred: false
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

//window.app.SendMessageToClient(convView,
function MessagesArea(convView, tagsArea) {
   var self = this;
   $.extend(this, app.defaultMessagesOptions);

   this.convView = convView;
   this.tagsArea = tagsArea;
   //set the filter to make on the top div (conversation) selecteble
  // in the absence of the filter option all elements within the conversation are made "selectable"
   $("#conversations").selectable({
       filter: ".conversation",
       selected: function (event, ui) {           
            //prepare to mark the conversation as read in 3 seconds - once the messages have been loaded
            //for now make sure to other timers are active
            gSelectedElement = ui.selected;
            resetTimer();                        
            var convId = ui.selected.getAttribute("conversationid");
            gSelectedConversationID = convId;
            app.selectedConversation = self.convView.convsList.get(convId);
            self.messagesView.getMessages(convId);
            self.tagsArea.getTags(convId);
       },
       cancel: ".conversationStarIcon"
    });

    var id = 412536; //this should be unique
    var sendMessageToClient = function () {
        var inputBox = $("#limitedtextarea");
        id++;
        //add it to the visual list
        //I should set values to all the properties
        var msgContent = inputBox.val();

        var fromTo = getFromToFromConversation(self.currentConversationId);
        var from = fromTo[0];
        var to = fromTo[1];
       //send it to the server
        $.getJSON('Messages/SendMessage',
                { from: from, to: to, text: msgContent },
                function (data) {
                    //delivered successfully? if yes - indicate this
                 console.log(data);
                 }
       );

        //TODO should be RFC822 format
        var timeSent = (new Date()).toUTCString();
        $(document).trigger('msgReceived', {
           fromID: to,
           toID: from,
           convID: self.currentConversationId,
           msgID: id,
           dateReceived: timeSent,
           text: msgContent,
           readStatus: false
        });       
       //reset the input form
        $("#replyToMessageForm")[0].reset();
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

    /*_.templateSettings = {
        interpolate: /\{\{(.+?)\}\}/g
    };*/

    _.templateSettings = {
        interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
        evaluate: /\{%([\s\S]+?)%\}/g,   // excute code: {% code_to_execute %}
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
            var arrowClass = "arrowFrom";
            var arrowInnerClass = "arrowInnerFrom";
            var arrowInnerMenuLeft = "arrowInnerLeft";
            //var arrowExtraMenu="arrowExtraMenuFrom";
            //var arrowInnerExtraMenu = "arrowInnerExtraMenuFrom";
            if (this.model.attributes["Direction"] === "to") {
               direction = "messageto";
               arrowClass= "arrowTo";
               arrowInnerClass = "arrowInnerTo";
               //arrowExtraMenu = "arrowExtraMenuTo";
               arrowInnerMenuLeft = "arrowInnerRight";
               //arrowInnerExtraMenu = "arrowInnerExtraMenuTo";
            }
            if (this.model.attributes["Starred"] === true)
            {
               $(".favConversation", this.$el).attr('src', app.domainName + "/Content/images/star-selected.svg");
            }
            this.$el.addClass("message");
            this.$el.addClass(direction);
         
            $(".arrow", this.$el).addClass(arrowClass);
            $(".arrowInner", this.$el).addClass(arrowInnerClass);
            $(".innerExtraMenu", this.$el).addClass(arrowInnerMenuLeft);

            $("div.sendEmailButton img", this.$el).qtip({
               content: { text: false },
               style: 'dark'
            });
            //$(".arrowInnerExtraMenu", this.$el).addClass(arrowInnerExtraMenu);
            return this;
        },
        updateView: function () {
           //the Starred attribute has changed (most probably) so reflect this in the view
           if (this.model.attributes["Starred"] === true) {
              $(".favConversation", this.$el).attr('src', app.domainName + "/Content/images/star-selected.svg");
           }
           else {
              $(".favConversation", this.$el).attr('src', app.domainName + "/Content/images/star.svg");
           }                  
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
            this.messages = new app.MessagesList();
            this.messages.bind("reset", this.render);
            //$("#messagesbox").selectable();
        },
        resetViewToDefault: function () {
           $('#messagesbox').html(' No conversation selected, please select one');
           $("#textareaContainer").addClass("invisible");
           $("#tagsContainer").addClass("invisible");
           self.currentConversationId = '';
           //$("textareaContainer").hide("slow");
        },
        getMessages: function (conversationId) {
            console.log("getting conversations with id:" + conversationId);
            $("#messagesbox").html('');
            var target = document.getElementById('scrollablemessagebox');
            spinner.spin(target);

            self.currentConversationId = conversationId;
            if (self.currentConversationId in app.globalMessagesRep) {
                //we have already loaded this conversation
                performFadeIn = false;
                spinner.stop();
                startTimer(3000);
                this.render();
                $("#textareaContainer").removeClass("invisible");
                $("textareaContainer").fadeIn("slow");
                $("#tagsContainer").removeClass("invisible");
                $("#tagsContainer").fadeIn("slow");
                //$("#textareaContainer").addClass("visible");
                
            }
            else {
                var messages1 = new app.MessagesList();
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
                app.globalMessagesRep[self.currentConversationId] = messages1;
                $.each(messages1, function (index, value) {
                    value.set("Starred", app.selectedConversation.get("Starred"));
                });
            }
            
        },
        render: function () {           
            $("#messagesbox").html('');
            var selfMessageView = this;
            app.globalMessagesRep[self.currentConversationId].each(function (msg) {
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
              console.log("Adding new message: " + msg.get("Text"));
              //when appending a new message always scroll to bottom
              this.appendMessageToDiv(msg, true, true);
            
           }
        },
        newMessageReceived: function (fromID, convID, msgID, dateReceived, text) {
            console.log("new message received: " + text + " with ID:" + msgID);
            var newMsg = new app.Message({ Id: msgID });
           //decide if this is a from or to message
            var fromTo = getFromToFromConversation(self.currentConversationId);
            var from = fromTo[0];
            var direction = "from";
            if (!comparePhoneNumbers(fromID,from)) {
               direction = "to";
            }
            newMsg.set("Direction", direction);
            newMsg.set("From", fromID);
            newMsg.set("ConvID", convID);
            newMsg.set("Text", text);
            newMsg.set("TimeReceived", dateReceived);            
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
            if (scrollToBottomParam) {
                var messagesEl = $("#scrollablemessagebox");
                messagesEl.animate({ scrollTop: messagesEl.prop("scrollHeight") }, 3000);
             }
          
        }
     });

    $(".favConversation").live("click", function (e) {
       e.preventDefault();
       //signal to the server that this conversation's starred status has changed
       $.getJSON('Messages/ChangeStarredStatusForConversation',
                { conversationId: self.currentConversationId, newStarredStatus: newStarredStatus },
                function (data) {
                   //conversation starred status changed
                   console.log(data);
                });
       //reflect the change visually
        var newStarredStatus = false;
        app.globalMessagesRep[self.currentConversationId].each(function (msg) {
           //var newStarredValue = ;
           msg.set("Starred", !msg.attributes["Starred"]);
           newStarredStatus = msg.attributes["Starred"];
        });
        app.selectedConversation.set("Starred", newStarredStatus);
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