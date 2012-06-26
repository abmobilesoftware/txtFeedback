function StatusBar(sel, options) {
    var _I = this;
    var _sb = null;

    // options
    this.elementId = "_showstatus";
    this.prependMultiline = true;
    this.showCloseButton = true;
    this.afterTimeoutText = null;

    this.cssClass = "statusbar";
    this.highlightClass = "statusbarhighlight";
    this.errorClass = "statuserror";
    this.closeButtonClass = "statusbarclose";
    this.additive = false;

    $.extend(this, options);

    if (sel)
        _sb = $(sel);

    // create statusbar object manually
    if (!_sb) {
        _sb = $("<div id='_statusbar' class='" + _I.cssClass + "'>" +
                "<div class='" + _I.closeButtonClass + "'>" +
                (_I.showCloseButton ? " X </div></div>" : ""))
                 .appendTo(document.body)
                 .show();
    }
    if (_I.showCloseButton) {
        $("." + _I.cssClass).click(function (e) { $(_sb).hide(); });
        //$("div.statusbarclose").bind("click", function () {
        //    $(_sb).hide();
        //});
    }

    this.show = function (message, timeout, isError) {
        if (_I.additive) {
            var html = "<div style='margin-bottom: 2px;' >" + message + "</div>";
            if (_I.prependMultiline)
                _sb.prepend(html);
            else
                _sb.append(html);
        }
        else {

            if (!_I.showCloseButton)
                _sb.text(message);
            else {
                var t = _sb.find("div.statusbarclose");
                _sb.text(message).prepend(t);
            }
        }

        _sb.show();

        if (timeout) {
            if (isError)
                _sb.addClass(_I.errorClass);
            else
                _sb.addClass(_I.highlightClass);

            setTimeout(
                function () {
                    _sb.removeClass(_I.highlightClass);
                    if (_I.afterTimeoutText)
                        _I.show(_I.afterTimeoutText);
                },
                 timeout);
        }
    }
    this.release = function () {
        if (_statusbar)
            $(_statusbar).remove();
    }
}
// use this as a global instance to customize constructor
// or do nothing and get a default status bar

var _statusbar = null;
function showStatus(message, timeout, additive, isError) {
    if (!_statusbar)
        _statusbar = new StatusBar();
    _statusbar.show(message, timeout, additive, isError);
}

function getFromToFromConversation(convID) {
    var fromToArray = convID.split('-');
    return fromToArray;
}

function buildConversationID(from, to) {
    return from + "-" + to;
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

//checkbox is the button img
function setCheckboxState(checkbox, state)
{
   if (state === true) {
      checkbox.attr('src', domainName + "/Content/images/check-white.svg");
      checkbox.removeClass('deletePhoneNumberIconUnselected');
      checkbox.addClass('deletePhoneNumberIconSelected');
   }
   else {
      //we actually need only the img placeholder (for the border and background)
      //so we set the image to a transparent 1 pixel gif
      checkbox.attr('src', domainName + "/Content/images/transparent.gif")
      checkbox.removeClass('deletePhoneNumberIconSelected');
      checkbox.addClass('deletePhoneNumberIconUnselected');
   }
}