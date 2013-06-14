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
/*global buildConversationID */
/*global cleanupPhoneNumber */
/*global setTimeout */
//#endregion
window.app = window.app || {};
window.app.xmppConn = {};
window.app.getFeaturesIQID = "info14";
window.app.selfXmppAddress = "";
window.app.tempMsgQueue = [];
window.app.pageBlinkIntervalId = null;
/*
when receiving messages it is important that each message is associated an unique id (js wise)
so we start from a certain id and each time we receive/send a message, we increment the id
*/
window.app.receivedMsgID = 12345;

//#region Helpers
function getMessage(msgID) {
   for (var j = 0; j < window.app.tempMsgQueue.length; ++j) {
      if (window.app.tempMsgQueue[j].msgID == msgID) {
         return window.app.tempMsgQueue[j];
      }
   }
   return null;
}

function removeMessageById(msgID) {
   for (var j = 0; j < window.app.tempMsgQueue.length; ++j) {
      if (window.app.tempMsgQueue[j].msgID == msgID) {
         window.app.tempMsgQueue.splice(j, 1);
         return true;
      }
   }
   return false;   
}

function updatePageTitle() {
    pageTitle1 = window.app.pageTitle;
    pageTitle2 = $("#gotANewMessage").val();
    if (document.title == pageTitle1) {
        document.title = pageTitle2;
    } else {
        document.title = pageTitle1;
    }
}

function stopUpdatingPageTitle() {
    clearInterval(window.app.pageBlinkIntervalId);
    document.title = window.app.pageTitle;
    document.onmousemove = null;
    window.app.pageBlinkIntervalId = null;
}

//#endregion

// TODO: Check if Message model from Messages.js can be used 
function MessageUnsent(body, xmppTo, msgID, convID, from, dateReceived, text, read, isSmsBased) {
   this.body = body;
   this.xmppTo = xmppTo;
   this.msgID = msgID;
   this.convID = convID;
   this.from = from;
   this.dateReceived = dateReceived;
   this.text = text;
   this.read = read;
   this.isSmsBased = isSmsBased;
}

function getNumberOfConversationsWithUnreadMessages() {
   "use strict";
   window.app.updateNrOfUnreadConversations(false);
}

window.app.setNrOfUnreadConversationOnTab = function (unreadConvs) {
   "use strict";
   window.app.nrOfUnreadConvs = unreadConvs;
   var toShow = "(" + unreadConvs + ")";
   $("#msgTabcount").text(toShow);
};

//#region "timer for reconnect"
window.app.reconnectTimer = {};
window.app.intervalToWaitBetweenChecks = 15000;
function reconnectIfRequired() {
   if (window.app.xmppConn && window.app.xmppConn.conn && !window.app.xmppConn.conn.connected && !window.app.XMPPConnecting) {
      window.app.XMPPConnecting = true;
      window.app.xmppHandlerInstance.connect(window.app.xmppConn.userid, window.app.xmppConn.password, window.app.xmppConn.connectCallback);
   }
   window.app.startReconnectTimer();
}
window.app.startReconnectTimer = function () {
   clearTimeout(window.app.reconnectTimer);
   window.app.reconnectTimer = setTimeout(reconnectIfRequired, window.app.intervalToWaitBetweenChecks);
};
//#endregion



