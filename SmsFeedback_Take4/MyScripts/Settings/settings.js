"use strict";
window.app = window.app || {};

window.app.showChangePassword = function () {
   $.ajax({
      url: "Settings/GetChangePasswordForm",
      cache: false,
      success: function (data) {
         // create a modal dialog with the data
         $('#rightColumn').html(data);
         $('#btnChangePassword').live('click', function (e) {
            e.preventDefault();
            $.ajax({
               url: 'Settings/GetChangePasswordForm',
               data: $('#rightColumn form').serialize(),
               type: 'post',
               cache: true,
               dataType: 'html',
               success: function (data) {
                  var x = data;
                  $('#rightColumn').html(data);
               },
               error: function (jqXHR, textStatus, errorThrown) {                  
                  $('#rightColumn').html(jqXHR);
               }
            });
         });
      }
   });
}

window.app.getDataForWorkingPoints = function () {
   var data = new Array();
   var i=0
   $('#rightColumn table').find('tr[name="dataRow"]').each(function () {
      var row = { TelNumber: $('span[name="TelNumber"]',this).text(),         
         Name: $('input[name="Name"]', this).val(),
         Description: $('input[name="Description"]', this).val(),
         NrOfSentSmsThisMonth: 15,
         MaxNrOfSmsToSendPerMonth: $('span[name="MaxNrOfSmsToSendPerMonth"]', this).text()
         }
      //$(this).find('input,span').each(function () {
      //   row[$(this).attr('name')] = $(this).val();
      //});
      data.push(row);
   });
   return data;
}
window.app.configureWorkingPoints = function () {
   $.ajax({
      url: "Settings/GetDefineWorkingPointsForm",
      cache: false,
      success: function (data) {
         $('#rightColumn').html(data);
         //var settngs = $.data($('form')[0], 'validator').settings;
         //settngs.onkeyup = true;
         //settngs.onfocusout = true;

         $('#btnSaveWorkingPoints').live('click', function (e) {
            e.preventDefault();
            //$('#workingPointsConfig').validate();
            var wps = app.getDataForWorkingPoints() 
            $.ajax({
               url: 'Settings/GetDefineWorkingPointsForm',
               //data: $('#rightColumn form').serializeArray(),
               data: $.toJSON(wps),
               type: 'post',               
               cache: false,               
               dataType: 'html',
               contentType: 'application/json; charset=utf-8',
               success: function (data) {                  
                  $('#rightColumn').html(data);
               },
               error: function (jqXHR, textStatus, errorThrown) {
                  $('#rightColumn').html(jqXHR);
               }
            });
         });
      },
      error: function (jqXHR, textStatus, errorThrown) {
         $('#rightColumn').html(jqXHR);
      }
   });
}
window.app.SettingsArea = function () {
   var self = this;

   var settingsMenu = new window.app.MenuView({
      el: $("#leftColumn"),
      eventToTriggerOnSelect: 'switchSetting',
      menuCollection: new window.app.MenuCollection({ url: '/Settings/GetMenuItems' }),
      afterInitializeFunction: function () {
          //by default open ChangePassword scren   
          $(".liItem21").addClass("menuItemSelected");
          $("ul.item20").css("display", "block");
          window.app.showChangePassword();
      }
   });
   $(document).bind("switchSetting", function (event, menuId) {
      switch (menuId) {
         case '21':
            window.app.showChangePassword();
            break;
         case '31':
            window.app.configureWorkingPoints();
            break;
         default:
      }
   });
};

