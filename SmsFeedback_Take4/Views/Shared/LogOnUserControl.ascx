<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
         <%: Resources.Global.welcomeMessage %> <strong><%: Page.User.Identity.Name %></strong>!
        [ <%: Html.ActionLink( Resources.Global.logOnLogOffMsg, "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%: Html.ActionLink(Resources.Global.logOnLogOnMsg, "LogOn", "Account") %> ]
<%
    }
%>
