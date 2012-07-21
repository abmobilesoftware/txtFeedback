"use strict";
window.app = window.app || {};
window.selectedConversation = {}

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
      Text: "some data",
      From: "defaultNumber",
      To: "defaultRecipient",
      Starred: false
   },
   parse: function (data, xhc) {
      data.TimeUpdated = data.TimeReceived;
      return data;
   },
   idAttribute: "ConvID" //the id shold be the combination from-to
});
//#endregion

//#region ConversationsList
window.app.ConversationsList = Backbone.Collection.extend({
   model: app.Conversation,
   convID: null,
   url: function () {
      return "Messages/ConversationsList";
   }
});
//#endregion

//#region ConversationView
$(function () {
   window.app = window.app || {};
   /*_.templateSettings = {
      interpolate: /\{\{(.+?)\}\}/g
   };*/

   _.templateSettings = {
       interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
       evaluate: /\{%([\s\S]+?)%\}/g,   // excute code: {% code_to_execute %}
       escape: /\{%-([\s\S]+?)%\}/g
   }; // excape HTML: {%- <script> %} prints &lt

   window.app.ConversationView = Backbone.View.extend({
      model: app.Conversation,
      tagName: "div",
      conversationTemplate: _.template($('#conversation-template').html()),
      initialize: function () {
          _.bindAll(this, 'render');
         this.model.on("change", this.render);
         return this.render;
      },
      render: function () {
         this.$el.html(this.conversationTemplate(this.model.toJSON()));
         var readUnread = "unreadconversation";
         if (this.model.attributes.Read === true) {
            readUnread = "readconversation";
         }
         this.$el.addClass("conversation");
         this.$el.addClass(readUnread);
         $(this.el).attr("conversationId", this.model.attributes.ConvID);
         return this;
      }
   });
});
//#endregion

