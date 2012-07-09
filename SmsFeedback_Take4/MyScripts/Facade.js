"use strict";

//the domain name should come from the server! - when publishing on cluj-info.com/smsfeedback
var domainName = '/smsfeedback';
//var domainName = '';

function newMessageReceivedGUI(convView, msgView, fromID, toId, convID, msgID, dateReceived, text, readStatus) {
   console.log("inside newMessageReceived");
   //the conversations window expects that the toID be a "name" and not a telephone number
   convView.newMessageReceived(fromID, toId, convID, dateReceived, text);
   msgView.newMessageReceived(fromID, convID, msgID, dateReceived, text);
}

function selectedWPsChanged(convView, msgView, checkedWorkingPoints) {
   console.log('selectedWPsChanged triggered');
   convView.getConversations(checkedWorkingPoints.checkedPhoneNumbers);
   msgView.resetViewToDefault();
}

function refreshConversationList(convView, msgView) {
   convView.getConversations(null);
   msgView.resetViewToDefault();
}

function InitializeGUI() {
   if (window.Prototype) {
      delete Object.prototype.toJSON;
      delete Array.prototype.toJSON;
      delete String.prototype.toJSON;
   }
   //initialize the filters area
   var filterArea = new FilterArea();

   //build the areas
   var wpsArea = WorkingPointsArea();
   var convView = ConversationArea(filterArea, wpsArea);
   var tagsArea = TagsArea();
   var msgView = MessagesArea(convView, tagsArea);

   //get the initial working points
   wpsArea.getWorkingPoints();
   //get the initial conversations
   convView.getConversations();

   //the xmpp handler for new messages
   var xmppHandler = CreateXMPPHandler(convView, msgView);
   $.getJSON('Xmpp/GetConnectionDetailsForLoggedInUser', function (data) {
      xmppHandler.connect(data.XmppUser, data.XmppPassword);
   });


   $(document).bind('msgReceived', function (ev, data) {
      $.getJSON('Messages/MessageReceived',
                    { from: data.fromID, to: data.toID, text: data.text, receivedTime: data.dateReceived, readStatus: data.readStatus },
                    function (data) {
                       //delivered successfully? if yes - indicate this
                       console.log(data);
                    });
      //we listen for all numbers so we have to filter what to show
      _.each(checkedPhoneNumbers.models, function (wp) {
         if (wp.get('CheckedStatus') === true && comparePhoneNumbers(wp.get('TelNumber'), data.toID)) {
            //checkedPhoneNumbersArray.push(wp.get('TelNumber'));
            newMessageReceivedGUI(convView, msgView, data.fromID, wp.get('Name'), data.convID, data.msgID, data.dateReceived, data.text);
         }
      });

   });

   $(document).bind('selectedWPsChanged', function (ev, data) {
      selectedWPsChanged(convView, msgView, data);
   });

   $(document).bind('refreshConversationList', function (ev, data) {
      refreshConversationList(convView, msgView);
   });

   window.addEventListener("resize", resizeTriggered, false);
   resizeTriggered();
}

$(document).ready(function () {
   $(".menuItem a").click(function () {
      $(".menuItem .active-link").removeClass("active-link");
      $(this).addClass("active-link");
   });
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