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
/*global FirstArea */
/*global ThirdArea */
/*global SecondArea */
//#endregion
window.app = window.app || {};
window.app.dateFormatForDatePicker = 'dd/mm/yy';

_.templateSettings = {
   interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
   evaluate: /\{%([\s\S]+?)%\}/g,   // excute code: {% code_to_execute %}
   escape: /\{%-([\s\S]+?)%\}/g
}; // excape HTML: {%- <script> %} prints &lt

function drawThisArea(element, indexOfTheElement) {
   element.drawArea();
}

var ReportModel = Backbone.Model.extend({
   menuId: 1,
   title: "Total sms report",
   scope: "Global",
   sections: [
               {
                  identifier: "PrimaryChartArea", visibility: true, resources: [
                                                                                 { name: "Get total no of sms report", source: "/Reports/getTotalNoOfSms" }
                  ]
               },
               {
                  identifier: "InfoBox", visibility: true, resources: [
                                                                          { name: "Total no of sms", source: "Reports/getNoOfSms" }
                  ]
               },
               {
                  identifier: "AdditionalChartArea", visibility: false, resources: []
               }
   ]
});

var Transition = function () {
   "use strict";
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
   var target = document.getElementById('chartArea');

   this.startTransition = function () {
      spinner.spin(target);
      $("#overlay").show();
   };

   this.endTransition = function () {
      spinner.stop();
      $("#overlay").hide();
   };
};

var ReportsContentArea = Backbone.View.extend({
   el: $("#rightColumn"),
   initialize: function () {
      _.bindAll(this, 'render', 'setupEnvironment', 'updateReport', 'renderSection');
      this.reportContentElement = $("#reportContent");
      window.app.areas = [];
      this.render();
   },
   render: function () {
      this.transition = new Transition();
      this.transition.startTransition();

      var template = _.template($("#report-template").html(), this.model.toJSON());
      // Load the compiled HTML into the Backbone "el"
      $(this.el).html(template);
      $("#reportScope").html(" :: " + window.app.currentWorkingPointFriendlyName);
      var displayTooltip = false;
      for (var k = 0; k < this.model.get("sections").length; ++k) {
         if (this.model.get("sections")[k].visibility) {
            // TODO: Naming refactoring
            this.renderSection("#" + this.model.get("sections")[k].identifier,
               this.model.get("sections")[k].uniqueId,
               this.model.get("sections")[k].sectionId,
               this.model.get("sections")[k].resources);

            if (this.model.get("sections")[k].resources[0].tooltip !== "no tooltip") { displayTooltip = true; }
         }
      }

      this.setupEnvironment(displayTooltip);

      this.transition.endTransition();
      // resize event is triggered here, because after populating the divs with content the page height will change
      $(document).trigger("resize");
   },
   renderSection: function (section, uniqueId, sectionId, resources) {
      var parameters = resources[0];
      parameters.uniqueId = uniqueId;
      parameters.sectionId = sectionId;
      var template = _.template($(section).html(), parameters);
      $("#reportContent").append(template);
      if (section === "#PrimaryChartArea") {
         var area = new FirstArea(resources[0].source, "day", resources[0].options, uniqueId, resources[0].tooltip, resources[0].name);
         area.drawArea();
         window.app.areas[uniqueId] = area;
         //window.app.areas.push(area);
      } else if (section === "#SecondaryChartArea") {
         window.app.thirdArea = new ThirdArea(resources[0].source);
         window.app.thirdArea.drawArea();
         //window.app.areas.push(window.app.thirdArea);
         window.app.areas[uniqueId] = window.app.thirdArea;
        } else if (section === "#InfoBox") {
         window.app.secondArea = new SecondArea(resources);
         window.app.secondArea.drawArea();
         //window.app.areas.push(window.app.secondArea);
         window.app.areas[uniqueId] = window.app.secondArea;
      }
   },
   setupEnvironment: function (displayTooltip) {
      //$("#granularitySelector").show();
      if (displayTooltip) {
         var infoBoxElement = $(".chartAreaTitle");
         infoBoxElement.qtip({
            content: infoBoxElement.attr('tooltiptitle'),
            position: {
               corner: {
                  target: 'bottomMiddle',
                  tooltip: 'topMiddle'
               }
            },
            style: 'dark'
         });
      }
      $(".chartAreaTitle").click(function (event) {
         event.preventDefault();
         var sectionId = $(this).attr("sectionId");
         var elementName = ".chartAreaContent" + sectionId;
         var descriptionElement = "#description" + sectionId;
         if ($(elementName).is(":visible")) {
            $(elementName).hide();
            $(this).children(".sectionVisibility").attr("src", "/Content/images/maximize_square.png");
            $(descriptionElement).show();
            $(document).trigger("resize");
         }
         else {
            $(elementName).show();
            $(this).children(".sectionVisibility").attr("src", "/Content/images/minimize_square.png");
            $(descriptionElement).hide();
            $(document).trigger("resize");
         }
      });

      var fromDatepicker = $("#from");
      // Setup the calendar
      fromDatepicker.datepicker({
         defaultDate: "+1w",
         changeMonth: true,
         numberOfMonths: 3,
         onSelect: function (selectedDate) {
            window.app.newStartDate = fromDatepicker.datepicker("getDate");
            if (window.app.newStartDate !== window.app.startDate) {
               window.app.startDate = window.app.newStartDate;
               window.app.endDate = window.app.newEndDate;
               var fromDateString = $.datepicker.formatDate(window.app.dateFormatForDatePicker, window.app.startDate);
               toDatepicker.datepicker("option", "minDate", fromDateString);
               $(document).trigger("intervalChanged");
            }
         }
      });

      var toDatepicker = $("#to");
      toDatepicker.datepicker({
         defaultDate: "+1w",
         changeMonth: true,
         numberOfMonths: 3,
         onSelect: function (selectedDate) {
            window.app.newEndDate = toDatepicker.datepicker("getDate");
            if (window.app.newEndDate !== window.app.endDate) {
               window.app.startDate = window.app.newStartDate;
               window.app.endDate = window.app.newEndDate;
               var endDateString = $.datepicker.formatDate(window.app.dateFormatForDatePicker, window.app.endDate);
               fromDatepicker.datepicker("option", "maxDate", endDateString);
               $(document).trigger("intervalChanged");
            }
         }
      });

      var fromDateString = $.datepicker.formatDate(window.app.dateFormatForDatePicker, window.app.startDate);
      var toDateString = $.datepicker.formatDate(window.app.dateFormatForDatePicker, window.app.endDate);
      var today = new Date();
      //var todayString = $.datepicker.formatDate(window.app.dateFormatForDatePicker, today);

      // Setup the calendar culture
      $.datepicker.regional[window.app.calendarCulture].dateFormat = window.app.dateFormatForDatePicker;
      var culture = $.datepicker.regional[window.app.calendarCulture];
      $("#from").datepicker("option", culture);
      $("#to").datepicker("option", culture);

      // Min/max dates and current date values
      $("#to").datepicker("option", "minDate", fromDateString);
      $("#to").datepicker("option", "maxDate", today);
      $("#from").datepicker("option", "maxDate", toDateString);
      $("#from").val(fromDateString);
      $("#to").val(toDateString);
   },
   updateReport: function () {
      $("#reportScope").html(" :: " + window.app.currentWorkingPointFriendlyName);
      /*for (i = 0; i < window.app.areas.length; ++i) {
          window.app.areas[i].drawArea();
      }        
      window.app.firstArea.drawArea();
      window.app.secondArea.drawArea();
      window.app.thirdArea.drawArea();
      */
      window.app.areas.forEach(drawThisArea);
   }
});

