"use strict";

window.app = window.app || {};
window.app.calendarCulture = "en-GB";

function InitializeGUI() {
   var self = this;
   window.addEventListener("resize", resizeTriggered, false);
   resizeTriggered();
}

$(document).ready(function () {
    var culture = $(".currentCulture").val().substring(0, 2).toLowerCase();
    if (culture == "en") {
        window.app.calendarCulture = "en-GB";
    } else {
        window.app.calendarCulture = culture;
    }
      
    var report = new Report();
    report.initialize();
   
});

function resizeTriggered() {
    //pick the highest between window size (- header) and messagesArea 
    // TODO: refactor variables names.
   var padding = 5;
   var filterStripHeigh = 45;
   var window_height = window.innerHeight;
   var headerHeight = $('header').height();
   var contentWindowHeight = window_height - headerHeight - (2 * padding) - filterStripHeigh;
   
   $('#leftColumn').height(contentWindowHeight);
   $('#rightColumn').height(contentWindowHeight);
}