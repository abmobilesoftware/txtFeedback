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
/*global resizeTriggered */
//#endregion
window.app = window.app || {};
window.settings = window.settings || {};
function setupForm(formData, sendButton, sendButtonAction) {
   $('#rightColumn').html(formData);
   $('#rightColumn form').submit(function () {
      return false;
   });
   $(sendButton).unbind('click');
   $(sendButton).bind('click', sendButtonAction);
}

window.app.changePassword = function () {
   $.ajax({
      url: 'Settings/GetChangePasswordForm',
      data: $('#rightColumn form').serialize(),
      type: 'post',
      cache: true,
      dataType: 'html',
      success: function (data) {
         $('#rightColumn').html(data);
         setupForm(data, '#btnChangePassword', window.app.changePassword);
         resizeTriggered();
      },
      error: function (jqXHR, textStatus, errorThrown) {
         $('#rightColumn').html(jqXHR);
      }
   });
};

window.settings.ConfigurePassword = function () {
   "use strict";
   $.ajax({
      url: "Settings/GetChangePasswordForm",
      cache: false,
      success: function (data) {
         setupForm(data, '#btnChangePassword', window.app.changePassword);
      }
   });
};

window.settings.ConfigureNotifications = function () {
   "use strict";
   $.ajax({
      url: "Settings/NotificationsPage",
      cache: false,
      success: function (data) {
         setupForm(data, '#btnChangeNotificationsSettings', window.app.saveNotificationsSettings);
      }
   });
};
// TODO : send select input value in data
window.app.saveNotificationsSettings = function () {
   $.ajax({
      url: 'Settings/SaveNotificationsSettings',
      type: 'post',
      cache: true,
      contentType: "application/json",
      data: "{'typeOfActivityReport':'" + $("#activityReportFrequencySelection").val() + "', " +
      "'soundNotificationsEnabled': '" + $("input:radio[name=sound]:checked").val() + "'}",
      success: function (data) {
         $('#rightColumn').html(data);
         setupForm(data, '#btnChangeNotificationsSettings', window.app.saveNotificationsSettings);
         resizeTriggered();
      },
      error: function (jqXHR, textStatus, errorThrown) {
         $('#rightColumn').html(jqXHR);
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
         WelcomeMessage: trim($('textarea[name="WelcomeMessage"]', this).val())
      };
      data.push(row);
   });
   return data;
};
window.app.localForSetting = {};
window.app.localForSetting.setTooltipsOnHeaders = function () {
   "use strict";
  //TODO DA -set tooltips on header
};
window.app.saveWorkingPoints = function (e) {
   "use strict";
   e.preventDefault();
   var wps = window.app.getDataForWorkingPoints();
   $.ajax({
      url: 'WorkingPoints/WorkingPointsInfo',
      data: $.toJSON(wps),
      type: 'post',
      cache: false,
      dataType: 'html',
      contentType: 'application/json; charset=utf-8',
      success: function (data) {
         setupForm(data, '#btnSaveWorkingPoints', window.app.saveWorkingPoints);
         window.app.localForSetting.setTooltipsOnHeaders();
         resizeTriggered();
      },
      error: function (jqXHR, textStatus, errorThrown) {
         $('#rightColumn').html(jqXHR);
      }
   });
};
window.settings.ConfigureWorkingPoints = function () {
   "use strict";
   $.ajax({
      url: "WorkingPoints/WorkingPointsInfo",
      cache: false,
      success: function (data) {
         setupForm(data, '#btnSaveWorkingPoints', window.app.saveWorkingPoints);
         window.app.localForSetting.setTooltipsOnHeaders();
         resizeTriggered();
      },
      error: function (jqXHR, textStatus, errorThrown) {
         $('#rightColumn').html(jqXHR);
      }
   });
};

window.app.updateGUIWhenBillingInfoArrived = function (data) {
   $('#rightColumn').html(data);
   $("#WarningLimit").attr("readonly", true);
   var maxValue = parseFloat($('#SpendingLimit').val());
   var minValue = 0;
   var stepvalue = maxValue / 100;
   var defaultValue = parseFloat($("#WarningLimit").val());
   $("#slider-range-max").slider({
      range: "max",
      min: minValue,
      max: maxValue,
      step: stepvalue,
      value: defaultValue,
      slide: function (event, ui) {
         $("#WarningLimit").val(ui.value);
      }
   });
   $('#saveBillingInfo').unbind('click');
   $('#saveBillingInfo').bind('click', window.app.saveBillingInfo);
   window.app.localForSetting.setTooltipsOnHeaders();
   resizeTriggered();
};

