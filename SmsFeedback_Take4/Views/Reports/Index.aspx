﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   <%: Resources.Global.reportsPageTitle %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="IncludesArea" runat="server">
   <% if (Html.IsReleaseBuild())
      { %>
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/css/Minified/reports.css") %>" />
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/spin.js") %>" type="application/javascript"></script>
   <script type="text/javascript" src='https://www.google.com/jsapi?autoload={"modules":[{"name":"visualization","version":"1","packages":["corechart","table"]}]}'></script>
   <!-- Format date to local specific -->
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/jquery.ui.datepicker-de.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/jquery.ui.datepicker-ro.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/jquery.ui.datepicker-en-GB.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-es.js") %>"
      type="application/javascript"></script>
   <!-- Helper scripts - used like static methods -->
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/DateHelper.js") %>"
      type="application/javascript"></script>
   <!-- Global variables -->
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/Json2Csv.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Helpers/Debounce.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/GlobalVariables.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/FirstArea.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/SecondArea.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/ThirdArea.js") %>"
      type="application/javascript"></script>
   <script type="text/javascript" src="<%: Url.UpdatedResourceLink("~/MyScripts/Base/Minified/BaseLeftSideMenu.js") %>"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/Reports.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Minified/RepFacade.js") %>"
      type="application/javascript"></script>
   <!-- Left menu, collapsible menu -->
   <script type="text/javascript" src="<%: Url.UpdatedResourceLink("~/Scripts/Minified/CollapsibleLists.js") %>"></script>
   <% }
      else
      { %>
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/css/reports.css") %>" />
   <!-- Cool spinner -->
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/spin.js") %>" type="application/javascript"></script>
   <!-- Google Ajax library, for loading other packages -->
   <script type="text/javascript" src='https://www.google.com/jsapi?autoload={"modules":[{"name":"visualization","version":"1","packages":["corechart","table"]}]}'></script>
   <!-- Format date to local specific -->
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-de.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-ro.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-en-GB.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-es.js") %>"
      type="application/javascript"></script>

   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Helpers/Debounce.js") %>" type="application/javascript"></script>
   <!-- Helper scripts - used like static methods -->
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/DateHelper.js") %>"
      type="application/javascript"></script>
   <!-- Global variables -->
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/GlobalVariables.js") %>"
      type="application/javascript"></script>
   <!-- Reports page specific scripts -->

   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Json2Csv.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/FirstArea.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/SecondArea.js") %>"
      type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/ThirdArea.js") %>"
      type="application/javascript"></script>
   <script type="text/javascript" src="<%: Url.UpdatedResourceLink("~/MyScripts/Base/BaseLeftSideMenu.js") %>"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Reports.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/RepFacade.js") %>"
      type="application/javascript"></script>
   <!-- Left menu, collapsible menu -->
   <script type="text/javascript" src="<%: Url.Content("~/Scripts/CollapsibleLists.js") %>"></script>

   <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="TemplatesArea" runat="server">
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
                                var startDateString = startDay + "/" + startMonth + "/" + startYear;

                                var endDay = (window.app.endDate.getDate() < 10) ? "0" + window.app.endDate.getDate() : window.app.endDate.getDate();
                                var endMonth = (window.app.endDate.getMonth() + 1 < 10) ? "0" + (window.app.endDate.getMonth() + 1) : window.app.endDate.getMonth() + 1;
                                var endYear = window.app.endDate.getFullYear();
                                var endDateString = endDay + "/" + endMonth + "/" + endYear;
                            %}
                            <input type="text" id="from" name="from" class="filterDate filterInputBox"  value="{{ startDateString }}"/> - 
                            <input type="text" id="to" name="to" class="filterDate filterInputBox"  value="{{ endDateString }}"/>
                            
                        </div>
                    </div>
                <div class="clear"></div>
            </div>
            <div id="reportContent"></div>
                   
            <div id="overlay"></div>                
   </script>
   <script type="text/template" id="PrimaryChartArea">
        <div class="chartArea">
            <div id="titleWrapper">
                <a href="#" class="chartAreaTitle" {% if (tooltip != "no tooltip") { %} title="{{ tooltip }}" {% } %} sectionId="{{ sectionId }}"><img class="sectionVisibility" src="<%: Url.UpdatedResourceLink("~/Content/images/minimize_square.png") %>" alt="Expand section" />{{ name }}</a>
                <p id="description{{uniqueId}}" class="sectionDescription invisible">Displays two dimensional data set</p>   
                <a href="#" class="toCsv{{ uniqueId }} exportBtn" title="<%: Resources.Global.RepExportToExcel %>"><img src="<%: Url.Content("~/Content/images/Excel-25.png") %>" width="25" height="25"/></a> 
        </div>
            <div class="chartAreaContent{{sectionId}}">
                <form action="">    
                <div id="granularitySelector{{ uniqueId }}" class="granularitySelector">
                    <div class="radioBtnWrapper active"><label for="day{{uniqueId}}" class="radioBtn"><input type="radio" id="day{{uniqueId}}" name="radio{{uniqueId}}" checked="checked" selectorId="{{ uniqueId }}" class="radioOption{{uniqueId}} radioOption" value="day"/><%: Resources.Global.RepDay %></label></div>
		            <div class="radioBtnWrapper"><label for="week{{uniqueId}}" class="radioBtn"><input type="radio" id="week{{uniqueId}}" name="radio{{uniqueId}}" selectorId="{{ uniqueId }}" class="radioOption{{uniqueId}} radioOption" value="week" /><%: Resources.Global.RepWeek %></label></div>
		            <div class="radioBtnWrapper"><label for="month{{uniqueId}}" class="radioBtn"><input type="radio" id="month{{uniqueId}}" name="radio{{uniqueId}}" selectorId="{{ uniqueId }}" class="radioOption{{uniqueId}} radioOption" value="month"/><%: Resources.Global.RepMonth %></label></div>
                </div>
                </form>
                
                <div id="chart_div{{uniqueId}}" class="chart_div"></div>
                <div class="clear"></div>
            </div>
         </div>     
   </script>
   <script type="text/template" id="InfoBox">
        <div id="infoBoxArea">
            <div class="clear"></div>
        </div>
   </script>
   <script type="text/template" id="SecondaryChartArea">
        <div id="tableArea" class="chartAreaContent{{sectionId}}">
            <div id="tableContent">
            </div>
            <div id="tableChart">
                <div id="comboChart_div"></div> 
            </div>
            <div class="clear"></div>
        </div>
   </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="LeftSideArea" runat="server">
   <select id='workingPointSelector'></select>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
   <script type="text/javascript">
      $(function () {
         var newGUI = new InitializeGUI();
      });
   </script>
   <input type="hidden" value="<%: ViewData["currentCulture"] %>" class="currentCulture" />
</asp:Content>
