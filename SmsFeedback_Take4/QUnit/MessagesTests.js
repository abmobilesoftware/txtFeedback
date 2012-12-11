$(document).ready(function () {
   module("Message");
   test("Constructor_defaultPropertiesAreInitializedCorrectly", 11, function () {
      var msg = new app.Message();
      ok(msg.has("From"), "From should be present");
      ok(msg.has("To"), "To should be present");
      ok(msg.has("Text"), "Text should be present");
      ok(msg.has("TimeReceived"), "TimeReceived should be present");
      ok(msg.has("ConvID"), "ConvesationID should be present");
      ok(msg.has("Direction"), "Direction should be present");
      ok(msg.has("Read"), "Read should be present");
      ok(msg.has("Starred"), "Starred should be present");
      ok(msg.has("IsSmsBased"), "IsSmsBased should be present");
      ok(msg.has("WasSuccessfullySent"), "WasSuccessfullySent should be present");
      //based on http://stackoverflow.com/questions/126100/how-to-efficiently-count-the-number-of-keys-properties-of-an-object-in-javascrip
      ok(Object.keys(msg.attributes).length === 10, "If you add more properties - they should be accounted for");
   });

   module("MessageArea");
   test("ConstructorMessagesArea_defaultPropertiesArePresentAndInitialized", function () {
      var tagsArea, convView = {};
      var msgArea = new MessagesArea(convView, tagsArea);
      ok($(msgArea).attr("messagesRep") != undefined, "messagesRep should be present");
      ok($(msgArea).attr("currentConversationId") != undefined, "currentConversationId should be present");
      ok($(msgArea).attr("messagesView") != undefined, "messagesView should be present");
   });
});