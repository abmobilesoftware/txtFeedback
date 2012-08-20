<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   <%: Resources.Global.reportsPageTitle %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <% if (Html.IsReleaseBuild())
      { %>
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/css/Minified/reports.css") %>" />
   
    <script src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/spin.js") %>" type="application/javascript" ></script>
   <script type="text/javascript" src='https://www.google.com/jsapi?autoload={"modules":[{"name":"visualization","version":"1","packages":["corechart","table"]}]}'></script>
   
   <!-- Format date to local specific -->
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/jquery.ui.datepicker-de.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/jquery.ui.datepicker-ro.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/jquery.ui.datepicker-en-GB.js") %>" type="application/javascript"></script>
   
   <!-- Helper scripts - used like static methods -->
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/DateHelper.js") %>" type="application/javascript"></script>
   
   <!-- Global variables -->
    <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Helpers/Debounce.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/GlobalVariables.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/FirstArea.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/SecondArea.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/ThirdArea.js") %>" type="application/javascript"></script>
   <script type="text/javascript" src=<%: Url.UpdatedResourceLink("~/MyScripts/Base/Minified/BaseLeftSideMenu.js") %>></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/Reports.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/RepFacade.js") %>" type="application/javascript"></script>
    <!-- Left menu, collapsible menu -->
   <script type="text/javascript" src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/CollapsibleLists.js") %>"></script>
     
   <% }
      else
      { %>
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/css/reports.css") %>" />
   <link rel="stylesheet" href="<%: Url.Content("~/Content/css/datepicker/datepicker.css") %>"
      type="text/css" />
   <link rel="stylesheet" media="screen" type="text/css" href="<%: Url.Content("~/Content/css/datepicker/layout.css") %>" />
   <!-- Cool spinner -->
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/spin.js") %>" type="application/javascript"></script>
   <!-- Google Ajax library, for loading other packages -->
   <script type="text/javascript" src='https://www.google.com/jsapi?autoload={"modules":[{"name":"visualization","version":"1","packages":["corechart","table"]}]}'></script>
   <!-- Range picker scripts -->
   <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/datepicker.js") %>"></script>
   <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/eye.js") %>"></script>
   <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/utils.js") %>"></script>
   <!-- Format date to local specific -->
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-de.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-ro.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-en-GB.js") %>" type="application/javascript"></script>

   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Helpers/Debounce.js") %>" type="application/javascript"></script>
    <!-- Helper scripts - used like static methods -->
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/DateHelper.js") %>" type="application/javascript"></script>
   <!-- Global variables -->
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/GlobalVariables.js") %>" type="application/javascript"></script>
   <!-- Reports page specific scripts -->
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/FirstArea.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/SecondArea.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/ThirdArea.js") %>" type="application/javascript"></script>
   <script type="text/javascript" src="<%: Url.UpdatedResourceLink("~/MyScripts/Base/BaseLeftSideMenu.js") %>"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Reports.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/RepFacade.js") %>"
      type="application/javascript"></script>
   <!-- Granularity vertical buttons -->
   <script type="text/javascript" src="<%: Url.Content("~/Scripts/jquery-ui-vertical-buttonset.js") %>"></script>
   <!-- Left menu, collapsible menu -->
   <script type="text/javascript" src="<%: Url.Content("~/Scripts/CollapsibleLists.js") %>"></script>
      
   <% } %>
   <!--<span class="bodyText">Work in progress</span>-->
   <script type="text/template" id="report-template">
               <div id="titleArea">
                <span id="reportTitle">{{ title }}<span id="reportScope"></span></span>
                    <div id="widgetWrapper">
                        <div id="widget">
                            {%
                                var startDay = (window.app.startDate.getDate() < 10) ? "0" + window.app.startDate.getDate() : window.app.startDate.getDate();
                                var startMonth = (window.app.startDate.getMonth() + 1 < 10) ? "0" + (window.app.startDate.getMonth() + 1) : window.app.startDate.getMonth() + 1;
                                var startYear = window.app.startDate.getFullYear();
                                var startDateString = startDay + "-" + startMonth + "-" + startYear;

                                var endDay = (window.app.endDate.getDate() < 10) ? "0" + window.app.endDate.getDate() : window.app.endDate.getDate();
                                var endMonth = (window.app.endDate.getMonth() + 1 < 10) ? "0" + (window.app.endDate.getMonth() + 1) : window.app.endDate.getMonth() + 1;
                                var endYear = window.app.endDate.getFullYear();
                                var endDateString = endDay + "-" + endMonth + "-" + endYear;
                            %}
                            <input type="text" id="from" name="from" class="filterDate filterInputBox"  value="{{ startDateString }}"/> - 
                            <input type="text" id="to" name="to" class="filterDate filterInputBox"  value="{{ endDateString }}"/>
                            
                        </div>
                    </div>
                <div class="clear"></div>
            </div>

            {% for (i=0; i< sections.length; ++i) {
                    if ((sections[i].identifier == "PrimaryChartArea") && sections[i].visibility) {
            %}
                        <div id="chartArea">
                            <div id="granularitySelector">
                                
                                <div class="radioBtnWrapper active"><label for="day" class="radioBtn"><input type="radio" id="day" name="radio" checked="checked" class="radioOption" value="day"/><%: Resources.Global.RepDay %></label></div>
		                        <div class="radioBtnWrapper"><label for="week" class="radioBtn"><input type="radio" id="week" name="radio" class="radioOption" value="week" /><%: Resources.Global.RepWeek %></label></div>
		                        <div class="radioBtnWrapper"><label for="month" class="radioBtn"><input type="radio" id="month" name="radio" class="radioOption" value="month"/><%: Resources.Global.RepMonth %></label></div>
                            </div>
                            <div id="chart_div"></div>
            {%
                            
            %}
                            <div class="clear"></div>
                         </div>
            {%
                    } else if ((sections[i].identifier == "InfoBox") && sections[i].visibility) {
            %}
                            <div id="infoBoxArea">
            
            
                            <div class="clear"></div>
                            </div>
            {%
                   } else if ((sections[i].identifier == "SecondaryChartArea") && sections[i].visibility) {
            %}
                            <div id="tableArea">
                                 <div id="tableContent">
                                     
                                 </div>
                                 <div id="tableChart">
                                    <div id="chart_div1"></div> 
                                 </div>
                                <div class="clear"></div>
                             </div>

            {%
                    }
                }                
             %}
            <div id="overlay"></div>       
             
   </script>
   <div id="menuBar"  class="headerArea">
   </div>
   <div id="leftColumn" class="wordwrap grid_2 leftSideArea">
      <div class="styled-select">
          <select id="workingPointSelector"></select>
      </div>
   </div>
   <div id="rightColumn" class="grid_11 rightColumn">
   </div>
   <script type="text/javascript">
      $(function () {
         var newGUI = new InitializeGUI();
      });
   </script>
   <input type="hidden" value="<%: ViewData["currentCulture"] %>" class="currentCulture" />
</asp:Content>
