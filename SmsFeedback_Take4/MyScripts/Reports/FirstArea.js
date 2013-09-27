﻿//#region Defines to stop jshint from complaining about "undefined objects"
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
/*global google */
/*global DownloadJSON2CSV */
//#endregion
window.app = window.app || {};
window.app.reportDataToBeSaved = {};
window.app.tagFilteringID = {};

function FirstArea(sectionModel) {
    var self = this;
    var jsonData = null;
    var title = sectionModel.title;
    var chartSource = sectionModel.chartSource;
    var options = {
        animation: {
            duration: 1000,
            easing: 'out'
        },
        vAxis: {
            gridlines: { count: 4 }
        },
        backgroundColor: '#F5F8FA'
    };
    if (sectionModel.options != null) {
        options.seriesType = sectionModel.options.seriesType;
        options.colors = sectionModel.options.colors;
    }
    var identifier = sectionModel.id;

    /*
        "identifier" Used to access a specific granularity selector. 
        Used on positive & negative feedback report.
    */
    $(".radioOption" + identifier).change(function (event) {
        $(this).parents("#granularitySelector" + identifier).find(".active").removeClass("active");
        $(this).parents(".radioBtnWrapper").addClass("active");

        window.app.areas[identifier].loadWithGranularity($(this).val());        
    });

    $(".toCsv" + identifier).click(function (event) {
        event.preventDefault();
        DownloadJSON2CSV(JSON.stringify(jsonData), title);
    });

    $(".exportRawBtn").click(function (event) {
       event.preventDefault();
       var jsonData = $.ajax({
          url: "/Reports/GetActivityReport",
          data: {
             iIntervalStart: window.app.dateHelper.transformStartDate(window.app.startDate),
             iIntervalEnd: window.app.dateHelper.transformEndDate(window.app.endDate),
             iScope: window.app.currentWorkingPoint
          },
          dataType: "json",
          traditional: true,
          async: false,
          success: function (data) {
             /* data contains 3 fields
             * header - the header definition
             * rows - the actual data
             * reportname - the name of the files to create
             */
             var str = data.header.join(",") + "\r\n";
             var line = '';
             var array = typeof data.rows != 'object' ? JSON.parse(data.rows) : data.rows;
             for (var i = 0; i < array.length; i++) {
                line = '';
                line += '"' + array[i].Store + '",';
                line += array[i].ConvID + ',';
                //remove txtfeedback.net suffixes
                line += array[i].From.replace("@moderator.txtfeedback.net", "").replace("@txtfeedback.net", "") + ',';
                line += array[i].To.replace("@moderator.txtfeedback.net", "").replace("@txtfeedback.net", "") + ',';
                //remove unnecessary break lines
                line += '"' + array[i].Text.replace(/(\r\n|\n|\r)/gm, "") + '",';
                line += array[i].ReceivedTime;
                str += line + '\r\n';
             }
             //do the actual submit to create csv file
             $("#exportrawreport").append('<form id="exportform" action="http://137.117.194.206/export2csv.php" method="post" target="_blank">' +
                '<input type="hidden" id="exportdata" name="exportdata" />' +
                '<input type="hidden" id="docTitle" name="docTitle" />' +
             '</form>');
             $("#exportdata").val(str);
             $("#docTitle").val(data.reportname);
             $("#exportform").submit().remove();            
          }
       });
    });

    this.loadWithGranularity = function (granularity) {
       self = this;

       var jsonData = $.ajax({
          data: {
             iIntervalStart: window.app.dateHelper.transformStartDate(window.app.startDate),
             iIntervalEnd: window.app.dateHelper.transformEndDate(window.app.endDate),
             iScope: window.app.currentWorkingPoint,
             iGranularity: granularity       
          },
          url: window.app.domainName + chartSource,
          traditional: true,
          dataType: "json",
          async: false,
          success: function (data) {
             self.load(data);
          }
       }).responseText;
    };

    this.load = function (iData) {
       //DA build the granularitySelector id
       var selector = "#granularitySelector" + identifier;
       $(selector).show();
        if (options.seriesType === "bars") {
            // usually combo charts don't require a granularitySelector.
           $(selector).hide();
        } 

        var chart;
        if (options.seriesType === undefined) {
            chart = new google.visualization.AreaChart(document.getElementById("chart_div" + identifier));
            options.pointSize = 6;
        } else {
            if (options.seriesType === "area") {
                chart = new google.visualization.AreaChart(document.getElementById("chart_div" + identifier));
                options.pointSize = 6;
            } else if (options.seriesType === "bars") {
                chart = new google.visualization.ComboChart(document.getElementById("chart_div" + identifier));
            }
        }

        // Create our data table out of JSON data loaded from server.
        jsonData = iData;
        var data = new google.visualization.DataTable(jsonData);
        chart.draw(data, options);
    };

    this.setGranularity = function (iGranularity) {
        granularity = iGranularity;
    };
}


