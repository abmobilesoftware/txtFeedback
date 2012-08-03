<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Reports
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
      <% if (Html.IsReleaseBuild())      { %>
           <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/css/reports.css") %>" />
           <link rel="stylesheet" href="<%: Url.Content("~/Content/css/datepicker/datepicker.css") %>" type="text/css" />
           <link rel="stylesheet" media="screen" type="text/css" href="<%: Url.Content("~/Content/css/datepicker/layout.css") %>" />

           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Facade.js") %>" type="application/javascript"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Report.js") %>" type="application/javascript"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Reports.js") %>" type="application/javascript"></script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/datepicker.js") %>"></script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/eye.js") %>"></script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/utils.js") %>"></script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/jquery-ui-vertical-buttonset.js") %>"></script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/CollapsibleLists.js") %>"></script>

           <script type="text/javascript" src="https://www.google.com/jsapi"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/GoogleChartsTool.js") %>" type="application/javascript"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/GooglePieChart.js") %>" type="application/javascript"></script>

           <link rel="stylesheet" href="http://code.jquery.com/ui/1.8.22/themes/base/jquery-ui.css" type="text/css" media="all" />
		   <link rel="stylesheet" href="http://static.jquery.com/ui/css/demo-docs-theme/ui.theme.css" type="text/css" media="all" />
            
               
    <% }      else      { %>
           <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/css/reports.css") %>" />
           <link rel="stylesheet" href="<%: Url.Content("~/Content/css/datepicker/datepicker.css") %>" type="text/css" />
           <link rel="stylesheet" media="screen" type="text/css" href="<%: Url.Content("~/Content/css/datepicker/layout.css") %>" />

           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Facade.js") %>" type="application/javascript"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Report.js") %>" type="application/javascript"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Reports.js") %>" type="application/javascript"></script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/datepicker.js") %>"></script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/eye.js") %>"></script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/utils.js") %>"></script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/jquery-ui-vertical-buttonset.js") %>"></script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/CollapsibleLists.js") %>"></script>
           
           <script type="text/javascript" src="https://www.google.com/jsapi"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/GoogleChartsTool.js") %>" type="application/javascript"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/GooglePieChart.js") %>" type="application/javascript"></script>

           <link rel="stylesheet" href="http://code.jquery.com/ui/1.8.22/themes/base/jquery-ui.css" type="text/css" media="all" /> 
		   <link rel="stylesheet" href="http://static.jquery.com/ui/css/demo-docs-theme/ui.theme.css" type="text/css" media="all" />
               
      <% } %>
   <!--<span class="bodyText">Work in progress</span>-->
   
    
     <div id="menuBar">
        
    </div>    
    <div id="leftColumn" class="grid1">
    <div class="styled-select">
        <select>
            <option>Global</option>
            <option>Working point 1</option>
            <option>Working point 2</option>
        </select>
    </div>
           
    </div>
   <div id="rightColumn" class="grid2">
    
       <div id="titleArea">
        <span id="reportTitle">Incoming vs Outgoings</span>
        <div id="widgetWrapper">
           <div id="widget">
		    <div id="widgetField">
			    <span>28 July, 2008 &divide; 31 July, 2008</span>
				<a href="#">Select date range</a>
			</div>
			<div id="widgetCalendar">
			</div>
		   </div>
        </div>
        <div class="clear"></div>
    </div>
    <div id="chartArea">
        <div id="radio">
            <input type="radio" id="radio1" name="radio" checked="checked" class="radioOption" value="day"/><label for="radio1">Day</label>
		    <input type="radio" id="radio2" name="radio" class="radioOption" value="week" /><label for="radio2">Week</label>
		    <input type="radio" id="radio3" name="radio" class="radioOption" value="year"/><label for="radio3">Month</label>
        </div>
        <div id="chart_div"></div>
        <div class="clear"></div>
     </div>
     <div id="infoBoxArea">
         <div class="boxArea">
            <div class="infoTitle">No of sms</div>
            <div class="infoContent">320</div>
         </div>
         <div class="boxArea">
            <div class="infoTitle">Average no of sms per day</div>
            <div class="infoContent">5</div>
         </div>
         <div class="boxArea">
            <div class="infoTitle">Number of new clients</div>
            <div class="infoContent">30</div>
         </div>
         <div class="boxArea">
            <div class="infoTitle">Average no of sms per client</div>
            <div class="infoContent">6</div>
         </div>
         <div class="boxArea">
            <div class="infoTitle">Average response time</div>
            <div class="infoContent">6 minutes</div>
         </div>
         <div class="clear"></div>
     </div>
     <div id="tableArea">
         <div id="tableContent">
             <table class="tbl">
                 <thead class="tblHead">
                     <td>Sms type</td>
                     <td>No of sms</td>
                     <td>Percentage</td>
                 </thead>
                  <tr class="tblRow">
                      <td>Incoming</td>
                      <td>568</td>
                      <td>44.2</td>                 
                 </tr>
                 <tr class="tblRow">
                      <td>Outgoing</td>
                      <td>651</td>
                      <td>55.8</td>                 
                 </tr>

             </table>
          </div>
         <div id="tableChart">
            <div id="chart_div1"></div> 
         </div>
     </div>
   </div>

    <script type="text/javascript">
        $(function () {
            var newGUI = new InitializeGUI();            
        });
   </script>
   <input type="hidden" value="<%: ViewData["currentCulture"] %>" class="currentCulture" />

</asp:Content>
