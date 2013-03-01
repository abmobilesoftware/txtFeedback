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
/*global CollapsibleLists */
//#endregion
window.app = window.app || {};

_.templateSettings = {
   interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
   evaluate: /\{%([\s\S]+?)%\}/g,   // execute code: {% code_to_execute %}
   escape: /\{%-([\s\S]+?)%\}/g
}; // excape HTML: {%- <script> %} prints &lt

window.app.MenuItemModel = Backbone.Model.extend({
   defaults: {
      itemId: 1,
      itemName: "Conversation",
      leaf: false,
      parent: 1,
      Action: "Action",
      callbackFunction: "",
      FriendlyName: "",
      selected: false
   },
   idAttribute: "itemId"
});

window.app.MenuCollection = Backbone.Collection.extend({
   model: window.app.MenuItemModel,
   initialize: function (options) {
      this.url = options.url;
   }
});

window.app.MenuItemView = Backbone.View.extend({
   tagName: 'li',
   model: window.app.MenuItemModel,
   events: {
      "click": "menuSelected"
   },
   menuSelected: function () {
      //DA due to the way we build the list, when clicking the child, the parent will also be called 
      //so make sure the parent does not trigger any events (besides the collapsing defined in collapsible list)
      if (this.model.get("parent") != 0) {
         this.$el.parents(".collapsibleList").find(".menuItemSelected").removeClass("menuItemSelected");
         this.$el.addClass("menuItemSelected");
         $(document).trigger(this.model.get("callbackFunction"),{
            menuId: this.model.get("itemId"),
            menuNavigation: this.model.get("FriendlyName"),
            menuAction: this.model.get("Action"),
         });
      }      
   },
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
   initialize: function (options) {
      _.bindAll(this, 'render', 'afterInitializeFunction');
      var self = this;
      self.eventToTriggerOnSelect = options.eventToTriggerOnSelect;
      self.afterInitializeFunction = options.afterInitializeFunction;
      this.menuItems = options.menuCollection;

      this.menuItems.fetch({
         success: function () {
            self.render();
            self.afterInitializeFunction(self.menuItems);
         }
      });
   },
   render: function () {
      var self = this;
      $(this.el).append("<ul class='primaryList collapsibleList'></ul>");
      _(this.menuItems.models).each(function (menuItemModel) {
         menuItemModel.set("callbackFunction",self.eventToTriggerOnSelect);
         var menuItemView = new window.app.MenuItemView({ model: menuItemModel });
         if (!menuItemModel.get("leaf")) {
            if (menuItemModel.get("parent") === 0) {
               $("ul.primaryList", self.el).append(menuItemView.renderParent().el);
            } else {
               var sel = ".item" + menuItemModel.get("parent");
               $(sel, self.el).append(menuItemView.renderParent().el);
            }
         } else {
            var selector = ".item" + menuItemModel.get("parent");
            $(selector, self.el).append(menuItemView.renderLeaf().el);
         }
      });

      // open report functionality
      //$(".innerLi").click(function () {
      //   $(this).parents(".collapsibleList").find(".menuItemSelected").removeClass("menuItemSelected");
      //   $(this).addClass("menuItemSelected");
      //   $(document).trigger(self.eventToTriggerOnSelect, $(this).attr("menuId"));
      //});

      // apply collapsible functionality to list
      CollapsibleLists.apply();

   },
   afterInitializeFunction: function () { }
});