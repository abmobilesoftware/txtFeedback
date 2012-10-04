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
    
    this.drawArea = function () {
       $("#infoBoxArea").empty();
       for (var i = 0; i < data.length; ++i) {
          this.buildInfoBox(data[i].name, data[i].source, data[i].tooltip);
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

    this.buildInfoBox = function (name, resource, tooltip) {
       var jsonData = $.ajax({
          data: { iIntervalStart: window.app.dateHelper.transformDate(window.app.startDate), iIntervalEnd: window.app.dateHelper.transformDate(window.app.endDate), culture: window.app.calendarCulture, scope: window.app.currentWorkingPoint },
          url: window.app.domainName + resource,
          dataType: "json",
          async: false,
          success: function (data) {
             self.fillInfoBox(name, tooltip, data);
          }
       }).responseText;
    };

    this.fillInfoBox = function (name, tooltip, data) {
       var infoBoxString = "<div class='boxArea' tooltiptitle='" + tooltip + "'>" +
                               "<div class='infoContent'><div class='infoContentMiddle'><div class='infoContentInner'><span class='boxContent'><span class='boxValue'>" + data.value + "</span><span class='boxUnit'> " + data.unit + "</span></span></div></div></div>" +
                               "<div class='infoTitle'><div class='infoTitleMiddle'><div class='infoTitleInner'><span class='boxTitle'>" + name + "</span></div></div></div>" +
                           "</div>";

       $("#infoBoxArea").append(infoBoxString);
    };

}