<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<SmsFeedback_Take4.Models.LogOnModel>" ValidateRequest = "false"%>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<!DOCTYPE html>

<html>
<head runat="server">
   <%: Styles.Render("~/Content/logonCss") %>   
   <%:Scripts.Render("~/bundles/logonjs")%>       
    <title> <%: Resources.Global.TitleLogOn %></title>
</head>

<body>
   <img id="bkgImage" src="<%: Url.Content("~/Content/images/loginImage.jpg") %>"/>
    <div id="logOnContainer">   
    <% using (Html.BeginForm()) { %>
    
        <div id="loginContainer" >
            <div id="logoContainerOnLogin">
               <img src="<%: Url.Content("~/Content/images/logo.png") %>" />
            </div>
            <fieldset id="loginForm">
                <legend>
                   <span class="text">
                   <%: Resources.Global.accountAuthenticate %>
                   </span>
                </legend>
                
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.UserName, new { @placeholder = Resources.Global.lblUserName, @class = "logInTextBox"}) %>
                    <%: Html.ValidationMessageFor(m => m.UserName) %>
                </div>
                
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password,new { @placeholder = Resources.Global.lblPassword, @class = "logInTextBox"}) %>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.CheckBoxFor(m => m.RememberMe) %>
                    <%: Html.LabelFor(m => m.RememberMe) %>
                </div>
                
                <p>
                    <input id="loginBtn" type="submit" value="<%: Resources.Global.LogOnButtonMessage %>" />
                </p>                 
               <div id="changeLanguage">                                               
                
                <%: Html.ImageLink(Url.Content("~/Content/images/Uk_flag.png"), "en-US",null,null) %>
                <%: Html.ImageLink(Url.Content("~/Content/images/De_flag.png"), "de-DE",null,null) %>
                <%: Html.ImageLink(Url.Content("~/Content/images/Spain_flag.png"), "es-ES",null,null) %>
                <%: Html.ImageLink(Url.Content("~/Content/images/Ro_flag.png"), "ro-RO",null,null) %>
             </div>
            </fieldset>
        </div>
        <%: Html.ValidationSummary(true, Resources.Global.loginUnsuccessfulSummary) %>
    <% } %>
   </div>
</body>
</html>
