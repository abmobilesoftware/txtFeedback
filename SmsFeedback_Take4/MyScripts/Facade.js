﻿"use strict";

window.app = window.app || {};
window.app.calendarCulture = "en-GB";

function newMessageReceivedGUI(convView, msgView, fromID, toId, convID, msgID, dateReceived, text, readStatus) {
   console.log("inside newMessageReceived");
   //the conversations window expects that the toID be a "name" and not a telephone number
   convView.newMessageReceived(fromID, toId, convID, dateReceived, text);
   msgView.messagesView.newMessageReceived(fromID, convID, msgID, dateReceived, text);
}

function refreshConversationList(convView, msgView) {
      convView.getConversations();
      msgView.messagesView.resetViewToDefault();
}

function InitializeGUI() {
   var self = this;
   if (window.Prototype) {
      delete Object.prototype.toJSON;
      delete Array.prototype.toJSON;
      delete String.prototype.toJSON;
   }
   //initialize the filters area
   this.filterArea = new FilterArea();

   //build the areas
   this.wpsArea = new WorkingPointsArea();
   this.wpsView = this.wpsArea.wpPoolView;
   this.convArea = new ConversationArea(self.filterArea, self.wpsArea);
   this.convView = this.convArea.convsView;
   this.tagsArea = TagsArea();
   this.msgView = new MessagesArea(self.convView, self.tagsArea);

    //get the initial working points   
   this.wpsView.getWorkingPoints(function () {
       self.convView.getConversations()
   });
   
   app.nrOfUnreadConvs = 0;
   //get the initial conversations
   app.requestIndex = 0; //make sure the first time we update from external sources
   

   ////the xmpp handler for new messages
   //this.xmppHandler = new app.XMPPhandler();
   //$.getJSON('Xmpp/GetConnectionDetailsForLoggedInUser', function (data) {
   //   if (data !== "") {
   //      self.xmppHandler.connect(data.XmppUser, data.XmppPassword);
   //   }
   //});

     
   $(document).bind('msgReceived', function (ev, data) {
      console.log("msgReceived triggered");    
      newMessageReceivedGUI(self.convView, self.msgView, data.fromID, data.toID, data.convID, data.msgID, data.dateReceived, data.text, false);
   });

   $(document).bind('refreshConversationList', function (ev, data) {
      refreshConversationList(self.convView, self.msgView);
   });

   window.addEventListener("resize", resizeTriggered, false);
   resizeTriggered();
}

$(document).ready(function () {
   $(".menuItem a").click(function () {
      $(".menuItem .active-link").removeClass("active-link");
      $(this).addClass("active-link");
   });
   window.app = window.app || {};

   var culture = $(".currentCulture").val().substring(0,2).toLowerCase();
   if (culture == "en") {
       window.app.calendarCulture = "en-GB";
   } else {
       window.app.calendarCulture = culture;
   }


});

function resizeTriggered() {
   //pick the highest between window size (- header) and messagesArea
   var padding = 5;
   var msgAreaMarginTop = 10;
   var filterStripHeigh = 45;
   var window_height = window.innerHeight;
   var messagesAreaHeight = $('#messagesArea').height();
   var headerHeight = $('header').height();
   var contentWindowHeight = window_height - headerHeight - (2 * padding) - filterStripHeigh;
   var msgAreaCalculatedHeight = messagesAreaHeight + msgAreaMarginTop;
   //TODO determine this factor
   var factor = 140;
   var minHeight = 400; //px
   if (contentWindowHeight <= msgAreaCalculatedHeight) {
      $('.container_12').height(msgAreaCalculatedHeight);
      $('#scrollablemessagebox').height(minHeight);
      $('#scrollableconversations').height(minHeight + factor - 12);
      //$('#conversationsArea').height(msgAreaCalculatedHeight - msgAreaMarginTop);
   }
   else {
      $('.container_12').height(contentWindowHeight);
      $('#scrollablemessagebox').height(contentWindowHeight - factor);
      $('#scrollableconversations').height(contentWindowHeight - 12);
      //$('#conversationsArea').height(contentWindowHeight - msgAreaMarginTop);
      //$('#scrollableconversations').height(contentWindowHeight - msgAreaMarginTop);
      //$('#conversations').height(contentWindowHeight - msgAreaMarginTop);
   }
}