$(function () {
   module("ConversationModel");
   test("Constructor_PropsAreInitializedWithDefaultValues", 8,function () {
      var conv = new app.Conversation();
      var expectedStarredValue = false;
      var expectedReadValue = false;
      ok(conv.has("Starred"), "Starred should be present");
      deepEqual(conv.get("Starred"), expectedStarredValue, "Starred should be false");
      ok(conv.has("Read"), "Read should be present");
      deepEqual(conv.get("Read"), expectedReadValue, "Read should be false");
      ok(conv.has("TimeUpdated"), "TimeUpdated should be present");
      ok(conv.has("To"), "To should be present");
      ok(conv.has("From"), "From should be present");
      ok(conv.has("Text"), "Text should be present");
   });

   module("ConversationView");
   test("Render_modelIsUnread_theRenderedElementWillBeUnread", function () {
      var conv = new app.Conversation({ 'Read': false });
      var convView = new app.ConversationView({ model: conv });
      convView.render();
      ok($(convView.el).hasClass("unreadconversation"), "Element should be unread");
   });
   test("Render_modelIsRead_theRenderedElementWillBeRead", function () {
      var conv = new app.Conversation({ 'Read': true });
      var convView = new app.ConversationView({ model: conv });
      convView.render();
      ok($(convView.el).hasClass("readconversation"), "Element should be read");
   });

   module("ConversationArea");
   test("Constructor_theRequiredMethodsAreThere", function () {
      var filterArea,wpArea;
      var convArea = new ConversationArea(filterArea, wpArea);
      ok(typeof (convArea.convsView) != undefined, "convsView shold be exposed");
      ok(typeof (convArea.newMessageReceived) != undefined, "newMessageReceived should be exposed");
      ok(typeof (convArea.getConversations) != undefined, "getConversations should be exposed");
      ok(typeof (convArea.getAdditionalConversations) != undefined, "getAdditionalConversations should be exposed");

   });
});