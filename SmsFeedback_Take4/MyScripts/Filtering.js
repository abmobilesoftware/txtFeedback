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
/*global setCheckboxState */
//#endregion
window.app = window.app || {};
window.app.dateFormatForDatePicker = 'dd/mm/yy';

window.app.defaultFilteringOptions = {
   tagsForFiltering: [],
   tagFilteringEnabled: false,
   dateFilteringEnabled: false,
   startDate: "",
   endDate: "",
   starredFilteringEnabled: false,
   unreadFilteringEnabled: false,
   supportFilteringEnabled: false
};

function Filter(iElement, iValue) {
   var elementName = iElement;
   this.behindVariable = iValue;

   this.getElementName = function () {
      return elementName;
   };
   this.getBehindVariable = function () {
      return behindVariable;
   };
}

function FilterArea() {
   "use strict";
   var self = this;
   $.extend(this, window.app.defaultFilteringOptions);

   //#region Tags
   var placeholderMessage = $('#filteringAddFilterTagMessage').val();
   var removeTagValue = $('#messagesRemoveTagPlaceHolderMessage').val();
   window.app.removeTagTitle = removeTagValue;

   $("#filterTag").tagsInput({
      'height': '22px',
      'width': 'auto',
      'autocomplete_url': "Tags/FindMatchingTags",
      'onAddTag': function (tagValue) {
         var delimiter = ',';
         self.tagsForFiltering = $("#filterTag").val().split(delimiter);
         if (self.tagFilteringEnabled) {
            $(document).trigger('refreshConversationList');
         }
      },
      'onRemoveTag': function (tagValue) {
         var delimiter = ',';
         self.tagsForFiltering = $("#filterTag").val().split(delimiter);
         //if there are no tags the split will return [""] and this will be sent to the server
         //we guard agains this
         if ("" === self.tagsForFiltering[0]) {
            self.tagsForFiltering = [];
         }
         if (self.tagFilteringEnabled) {
            $(document).trigger('refreshConversationList');
         }
      },
      'defaultText': placeholderMessage,
      'placeholder': placeholderMessage,
      'interactve': true,
      'placeholderColor': '#666666'
   });

   self.deactivateFilters = function (onlySupport) {
      if (onlySupport) {
         self.tagFilteringEnabled = false;
         self.starredFilteringEnabled = false;
         self.unreadFilteringEnabled = false;
         setCheckboxState($('#includeTagsInFilter'), self.tagFilteringEnabled);
         setCheckboxState($('#includeStarredInFilter'), self.starredFilteringEnabled);
         setCheckboxState($('#includeUnreadInFilter'), self.unreadFilteringEnabled);
      } else {
         self.supportFilteringEnabled = false;
         setCheckboxState($('#includeSupportInFilter'), self.supportFilteringEnabled);
      }
   }

   $("#includeTagsInFilter").bind('click', function () {
      //set internal state
      if (self.tagFilteringEnabled) {
         self.tagFilteringEnabled = false;
      }
      else {
         self.tagFilteringEnabled = true;
      }
      //change checkbox state
      setCheckboxState($(this), self.tagFilteringEnabled);
      //trigger filtering if required      
      if (self.tagsForFiltering.length !== 0) {
         self.deactivateFilters(false);
         $(document).trigger('refreshConversationList');
      }
   });
   //#endregion

   //#region Starred
   $("#includeStarredInFilter").bind('click', function () {
      //set internal state
      self.starredFilteringEnabled = !self.starredFilteringEnabled;
      //change checkbox state
      setCheckboxState($(this), self.starredFilteringEnabled);
      //trigger filtering if required
      self.deactivateFilters(false);
      $(document).trigger('refreshConversationList');
   });
   //#endregion

   //#region Unread
   $("#includeUnreadInFilter").bind('click', function () {
      //set internal state
      self.unreadFilteringEnabled = !self.unreadFilteringEnabled;
      //change checkbox state
      setCheckboxState($(this), self.unreadFilteringEnabled);
      //trigger filtering if required
      self.deactivateFilters(false);
      $(document).trigger('refreshConversationList');
   });
   //#endregion

   //#region Support
   $("#includeSupportInFilter").bind('click', function () {
      //set internal state
      self.supportFilteringEnabled = !self.supportFilteringEnabled;
      //change checkbox state
      setCheckboxState($(this), self.supportFilteringEnabled);
      self.deactivateFilters(true);
      //trigger filtering if required
      $(document).trigger('refreshConversationList');
   });
   //#endregion

   //#region IsFilteringEnabled
   this.IsFilteringEnabled = function () {
      return self.tagFilteringEnabled || self.dateFilteringEnabled || self.starredFilteringEnabled || self.unreadFilteringEnabled || self.supportFilteringEnabled;
   };
   //#endregion

   $("#includeDateInFilter, #includeStarredInFilter, #includeUnreadInFilter, #includeTagsInFilter, #includeSupportInFilter").each(function () {
      var elementToShowTooltipOn = $(this);      
   });

}