"use strict";
window.app = window.app || {};
window.app.xmppConn = {};
window.app.receivedMsgID = 12345;
   window.app.XMPPhandler = function XMPPhandler() {
      this.userid = null;
      this.password = null;      
      this.conn = null;
    
      this.connection = null;
      this.start_time= null;
      this.connectCallback = function (status) {
            var needReconnect = false;
         if (status === Strophe.Status.CONNECTED) {
            showStatus("XMPP connected", 5000);
            console.log("CONNECTED");
            app.xmppConn.connection = app.xmppConn.conn;
            app.xmppConn.connection.addHandler(app.xmppConn.handle_infoquery, null, "iq", null, "ping1");
            app.xmppConn.connection.addHandler(app.xmppConn.handle_message, null, "message", null, null);
            var domain = Strophe.getDomainFromJid(app.xmppConn.connection.jid);
            app.xmppConn.send_ping(domain);
            app.xmppConn.send_initial_presence(domain);
            //self.request_conversations(self.account_number);
         } else if (status === Strophe.Status.CONNECTING) {
            showStatus("XMPP CONNECTING", 5000);
            console.log("CONNECTING");
         } else if (status === Strophe.Status.AUTHENTICATING) {
            showStatus("XMPP AUTHENTICATING", 5000);
            console.log("AUTHENTICATING");
         } else if (status === Strophe.Status.DISCONNECTED) {
            showStatus("XMPP DISCONNECTED", 5000);
            console.log("DISCONNECTED");
                needReconnect = true;
         } else if (status === Strophe.Status.CONNFAIL) {
            showStatus("XMPP CONNFAILED", 5000);
            console.log("CONNFAILED");
                needReconnect = true;
         } else if (status === Strophe.Status.AUTHFAIL) {
            showStatus("XMPP AUTHFAIL", 5000);
            console.log("AUTHFAIL");
                needReconnect = true;
         } else if (status === Strophe.Status.ERROR) {
            showStatus("XMPP ERROR", 5000);
            console.log("ERROR");
            needReconnect = true;
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
      this.log = function (msg) {
         console.log(msg);
      };
      this.send_ping= function (to) {
         var ping = $iq({
            to: to,
            type: "get",
            id: "ping1"
         }).c("ping", { xmlns: "urn:xmpp:ping" });

         this.log("Sending ping to " + to + ".");
         this.start_time = (new Date()).getTime();
         this.connection.send(ping);
      };
      this.send_initial_presence= function (to) {
         var presence = $pres().c("status").t("Ready or not");
         this.connection.send(presence);

         this.log("Initial presence sent");
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
         this.log("Reply sent");
      };
      this.handle_infoquery = function (iq) {
         var elapsed = (new Date()).getTime() - this.start_time;
         console.log("Received pong from server in " + elapsed + "ms");
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
         console.log(message);
         if ($(message).attr("type") === "getConversationsResponse") {
            //this.displayConversationsList(message);
            this.log("Conversations list reponse received");
         } else if ($(message).attr("type") === "sendSmsResponse") {
            if ($(message).children("body").text() === "error") {
               this.log("SMS send failed!!!");
               $("#quick_replay_text").val("");
            } else {
               this.displayConversationsList(message);
               $("#quick_replay_text").val("");
            }
         } else if ($(message).attr("type") === "getMessagesForConversationResponse") {
            var messages = $(message).children("body").text();
            this.displayMessagesForConversation(messages);
            this.log("Messages for conversation retrieved!");
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