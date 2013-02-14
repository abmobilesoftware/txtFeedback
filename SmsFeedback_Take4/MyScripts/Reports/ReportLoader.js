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

function ReportLoader(iResource, iGranularity, iModel) {
    var self = this;
    var resource = iResource;
    var granularity = iGranularity;
    var chart = null;
    var data = null;
    var reportArea = $("#reportContent");
    var firstAreaId = "#PrimaryChartArea";
    var secondAreaId = "#InfoBox";
    var thirdAreaId = "#SecondaryChartArea";
    var model = iModel;
   
    this.drawArea = function () {
       var jsonData = $.ajax({
            data: {
                iIntervalStart: window.app.dateHelper.transformDate(window.app.startDate),
                iIntervalEnd: window.app.dateHelper.transformDate(window.app.endDate),
                iGranularity: granularity,
                scope: window.app.currentWorkingPoint
            },
            url: window.app.domainName + resource,
            dataType: "json",
            async: false,
            success: function (data) {                
                loadReport(data);
            }
        }).responseText;
    };

    function loadReport(data) {
        // TO DO iterate through model and call the renderSection func when is needed
        for (var k = 0; k < model.get("sections").length; ++k) {
            if (model.get("sections")[k].visibility) {
                // TODO: Naming refactoring
                renderSection("#" + model.get("sections")[k].identifier,
                   model.get("sections")[k].uniqueId,
                   model.get("sections")[k].sectionId,
                   model.get("sections")[k].resources,
                   data);                
            }
        }
    }

    function renderSection(section, uniqueId, sectionId, resources, data) {
        var parameters = resources[0];
        parameters.uniqueId = uniqueId;
        parameters.sectionId = sectionId;
        var template = _.template($(section).html(), parameters);
        $("#reportContent").append(template);
        if (section === "#PrimaryChartArea") {
            var area = new FirstArea("day", resources[0].options, uniqueId, resources[0].tooltip, resources[0].name);
            var chartData = data.ChartDataArray.ChartDataArray[0];
            area.drawChart(chartData);
            window.app.areas[uniqueId] = area;
            //window.app.areas.push(area);
        } else if (section === "#SecondaryChartArea") {
            /*window.app.thirdArea = new ThirdArea(resources[0].source);
            window.app.thirdArea.drawArea();
            //window.app.areas.push(window.app.thirdArea);
            window.app.areas[uniqueId] = window.app.thirdArea; */
        } else if (section === "#InfoBox") {
            window.app.secondArea = new SecondArea(resources);
            var infoBoxData = data.InfoBoxArray.InfoBoxArray;
            window.app.secondArea.drawArea(infoBoxData);
            //window.app.areas.push(window.app.secondArea);
            window.app.areas[uniqueId] = window.app.secondArea; 
        }
    }
}