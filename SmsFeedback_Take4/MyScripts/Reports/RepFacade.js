"use strict";

function InitializeGUI() {
   var self = this;
   window.addEventListener("resize", resizeTriggered, false);
   
   $(window).smartresize(function () {
       updateChartsDimensions();
   });
   resizeTriggered();
}

$(document).ready(function () {
    var culture = $(".currentCulture").val().substring(0, 2).toLowerCase();
    if (culture == "en") {
        window.app.calendarCulture = "en-GB";
    } else {
        window.app.calendarCulture = culture;
    }
         
    window.app.reportsPage = new ReportsArea();
    $(document).bind("resize", resizeTriggered);
});

function resizeTriggered() {
    //pick the highest between window size (- header) and messagesArea 
    // TODO: refactor variables names.
   var padding = 5;
   var filterStripHeigh = 45;
   var window_height = window.innerHeight;
   var headerHeight = $('header').height();

   var marginTop = 7;
   var contentWindowHeight = window_height - headerHeight - padding - filterStripHeigh - marginTop;
   
   var titleAreaMarginTopBottom = 8;
   var titleAreaPaddingTop = 10;
   var titleAreaPaddingBottom = 6;

   var chartAreaMarginTopBottom = 10;
   var chartAreaPaddingTopBottom = 10;

   var infoBoxAreaMarginTop = 5;
   var infoBoxAreaMarginBottom = 25;

   var tableAreaMarginTopBottom = 10;
   var tableAreaPaddingTopBottom = 10;
  
   /*var reportsAreaHeight = 2 * titleAreaMarginTopBottom + titleAreaPaddingTop + titleAreaPaddingBottom + $("#titleArea").height() +
                            2 * chartAreaMarginTopBottom + 2 * chartAreaPaddingTopBottom + $("#chartArea").height() + infoBoxAreaMarginTop + infoBoxAreaMarginBottom +
                            $("#infoBoxArea").height() + 2 * tableAreaMarginTopBottom + 2 * tableAreaPaddingTopBottom + $("#tableArea").height() - marginTop;
   */
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