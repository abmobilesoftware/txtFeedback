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

            <script src="<%: Url.UpdatedResourceLink("~/Scripts/spin.js") %>" type="application/javascript" ></script>
            <script type="text/javascript" src="https://www.google.com/jsapi"></script>

            

            <!-- Format date to local specific -->
            <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-de.js") %>" type="application/javascript"></script>
            <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-ro.js") %>" type="application/javascript"></script>
            <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-en-GB.js") %>" type="application/javascript"></script>
            
            <!-- Helper scripts - used like static methods -->
            <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/DateHelper.js") %>" type="application/javascript"></script>
            <!-- Global variables -->
            <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/GlobalVariables.js") %>" type="application/javascript"></script>

            <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/FirstArea.js") %>" type="application/javascript"></script>
            <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/SecondArea.js") %>" type="application/javascript"></script>
            <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/ThirdArea.js") %>" type="application/javascript"></script>
            <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/datepicker.js") %>"></script>
            <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/eye.js") %>"></script>
            <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/utils.js") %>"></script>
           
            
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Reports.js") %>" type="application/javascript"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Facade.js") %>" type="application/javascript"></script>
           
              
             <script type="text/javascript" src="<%: Url.Content("~/Scripts/CollapsibleLists.js") %>"></script>
            <script type="text/javascript">
                google.load('visualization', '1', { 'packages': ['corechart'] });
           </script>
           <script type="text/javascript" src="<%: Url.Content("~/Scripts/jquery-ui-vertical-buttonset.js") %>"></script>
           
           
           <link rel="stylesheet" href="http://code.jquery.com/ui/1.8.22/themes/base/jquery-ui.css" type="text/css" media="all" />
		   <link rel="stylesheet" href="http://static.jquery.com/ui/css/demo-docs-theme/ui.theme.css" type="text/css" media="all" />
            
               
    <% }      else      { %>
           <link rel="stylesheet" type="text/css" media="all" href="<%: Url.Content("~/Content/css/reports.css") %>" />
           <link rel="stylesheet" href="<%: Url.Content("~/Content/css/datepicker/datepicker.css") %>" type="text/css" />
           <link rel="stylesheet" media="screen" type="text/css" href="<%: Url.Content("~/Content/css/datepicker/layout.css") %>" />

            <!-- Cool spinner -->
            <script src="<%: Url.UpdatedResourceLink("~/Scripts/spin.js") %>" type="application/javascript" ></script>
            
            <!-- Google Ajax library, for loading other packages -->
            <script type="text/javascript" src="https://www.google.com/jsapi"></script>
            <!-- Range picker scripts -->
            <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/datepicker.js") %>"></script>
            <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/eye.js") %>"></script>
            <script type="text/javascript" src="<%: Url.Content("~/Scripts/datepicker/utils.js") %>"></script>       
     
            <!-- Format date to local specific -->
            <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-de.js") %>" type="application/javascript"></script>
            <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-ro.js") %>" type="application/javascript"></script>
            <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.ui.datepicker-en-GB.js") %>" type="application/javascript"></script>

            <!-- Helper scripts - used like static methods -->
            <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/DateHelper.js") %>" type="application/javascript"></script>
            <!-- Global variables -->
            <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/GlobalVariables.js") %>" type="application/javascript"></script>

            <!-- Reports page specific scripts -->
            <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/FirstArea.js") %>" type="application/javascript"></script>
            <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/SecondArea.js") %>" type="application/javascript"></script>
            <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/ThirdArea.js") %>" type="application/javascript"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Reports.js") %>" type="application/javascript"></script>
           <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Reports/Facade.js") %>" type="application/javascript"></script>
            
            <!-- Granularity vertical buttons -->
            <script type="text/javascript" src="<%: Url.Content("~/Scripts/jquery-ui-vertical-buttonset.js") %>"></script>
                      
            <!-- Left menu, collapsible menu -->
            <script type="text/javascript" src="<%: Url.Content("~/Scripts/CollapsibleLists.js") %>"></script>
           
            <!-- Early loading of the corechar package, it's used in charts -->
            <script type="text/javascript">
               google.load('visualization', '1', {'packages':['corechart']});
           </script>
           
                    

           <link rel="stylesheet" href="http://code.jquery.com/ui/1.8.22/themes/base/jquery-ui.css" type="text/css" media="all" /> 
		   <link rel="stylesheet" href="http://static.jquery.com/ui/css/demo-docs-theme/ui.theme.css" type="text/css" media="all" />
               
      <% } %>
   <!--<span class="bodyText">Work in progress</span>-->
   
    <script type="text/template" id="report-template">
               <div id="titleArea">
                <span id="reportTitle">{{ title }}</span>
                <div id="widgetWrapper">
                   <div id="widget">
		            <div id="widgetField">
                        <span> {{ window.app.dateHelper.transformDateToLocal(window.app.startDate) }} - {{ window.app.dateHelper.transformDateToLocal(window.app.endDate) }} </span>
				        <a href="#">Select date range</a>
			        </div>
			        <div id="widgetCalendar">
			        </div>
		           </div>
                </div>
                <div class="clear"></div>
            </div>

            {% for (i=0; i< sections.length; ++i) {
                    if ((sections[i].identifier == "PrimaryChartArea") && sections[i].visibility) {
            %}
                        <div id="chartArea">
                            <div id="radio">
                                <input type="radio" id="radio1" name="radio" checked="checked" class="radioOption" value="day"/><label for="radio1">Day</label>
		                        <input type="radio" id="radio2" name="radio" class="radioOption" value="week" /><label for="radio2">Week</label>
		                        <input type="radio" id="radio3" name="radio" class="radioOption" value="year"/><label for="radio3">Month</label>
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
    
    
  
    
    <div id="menuBar">
    </div>    
    <div id="leftColumn" class="grid1">
    <div class="styled-select">
        <select id="workingPointSelector">
            
        </select>
    </div>
           
    </div>
   <div id="rightColumn" class="grid2">
   
        
  </div>
   
     <script type="text/javascript">
         $(function () {
             var newGUI = new InitializeGUI();            
         });
   </script>
  
   <input type="hidden" value="<%: ViewData["currentCulture"] %>" class="currentCulture" />

</asp:Content>
