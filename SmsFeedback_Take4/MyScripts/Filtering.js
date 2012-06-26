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
      },
      'onRemoveTag': function(tagValue) {
         var delimiter = ',';
         self.tagsForFiltering = $("#filterTag").val().split(delimiter);
         //if there are no tags the split will return [""] and this will be sent to the server
         //we guard agains this
         if ("" === self.tagsForFiltering[0]) {
            self.tagsForFiltering = [];
         }
      }
   });  
   this.tagsForFiltering = [];

   this.tagFilteringEnabled = false;
   $("#includeTagsInFilter").bind('click', function () {
      if (self.tagFilteringEnabled) {
         self.tagFilteringEnabled = false;
         
      }
      else {
         self.tagFilteringEnabled = true;
      }
      setCheckboxState($(this), self.tagFilteringEnabled);
      //var checkboxImg = $("img", this.$el);
      
   });

}