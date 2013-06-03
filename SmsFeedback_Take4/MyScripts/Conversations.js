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
/*global setTooltipOnElement */
/*global gSelectedElement */
/*global startTimer */
/*global confirm */
/*global resetTimer */
//#endregion
window.app = window.app || {};
window.app.selectedConversation = {};
window.app.isInConversationsTab = true;

//#region Constants
window.app.defaultNrOfConversationsToDisplay = 10;
window.app.defaultNrOfConversationsToSkip = 0;
window.app.cummulativeSkip = window.app.defaultNrOfConversationsToSkip;
//#endregion

//#region Conversation Model
window.app.Conversation = Backbone.Model.extend({    
    defaults: {
        TimeUpdated: Date.now(),
        Read: false,
        Text: "some data", //content text
        From: "defaultDirectionNumber", //used for indication if the last message was from or to
        To: "defaultRecipient",
        ConvID: "defaultConversationID", //unique identifier for a conversation
        Starred: false, //favorite conversation or not
        ClientDisplayName: "defaultClient",
        ClientIsSupportBot: false,
        IsSmsBased: false     
    },
   //DA we override the initialize function so that all Conversation objects have their properties properly initialized
   //the problem before was that parse was only called when initialized via collection.fetch
    initialize: function (attributes, options) {
       this.set('IsSmsBased', (attributes.IsSmsBased === "false" || attributes.IsSmsBased === false) ? false : true);
       this.set('TimeUpdated', attributes.TimeReceived);
       if (this.get('IsSmsBased')) {
          var fromTo = getFromToFromConversation(attributes.ConvID);
          this.set('ClientDisplayName', fromTo[0].substring(0, fromTo[0].length - 4) + "....");
       }
    },
    toggleStarred: function () {
        /* Toggle the starred value using an optimistically approach
        Change the value of Starred attribute after click to affect 
        display. Then save the new value on the server, if the request
        fails either a connection problem (fail) or application logic
        (request failed) the value is reverted.
        */
        var self = this;
        self.set("Starred", !self.get("Starred"));
        $.getJSON('Conversations/ChangeStarredStatusForConversation',
                { conversationId: this.get("ConvID"), newStarredStatus: this.get("Starred") },
                function (data) {
                    if (data !== "Update successful") {
                        self.set("Starred", !self.get("Starred"));
                    }
                }).fail(function () {
                    self.set("Starred", !self.get("Starred"));
                });
    },
    methodUrl: {
       "delete": "Conversations/Delete"
    },
    sync: function (method, model, options) {
       if (model.methodUrl && model.methodUrl[method]) {
          options = options || {};
          options.url = model.methodUrl[method];
       }
       var parseMethod = (method === "delete") ? "create" : method;
       Backbone.sync(parseMethod, model, options);
    },
});
//#endregion
//#region ConversationsList
window.app.ConversationsList = Backbone.Collection.extend({
    model: window.app.Conversation,
    convID: null,
    methodUrl: {
        "read": "Conversations/ConversationsList",
        "delete": "Conversations/Delete"
    },
    sync: function (method, collection, options) {
        if (collection.methodUrl && collection.methodUrl[method]) {
            options = options || {};
            options.url = collection.methodUrl[method];
        }
        var parseMethod = (method === "delete") ? "create" : method;
        Backbone.sync(parseMethod, collection, options);
    }
});
//#endregion
//#region ConversationView
_.templateSettings = {
   interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
   evaluate: /\{%([\s\S]+?)%\}/g,   // excute code: {% code_to_execute %}
   escape: /\{%-([\s\S]+?)%\}/g
}; // excape HTML: {%- <script> %} prints &lt

