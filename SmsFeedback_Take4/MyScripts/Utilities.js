/*global window */
/*global Persist */
/*global document */
/*global navigator */

window.app = window.app || {};
function showStatus(message, timeout, additive, isError) {
  
}
//the domain name should come from the server! - when publishing on cluj-info.com/smsfeedback
window.app.domainName = '';
//window.app.domainName = '/smsfeedback';
window.app.lastAjaxCall = { settings: null, jqXHR: null };
window.app.loginSummaryUrl = "/Account/LogOnSummary";
window.app.loginUrl = "/Account/LogOn";
var RESTDomain = "http://t3xt.cloudapp.net:81";

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
function getFromToFromConversation(convID) {
   var fromToArray = convID.split(cConversationIdNumbersSeparator);
   return fromToArray;
}
function cleanupPhoneNumber(data) {
   var prefixes = new Array("00", "\\+");
   //remove 00 and + from the beginning of the number
   //remove all domain qualifiers - everything after @
   var reg = new RegExp('^(' + prefixes.join('|') + ')|@.+$', "g");
   data = data.replace(reg, "");
   return data;
}
function buildConversationID(from, to) {
   return cleanupPhoneNumber(from) + cConversationIdNumbersSeparator + cleanupPhoneNumber(to);
}
function comparePhoneNumbers(phoneNumber1, phoneNumber2){
   //take into account that they could start with + or 00 - so we strip away any leading + or 00
   return cleanupPhoneNumber(phoneNumber1) === cleanupPhoneNumber(phoneNumber2);
}

