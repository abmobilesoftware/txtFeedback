//#region Defines to stop jshint from complaining about "undefined objects"
/*global window */
/*global Strophe */
/*global document */
/*global console */
/*global $pres */
/*global $iq */
/*global $msg */
/*global Persist */
/*global DOMParser */
/*global ActiveXObject */
/*global Backbone */
/*global _ */
/*global Spinner */
/*global FilterArea */
/*global WorkingPointsArea */
/*global ConversationArea */
/*global TagsArea */
/*global MessagesArea */
//#endregion

window.app = window.app || {};
window.app.calendarCulture = "en-GB";
window.app.appStartTime = "";

function newMessageReceivedGUI(convView, msgView, fromID, toId, convID, msgID, dateReceived, text, readStatus, isSmsBased) {
   //the conversations window expects that the toID be a "name" and not a telephone number
   convView.newMessageReceived(fromID, toId, convID, dateReceived, text, readStatus, isSmsBased);
   msgView.messagesView.newMessageReceived(fromID, convID, msgID, dateReceived, text, readStatus, isSmsBased);
}

function resetMessagesViewToInitialState(msgView) {
   msgView.messagesView.resetViewToDefault();
}

function refreshConversationList(convView, msgView) {
   convView.getConversations();
   resetMessagesViewToInitialState(msgView);
}

function messageSuccessfullySent(msgView, message) {
   msgView.messagesView.messageSuccessfullySent(message);
}

function acknowledgeFromClient(msgView, message) {
   msgView.messagesView.setAcknowledgeFromClient(message);   
}



function InitializeGUI() {
   "use strict";
   var self = this;
   if (window.Prototype) {
      delete Object.prototype.toJSON;
      delete Array.prototype.toJSON;
      delete String.prototype.toJSON;
   }
   //make a note of the starting time - we'll use this for "delayed sent messages"    
   window.app.appStartTime = new Date();
   //initialize the filters area
   this.filterArea = new FilterArea();

   //build the areas
   this.wpsArea = new window.app.WorkingPointsArea();
   this.wpsView = this.wpsArea.wpPoolView;
   this.convArea = new ConversationArea(self.filterArea, self.wpsArea);
   this.convView = this.convArea.convsView;
   this.tagsArea = TagsArea();
   this.msgView = new MessagesArea(self.convView, self.tagsArea, self.wpsArea);

   //get the initial working points   
   this.wpsView.getWorkingPoints(function () {
      self.convView.getConversations();
   });

   window.app.nrOfUnreadConvs = 0;
   //get the initial conversations
   window.app.requestIndex = 0; //make sure the first time we update from external sources

   $(document).bind('msgReceived', function (ev, data) {
      newMessageReceivedGUI(self.convView, self.msgView, data.fromID, data.toID, data.convID, data.msgID, data.dateReceived, data.text, data.readStatus, data.isSmsBased);
   });
   $(document).bind('refreshConversationList', function (ev, data) {
      refreshConversationList(self.convView, self.msgView);
   });
   $(document).bind('selectedConvDeleted', function (ev, data) {
      resetMessagesViewToInitialState(self.msgView);
   });
   $(document).bind('serverAcknowledge', function (ev, data) {
      messageSuccessfullySent(self.msgView, data.message);
   });
   $(document).bind('clientAcknowledge', function (ev, data) {
      acknowledgeFromClient(self.msgView, data.message);
   });


   resizeTriggered();
   window.app.startReconnectTimer();
}

$(document).ready(function () {
   $(".menuItem a").click(function () {
      $(".menuItem .active-link").removeClass("active-link");
      $(this).addClass("active-link");
   });
   window.app = window.app || {};

   var culture = $(".currentCulture").val().substring(0, 2).toLowerCase();
   if (culture === "en") {
      window.app.calendarCulture = "en-GB";
   } else {
      window.app.calendarCulture = culture;
   }
});


