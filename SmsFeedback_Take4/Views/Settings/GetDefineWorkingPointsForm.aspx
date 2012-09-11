<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<SmsFeedback_Take4.Models.WorkingPoint>>" %>

<%@ Import Namespace="SmsFeedback_Take4.Models" %>
<!DOCTYPE html>
<div>
   <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>
   <script src="../../Scripts/jquery.json-2.3.min.js" type="text/javascript" />
   <legend id="wpConfigLegend">Working Points</legend>
   <% using (Html.BeginForm())
      { %>
   <fieldset id="wpConfig">
      <table id="workingPointsConfig" cellspacing="10" cellpadding="10">
         <tr>
            <th align="left">TelNumber</th>
            <th align="left">Max outbound SMS</th>
            <th align="left">Name</th>
            <th align="left">Description</th>
         </tr>
         <% foreach (SmsFeedback_Take4.Models.WorkingPoint wp in Model)
            { %>
         <tr name="dataRow">
            <td><span name="TelNumber"><%= wp.TelNumber %></td>
            </span>
            <td><span name="MaxNrOfSmsToSendPerMonth"><%= wp.MaxNrOfSmsToSendPerMonth %></span></td>
            <td>                
               <input name="Name" type="text" value="<%=wp.Name %>" />
            <td>
               <input name="Description" type="text" value="<%=wp.Description %>" />
            </td>
         </tr>
         <% }; %>
      </table>
      <button id="btnSaveWorkingPoints" class="btnSaveChanges">Save</button>
   </fieldset>
   <%} %>
</div>
</body> </html> 