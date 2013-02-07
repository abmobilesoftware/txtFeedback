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
//#endregion

function ThirdArea(iResource) {
   var self = this;
   var resource = iResource;
   var data = null;
   var options = { backgroundColor: '#F5F8FA' };
   var chart = new google.visualization.PieChart(document.getElementById('comboChart_div'));

   this.drawArea = function () {
      var jsonData = $.ajax({
         data: {
            iIntervalStart: window.app.dateHelper.transformDate(window.app.startDate),
            iIntervalEnd: window.app.dateHelper.transformDate(window.app.endDate),
            culture: window.app.calendarCulture,
            scope: window.app.currentWorkingPoint
         },
         url: window.app.domainName + resource,
         dataType: "json",
         async: false,
         success: function (data) {
            self.buildTable(data);
            self.drawChart(data);
         }
      }).responseText;
   };

   this.drawChart = function (jsonData) {
      // Create our data table out of JSON data loaded from server.
      data = new google.visualization.DataTable(jsonData);
      chart.draw(data, options);
   };

   this.buildTable = function (data) {

      var tableString = "<table class='tbl'>";

      var tableHeader = "<thead class='tblHead'>";
      for (var i = 0; i < data.cols.length; ++i) {
         tableHeader += "<td>" + data.cols[i].label + "</td>";
      }
      tableHeader += "</thead>";

      var tableContent = "";
      for (i = 0; i < data.rows.length; ++i) {
         tableContent += "<tr class='tblRow'>";

         for (var j = 0; j < data.rows[i].c.length; ++j) {
            tableContent += "<td>" + data.rows[i].c[j].v + "</td>";
         }
         tableContent += "</tr>";
      }
      tableString += tableHeader + tableContent + "</table>";

      $("#tableContent").html(tableString);
   };

}