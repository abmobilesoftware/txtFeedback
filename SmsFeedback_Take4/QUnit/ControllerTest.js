$(function () {
   module("MessagesController")   
   asyncTest("Messages/MessagesList_validConvID_dataIsReturnedWithRequiredFields", 9, function () {
      var validConvID = "442033221134-442033221134";
      var validUrlLocationForMessagesList = "/ro-RO/Messages/MessagesList";
      //we have to be logged in for this to work, otherwise we would receive back the LogOn dialog
      $.ajax({         
         url: validUrlLocationForMessagesList,
         data: { "conversationId": validConvID },
         success: function (data) {
            ok(data.length >= 1, "Some valid data should be returned");
            var msg = data[0];
            ok($(msg).attr("Id") != undefined, "Id should be present");
            ok($(msg).attr("From") != undefined, "From should be present");
            ok($(msg).attr("Text") != undefined, "Text should be present");
            ok($(msg).attr("TimeReceived") != undefined, "TimeReceived should be present");
            ok($(msg).attr("ConvID") != undefined, "Conversation ID should be present");

            ok($(msg).attr("Direction") == undefined, "Direction should not be present");

            ok($(msg).attr("Read") != undefined, "Read should be present");
            ok($(msg).attr("Starred") != undefined, "Starred should be present");
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      })   
   });
   asyncTest("Messages/MessagesList_validConvIDUppercaseParamSpec_dataIsReturnedWithRequiredFields", 9, function () {
      var validConvID = "442033221134-442033221134";
      var validUrlLocationForMessagesList = "/ro-RO/Messages/MessagesList";
      //we have to be logged in for this to work, otherwise we would receive back the LogOn dialog
      $.ajax({
         url: validUrlLocationForMessagesList,
         data: { "CONVERSATIONID": validConvID },
         success: function (data) {
            ok(data.length >= 1, "Some valid data should be returned");
            var msg = data[0];
            ok($(msg).attr("Id") != undefined, "Id should be present");
            ok($(msg).attr("From") != undefined, "From should be present");
            ok($(msg).attr("Text") != undefined, "Text should be present");
            ok($(msg).attr("TimeReceived") != undefined, "TimeReceived should be present");
            ok($(msg).attr("ConvID") != undefined, "Conversation ID should be present");

            ok($(msg).attr("Direction") == undefined, "Direction should not be present");

            ok($(msg).attr("Read") != undefined, "Read should be present");
            ok($(msg).attr("Starred") != undefined, "Starred should be present");
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      })
   });
   asyncTest("Messages/MessagesList_validConvIDLowercaseParamSpec_dataIsReturnedWithRequiredFields", 9, function () {
      var validConvID = "442033221134-442033221134";
      var validUrlLocationForMessagesList = "/ro-RO/Messages/MessagesList";
      //we have to be logged in for this to work, otherwise we would receive back the LogOn dialog
      $.ajax({
         url: validUrlLocationForMessagesList,
         data: { "conversationid": validConvID },
         success: function (data) {
            ok(data.length >= 1, "Some valid data should be returned");
            var msg = data[0];
            ok($(msg).attr("Id") != undefined, "Id should be present");
            ok($(msg).attr("From") != undefined, "From should be present");
            ok($(msg).attr("Text") != undefined, "Text should be present");
            ok($(msg).attr("TimeReceived") != undefined, "TimeReceived should be present");
            ok($(msg).attr("ConvID") != undefined, "Conversation ID should be present");

            ok($(msg).attr("Direction") == undefined, "Direction should not be present");

            ok($(msg).attr("Read") != undefined, "Read should be present");
            ok($(msg).attr("Starred") != undefined, "Starred should be present");
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      })
   });
   asyncTest("Messages/MessagesList_nullConvID_JsonWithErrorMessageShouldBeReturned", 1, function () {
      var nullConvID = null;
      var validUrlLocationForMessagesList = "/ro-RO/Messages/MessagesList";
      //we have to be logged in for this to work, otherwise we would receive back the LogOn dialog
      $.ajax({
         url: validUrlLocationForMessagesList,
         data: { "conversationId": nullConvID },
         success: function (data) {
            var msg = data;
            ok($(msg).attr("error") == "NullConvId", "The convId is null, json with message describing the error should be returned");
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      })
   });
   asyncTest("Messages/MessagesList_invalidConvID_noDataIsReturned",1, function () {
      var invalidConvID = "000000000000";
      var validUrlLocationForMessagesList = "/ro-RO/Messages/MessagesList";
      //we have to be logged in for this to work, otherwise we would receive back the LogOn dialog
      $.ajax({
         url: validUrlLocationForMessagesList,
         data: { "conversationId": invalidConvID },
         success: function (data) {
            ok(data == null, "Null result received");
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      })
   });
   asyncTest("Messages/ConversationsList_widestOptionsPossible_dataIsReturned",7, function () {
      var filterOptions = {};
      filterOptions["onlyFavorites"] = false;
      filterOptions["tags"] = [];
      filterOptions["workingPointsNumbers"] = [];
      filterOptions["startDate"] = null;
      filterOptions["endDate"] = null;
      filterOptions["onlyUnread"] = false;
      filterOptions["skip"] = 0;
      filterOptions["top"] = 10;
      filterOptions["requestIndex"] = 0

      var validUrlLocationForConversationsList = "/ro-RO/Messages/ConversationsList";
      $.ajax({
         url: validUrlLocationForConversationsList,
         data: filterOptions,
         success: function (data) {
            ok(data.length >= 1, "Valid data should be returned");
            var conv = data[0];
            ok($(conv).attr("TimeReceived") != undefined, "TimeReceived should be present");
            ok($(conv).attr("Read") != undefined, "Read should be present");
            ok($(conv).attr("Text") != undefined, "Text should be present");
            ok($(conv).attr("From") != undefined, "From should be present");
            ok($(conv).attr("To") != undefined, "To should be present");
            ok($(conv).attr("Starred") != undefined, "Starred should be present");
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      })
   });
   asyncTest("Messages/WorkingPointsPerUser_loggedInUser_ReturnsAtLeast1Wp",4, function () {
      var validUrlForWpList = "/ro-Ro/Messages/WorkingPointsPerUser";
      $.ajax({
         url: validUrlForWpList,
         success: function (data) {
            ok(data.length >= 1, "At least one wp should be returned");
            var wp = data[0];
            ok($(wp).attr("TelNumber") != undefined, "TelNumber should be present");
            ok($(wp).attr("Name") != undefined, "Name should be present");
            ok($(wp).attr("Description") != undefined, "Description should be present");
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      });
   });
   asyncTest("Messages/ChangeStarredStatusForConversation_nullConvId_JsonWithErrorMessageIsReturned",1, function () {
      var validUrlForChangeStarredStatus = "/ro-RO/Messages/ChangeStarredStatusForConversation";
      var nullConvIdValue = null;
      var validStarredStatus = true;
      $.ajax({
         url: validUrlForChangeStarredStatus,
         data: {
            "conversationId": nullConvIdValue,
            "newStarredStatus": validStarredStatus
         },
         success: function (data) {
            ok($(data).attr("error") == "NullConvId", "Json with error message: NullConvId");
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      });
   });
   asyncTest("Messages/ChangeStarredStatusForConversation_noParamsPassed_JsonWithErrorMessageIsReturned", 1, function () {
      var validUrlForChangeStarredStatus = "/ro-RO/Messages/ChangeStarredStatusForConversation";
      
      $.ajax({
         url: validUrlForChangeStarredStatus,      
         success: function (data) {
             ok($(data).attr("error") == "noConvIdPassed", "Json with error message: noConvIdPassed");             
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      });
   });
   asyncTest("Messages/ChangeStarredStatusForConversation_nullStarredStatusPassed_JsonWithErrorMessageIsReturned", 1, function () {
      var validUrlForChangeStarredStatus = "/ro-RO/Messages/ChangeStarredStatusForConversation";
      var validConvIdValue = "442033221134-442033221134";
      var nullStarredStatus = null;
      $.ajax({
         url: validUrlForChangeStarredStatus,
         data: {
            conversationId: validConvIdValue,
            newStarredStatus: nullStarredStatus
         },
         success: function (data) {
            ok($(data).attr("error") == "nullStarredStatus", "Json with error message: nullStarredStatus");
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      });
   });
   asyncTest("Messages/MarkConversationAsRead_nullConvID_JsonWithErrorMessageIsReturned", 1, function () {
      var validUrlForMarkAsRead = "/ro-RO/Messages/MarkConversationAsRead";
      var nullConvId = null;
      $.ajax({
         url: validUrlForMarkAsRead,
         data: { conversationId: nullConvId },
         success: function (data) {
             ok($(data).attr("error") == "NullConvId", "Json with error message: NullConvId");
            start();
         },
         error: function (xhr, ajaxOptions, thrownError) {
            ok(false, xhr.responseText);
            start();
         }
      });
   });
});