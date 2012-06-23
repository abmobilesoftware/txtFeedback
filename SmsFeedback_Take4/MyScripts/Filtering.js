"use strict";

function FilterArea() {
   $("#filterTag").tagsInput({
      'height': '25px',
      'width': 'auto',
      'autocomplete_url': "Tags/FindMatchingTags",
      'onAddTag': this.onAddTag,
      'onRemoveTag': this.onRemoveTag
   });

   //this.onAddTag = function (tagValue) {
  
   //};
   this.onRemoveTag = function (tagValue) {
      alert(tagValue);
   };
   this.tagsForFiltering = [];
}

FilterArea.prototype.onAddTag = function (tagValue) {
   var delimiter = ',';
   this.tagsForFiltering = $("#filterTag").val().split(delimiter);
      //$.each(tagslist, function (index, value) {
      //   alert(value);
      //});
}