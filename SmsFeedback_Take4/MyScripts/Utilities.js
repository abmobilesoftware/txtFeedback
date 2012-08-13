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
      checkbox.attr('src', app.domainName + "/Content/images/transparent.gif")
      checkbox.removeClass('deletePhoneNumberIconSelected');
      checkbox.addClass('deletePhoneNumberIconUnselected');
   }
}

window.app.updateNrOfUnreadConversations = function (performUpdateBefore) {
   console.log("updateNrOfUnreadConversations called");
   $.getJSON(window.app.domainName + '/Messages/NrOfUnreadConversations',
   {
      performUpdateBefore: performUpdateBefore
   }, 
                function (data) {
                   if (data != null) {
                      console.log("new value for nr of conversations received");
                      app.setNrOfUnreadConversationOnTab(data.Value);
                   }
                });
}

//#region Store/restore the nr of convs with unread messages when navigating between pages
$(function() {
   $(window).unload(function () {  
      //save the state of the nr of unread messages
      var store = new Persist.Store('SmsFeedback');
      store.set('msgTabcountValue', $("#msgTabcount").text());      
   });
   
})

$(function () {
   var store = new Persist.Store('SmsFeedback');
   store.get('msgTabcountValue', function (ok, val) {
      if (ok)
         $("#msgTabcount").text(val);
   });
   //#endregion
})

//#region handle "authentication expired"
$(function () {
   $(document).ready(function () {
      $(document).ajaxError(function (event, jqxhr, settings) {
         if (jqxhr.status == 401) {
            //for the time being we redirect to the login screen - in the future we should bring up the "relogin" screen
            //http://stackoverflow.com/questions/7532261/ajax-and-formsauthentication-how-prevent-formsauthentication-overrides-http-401
            window.location.href = window.app.loginUrl;
            //if (window.app.loginSummaryUrl) {
            //   if ($('.loginoverlay').length == 0) {
            //      $("body").prepend("<div id='reLogin'><div class='loginoverlay'/><div class='iframe'><iframe id='relogin' src='" + window.app.loginSummaryUrl + "'></iframe></div></div>");
            //      $("div.loginoverlay").show();
            //      $('#summaryLoginBtn').bind('click', function (e) {
            //         e.preventDefault();
            //         $("#reLogin").fadeOut(400, function () { });
            //      });
            //      window.app.lastAjaxCall.jqXHR = jqxhr;
            //      window.app.lastAjaxCall.settings = settings;
            //   }
            //}
         }
      });

      
   });
})
//#endregion
//#region Client side javascript erros
$(function () {
   window.onerror = function (message, file, line) {
      var details = file + ':' + line + '\n' + message;
      $.ajax({
         type: 'POST',
         url: 'ErrorLog/LogError',
         data: JSON.stringify({ errorMsg: details, context: navigator.userAgent }),
         contentType: 'application/json; charset=utf-8'
      });
   };
})
//#endregion
//#region Set tooltip on Messages
$(function () {
   var nrOfConvsWithUnreadMessages = $("#msgTabcount");
   setTooltipOnElement(nrOfConvsWithUnreadMessages, nrOfConvsWithUnreadMessages.attr('tooltiptitle'), 'light');
})
//#endregion