function ConversationArea(filterArea, workingPointsArea) {
    var self = this;    
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
             "gatherFilterOptions");
          this.convsList = new app.ConversationsList();
          this.convsList.bind("reset", this.render);
          this.convsList.bind("add", this.addConversationWithEffect, this);
          // create an array of views to keep track of children
          this._convViews = [];
          //by default conversations are "new"
          this.addConversationAsNewElement = true;
          //in refreshsInProgress we keep count of how many refresh requests are running simultaneously
          this.refreshsInProgress = 0;
       },
       getConversations: function (workingPoints) {
          //#region  Reseting internal variables                    
          this._convViews = [];
          
          var selfConvArea = this;
          //reset the cummulative skip because we start with a "fresh" view
          app.cummulativeSkip = app.defaultNrOfConversationsToSkip;
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
             traditional:true,
             success: function (data) {
                if (selfConvArea.refreshsInProgress <= 1) {
                   spinner.stop();
                }
                //spinner.stop();
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
          var workingPointsNumbers = this.workingPoints.checkedPhoneNumbersArray;
     
          var startDate, endDate;
          if (this.filters.dateFilteringEnabled) {
             startDate = this.filters.startDate;
             endDate = this.filters.endDate;
          }
          var onlyUnreadConvs = this.filters.unreadFilteringEnabled;
          var top = app.defaultNrOfConversationsToDisplay;
          var skip = app.cummulativeSkip;

          filterOptions["onlyFavorites"] = showFavorites;
          filterOptions["tags"] = selectedTags;
          filterOptions["workingPointsNumbers"] = workingPointsNumbers;
          filterOptions["startDate"] = startDate;
          filterOptions["endDate"] = endDate;
          filterOptions["onlyUnread"] = onlyUnreadConvs;
          filterOptions["skip"] = skip;
          filterOptions["top"] = top;
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
          app.cummulativeSkip = app.cummulativeSkip + app.defaultNrOfConversationsToDisplay;
          
          //#region Prepare parameters
          var options = this.gatherFilterOptions();
          //#endregion

          //add these "old" conversations to the end
          var selfConversationsView = this;
          $.ajax({
             url: "Messages/ConversationsList",
             data: options,
             traditional: true,
             success: function (data) {
                spinnerAddConvs.stop();
                $(target).removeClass("unreadable");
                $(target).addClass("readable");

                $.each(data, function () {
                   var conv = new app.Conversation({
                      From: $(this).attr("From"),
                      ConvID: $(this).attr("ConvID"),
                      TimeReceived: $(this).attr("TimeReceived"),
                      Text: $(this).attr("Text"),
                      Read: $(this).attr("Read"),
                      To: $(this).attr('To'),
                      Starred:$(this).attr('Starred')
                   });
                   selfConversationsView.addConversationBasicEffect(conv, false);
                });
                if (data.length === 0 || data.length < app.defaultNrOfConversationsToDisplay) {
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

          if (this.refreshsInProgress == 0) {
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
          var timer = 300;
          //if the element that was updated was selected - reflect this on the new element
          if (newElementIsSelected) {
             $(item).addClass("ui-selected");
             gSelectedElement = item;
             resetTimer();
             startTimer(3000);
          }
          $(item).hide().fadeIn(timer).fadeOut(timer).fadeIn(timer).fadeOut(timer).fadeIn(timer).fadeOut(timer).fadeIn(timer);
       },
       addConversationBasicEffect: function (conv, addConversationAsNewElement) {
          if (addConversationAsNewElement === null) {
             addConversationAsNewElement = true;
          }
          var item = this.addConversationNoEffect(conv, addConversationAsNewElement);
          $(item).hide().fadeIn("slow");
       },
       addConversationNoEffect: function (conv, addConversationAsNewElement) {
       
          var convView = new app.ConversationView({ model: conv });
          this._convViews.push(convView);
          var item = convView.render().el;
          if (addConversationAsNewElement) {
             $(this.el).prepend(item);
          }
          else {
             $(this.el).append(item);
          }
       
          var selfConversationsView = this;
          conv.on("change", function (model) {
              if (model.hasChanged("Read") && model.get("Read")) {
                  // read status s-a schimbat in true -> mesaj citit. 
              } else if (model.hasChanged("Starred")) {
                  // s-a schimbat doar favorites status.
              } else {
                  selfConversationsView.updateConversation(model);
              }
                  
          });
          //if we are displaying more then 10 conversations then prepare to
          if (this.convsList.models.length >= app.defaultNrOfConversationsToDisplay) {
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
          if (viewToRemove != undefined && viewToRemove !== null) {
             this._convViews = _(this._convViews).without(viewToRemove);
             if (this._rendered) {
                var thisElementWasSelected = false;
                if (gSelectedElement === viewToRemove.el) {
                   thisElementWasSelected = true;
                }
                var elem = $(viewToRemove.el);
                elem.fadeOut("slow", function () {
                   elem.remove();
                   //make sure to clear any event handlers, so we don't handle the same event twice
                   conversation.off("change");
                   selfConversationsView.addConversationWithEffect(conversation, true, thisElementWasSelected);
                });
             }
          }
       },
       newMessageReceived: function (fromID, toID, convID, dateReceived, newText) {
          //if the given conversation exists we update it, otherwise we create a new conversation         
          var modelToUpdate = self.convsView.convsList.get(convID);
          if (modelToUpdate) {
             //since the view will react to model changes we make sure that we do "batch updates" - only the last update will trigger the update
             //all the previous updates will be "silent"
             modelToUpdate.set({ "Text": newText }, { silent: true });
             modelToUpdate.set("Read", false);             
          }
          else {
             var modelToAdd = new app.Conversation({ From: fromID,To: toID, ConvID: convID, TimeReceived: dateReceived, Text: newText });
             //model.id = assign unique id
             self.convsView.convsList.add(modelToAdd);
          }
       }
    });
       
    $("#loadMoreConversations").bind("click", function () {
       self.convsView.getAdditionalConversations();
    });

    $(".conversationStarIconImg").live("click", function (e) {
        e.preventDefault();
        var id = $(this).parents(".conversation").attr("conversationId");
        /*var newStarredStatus = false;
        self.messagesRep[id].each(function (msg) {
            //var newStarredValue = ;
            msg.set("Starred", !msg.attributes["Starred"]);
            newStarredStatus = msg.attributes["Starred"];
        });*/
        var starredStatus = self.convsView.convsList.get(id).get("Starred");
        self.convsView.convsList.get(id).set("Starred", !starredStatus);

        $.getJSON('Messages/ChangeStarredStatusForConversation',
                { convID: id, newStarredStatus: !starredStatus },
                function (data) {
                    //conversation starred status changed
                    console.log(data);
                });
                
    });

    this.convsView = new ConversationsArea();     
}