<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<SmsFeedback_Take4.Models.WorkingPoint>>" %>
<%@ Import Namespace="SmsFeedback_Take4.Models" %>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<!DOCTYPE html>   
  <script type="text/javascript"  src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.json-2.3.min.js")%>"></script>   
   <legend id="wpConfigLegend"><%= Resources.Global.settingsWpConfigLegend %></legend>
   <% using (Html.BeginForm())
      { %>
   <fieldset id="wpConfig">
      <table id="workingPointsConfig" cellspacing="10" cellpadding="10">
         <tr>
            <th align="left" class="limitedColWidth" tooltiptitle="<%: Resources.Global.settingsWpTelNoHeaderTooltip %>"><%= Resources.Global.settingsTelNoHeader %></th>
            <th align="left" class="limitedColWidth" tooltiptitle="<%: Resources.Global.settingsShortIDHeaderTooltip %>"><%= Resources.Global.settingsShortIDHeader %></th>
            <th align="left" class="limitedColWidth" tooltiptitle="<%: Resources.Global.settingsMaxOutboundSmsHeaderTooltip %>"><%= Resources.Global.settingsMaxOutboundSmsHeader %></th>
            <th align="left" tooltiptitle="<%: Resources.Global.settingsWpNameHeaderTooltip %>"><%= Resources.Global.settingsWpNameHeader %></th>
            <th align="left" tooltiptitle="<%: Resources.Global.settingsWpDescriptionHeaderTooltip %>"><%= Resources.Global.settingsWpDescriptionHeader %></th>
            <th align="left" tooltiptitle="<%: Resources.Global.settingsWpWelcomeMessageHeaderTooltip %>"><%= Resources.Global.settingsWpWelcomeMessageHeader %></th>
         </tr>
         <% foreach (SmsFeedback_Take4.Models.WorkingPoint wp in Model)
            { %>
         <tr name="dataRow">
            <td><span name="TelNumber"><%= wp.TelNumber %></span></td>
            <td><span name="ShortID"><%= wp.ShortID %></span></td>
            <td><span name="MaxNrOfSmsToSendPerMonth"><%= wp.MaxNrOfSmsToSendPerMonth %></span></td>
            <td>                
               <input name="Name" type="text"  maxlength="40" value="<%=wp.Name %>" />
            <td>
               <input name="Description" type="text" maxlength="160" value="<%=wp.Description %>" />
            </td>
            <td>
               <input name="WelcomeMessage" type="text" maxlength="160" value="<%= wp.WelcomeMessage %>" />
            </td>
         </tr>
         <% }; %>
      </table>
      <button id="btnSaveWorkingPoints" class="btnSaveChanges"><%: Resources.Global.saveBtnCaption %></button>
   </fieldset>
   <%: Html.ValidationSummary(false, Resources.Global.settingsWpConfigErrors, new {id= "wpConfigErrors", })%>
   <% if ( ViewData["saveMessage"]!=null ){ %>
		<div id="wpSaveResult">
         <span><%= ViewData["saveMessage"] %></span>
		</div>
   <% } %>
   <%} %>
</body> </html> 