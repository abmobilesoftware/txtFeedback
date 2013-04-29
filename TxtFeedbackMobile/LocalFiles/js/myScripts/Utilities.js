function cleanupPhoneNumber(data) {
   var prefixes = new Array("00", "\\+");
   //remove 00 and + from the beginning of the number
   //remove all domain qualifiers - everything after @
   var reg = new RegExp('^(' + prefixes.join('|') + ')|@.+$', "g");
   data = data.replace(reg, "");
   return data;
}

var cConversationIdNumbersSeparator = '-';
function getFromToFromConversation(convID) {
   var fromToArray = convID.split(cConversationIdNumbersSeparator);
   return fromToArray;
}

function extractUser(xmppAddress) {
	return xmppAddress.substring(0, xmppAddress.indexOf("@"));
}

/* XMPP Utilities */
function formatXmppMsgBody(from, to, dateSent, convId, message, toStaff, isSmsBased) {
	var messageBody = 
		"<msg>" +
    		" <from>" + from + "</from>" +
    		" <to>" + to + "</to>" +
    		" <datesent>" + dateSent + "</datesent>" +
    		"<convID>" + convId + "</convID>" +
    		" <body>" + Strophe.xmlescape(message) + "</body>" +
    		" <staff>" + toStaff.toString() + "</staff>" +
    		" <sms>" + isSmsBased.toString() + "</sms>" +
    	" </msg>";
	return messageBody;
}

function formatXmppMessage(msgId, xmppTo, body) {
	var message = $msg({
		id: msgId,
		to: xmppTo,
		type: "chat"
	}).c("body").t(body);
	message.up();
	message.c("subject").t("internal_packet");
	message.up();
	message.c("request", { xmlns: "urn:xmpp:receipts" });
    return message;
}

function formatAckowledgeMessage(xmppTo, acknowledgeId, 
		acknowledgeDestination) {
	var acknowledgeMessage = $msg({
		to: xmppTo
	}).c("subject").t("ClientMsgDeliveryReceipt");
	acknowledgeMessage.up();
	acknowledgeMessage.c("received", { 
		xmlns: "urn:xmpp:receipts", 
		id: acknowledgeId, 
		ackDest: acknowledgeDestination 
	});
	return acknowledgeMessage;
}

// References http://xmpp.org/extensions/xep-0280.html
function formatInfoQueryMessage(to, queryId) {
	var infoQuery = $iq({
        to: to,
        type: "get",
        id: queryId
     }).c("query", { xmlns: "http://jabber.org/protocol/disco#info" });
	return infoQuery;
}

function formatCarbonsStanza(id) {
	var carbonsStanza = $iq({
        type: "set",
        id: id
     }).c("enable", { xmlns: "urn:xmpp:carbons:1" });
	return carbonsStanza;
}

function getXmppMessageId(message) {
	return $("forwarded", message).children("message").attr("id");
}

function getClientAcknowledgeId(message) {
	return $("forwarded", message).children("message").children("received").attr("id");
}

function getServerAcknowledgeId(message) {
	return $("received", message).attr("id");
}

function getXmppMessageAckDestination(message) {
	return $("forwarded", message).children("message").children("request").attr("ackDest");
}

function getXmppMessageSubject(message) {
	return $(message).find("subject").text();
}

function getXmppMessageType(message) {
	return $(message).attr("type");
}

function getXmppMessageBody(message) {
	return $("body", message).text();
}

function getXmppMessageNs(message) {
	return $(message).children("sent").attr("xmlns");
}

function isMessageDelayed(message) {
	return ($(message).children("delay").length === 0) ? false : true;
}
/* End XMPP Utilities */

// UUID generator, rfc4122 compliant, details http://www.ietf.org/rfc/rfc4122.txt
function generateUUID() {
   var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
   });
   return uuid;
}

// Sanitize, check if text is javascript
function parseMessage(message) {
	var xmlDoc;
	if (window.DOMParser) {
		var parser = new DOMParser();
		xmlDoc = parser.parseFromString(message, "text/xml");
	}
	else {
		xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
		xmlDoc.async = false;
		xmlDoc.loadXML(message);
	}
	var xmlMsgToBeDecoded = xmlDoc.getElementsByTagName("msg")[0];
	var timeReceived = new Date(Date.parse(xmlMsgToBeDecoded.getElementsByTagName('datesent')[0].textContent));
	if (xmlMsgToBeDecoded !== undefined) {
		return {
			ConvID: xmlMsgToBeDecoded.getElementsByTagName('convID')[0].textContent,
			From: xmlMsgToBeDecoded.getElementsByTagName('from')[0].textContent,
			To: xmlMsgToBeDecoded.getElementsByTagName('to')[0].textContent,
			Text: xmlMsgToBeDecoded.getElementsByTagName('body')[0].textContent,
			TimeReceived: timeReceived,
			IsSmsBased: xmlMsgToBeDecoded.getElementsByTagName('sms')[0].textContent,
			Year: timeReceived.getFullYear(),
			Month: timeReceived.getMonth(),
			Day: timeReceived.getDay(),
			Hours: timeReceived.getHours(),
			Minutes: timeReceived.getMinutes(),
			Seconds: timeReceived.getSeconds()
		}
	} else return null;
	
}