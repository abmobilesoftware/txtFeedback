
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/UserMailer/_Layout.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <%: ViewData["Content"] %> <br/>
	
</asp:Content>
