"use strict";
window.app = window.app || {};
var tagsRep = {}; //I need this to be "global" because otherwise I can't see it from the callbacks "onAddTag" and "onRemoveTag"

//#region Tag model
window.app.Tag = Backbone.Model.extend({
   defaults: {
      Name: "tag",
      Description: "description",
      TagType: "none",
      IsDefault: false
   },
   idAttribute: "Name"
});
//#endregion

//#region TagsPool model
window.app.TagsPool = Backbone.Collection.extend({
   model: window.app.Tag,
   url: function () {
      return "Tags/GetTagsForConversation";
   }
});    
//#endregion

//#region SpecialTagsTool model
window.app.SpecialTagsPool = Backbone.Collection.extend({
   model: window.app.Tag,
   url: function () {
      return "Tags/GetSpecialTags";
   }
});
//$endregion

function TagsArea() { 
   //We use the collection for caching purposes
   var opts = {
      lines: 9, // The number of lines to draw
      length: 5, // The length of each line
      width: 3, // The line thickness
      radius: 3, // The radius of the inner circle
      rotate: 0, // The rotation offset
      color: 'black', // #rgb or #rrggbb
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
   
   var placeholderValue = $('#messagesAddTagPlaceHolderMessage').val();
   var removeTagValue = $('#messagesRemoveTagPlaceHolderMessage').val();
   var TagsPoolView = Backbone.View.extend({
      el: $("#tagsPool"),      
      initialize: function () {        
         this.conversationID = '';
         app.removeTagTitle = removeTagValue;
         $("#tags").tagsInput({
            'height': '22px',
            'width': 'auto',
            'onAddTag' : this.onAddTag,
            'onRemoveTag': this.onRemoveTag,
            'autocomplete_url': "Tags/FindMatchingTags",            
            'defaultText': placeholderValue,
            'placeholder': placeholderValue,
            'interactve': true
         });
         _.bindAll(this, 'render', 'appendTag', 'onAddTag', 'onRemoveTag','getTags');                      
      },
      onAddTag: function (tagValue) {
         //add the tag to the cache
         //TODO maybe we should cache this only on success
         var newTag = new app.Tag({ Name: tagValue });
         tagsRep[gSelectedConversationID].add(newTag);

         $.getJSON('Tags/AddTagToConversations',
                { tagName: tagValue,
                   convID: gSelectedConversationID
                },
                function (data) {
                   //tag added to conversation (or not? )                   
                }
        );
      },
      onRemoveTag: function (tagValue) {
         //remove tag from cache
         var tagsCollection = tagsRep[gSelectedConversationID];
         var tagToRemove = _(tagsCollection.models).select(function (tg) {
             return tg.get("Name") === tagValue;
          })[0];
         tagsCollection.remove(tagToRemove);

         $.getJSON('Tags/RemoveTagFromConversation',
                {
                   tagName: tagValue,
                   convID: gSelectedConversationID
                },
                function (data) {
                   //tag removed from conversation (or not? )
                }
        );
      },
      getTags: function (convId) {
         //$('#tagsPool').html('');
         var tg = $('#tags');
         tg.importTags('');
         //$('#tagsPool').hide();
         //$('#tags').hide();
         //$('#tags_tagsinput').hide();
         var target = document.getElementById('tagsContainer');
         //spinner.spin(target);
         this.conversationID = convId;
         if( convId in tagsRep) {
            spinner.stop();
            this.render();
         }
         else {
            var tagCollection = new app.TagsPool();
            tagCollection.bind("reset", this.render); 
            tagCollection.fetch({
               data: { "conversationID": convId },
               success: function () {
                  //spinner.stop();
                  //$('#tags_tagsinput').show();
                  //$('#tags_tag').show();
               }
            });
            tagsRep[convId] = tagCollection;
         }
         
      },
      render: function () {
         var tg = $('#tags');
         tg.importTags('');
         var self = this;
         tagsRep[this.conversationID].each(function (tag) {
            self.appendTag(tag);
         });

      },
      appendTag: function (tag) {
         $('#tags').addTag(tag.get("Name"), { callback: false });         
      }           
   });

   var tagsView = new TagsPoolView();
   return tagsView;
}  