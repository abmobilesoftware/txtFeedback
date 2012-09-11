<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   <%: Resources.Global.settingsPageTitle %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <% if (Html.IsReleaseBuild())
      { %>
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/css/Minified/settings.css") %>" />
   
   <script type="text/javascript" src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/CollapsibleLists.js") %>"></script>
   <script type="text/javascript" src=<%: Url.UpdatedResourceLink("~/MyScripts/Base/Minified/BaseLeftSideMenu.js") %>></script>
   <script type="text/javascript" src=<%: Url.UpdatedResourceLink("~/MyScripts/Settings/Minified/settings.js") %>></script>
   <script type="text/javascript" src=<%: Url.UpdatedResourceLink("~/MyScripts/Settings/Minified/SettingsFacade.js") %>></script>   
   <% }
      else
      { %>
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/css/settings.css") %>" />

   <script type="text/javascript" src="<%: Url.UpdatedResourceLink("~/Scripts/CollapsibleLists.js") %>"></script>
   <script type="text/javascript" src=<%: Url.UpdatedResourceLink("~/MyScripts/Base/BaseLeftSideMenu.js") %>></script>
   <script type="text/javascript" src=<%: Url.UpdatedResourceLink("~/MyScripts/Settings/settings.js") %>></script>
   <script type="text/javascript" src=<%: Url.UpdatedResourceLink("~/MyScripts/Settings/SettingsFacade.js") %>></script>   
   <% } %>
 
   <div id="settingsMenuBar" class="headerArea">
   </div>
   <div class="clear"></div>
   <div id="leftColumn" class="wordwrap tagsPhoneNumbers grid_2 leftSideArea"></div>
   <div id="rightColumn" class="rightColumn grid_11"></div>
</asp:Content>
