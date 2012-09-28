window.app = window.app || {};
function showStatus(message, timeout, additive, isError) {
  
}
//the domain name should come from the server! - when publishing on cluj-info.com/smsfeedback
window.app.domainName = '';
//window.app.domainName = '/smsfeedback';
window.app.lastAjaxCall = { settings: null, jqXHR: null };
window.app.loginSummaryUrl = "/Account/LogOnSummary";
window.app.loginUrl = "/Account/LogOn";

window.app.firstCall = true;
window.app.requestIndex = 0;

function getFromToFromConversation(convID) {
   var fromToArray = convID.split(cConversationIdNumbersSeparator);
    return fromToArray;
}

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
   var prefix = new RegExp('^(' + prefixes.join('|') + ')', "g");   
   data = data.replace(prefix, "");   
   return data;
}
//#endregion

//uses qtip
function setTooltipOnElement(elem, tooltip, style) {
   elem.qtip({
      content: { text: tooltip },
      style: style
   });
}

//checkbox is the button img
function setCheckboxState(checkbox, state)
{
   if (state === true) {
      checkbox.attr('src', app.domainName + "/Content/images/check-white.svg");
      checkbox.removeClass('deletePhoneNumberIconUnselected');
      checkbox.addClass('deletePhoneNumberIconSelected');
   }
   else {
      //we actually need only the img placeholder (for the border and background)
      //so we set the image to a transparent 1 pixel gif
      checkbox.attr('src', app.domainName + "/Content/images/transparent.gif");
      checkbox.removeClass('deletePhoneNumberIconSelected');
      checkbox.addClass('deletePhoneNumberIconUnselected');
   }
}

window.app.updateNrOfUnreadConversations = function (performUpdateBefore) {
   $.getJSON(window.app.domainName + '/Messages/NrOfUnreadConversations',
   { performUpdateBefore: performUpdateBefore },
   function (data) {
      if (data !== null) {
         app.setNrOfUnreadConversationOnTab(data.Value);
      }
   });
};

//#region make sure Jquery AJAX requests are not cached
$(function () {
   $.ajaxSetup({ cache: false });
})
//#endregion

//#region Client side javascript erros
window.app.logErrorOnServer = function logError(message) {
   $.ajax({
      type: 'POST',
      url: 'ErrorLog/LogError',
      data: JSON.stringify({ errorMsg: message, context: navigator.userAgent }),
      contentType: 'application/json; charset=utf-8'
   });
};

window.app.logDebugOnServer = function logError(message) {
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
