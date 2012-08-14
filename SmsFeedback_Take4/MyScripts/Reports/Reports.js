"use strict";
window.app = window.app || {};

_.templateSettings = {
    interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
    evaluate: /\{%([\s\S]+?)%\}/g,   // excute code: {% code_to_execute %}
    escape: /\{%-([\s\S]+?)%\}/g
}; // excape HTML: {%- <script> %} prints &lt


var ReportModel = Backbone.Model.extend({
    reportId: 1,
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

var ReportsMenuItemModel = Backbone.Model.extend({
    defaults: {
        itemId: 1,
        itemName: "Conversation",
        leaf: false,
        parent: 1
    }
});

var ReportsMenu = Backbone.Collection.extend({
    model: ReportsMenuItemModel,
    url: function () {
        return window.app.domainName + "/Reports/getReportsMenuItems";
    }
});

var ReportsMenuItemView = Backbone.View.extend({
    tagName: 'li',
    initialize: function () {
        _.bindAll(this, 'render');
    },
    renderParent: function () {
        $(this.el).html("<span class='reportMenuParentItem'>" + this.model.get("itemName") + "</span>" + "<ul class='item" + this.model.get("itemId") + "'></ul>");
        return this;
    },
    renderLeaf: function () {
       $(this.el).addClass('innerLi');
        $(this.el).html("<span class='reportMenuLeafItem' reportId='" + this.model.get("itemId") + "'>" + this.model.get("itemName") + "</span>");
        return this;
    }
});

var ReportsMenuView = Backbone.View.extend({
    el: $("#leftColumn"),
    initialize: function () {
        _.bindAll(this, 'render');
        var self = this;
        this.menuItems = new ReportsMenu();

        this.menuItems.fetch({
            success: function () {
                self.render();
            }
        });
        //this.render();
    },
    render: function () {
        var self = this;
        $(this.el).append("<ul class='primaryList collapsibleList'></ul>");
        _(this.menuItems.models).each(function (menuItemModel) {
            var reportsMenuItemView = new ReportsMenuItemView({ model: menuItemModel });
            if (!menuItemModel.get("leaf")) {
                $("ul.primaryList", self.el).append(reportsMenuItemView.renderParent().el);
            } else {
                var selector = ".item" + menuItemModel.get("parent");
                $(selector, self.el).append(reportsMenuItemView.renderLeaf().el);
            }
        });
        $(".reportMenuLeafItem").click(function () {
            $("*").removeClass("reportMenuItemSelected");
            $(this).addClass("reportMenuItemSelected");
            $(document).trigger("switchReport", $(this).attr("reportId"));
        });
        CollapsibleLists.apply();
        
    }    
});

var ReportsContentArea = Backbone.View.extend({
    el: $("#rightColumn"),
    initialize: function () {
        _.bindAll(this, 'render', 'setupEnvironment');
        this.render();
    },
    render: function () {
        var template = _.template($("#report-template").html(), this.model.toJSON());
        // Load the compiled HTML into the Backbone "el"
        $(this.el).html(template);
        this.setupEnvironment();
    },
    setupEnvironment: function () {
        // after rendering the template, initialize the scripts that will do the magic.
        for (var k = 0; k < this.model.get("sections").length; ++k) {
            if (this.model.get("sections")[k].visibility && (this.model.get("sections")[k].identifier === "PrimaryChartArea")) {
                window.app.firstArea = new FirstArea(this.model.get("sections")[k].resources[0].source, "day");
                window.app.firstArea.drawArea();
            } else if (this.model.get("sections")[k].visibility && (this.model.get("sections")[k].identifier == "SecondaryChartArea")) {
                window.app.thirdArea = new ThirdArea(this.model.get("sections")[k].resources[0].source);
                window.app.thirdArea.drawArea();
            } else if (this.model.get("sections")[k].visibility && (this.model.get("sections")[k].identifier === "InfoBox")) {
                window.app.secondArea = new SecondArea(this.model.get("sections")[k].resources);
                window.app.secondArea.drawArea();
            }
        }

        //setTimeout(function () { }, 200);

        // Calendar setup
        $('#widgetCalendar').DatePicker({
            flat: true,
            format: 'd B, Y',
            date: [window.app.dateHelper.transformDate(window.app.startDate), window.app.dateHelper.transformDate(window.app.endDate)],
            calendars: 3,
            mode: 'range',
            starts: 1,
            onChange: function (formated) {
                // $('#widgetField span').get(0).innerHTML = formated.join(' &divide; '); // old formatting
                window.app.newStartDate = new Date(formated[0]);
                window.app.newEndDate = new Date(formated[1]);
                $('#widgetField span').get(0).innerHTML = window.app.dateHelper.transformDateToLocal(window.app.newStartDate) + " - " + window.app.dateHelper.transformDateToLocal(window.app.newEndDate);

            }
        });
        var state = false;
        $('#widgetField>a').bind('click', function () {
            $('#widgetCalendar').stop().animate({ height: state ? 0 : $('#widgetCalendar div.datepicker').get(0).offsetHeight }, 500);
            state = !state;
            // Datepicker was closed
            if (!state) {
                if (window.app.startDate !== window.app.newStartDate || window.app.endDate !== window.app.newEndDate) {
                    window.app.startDate = window.app.newStartDate;
                    window.app.endDate = window.app.newEndDate;
                    $(document).trigger("intervalChanged");
                }
            }
            return false;
        });
        $('#reportRange div.datepicker').css('position', 'absolute');

        // Setup grannularity buttons. TODO: rename the radio buttons group more appropriate.
        $("#radio").buttonsetv();
        $("#radio").show();
        $(".radioOption").change(function () {
            window.app.firstArea.setGranularity($(this).val());
            window.app.firstArea.drawArea();
        });        
    }

});

var ReportsArea = function () {
   var self = this;
   var localReportsRepository = {};

   // initializing leftColumn
   var reportsMenu = new ReportsMenuView({ el: $("#leftColumn") });
   // initializing rightColumn
   var reportsContent;


   $(document).bind("workingPointChanged", function (event, newWorkingPoint) {
      self.changeWorkingPoint(newWorkingPoint);
   });

   $(document).bind("intervalChanged", function (event) {
      self.changeReportingInterval();
   });

   $(document).bind("switchReport", function (event, reportId) {
      self.loadReport(reportId);
   });

   this.redrawContent = function () {
      reportsContent.render();
   };

   this.changeWorkingPoint = function (newWorkingPoint) {
      window.app.currentWorkingPoint = newWorkingPoint;
      reportsContent.render();
   };

   this.changeReportingInterval = function () {
      reportsContent.render();
   };

   this.displayReport = function (reportModel) {
      reportsContent = new ReportsContentArea({ model: reportModel, el: $("#rightColumn") });
   };

   this.loadReport = function (reportId) {

      if (localReportsRepository[reportId] === undefined) {
         $.getJSON('Reports/getReportById',
               { reportId: reportId },
               function (data) {
                  var receivedReportModel = new ReportModel(data);
                  localReportsRepository[reportId] = receivedReportModel;
                  self.displayReport(localReportsRepository[reportId]);
               }
            );

      } else {
         // load report from local repository
         self.displayReport(localReportsRepository[reportId]);
      }
   };

   this.loadWorkingPoints = function () {
      $.getJSON('/Messages/WorkingPointsPerUser',
               {},
               function (data) {
                  var workingPointsSelectorContent = "<option value='Global'>Global</option>";
                  for (var i = 0; i < data.length; ++i) {
                     workingPointsSelectorContent += "<option value='" + data[i].Name + "'>" + data[i].Name + "</option>";
                  }
                  $("#workingPointSelector").append(workingPointsSelectorContent);
               }
            );
   };

   this.loadReport(2);

   // initial setup of the page
   $("#overlay").hide();
   this.loadWorkingPoints();
};