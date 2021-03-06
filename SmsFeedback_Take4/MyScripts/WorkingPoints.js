﻿//#region Defines to stop jshint from complaining about "undefined objects"
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
/*global Spinner */
/*global setTooltipOnElement */
/*global setCheckboxState */
//#endregion
/* 
 * window.app = window.app || { } will set window.app to an empty object if there is no window.app 
 * and will leave window.app alone if it has already been set; doing it like this makes the JavaScript files 
 * more self-contained and less subject to loading order 
 */
window.app = window.app || {};
window.app.workingPointsNameDictionary = {}; //
window.app.workingPointsSuffixDictionary = {};

//#region WorkingPoint model
//A working point is defined by
// telNumber - used for SMS communication
// shortID - used for XMPP communication
//this is why, we use both tel number and short id when selecting conversations belonging to a certain wp
window.app.WorkingPoint = Backbone.Model.extend({
   defaults: {
      TelNumber: "defaultNumber",
      Name: "defaultNumber",
      Description: "defaultDescription",
      CheckedStatus: true,
      ShortID: "defaultID",
      XMPPsuffix: "defaultXMPPsuffix",
      XMPPaddress: "defaultXmppAddress"
   },
   parse: function (data, xhc) {
      //a small hack: the TimeReceived will be something like: "\/Date(1335790178707)\/" which is not something we can work with
      //in the TimeReceived property we have the same info coded as ticks, so we replace the TimeReceived value with a value build from the ticks value
      window.app.workingPointsNameDictionary[data.TelNumber] = data.Name;
      window.app.workingPointsNameDictionary[data.ShortID] = data.Name;
      window.app.workingPointsSuffixDictionary[data.ShortID] = data.XMPPsuffix;
      //for convenience we build the XMPPAddess
      data.XMPPaddress = data.ShortID + data.XMPPsuffix;
      return data;
   },
   idAttribute: "TelNumber"
});
//#endregion 

//#region WorkingPointPool
window.app.WorkingPointPool = Backbone.Collection.extend({
   model: window.app.WorkingPoint,
   url: function () {
      return window.app.domainName + "/WorkingPoints/WorkingPointsPerUser";
   },
   getWorkingPointXmppAddress: function (id) {
      //the ID could be the telNumber or the shortID
      if (this.get(id) !== undefined) {
         return this.get(id).get("XMPPaddress");
      } else {
         //not the TelNumber - now try the short id
         var result = this.where({ ShortID: id });
         if (result.length === 1) {
            return result[0].get("XMPPaddress");
         }
      }
      return null;
   }
});
//#endregion

//#region Global variables
window.app.nrOfCheckedWorkingPoints = 0;
//#endregion

//#region WorkingPointView
_.templateSettings = {
   interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
   evaluate: /\{%([\s\S]+?)%\}/g,   // excute code: {% code_to_execute %}
   escape: /\{%-([\s\S]+?)%\}/g
}; // excape HTML: {%- <script> %} prints &lt

window.app.WorkingPointView = Backbone.View.extend({
   model: window.app.WorkingPoint,
   tagName: "span",     
   events: {
      "click .wpSelectorIcon": "selectedChanged"
   },
   initialize: function () {
      _.bindAll(this, 'render', 'selectedChanged', 'unrender');
      this.model.bind('destroy', this.unrender);
      return this.render;
   },
   render: function () {
      //DA we instantiate the template only when required (after the DOM has been loaded)
      this.phoneNumberTemplate = _.template($('#phoneNumber-template').html());
      this.$el.html(this.phoneNumberTemplate(this.model.toJSON()));
      this.$el.addClass("phoneNumber");
      this.$el.addClass("phoneNumberSelected");
      var selectWp = $("img", this.$el);
      setTooltipOnElement(selectWp, selectWp.attr('tooltiptitle'), 'light');
      return this;
   },
   unrender: function () {
      this.$el.remove();
   },
   selectedChanged: function () {
      //you can enable whenever you want and you can disable when you have at least 2 wps enabled
      if ((window.app.nrOfCheckedWorkingPoints >= 2) || !this.model.attributes.CheckedStatus) {
         //changing the checked status will trigger an event in the wpArea
         this.model.set('CheckedStatus', !this.model.get('CheckedStatus'));
         //change the visual status
         var checkboxImg = $("img", this.$el);
         if (this.model.get('CheckedStatus') === true) {
            this.$el.removeClass('phoneNumberUnselected');
            this.$el.addClass('phoneNumberSelected');
            setCheckboxState(checkboxImg, true);
         }
         else {
            this.$el.removeClass('phoneNumberSelected');
            this.$el.addClass('phoneNumberUnselected');
            setCheckboxState(checkboxImg, false);
         }
      }
   }
});
//#endregion

//#region WorkingPointsArea default properties
window.app.defaultWpAreaOptions = {
   checkedPhoneNumbersArray: []
};
//#endregion
window.app.WorkingPointsArea = function () {
   "use strict";
   var self = this;
   $.extend(this, window.app.defaultWpAreaOptions);

   var opts = {
      lines: 13, // The number of lines to draw
      length: 7, // The length of each line
      width: 4, // The line thickness
      radius: 10, // The radius of the inner circle
      rotate: 0, // The rotation offset
      color: '#fff', // #rgb or #rrggbb
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

   var WpPoolArea = Backbone.View.extend({
      el: $("#leftColumn"),
      initialize: function () {
         _.bindAll(this,
            'render',
            'appendWorkingPoint',
            'getWorkingPoints',
            'triggerFilteringOnCheckedStatusChange');
         this.phoneNumbersPool = new window.app.WorkingPointPool();
         this.phoneNumbersPool.bind("add", this.appendWorkingPoint, this);
         this.phoneNumbersPool.bind("reset", this.render);
      },
      getWorkingPoints: function (getConversationsFunction) {
         //#region reset internal variables
         window.app.nrOfCheckedWorkingPoints = 0;
         //#endregion
         var target = document.getElementById('leftColumn');
         spinner.spin(target);
         this.phoneNumbersPool.fetch({
            success: function () {
               spinner.stop();
               getConversationsFunction();
            }
         });
      },
      render: function () {
         var selfWpPoolView = this;
         this.phoneNumbersPool.each(function (wp) {
            selfWpPoolView.appendWorkingPoint(wp);
         });
      },
      appendWorkingPoint: function (wp) {
         var selfWpPoolView = this;
         //when an wp's checked status changes refresh the conversations list
         wp.on("change", function (wpModel) {
            selfWpPoolView.triggerFilteringOnCheckedStatusChange(wpModel);
         });
         var wpView = new window.app.WorkingPointView({ model: wp });
         window.app.nrOfCheckedWorkingPoints++;
         $(this.el).append(wpView.render().el);
      },
      triggerFilteringOnCheckedStatusChange: function (wpModel) {
         if (wpModel.get('CheckedStatus') === true) {
            window.app.nrOfCheckedWorkingPoints++;
         }
         else {
            window.app.nrOfCheckedWorkingPoints--;
         }
         self.checkedPhoneNumbersArray = [];
         _.each(this.phoneNumbersPool.models, function (wp) {
            if (wp.get('CheckedStatus') === true) {
               //We don't need to send the short ID as the WP - conversation relationship is based on the TelNumber key
               self.checkedPhoneNumbersArray.push(wp.get('TelNumber'));
            }
         });
         $(document).trigger('refreshConversationList');
      }
   });
   self.wpPoolView = new WpPoolArea();
};