var ReportsArea = function () {
   "use strict";
   var self = this;
   var localReportsRepository = {};

   // initializing leftColumn
   var reportsMenu = new window.app.MenuView({
      el: $("#leftColumn"),
      eventToTriggerOnSelect: 'switchReport',
      menuCollection: new window.app.MenuCollection({ url: '/Reports/getReportsMenuItems' }),
      afterInitializeFunction: function () {
         // mark the first opened report
         $(".liItem2").addClass("menuItemSelected");
         $("ul.item1").css("display", "block");
      }
   });


   // initializing rightColumn
   var reportsContent;


   $(document).bind("workingPointChanged", function (event, newWorkingPoint) {
      self.changeWorkingPoint(newWorkingPoint);
   });

   $(document).bind("intervalChanged", function (event) {
      self.changeReportingInterval();
   });

   $(document).bind("switchReport", function (event, menuOptions) {
      self.loadReport(menuOptions.menuId);
   });

   this.redrawContent = function () {
      reportsContent.render();
   };

   this.changeWorkingPoint = function (newWorkingPoint) {
      window.app.currentWorkingPoint = newWorkingPoint;
      reportsContent.updateReport();
   };

   this.changeReportingInterval = function () {
      $("from").datepicker('hide');
      $("to").datepicker('hide');
      reportsContent.updateReport();
   };

   this.displayReport = function (reportModel) {
      reportsContent = new ReportsContentArea({ model: reportModel, el: $("#rightColumn") });
   };

   this.loadReport = function (menuId) {

      if (localReportsRepository[menuId] === undefined) {
         $.getJSON('Reports/getReportById',
               { reportId: menuId },
               function (data) {
                  var receivedReportModel = new ReportModel(data);
                  localReportsRepository[menuId] = receivedReportModel;
                  self.displayReport(localReportsRepository[menuId]);
               }
            );

      } else {
         // load report from local repository
         self.displayReport(localReportsRepository[menuId]);
      }
   };

   this.loadWorkingPoints = function () {
      $.getJSON(window.app.domainName + '/Messages/WorkingPointsPerUser',
             {},
             function (data) {
                var workingPointsSelectorContent = "<option value='Global'>Global</option>";
                for (var i = 0; i < data.length; ++i) {
                   workingPointsSelectorContent += "<option value='" + data[i].TelNumber + "'>" + data[i].Name + "</option>";
                }
                $("#workingPointSelector").append(workingPointsSelectorContent);

                // Default setup of the page
                $("#workingPointSelector").val("Global");
                $("#workingPointSelector").change(function () {
                   window.app.currentWorkingPointFriendlyName = $("#workingPointSelector").children("option").filter(":selected").text();
                   $(document).trigger("workingPointChanged", $(this).val());
                });

             }
          );
   };

   this.loadReport(2);

   // initial setup of the page
   $("#overlay").hide();
   this.loadWorkingPoints();
};