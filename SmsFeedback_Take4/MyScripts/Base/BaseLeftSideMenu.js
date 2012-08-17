"use strict";
window.app = window.app || {};

_.templateSettings = {
   interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
   evaluate: /\{%([\s\S]+?)%\}/g,   // excute code: {% code_to_execute %}
   escape: /\{%-([\s\S]+?)%\}/g
}; // excape HTML: {%- <script> %} prints &lt

window.app.MenuItemModel = Backbone.Model.extend({
   defaults: {
      itemId: 1,
      itemName: "Conversation",
      leaf: false,
      parent: 1
   }
});

window.app.MenuCollection = Backbone.Collection.extend({
   model: window.app.MenuItemModel,
   initialize: function (options) {
      this.url = options.url;
   }
});

window.app.MenuItemView = Backbone.View.extend({
   tagName: 'li',
   model: app.MenuItemModel,
   initialize: function () {
      _.bindAll(this, 'render');
   },
   renderParent: function () {
      $(this.el).addClass('outerLi');
      $(this.el).addClass('listItem');
      $(this.el).html("<div class='menuParentItem'>" + this.model.get("itemName") + "</div>" + "<ul class='item" + this.model.get("itemId") + "'></ul>");
      return this;
   },
   renderLeaf: function () {
      $(this.el).addClass('innerLi');
      $(this.el).addClass('listItem');
      $(this.el).addClass('liItem' + this.model.get("itemId"));
      $(this.el).attr("menuId", this.model.get("itemId"));
      $(this.el).html("<span class='menuLeafItem' menuId='" + this.model.get("itemId") + "'>" + this.model.get("itemName") + "</span>");
      return this;
   }
});

window.app.MenuView = Backbone.View.extend({
   el: $("#leftColumn"),
   eventToTriggerOnSelect: "defaultEventName",
   initialize: function(options) {
      _.bindAll(this, 'render');      
      var self = this;
      self.eventToTriggerOnSelect = options.eventToTriggerOnSelect;
      this.menuItems = options.menuCollection;

      this.menuItems.fetch({
         success: function () {
            self.render();
         }
      });
   },
   render: function () {
      var self = this;
      $(this.el).append("<ul class='primaryList collapsibleList'></ul>");
      _(this.menuItems.models).each(function (menuItemModel) {
         var menuItemView = new window.app.MenuItemView({ model: menuItemModel });
         if (!menuItemModel.get("leaf")) {
            if (menuItemModel.get("parent") == 0) {
               $("ul.primaryList", self.el).append(menuItemView.renderParent().el);
            } else {
               var selector = ".item" + menuItemModel.get("parent");
               $(selector, self.el).append(menuItemView.renderParent().el);
            }
         } else {
            var selector = ".item" + menuItemModel.get("parent");
            $(selector, self.el).append(menuItemView.renderLeaf().el);
         }
      });

      // open report functionality
      $(".innerLi").click(function () {
         $(this).parents(".collapsibleList").find(".menuItemSelected").removeClass("menuItemSelected");
         $(this).addClass("menuItemSelected");
         $(document).trigger(self.eventToTriggerOnSelect, $(this).attr("menuId"));
      });
      // apply collapsible functionality to list
      CollapsibleLists.apply();
      // mark the first opened report
      $(".liItem2").addClass("menuItemSelected");
      $("ul.item1").css("display", "block");
   }
});