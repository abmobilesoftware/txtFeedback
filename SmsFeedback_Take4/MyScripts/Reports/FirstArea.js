window.app = window.app || {};

function FirstArea(iResource, iGranularity, iOptions) {
    var self = this;
    var resource = iResource;
    var granularity = iGranularity;
    var chart = null;
    var data = null;
    var options = {
        animation: {
            duration: 1000,
            easing: 'out'
        }
    };
    options.seriesType = iOptions.seriesType;
    
    if (options.seriesType == "area") {
        chart = new google.visualization.AreaChart(document.getElementById('chart_div'));
    } else if (options.seriesType == "bars") {
        chart = new google.visualization.ComboChart(document.getElementById('chart_div'));        
    }

    this.drawArea = function () {
        if (options.seriesType == "bars") {
            // usually combo charts don't require a granularitySelector.
            $("#granularitySelector").hide();
        } else if (options.seriesType == "area") {
            $("#granularitySelector").show();
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

    this.drawChart = function(jsonData) {
        // Create our data table out of JSON data loaded from server.
        data = new google.visualization.DataTable(jsonData);
        chart.draw(data, options);
    }

    this.setGranularity = function (iGranularity) {
        granularity = iGranularity;
    }

}