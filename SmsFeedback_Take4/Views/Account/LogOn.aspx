<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<SmsFeedback_Take4.Models.LogOnModel>" %>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<!DOCTYPE html>

<html>
<head runat="server">
   <link rel="stylesheet" href="../../Content/themes/base/jquery.ui.all.css">

      <% if (Html.IsReleaseBuild())      { %>
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/Minified/reset.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/Minified/text.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/Minified/grid.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/Minified/layout.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/Minified/nav.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/Minified/Site.css") %>" />
   <% }      else      { %>
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/reset.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/text.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/grid.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/layout.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/nav.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%:Url.UpdatedResourceLink("~/Content/Site.css") %>" />
   <% } %>

   <script src="<%: Url.Content("~/Scripts/jquery-1.6.2.min.js") %>" type="text/javascript"></script>
    <title>Log On</title>
</head>

<body>
   <script>
      $(function () {
         $("#UserName").focus(function () {
            var x = this;
         });
      });
   </script>
   <img id="bkgImage" src="<%: Url.Content("~/Content/images/loginImage.png") %>"/>
     
    <div id="logOnContainer">   
  
    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

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
                    <%: Html.TextBoxFor(m => m.UserName, new { @placeholder = Resources.Global.lblUserName}) %>
                    <%: Html.ValidationMessageFor(m => m.UserName) %>
                </div>
                
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password,new { @placeholder = Resources.Global.lblPassword}) %>
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
                <%: Html.ImageLink(Url.Content("~/Content/images/Ro_flag.png"), "ro-RO",null,null) %>
             </div>
            </fieldset>
        </div>
        <%: Html.ValidationSummary(true, Resources.Global.loginUnsuccessfulSummary) %>
    <% } %>
   </div>
</body>
</html>
