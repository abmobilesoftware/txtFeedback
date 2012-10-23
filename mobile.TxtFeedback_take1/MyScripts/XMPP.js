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
//#endregion

window.app = window.app || {};
window.app.receivedMsgID = 12345;

//#region XMPP connection variables
window.app.xmppConn = {};
//#region XMPP constants
window.app.getFeaturesIQID = "infomobile";
window.app.xmppServerExtension = "@txtfeedback.net";
window.app.xmppComponentExtension = "@moderator.txtfeedback.net";
//#endregion

window.app.messageModeratorAddress = '';
window.app.suffixedMessageModeratorAddress = '';
window.app.xmppUserToConnectAs = '';
window.app.xmppPasswordForUser = '';
window.app.xmppSuffixedUserToConnectAs = '';

window.app.xmppHandlerInstance = {};
window.app.separatorForUnsentMessages = "@@";
window.app.unsentMsgQueue = [];
window.app.unsentMsgXmppToQueue = [];
window.app.XMPPConnecting = false;
//#endregion

//#region "timer for reconnect"
window.app.reconnectTimer;
window.app.intervalToWaitBetweenChecks = 15000;
window.app.startReconnectTimer = function() {
   clearTimeout(window.app.reconnectTimer);
   window.app.reconnectTimer = setTimeout(reconnectIfRequired, window.app.intervalToWaitBetweenChecks);
}
function reconnectIfRequired() {
   if (window.app.xmppConn && window.app.xmppConn.conn && !window.app.xmppConn.conn.connected && !window.app.XMPPConnecting) {
      window.app.xmppHandlerInstance.connect(window.app.xmppSuffixedUserToConnectAs, window.app.xmppPasswordForUser, window.app.xmppHandlerInstance.connectCallback)
   }
   window.app.startReconnectTimer();
}
//#endregion
window.app.defaultConversationID = '';
window.app.defaultFrom = '';
window.app.defaultTo = '';
window.app.defaultMessage = '';

window.app.initializeBasedOnLocation = function () {
   
   //based on the address we should have received from the server some configuration data, so now we initialize those variables based on that data
   window.app.messageModeratorAddress = $("#componentLocation").text();
   if (window.app.messageModeratorAddress === '') {
      return false;
   }
   window.app.suffixedMessageModeratorAddress = window.app.messageModeratorAddress + window.app.xmppComponentExtension;
   window.app.welcomeMessage = $("#welcomeMessage").text();
   return true;
};

window.app.initializeBasedOnConnectionDetails = function (user, password, convID) {
   window.app.xmppUserToConnectAs = user;
   window.app.xmppPasswordForUser = password;
   window.app.xmppSuffixedUserToConnectAs = window.app.xmppUserToConnectAs + window.app.xmppServerExtension;
   window.app.defaultConversationID = convID;
   var fromTo = getFromToFromConversation(convID);
   window.app.defaultFrom = fromTo[0];
   window.app.defaultTo = fromTo[1];
};

window.app.logOnXmppNetwork = function (register) {
   //console.log("log on xmpp");
   window.app.xmppHandlerInstance = new window.app.XMPPhandler();
   window.app.msgView.messagesView.getMessages(window.app.defaultConversationID);
   if (register) {
      window.app.xmppHandlerInstance.register();
   } else {
      window.app.xmppHandlerInstance.connect(window.app.xmppSuffixedUserToConnectAs, window.app.xmppPasswordForUser);
   }
};

window.app.handleIncommingMessage = function (msgContent, isIncomming) {      
   window.app.receivedMsgID++;
   //if the message comes from the multiplexer then we will have an encoded message, otherwise it will be a plain message
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
   var convID;
   convID = buildConversationID(toID, fromID);   
   var newText = xmlMsgToBeDecoded.getElementsByTagName("body")[0].textContent;
   if (isIncomming) {
      $(document).trigger('msgReceived', {
         fromID: fromID,
         toID: toID,
         convID: convID,
         msgID: window.app.receivedMsgID,
         dateReceived: new Date(),
         text: newText,
         readStatus: false
      });
   }   
};

