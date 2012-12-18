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
   parse: function (data, xhc) {
      data.TimeUpdated = data.TimeReceived;
      if (data.IsSmsBased === "false" || data.IsSmsBased === false) {
         data.IsSmsBased = false;
      } else {
         data.IsSmsBased = true;
      }

      return data;
   },
   idAttribute: "ConvID" //the id shold be the combination from-to
});
//#endregion

//#region ConversationsList
window.app.ConversationsList = Backbone.Collection.extend({
   model: window.app.Conversation,
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

   $("#convOverlay").hide();
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
   var deleteConvSpinner = new Spinner(opts);

   window.app.ConversationView = Backbone.View.extend({
      model: window.app.Conversation,
      tagName: "div",
      conversationTemplate: _.template($('#conversation-template').html()),
      self: this,
      initialize: function () {
         _.bindAll(this, 'render', "updateFavouritesIcon");
         this.model.on("change:Starred", this.updateFavouritesIcon);
         this.model.on("change:Read", this.render);
         return this.render;
      },
      render: function () {
         var selfConvView = this;
         this.$el.html(this.conversationTemplate(this.model.toJSON()));
         var readUnread = "unreadconversation";
         if (this.model.attributes.Read === true) {
            readUnread = "readconversation";
         }
         var convNormalSupport = "normalConversation";
         if (this.model.attributes.ClientIsSupportBot) {
            convNormalSupport = "supportConversation";
         }
         this.$el.addClass("conversation");
         this.$el.addClass(readUnread);
         this.$el.addClass(convNormalSupport);
         $(this.el).attr("conversationId", this.model.attributes.ConvID);
         var markAsFavImg = $(".conversationStarIcon img", this.$el);
         setTooltipOnElement(markAsFavImg, markAsFavImg.attr('tooltiptitle'), 'dark');
         var deleteConvImg = $(".deleteConv img", this.$el);
         setTooltipOnElement(deleteConvImg, deleteConvImg.attr('tooltiptitle'), 'dark');
         //#region Click on Mark as Favorite
         $(".conversationStarIconImg", this.$el).bind("click", function (e) {
            e.preventDefault();
            var starredStatus = selfConvView.model.get("Starred");
            selfConvView.model.set("Starred", !starredStatus);
            var id = $(this).parents(".conversation").attr("conversationId");
            $.getJSON('Messages/ChangeStarredStatusForConversation',
                    { conversationId: id, newStarredStatus: !starredStatus },
                    function (data) {
                       //conversation starred status changed                            
                    });
         });

         $(".deleteConvImg", this.$el).bind("click", function (e) {
            e.preventDefault();
            var conversationElement = $(this).parents(".conversation");
            var id = conversationElement.attr("conversationId");
            var user = id.split("-")[0];
               //DA if we deleted the selected conversation reset the messagesView
                var convIsSelected = (gSelectedConversationID === id);
            $("#convOverlay").show();
            if (confirm($("#confirmDeleteConversation").val() + " \"" + user + "\" ?")) {
               var target = document.getElementById('scrollableconversations');
               deleteConvSpinner.spin(target);
               $.getJSON('Messages/DeleteConversation',
                       { convId: id },
                       function (data) {
                               if (data === "success") {                                 
                                  $(conversationElement).remove();
                                  deleteConvSpinner.stop();
                                  $("#convOverlay").hide();                                 
                                  if (convIsSelected) {
                                     $(document).trigger('selectedConvDeleted');
                                     gSelectedConversationID = null;
                                     gSelectedElement = null;
                                  }
                          }
                       });
            } else {
               $("#convOverlay").hide();
            }
         });
           //#endregion
            
         return this;
      },
      updateFavouritesIcon: function () {
         var markAsFavImg = $(".conversationStarIcon img", this.$el);
         markAsFavImg.qtip("destroy");
         if (this.model.get("Starred")) {
            markAsFavImg.attr("src", window.app.domainName + "/Content/images/star-selected_orange.svg");
         } else {
            markAsFavImg.attr("src", window.app.domainName + "/Content/images/star.svg");
         }
         setTooltipOnElement(markAsFavImg, markAsFavImg.attr('tooltiptitle'), 'dark');
      }
   });
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

   var ConversationsView = Backbone.View.extend({
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
         this.convsList = new window.app.ConversationsList();
         this.convsList.bind("reset", this.render);
         // create an array of views to keep track of children
         this._convViews = [];
         //by default conversations are "new"
         this.addConversationAsNewElement = true;
         //in refreshsInProgress we keep count of how many refresh requests are running simultaneously
         this.refreshsInProgress = 0;
      },
      getConversations: function () {
         //#region  Reseting internal variables                    
         this._convViews = [];

         var selfConvArea = this;
         //reset the cummulative skip because we start with a "fresh" view
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
            url: "Messages/ConversationsList",
            data: options,
            traditional: true,
            success: function (data, textStatus, jqXHR) {
               spinnerAddConvs.stop();
               $(target).removeClass("unreadable");
               $(target).addClass("readable");

               $.each(data, function () {
                  var conv = new window.app.Conversation({
                     From: $(this).attr("From"),
                     ConvID: $(this).attr("ConvID"),
                     TimeReceived: $(this).attr("TimeReceived"),
                     Text: $(this).attr("Text"),
                     Read: $(this).attr("Read"),
                     To: $(this).attr('To'),
                     Starred: $(this).attr('Starred'),
                     ClientDisplayName: $(this).attr('ClientDisplayName'),
                     ClientIsSupportBot: $(this).attr('ClientIsSupportBot')
                  });
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
         var timer = 300;
         //if the element that was updated was selected - reflect this on the new element
         if (newElementIsSelected) {
            // make a distinction between the selected status of a normal conv and a support conversation
            if (conv.ClientIsSupportBot) {
               $(item).addClass("ui-selectedSupport");
            } else {
               $(item).addClass("ui-selected");
            }
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
      newMessageReceived: function (fromID, toID, convID, dateReceived, newText, readStatus, isSmsBased) {
         //if the given conversation exists we update it, otherwise we create a new conversation          
         var modelToUpdate = self.convsView.convsList.get(convID);
         if (modelToUpdate) {
            //since the view will react to model changes we make sure that we do "batch updates" - only the last update will trigger the update
            //all the previous updates will be "silent"
            modelToUpdate.set({ "Text": newText }, { silent: true });
            modelToUpdate.set({ "Read": readStatus }, { silent: true });

            modelToUpdate.set("To", toID);
            modelToUpdate.set("From", fromID);
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
      }
   });

   $("#loadMoreConversations").bind("click", function (e) {
      self.convsView.getAdditionalConversations();
   });


   this.convsView = new ConversationsView();
}