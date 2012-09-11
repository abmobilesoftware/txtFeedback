<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<SmsFeedback_Take4.Models.ChangePasswordModel>" %>

<!DOCTYPE html>

<div id="changePasswordContainer">
    <h2><%: Resources.Global.settingsChangePasswordTitle %></h2>   
    <p>
        <%: String.Format(Resources.Global.settingsDescriptionPrefix, Membership.MinRequiredPasswordLength) %>.
    </p>

    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

    <% using (Html.BeginForm()) { %>      
        <div>
            <fieldset>
                <legend><%: Resources.Global.accountInformation %></legend>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.OldPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.OldPassword) %>
                    <%: Html.ValidationMessageFor(m => m.OldPassword) %>
                </div>
                
                <div id="newPasswordAreaLabelStart" class="editor-label newPasswordAreaLabel">
                    <%: Html.LabelFor(m => m.NewPassword) %>
                </div>
                <div class="editor-field newPasswordAreaPassw">
                    <%: Html.PasswordFor(m => m.NewPassword) %>
                    <%: Html.ValidationMessageFor(m => m.NewPassword) %>
                </div>
                
                <div class="editor-label newPasswordAreaLabel">
                    <%: Html.LabelFor(m => m.ConfirmPassword) %>
                </div>
                <div class="editor-field newPasswordAreaPassw">
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                
                <p>
                    <button id="btnChangePassword" class="btnSaveChanges"><%: Resources.Global.settingsBtnChangePassword %></button> 
                </p>
            </fieldset>
        </div>
        <%: Html.ValidationSummary(true, Resources.Global.settingsErrorPasswordChangeSummary) %>
    <% } %>
</div>
</html>
