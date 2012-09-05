"use strict";
window.app = window.app || {};

window.app.showChangePassword = function () {
   $.ajax({
      url: "Settings/GetChangePasswordForm",
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
               error: function (data) {
                  var y = data;
                  $('#rightColumn').html(data);
               }
            });
         });
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
         default:
      }
   });
};

