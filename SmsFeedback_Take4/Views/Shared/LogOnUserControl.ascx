<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>       <span class="noShowOnTablet">
         <%: Resources.Global.welcomeMessage %> <strong><%: Page.User.Identity.Name %></strong>!
         </span>
        [ <%: Html.ActionLink( Resources.Global.logOnLogOffMsg, "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%: Html.ActionLink(Resources.Global.logOnLogOnMsg, "LogOn", "Account") %> ]
<%
    }
%>