window.app.XMPPhandler = function XMPPhandler() {
   "use strict";
   this.userid = null;
   this.password = null;
   this.conn = null;

   this.connection = null;
   this.start_time = null;
   this.connectCallback = function (status, condition) {
      var needReconnect = false;
      if (status === Strophe.Status.CONNECTED) {
         window.app.XMPPConnecting = false;
         window.app.xmppConn.conn.addHandler(window.app.xmppConn.handle_infoquery, null, "iq", null, null);
         window.app.xmppConn.conn.addHandler(window.app.xmppConn.handle_message, null, "message", null, null);
         var domain = Strophe.getDomainFromJid(window.app.xmppConn.conn.jid);         
         window.app.xmppConn.send_initial_presence(domain);
         //DA after we are connected sent any potentially unsent messages
         window.app.xmppConn.sendMessagesInQueue();         
      } else if (status === Strophe.Status.REGIFAIL) {
         window.app.logErrorOnServer("XMPP registration failed");
      } else if (status === Strophe.Status.REGISTER) {
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
            window.app.logErrorOnServer("XMPP registration failed: user already exists");
            window.app.xmppHandlerInstance.connect(window.app.xmppSuffixedUserToConnectAs, window.app.xmppPasswordForUser);
         }
      } else if (status === Strophe.Status.CONNECTING) {
         
      } else if (status === Strophe.Status.AUTHENTICATING) {
         
      } else if (status === Strophe.Status.DISCONNECTED) {
         needReconnect = true;
      } else if (status === Strophe.Status.CONNFAIL) {         
         window.app.logErrorOnServer("XMPP connection fail");
         window.app.xmppConn.conn.disconnect();
      } else if (status === Strophe.Status.AUTHFAIL) {
         window.app.logErrorOnServer("XMPP authentication failed");
         window.app.xmppConn.conn.disconnect();
      } else if (status === Strophe.Status.ERROR) {
         window.app.logErrorOnServer("XMPP status error");
         window.app.xmppConn.conn.disconnect();
      }
      if (needReconnect) {
         window.app.xmppConn.connect(window.app.xmppConn.userid, window.app.xmppConn.password, window.app.xmppConn.connectCallback);
      }
   };
   this.sendMessagesInQueue = function () {
      var self = this;
      while (window.app.unsentMsgQueue.length > 0) {
         var msg = window.app.unsentMsgQueue.shift();
         var xmppTo = window.app.unsentMsgXmppToQueue.shift();
         window.app.xmppConn.send_message(msg, xmppTo);
      }
   }
   this.connect = function (userid, password) {
      //window.app.logDebugOnServer("XMPP connecting with user [" + userid + "]");
      var self = this;
      var xmppServerAddress = "http://176.34.122.48:5280/http-bind/";
      if (!self.conn || !window.app.xmppConn) {
         self.conn = new Strophe.Connection(xmppServerAddress);
         self.userid = userid;
         self.password = password;
         window.app.xmppConn = this;
      }
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
      //DA based on http://stackoverflow.com/questions/5198410/disconnect-strophe-connection-on-page-unload
      self.conn.sync = true; // Switch to using synchronous requests since this is typically called onUnload.
      self.conn.flush();
      self.conn.disconnect();
   };
   this.getUsername = function () {
       var self = this;
       return self.userid;
   };
   this.getPassword = function () {
       var self = this;
       return self.password;
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
      //console.log("presence sent!");
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
   this.send_message = function (body, xmppTo) {
      var self = this;
      //the message was already formatted
      var replymsg = $msg({
         from: window.app.xmppSuffixedUserToConnectAs,
         to: xmppTo,
         "type": "chat"
      }).c("body").t(body);
      replymsg.up();
      replymsg.c("subject").t("internal_packet");   
      //DA - when sending make sure that we are connected
      if (window.app.xmppConn.conn.connected) {
         window.app.xmppConn.conn.send(replymsg);
      }
      else {
         window.app.unsentMsgQueue.push(replymsg);
         window.app.unsentMsgXmppToQueue.push(xmppTo);
         //force synch connect 
         if (!window.app.XMPPConnecting) {
            window.app.XMPPConnecting = true;
            window.app.xmppHandlerInstance.connect(window.app.xmppSuffixedUserToConnectAs, window.app.xmppPasswordForUser, self.connectCallback)
         }
      }
   },
   this.send_reply = function (from, to, dateSent, convID, message, xmppTo) {
      var self = this;
      var message_body = "<msg>" +
                                     " <from>" + window.app.xmppSuffixedUserToConnectAs + "</from>" +
                                     " <to>" + window.app.suffixedMessageModeratorAddress + "</to>" +
                                     " <datesent>" + dateSent + "</datesent>" +
                                      "<convID>" + convID +"</convID>" +
                                     " <body>" + message + "</body>" +
                                     " <staff>true</staff>" +
                                     " <sms>false</sms>" +
                                     " </msg>";
      //console.log(message_body);      
      self.send_message(message_body, xmppTo);
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
         //var x = $(message).children("body").text();

      } else if ($(message).attr("type") === "chat") {
         //if we are dealing with a forwarded message then the body tag will be non zero
         if ($(message).children("sent").attr("xmlns") === "urn:xmpp:carbons:1") {
            if (message.getElementsByTagName("body") !== undefined && message.getElementsByTagName("body").length !== 0) {
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
      } else if ($(message).attr("type") === "error") {
         var error = message.getElementsByTagName("error")[0];
         if (error !== undefined) {
            var type = $(error).attr("type");
            window.app.logDebugOnServer("XMPP error, type [" + type+ "]");
         }
         
         //TODO what other messages might there be?
         //TODO log as error
      }
      return true;
   };
};

window.app.disconnectXMPP = function () {
   if (window.app.xmppHandlerInstance && window.app.xmppHandlerInstance.disconnect) {
      window.app.xmppHandlerInstance.disconnect();
   }
};
//#region Store/retrieve login details
window.app.saveLoginDetails = function () {
   var store = new Persist.Store('TxtFeedback');
   store.set('xmppUser', window.app.xmppUserToConnectAs);
   store.set('xmppPassw', window.app.xmppPasswordForUser);
   //save any unsent messages   
   var unsentMsgs = window.app.unsentMsgQueue.join(window.app.separatorForUnsentMessages);
   store.set('unsentMessages', unsentMsgs);
   var unsentMsgsXmppTo = window.app.unsentMsgXmppToQueue.join(window.app.separatorForUnsentMessages);
   store.set('unsentMessagesXmppTo', unsentMsgsXmppTo);
};

window.app.loadLoginDetails = function () {
   //console.log("load login details");     
   var store = new Persist.Store('TxtFeedback');
   var unsentMsgs = store.get('unsentMessages');
   if (unsentMsgs) {
      window.app.unsentMsgQueue = unsentMsgs.split(window.app.separatorForUnsentMessages);
   }
   var unsentMsgsXmppTo = store.get('unsentMessagesXmppTo');
   if (unsentMsgsXmppTo) {
      window.app.unsentMsgXmppToQueue = unsentMsgsXmppTo.split(window.app.separatorForUnsentMessages);
   }
   //sanity check
   if(window.app.unsentMsgQueue.length !== window.app.unsentMsgXmppToQueue.length) {
      window.app.unsentMsgQueue = [];
      window.app.unsentMsgXmppToQueue = [];
   }
   var user = store.get('xmppUser');   
   if (user !== undefined && user) {
      //if (false) {
      //we found a previous logged in user, so we reuse that on         
      //console.log("reuse existing user");         
      var password = store.get('xmppPassw');
      //we cannot store/retrieve the conversation ID as it changes from store to store
      var defaultConversationID = buildConversationID(user, window.app.messageModeratorAddress);
      window.app.initializeBasedOnConnectionDetails(user, password, defaultConversationID);
      window.app.logOnXmppNetwork(false);
   }
   else {
      //console.log("new user");
      //no previous user found, create a new one
      $.getJSON(
         'Home/GetUser',
         { location: window.app.messageModeratorAddress },
         function (data) {
            //console.log("create new user");                              
            window.app.initializeBasedOnConnectionDetails(data.Name, data.Password, data.ConversationID);
            window.app.logOnXmppNetwork(true);
         }
      );
   }   
};
//#endregion