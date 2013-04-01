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
//#endregion
window.app = window.app || {};
function SecondArea(iModel) {
   "use strict";
   var model = iModel;

   this.load = function (data) {
     var infoBoxString = "<div class='boxArea' title='" + model.tooltip + "'>" +
                              "<div class='infoContent'>" +
                                "<div class='boxValue'>" + data.value + "</div>" +
                                "<div class='boxUnit'> " + data.unit + "</div>" +
                              "</div>" +
                              "<div class='infoTitle'>" + model.title + "</div>" +
                          "</div>";

      $("#secondSection").append(infoBoxString);
   };  

}