"use strict";
window.app = window.app || {};
window.app.silentRemove = false;
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
   $("#thumbsUp").qtip({
       content: $("#thumbsUp").attr('tooltiptitle'),
       position: {
           corner: {
               target: 'leftMiddle',
               tooltip: 'rightMiddle'
           }
       },
       style: 'dark'
   });
   $("#thumbsDown").qtip({
       content: $("#thumbsDown").attr('tooltiptitle'),
       position: {
           corner: {
               target: 'leftMiddle',
               tooltip: 'rightMiddle'
           }
       },
       style: 'dark'
   });
   
   var placeholderValue = $('#messagesAddTagPlaceHolderMessage').val();
   var removeTagValue = $('#messagesRemoveTagPlaceHolderMessage').val();
   var TagsPoolView = Backbone.View.extend({
      el: $("#tagsPool"),      
      initialize: function () {        
          self = this;
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
         this.thumbsUp = $("#thumbsUp");
         this.thumbsDown = $("#thumbsDown");
         this.specialTagsPool = new window.app.SpecialTagsPool();
         this.getSpecialTags();
         _.bindAll(this, 'render', 'appendTag', 'onAddTag', 'onRemoveTag', 'getTags', 'getSpecialTags');
         $(".specialTag").click(function () {
             window.app.silentRemove = false;
             var tagType = $(this).attr("tagType");
             var tag = self.specialTagsPool.where({ TagType: tagType, IsDefault: true })[0];
             var tagName = tag.get("Name");
             if (!$("#tags").tagExist(tagName)) {
                 $("#tags").addTag(tagName);                 
             } else {
                 $("#tags").removeTag(tagName);                  
             }
         });
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
         var tagType = self.specialTagsPool.where({ Name: tagValue, IsDefault: true })[0].get("TagType");
         self.toggleHands(tagType);
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
         var tagType = self.specialTagsPool.where({ Name: tagValue, IsDefault: true })[0].get("TagType");
         self.turnHandOff(tagType);
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
      getSpecialTags: function() {
          this.specialTagsPool.fetch();
      },
      render: function () {
         var tg = $('#tags');
         tg.importTags('');
         var self = this;
         tagsRep[this.conversationID].each(function (tag) {
            self.appendTag(tag);
         });

          // initialize 
         this.turnHandsOff();
         this.turnTheHandOn("positiveFeedback", $("#thumbsUp"));
         this.turnTheHandOn("negativeFeedback", $("#thumbsDown"));
      },
      appendTag: function (tag) {
         $('#tags').addTag(tag.get("Name"), { callback: false });         
      },
      turnTheHandOn: function (tagType, element) {
          var tagName = this.specialTagsPool.where({ TagType: tagType, IsDefault: true })[0].get("Name");
          if ($("#tags").tagExist(tagName)) {
              $(element).css("background-position", "24px 0");
          }
      },
      toggleHands: function (tagType) {
          if (tagType == "positiveFeedback") {
              self.thumbsUp.css("background-position", "24px 0");
              // remove negative feedback 
              var oppositeTagType = "negativeFeedback";
              var oppositeTagName = self.specialTagsPool.where({ TagType: oppositeTagType, IsDefault: true })[0].get("Name");
              if ($("#tags").tagExist(oppositeTagName)) {
                  // Transition negative to positive
                  self.sendEventToServer("negToPos");
                  window.app.silentRemove = true;
                  $("#tags").removeTag(oppositeTagName);
              } else {
                  self.sendEventToServer("posAdd");
              }
          } else if (tagType == "negativeFeedback") {
              self.thumbsDown.css("background-position", "24px 0");
              var oppositeTagType = "positiveFeedback";
              var oppositeTagName = self.specialTagsPool.where({ TagType: oppositeTagType, IsDefault: true })[0].get("Name");
              if ($("#tags").tagExist(oppositeTagName)) {
                  // transition positive to negative
                  self.sendEventToServer("posToNeg");
                  window.app.silentRemove = true;
                  $("#tags").removeTag(oppositeTagName, true);
              } else {
                  self.sendEventToServer("negAdd");
              }
          }
      },
      turnHandsOff: function () {
          self.thumbsUp.css("background-position", "0 0");
          self.thumbsDown.css("background-position", "0 0");
      },
      turnHandOff: function (tagType) {
          if (!window.app.silentRemove) {
              if (tagType == "positiveFeedback") {
                  self.thumbsUp.css("background-position", "0 0");
                  self.sendEventToServer("posRemove");
              } else {
                  self.thumbsDown.css("background-position", "0 0");
                  self.sendEventToServer("negRemove");
              }
          } else {
              if (tagType == "positiveFeedback") {
                  self.thumbsUp.css("background-position", "0 0");                  
              } else {
                  self.thumbsDown.css("background-position", "0 0");                  
              }
          }
      },
      sendEventToServer: function (eventType) {
          $.getJSON('Messages/AddAnEventInConversationHistory',
                        { conversationId: gSelectedConversationID, eventType: eventType },
                        function (data) {
                            //conversation starred status changed                            
                        });
      }
      
   });

   var tagsView = new TagsPoolView();
   return tagsView;
}

