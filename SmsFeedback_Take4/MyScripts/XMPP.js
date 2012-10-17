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
//#endregion
window.app = window.app || {};
window.app.xmppConn = {};
window.app.receivedMsgID = 12345;
window.app.getFeaturesIQID = "info14";
window.app.suffixedMessageModeratorAddress = "dragos@moderator.txtfeedback.net";
window.app.selfXmppAddress = "";

function signalMsgReceivedAtServer(fromID, toId, convID, msgID, dateReceived, text, readStatus) {
   "use strict";
   window.app.updateNrOfUnreadConversations(false);
}

window.app.setNrOfUnreadConversationOnTab = function (unreadConvs) {
   "use strict";
   window.app.nrOfUnreadConvs = unreadConvs;
   var toShow = "(" + unreadConvs + ")";
   $("#msgTabcount").text(toShow);
};

window.app.xmppHandlerInstance = {};
$(function () {
   //the xmpp handler for new messages
   window.app.xmppHandlerInstance = new window.app.XMPPhandler();
   $.getJSON(window.app.domainName + '/Xmpp/GetConnectionDetailsForLoggedInUser', function (data) {
      if (data !== "") {
         window.app.selfXmppAddress = data.XmppUser;
         window.app.xmppHandlerInstance.connect(window.app.selfXmppAddress, data.XmppPassword);
         //window.app.xmppHandlerInstance.connect("supportUK@txtfeedback.net", "123456");
         window.app.updateNrOfUnreadConversations(true);
      }
   });

   $(document).bind('msgReceived', function (ev, data) {
      if (data.messageIsSent === undefined || (data.messageIsSent !== undefined && !data.messageIsSent)) {
         signalMsgReceivedAtServer(data.fromID, data.toID, data.convID, data.msgID, data.dateReceived, data.text, false);
      }
   });
});

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
   var rawFromID = xmlMsgToBeDecoded.getElementsByTagName('from')[0].textContent;
   var rawToID = xmlMsgToBeDecoded.getElementsByTagName('to')[0].textContent;
   var toID = cleanupPhoneNumber(rawToID);
   var fromID = cleanupPhoneNumber(rawFromID);
   var extension;   
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

   if (!isIncomming && !isFromWorkingPoint) {
      //carbon
      var swapTmp = fromID;
      fromID = toID;
      toID = swapTmp;      
   }
   else {
      //regular message
   }
   
   var newText = xmlMsgToBeDecoded.getElementsByTagName("body")[0].textContent;
   
   var readStatus = false;

   window.app.receivedMsgID++;
   $(document).trigger('msgReceived', {
      fromID: fromID,
      toID: toID,
      convID: convID,
      msgID: window.app.receivedMsgID,
      dateReceived: dateReceived,
      text: newText,
      readStatus: readStatus,
      isSmsBased: isSmsBased
   });
   
};

window.app.XMPPhandler = function XMPPhandler() {
   this.userid = null;
   this.password = null;
   this.conn = null;

   this.connection = null;
   this.start_time = null;
   this.connectCallback = function (status) {
      var needReconnect = false;
      if (status === Strophe.Status.CONNECTED) {
         window.app.logDebugOnServer("XMPP connected");         
         window.app.xmppConn.connection = window.app.xmppConn.conn;
         window.app.xmppConn.connection.addHandler(window.app.xmppConn.handle_infoquery, null, "iq", null,null);
         window.app.xmppConn.connection.addHandler(window.app.xmppConn.handle_message, null, "message", null, null);
         var domain = Strophe.getDomainFromJid(window.app.xmppConn.connection.jid);
         //window.app.xmppConn.send_ping(domain);
         window.app.xmppConn.send_initial_presence(domain);
         window.app.xmppConn.getInfo(domain);
       
         //self.request_conversations(self.account_number);
      } else if (status === Strophe.Status.CONNECTING) {
         window.app.logDebugOnServer("XMPP connecting...");         
      } else if (status === Strophe.Status.AUTHENTICATING) {
         window.app.logDebugOnServer("XMPP authenticating...");         
      } else if (status === Strophe.Status.DISCONNECTED) {
         window.app.logDebugOnServer("XMPP disconnected");        
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
      var xmppServerAddress = "http://176.34.122.48:5280/http-bind/";
      self.conn = new Strophe.Connection(xmppServerAddress);
      self.userid = userid;
      self.password = password;
      window.app.xmppConn = this;

      self.conn.connect(userid, password, self.connectCallback);
   };
   this.disconnect = function () {
      var self = this;
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
   this.send_reply = function (from, to, dateSent, convID, message, xmppTo, isSmsBased, toStaff) {
      /*
      here we should just build the message from what is handed to us
      there should be no responsibility regarding the logic of building the content of the message
      but only regarding the structure (what fields have to be filled in)
      */
      var message_body = "<msg>" +
                                    " <from>" + from +  "</from>" +
                                    " <to>" + to + "</to>" +
                                    " <datesent>" + dateSent + "</datesent>" +
                                     "<convID>" + convID + "</convID>" +
                                    " <body>" + message + "</body>" +
                                    " <staff>" + toStaff.toString() + "</staff>" +
                                    " <sms>" + isSmsBased.toString() +"</sms>" +
                                " </msg>";
      var replymsg = $msg({
         from: window.app.selfXmppAddress,
         to: xmppTo,
         "type": "chat"
      }).c("body").t(message_body);
      replymsg.up();
      replymsg.c("subject").t("internal_packet");
      window.app.xmppConn.conn.send(replymsg);
   };
   this.handle_infoquery = function (iq) {      
      var currentIq = $(iq);
      if (currentIq.attr("id") === window.app.getFeaturesIQID)
      {
         window.app.xmppConn.enableCarbons(currentIq.attr("from"));
      }
      if (currentIq.attr("type") === "result")
      {         
         //TODO establish if relevant
      }
      if (currentIq.attr("type") === "error")
      {
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
      } else if ($(message).attr("type") === "result") {
         //TODO result relevant to us?
      } else if ($(message).attr("type") === "chat") {
         //if we are dealing with a forwarded message then the body tag will be non zero
         if ($(message).children("sent").attr("xmlns") === "urn:xmpp:carbons:1") {
            if (message.getElementsByTagName("body") !== undefined && message.getElementsByTagName("body").length !== 0) {
               var carbonMsg = Strophe.getText(message.getElementsByTagName('body')[0]);
               window.app.handleIncommingMessage(carbonMsg,false);
            }
         }
         else {
            //incommingMSG
            var incommingMsg = Strophe.getText(message.getElementsByTagName('body')[0]);
            window.app.handleIncommingMessage(incommingMsg,true);
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