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

function FirstArea(iResource, iGranularity, iOptions, iId, iTooltip, iTitle) {
   var self = this;
   var resource = iResource;
   var granularity = iGranularity;
   var chart = null;
   var data = null;
   var jsonDataReceived = null;
   var tooltip = iTooltip;
   var title = iTitle;

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
   options.seriesType = iOptions.seriesType;
   options.colors = iOptions.colors;

   var identifier = iId;

   // create div in chart_div
   if (options.seriesType === "area") {
      chart = new google.visualization.AreaChart(document.getElementById("chart_div" + iId));
      options.pointSize = 6;
   } else if (options.seriesType === "bars") {
      chart = new google.visualization.ComboChart(document.getElementById("chart_div" + iId));
   }
   this.drawArea = function () {
      if (options.seriesType === "bars") {
         // usually combo charts don't require a granularitySelector.
         $(".granularitySelector").hide();
      } else if (options.seriesType === "area") {
         $(".granularitySelector").show();
      }

      var jsonData = $.ajax({
         data: {
            iIntervalStart: window.app.dateHelper.transformDate(window.app.startDate),
            iIntervalEnd: window.app.dateHelper.transformDate(window.app.endDate),
            iGranularity: granularity,
            culture: window.app.calendarCulture,
            scope: window.app.currentWorkingPoint
         },
         url: window.app.domainName + resource,
         dataType: "json",
         async: false,
         success: function (data) {
            self.drawChart(data);
         }
      }).responseText;
   };

   $(".radioOption" + identifier).change(function (event) {
      var selectorId = $(this).attr("selectorId");
      $(this).parents("#granularitySelector" + selectorId).find(".active").removeClass("active");
      $(this).parents(".radioBtnWrapper").addClass("active");

      window.app.areas[selectorId].setGranularity($(this).val());
      window.app.areas[selectorId].drawArea();
   });

   $(".toCsv" + identifier).click(function () {
      DownloadJSON2CSV(JSON.stringify(jsonDataReceived), title);
   });

   this.drawChart = function (jsonData) {
      // Create our data table out of JSON data loaded from server.
      jsonDataReceived = jsonData;
      data = new google.visualization.DataTable(jsonData);
      chart.draw(data, options);
   };

   this.setGranularity = function (iGranularity) {
      granularity = iGranularity;
   };
}