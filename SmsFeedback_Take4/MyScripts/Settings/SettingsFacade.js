//#region Defines to stop jshint from complaining about "undefined objects"
/*global window */
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
//#endregion
window.app = window.app || {};

function resizeTriggered() {
   "use strict";
   //pick the highest between window size (- header) and messagesArea
   var padding = 5;
   //var msgAreaMarginTop = 10;
   var filterStripHeigh = 45;
   var window_height = window.innerHeight;
   //var messagesAreaHeight = $('#messagesArea').height();
   var headerHeight = $('header').height();
   var contentWindowHeight = window_height - headerHeight - (2 * padding) - filterStripHeigh;   
   $('.container_12').height(contentWindowHeight);
   var marginTop = 7;
   $('#rightColumn').height(contentWindowHeight - marginTop);
}

function initializeSettingsGUI() {
   "use strict";   
   
   //DA IE8 doesn't support addEventListener so we use attachEvent
   //source http://stackoverflow.com/questions/9769868/addeventlistener-not-working-in-ie8
   if (!window.addEventListener) {
      window.attachEvent("resize", resizeTriggered);
   }
   else {
      window.addEventListener("resize", resizeTriggered, false);
   }
   resizeTriggered();
   window.app.settingsPage = new window.app.SettingsArea();
}

$(function () {
   initializeSettingsGUI();
});