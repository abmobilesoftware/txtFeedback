$(function () {
   module("TagModel");
   test("Constructor_propertiesAreInitilizedWithDefaultValues", 4, function() {
      var tag = new app.Tag();
      ok(tag.has("Name"), "Name should be present");
      ok(tag.has("Description"), "Description should be present");
      ok(tag.has("TagType"), "TagType should be present");
      ok(tag.has("IsDefault"), "IsDefault should be present");
   });
});