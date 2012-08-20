"use strict";
window.app = window.app || {};

function resizeTriggered() {
   //pick the highest between window size (- header) and messagesArea
   var padding = 5;
   var msgAreaMarginTop = 10;
   var filterStripHeigh = 45;
   var window_height = window.innerHeight;
   var messagesAreaHeight = $('#messagesArea').height();
   var headerHeight = $('header').height();
   var contentWindowHeight = window_height - headerHeight - (2 * padding) - filterStripHeigh;   
   $('.container_12').height(contentWindowHeight);
   var marginTop = 7;
   $('#rightColumn').height(contentWindowHeight - marginTop);
}

function InitializeGUI() {
   var self = this;
   window.addEventListener("resize", resizeTriggered, false);
   resizeTriggered();
  

   window.app.settingsPage = new window.app.SettingsArea();
}

$(function () {
   InitializeGUI();
})