"use strict";

function TagsArea() {
   var Tag = Backbone.Model.extend({
      defaults: {         
         Name: "tag",
         Description: "description",         
      },
      idAttribute: "Id"
   });

   var TagPool = Backbone.Collection.extend({
      model: Tag,
      url: function () {
         return "Messages/WorkingPointsPerUser";
      }
   });
}  