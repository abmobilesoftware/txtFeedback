"use strict";

function FilterArea() {
   var self = this;
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
      'onRemoveTag': function(tagValue) {
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
      }
   });  
   this.tagsForFiltering = [];

   this.tagFilteringEnabled = false;
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
      if (self.tagsForFiltering.length != 0) {
         $(document).trigger('refreshConversationList');
      }
   });

   $("#startDateTimePicker").datepicker({ currentText: "Now", dateFormat: "dd-mm-yy", showButtonPanel: true });   
   $("#endDateTimePicker").datepicker({ dateFormat: "dd-mm-yy" });
   this.dateFilteringEnabled = false;
   $("#includeDateInFilter").bind('click', function () {
      //set internal state
      if (self.dateFilteringEnabled) {
         self.dateFilteringEnabled = false;
      }
      else {
         self.dateFilteringEnabled = true;
      }
      //change checkbox state
      setCheckboxState($(this), self.dateFilteringEnabled);
      //trigger filtering if required
      //if (self.tagsForFiltering.length != 0) {
      //   $(document).trigger('refreshConversationList');
      //}
   });

   this.starredFilteringEnabled = false;
   $("#includeStarredInFilter").bind('click', function () {
      //set internal state
      if (self.starredFilteringEnabled) {
         self.starredFilteringEnabled = false;
      }
      else {
         self.starredFilteringEnabled = true;
      }
      //change checkbox state
      setCheckboxState($(this), self.starredFilteringEnabled);
      //trigger filtering if required
      $(document).trigger('refreshConversationList');     
   });

}