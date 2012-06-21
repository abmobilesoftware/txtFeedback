"use strict";

function TagsArea() {
   var Tag = Backbone.Model.extend({
      defaults: {         
         Name: "tag",
         Description: "description",         
      },
      idAttribute: "Id"
   });

   var TagsPool = Backbone.Collection.extend({
      model: Tag,
      url: function () {
         return "Tags/GetTagsForConversation";
      }
   });
   _.templateSettings = {
      interpolate: /\{\{(.+?)\}\}/g
   };

   var TagView = Backbone.View.extend({
      model: Tag,
      tagName: "span",
      tagTemplate: _.template($('#tag-template').html()),
      events: {
         "click .removeTag": "removeTag"
      },
      initialize: function () {
         _.bindAll(this, 'render');
         this.model.bind('destroy', this.unrender, this);
         return this.render;
      },
      render: function () {
         this.$el.html(this.tagTemplate(this.model.toJSON()));
         return this;
      },
      unrender: function () {
         this.$el.remove();
      },
      removeTag: function () {
         this.model.destroy();
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
         $("#tags").tagsInput({
            'height': '32px',
            'width': 'auto',
            'onAddTag' : this.onAddTag,
            'onRemoveTag': this.onRemoveTag,
            'autocomplete_url': "Tags/FindMatchingTags",          
         });

         _.bindAll(this, 'render', 'appendTag', 'tagsPoolChanged', 'getTags');
         this.tagsPool = new TagsPool();
         this.tagsPool.bind("add", this.appendTag, this);
         this.tagsPool.bind("reset", this.render);
         this.appendTag.bind("remove", this.tagsPoolChanged, this);
      },
      onAddTag: function (tagValue) {
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
         var tg = $('#tags');
         tg.importTags('')
         //$('#tags_tagsinput').hide();
         //$('#tags_tag').hide();
         var target = document.getElementById('tagsContainer');         
         spinner.spin(target);
         this.tagsPool.fetch({
            data: { "conversationID": convId },
            success: function () {
               spinner.stop();
               //$('#tags_tagsinput').show();
               //$('#tags_tag').show();
            }
         })
      },
      render: function () {
         var self = this;
         this.tagsPool.each(function (tag) {
            self.appendTag(tag);
         });
      },
      appendTag: function (tag) {
         $('#tags').addTag(tag.get("Name"), { callback: false });
         //var tagView = new TagView({ model: tag });
         //var item = tagView.render().el;
         //$(this.el).prepend(item);
         //$(item).hide().fadeIn('slow');
      },
      tagsPoolChanged: function () {
         if (this.tagsPool.models.length == 1) {
            //this.$el.hide();
            $(".removeTag").hide();
         }
      }
   });

   var tagsView = new TagsPoolView();
   return tagsView;
}  