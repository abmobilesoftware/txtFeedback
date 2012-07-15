$(document).ready(function () {
   module("Message");
   test("Constructor_defaultPropertiesAreInitializedCorrectly", 8, function () {
      var msg = new app.Message();
      ok(msg.has("From"), "From should be present");
      ok(msg.has("To"), "To should be present");
      ok(msg.has("Text"), "Text should be present");
      ok(msg.has("TimeReceived"), "TimeReceived should be present");
      ok(msg.has("ConvID"), "ConvesationID should be present");
      ok(msg.has("Direction"), "Direction should be present");
      ok(msg.has("Read"), "Read should be present");
      ok(msg.has("Starred"), "Starred should be present");
   });

});