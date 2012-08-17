"use strict";
window.app = window.app || {};

window.app.SettingsArea = function () {
   var self = this;

   var settingsMenu = new window.app.MenuView({
      el: $("#leftColumn"),
      eventToTriggerOnSelect: 'switchSetting',
      menuCollection: new window.app.MenuCollection({ url: '/Settings/GetMenuItems' })
   });

};