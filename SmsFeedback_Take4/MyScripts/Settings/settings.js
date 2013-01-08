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
/*global trim */
//#endregion
window.app = window.app || {};

window.app.showChangePassword = function () {
   "use strict";
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
                  $('#rightColumn').html(data);
                  //$('th').each(function () { setTooltipOnElement(this, this.attr('tooltiptitle'), 'light'); });
               },
               error: function (jqXHR, textStatus, errorThrown) {
                  $('#rightColumn').html(jqXHR);
               }
            });
         });
      }
   });
};

window.app.getDataForWorkingPoints = function () {
   "use strict";
   var data = [];
   $('#rightColumn table').find('tr[name="dataRow"]').each(function () {
      var row = {
         TelNumber: $('span[name="TelNumber"]', this).text(),
         ShortID: $('span[name="ShortID"]', this).text(),
         MaxNrOfSmsToSendPerMonth: $('span[name="MaxNrOfSmsToSendPerMonth"]', this).text(),
         Name: trim($('input[name="Name"]', this).val()),
         Description: trim($('input[name="Description"]', this).val()),
         NrOfSentSmsThisMonth: 15,
         WelcomeMessage: trim($('input[name="WelcomeMessage"]', this).val())
      };
      data.push(row);
   });
   return data;
};
window.app.localForSetting = {};
window.app.localForSetting.setTooltipsOnHeaders = function () {
   "use strict";
   $('th').each(function () {
      $(this).qtip({
         content: $(this).attr('tooltiptitle'),
         position: {
            corner: {
               target: 'topLeft',
               tooltip: 'bottomLeft'
            }
         },
         style: 'dark'
      });
   });
};
window.app.saveWorkingPoints = function (e) {
   "use strict";
   e.preventDefault();
   var wps = window.app.getDataForWorkingPoints();
   $.ajax({
      url: 'Settings/GetDefineWorkingPointsForm',
      data: $.toJSON(wps),
      type: 'post',
      cache: false,
      dataType: 'html',
      contentType: 'application/json; charset=utf-8',
      success: function (data) {
         $('#rightColumn').html(data);
         window.app.localForSetting.setTooltipsOnHeaders();
      },
      error: function (jqXHR, textStatus, errorThrown) {
         $('#rightColumn').html(jqXHR);
      }
   });
};
window.app.configureWorkingPoints = function () {
   "use strict";
   $.ajax({
      url: "Settings/GetDefineWorkingPointsForm",
      cache: false,
      success: function (data) {
         $('#rightColumn').html(data);
         $('#btnSaveWorkingPoints').live('click', window.app.saveWorkingPoints);
         window.app.localForSetting.setTooltipsOnHeaders();
      },
      error: function (jqXHR, textStatus, errorThrown) {
         $('#rightColumn').html(jqXHR);
      }
   });
};

window.app.configureBillingInfo = function () {
   "use strict";
   $.ajax({
      url: "Settings/CompanyBillingInfo",
      cache: false,
      success: function (data) {
         $('#rightColumn').html(data);
         //$('#btnSaveWorkingPoints').live('click', window.app.saveWorkingPoints);
         window.app.localForSetting.setTooltipsOnHeaders();
      },
      error: function (jqXHR, textStatus, errorThrown) {
         $('#rightColumn').html(jqXHR);
      }
   });
}
window.app.SettingsArea = function () {
   "use strict";
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
         case '41':
            window.app.configureBillingInfo();
            break;
         default:
      }
   });
};

