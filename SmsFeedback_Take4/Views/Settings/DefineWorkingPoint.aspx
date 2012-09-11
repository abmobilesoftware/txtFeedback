<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<SmsFeedback_Take4.Models.WorkingPoint>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>DefineWorkingPoint</title>
</head>
<body>
    <script src="<%: Url.Content("~/Scripts/jquery-1.6.2.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>
    
    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true) %>
        <fieldset>
            <legend>WorkingPoint</legend>
    
            <div class="editor-label">
                <%: Html.LabelFor(model => model.TelNumber) %>
            </div>
            <div class="editor-field">
                <%: Html.EditorFor(model => model.TelNumber) %>
                <%: Html.ValidationMessageFor(model => model.TelNumber) %>
            </div>
    
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Name) %>
            </div>
            <div class="editor-field">
                <%: Html.EditorFor(model => model.Name) %>
                <%: Html.ValidationMessageFor(model => model.Name) %>
            </div>
    
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Description) %>
            </div>
            <div class="editor-field">
                <%: Html.EditorFor(model => model.Description) %>
                <%: Html.ValidationMessageFor(model => model.Description) %>
            </div>
    
            <div class="editor-label">
                <%: Html.LabelFor(model => model.NrOfSentSmsThisMonth) %>
            </div>
            <div class="editor-field">
                <%: Html.EditorFor(model => model.NrOfSentSmsThisMonth) %>
                <%: Html.ValidationMessageFor(model => model.NrOfSentSmsThisMonth) %>
            </div>
    
            <div class="editor-label">
                <%: Html.LabelFor(model => model.MaxNrOfSmsToSendPerMonth) %>
            </div>
            <div class="editor-field">
                <%: Html.EditorFor(model => model.MaxNrOfSmsToSendPerMonth) %>
                <%: Html.ValidationMessageFor(model => model.MaxNrOfSmsToSendPerMonth) %>
            </div>
    
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>
    <% } %>
    
    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>
</body>
</html>
