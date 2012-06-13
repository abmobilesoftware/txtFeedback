function CreateXMPPHandler(conversationsView, messagesView) {
    var convView = conversationsView;
    var msgView = messagesView;


    var msgID = 12345;

    var XMPPhandler = {
        account_number: "+442033221134",
        sender_number: null,
        receiver_number: null,

        office_number: null, // folosit pentru afisarea mesajelor pentru o anumita conversatie. Care e office number-ul?
        connection: null,
        connect: function (userid, password) {
            var self = this;
            var xmppServerAddress = "http://176.34.122.48:7070/http-bind/";
            //Strophe.log = function (level, msg) {
            //    console.log(msg);
            //};
            var conn = new Strophe.Connection(xmppServerAddress);
            conn.connect(userid, password, function (status) {
                if (status === Strophe.Status.CONNECTED) {
                    showStatus("XMPP connected", 5000);
                    console.log("CONNECTED");
                    self.connection = conn;
                    self.connection.addHandler(self.handle_infoquery, null, "iq", null, "ping1");
                    self.connection.addHandler(self.handle_message, null, "message", null, null);
                    var domain = Strophe.getDomainFromJid(self.connection.jid);
                    self.send_ping(domain);
                    self.send_initial_presence(domain);
                    //self.request_conversations(self.account_number);                   
                } else if (status === Strophe.Status.CONNECTING) {
                    showStatus("XMPP CONNECTING" , 5000);
                    console.log("CONNECTING");
                } else if (status === Strophe.Status.AUTHENTICATING) {
                    showStatus("XMPP AUTHENTICATING", 5000);                    
                    console.log("AUTHENTICATING");
                } else if (status === Strophe.Status.DISCONNECTED) {
                    showStatus("XMPP DISCONNECTED", 5000);                    
                    console.log("DISCONNECTED");
                } else if (status === Strophe.Status.CONNFAIL) {
                    showStatus("XMPP CONNFAILED", 5000);                    
                    console.log("CONNFAILED");
                } else if (status === Strophe.Status.AUTHFAIL) {
                    showStatus("XMPP AUTHFAIL", 5000);                    
                    console.log("AUTHFAIL");
                } else if (status === Strophe.Status.ERROR) {
                    showStatus("XMPP ERROR", 5000);                    
                    console.log("ERROR");
                }
            });
        },
        start_time: null,
        log: function (msg) {
            console.log(msg);
        },
        send_ping: function (to) {
            var ping = $iq({
                to: to,
                type: "get",
                id: "ping1"
            }).c("ping", { xmlns: "urn:xmpp:ping" });

            this.log("Sending ping to " + to + ".");           
            this.start_time = (new Date()).getTime();
            this.connection.send(ping);
        },
        send_initial_presence: function (to) {
            var presence = $pres().c("status").t("Ready or not");
            this.connection.send(presence);
            this.log("Initial presence sent");
        },
        send_reply: function (from, to, message) {
            var message_body = Strophe.xmlescape("<sms>" +
                                        " <sms_from>" + from + "</sms_from>" +
                                        " <sms_to>" + to + "</sms_to>" +
                                        " <sms_body>" + message + "</sms_body>" +
                                    " </sms>");
            var message = $msg({
                to: "logic.smsfeedback.com",
                from: "smsapp@smsfeedback.com",
                "type": "sendSmsRequest"
            }).c("body").t(message_body);
            this.connection.send(message);
            this.log("Replay sent");
        }, 
        handle_infoquery: function (iq) {
            var elapsed = (new Date()).getTime() - this.start_time;
            console.log("Received pong from server in " + elapsed + "ms");
            //Hello.connection.disconnect();
            return false;
        }, 
        handle_message: function (message) {
            console.log(message);
            if ($(message).attr("type") == "getConversationsResponse") {
                //this.displayConversationsList(message);
                this.log("Conversations list reponse received");
            } else if ($(message).attr("type") == "sendSmsResponse") {
                if ($(message).children("body").text() == "error") {
                    this.log("SMS send failed!!!");
                    $("#quick_replay_text").val("");
                } else {
                    this.displayConversationsList(message);
                    $("#quick_replay_text").val("");
                }
            } else if ($(message).attr("type") == "getMessagesForConversationResponse") {
                var messages = $(message).children("body").text();
                this.displayMessagesForConversation(messages);
                this.log("Messages for conversation retrieved!");
            } else {

                var msgContent = (Strophe.getText(message.getElementsByTagName('body')[0]))
                var xmlDoc;
                if (window.DOMParser) {
                    parser = new DOMParser();
                    xmlDoc = parser.parseFromString(msgContent, "text/xml");
                }
                else {
                    xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
                   xmlDoc.async = false;
                   xmlDoc.loadXML(msgContent);
                }                  
                var xmlMsgToBeDecoded = xmlDoc.getElementsByTagName("msg")[0];
                var fromID = xmlMsgToBeDecoded.getElementsByTagName('from')[0].textContent;
                var toID = xmlMsgToBeDecoded.getElementsByTagName('to')[0].textContent;
                var dateReceived = xmlMsgToBeDecoded.getElementsByTagName('datesent')[0].textContent;
                var convID = fromID + "-" + toID;
                var newText = xmlMsgToBeDecoded.getElementsByTagName("body")[0].textContent
                var newTrimmedText = newText.substring(0, 40);
                           
                //this should not be here - it is just a temporary hack
                //decide if the user has read this :)
                var readStatus = false;
                if (messagesView.currentConversationId == convID) {
                    //the correct conversation was in focus, so we have read the message
                    readStatus = true;
                }
                $(document).trigger('msgReceived', { fromID: fromID, toID: toID, convID: convID, msgID: msgID, dateReceived: dateReceived, text: newText, readStatus: readStatus });
              //  newMessageReceived(convView, msgView, fromID, toID, convID, msgID, dateReceived, newText, readStatus);
                msgID++;
                               
            }

            return true;
        }
        
    };   
    return XMPPhandler;
};