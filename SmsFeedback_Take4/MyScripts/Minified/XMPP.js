window.app=window.app||{};window.app.xmppConn={};window.app.getFeaturesIQID="info14";window.app.selfXmppAddress="";window.app.tempMsgQueue=[];window.app.receivedMsgID=12345;function getMessage(b){for(var a=0;a<window.app.tempMsgQueue.length;++a){if(window.app.tempMsgQueue[a].msgID==b){return window.app.tempMsgQueue[a]}}return null}function removeMessageById(b){for(var a=0;a<window.app.tempMsgQueue.length;++a){if(window.app.tempMsgQueue[a].msgID==b){window.app.tempMsgQueue.splice(a,1);return true}}return false}function MessageUnsent(d,c,e,b,f,g,i,a,h){this.body=d;this.xmppTo=c;this.msgID=e;this.convID=b;this.from=f;this.dateReceived=g;this.text=i;this.read=a;this.isSmsBased=h}function getNumberOfConversationsWithUnreadMessages(){window.app.updateNrOfUnreadConversations(false)}window.app.setNrOfUnreadConversationOnTab=function(b){window.app.nrOfUnreadConvs=b;var a="("+b+")";$("#msgTabcount").text(a)};window.app.reconnectTimer={};window.app.intervalToWaitBetweenChecks=15000;function reconnectIfRequired(){if(window.app.xmppConn&&window.app.xmppConn.conn&&!window.app.xmppConn.conn.connected&&!window.app.XMPPConnecting){window.app.XMPPConnecting=true;window.app.xmppHandlerInstance.connect(window.app.xmppConn.userid,window.app.xmppConn.password,window.app.xmppConn.connectCallback)}window.app.startReconnectTimer()}window.app.startReconnectTimer=function(){clearTimeout(window.app.reconnectTimer);window.app.reconnectTimer=setTimeout(reconnectIfRequired,window.app.intervalToWaitBetweenChecks)};window.app.xmppHandlerInstance={};$(function(){window.app.xmppHandlerInstance=new window.app.XMPPhandler();$(window).unload(function(){if(window.app.xmppHandlerInstance&&window.app.xmppHandlerInstance.disconnect){window.app.xmppHandlerInstance.disconnect()}});$.getJSON(window.app.domainName+"/Xmpp/GetConnectionDetailsForLoggedInUser",function(a){if(a!==""){window.app.selfXmppAddress=a.XmppUser;window.app.xmppHandlerInstance.connect(window.app.selfXmppAddress,a.XmppPassword);window.app.updateNrOfUnreadConversations(true)}});$(document).bind("updateUnreadConvsNr",function(a,b){getNumberOfConversationsWithUnreadMessages()})});window.app.handleIncommingMessage=function(d,g){window.app.receivedMsgID++;var h;if(window.DOMParser){var f=new DOMParser();h=f.parseFromString(d,"text/xml")}else{h=new ActiveXObject("Microsoft.XMLDOM");h.async=false;h.loadXML(d)}var e=h.getElementsByTagName("msg")[0];if(e!==undefined){$(document).trigger("updateUnreadConvsNr");if(window.app.isInConversationsTab!=undefined&&window.app.isInConversationsTab===true){var k=e.getElementsByTagName("from")[0].textContent;var m=e.getElementsByTagName("to")[0].textContent;var c=cleanupPhoneNumber(m);var a=cleanupPhoneNumber(k);var i;i=window.app.workingPointsSuffixDictionary[c]||window.app.workingPointsSuffixDictionary[a];var b=isWorkingPoint(k,i);var t=e.getElementsByTagName("datesent")[0].textContent;var s=e.getElementsByTagName("sms")[0].textContent;var o=false;if(s==="true"){o=true}var l;if(b&&g){l=buildConversationID(a,c)}else{l=e.getElementsByTagName("convID")[0].textContent}var r=e.getElementsByTagName("body")[0].textContent;var p=false;window.app.receivedMsgID++;var j=new Date(Date.parse(t));var n=60000;var q=window.app.appStartTime.getTimezoneOffset()*n;j=new Date(j.getTime()-q);$(document).trigger("msgReceived",{fromID:a,toID:c,convID:l,msgID:window.app.receivedMsgID,dateReceived:j,text:r,readStatus:p,isSmsBased:o})}}};window.app.XMPPhandler=function XMPPhandler(){var a=this;this.userid=null;this.password=null;this.conn=null;this.connection=null;this.start_time=null;this.connectCallback=function(b){var d=false;if(b===Strophe.Status.CONNECTED){window.app.xmppConn.connection=window.app.xmppConn.conn;window.app.xmppConn.connection.addHandler(window.app.xmppConn.handle_infoquery,null,"iq",null,null);window.app.xmppConn.connection.addHandler(window.app.xmppConn.handle_message,null,"message",null,null);var c=Strophe.getDomainFromJid(window.app.xmppConn.connection.jid);window.app.xmppConn.send_initial_presence(c);window.app.xmppConn.getInfo(c)}else{if(b===Strophe.Status.CONNECTING){}else{if(b===Strophe.Status.AUTHENTICATING){}else{if(b===Strophe.Status.DISCONNECTED){d=true}else{if(b===Strophe.Status.CONNFAIL){window.app.logDebugOnServer("XMPP connection fail");window.app.xmppConn.conn.disconnect()}else{if(b===Strophe.Status.AUTHFAIL){window.app.logDebugOnServer("XMPP authentication failed");window.app.xmppConn.conn.disconnect()}else{if(b===Strophe.Status.ERROR){window.app.logDebugOnServer("XMPP status error");window.app.xmppConn.conn.disconnect()}}}}}}}if(d){window.app.xmppConn.connect(window.app.xmppConn.userid,window.app.xmppConn.password,window.app.xmppConn.connectCallback)}};this.connect=function(c,d){window.app.logDebugOnServer("XMPP connecting with user ["+c+"]");var b=this;var e="http://46.137.26.124:5280/http-bind/";b.conn=new Strophe.Connection(e);b.userid=c;b.password=d;window.app.xmppConn=this;b.conn.connect(c,d,b.connectCallback)};this.disconnect=function(){var b=this;b.conn.sync=true;b.conn.flush();b.conn.disconnect()};this.send_ping=function(c){var b=$iq({to:c,type:"get",id:"ping1"}).c("ping",{xmlns:"urn:xmpp:ping"});this.start_time=(new Date()).getTime();this.connection.send(b)};this.send_initial_presence=function(c){var b=$pres().c("status").t("Ready or not");this.connection.send(b)};this.enableCarbons=function(c){var b=$iq({type:"set",id:"enableCarbons"}).c("enable",{xmlns:"urn:xmpp:carbons:1"});this.connection.send(b)};this.getInfo=function(c){var b=$iq({to:c,type:"get",id:window.app.getFeaturesIQID}).c("query",{xmlns:"http://jabber.org/protocol/disco#info"});this.connection.send(b)};this.sendAckMessage=function(e,b,d){var c=this;var f=$msg({to:e}).c("subject").t("ClientMsgDeliveryReceipt");f.up();f.c("received",{xmlns:"urn:xmpp:receipts",id:b,ackDest:d});window.app.xmppConn.conn.send(f)};this.send_reply=function(i,j,d,b,l,c,k,g,e){var h="<msg> <from>"+i+"</from> <to>"+j+"</to> <datesent>"+d+"</datesent><convID>"+b+"</convID> <body>"+Strophe.xmlescape(l)+"</body> <staff>"+g.toString()+"</staff> <sms>"+k.toString()+"</sms> </msg>";var f=new MessageUnsent(h,c,e,b,i,d,l,false,false);this.send_message(f)};this.send_message=function(c){var b=this;var d=$msg({id:c.msgID,to:c.xmppTo,type:"chat"}).c("body").t(c.body);d.up();d.c("subject").t("internal_packet");d.up();d.c("request",{xmlns:"urn:xmpp:receipts"});if(window.app.xmppConn.conn.authenticated){window.app.tempMsgQueue.push(c);window.app.xmppConn.conn.send(d)}else{if(!window.app.XMPPConnecting){window.app.XMPPConnecting=true;window.app.xmppHandlerInstance.connect(window.app.xmppHandlerInstance.userid,window.app.xmppHandlerInstance.password,b.connectCallback)}}};this.handle_infoquery=function(b){var c=$(b);if(c.attr("id")===window.app.getFeaturesIQID){window.app.xmppConn.enableCarbons(c.attr("from"))}if(c.attr("type")==="result"){}if(c.attr("type")==="error"){window.app.logDebugOnServer("XMPP info query error, ")}return true};this.handle_message=function(h){if($(h).attr("type")==="getConversationsResponse"){window.app.logDebugOnServer("XMPP Conversations list reponse received")}else{if($(h).attr("type")==="sendSmsResponse"){if($(h).children("body").text()==="error"){$("#quick_replay_text").val("")}else{this.displayConversationsList(h);$("#quick_replay_text").val("")}}else{if($(h).attr("type")==="getMessagesForConversationResponse"){var g=$(h).children("body").text();this.displayMessagesForConversation(g)}else{if($(h).children("subject").text()==="ServerMsgDeliveryReceipt"){var e=$(h).children("received").attr("id");$(document).trigger("serverAcknowledge",{message:getMessage(e)})}else{if($(h).children("subject").text()==="ClientMsgDeliveryReceipt"){var e=$(h).children("received").attr("id");if($(h).children("payload").length==1){var i=$(h).children("payload").text();$(document).trigger("smsPayloadReceived",{content:i,messageId:e})}else{$(document).trigger("clientAcknowledge",{message:getMessage(e)})}}else{if($(h).attr("type")==="result"){}else{if($(h).attr("type")==="chat"){if($(h).children("sent").attr("xmlns")==="urn:xmpp:carbons:1"){if(h.getElementsByTagName("body")!==undefined&&h.getElementsByTagName("body").length!==0){var b=Strophe.getText(h.getElementsByTagName("body")[0]);if(window.app.handleIncommingMessage!==undefined){window.app.handleIncommingMessage(b,false)}}}else{if($(h).children("delay").length===0){var d=Strophe.getText(h.getElementsByTagName("body")[0]);window.app.handleIncommingMessage(d,true);a.sendAckMessage($(h).attr("from"),$(h).attr("id"),$($(h).children("request")[0]).attr("ackDest"))}else{a.sendAckMessage($(h).attr("from"),$(h).attr("id"),$($(h).children("request")[0]).attr("ackDest"))}}}else{if($(h).attr("type")==="error"){var c=h.getElementsByTagName("error")[0];if(c!==undefined){var f=$(c).attr("type");window.app.logDebugOnServer("XMPP error, type ["+f+"]")}}}}}}}}}return true}};