window.app.ConversationView = Backbone.View.extend({
   events: {
      "click .star": "favoriteStarClicked",
      "click .conversation": "conversationClicked",
      "click .deleteConvImg" : "deleteConversation"
   },
   model: window.app.Conversation,
   tagName: "div",
   RequestSelect:false,
   initialize: function () {
      $("#convOverlay").hide();
      _.bindAll(this, 'render', "updateFavouritesIcon",
          "unrender", "favoriteStarClicked", "conversationClicked",
          "updateSelectedState", "deleteConversation");
      this.conversationTemplate= _.template($('#conversation-template').html()),
      this.transition = new Transition(
          document.getElementById("scrollableconversations"),$("#convOverlay"));
      this.render();
      this.model.on("change:Starred", this.updateFavouritesIcon);
      this.model.on("change:Read", this.render);                  
   },
   render: function () {
      var selfConvView = this;
      this.$el.html(this.conversationTemplate(this.model.toJSON()));
      // TODO : Move this action
      var deleteConvImg = $(".deleteConv img", this.$el);

      //#region Hover on a conversation
      var deleteConvArea = $(".deleteConv", this.$el);
      if (deleteConvArea !== undefined) {
         $(this.$el).hover(function () {
            $(deleteConvArea).fadeIn(100);
         }, function () {
            $(deleteConvArea).fadeOut(100);
         });
      }
      //#endregion
      return this;
   },
   unrender: function () {
      $(this.el).remove();
   },
   // TODO MB: Investigate why the first version is not working
   updateFavouritesIcon: function () {
      if (this.model.get("Starred")) {       
         $(".starred", this.$el).show();
         $(".unstarred", this.$el).hide();
      } else {       
         $(".starred", this.$el).hide();
         $(".unstarred", this.$el).show();
      }
   },
   favoriteStarClicked: function (event) {
      event.preventDefault();
      this.model.toggleStarred();
   },
   conversationClicked: function (event) {
      event.preventDefault();
      // click on the conversation but outside the star icon
      if (!$(event.target).hasClass("star")) {
         this.RequestSelect  = true;
         gSelectedElement = this.$el;
         resetTimer();
         gSelectedConversationID = this.model.get("ConvID");
         window.app.selectedConversation = this.model;
         $(document).trigger("conversationSelected", { convID: gSelectedConversationID });
      }
   },
   updateSelectedState: function () {     
      if (this.RequestSelect) {
         $(".conversation", this.$el).addClass("ui-selected");
         this.RequestSelect = false;
      } else {
         $(".conversation", this.$el).removeClass("ui-selected");
      }
   },
   deleteConversation: function () {    
      var selfConvView = this;
      if (confirm($("#confirmDeleteConversation").val() + " \"" + selfConvView.model.get("ClientDisplayName") + "\" ?")) {
         selfConvView.transition.startTransition();
         selfConvView.model.destroy({
            wait: true,
            success: function (model, response, options) {
               selfConvView.unrender();
               selfConvView.transition.endTransition();
               $(document).trigger("deleteMessagesOfAConversation", { convId: model.get("ConvID") });
            },
            error: function (model, xhr, options) {
               selfConvView.transition.endTransition();
            }
         });
      }
   }
});
//#endregion

