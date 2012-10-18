/*global window */
/*global navigator */
/*global console */
window.app = window.app || {};
window.app.domainName = '';
window.app.lastAjaxCall = { settings: null, jqXHR: null };
window.app.loginSummaryUrl = "/Account/LogOnSummary";
window.app.loginUrl = "/Account/LogOn";

window.app.firstCall = true;
window.app.requestIndex = 0;

function trim(s) {
   s = s.replace(/(^\s*)|(\s*$)/gi, "");
   s = s.replace(/[ ]{2,}/gi, " ");
   s = s.replace(/\n /, "\n");
   return s;
}

//#region Utilities for processing phone numbers
var cConversationIdNumbersSeparator = '-';
function buildConversationID(from, to) {
   return cleanupPhoneNumber(from) + cConversationIdNumbersSeparator + cleanupPhoneNumber(to);
}
function comparePhoneNumbers(phoneNumber1, phoneNumber2)
{
   //take into account that they could start with + or 00 - so we strip away any leading + or 00
   return cleanupPhoneNumber(phoneNumber1) === cleanupPhoneNumber(phoneNumber2);
}
function cleanupPhoneNumber(data) {
   var prefixes = new Array("00", "\\+");
   //remove 00 and + from the beginning of the number
   //remove all domain qualifiers - everything after @
   var reg = new RegExp('^(' + prefixes.join('|') + ')|@.+$', "g");
   data = data.replace(reg, "");   
   return data;
}
function getFromToFromConversation(convID) {
   var fromToArray = convID.split(cConversationIdNumbersSeparator);
   return fromToArray;
}
//#endregion

//#region make sure Jquery AJAX requests are not cached
$(function () {
   $.ajaxSetup({ cache: false });
});
//#endregion

//#region Client side JavaScript errors
window.app.logErrorOnServer = function logError(message) {
   //console.log("error: " + message);
   $.ajax({
      type: 'POST',
      url: 'ErrorLog/LogError',
      data: JSON.stringify({ errorMsg: message, context: navigator.userAgent }),
      contentType: 'application/json; charset=utf-8'
   });
};

window.app.logDebugOnServer = function logError(message) {
   //console.log("message: " + message);
   $.ajax({
      type: 'POST',
      url: 'ErrorLog/LogDebug',
      data: JSON.stringify({ errorMsg: message, context: navigator.userAgent }),
      contentType: 'application/json; charset=utf-8'
   });
};

$(function () {
   window.onerror = function (message, file, line) {
      var details = file + ':' + line + '\n' + message;
      window.app.logErrorOnServer(details);
   };
});
//#endregion