function endsWith(str, suffix) {
   return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function isWorkingPoint(phoneNumber, componentExtension) {
   if (componentExtension === undefined || componentExtension==="null") {
      return false;
   }
   //yes if its format is id@componentExtension
   return endsWith(phoneNumber, componentExtension);
}
//#endregion

//uses qtip
function setTooltipOnElement(elem, tooltip, style) {
  //TODO DA set tooltip on element
}

//checkbox is the button img
function setCheckboxState(checkbox, state)
{
   if (state === true) {
      checkbox.attr('src', window.app.domainName + "/Content/images/check-white.svg");
      checkbox.removeClass('deletePhoneNumberIconUnselected');
      checkbox.addClass('deletePhoneNumberIconSelected');
   }
   else {
      //we actually need only the img placeholder (for the border and background)
      //so we set the image to a transparent 1 pixel gif
      checkbox.attr('src', window.app.domainName + "/Content/images/transparent.gif");
      checkbox.removeClass('deletePhoneNumberIconSelected');
      checkbox.addClass('deletePhoneNumberIconUnselected');
   }
}

window.app.updateNrOfUnreadConversations = function (performUpdateBefore) {
   $.getJSON(window.app.domainName + '/Conversations/NrOfUnreadConversations',
   { performUpdateBefore: performUpdateBefore },
   function (data) {
      if (data !== null) {
         window.app.setNrOfUnreadConversationOnTab(data.Value);
      }
   });
};

//#region Check Sms Subscription Status and set the canSendSms flag
window.app.canSendSmS = true;
window.app.checkSmsSubscriptionStatus = function () {
   $.getJSON('Messages/SMSSSubscriptionStatus',
                  { },
                  function (data) {
                     //if error required
                     if (data.SpendingLimitReached) {
                        window.app.canSendSmS = false;
                        window.app.NotifyArea.show(data.SpendingLimitReachedMessage, function ()
                        {
                           window.location.href = "mailto:contact@txtfeedback.net?subject=Increase spending limit or Buy Credit";
                           
                        }, true);
                     }
                     else if (data.WarningLimitReached) {//if warning required                                          
                        window.app.NotifyArea.show(data.WarningLimitReachedMessage, function ()
                        {
                           window.location.href = "mailto:contact@txtfeedback.net?subject=Ensure that enough credit is still available";
                        }, false);
                     }
                     
                  }
          );
};
//#endregion

window.app.Payload = Backbone.Model.extend({
   defaults: {      
      SpendingLimitReached: false,
      WarningLimitReached: false,
      MessageID: 0,      
      MessageSent: false,
      Reason: ""
   }
});
function payloadReceived(content, messageId) {
   var payload = new window.app.Payload(jQuery.parseJSON(content));
   if (!payload.get("MessageSent")) {
      var msg = $('#messageNotSentReasonUnknown').val();
      if (payload.get("SpendingLimitReached")) {
         //could not sent message as spending limit reached
         window.app.canSendSmS = false; //make sure that no other messages can be sent
         msg = $('#messageNotSentInsufficientCredits').val();
         window.app.NotifyArea.show(msg, function () {
            window.location.href = "mailto:contact@txtfeedback.net?subject=Increase spending limit or Buy Credit";
         }, true);
      } else {
         window.app.NotifyArea.show(msg, function () {
            window.location.href = "mailto:contact@txtfeedback.net?subject=Unable to deliver message with id " + payload.get("MessageID");
         }, true);         
      }
   } else {
      if (payload.get("SpendingLimitReached")) {
         var errorMsg = $('#messageSentSpendingLimitReached').val();
         window.app.NotifyArea.show(errorMsg, function () {
            window.location.href = "mailto:contact@txtfeedback.net?subject=Increase spending limit or Buy Credit";
         }, false);
      }
      $(document).trigger('clientAcknowledge', {
         message: getMessage(messageId)
      });
   }
}

//#region Region resize
window.app.resizeInProgress = false;
function resizeTriggered() {
   "use strict";
   if (!window.app.resizeInProgress) {
      window.app.resizeInProgress = true;
      //pick the highest between window size (- header) and messagesArea
      //var padding = 5;
      //var msgAreaMarginTop = 10;
      var filterStripHeigh, magicNumber;
      //if on tablet, or if resolution == 1024
      //DA - determine if on 1024
      //TODO DA - clean up this messy code :)
      var rezIs1024 = false;
      if(!$('div#title').is(":visible")) {
         rezIs1024 = true;
      }
      if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent) || rezIs1024 === true) {
      filterStripHeigh = 0;
      magicNumber = 82;
   }
   else {
      filterStripHeigh =  $(".headerArea").outerHeight(true);
      magicNumber =128;
   }   
   var window_height = window.innerHeight;  
   var headerHeight = $('header').outerHeight(true);
   var contentWindowHeight = window_height - headerHeight;// - (2 * padding) - filterStripHeigh;
   var quickActionBtnsBar = $("#quickActionBtns").is(":hidden") || $("#quickActionBtns").length == 0 ? 0 : $("#quickActionBtns").outerHeight(true);
   var marginTop = parseInt($('#rightColumn').css('margin-top'),10);
   var marginBottom = parseInt($('#rightColumn').css('margin-bottom'),10);
   var rightAreaCandidateHeight = contentWindowHeight - filterStripHeigh - marginTop - marginBottom;
   //first make sure we have a valid right side heigt
   $('#rightColumn').height('auto');
   var rightAreaHeight = $('#rightColumn').outerHeight(true);
   var contentContainer = $('.container_12');
   
   //we need to set a fixed height for the conversations area and messages area to ensure that scrolling is enabled
   if (rightAreaCandidateHeight > rightAreaHeight) {
      contentContainer.height(contentWindowHeight);
      $('#rightColumn').height(rightAreaCandidateHeight);
      $('#leftColumn').height(contentWindowHeight - filterStripHeigh);
      $('#scrollableconversations').height(rightAreaCandidateHeight);
      $('#scrollablemessagebox').height(rightAreaCandidateHeight - magicNumber - quickActionBtnsBar);
   } else {      
      contentContainer.height(rightAreaHeight + filterStripHeigh + marginTop + marginBottom);
      $('#leftColumn').height(rightAreaHeight + marginTop + marginBottom);
      $('#scrollableconversations').height(rightAreaHeight);
      $('#scrollablemessagebox').height(rightAreaHeight - magicNumber- quickActionBtnsBar);
   } 
   $('.page').height(headerHeight + contentContainer.height());
   $('body').height(headerHeight + contentContainer.height());
   $('html').height(headerHeight + contentContainer.height());
   window.app.resizeInProgress = false;
   }
}

$(function () {
   //DA IE8 doesn't support addEventListener so we use attachEvent
   //source http://stackoverflow.com/questions/9769868/addeventlistener-not-working-in-ie8
   if (!window.addEventListener) {
      window.attachEvent("resize", resizeTriggered);
     
   }
   else {
      window.addEventListener("resize", resizeTriggered, false);
      window.addEventListener("smsPayloadReceived", payloadReceived, false);
   }
   $(document).bind('smsPayloadReceived', function (ev, data) {
      payloadReceived(data.content, data.messageId);
   });
});
//#endregion

