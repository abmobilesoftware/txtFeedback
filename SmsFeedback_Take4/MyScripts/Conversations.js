var defaultNrOfConversationsToDisplay = 10;
var cummulativeSkip = defaultNrOfConversationsToDisplay;

function ConversationArea() {
    //define the models
    var Conversation = Backbone.Model.extend({
        defaults: {
            ConvID: 1,
            TimeUpdated: Date.now(),         
            Read: false,
            Text: "some data",
            From: "defaultNumber",
            To: "defaultRecipient"
        },
        parse: function (data, xhc) {
            //a small hack: the TimeReceived will be something like: "\/Date(1335790178707)\/" which is not something we can work with
            //in the DateTimeInTicks property we have the same info coded as ticks, so we replace the TimeReceived value with a value build from the ticks value
            //data["TimeReceived"] = (new Date(data["DateTimeInTicks"])).toUTCString();
            var unTrimmedText = data["Text"];
            if (unTrimmedText.length > 40) {
                unTrimmedText = unTrimmedText.substring(0, 37) + '...';
            }
            data["Text"] = unTrimmedText //it is up to the model to make sure that we use only 40 characters            
            return data;
        },
        idAttribute: "ConvID" //the id shold be the combination from-to
    });

    var ConversationsList = Backbone.Collection.extend({
        model: Conversation,
        convID: null,
        parse: function (responce) {
            var temp = responce;
            return responce;
        },
        url: function () {
            return "Messages/ConversationsList";
        }
        //url: function () {
        //    return "Services/Conversations.svc/GetConversationsList";
        //}
    });

    _.templateSettings = {
        interpolate: /\{\{(.+?)\}\}/g
    };

    var ConversationView = Backbone.View.extend({
        model: Conversation,
        tagName: "div",
        conversationTemplate: _.template($('#conversation-template').html()),
        initialize: function () {
            _.bindAll(this, 'render');
            return this.render;
        },
        render: function () {
            this.$el.html(this.conversationTemplate(this.model.toJSON()));
            var readUnread = "unreadconversation";
            if (this.model.attributes["Read"] == true) readUnread = "readconversation";
            this.$el.addClass("conversation");
            this.$el.addClass(readUnread);
            $(this.el).attr("conversationId", this.model.attributes["ConvID"]);
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

    var optsForLoadMoreConversationsSpinner = {
       lines: 13, // The number of lines to draw
       length: 7, // The length of each line
       width: 4, // The line thickness
       radius: 4, // The radius of the inner circle
       rotate: 0, // The rotation offset
       color: '#fff', // #rgb or #rrggbb
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

    var spinnerAddConvs = new Spinner(optsForLoadMoreConversationsSpinner);
    var ConversationsView = Backbone.View.extend({
        el: $("#conversations"),
        initialize: function () {
            _.bindAll(this, "render", "getConversations", "addConversationBasicEffect", "updateConversation", "removeConversation", "addConversationWithEffect");
            this.convsList = new ConversationsList();
            this.convsList.bind("reset", this.render);
            this.convsList.bind("add", this.addConversationWithEffect, this);
            this.selectedWorkingPoints = [];
            // this.convsList.change("change", this.updatedConversation, this);
            this.convsList.bind("remove", this.removeConversation, this);
            $("#conversations").selectable();
            // create an array of views to keep track of children
            this._convViews = [];
            //by default conversations are "new"
            this.addConversationAsNewElement = true;
        },
        getConversations: function (workingPoints) {           
           var target = document.getElementById('scrollableconversations');
           $('#loadMoreConversations').hide();
           $('#conversations').html('');           
            spinner.spin(target);
            var selectedTags = ["complaint", "praise", "electronics"];
            var workingPointsNumbers;
            if (workingPoints == null) {
               workingPointsNumbers = []; //["0451215454", "4554245454"];
            }
            else {
               workingPointsNumbers = workingPoints;
               this.selectedWorkingPoints = workingPoints;
            }
           //reset the cummulative skip because we start with a "fresh" view
            cummulativeSkip = defaultNrOfConversationsToDisplay;

            var top = defaultNrOfConversationsToDisplay;
            var skip = 0;
            this.convsList.fetch({
                data: { "showAll": true, "showFavourites": false, "tags": selectedTags, "workingPointsNumbers": workingPointsNumbers, "skip": skip, "top":top },
                traditional: true,
                success: function () {                    
                    spinner.stop();
                }
            });
        },
        getAdditionalConversations: function () {
           //the spinner button should only affect the LoadMoreConversationsDiv
           var target = document.getElementById('loadMoreConversations');
           $(target).removeClass("readable");
           $(target).addClass("unreadable");
           spinnerAddConvs.spin(target);
           var selectedTags = ["complaint", "praise", "electronics"];
           var workingPointsNumbers = this.selectedWorkingPoints;
           var top = defaultNrOfConversationsToDisplay;
           var skip = cummulativeSkip;
           cummulativeSkip = cummulativeSkip + defaultNrOfConversationsToDisplay;          
           //add these "old" conversations to the end
           var self = this;
           $.ajax({
              url: "Messages/ConversationsList",
              data: { "showAll": true, "showFavourites": false, "tags": selectedTags, "workingPointsNumbers": workingPointsNumbers, "skip": skip, "top": top },
              traditional: true,
              success: function (data) {                 
                 spinnerAddConvs.stop();
                 $(target).removeClass("unreadable");
                 $(target).addClass("readable");
                
                 $.each(data, function () {
                    var conv = new Conversation({ From: $(this).attr("From"), ConvID: $(this).attr("ConvID"), TimeReceived: $(this).attr("TimeReceived"), Text: $(this).attr("Text"), Read: $(this).attr("Read"), To: $(this).attr('To') });
                    self.addConversationBasicEffect(conv, false);
                 });
                 if (data.length == 0 || data.length < defaultNrOfConversationsToDisplay) {
                    $(target).hide('slow');
                 }
              }
           }
       );          
        },
        render: function () {
            // We keep track of the rendered state of the view
            this._rendered = true;
           
            var convEl = $("#conversations");
            convEl.html('');
            var self = this;
            this.convsList.each(function (conv) {
                self.addConversationBasicEffect(conv);
            });
        },
        addConversationWithEffect: function (conv, addConversationAsNewElement) {
            if (addConversationAsNewElement == null) {
                addConversationAsNewElement = true;
            }
            var item = this.addConversationNoEffect(conv, addConversationAsNewElement);
            var timer = 300;
            $(item).hide().fadeIn(timer).fadeOut(timer).fadeIn(timer).fadeOut(timer).fadeIn(timer).fadeOut(timer).fadeIn(timer);            
        },
        addConversationBasicEffect: function (conv, addConversationAsNewElement) {
            if (addConversationAsNewElement == null) {
                addConversationAsNewElement = true;
            }
            var item = this.addConversationNoEffect(conv, addConversationAsNewElement);
            $(item).hide().fadeIn("slow");
        },
        addConversationNoEffect: function (conv, addConversationAsNewElement) {
            var convView = new ConversationView({ model: conv });
            this._convViews.push(convView);
            var item = convView.render().el;
            if (addConversationAsNewElement) {
                $(this.el).prepend(item);
            }
            else {
                $(this.el).append(item);
            }            
            var self = this;
            conv.on("change", function (model) {
                self.updateConversation(model);
            });
            //if we are displaying more then 10 conversations then prepare to 
            if (this.convsList.models.length >= defaultNrOfConversationsToDisplay) {
                $("#loadMoreConversations").show('slow');
            }
            return item;
        },
        updateConversation: function (conversation) {
            //when we get an update the conversation will move to the top of the list
            var self = this;            
            var viewToRemove = _(this._convViews).select(function (cv) {
                return cv.model.get("ConvID") == conversation.get("ConvID");
            })[0];                
            this._convViews = _(this._convViews).without(viewToRemove);
            if (this._rendered) {
                var elem = $(viewToRemove.el);
                elem.fadeOut("slow", function () {
                    elem.remove();
                    //make sure to clear any event handlers, so we don't handle the same event twice
                    conversation.off("change");
                    self.addConversationWithEffect(conversation);
                });
            }            
        },
        newMessageReceived: function (fromID, convID, dateReceived, newText) {
            //if the given conversation exists we update it, otherwise we create a new conversation
            if (newText.length > 40) {
                newText = newText.substring(0, 37) + '...';
            } //make sure that we display max 40 characters

            var modelToUpdate = convView.convsList.get(convID);
            if (modelToUpdate) {
                //make sure that the text does not exceed 40 characters
                modelToUpdate.set("Text", newText);
            }
            else {                
                var modelToAdd = new Conversation({From : fromID, ConvID: convID, TimeReceived: dateReceived, Text: newText });
                //model.id = assign unique id
                convView.convsList.add(modelToAdd);
            }
        },
        removeConversation: function (conv) {
            //select the correct view based on the model
            var viewToRemove = _(this._convViews).select(function (cv) {
                return cv.model.get("ConvID") == conv.get("ConvID");
            })[0];
            this._convViews = _(this._convViews).without(viewToRemove);
            if (this._rendered) {
                var elem = $(viewToRemove.el);
                elem.fadeOut("slow", function () {
                    elem.remove();
                });                
            }
        },
        filterConversations: function (convID, slideUp) {
            var viewToFilter = _(this._convViews).select(function (cv) {
                return cv.model.get("ConvID") == convID;
            })[0];
            if (viewToFilter) {
                if (this._rendered) {
                    var elem = $(viewToFilter.el);
                    if (slideUp) {
                        elem.slideUp();
                    }
                    else {
                        elem.slideDown();
                    }

                }
            }
        }
    })
    
    $("#removeMessage").bind("click", function () {
        var id = '0754654213-0745103618';
        var modelToRemove = convView.convsList.get(id);
        convView.convsList.remove(modelToRemove);
    });

    $("#btnSlideUp").bind("click", function () {
        var id = '0754654213-0745103618';
        convView.filterConversations(id, true);        
    });
    $("#btnSlideDown").bind("click", function () {
        var id = '0754654213-0745103618';
        convView.filterConversations(id, false);
    });
    $("#loadMoreConversations").bind("click", function () {
        convView.getAdditionalConversations();
    });

    var convView = new ConversationsView();   
    return convView;
};