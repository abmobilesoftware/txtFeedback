﻿$(document).ready(function () {
   module("WorkingPoints", {
      setup: function () {
         this.wpArea = new WorkingPointsArea();
      },
      teardown: function () { }
   });
   test("constructor_allMemberVariablesAreProperlyInitialized", function () {      
      ok(typeof(this.wpArea.wpPoolView) != undefined, "The WpPoolView should be exposed to the outside world");
      ok(typeof(this.wpArea.checkedPhoneNumbers) != undefined, "The checked phone numbers should be exposed");
      ok(typeof (this.wpArea.wpPoolView.getWorkingPoints) != undefined, "getWorkingPoints should be exposed");
      ok(typeof (this.wpArea.wpPoolView.phoneNumbersPool) != undefined, "we should have a collection of wps");
   });

   module("WorkingPoint")
   test("Constructor_defaultConstructor_CheckedStateIsTrue", function () {
      var wp = new app.WorkingPoint();
      deepEqual(wp.get('CheckedStatus'), true, "By default a number should be checked");
   });

   module("WorkingPointView", {
      setup: function () {
         var wp = new app.WorkingPoint({ TelNumber: "0745432345" });
         this.wpView = new app.WorkingPointView({ model: wp });
      },
      teardown: function () {
      }
   });
   test("selectedChanged_ifNrOfCheckedWPointsIs>=2_modelIsUpdated", 1, function () {
      //$(this.wpView.render().el).trigger("click .wpSelectorIcon");
      app.nrOfCheckedWorkingPoints = 2;
      var initialValue = this.wpView.model.get('CheckedStatus');
      this.wpView.selectedChanged();
      deepEqual(this.wpView.model.get('CheckedStatus'), !initialValue, "If enough checked working points the model is updated");
   });
   test("selectedChanged_ifNrOfCheckedWPointsIs<2AndModelInitiallyChecked_modelIsNotUpdated", 1, function () {
      //$(this.wpView.render().el).trigger("click .wpSelectorIcon");
      app.nrOfCheckedWorkingPoints = 1;
      var initialValue = this.wpView.model.get('CheckedStatus');
      this.wpView.selectedChanged();
      deepEqual(this.wpView.model.get('CheckedStatus'), initialValue, "If not enough checked working points the model is not unchecked");
   });
   test("selectedChanged_ifNrOfCheckedWPointsIs<2AndModelInitiallyUnchecked_modelIsUpdated", 1, function () {
      //$(this.wpView.render().el).trigger("click .wpSelectorIcon");
      app.nrOfCheckedWorkingPoints = 1;
      this.wpView.model.set('CheckedStatus', false);
      this.wpView.selectedChanged();
      deepEqual(this.wpView.model.get('CheckedStatus'), true, "You can always enable a working point");
   });
   test("selectedChanged_clickCheckboxAndNrOfCheckedWPoints>=2_modelIsUpdated", 1, function () {
      app.nrOfCheckedWorkingPoints = 2;
      var event = $.Event("click .wpSelectorIcon");
      var initialValue = this.wpView.model.get('CheckedStatus');
      $(this.wpView).trigger(event);
      deepEqual(this.wpView.model.get('CheckedStatus'), !initialValue, "When clicking on the checkbutton the model is updated");
   });
});