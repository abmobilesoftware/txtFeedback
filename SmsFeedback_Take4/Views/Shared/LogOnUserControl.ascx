<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
         <%: Resources.Global.welcomeMessage %> <strong><%: Page.User.Identity.Name %></strong>!
        [ <%: Html.ActionLink("Log Off", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("Log On", "LogOn", "Account") %> ]
<%
    }
%>
