window.app = window.app || {};

function FirstArea(iResource, iGranularity, iOptions, iId) {
    var self = this;
    var resource = iResource;
    var granularity = iGranularity;
    var chart = null;
    var data = null;
    var options = {
        animation: {
            duration: 1000,
            easing: 'out'
        },
        vAxis: { gridlines: { count: 4 } }
    };
    options.seriesType = iOptions.seriesType;
    options.colors = iOptions.colors;

    var identifier = iId;
    
    // create div in chart_div
    if (options.seriesType == "area") {
        chart = new google.visualization.AreaChart(document.getElementById("chart_div" + iId));
        options.pointSize = 6;
    } else if (options.seriesType == "bars") {
        chart = new google.visualization.ComboChart(document.getElementById("chart_div" + iId));
    }
    var self = this;

    this.drawArea = function () {
        if (options.seriesType == "bars") {
            // usually combo charts don't require a granularitySelector.
            $(".granularitySelector").hide();
        } else if (options.seriesType == "area") {
            $(".granularitySelector").show();
        }

        var jsonData = $.ajax({
            data: { iIntervalStart: window.app.dateHelper.transformDate(window.app.startDate), iIntervalEnd: window.app.dateHelper.transformDate(window.app.endDate), iGranularity: granularity, culture: window.app.calendarCulture, scope: window.app.currentWorkingPoint },
            url: app.domainName + resource,
            dataType: "json",
            async: false,
            success: function (data) {
                self.drawChart(data);                
            }
        }).responseText;        
    }

    $(".radioOption" + identifier).change(function (event) {
        var selectorId = $(this).attr("selectorId");
        $(this).parents("#granularitySelector" + selectorId).find(".active").removeClass("active");
        $(this).parents(".radioBtnWrapper").addClass("active");

        window.app.areas[selectorId].setGranularity($(this).val());
        window.app.areas[selectorId].drawArea();
    });

    this.drawChart = function(jsonData) {
        // Create our data table out of JSON data loaded from server.
        data = new google.visualization.DataTable(jsonData);
        chart.draw(data, options);
    }

    this.setGranularity = function (iGranularity) {
        granularity = iGranularity;
    }
   

}