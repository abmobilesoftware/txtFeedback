"use strict";
window.app = window.app || {};
window.app.xmppConn = {};
window.app.receivedMsgID = 12345;

function signalMsgReceivedAtServer(fromID, toId, convID, msgID, dateReceived, text, readStatus) {   
   app.updateNrOfUnreadConversations(false); 
}

window.app.setNrOfUnreadConversationOnTab = function (unreadConvs) {
   app.nrOfUnreadConvs = unreadConvs;
   var toShow = "(" + unreadConvs + ")";
   $("#msgTabcount").text(toShow);
};

window.app.xmppHandlerInstance = {};
$(function () {
   //the xmpp handler for new messages
   app.xmppHandlerInstance = new app.XMPPhandler();
   $.getJSON(app.domainName + '/Xmpp/GetConnectionDetailsForLoggedInUser', function (data) {
      if (data !== "") {
         app.xmppHandlerInstance.connect(data.XmppUser, data.XmppPassword);
         app.updateNrOfUnreadConversations(true);
      }
   });

   $(document).bind('msgReceived', function (ev, data) {
      if (data.messageIsSent === undefined || (data.messageIsSent !== undefined && !data.messageIsSent)) {
         signalMsgReceivedAtServer(data.fromID, data.toID, data.convID, data.msgID, data.dateReceived, data.text, false);
      }
   });
});


window.app.XMPPhandler = function XMPPhandler() {
      this.userid = null;
      this.password = null;      
      this.conn = null;
    
      this.connection = null;
      this.start_time= null;
      this.connectCallback = function (status) {
            var needReconnect = false;
         if (status === Strophe.Status.CONNECTED) {
            window.app.logDebugOnServer("XMPP connected");
            app.xmppConn.connection = app.xmppConn.conn;
            app.xmppConn.connection.addHandler(app.xmppConn.handle_infoquery, null, "iq", null, "ping1");
            app.xmppConn.connection.addHandler(app.xmppConn.handle_message, null, "message", null, null);
            var domain = Strophe.getDomainFromJid(app.xmppConn.connection.jid);
            app.xmppConn.send_ping(domain);
            app.xmppConn.send_initial_presence(domain);
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
            self.conn.disconnect();
         } else if (status === Strophe.Status.AUTHFAIL) {            
            window.app.logDebugOnServer("XMPP authentication failed");
            self.conn.disconnect();
         } else if (status === Strophe.Status.ERROR) {            
            window.app.logDebugOnServer("XMPP status error");
            self.conn.disconnect();
         }
            if (needReconnect) {
              app.xmppConn.connect(app.xmppConn.userid, app.xmppConn.password, app.xmppConn.connectCallback);
         }
      };
      this.connect = function (userid, password) {
         var self = this;
         //var xmppServerAddress = "http://localhost:3333/app/dsadsa/http-bindours/";
          //var xmppServerAddress = "http://www.cluj-info.com/smsfeedback/nocontroller/http-bindours/";
         var xmppServerAddress = "http://176.34.122.48:7070/http-bind/";
         self.conn = new Strophe.Connection(xmppServerAddress);
         self.userid = userid;
         self.password = password;
         window.app.xmppConn = this;

         self.conn.connect(userid, password, self.connectCallback);
      };
      this.disconnect= function () {
         var self = this;
         self.conn.disconnect();
      };     
      this.send_ping= function (to) {
         var ping = $iq({
            to: to,
            type: "get",
            id: "ping1"
         }).c("ping", { xmlns: "urn:xmpp:ping" });
         
         this.start_time = (new Date()).getTime();
         this.connection.send(ping);
      };
      this.send_initial_presence= function (to) {
         var presence = $pres().c("status").t("Ready or not");
         this.connection.send(presence);
      };

      this.send_reply= function (from, to, message) {
         var message_body = Strophe.xmlescape("<sms>" +
                                        " <sms_from>" + from + "</sms_from>" +
                                        " <sms_to>" + to + "</sms_to>" +
                                        " <sms_body>" + message + "</sms_body>" +
                                    " </sms>");
         var replymsg = $msg({
            to: "logic.smsfeedback.com",
            from: "smsapp@smsfeedback.com",
            "type": "sendSmsRequest"
         }).c("body").t(message_body);
         this.connection.send(replymsg);         
      };
      this.handle_infoquery = function (iq) {
         var elapsed = (new Date()).getTime() - this.start_time;
         var pong = $iq({
            to: $(iq).attr("from"),
            from: $(iq).attr("to"),
            type: "result",
            id: $(iq).attr("id")
         });

         if ($(iq).children("ping") != null) {
            app.xmppConn.connection.send(pong);
         }
         //Hello.connection.disconnect();
         return false;
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
         } else {

            var msgContent = (Strophe.getText(message.getElementsByTagName('body')[0]));
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
            var fromID = xmlMsgToBeDecoded.getElementsByTagName('from')[0].textContent;
            fromID = cleanupPhoneNumber(fromID);
            var toID = xmlMsgToBeDecoded.getElementsByTagName('to')[0].textContent;
            toID = cleanupPhoneNumber(toID);
            var dateReceived = xmlMsgToBeDecoded.getElementsByTagName('datesent')[0].textContent;
            var convID = buildConversationID(fromID, toID);
            var newText = xmlMsgToBeDecoded.getElementsByTagName("body")[0].textContent;            
            
            $(document).trigger('msgReceived', {
               fromID: fromID,
               toID: toID,
               convID: convID,
               msgID: app.receivedMsgID,
               dateReceived: dateReceived,
               text: newText,
               readStatus: false
            });
            app.receivedMsgID++;
         }
         return true;
      }
   }