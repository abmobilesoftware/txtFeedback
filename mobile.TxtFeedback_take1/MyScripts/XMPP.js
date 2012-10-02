"use strict";
window.app = window.app || {};
window.app.receivedMsgID = 12345;

//#region Xmpp connection variables
window.app.xmppConn = {};
window.app.getFeaturesIQID = "infomobile";
window.app.addressOfPhpScripts = "dragos@txtfeedback.net";
window.app.xmppUserToConnectAs = "testDA_temp2";
window.app.xmppPasswordForUser = "123456";
window.app.selfXmppAddress = window.app.xmppUserToConnectAs + "@txtfeedback.net";
window.app.xmppHandlerInstance = {};
//#endregion

$(function () {
   $(window).unload(function () {
      window.app.saveLoginDetails();
   });
   window.app.loadLoginDetails();   
});

window.app.logOnXmppNetwork = function (register) {
   window.app.xmppHandlerInstance = new window.app.XMPPhandler();
   window.app.msgView.messagesView.getMessages(window.app.defaultConversationID);
   if (register) {
      window.app.xmppHandlerInstance.register();
   } else {
      window.app.xmppHandlerInstance.connect(window.app.xmppUserToConnectAs + "@txtfeedback.net", window.app.xmppPasswordForUser);
   }
}

window.app.handleIncommingMessage = function (msgContent, isIncomming) {      
   window.app.receivedMsgID++;
   //if the message comes from the multiplexer then we will have an encoded message, otherwise it will be a plain message
   if (isIncomming) {
      $(document).trigger('msgReceived', {
         fromID: window.app.defaultTo,
         toID: window.app.defaultFrom,
         convID: window.app.defaultConversationID,
         msgID: window.app.receivedMsgID,
         dateReceived: new Date,
         text: msgContent,
         readStatus: false
      });
   }   
};

