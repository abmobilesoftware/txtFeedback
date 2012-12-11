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
function resizeTriggered() {
   //pick the highest between window size (- header) and messagesArea 
   // TODO: refactor variables names.
   var padding = 5;
   var filterStripHeigh = 45;
   var window_height = window.innerHeight;
   var headerHeight = $('header').height();

   var marginTop = 7;
   var contentWindowHeight = window_height - headerHeight - padding - filterStripHeigh - marginTop;

   var reportsAreaHeight = $("#reportContent").height() + $("#titleArea").height() + filterStripHeigh;
   if (contentWindowHeight <= reportsAreaHeight) {
      $('#leftColumn').height(reportsAreaHeight);
      $('#rightColumn').height(reportsAreaHeight);
   } else {
      $('#leftColumn').height(contentWindowHeight);
      $('#rightColumn').height(contentWindowHeight);
   }
}
function updateChartsDimensions() {
   // the charts are redrawn at a more appropriate scale.
   window.app.reportsPage.redrawContent();
}

function InitializeGUI() {
   "use strict";
   window.addEventListener("resize", resizeTriggered, false);

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



