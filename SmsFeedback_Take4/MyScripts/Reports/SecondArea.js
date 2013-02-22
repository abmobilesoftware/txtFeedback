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
     var infoBoxString = "<div class='boxArea' tooltiptitle='" + model.tooltip + "'>" +
                              "<div class='infoContent'>" +
                                "<div class='infoContentMiddle'><div class='infoContentInner'>" +
                                   "<span class='boxContent'>" +
                                      "<span class='boxValue'>" + data.value + "</span>" +
                                      "<span class='boxUnit'> " + data.unit + "</span>" +
                                   "</span>" +
                                 "</div></div></div>" +
                              "<div class='infoTitle'><div class='infoTitleMiddle'><div class='infoTitleInner'><span class='boxTitle'>" + model.title + "</span></div></div></div>" +
                          "</div>";

      $("#secondSection").prepend(infoBoxString);
   };  

}