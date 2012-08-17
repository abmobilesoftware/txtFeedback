"use strict";
window.app = window.app || {};

_.templateSettings = {
    interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
    evaluate: /\{%([\s\S]+?)%\}/g,   // excute code: {% code_to_execute %}
    escape: /\{%-([\s\S]+?)%\}/g
}; // excape HTML: {%- <script> %} prints &lt

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

var ReportsContentArea = Backbone.View.extend({
    el: $("#rightColumn"),
    initialize: function () {
        _.bindAll(this, 'render', 'setupEnvironment', 'updateReport');
        this.render();
    },
    render: function () {
        var template = _.template($("#report-template").html(), this.model.toJSON());
        // Load the compiled HTML into the Backbone "el"
        $(this.el).html(template);
        $("#reportScope").html(" :: " + window.app.currentWorkingPointFriendlyName);
        this.setupEnvironment();
        $(document).trigger("resize");
    },
    setupEnvironment: function () {
        // after rendering the template, initialize the scripts that will do the magic.
        window.app.areas = [];
        for (var k = 0; k < this.model.get("sections").length; ++k) {
            if (this.model.get("sections")[k].visibility && (this.model.get("sections")[k].identifier === "PrimaryChartArea")) {
                window.app.firstArea = new FirstArea(this.model.get("sections")[k].resources[0].source, "day", this.model.get("sections")[k].resources[0].options);
                window.app.firstArea.drawArea();
                window.app.areas.push(window.app.firstArea);
            } else if (this.model.get("sections")[k].visibility && (this.model.get("sections")[k].identifier == "SecondaryChartArea")) {
                window.app.thirdArea = new ThirdArea(this.model.get("sections")[k].resources[0].source);
                window.app.thirdArea.drawArea();
                window.app.areas.push(window.app.thirdArea);
            } else if (this.model.get("sections")[k].visibility && (this.model.get("sections")[k].identifier === "InfoBox")) {
                window.app.secondArea = new SecondArea(this.model.get("sections")[k].resources);
                window.app.secondArea.drawArea();
                window.app.areas.push(window.app.secondArea);
            }
        }

        // Setup the calendar
        $("#from").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            numberOfMonths: 3,
            onSelect: function (selectedDate) {
                $("#to").datepicker("option", "minDate", selectedDate);
                var day = selectedDate.substring(0, 2);
                var month = selectedDate.substring(3, 5) - 1;
                var year = selectedDate.substring(6, selectedDate.length);
                window.app.newStartDate = new Date(year, month, day);

                if (window.app.newStartDate != window.app.startDate) {
                    window.app.startDate = window.app.newStartDate;
                    window.app.endDate = window.app.newEndDate;
                    $(document).trigger("intervalChanged");
                }
            }
        });
        $("#to").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            numberOfMonths: 3,
            onSelect: function (selectedDate) {
                $("#from").datepicker("option", "maxDate", selectedDate);
                var day = selectedDate.substring(0, 2);
                var month = selectedDate.substring(3, 5) - 1;
                var year = selectedDate.substring(6, selectedDate.length);
                window.app.newEndDate = new Date(year, month, day);

                if (window.app.newEndDate != window.app.endDate) {
                    window.app.startDate = window.app.newStartDate;
                    window.app.endDate = window.app.newEndDate;
                    $(document).trigger("intervalChanged");
                }
            }
        });
        // Setup the calendar culture
        var fromTranslation = $("#from").val();
        var toTranslation = $("#to").val();

        $("#from").datepicker("option", $.datepicker.regional[window.app.calendarCulture]);
        $("#to").datepicker("option", $.datepicker.regional[window.app.calendarCulture]);

        $("#from").val(fromTranslation);
        $("#to").val(toTranslation);

        // Setup grannularity buttons. TODO: rename the radio buttons group more appropriate.
        //$("#granularitySelector").show();
        $(".radioOption").change(function () {
            $(this).parents("#granularitySelector").find(".active").removeClass("active");
            $(this).parents(".radioBtnWrapper").addClass("active");
            
            window.app.firstArea.setGranularity($(this).val());
            window.app.firstArea.drawArea();
        });       
    },
    updateReport: function () {
        $("#reportScope").html(" :: " + window.app.currentWorkingPointFriendlyName);
        /*for (i = 0; i < window.app.areas.length; ++i) {
            window.app.areas[i].drawArea();
        }*/
        window.app.firstArea.drawArea();
        window.app.secondArea.drawArea();
        window.app.thirdArea.drawArea();
    }
});

var ReportsArea = function () {
   var self = this;
   var localReportsRepository = {};

   // initializing leftColumn
   var reportsMenu = new window.app.MenuView({
      el: $("#leftColumn"),
      eventToTriggerOnSelect: 'switchReport',
      menuCollection: new window.app.MenuCollection({ url: '/Reports/getReportsMenuItems' })
   });
   // initializing rightColumn
   var reportsContent;


   $(document).bind("workingPointChanged", function (event, newWorkingPoint) {
      self.changeWorkingPoint(newWorkingPoint);
   });

   $(document).bind("intervalChanged", function (event) {
      self.changeReportingInterval();
   });

   $(document).bind("switchReport", function (event, menuId) {
      self.loadReport(menuId);
   });

   this.redrawContent = function () {
      reportsContent.render();
   };

   this.changeWorkingPoint = function (newWorkingPoint) {
      window.app.currentWorkingPoint = newWorkingPoint;
        reportsContent.updateReport();
   };

   this.changeReportingInterval = function () {
        reportsContent.updateReport();
   };

   this.displayReport = function (reportModel) {
      reportsContent = new ReportsContentArea({ model: reportModel, el: $("#rightColumn") });
   };

    this.loadReport = function(menuId) {        

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