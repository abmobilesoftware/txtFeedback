<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<!DOCTYPE html>  
    <div class='contact-top'></div>
    <div class='contact-content'>
        <h1 class='contact-title'><%: Resources.Global.sendEmailHeader %></h1>
        <div class='contact-loading' style='display: none'></div>
        <div class='contact-message' style='display: none'></div>
        <form action='#' style='display:none'>
            <label for='contact-email'><%: Resources.Global.sendEmailTo %></label>
            <input type='text' id='contact-email' class='contact-input' name='email' tabindex='1001' />
            <label for='contact-subject'><%: Resources.Global.sendEmailSubject %></label>
            <input type='text' id='contact-subject' class='contact-input' name='subject' value=''
                tabindex='1002' />
            <label for='contact-message'><%: Resources.Global.sendEmailMessage %></label>
            <textarea id='contact-message' class='contact-input' name='message' cols='40' rows='4'
                tabindex='1003'> <%: ViewData["emailText"] %></textarea>
            <br />
            <label>&nbsp;</label>
            <button type='submit' class='contact-send contact-button' tabindex='1004'>Send</button>
            <button type='submit' class='contact-cancel contact-button simplemodal-close' tabindex='1005'>
                <%: Resources.Global.sendEmailCancel %></button>
            <br />
            <input type='hidden' name='token' value='" . smcf_token($to) . "' />
        </form>
    </div>
</div>