//#region Receive message
//TODO DA move this somewhere else :)
window.app.handleIncommingMessage = function (msgContent, isIncomming) {
   window.app.receivedMsgID++;
   var xmlDoc;
   if (window.DOMParser) {
      var parser = new DOMParser();
      xmlDoc = parser.parseFromString(msgContent, "text/xml");
   }
   else {
      xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
      xmlDoc.async = false;
      xmlDoc.loadXML(msgContent);
   }
   var xmlMsgToBeDecoded = xmlDoc.getElementsByTagName("msg")[0];
   if (xmlMsgToBeDecoded !== undefined) {
      //DA we have received a new message - so update the nr of unread conversations (this is generic)
      $(document).trigger('updateUnreadConvsNr');
      //DA now determine if we are in the Conversations tab so that we need to display the message
      if (window.app.isInConversationsTab != undefined && window.app.isInConversationsTab === true) {
         var rawFromID = xmlMsgToBeDecoded.getElementsByTagName('from')[0].textContent;
         var rawToID = xmlMsgToBeDecoded.getElementsByTagName('to')[0].textContent;
         var toID = cleanupPhoneNumber(rawToID);
         var fromID = cleanupPhoneNumber(rawFromID);
         var extension;
         /*
DA: the following line seems weird and it actually is :)
Right now a Working Point XMPP address is shortID@moderator.txtfeedback.net
In order not to hard code the @ prefix we try to retrieve it from SuffixDictionary
The issue is that the WP's address might be the from address or the to address (depending on different factors)
But for sure the WP is either the to or the from -> we will find it in the suffix dictionary
To avoid complicated logic we test both from and to in the suffix dictionary and one of them will hit :)
*/
         extension = window.app.workingPointsSuffixDictionary[toID] || window.app.workingPointsSuffixDictionary[fromID];
         //decide if we are dealing with a message coming from another WorkingPoint
         var isFromWorkingPoint = isWorkingPoint(rawFromID, extension);
         var dateReceived = xmlMsgToBeDecoded.getElementsByTagName('datesent')[0].textContent;
         var isSmsBasedAsString = xmlMsgToBeDecoded.getElementsByTagName('sms')[0].textContent;
         var isSmsBased = false;
         if (isSmsBasedAsString === "true") {
            isSmsBased = true;
         }
         var convID;
         if (isFromWorkingPoint && isIncomming) {
            convID = buildConversationID(fromID, toID);
         } else {
            convID = xmlMsgToBeDecoded.getElementsByTagName("convID")[0].textContent;
         }

         var newText = xmlMsgToBeDecoded.getElementsByTagName("body")[0].textContent;
         var readStatus = false; //one "freshly received" message is always unread
         window.app.receivedMsgID++;
         //DA the received time should be in UTC time
         var asDateObject = new Date(Date.parse(dateReceived));
         //Although now the time is shown in the correct timezone, we have to actually add timezone difference
         var milisecondsInMinute = 60000;
         var localOffset = window.app.appStartTime.getTimezoneOffset() * milisecondsInMinute;
         //a negative return value from getTimezoneOffset() indicates that the current location is ahead of UTC, while a positive value indicates that the location is behind UTC.
         asDateObject = new Date(asDateObject.getTime() - localOffset);
         $(document).trigger('msgReceived', {
            fromID: fromID,
            toID: toID,
            convID: convID,
            msgID: window.app.receivedMsgID,
            dateReceived: asDateObject,
            text: newText,
            readStatus: readStatus,
            isSmsBased: isSmsBased
         });
      }
   }
};
//#endregion


