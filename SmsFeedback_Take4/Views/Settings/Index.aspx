<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Resources.Global.settingsPageTitle %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
      <% if (Html.IsReleaseBuild())      { %>
        <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/css/settings.css") %>" />
    <% }      else      { %>
      <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/css/settings.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/areaDefinitions.css") %>" />
     <% } %>
   <div class="headerArea">
   </div>
   <div class="clear"></div>
   <div  class="wordwrap tagsPhoneNumbers grid_2 leftSideArea">

</asp:Content>
 
