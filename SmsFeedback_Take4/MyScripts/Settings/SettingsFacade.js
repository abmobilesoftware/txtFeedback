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


function initializeSettingsGUI() {
   "use strict";   
   
   resizeTriggered();
   window.app.settingsPage = new window.app.SettingsArea();
}

$(function () {
   initializeSettingsGUI();
});