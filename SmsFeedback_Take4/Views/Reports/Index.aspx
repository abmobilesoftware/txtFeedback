<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   <%: Resources.Global.reportsPageTitle %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="IncludesCssArea" runat="server">
       <%: Styles.Render("~/Content/reportsCss") %>   
</asp:Content>
<asp:Content ID="Content15" ContentPlaceHolderID="IncludesJsArea" runat="server">   
      <!-- Google Ajax library, for loading other packages -->
      <script type="text/javascript" src='https://www.google.com/jsapi?autoload={"modules":[{"name":"visualization","version":"1","packages":["corechart","table"]}]}'></script>
      <%:Scripts.Render("~/bundles/reportsjs")%>    
    <script type="text/javascript">
       $(function () {
          var newGUI = new InitializeGUI();
       });
   </script>
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
            <div id="reportContent">
                <div id="firstSection">
                </div>
                <div id="secondSection">
                    <div class="clear"></div>
                </div>
                <div id="thirdSection">
                </div>                 
            </div>
                   
            <div id="overlay"></div>                
   </script>
   <script type="text/template" id="FirstSection">
        <div class="chartArea">
            <div id="titleWrapper{{id}}">
                <a href="#" class="chartAreaTitle" {% if (tooltip != "no tooltip") { %} title="{{ tooltip }}" {% } %} sectionId="{{ groupId }}"><img class="sectionVisibility" src="<%: Url.Content("~/Content/images/arrow_down_dblue_16.png") %>" alt="Expand section" />{{ title }}</a>
                <p id="description{{id}}" class="sectionDescription invisible">Displays two dimensional data set</p>   
                <a href="#" class="toCsv{{ id }} exportBtn" title="<%: Resources.Global.RepExportToExcel %>"><img src="<%: Url.Content("~/Content/images/Excel-25_chenar.png") %>" width="25" height="25"/></a> 
                {% if (hasExportRawData) { %}
                <a href="#" class="exportRawBtn" title="<%: Resources.Global.RepExportRawToExcel %>"><img src="<%: Url.Content("~/Content/images/database-Chart-25_chenar.png") %>" width="25" height="25"/></a> 
                <div type="hidded" style="disply:none" id="exportrawreport"></div>
               {% } %}
        </div>
            <div class="chartAreaContent{{groupId}}">
                <form action="">    
                <div id="granularitySelector{{ id }}" class="granularitySelector">
                    <div class="radioBtnWrapper active"><label for="day{{id}}" class="radioBtn"><input type="radio" id="day{{id}}" name="radio{{id}}" checked="checked" selectorId="{{ id }}" class="radioOption{{id}} radioOption" value="day"/><%: Resources.Global.RepDay %></label></div>
		            <div class="radioBtnWrapper"><label for="week{{id}}" class="radioBtn"><input type="radio" id="week{{id}}" name="radio{{id}}" selectorId="{{ id }}" class="radioOption{{id}} radioOption" value="week" /><%: Resources.Global.RepWeek %></label></div>
		            <div class="radioBtnWrapper"><label for="month{{id}}" class="radioBtn"><input type="radio" id="month{{id}}" name="radio{{id}}" selectorId="{{ id }}" class="radioOption{{id}} radioOption" value="month"/><%: Resources.Global.RepMonth %></label></div>
                </div>
                </form>
                
                <div id="chart_div{{id}}" class="chart_div"></div>
                <div class="clear"></div>
            </div>
         </div>     
   </script>   
    <script type="text/template" id="TagsReportSection">
       <div class="chartArea">
       
          <div id="titleWrapper{{id}}">
                <a href="#" class="chartAreaTitle" {% if (tooltip != "no tooltip") { %} title="{{ tooltip }}" {% } %} sectionId="{{ groupId }}"><img class="sectionVisibility" src="<%: Url.Content("~/Content/images/arrow_down_dblue_16.png") %>" alt="Expand section" />{{ title }}</a>
                <p id="description{{id}}" class="sectionDescription invisible">Displays two dimensional data set</p>   
             
                <a href="#" class="toCsv{{ id }} exportBtn" title="<%: Resources.Global.RepExportToExcel %>"><img src="<%: Url.Content("~/Content/images/Excel-25_chenar.png") %>" width="25" height="25"/></a> 
                {% if (hasExportRawData) { %}
                <a href="#" class="exportRawBtn" title="<%: Resources.Global.RepExportRawToExcel %>"><img src="<%: Url.Content("~/Content/images/database-Chart-25_chenar.png") %>" width="25" height="25"/></a> 
                <div type="hidded" style="disply:none" id="exportrawreport"></div>
               {% } %}
         </div>
          
            <div class="chartAreaContent{{groupId}}">
                <div class="tagFilterArea">
             <div id="tagFilteringReports" class="filterInputBox">
                <input name="filterTagReports" id="filterTagReports" />
                <input type="hidden" value="<%: Resources.Global.addATagLabel %>" class="filterLabel" />
                <input type="hidden" value="<%: Resources.Global.filteringAddFilterTag %>" id="filteringAddFilterTagMessage" />
                <input type="hidden" value="<%: Resources.Global.messagesRemoveTagPlaceHolder %>" id="messagesRemoveTagPlaceHolderMessage" />
             </div>
             <button class="refreshTagReport btn btn-info">Refresh</button>
            </div>

                <form action="">    
                <div id="granularitySelector{{ id }}" class="granularitySelector">
                    <div class="radioBtnWrapper active"><label for="day{{id}}" class="radioBtn"><input type="radio" id="day{{id}}" name="radio{{id}}" checked="checked" selectorId="{{ id }}" class="radioOption{{id}} radioOption" value="day"/><%: Resources.Global.RepDay %></label></div>
		            <div class="radioBtnWrapper"><label for="week{{id}}" class="radioBtn"><input type="radio" id="week{{id}}" name="radio{{id}}" selectorId="{{ id }}" class="radioOption{{id}} radioOption" value="week" /><%: Resources.Global.RepWeek %></label></div>
		            <div class="radioBtnWrapper"><label for="month{{id}}" class="radioBtn"><input type="radio" id="month{{id}}" name="radio{{id}}" selectorId="{{ id }}" class="radioOption{{id}} radioOption" value="month"/><%: Resources.Global.RepMonth %></label></div>
                </div>
                </form>
                
                <div id="chart_div{{id}}" class="chart_div"></div>
                <div class="clear"></div>
            </div>
         </div>     
   </script>   
   <script type="text/template" id="ThirdSection">
        <div id="tableArea" class="chartAreaContent{{groupId}}">
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
  
   <input type="hidden" value="<%: ViewData["currentCulture"] %>" class="currentCulture" />
</asp:Content>
