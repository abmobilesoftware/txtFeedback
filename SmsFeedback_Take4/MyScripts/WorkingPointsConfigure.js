window.app = window.app || {};
window.settings = window.settings || {};

window.settings.initializeWpInfoArea = function () {      
   $('#saveWpInfo').unbind('click');
   $('#saveWpInfo').bind('click', function (event) {
      event.preventDefault();
      $.ajax({
               url: 'WorkingPoints/EditWorkingPointInfo',
               data: $('#wpDetailsArea form').serialize(),
               type: 'post',
               cache: false,
               dataType: 'html',
               success: function (data) {                 
                  $('#wpDetailsArea').html(data);
                  //if the Name has changed, reflect this in the leftside selection column
                  if (!_.isEmpty(window.settings.selectedWP)) {
                     //DA it would be nice if we would be working directly on the data and not on the dom
                     $(window.settings.selectedWP).text($('#wpInfoDetailsLegend').text());
                  }
                  window.settings.initializeWpInfoArea();
               },
               error: function (jqXHR, textStatus, errorThrown) {
                  $('#wpDetailsArea').html(jqXHR);
               }
            });
   });   
   _.each($(".triggerSave"), function (elem) {      
      //DA according to http://stackoverflow.com/questions/6771140/jquery-text-change-event there are even better ways
      //of detecting when the value has changed - like using setTimeout 
      if (isEventSupported('input') && $(elem).attr('type') != 'checkbox') {
         $(elem).unbind('input');
         $(elem).bind('input', function () {
         $('#saveWpInfo').removeAttr('disabled');
      });
      } else {
         $(elem).unbind('change');
         $(elem).on('change', function (e) {            
            $('#saveWpInfo').removeAttr('disabled');
         });
      }
   });
};

window.settings.selectedWP = {};
window.settings.initializeGUI = function () {
   //only do the following if we are dealing with multiple working points
   if ($('#wpSelectionList') !== undefined) {
      $("#wpSelectionList").selectable({
         selected: function (event, ui)
         {
            window.settings.selectedWP = ui.selected;
            //when selected change the WP under watch
            $.ajax({
               url: 'WorkingPoints/EditWorkingPointInfo',
               data: { 'wpId': ui.selected.getAttribute("wpId") },
               success: function (data) {
                  $('#wpDetailsArea').html(data);
                  window.settings.initializeWpInfoArea();
                  //setupForm(data, '#btnChangePassword', window.app.changePassword);
                  //resizeTriggered();
               },
               error: function (jqXHR, textStatus, errorThrown) {
                  $('#rightColumn').html(jqXHR);
               }
            });
         }
      });
      $($("#wpSelectionList .wpInfo")[0]).addClass("ui-selected");
      window.settings.initializeWpInfoArea();
   }
};

$(function () {
   window.settings.initializeGUI();
});