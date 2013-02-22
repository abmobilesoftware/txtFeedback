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
/*global google */
/*global DownloadJSON2CSV */
//#endregion
window.app = window.app || {};

function FirstArea(sectionModel) {
    var self = this;
    var jsonData = null;
    var title = sectionModel.title;
    var chartSource = sectionModel.chartSource;
    var options = {
        animation: {
            duration: 1000,
            easing: 'out'
        },
        vAxis: {
            gridlines: { count: 4 }
        },
        backgroundColor: '#F5F8FA'
    };
    if (sectionModel.options != null) {
        options.seriesType = sectionModel.options.seriesType;
        options.colors = sectionModel.options.colors;
    }
    var identifier = sectionModel.id;

    /*
        "identifier" Used to access a specific granularity selector. 
        Used on positive & negative feedback report.
    */
    $(".radioOption" + identifier).change(function (event) {
        $(this).parents("#granularitySelector" + identifier).find(".active").removeClass("active");
        $(this).parents(".radioBtnWrapper").addClass("active");

        window.app.areas[identifier].loadWithGranularity($(this).val());        
    });

    $(".toCsv" + identifier).click(function () {
        DownloadJSON2CSV(JSON.stringify(jsonData), title);
    });

    this.loadWithGranularity = function(granularity) {
        self = this;
        var jsonData = $.ajax({
            data: {
                iIntervalStart: window.app.dateHelper.transformDate(window.app.startDate),
                iIntervalEnd: window.app.dateHelper.transformDate(window.app.endDate),
                iScope: window.app.currentWorkingPoint,
                iGranularity: granularity
            },
            url: window.app.domainName + chartSource,
            dataType: "json",
            async: false,
            success: function (data) {
                self.load(data);
            }
        }).responseText;
    }

    this.load = function (iData) {
        $(".granularitySelector").show();
        if (options.seriesType === "bars") {
            // usually combo charts don't require a granularitySelector.
            $(".granularitySelector").hide();
        } 

        var chart;
        if (options.seriesType == undefined) {
            chart = new google.visualization.AreaChart(document.getElementById("chart_div" + identifier));
            options.pointSize = 6;
        } else {
            if (options.seriesType === "area") {
                chart = new google.visualization.AreaChart(document.getElementById("chart_div" + identifier));
                options.pointSize = 6;
            } else if (options.seriesType === "bars") {
                chart = new google.visualization.ComboChart(document.getElementById("chart_div" + identifier));
            }
        }

        // Create our data table out of JSON data loaded from server.
        jsonData = iData;
        var data = new google.visualization.DataTable(jsonData);
        chart.draw(data, options);
    };

    this.setGranularity = function (iGranularity) {
        granularity = iGranularity;
    };
}