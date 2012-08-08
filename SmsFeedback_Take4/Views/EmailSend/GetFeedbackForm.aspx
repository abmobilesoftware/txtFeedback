<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>
    <div style='display:none; >
    <div class='contact-top'></div>
    <div class='contact-content'>
         <h1 class='contact-title'><%: Resources.Global.sendFeedbackHeader %></h1>
        <div class='contact-loading' style='display: none'></div>
        <div class='contact-message' style='display: none'></div>
        <form action='#' style='display:none'>
            <label for='contact-email'><%: Resources.Global.sendEmailTo %></label>
            <input type='text' id='contact-email' class='contact-input' name='email' tabindex='1001' value="<%: ViewData["emailTo"]%>" />
            <label for='contact-subject'><%: Resources.Global.sendEmailSubject %></label>
            <input type='text' id='contact-subject' class='contact-input' name='subject' value=' <%:  ViewData["emailSubject"]%>'
                tabindex='1002' > </input>
           <label for='contact-message'><%: Resources.Global.sendEmailMessage %></label>
            <textarea id='contact-message' class='contact-input' name='message' cols='40' rows='8'
                tabindex='1003'> <%: ViewData["emailText"] %></textarea>
            <br />
            <label>&nbsp;</label>
            <button type='submit' class='contact-send contact-button' tabindex='1004'> <%: Resources.Global.sendEmailSend %></button>
            <button type='submit' class='contact-cancel contact-button simplemodal-close' tabindex='1005'>
                 <%: Resources.Global.sendEmailCancel %></button>
            <br />
            <input type='hidden' name='token' value='" . smcf_token($to) . "' />
            <input type='hidden' name='isFeedbackForm' value='true' />
            <input type='hidden' name='url' value='<%: ViewData["url"] %>' />
        </form>
        <input type="hidden" value="<%: Resources.Global.sendEmailLoadingScreen %>" id="sendEmailLoadingMsg" />
         <input type="hidden" value="<%: Resources.Global.sendEmailSendingEmail %>" id="sendEmailSendingEmailMsg" />
         <input type="hidden" value="<%: Resources.Global.sendEmailEmailSent %>" id="sendEmailEmailSentMsg" />
         <input type="hidden" value="<%: Resources.Global.sendEmailValidationMessageRequired %>" id="sendEmailValidationMessageRequiredMsg" />
         <input type="hidden" value="<%: Resources.Global.sendEmailValidationEmailInvalid %>" id="sendEmailValidationEmailInvalidMsg" />
         <input type="hidden" value="<%: Resources.Global.sendEmailValidationEmailRequired %>" id="sendEmailValidationEmailRequiredMsg" />
         <input type="hidden" value="<%: Resources.Global.sendEmailValidationSubjectRequired %>" id="sendEmailValidationSubjectRequiredMsg" />
    </div>
</div>
