<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Reports
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/css/reports.css") %>" />
   <span class="bodyText">Work in progress</span>
</asp:Content>
