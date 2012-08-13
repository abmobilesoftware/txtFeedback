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
        $(this.el).addClass('outerLi');
        $(this.el).addClass('listItem');
        $(this.el).html("<div class='reportMenuParentItem'>" + this.model.get("itemName") + "</div>" + "<ul class='item" + this.model.get("itemId") + "'></ul>");
        return this;
    },
    renderLeaf: function () {
        $(this.el).addClass('innerLi');
        $(this.el).addClass('listItem');
        $(this.el).addClass('liItem' + this.model.get("itemId"));
        $(this.el).attr("reportId", this.model.get("itemId"));
        $(this.el).html("<span class='reportMenuLeafItem' reportId='" + this.model.get("itemId") + "'>" + this.model.get("itemName") + "</span>");
        return this;
    }
});

var ReportsMenuView = Backbone.View.extend({
    el: $("#leftColumn"),
    initialize: function () {
        _.bindAll(this, 'render');
        self = this;
        this.menuItems = new ReportsMenu();
        this.menuItems.fetch({
            success: function () {
                self.render();
            }
        });
        //this.render();
    },
    render: function () {
        self = this;
        $(this.el).append("<ul class='primaryList collapsibleList'></ul>");
        _(this.menuItems.models).each(function (menuItemModel) {
            var reportsMenuItemView = new ReportsMenuItemView({ model: menuItemModel });
            if (!menuItemModel.get("leaf")) {
                if (menuItemModel.get("parent") == 0) {
                    $("ul.primaryList", self.el).append(reportsMenuItemView.renderParent().el);
                } else {
                    var selector = ".item" + menuItemModel.get("parent");
                    $(selector, self.el).append(reportsMenuItemView.renderParent().el);
                }
            } else {
                var selector = ".item" + menuItemModel.get("parent");
                $(selector, self.el).append(reportsMenuItemView.renderLeaf().el);
            }
        });
        
        // open report functionality
        $(".innerLi").click(function () {
            $(this).parents(".collapsibleList").find(".reportMenuItemSelected").removeClass("reportMenuItemSelected");
            $(this).addClass("reportMenuItemSelected");
            $(document).trigger("switchReport", $(this).attr("reportId"));
        });

        // apply collapsible functionality to list 
        CollapsibleLists.apply();

        // mark the first opened report
        $(".liItem2").addClass("reportMenuItemSelected");
        $("ul.item1").css("display", "block");
        
    }    
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
        

        this.setupEnvironment();
    },
    setupEnvironment: function () {
        // after rendering the template, initialize the scripts that will do the magic.
        for (k = 0; k < this.model.get("sections").length; ++k) {
            if (this.model.get("sections")[k].visibility && (this.model.get("sections")[k].identifier == "PrimaryChartArea")) {
                window.app.firstArea = new FirstArea(this.model.get("sections")[k].resources[0].source, "day");
                window.app.firstArea.drawArea();
            } else if (this.model.get("sections")[k].visibility && (this.model.get("sections")[k].identifier == "SecondaryChartArea")) {
                window.app.thirdArea = new ThirdArea(this.model.get("sections")[k].resources[0].source);
                window.app.thirdArea.drawArea();
            } else if (this.model.get("sections")[k].visibility && (this.model.get("sections")[k].identifier == "InfoBox")) {
                window.app.secondArea = new SecondArea(this.model.get("sections")[k].resources);
                window.app.secondArea.drawArea();
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
        $("#granularitySelector").show();
        $(".radioOption").change(function () {
            $(this).parents("#granularitySelector").find(".active").removeClass("active");
            $(this).parents(".radioBtnWrapper").addClass("active");
            
            window.app.firstArea.setGranularity($(this).val());
            window.app.firstArea.drawArea();
        });

        
    },
    updateReport: function () {
        window.app.firstArea.drawArea();
        window.app.secondArea.drawArea();
        window.app.thirdArea.drawArea();
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
    }

    this.changeWorkingPoint = function (newWorkingPoint) {
        window.app.currentWorkingPoint = newWorkingPoint;
        reportsContent.updateReport();
    }

    this.changeReportingInterval = function () {
        reportsContent.updateReport();
    }

    this.displayReport = function (reportModel) {
        reportsContent = new ReportsContentArea({ model: reportModel, el: $("#rightColumn") });
    }

    this.loadReport = function(reportId) {
        
        if (localReportsRepository[reportId] == undefined) {
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
    }

    this.loadWorkingPoints = function () {
        $.getJSON(window.app.domainName + '/Messages/WorkingPointsPerUser',
               {},
               function (data) {
                   var workingPointsSelectorContent = "<option value='Global'>Global</option>";
                   for (i = 0; i < data.length; ++i) {
                       workingPointsSelectorContent += "<option value='" + data[i].TelNumber + "'>" + data[i].Name + "</option>";
                   }
                   $("#workingPointSelector").append(workingPointsSelectorContent);
               }
            );
    }

    this.loadReport(2);
    
    // initial setup of the page
    $("#overlay").hide();
    this.loadWorkingPoints();
             
    

}