window.settings.ConfigureCompanyBillingInfo = function () {
   "use strict";
   $.ajax({
      url: "Settings/CompanyBillingInfo",
      cache: false,
      success: function (data) {
         window.app.updateGUIWhenBillingInfoArrived(data);
      },
      error: function (jqXHR, textStatus, errorThrown) {
         $('#rightColumn').html(jqXHR);
      }
   });
};

window.app.saveBillingInfo = function (e) {
   "use strict";
   e.preventDefault();
   var serializedInfo = $('#rightColumn form').serialize();
   $.ajax({
      url: 'Settings/CompanyBillingInfo',
      data: serializedInfo,
      type: 'post',
      cache: false,
      dataType: 'html',      
      success: function (data) {
         window.app.updateGUIWhenBillingInfoArrived(data);
      },
      error: function (jqXHR, textStatus, errorThrown) {
         $('#rightColumn').html(jqXHR);
      }
   });
};

/*DA the convention is that the name of the javascript function is the ReportsMenuItem Action property
* this way we ensure that the correct function is called
*/
window.app.leftSideMenus = {}; //this will hold the FriendlyName <-> Action correspondence 
window.app.SettingsArea = function () {
   "use strict";
   //DA the MenuView is defined in BaseLeftSideMenu.js
   var settingsMenu = new window.app.MenuView({
      el: $("#leftColumn"),
      eventToTriggerOnSelect: 'switchSetting',
      menuCollection: new window.app.MenuCollection({ url: '/Settings/GetMenuItems' }),
      afterInitializeFunction: function (menuItems) {         
         //initialize the routing
         _(menuItems.models).each(function (menuItemModel) {
            //DA the idea is to correctly connect an route to an action -> we need this "actions map"
            if (menuItemModel.get("parent") !== 0) {
               window.app.leftSideMenus[menuItemModel.get("FriendlyName")] = {
                  id: menuItemModel.get("itemId"),
                  action: menuItemModel.get("Action")
               };
            }
         });
         //#region Routing
         var SettingsRouter = Backbone.Router.extend({
            routes: {
               '': 'defaultCall',
               ":menu": "goToMenu"
            },
            goToMenu: function (menuOption) {
               //call the appropriate function
               var action = window.app.leftSideMenus[menuOption];
               if (action !== undefined) {
                  var fn = window.settings[action.action];
                  if (typeof fn === 'function') {
                     fn();
                     //DA this is some fucked up code :)
                     //is should MenuView instance
                     //we operate directly on the DOM (we should not do this)
                     var liItem = ".liItem" + action.id;
                     if (!$(liItem).hasClass("menuItemSelected")) {
                        $(liItem).parents(".collapsibleList").find(".menuItemSelected").removeClass("menuItemSelected");
                        $(liItem).addClass("menuItemSelected");
                        //make sure the parent is expanded
                        //this follows the logic that the parent is 10, 20, 30 and the leafs are 11,12 .. 21, 22
                        var parentID = Math.floor(action.id / 10) * 10;
                        var parentUlItem = "ul.item" + parentID;
                        $(parentUlItem).css("display", "block");
                     }                     
                  }
               }
               else {
                  //DA this means that we don't have access to that function -> go with the default call
                  this.defaultCall();
               }              
            },
            defaultCall: function () {
               $(".liItem21").addClass("menuItemSelected");
               $("ul.item20").css("display", "block");
               window.settings.router.navigate('/ChangePassword', { trigger: true });
            }
         });
         window.settings.router = new SettingsRouter();
         Backbone.history.start();
         //#endregion
      }
   });
   //TODO DA model the menu via a backbone model
   $(document).bind("switchSetting", function (event, menuOptions) {
      window.settings.router.navigate('/' + menuOptions.menuNavigation, { trigger: true });
   });
};