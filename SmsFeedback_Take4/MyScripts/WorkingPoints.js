"use strict";

//#region WorkingPoint model
$(function () {
   window.app = window.app || {};   //window.app = window.app || { } will set window.app to an empty object if there is no window.app and will leave window.app alone if it has already been set; doing it like this makes the JavaScript files more self-contained and less subject to loading order
   window.app.WorkingPoint = Backbone.Model.extend({
      defaults: {
         TelNumber: "defaultNumber",
         Name: "defaultNumber",
         Description: "defaultDescription",
         CheckedStatus: true
      },
      idAttribute: "TelNumber"
   });
})
//#endregion 

//#region WorkingPointPool
$(function () {
   window.app = window.app || {};
   window.app.WorkingPointPool = Backbone.Collection.extend({
      model: app.WorkingPoint,
      url: function () {
         return "Messages/WorkingPointsPerUser";
      }
   });
})
//#endregion

//#region Global variables
$(function () {
   window.app = window.app || {};
   window.app.nrOfCheckedWorkingPoints = 0;
});
//#endregion

//#region WorkingPointView
$(function () {
   window.app = window.app || {};

   _.templateSettings = {
      interpolate: /\{\{(.+?)\}\}/g
   };
   window.app.WorkingPointView = Backbone.View.extend({
      model: app.WorkingPoint,
      tagName: "span",
      phoneNumberTemplate: _.template($('#phoneNumber-template').html()),
      events: {
         "click .wpSelectorIcon": "selectedChanged"
      },
      initialize: function () {
         _.bindAll(this, 'render', 'selectedChanged');
         this.model.bind('destroy', this.unrender, this);
         return this.render;
      },
      render: function () {
         this.$el.html(this.phoneNumberTemplate(this.model.toJSON()));
         this.$el.addClass("phoneNumber");
         this.$el.addClass("phoneNumberSelected");
         return this;
      },
      unrender: function () {
         this.$el.remove();
      },
      selectedChanged: function () {
         //you can enable whenever you want and you can disable when you have at least 2 wps enabled
         if ((app.nrOfCheckedWorkingPoints >= 2) || !this.model.attributes.CheckedStatus) {
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
})
//#endregion

function WorkingPointsArea() {
    var self = this;   
    self.checkedPhoneNumbersArray = [];

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
        el: $("#phoneNumbersPool"),
        initialize: function () {
           _.bindAll(this,
              'render',
              'appendWorkingPoint',
              'getWorkingPoints',
              'triggerFilteringOnCheckedStatusChange');
           this.phoneNumbersPool = new app.WorkingPointPool();
           this.phoneNumbersPool.bind("add", this.appendWorkingPoint, this);
           this.phoneNumbersPool.bind("reset", this.render);          
        },
        getWorkingPoints: function () {
           //#region reset internal variables
           app.nrOfCheckedWorkingPoints = 0;
           //#endregion
           var target = document.getElementById('phoneNumbersPool');
           spinner.spin(target);           
           this.phoneNumbersPool.fetch({
              success: function () {
                 spinner.stop();
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
           //when an wp's checked status changed refresh the conversations list
           wp.on("change", function (wpModel) {
              selfWpPoolView.triggerFilteringOnCheckedStatusChange(wpModel);
           });
           var wpView = new app.WorkingPointView({ model: wp });

           app.nrOfCheckedWorkingPoints++;
           $(this.el).append(wpView.render().el);
        },
        triggerFilteringOnCheckedStatusChange: function (wpModel) {
           if (wpModel.get('CheckedStatus') === true) {
              app.nrOfCheckedWorkingPoints++;
           }
           else {
              app.nrOfCheckedWorkingPoints--;
           }                         
           self.checkedPhoneNumbersArray = [];
           _.each(this.phoneNumbersPool.models, function (wp) {
              if (wp.get('CheckedStatus') === true) {
                 self.checkedPhoneNumbersArray.push(wp.get('TelNumber'));
              }
           });
           $(document).trigger('selectedWPsChanged');
        }
    });
    self.wpPoolView = new WpPoolArea();   
}