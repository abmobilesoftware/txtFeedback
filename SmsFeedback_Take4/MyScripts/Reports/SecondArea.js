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
function SecondArea(iData) {
   "use strict";
   var data = iData;
   var self = this;
   // enable tooltips

   this.drawArea = function (areaSource) {
      $("#infoBoxArea").empty();
      for (var i = 0; i < data.length; ++i) {
          this.fillInfoBox(data[i].name, data[i].tooltip, areaSource[i]);
      }
      $("#infoBoxArea").append("<div class='clear'></div>");

      /* For now, tooltips for infoboxes are disabled.
      var infoBoxElement = $(".boxArea");
      infoBoxElement.qtip({
          content: infoBoxElement.attr('tooltiptitle'),
          position: {
              corner: {
                  target: 'topRight',
                  tooltip: 'bottomLeft'
              }
          },
          style: {
              width: 200,
              padding: 5,
              background: '#345062',
              color: '#ffffff',
              textAlign: 'center',
              border: {
                  width: 5,
                  radius: 5,
                  color: '#345062'
              },
              tip: 'bottomLeft',
              name: 'light'
          }
      });*/
   };

   this.fillInfoBox = function (name, tooltip, data) {
      var infoBoxString = "<div class='boxArea' tooltiptitle='" + tooltip + "'>" +
                              "<div class='infoContent'>" +
                                "<div class='infoContentMiddle'><div class='infoContentInner'>" +
                                   "<span class='boxContent'>" +
                                      "<span class='boxValue'>" + data.value + "</span>" +
                                      "<span class='boxUnit'> " + data.unit + "</span>" +
                                   "</span>" +
                                 "</div></div></div>" +
                              "<div class='infoTitle'><div class='infoTitleMiddle'><div class='infoTitleInner'><span class='boxTitle'>" + name + "</span></div></div></div>" +
                          "</div>";

      $("#infoBoxArea").append(infoBoxString);
   };

}