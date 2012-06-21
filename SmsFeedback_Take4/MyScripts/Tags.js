"use strict";

var tagsRep = {}; //I need this to be "global" because otherwise I can't see it from the callbacks "onAddTag" and "onRemoveTag"
function TagsArea() {
   var Tag = Backbone.Model.extend({
      defaults: {         
         Name: "tag",
         Description: "description",         
      },
      idAttribute: "Name"
   });
   //We use the collection for caching purposes
   var TagsPool = Backbone.Collection.extend({
      model: Tag,
      url: function () {
         return "Tags/GetTagsForConversation";
      }
   });    

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
   

   var TagsPoolView = Backbone.View.extend({
      el: $("#tagsPool"),      
      initialize: function () {        
         this.conversationID = '';
         $("#tags").tagsInput({
            'height': '32px',
            'width': 'auto',
            'onAddTag' : this.onAddTag,
            'onRemoveTag': this.onRemoveTag,
            'autocomplete_url': "Tags/FindMatchingTags",          
         });
         _.bindAll(this, 'render', 'appendTag', 'onAddTag', 'onRemoveTag','getTags');         
         //this.tagsPool.bind("add", this.appendTag, this);
             
      },
      onAddTag: function (tagValue) {
         //add the tag to the cache
         //TODO maybe we should cache this only on success
         var newTag = new Tag({ Name: tagValue });
         tagsRep[gSelectedConversationID].add(newTag);

         $.getJSON('Tags/AddTagToConversations',
                { tagName: tagValue,
                   convID: gSelectedConversationID
                },
                function (data) {
                   //tag added to conversation (or not? )
                   console.log(data);
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
                   console.log(data);
                }
        );
      },
      getTags: function (convId) {
         //$('#tagsPool').html('');
      
         //$('#tags_tagsinput').hide();
         //$('#tags_tag').hide();
         var target = document.getElementById('tagsContainer');         
         spinner.spin(target);
         this.conversationID = convId;
         if( convId in tagsRep) {
            spinner.stop();
            this.render();
         }
         else {
            var tagCollection = new TagsPool();
            tagCollection.bind("reset", this.render); 
            tagCollection.fetch({
               data: { "conversationID": convId },
               success: function () {
                  spinner.stop();
                  //$('#tags_tagsinput').show();
                  //$('#tags_tag').show();
               }
            });
            tagsRep[convId] = tagCollection;
         }
         
      },
      render: function () {
          var tg = $('#tags');
         tg.importTags('')
         var self = this;
         tagsRep[this.conversationID].each(function (tag) {
            self.appendTag(tag);
         });

      },
      appendTag: function (tag) {
         $('#tags').addTag(tag.get("Name"), { callback: false });
         //var tagView = new TagView({ model: tag });
         //var item = tagView.render().el;
         //$(this.el).prepend(item);
         //$(item).hide().fadeIn('slow');
      }           
   });

   var tagsView = new TagsPoolView();
   return tagsView;
}  