window.app.XMPPhandler = function XMPPhandler() {
   var self = this;
   this.userid = null;
   this.password = null;
   this.conn = null;

   this.connection = null;
   this.start_time = null;
   this.connectCallback = function (status) {
      var needReconnect = false;
      if (status === Strophe.Status.CONNECTED) {
         //window.app.logDebugOnServer("XMPP connected");
         window.app.xmppConn.connection = window.app.xmppConn.conn;
         window.app.xmppConn.connection.addHandler(window.app.xmppConn.handle_infoquery, null, "iq", null, null);
         window.app.xmppConn.connection.addHandler(window.app.xmppConn.handle_message, null, "message", null, null);
         var domain = Strophe.getDomainFromJid(window.app.xmppConn.connection.jid);
         //window.app.xmppConn.send_ping(domain);
         window.app.xmppConn.send_initial_presence(domain);
         window.app.xmppConn.getInfo(domain);

         //self.request_conversations(self.account_number);
      } else if (status === Strophe.Status.CONNECTING) {
         //window.app.logDebugOnServer("XMPP connecting...");
      } else if (status === Strophe.Status.AUTHENTICATING) {
         // window.app.logDebugOnServer("XMPP authenticating...");
      } else if (status === Strophe.Status.DISCONNECTED) {
         //window.app.logDebugOnServer("XMPP disconnected");
         needReconnect = true;
      } else if (status === Strophe.Status.CONNFAIL) {
         window.app.logDebugOnServer("XMPP connection fail");
         window.app.xmppConn.conn.disconnect();
      } else if (status === Strophe.Status.AUTHFAIL) {
         window.app.logDebugOnServer("XMPP authentication failed");
         window.app.xmppConn.conn.disconnect();
      } else if (status === Strophe.Status.ERROR) {
         window.app.logDebugOnServer("XMPP status error");
         window.app.xmppConn.conn.disconnect();
      }
      if (needReconnect) {
         window.app.xmppConn.connect(window.app.xmppConn.userid, window.app.xmppConn.password, window.app.xmppConn.connectCallback);
      }
   };
   this.connect = function (userid, password) {
      window.app.logDebugOnServer("XMPP connecting with user [" + userid + "]");
      var self = this;
      //var xmppServerAddress = "http://localhost:3333/app/dsadsa/http-bindours/";
      //var xmppServerAddress = "http://www.cluj-info.com/smsfeedback/nocontroller/http-bindours/";
      var xmppServerAddress = "http://46.137.26.124:5280/http-bind/";
      self.conn = new Strophe.Connection(xmppServerAddress);
      self.userid = userid;
      self.password = password;
      window.app.xmppConn = this;

      self.conn.connect(userid, password, self.connectCallback);
   };
   this.disconnect = function () {
      var self = this;
      //DA based on http://stackoverflow.com/questions/5198410/disconnect-strophe-connection-on-page-unload
      self.conn.sync = true; // Switch to using synchronous requests since this is typically called onUnload.
      self.conn.flush();
      self.conn.disconnect();
   };
   this.send_ping = function (to) {
      var ping = $iq({
         to: to,
         type: "get",
         id: "ping1"
      }).c("ping", { xmlns: "urn:xmpp:ping" });

      this.start_time = (new Date()).getTime();
      this.connection.send(ping);
   };
   this.send_initial_presence = function (to) {
      var presence = $pres().c("status").t("Ready or not");
      this.connection.send(presence);
   };
   this.enableCarbons = function (to) {
      var enCarbons = $iq({
         type: "set",
         id: "enableCarbons"
      }).c("enable", { xmlns: "urn:xmpp:carbons:1" });
      this.connection.send(enCarbons);
   };
   this.getInfo = function (to) {
      var reqInfo = $iq({
         to: to,
         type: "get",
         id: window.app.getFeaturesIQID
      }).c("query", { xmlns: "http://jabber.org/protocol/disco#info" });
      this.connection.send(reqInfo);
   };
   this.sendAckMessage = function (xmppTo, ackID, ackDest) {
      var self = this;
      var replymsg = $msg({
         to: xmppTo
      }).c("subject").t("ClientMsgDeliveryReceipt");
      replymsg.up();
      replymsg.c("received", { xmlns: "urn:xmpp:receipts", id: ackID, ackDest: ackDest });
      //DA - when sending make sure that we are connected
      window.app.xmppConn.conn.send(replymsg);
   };
   this.send_reply = function (from, to, dateSent, convID, message, xmppTo, isSmsBased, toStaff, msgID) {
      /*
         here we should just build the message from what is handed to us
         there should be no responsibility regarding the logic of building the content of the message
         but only regarding the structure (what fields have to be filled in)
      */
      var message_body = "<msg>" +
                                    " <from>" + from + "</from>" +
                                    " <to>" + to + "</to>" +
                                    " <datesent>" + dateSent + "</datesent>" +
                                     "<convID>" + convID + "</convID>" +
                                    " <body>" + Strophe.xmlescape(message) + "</body>" +
                                    " <staff>" + toStaff.toString() + "</staff>" +
                                    " <sms>" + isSmsBased.toString() + "</sms>" +
                                " </msg>";
      var messageToBeSent = new MessageUnsent(message_body, xmppTo, msgID, convID, from, dateSent, message, false, false);
      this.send_message(messageToBeSent);      
   };
   this.send_message = function (message) {
      var self = this;
      var replymsg = $msg({
         id: message.msgID,
         to: message.xmppTo,
         type: "chat"
      }).c("body").t(message.body);
      replymsg.up();
      replymsg.c("subject").t("internal_packet");
      replymsg.up();
      replymsg.c("request", { xmlns: "urn:xmpp:receipts" });

      if (window.app.xmppConn.conn.authenticated) {
         window.app.tempMsgQueue.push(message);
         window.app.xmppConn.conn.send(replymsg);
      } else {
         //force synch connect 
         if (!window.app.XMPPConnecting) {
            window.app.XMPPConnecting = true;
            window.app.xmppHandlerInstance.connect(window.app.xmppHandlerInstance.userid, window.app.xmppHandlerInstance.password, self.connectCallback);
         }
      }
   };

   this.handle_infoquery = function (iq) {
      var currentIq = $(iq);
      if (currentIq.attr("id") === window.app.getFeaturesIQID) {
         window.app.xmppConn.enableCarbons(currentIq.attr("from"));
      }
      if (currentIq.attr("type") === "result") {
         //TODO establish if relevant
      }
      if (currentIq.attr("type") === "error") {
         //TODO what now?
         window.app.logDebugOnServer("XMPP info query error, ");
      }
      return true;
   };
   this.handle_message = function (message) {
      if ($(message).attr("type") === "getConversationsResponse") {
         //this.displayConversationsList(message);
         window.app.logDebugOnServer("XMPP Conversations list reponse received");
      } else if ($(message).attr("type") === "sendSmsResponse") {
         if ($(message).children("body").text() === "error") {
            $("#quick_replay_text").val("");
         } else {
            this.displayConversationsList(message);
            $("#quick_replay_text").val("");
         }
      } else if ($(message).attr("type") === "getMessagesForConversationResponse") {
         var messages = $(message).children("body").text();
         this.displayMessagesForConversation(messages);
      } else if ($(message).children("subject").text() === "ServerMsgDeliveryReceipt") {
         var messageId = $(message).children("received").attr("id");
         $(document).trigger('serverAcknowledge', {
            message: getMessage(messageId)
         });         
      } else if ($(message).children("subject").text() === "ClientMsgDeliveryReceipt") {
         var messageId = $(message).children("received").attr("id");        
         if ($(message).children("payload").length == 1) {
            var payload = $(message).children("payload").text();
            $(document).trigger('smsPayloadReceived', { content: payload, messageId:messageId });
         } else {
            $(document).trigger('clientAcknowledge', {
               message: getMessage(messageId)
            });
         }
      } else if ($(message).attr("type") === "result") {
         //TODO result relevant to us?
      } else if ($(message).attr("type") === "chat") {
         //if we are dealing with a forwarded message then the body tag will be non zero
         if ($(message).children("sent").attr("xmlns") === "urn:xmpp:carbons:1") {
            if (message.getElementsByTagName("body") !== undefined && message.getElementsByTagName("body").length !== 0) {
               var carbonMsg = Strophe.getText(message.getElementsByTagName('body')[0]);
               if (window.app.handleIncommingMessage !== undefined) {
                  window.app.handleIncommingMessage(carbonMsg, false);
               }
            }
         }
         else {
             //incommingMSG
             if (window.app.pageBlinkIntervalId == null) {
                 window.app.pageBlinkIntervalId = setInterval(updatePageTitle, 2000);
                 document.onmousemove = stopUpdatingPageTitle;
             }
            //DA check if we are dealing with a delayed message or not - if yes - disregard the message
            if ($(message).children("delay").length === 0) {
               //according to http://xmpp.org/extensions/xep-0160.html#flow if we are dealing with a offline message we will have a delay child
               var incommingMsg = Strophe.getText(message.getElementsByTagName('body')[0]);
               window.app.handleIncommingMessage(incommingMsg, true);
               self.sendAckMessage($(message).attr("from"),
                                    $(message).attr("id"),
                                    $($(message).children("request")[0]).attr("ackDest"));
            } else {
               self.sendAckMessage($(message).attr("from"),
                                    $(message).attr("id"),
                                    $($(message).children("request")[0]).attr("ackDest"));
            }

         }
      } else if ($(message).attr("type") === "error") {
         var error = message.getElementsByTagName("error")[0];
         if (error !== undefined) {
            var type = $(error).attr("type");
            window.app.logDebugOnServer("XMPP error, type [" + type + "]");
         }
      }
      return true;
   };
};

window.app.xmppHandlerInstance = {};
$(function () {
   //the xmpp handler for new messages
   window.app.xmppHandlerInstance = new window.app.XMPPhandler();
   $(window).unload(function () {
      if (window.app.xmppHandlerInstance && window.app.xmppHandlerInstance.disconnect) {
         window.app.xmppHandlerInstance.disconnect();
      }
   });
   $.getJSON(window.app.domainName + '/Xmpp/GetConnectionDetailsForLoggedInUser', function (data) {
      if (data !== "") {
         window.app.selfXmppAddress = data.XmppUser;
         window.app.xmppHandlerInstance.connect(window.app.selfXmppAddress, data.XmppPassword);
         //window.app.xmppHandlerInstance.connect("supportUK@txtfeedback.net", "123456");
         window.app.updateNrOfUnreadConversations(true);
      }
   });

   $(document).bind('updateUnreadConvsNr', function (ev, data) {
      getNumberOfConversationsWithUnreadMessages();
   });
});