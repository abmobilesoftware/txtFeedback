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
/*global ReportsArea */
//#endregion
window.app = window.app || {};
window.app.pageTitle = document.title;

function updateChartsDimensions() {
   // the charts are redrawn at a more appropriate scale.
   window.app.reportsPage.redrawContent();
}

function InitializeGUI() {
   "use strict";      

   $(window).smartresize(function () {
      updateChartsDimensions();
   });
   resizeTriggered();
}

$(document).ready(function () {
   var culture = $(".currentCulture").val().substring(0, 2).toLowerCase();
   if (culture === "en") {
      window.app.calendarCulture = "en-GB";
   } else {
      window.app.calendarCulture = culture;
   }

   window.app.reportsPage = new ReportsArea();
   $(document).bind("resize", resizeTriggered);
});



