$(function () {
   module("Filtering");
   test("Constructor_memberVariablesAreInitializedWithDefaultValues", function () {
      expect(15);
      var filterArea = new FilterArea();
      ok(filterArea != undefined, "The object should be initialized");
      ok(filterArea['tagsForFiltering'] != undefined, "tagsForFiltering should be present");
      deepEqual(filterArea['tagsForFiltering'], app.defaultFilteringOptions.tagsForFiltering, "tagsForFiltering is properlly initialized");
      ok(filterArea['tagFilteringEnabled'] != undefined, "tagFilteringEnabled should be present");
      deepEqual(filterArea['tagFilteringEnabled'], app.defaultFilteringOptions.tagFilteringEnabled, "tagFilteringEnabled is properlly initialized");
      ok(filterArea['dateFilteringEnabled'] != undefined, "dateFilteringEnabled should be present");
      deepEqual(filterArea['dateFilteringEnabled'], app.defaultFilteringOptions.dateFilteringEnabled, "dateFilteringEnabled is properlly initialized");
      ok(filterArea['startDate'] != undefined, "startDate should be present");
      deepEqual(filterArea['startDate'], app.defaultFilteringOptions.startDate, "startDate is properlly initialized");
      ok(filterArea['endDate'] != undefined, "endDate should be present");
      deepEqual(filterArea['endDate'], app.defaultFilteringOptions.endDate, "endDate is properlly initialized");
      ok(filterArea['starredFilteringEnabled'] != undefined, "starredFilteringEnabled should be present");
      deepEqual(filterArea['starredFilteringEnabled'], app.defaultFilteringOptions.starredFilteringEnabled, "starredFilteringEnabled is properlly initialized");
      ok(filterArea['unreadFilteringEnabled'] != undefined, "unreadFilteringEnabled should be present");
      deepEqual(filterArea['unreadFilteringEnabled'], app.defaultFilteringOptions.unreadFilteringEnabled, "unreadFilteringEnabled is properlly initialized");
      //ok(filterArea['unreadFilteringEnabled'] != undefined, "unreadFilteringEnabled should be present");
      //deepEqual(filterArea['unreadFilteringEnabled'], app.defaultFilteringOptions.unreadFilteringEnabled, "unreadFilteringEnabled is properlly initialized");
   });

   test("IsFilteringEnabled_constructor_retursByDefaultFalse", function () {
      expect(2);
      var filterArea = new FilterArea();
      ok(filterArea.IsFilteringEnabled != undefined, "IsFilteringEnabled should be exposed");
      deepEqual(filterArea.IsFilteringEnabled(), false, "IsFilteringEnabled should be false by default");
   });
   test("IsFilteringEnabled_starredFilteringEnabled_returnsTrue", function () {
      var filterArea = new FilterArea();
      filterArea.starredFilteringEnabled = true;
      equal(filterArea.IsFilteringEnabled(), true, "IsFilteringEnabled should return true");
   });
   test("IsFilteringEnabled_dateFilteringEnabled_returnsTrue", function () {
      var filterArea = new FilterArea();
      filterArea.dateFilteringEnabled = true;
      equal(filterArea.IsFilteringEnabled(), true, "IsFilteringEnabled should return true");
   });
   test("IsFilteringEnabled_tagFilteringEnabled_returnsTrue", function () {
      var filterArea = new FilterArea();
      filterArea.tagFilteringEnabled = true;
      equal(filterArea.IsFilteringEnabled(), true, "IsFilteringEnabled should return true");
   });
   test("IsFilteringEnabled_unreadFilteringEnabled_returnsTrue", function () {
      var filterArea = new FilterArea();
      filterArea.unreadFilteringEnabled = true;
      equal(filterArea.IsFilteringEnabled(), true, "IsFilteringEnabled should return true");
   });
});