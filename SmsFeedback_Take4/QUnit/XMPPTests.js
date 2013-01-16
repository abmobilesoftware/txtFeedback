/*global window */
/*global test */
/*global ok */
/*global module */
$(document).ready(function () {
   module("XMPP_TemporaryMessageQueue_Helpers", {
      setup: function () {
         // msgID = "c"
         var msgUnsent = new MessageUnsent("a", "b", "c", "d", "e", "f", "g", "h", "i");
         window.app.tempMsgQueue.push(msgUnsent);
      }
   });
   test("TemporaryMessageQueue_GetMessageByID_c_ReturnValidResult", 1, function () {
      var message = getMessage("c");
      ok(message.msgID, "c");
   });

   test("TemporaryMessageQueue_GetMessageByID_d_ReturnNullResult", 1, function () {
      ok(getMessage("d"), null);
   });

   test("TemporaryMessageQueue_RemoveMessageByID_c_MessageDeleted", 1, function () {
      removeMessageById("c");
      ok(getMessage("c"), null);
   });

});