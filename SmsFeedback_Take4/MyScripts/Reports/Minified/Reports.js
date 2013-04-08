window.app=window.app||{};window.reports=window.reports||{};window.app.dateFormatForDatePicker="dd/mm/yy";_.templateSettings={interpolate:/\{\{(.+?)\}\}/g,evaluate:/\{%([\s\S]+?)%\}/g,escape:/\{%-([\s\S]+?)%\}/g};function drawThisArea(a,b){a.drawArea()}var ReportModel=Backbone.Model.extend({reportId:1,title:"Total sms report",source:"/Reports/GetReportOverviewData",sections:[{type:"FirstSection",id:"4",groupId:"xt4ga",title:"Get total no of sms report",options:{seriesType:"area",colors:["#ccc7f1","#459aaa"]},tooltip:"no tooltip",dataIndex:1}]});var ReportsContentArea=Backbone.View.extend({el:$("#rightColumn"),initialize:function(){_.bindAll(this,"render","setupEnvironment","updateReport","renderSection");window.app.areas=[];this.FIRST_SECTION="FirstSection";this.SECOND_SECTION="SecondSection";this.THIRD_SECTION="ThirdSection";this.loadReportData()},render:function(){},loadReportData:function(){var a=this;window.app.areas=[];var c=_.template($("#report-template").html(),this.model.toJSON());$(this.el).html(c);$("#secondSection").empty();$("#reportScope").html(" :: "+window.app.currentWorkingPointFriendlyName);this.transition=new Transition(document.getElementById("rightColumn"),$("#overlay"));this.transition.startTransition();var b=$.ajax({data:{iIntervalStart:window.app.dateHelper.transformDate(window.app.startDate),iIntervalEnd:window.app.dateHelper.transformDate(window.app.endDate),iScope:window.app.currentWorkingPoint},url:window.app.domainName+a.model.get("source"),dataType:"json",async:false,success:function(e){for(var d=0;d<a.model.get("sections").length;++d){a.renderSection(a.model.get("sections")[d],e)}$("#secondSection").append("<div class='clear'></div>");a.setupEnvironment(false);a.transition.endTransition();$(document).trigger("resize")}}).responseText},renderSection:function(a,d){if(a.type===this.FIRST_SECTION){var c=_.template($("#"+a.type).html(),a);$("#firstSection").append(c);var e=new FirstArea(a);e.load(d.charts[a.dataIndex]);window.app.areas[a.id]=e}else{if(a.type===this.SECOND_SECTION){var b=new SecondArea(a);b.load(d.infoBoxes[a.dataIndex]);window.app.areas[a.id]=b}else{if(a.type===this.THIRD_SECTION){var c=_.template($("#"+a.type).html(),a);$("#firstSection").append(c);var f=new ThirdArea();f.load(d.charts[a.dataIndex]);window.app.areas[a.id]=f}}}},setupEnvironment:function(f){if(f){var h=$(".chartAreaTitle");h.qtip({content:h.attr("tooltiptitle"),position:{corner:{target:"bottomMiddle",tooltip:"topMiddle"}},style:"dark"})}$(".exportBtn").qtip({content:$(".exportBtn").attr("tooltiptitle"),position:{corner:{target:"leftMiddle",tooltip:"rightMiddle"}},style:"dark"});$(".chartAreaTitle").click(function(k){k.preventDefault();var j=$(this).attr("sectionId");var i=".chartAreaContent"+j;var l="#description"+j;if($(i).is(":visible")){$(i).hide();$(this).children(".sectionVisibility").attr("src","/Content/images/arrow_up_dblue_16.png");$(l).show();$(document).trigger("resize")}else{$(i).show();$(this).children(".sectionVisibility").attr("src","/Content/images/arrow_down_dblue_16.png");$(l).hide();$(document).trigger("resize")}});var e=$("#from");e.datepicker({defaultDate:"+1w",changeMonth:true,numberOfMonths:3,onSelect:function(j){window.app.newStartDate=e.datepicker("getDate");if(window.app.newStartDate!==window.app.startDate){window.app.startDate=window.app.newStartDate;window.app.endDate=window.app.newEndDate;var i=$.datepicker.formatDate(window.app.dateFormatForDatePicker,window.app.startDate);g.datepicker("option","minDate",i);$(document).trigger("intervalChanged")}}});var g=$("#to");g.datepicker({defaultDate:"+1w",changeMonth:true,numberOfMonths:3,onSelect:function(j){window.app.newEndDate=g.datepicker("getDate");if(window.app.newEndDate!==window.app.endDate){window.app.startDate=window.app.newStartDate;window.app.endDate=window.app.newEndDate;var i=$.datepicker.formatDate(window.app.dateFormatForDatePicker,window.app.endDate);e.datepicker("option","maxDate",i);$(document).trigger("intervalChanged")}}});var a=$.datepicker.formatDate(window.app.dateFormatForDatePicker,window.app.startDate);var d=$.datepicker.formatDate(window.app.dateFormatForDatePicker,window.app.endDate);var c=new Date();$.datepicker.regional[window.app.calendarCulture].dateFormat=window.app.dateFormatForDatePicker;var b=$.datepicker.regional[window.app.calendarCulture];$("#from").datepicker("option",b);$("#to").datepicker("option",b);$("#to").datepicker("option","minDate",a);$("#to").datepicker("option","maxDate",c);$("#from").datepicker("option","maxDate",d);$("#from").val(a);$("#to").val(d)},updateReport:function(){this.loadReportData();$("#reportScope").html(" :: "+window.app.currentWorkingPointFriendlyName)}});window.app.leftSideMenus={};var ReportsArea=function(){var b=this;var c={};var d=new window.app.MenuView({el:$("#leftColumn"),eventToTriggerOnSelect:"switchReport",menuCollection:new window.app.MenuCollection({url:"/Reports/getReportsMenuItems"}),afterInitializeFunction:function(f){_(f.models).each(function(g){if(g.get("parent")!=0){window.app.leftSideMenus[g.get("FriendlyName")]={id:g.get("itemId"),action:g.get("Action")}}});var e=Backbone.Router.extend({routes:{"":"defaultCall",":menu":"goToMenu"},goToMenu:function(h){var i=window.app.leftSideMenus[h];if(i!==undefined){var l=i.id;b.loadReport(l);var k=".liItem"+i.id;if(!$(k).hasClass("menuItemSelected")){$(k).parents(".collapsibleList").find(".menuItemSelected").removeClass("menuItemSelected");$(k).addClass("menuItemSelected");var j=Math.floor(i.id/10)*10;var g="ul.item"+j;$(g).css("display","block")}}else{this.defaultCall()}},defaultCall:function(){$(".liItem11").addClass("menuItemSelected");$("ul.item10").css("display","block");window.reports.router.navigate("/ConversationsOverview",{trigger:true})}});window.reports.router=new e();Backbone.history.start()}});var a;$(document).bind("workingPointChanged",function(f,e){b.changeWorkingPoint(e)});$(document).bind("intervalChanged",function(e){b.changeReportingInterval()});$(document).bind("switchReport",function(e,f){window.reports.router.navigate("/"+f.menuNavigation,{trigger:true})});this.redrawContent=function(){a.loadReportData()};this.changeWorkingPoint=function(e){window.app.currentWorkingPoint=e;a.updateReport()};this.changeReportingInterval=function(){$("from").datepicker("hide");$("to").datepicker("hide");a.updateReport()};this.displayReport=function(e){a=new ReportsContentArea({model:e,el:$("#rightColumn")})};this.loadReport=function(e){if(c[e]===undefined){$.getJSON("Reports/getReportById",{reportId:e},function(g){var f=new ReportModel(g);c[e]=f;b.displayReport(c[e])})}else{b.displayReport(c[e])}};this.loadWorkingPoints=function(){$.getJSON(window.app.domainName+"/WorkingPoints/WorkingPointsPerUser",{},function(g){var e="<option value='Global'>Global</option>";for(var f=0;f<g.length;++f){e+="<option value='"+g[f].TelNumber+"'>"+g[f].Name+"</option>"}$("#workingPointSelector").append(e);$("#workingPointSelector").val("Global");$("#workingPointSelector").change(function(){window.app.currentWorkingPointFriendlyName=$("#workingPointSelector").children("option").filter(":selected").text();$(document).trigger("workingPointChanged",$(this).val())})})};$("#overlay").hide();this.loadWorkingPoints()};