function ConversationArea(filterArea, workingPointsArea) {
    "use strict";
    var self = this;
    var opts = {
        lines: 13, // The number of lines to draw
        length: 7, // The length of each line
        width: 4, // The line7 thickness
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
    var spinnerAddConvs = new Spinner(optsForLoadMoreConversationsSpinner);

    var ConversationsArea = Backbone.View.extend({
        el: $("#conversations"),
        initialize: function () {
            this.filters = filterArea;
            this.workingPoints = workingPointsArea;
            //this.workingpoints = workingPointsArea;
            _.bindAll(this,
               "render",
               "getConversations",
               "getAdditionalConversations",
               "addConversationWithEffect",
               "addConversationBasicEffect",
               "updateConversation",
               "newMessageReceived",
               "gatherFilterOptions",
               "selectionChanged");
            this.convsList = new window.app.ConversationsList();
            this.convsList.bind("reset", this.render);
            // create an array of views to keep track of children
            this._convViews = [];
            //by default conversations are "new"
            this.addConversationAsNewElement = true;
            //in refreshsInProgress we keep count of how many refresh requests are running simultaneously
            this.refreshsInProgress = 0;
            $(document).bind('conversationSelected', this.selectionChanged);
        },
        getConversations: function () {
            //#region  Resetting internal variables                    
            this._convViews = [];

            var selfConvArea = this;
            //reset the cumulative skip because we start with a "fresh" view
            window.app.cummulativeSkip = window.app.defaultNrOfConversationsToSkip;
            //#endregion

            this.refreshsInProgress++;
            //#region Visual manipulation
            var target = document.getElementById('scrollableconversations');

            $('#loadMoreConversations').hide();
            $('#conversations').html('');

            spinner.spin(target);
            //#endregion

            //#region Prepare parameters
            var options = this.gatherFilterOptions();
            //#endregion

            this.convsList.fetch({
                data: options,
                traditional: true,
                success: function (data) {
                    if (selfConvArea.refreshsInProgress <= 1) {
                        spinner.stop();
                    }
                    if (window.app.firstCall) {
                        window.app.updateNrOfUnreadConversations(false);
                        window.app.firstCall = false;
                    }
                    window.app.requestIndex++;
                }
            });
        },
        gatherFilterOptions: function () {
            var filterOptions = {};
            var selectedTags = [];
            if (this.filters.tagFilteringEnabled) {
                selectedTags = this.filters.tagsForFiltering;
            }
            var showFavorites = this.filters.starredFilteringEnabled;
            var popUpSupport = this.filters.supportFilteringEnabled;
            var workingPointsNumbers = this.workingPoints.checkedPhoneNumbersArray;

            var startDate, endDate;
            if (this.filters.dateFilteringEnabled) {
                startDate = this.filters.startDate;
                endDate = this.filters.endDate;
            }
            var onlyUnreadConvs = this.filters.unreadFilteringEnabled;
            var top = window.app.defaultNrOfConversationsToDisplay;
            var skip = window.app.cummulativeSkip;

            filterOptions.onlyFavorites = showFavorites;
            filterOptions.tags = selectedTags;
            filterOptions.workingPointsNumbers = workingPointsNumbers;
            filterOptions.startDate = startDate;
            filterOptions.endDate = endDate;
            filterOptions.onlyUnread = onlyUnreadConvs;
            filterOptions.skip = skip;
            filterOptions.top = top;
            filterOptions.requestIndex = window.app.requestIndex;
            filterOptions.popUpSupport = popUpSupport;
            return filterOptions;
        },
        getAdditionalConversations: function () {
            //#region Visual manipulation
            //the spinner button should only affect the LoadMoreConversationsDiv
            var target = document.getElementById('loadMoreConversations');
            $(target).removeClass("readable");
            $(target).addClass("unreadable");
            spinnerAddConvs.spin(target);
            //#endregion

            //update the cumulativeSkip (supporting the use case where we use LoadMoreConversations multiple times)
            window.app.cummulativeSkip = this.convsList.length;

            //#region Prepare parameters
            var options = this.gatherFilterOptions();
            //#endregion

            //add these "old" conversations to the end
            var selfConversationsView = this;
            $.ajax({
                url: "Conversations/ConversationsList",
                data: options,
                traditional: true,
                success: function (data, textStatus, jqXHR) {
                    spinnerAddConvs.stop();
                    $(target).removeClass("unreadable");
                    $(target).addClass("readable");

                    $.each(data, function () {
                       var conv = new window.app.Conversation(this);                            
                        selfConversationsView.convsList.add(conv, { silent: true });
                        selfConversationsView.addConversationBasicEffect(conv, false);

                    });
                    if (data.length === 0 || data.length < window.app.defaultNrOfConversationsToDisplay) {
                        $(target).hide('slow');
                    }
                }
            }
         );
        },
        render: function () {
            // We keep track of the rendered state of the view
            this._rendered = true;
            this.refreshsInProgress--;

            if (this.refreshsInProgress === 0) {
                var convEl = $("#conversations");
                convEl.html('');
                var selfConversationsView = this;
                this.convsList.each(function (conv) {
                    selfConversationsView.addConversationBasicEffect(conv);
                });
            }
        },
        addConversationWithEffect: function (conv, addConversationAsNewElement, newElementIsSelected) {
            if (addConversationAsNewElement === null) {
                addConversationAsNewElement = true;
            }
            if (newElementIsSelected === null) {
                newElementIsSelected = false;
            }
            var item = this.addConversationNoEffect(conv, addConversationAsNewElement);
            var timer = 500;
            //if the element that was updated was selected - reflect this on the new element
            if (newElementIsSelected) {
                // make a distinction between the selected status of a normal conv and a support conversation
                if (conv.ClientIsSupportBot) {
                   //$(item).addClass("ui-selectedSupport");
                   $(".conversation", $(item)).addClass("ui-selectedSupport");
                } else {
                   //$(item).addClass("ui-selected");
                   $(".conversation", $(item)).addClass("ui-selected");
                }
                gSelectedElement = item;
                resetTimer();
                startTimer(3000);
            }
            $(item).hide().fadeIn(timer);           
        },
        addConversationBasicEffect: function (conv, addConversationAsNewElement) {
            if (addConversationAsNewElement === null) {
                addConversationAsNewElement = true;
            }
            var item = this.addConversationNoEffect(conv, addConversationAsNewElement);
            $(item).hide().fadeIn("slow");
        },
        addConversationNoEffect: function (conv, addConversationAsNewElement) {
            var convView = new window.app.ConversationView({ model: conv });
            this._convViews.push(convView);
            var item = convView.render().el;
            if (addConversationAsNewElement) {
                $(this.el).prepend(item);
            }
            else {
                $(this.el).append(item);
            }
            var selfConversationsView = this;
            conv.off("change:TimeUpdated");
            conv.on("change:TimeUpdated", function (model) {
                    selfConversationsView.updateConversation(model);                
            });
            //if we are displaying more then 10 conversations then prepare to
            if (this.convsList.models.length >= window.app.defaultNrOfConversationsToDisplay) {
                $("#loadMoreConversations").show('slow');
            }
            return item;
        },
        updateConversation: function (conversation) {
            //when we get an update the conversation will move to the top of the list
            var selfConversationsView = this;
            var viewToRemove = _(this._convViews).select(function (cv) {
                return cv.model.get("ConvID") === conversation.get("ConvID");
            })[0];
            if (viewToRemove !== undefined && viewToRemove !== null) {
                this._convViews = _(this._convViews).without(viewToRemove);
                if (this._rendered) {
                   var thisElementWasSelected = false;
                   if($('.conversation',viewToRemove.$el).hasClass('ui-selected')) {
                        thisElementWasSelected = true;
                    }
                    var elem = $(viewToRemove.el);

                    elem.fadeOut("slow", function () {
                        elem.remove();
                        //make sure to clear any event handlers, so we don't handle the same event twice                      
                        selfConversationsView.addConversationWithEffect(conversation, true, thisElementWasSelected);
                    });
                }
            }
        },
        newMessageReceived: function (fromID, toID, convID, dateReceived, newText, readStatus, isSmsBased) {
            //if the given conversation exists we update it, otherwise we create a new conversation          
            var modelToUpdate = self.convsView.convsList.get(convID);
            if (modelToUpdate) {
                //since the view will react to model changes we make sure that we do "batch updates" - only the last update will trigger the update
                //all the previous updates will be "silent"
                modelToUpdate.set({ "Text": newText }, { silent: true });
                modelToUpdate.set({ "Read": readStatus }, { silent: true });

                modelToUpdate.set({ "To": toID }, {silent:true});
                modelToUpdate.set({ "From": fromID }, { silent: true });
                modelToUpdate.set({ "TimeUpdated": dateReceived }, { silent: false });
            } else {
                //indicate that a new message has been received
                //show conversation only if not filtering
                if (!self.convsView.filters.IsFilteringEnabled()) {
                   var modelToAdd = new window.app.Conversation({
                      From: fromID, To: toID, ConvID: convID, TimeReceived: dateReceived, Text: newText, ClientDisplayName: fromID, ClientIsSupportBot: false, IsSmsBased: isSmsBased
                   });
                    self.convsView.convsList.add(modelToAdd);
                    self.convsView.addConversationWithEffect(modelToAdd, true, false);
                }
            }
        },
        selectionChanged: function () {
           _.each(this._convViews, function (view) {
              view.updateSelectedState();
           });
        }
    });

    $("#loadMoreConversations").bind("click", function (e) {
        self.convsView.getAdditionalConversations();
    });

    this.convsView = new ConversationsArea();  
}