window.app.XMPPhandler = function XMPPhandler() {
   this.userid = null;
   this.password = null;
   this.conn = null;

   this.connection = null;
   this.start_time = null;
   this.connectCallback = function (status, condition) {
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
      } else if (status === Strophe.Status.REGIFAIL) {
         window.app.logErrorOnServer("XMPP registration failed");
      } else if (status === Strophe.Status.REGISTER) {
         window.app.logDebugOnServer("XMPP registering with user [" + window.app.xmppUserToConnectAs + "]");
         window.app.xmppConn.conn.register.fields.username = window.app.xmppUserToConnectAs;
         window.app.xmppConn.conn.register.fields.password = window.app.xmppPasswordForUser;
         window.app.xmppConn.conn.register.submit();
      } else if (status === Strophe.Status.REGISTERED) {
         window.app.logDebugOnServer("XMPP Register successful");
         window.app.xmppConn.conn.register.authenticate();
      } else if (status === Strophe.Status.SBMTFAIL) {
         window.app.logDebugOnServer("XMPP registration failed, condition [" +condition + "]");
         //registration failed - check the reason
         if (condition === "conflict") {
            //the user already exists
            window.app.logDebugOnServer("XMPP registration failed: user already exists");
            window.app.xmppHandlerInstance.connect(window.app.xmppUserToConnectAs + "@txtfeedback.net", window.app.xmppPasswordForUser)
         }
      } else if (status === Strophe.Status.CONNECTING) {
         
      } else if (status === Strophe.Status.AUTHENTICATING) {
         
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
      window.app.logDebugOnServer("XMPP connecting with user [" +userid + "]");
      var self = this;
      var xmppServerAddress = "http://176.34.122.48:5280/http-bind/"
      self.conn = new Strophe.Connection(xmppServerAddress);
      self.userid = userid;
      self.password = password;
      window.app.xmppConn = this;
      self.conn.connect(userid, password, self.connectCallback);
   };
   this.register = function () {
      var self = this;
      var xmppServerAddress = "http://176.34.122.48:5280/http-bind/";
      self.conn = new Strophe.Connection(xmppServerAddress);
      window.app.xmppConn = this;
      self.conn.register.connect("txtfeedback.net", self.connectCallback);
   };

   this.disconnect = function () {
      var self = this;
      self.conn.disconnect();
   };
   this.send_ping = function (to) {
      var self = this;
      var ping = $iq({
         to: to,
         type: "get",
         id: "ping1"
      }).c("ping", { xmlns: "urn:xmpp:ping" });
      self.start_time = (new Date()).getTime();
      self.conn.send(ping);
   };
   this.send_initial_presence = function (to) {
      var self = this;
      var presence = $pres().c("status").t("MobileClient");
      self.conn.send(presence);
   };
   this.enableCarbons = function (to) {
      var self = this;
      var enCarbons = $iq({         
         type: "set",
         id: "enableCarbons"         
      }).c("enable", { xmlns: "urn:xmpp:carbons:1" });     
      self.conn.send(enCarbons);
   };
   this.getInfo = function (to) {
      var reqInfo = $iq({
         to: to,
         type: "get",
         id: window.app.getFeaturesIQID
      }).c("query", { xmlns: "http://jabber.org/protocol/disco#info" });
      this.connection.send(reqInfo);
   };
   this.send_reply = function (from, to, dateSent, message, xmppTo) {
      var self = this;
      var message_body = message;
      var replymsg = $msg({
         from: window.app.selfXmppAddress,
         to: xmppTo,
         "type": "chat"
      }).c("body").t(message_body);
      self.conn.send(replymsg);
   };
   this.handle_infoquery = function (iq) {
      var elapsed = (new Date()).getTime() - this.start_time;
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
         var x = $(message).children("body").text();
      } else if ($(message).attr("type") === "chat") {
         //if we are dealing with a forwarded message then the body tag will be non zero
         if ($(message).children("sent").attr("xmlns") === "urn:xmpp:carbons:1") {
            if (message.getElementsByTagName("body") != undefined && message.getElementsByTagName("body").length !== 0) {
               var carbonMsg = Strophe.getText(message.getElementsByTagName('body')[0]);
               window.app.handleIncommingMessage(carbonMsg,false);
            }
         }
         else if ($(message).children("received").attr("xmlns") === "urn:xmpp:carbons:1") {
         }
         else {
            //incommingMSG
            var incommingMsg = Strophe.getText(message.getElementsByTagName('body')[0]);
            window.app.handleIncommingMessage(incommingMsg, true);
         }
      } else {
         //TODO what other messages might there be?
         //TODO log as error
      }
      return true;
   };
};

//#region Store/retrieve login details
window.app.saveLoginDetails = function () {
   var store = new Persist.Store('TxtFeedback');
   store.set('xmppUser', window.app.xmppUserToConnectAs);
   store.set('xmppPassw', window.app.xmppPasswordForUser);
   store.set('conversationID', window.app.defaultConversationID);
}
window.app.loadLoginDetails = function () {
   var store = new Persist.Store('TxtFeedback');
   store.get('xmppUser', function (ok, val) {
      if (ok && val) {
      //if (false) {
         //we found a previous logged in user, so we reuse that on
         //$('h1')[0].textContent = val;
         window.app.xmppUserToConnectAs = val;
         var password = store.get('xmppPassw');
         if (password) { window.app.xmppPasswordForUser = password; }      
         var defaultConversationID = store.get('conversationID');
         if (defaultConversationID) {
            window.app.defaultConversationID = defaultConversationID
            var fromTo = getFromToFromConversation(defaultConversationID);
            window.app.defaultFrom = fromTo[0];
            window.app.defaultTo = fromTo[1];
         }
         window.app.logOnXmppNetwork(false);
      }
      else {
         //no previous user found, create a new one
         $.getJSON(            
            'Home/GetUser',
            { location: "0751569435"},
            function (data) {
               window.app.xmppUserToConnectAs = data.Name;
               window.app.xmppPasswordForUser = data.Password;
               var convID = data.ConversationID;
               window.app.defaultConversationID = convID;
               var fromTo = getFromToFromConversation(convID);
               window.app.defaultFrom = fromTo[0];
               window.app.defaultTo = fromTo[1];               
               window.app.logOnXmppNetwork(true);
            }
         );
      }
   });
}
//#endregion