function TagsReportArea(sectionModel) {
   var self = this;
   self.tagsForFiltering = [];
   var jsonData = null;
   var title = sectionModel.title;
   var chartSource = sectionModel.chartSource;
   var options = {
      animation: {
         duration: 1000,
         easing: 'out'
      },
      vAxis: {
         gridlines: { count: 4 }
      },
      backgroundColor: '#F5F8FA'
   };
   if (sectionModel.options != null) {
      options.seriesType = sectionModel.options.seriesType;
      options.colors = sectionModel.options.colors;
   }
   var identifier = sectionModel.id;

   /*
       "identifier" Used to access a specific granularity selector. 
       Used on positive & negative feedback report.
   */
   $(".radioOption" + identifier).change(function (event) {
      $(this).parents("#granularitySelector" + identifier).find(".active").removeClass("active");
      $(this).parents(".radioBtnWrapper").addClass("active");
      self.setGranularity($(this).val());
      self.loadWithGranularity($(this).val());
   });

   $(".toCsv" + identifier).click(function (event) {
      event.preventDefault();
      DownloadJSON2CSV(JSON.stringify(jsonData), title);
   });

   $(".exportRawBtn").click(function (event) {
      event.preventDefault();
      var jsonData = $.ajax({
         url: "/Reports/GetActivityReport",
         data: {
            iIntervalStart: window.app.dateHelper.transformStartDate(window.app.startDate),
            iIntervalEnd: window.app.dateHelper.transformEndDate(window.app.endDate),
            iScope: window.app.currentWorkingPoint
         },
         traditional: true,
         dataType: "json",
         async: false,
         success: function (data) {
            /* data contains 3 fields
            * header - the header definition
            * rows - the actual data
            * reportname - the name of the files to create
            */
            var str = data.header.join(",") + "\r\n";
            var line = '';
            var array = typeof data.rows != 'object' ? JSON.parse(data.rows) : data.rows;
            for (var i = 0; i < array.length; i++) {
               line = '';
               line += '"' + array[i].Store + '",';
               line += array[i].ConvID + ',';
               //remove txtfeedback.net suffixes
               line += array[i].From.replace("@moderator.txtfeedback.net", "").replace("@txtfeedback.net", "") + ',';
               line += array[i].To.replace("@moderator.txtfeedback.net", "").replace("@txtfeedback.net", "") + ',';
               //remove unnecessary break lines
               line += '"' + array[i].Text.replace(/(\r\n|\n|\r)/gm, "") + '",';
               line += array[i].ReceivedTime;
               str += line + '\r\n';
            }
            //do the actual submit to create csv file
            $("#exportrawreport").append('<form id="exportform" action="http://137.117.194.206/export2csv.php" method="post" target="_blank">' +
               '<input type="hidden" id="exportdata" name="exportdata" />' +
               '<input type="hidden" id="docTitle" name="docTitle" />' +
            '</form>');
            $("#exportdata").val(str);
            $("#docTitle").val(data.reportname);
            $("#exportform").submit().remove();
         }
      });
   });

   this.loadWithGranularity = function (granularity) {      
      //get the already defined tags
      var delimiter = ',';
      self.tagsForFiltering = $("input[name=filterTagReports]").val().split(delimiter);
      if ("" === self.tagsForFiltering[0]) {
         self.tagsForFiltering = [];
      }
      var jsonData = $.ajax({
         data: {
            iIntervalStart: window.app.dateHelper.transformStartDate(window.app.startDate),
            iIntervalEnd: window.app.dateHelper.transformEndDate(window.app.endDate),
            iScope: window.app.currentWorkingPoint,
            iGranularity: granularity,
            tags: self.tagsForFiltering
         },
         url: window.app.domainName + chartSource,
         dataType: "json",
         async: false,
         traditional: true,
         success: function (data) {
            //DA don't reinitialize the filter functionality as only the grid data has changed
            self.load(data, true);
         }
      }).responseText;
   };

   this.load = function (iData, dontInitializeFilter) {
      //DA build the granularitySelector id
      var selector = "#granularitySelector" + identifier;
      $(selector).show();
      if (options.seriesType === "bars") {
         // usually combo charts don't require a granularitySelector.
         $(selector).hide();
      }

      var chart;
      if (options.seriesType === undefined) {
         chart = new google.visualization.AreaChart(document.getElementById("chart_div" + identifier));
         options.pointSize = 6;
      } else {
         if (options.seriesType === "area") {
            chart = new google.visualization.AreaChart(document.getElementById("chart_div" + identifier));
            options.pointSize = 6;
         } else if (options.seriesType === "bars") {
            chart = new google.visualization.ComboChart(document.getElementById("chart_div" + identifier));
         }
      }

      // Create our data table out of JSON data loaded from server.
      jsonData = iData;
      var data = new google.visualization.DataTable(jsonData);
      chart.draw(data, options);

      if (dontInitializeFilter != undefined && dontInitializeFilter) {

      } else
      {
         var placeholderMessage = $('#filteringAddFilterTagMessage').val();
         var removeTagValue = $('#messagesRemoveTagPlaceHolderMessage').val();
         var tagFilter = $("#filterTagReports").tagsInput({
            'height': '22px',
            'width': 'auto',
            'autocomplete_url': "Tags/FindMatchingTags",
            'onAddTag': function (tagValue) {
               window.app.reportDataToBeSaved[tagValue] = tagValue;
            },
            'onRemoveTag': function (tagValue) {
               delete window.app.reportDataToBeSaved[tagValue];
            },
            'placeholder': placeholderMessage,
            'interactive': true
         });
         //DA decide if we already had tags or to use the default one      
         if ($.isEmptyObject(window.app.reportDataToBeSaved)) {
            tagFilter.addTag($('#defaultTagForTagReports').val());
         }
         else {
            for (prop in window.app.reportDataToBeSaved) {
               tagFilter.addTag(window.app.reportDataToBeSaved[prop]);
            }
         }
      }
      
   };
   this.setGranularity = function (iGranularity) {
      self.granularity = iGranularity;
   };

   $(".refreshTagReport").click(function (e) {
      e.preventDefault();
     
      self.loadWithGranularity(self.granularity);
   });
}