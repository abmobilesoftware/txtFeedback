"use strict";
window.app = window.app || {};
window.app.dateFormatForDatePicker = 'dd/mm/yy';

window.app.defaultFilteringOptions = {
   tagsForFiltering: [],
   tagFilteringEnabled: false,
   dateFilteringEnabled: false,
   startDate: "",
   endDate: "",
   starredFilteringEnabled: false,
   unreadFilteringEnabled: false,
   supportFilteringEnabled: false
}

function FilterArea() {
   var self = this;
   $.extend(this, app.defaultFilteringOptions);

   //#region Tags
   var placeholderMessage = $('#filteringAddFilterTagMessage').val();

   $("#filterTag").tagsInput({
      'height': '22px',
      'width': 'auto',
      'autocomplete_url': "Tags/FindMatchingTags",
      'onAddTag': function (tagValue) {
         var delimiter = ',';
         self.tagsForFiltering = $("#filterTag").val().split(delimiter);
         if (self.tagFilteringEnabled) {
            $(document).trigger('refreshConversationList');
         }
      },
      'onRemoveTag': function(tagValue) {
         var delimiter = ',';
         self.tagsForFiltering = $("#filterTag").val().split(delimiter);
         //if there are no tags the split will return [""] and this will be sent to the server
         //we guard agains this
         if ("" === self.tagsForFiltering[0]) {
            self.tagsForFiltering = [];
         }
         if (self.tagFilteringEnabled) {
            $(document).trigger('refreshConversationList');
         }
      },      
      'defaultText': placeholderMessage,
      'placeholder': placeholderMessage,
      'interactve': true,
      'placeholderColor' : '#666666'   
   });  

   $("#includeTagsInFilter").bind('click', function () {
      //set internal state
      if (self.tagFilteringEnabled) {
         self.tagFilteringEnabled = false;       
      }
      else {
         self.tagFilteringEnabled = true;        
      }
      //change checkbox state
      setCheckboxState($(this), self.tagFilteringEnabled);
      //trigger filtering if required
      if (self.tagsForFiltering.length != 0) {
         $(document).trigger('refreshConversationList');
      }
   });
    //#endregion

    //#region Date

    /*
   this.previousStartDate = "";
   this.defaultStartDate = this.startDate;
   var startDatePicker = $("#startDateTimePicker");
   if (startDatePicker.length !== 0)
   {     
      //TODO add option to specify which language to use (according to selected language)
      startDatePicker.datepicker({
          changeMonth: true,
          numberOfMonths: 3,
          dateFormat: app.dateFormatForDatePicker,
         onClose: function (dateText, inst) {
            //compare the new value with the previous value
            //if changed and date filtering is checked, trigger refreshConversationList
            if (self.previousStartDate !== dateText) {
               //the date has been modified
               self.previousStartDate = dateText;
               self.startDate = dateText;
               if (self.dateFilteringEnabled) {
                  $(document).trigger('refreshConversationList');
               }
            }

         }         
      });
      
      this.previousStartDate = $.datepicker.formatDate(app.dateFormatForDatePicker, startDatePicker.datepicker("getDate"));
      this.defaultStartDate = $.datepicker.formatDate(app.dateFormatForDatePicker, startDatePicker.datepicker("getDate"));
   }    
   var endDatePicker = $("#endDateTimePicker");
   this.previousEndDate = "";
   this.defaultEndDate = this.endDate;
   if (endDatePicker.length !== 0)   {
       endDatePicker.datepicker({
         changeMonth: true,
         numberOfMonths: 3,
         dateFormat: app.dateFormatForDatePicker,
         onClose: function (dateText, inst) {
            //compare the new value with the previous value
            //if changed and date filtering is checked, trigger refreshConversationList
            if (self.previousEndDate !== dateText) {
               //the date has been modified
               self.previousEndDate = dateText;
               self.endDate = dateText;
               if (self.dateFilteringEnabled) {
                  $(document).trigger('refreshConversationList');
               }
            }

         }         
      });
      
      var fromTranslation = $("#startDateTimePicker").val();
      var toTranslation = $("#endDateTimePicker").val();
      
      // Setup the calendar culture
      $.datepicker.regional[window.app.calendarCulture].dateFormat = window.app.dateFormatForDatePicker;
      var culture = $.datepicker.regional[window.app.calendarCulture];
      $("#startDateTimePicker").datepicker("option", culture);
      $("#endDateTimePicker").datepicker("option", culture);

      $("#startDateTimePicker").val(fromTranslation);
      $("#endDateTimePicker").val(toTranslation);

      
      this.previousEndDate = $.datepicker.formatDate(app.dateFormatForDatePicker, endDatePicker.datepicker("getDate"))
      this.defaultEndDate = $.datepicker.formatDate(app.dateFormatForDatePicker, endDatePicker.datepicker("getDate"));
   }

   
   $("#includeDateInFilter").bind('click', function () {
      //set internal state
      self.dateFilteringEnabled = !self.dateFilteringEnabled;     
      //change checkbox state
      setCheckboxState($(this), self.dateFilteringEnabled);

      //get the values for start/end date
      var startDatePicker = $("#startDateTimePicker");
      var newStartDate = $.datepicker.formatDate(app.dateFormatForDatePicker, startDatePicker.datepicker("getDate"));
      var endDatePicker = $("#endDateTimePicker");
      var newEndDate = $.datepicker.formatDate(app.dateFormatForDatePicker, endDatePicker.datepicker("getDate"));
      //trigger filtering if required
      if (self.defaultStartDate !== newStartDate || self.defaultEndDate !== newEndDate) {
         self.startDate = newStartDate;
         self.endDate = newEndDate;
         $(document).trigger('refreshConversationList');
      }
   });
   */

   //#endregion

   //#region Starred
   $("#includeStarredInFilter").bind('click', function () {
      //set internal state
      self.starredFilteringEnabled = !self.starredFilteringEnabled;      
      //change checkbox state
      setCheckboxState($(this), self.starredFilteringEnabled);      
      //trigger filtering if required
      $(document).trigger('refreshConversationList');     
   });
   //#endregion

   //#region Unread
   $("#includeUnreadInFilter").bind('click', function () {
      //set internal state
      self.unreadFilteringEnabled = !self.unreadFilteringEnabled;      
      //change checkbox state
      setCheckboxState($(this), self.unreadFilteringEnabled);
      //trigger filtering if required
      $(document).trigger('refreshConversationList');
   });
   //#endregion

    //#region Support
   $("#includeSupportInFilter").bind('click', function () {
       //set internal state
       self.supportFilteringEnabled = !self.supportFilteringEnabled;
       //change checkbox state
       setCheckboxState($(this), self.supportFilteringEnabled);
       //trigger filtering if required
       $(document).trigger('refreshConversationList');
   });
    //#endregion

   //#region IsFilteringEnabled
   this.IsFilteringEnabled = function () {      
      return self.tagFilteringEnabled || self.dateFilteringEnabled || self.starredFilteringEnabled || self.unreadFilteringEnabled;
   }
   //#endregion

   $("#includeDateInFilter, #includeStarredInFilter, #includeUnreadInFilter, #includeTagsInFilter, #includeSupportInFilter").each(function () {
      var elementToShowTooltipOn = $(this);
      setTooltipOnElement(elementToShowTooltipOn, elementToShowTooltipOn.attr('tooltiptitle'), 'light');
   });

}