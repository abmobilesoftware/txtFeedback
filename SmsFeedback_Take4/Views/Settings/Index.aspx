<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   <%: Resources.Global.settingsPageTitle %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="IncludesCssArea" runat="server">
    <%: Styles.Render("~/Content/settingsCss") %>   
</asp:Content>
<asp:Content ID="Content15" ContentPlaceHolderID="IncludesJsArea" runat="server">   
   <%:Scripts.Render("~/bundles/settingsjs")%>   
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