//#region Store/restore the nr of convs with unread messages when navigating between pages
$(function () {
   $(window).unload(function () {
      //save the state of the nr of unread messages
      var store = new Persist.Store('SmsFeedback');
      store.set('msgTabcountValue', $("#msgTabcount").text());
   });

});

$(function () {
   var store = new Persist.Store('SmsFeedback');
   store.get('msgTabcountValue', function (ok, val) {
      if (ok) {
         $("#msgTabcount").text(val);
      }
   });

});
//#endregion
//#region make sure Jquery AJAX requests are not cached
$(function () {
   $.ajaxSetup({ cache: false });
});
//#endregion
//#region handle "authentication expired"
$(function () {
   $(document).ready(function () {
      $(document).ajaxError(function (event, jqxhr, settings) {
         if (jqxhr.status === 401) {
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
});
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
      var details = file + ': ' + line + '\n' + message;
      window.app.logErrorOnServer(details);
   };
});
//#endregion
//#region Set tooltip on Messages
$(function () {
   var nrOfConvsWithUnreadMessages = $("#msgTabcount");
   //setTooltipOnElement(nrOfConvsWithUnreadMessages, nrOfConvsWithUnreadMessages.attr('title'), 'light');
});
//#endregion

//#region Notification Area

window.app.NotifyAreaModel = Backbone.Model.extend({
   defaults: {
      Message: "You are approaching your spending limit for this month - make sure your limit or credit are high enough for normal operation",
      SolveAction: function () {         
         window.location.href = "mailto:contact@txtfeedback.net";
      },
      Visible: false,
      IsError: true
   }   
});

_.templateSettings = {
   interpolate: /\{\{(.+?)\}\}/g,      // print value: {{ value_name }}
   evaluate: /\{%([\s\S]+?)%\}/g,   // execute code: {% code_to_execute %}
   escape: /\{%-([\s\S]+?)%\}/g
};

window.app.NotifyAreaView = Backbone.View.extend({
   model: window.app.NotifyAreaModel,
   tagName: 'div',
   events: {
      "click #takeAction": "callAction",
      "click #notificationAck": "hide"
   },
   initialize: function () {
      this.template = _.template($('#notifyAreaTemplate').html());
      _.bindAll(this, 'render', 'callAction', 'hide');
      this.model.bind('change', this.render);
      return this.render;
   },
   render: function () {
      this.$el.html(this.template(this.model.toJSON()));
      this.$el.addClass('notificationArea');
      if (!this.model.get('Visible')) {
         this.$el.hide();
      }
      return this;
   },
   callAction: function () {
      //DA here we should have actions like "mailto:contact@txtfeedback.net"
      var fn = this.model.get("SolveAction");
      fn();
   },
   show: function (message, solveAction, isError) {
      this.model.set("Visible", true);
      this.model.set("Message", message);
      this.model.set("IsError", isError);
      this.model.set("SolveAction", solveAction);
      this.$el.show();
      resizeTriggered();
   },
   hide: function () {
      this.model.set("Visible", false);
      this.$el.hide();
      resizeTriggered();
   }
});

$(function () {
   var m = new window.app.NotifyAreaModel();
   window.app.NotifyArea = new window.app.NotifyAreaView({model:m});
   $('header').prepend(window.app.NotifyArea.render().$el);
   window.app.checkSmsSubscriptionStatus();  
});
//#endregion

//#region Detect if event is supported
var isEventSupported = (function () {
   var TAGNAMES = {
      'select': 'input', 'change': 'input',
      'submit': 'form', 'reset': 'form',
      'error': 'img', 'load': 'img', 'abort': 'img'
   };
   function isEventSupported(eventName) {
      var el = document.createElement(TAGNAMES[eventName] || 'div');
      eventName = 'on' + eventName;
      var isSupported = (eventName in el);
      if (!isSupported) {
         el.setAttribute(eventName, 'return;');
         isSupported = typeof el[eventName] == 'function';
      }
      el = null;
      return isSupported;
   }
   return isEventSupported;
})();
//#endregion