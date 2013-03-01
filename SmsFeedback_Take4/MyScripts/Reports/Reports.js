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
/*global Spinner */
/*global FirstArea */
/*global ThirdArea */
/*global SecondArea */
//#endregion
window.app = window.app || {};
window.reports = window.reports || {};
window.app.dateFormatForDatePicker = 'dd/mm/yy';

_.templateSettings = {
   interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
   evaluate: /\{%([\s\S]+?)%\}/g,   // excute code: {% code_to_execute %}
   escape: /\{%-([\s\S]+?)%\}/g
}; // excape HTML: {%- <script> %} prints &lt

function drawThisArea(element, indexOfTheElement) {
   element.drawArea();
}

/* Section identifier can have one of the following values:
    1. FirstSection - data for the main chart/s
    2. SecondSection  - data for info boxes
    3. ThirdSection  - data for table/s and other charts
*/
var ReportModel = Backbone.Model.extend({
   reportId: 1,
   title: "Total sms report",
   source: "/Reports/GetReportOverviewData",
   sections: [
               {
                   type: "FirstSection",
                   id: "4", // only one section can have this id
                   groupId: "xt4ga", // more than one section can have this id. Used to group sections
                   title: "Get total no of sms report",
                   options: {
                       seriesType: "area",
                       colors: ["#ccc7f1", "#459aaa"]
                   },
                   tooltip: "no tooltip",
                   dataIndex: 1
               }
   ]
});

var ReportsContentArea = Backbone.View.extend({
   el: $("#rightColumn"),
   initialize: function () {
      _.bindAll(this, 'render', 'setupEnvironment', 'updateReport', 'renderSection');
      window.app.areas = [];
      this.FIRST_SECTION = "FirstSection";
      this.SECOND_SECTION = "SecondSection";
      this.THIRD_SECTION = "ThirdSection";
      this.loadReportData();      
   },
   render: function () {      
       
   },
   loadReportData: function () {
       var self = this;
       window.app.areas = [];
       var template = _.template($("#report-template").html(), this.model.toJSON());
       // Load the compiled HTML into the Backbone "el"
       $(this.el).html(template);
       $("#secondSection").empty();
       $("#reportScope").html(" :: " + window.app.currentWorkingPointFriendlyName);
       this.transition = new Transition(document.getElementById('rightColumn'), $("#overlay"));
       this.transition.startTransition();
       
       var jsonData = $.ajax({
           data: {
               iIntervalStart: window.app.dateHelper.transformDate(window.app.startDate),
               iIntervalEnd: window.app.dateHelper.transformDate(window.app.endDate),
               iScope: window.app.currentWorkingPoint
           },
           url: window.app.domainName + self.model.get("source"),
           dataType: "json",
           async: false,
           success: function (data) {
               for (var k = 0; k < self.model.get("sections").length; ++k) {
                   self.renderSection(self.model.get("sections")[k], data);                       
               }
               $("#secondSection").append("<div class='clear'></div>");
               self.setupEnvironment(false);
               self.transition.endTransition();
               $(document).trigger("resize");
           }
       }).responseText;
   },
   renderSection: function (model, data) {
        
       if (model.type === this.FIRST_SECTION) {
           var template = _.template($("#" + model.type).html(), model);
           $("#firstSection").append(template);
           var firstArea = new FirstArea(model);
           firstArea.load(data.charts[model.dataIndex]);
           window.app.areas[model.id] = firstArea;
       }  else if (model.type === this.SECOND_SECTION) {           
           var secondArea = new SecondArea(model);
           secondArea.load(data.infoBoxes[model.dataIndex]);
           window.app.areas[model.id] = secondArea;
       } else if (model.type === this.THIRD_SECTION) {
           var template = _.template($("#" + model.type).html(), model);
           $("#firstSection").append(template);
           var thirdArea = new ThirdArea();
           thirdArea.load(data.charts[model.dataIndex]);
           window.app.areas[model.id] = thirdArea;
       }
   },
   setupEnvironment: function (displayTooltip) {
      // Hover tooltip
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
      $(".exportBtn").qtip({
         content: $(".exportBtn").attr('tooltiptitle'),
         position: {
            corner: {
               target: 'leftMiddle',
               tooltip: 'rightMiddle'
            }
         },
         style: 'dark'
      });

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
       this.loadReportData();
       $("#reportScope").html(" :: " + window.app.currentWorkingPointFriendlyName);      
   }
});

window.app.leftSideMenus = {}; //this will hold the FriendlyName <-> Action correspondence 
var ReportsArea = function () {
   "use strict";
   var self = this;
   var localReportsRepository = {};

   // initializing leftColumn
   var reportsMenu = new window.app.MenuView({
      el: $("#leftColumn"),
      eventToTriggerOnSelect: 'switchReport',
      menuCollection: new window.app.MenuCollection({ url: '/Reports/getReportsMenuItems' }),
      afterInitializeFunction: function (menuItems) {
          _(menuItems.models).each(function (menuItemModel) {
            //DA the idea is to correctly connect an route to an action -> we need this "actions map"
            if (menuItemModel.get("parent") != 0) {
               window.app.leftSideMenus[menuItemModel.get("FriendlyName")] = {
                  id: menuItemModel.get("itemId"),
                  action: menuItemModel.get("Action")
               };
            }
         });
         //#region Routing
         var ReportsRooter = Backbone.Router.extend({
            routes: {
               '': 'defaultCall',
               ":menu": "goToMenu"
            },
            goToMenu: function (menuOption) {
               //call the appropriate function
               var action = window.app.leftSideMenus[menuOption];
               if (action !== undefined) {
                  //DA the reports are a bit different from settings
                  //all the reports call the same function, with a different report ID
                  var id = action.id;
                  self.loadReport(id);                  
                     //DA this is some fucked up code :)
                     //is should MenuView instance
                     //we operate directly on the DOM (we should not do this)
                     var liItem = ".liItem" + action.id;
                     if (!$(liItem).hasClass("menuItemSelected")) {
                        $(liItem).parents(".collapsibleList").find(".menuItemSelected").removeClass("menuItemSelected")
                        $(liItem).addClass("menuItemSelected");
                        //make sure the parent is expanded
                        //this follows the logic that the parent is 10, 20, 30 and the leafs are 11,12 .. 21, 22
                        var parentID = Math.floor(action.id / 10) * 10;
                        var parentUlItem = "ul.item" + parentID;
                        $(parentUlItem).css("display", "block");
                     }                  
               }
               else {
                  //DA this means that we don't have access to that function -> go with the default call
                  this.defaultCall();
               }
            },
            defaultCall: function () {
               // mark the first opened report
               $(".liItem11").addClass("menuItemSelected");
               $("ul.item10").css("display", "block");               
               window.reports.router.navigate('/ConversationsOverview', { trigger: true });
            }            
         });
         window.reports.router = new ReportsRooter();
         Backbone.history.start();
         //#endregion
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
      //self.loadReport(menuOptions.menuId);
      window.reports.router.navigate('/' + menuOptions.menuNavigation, { trigger: true });
   });

   this.redrawContent = function () {
       reportsContent.loadReportData();
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
      $.getJSON(window.app.domainName + '/WorkingPoints/WorkingPointsPerUser',
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

   // initial setup of the page
   $("#overlay").hide();
   this.loadWorkingPoints();
};