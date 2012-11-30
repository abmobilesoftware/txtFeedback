window.app=window.app||{};window.app.dateFormatForDatePicker="dd/mm/yy";_.templateSettings={interpolate:/\{\{(.+?)\}\}/g,evaluate:/\{%([\s\S]+?)%\}/g,escape:/\{%-([\s\S]+?)%\}/g};function drawThisArea(a,b){a.drawArea()}var ReportModel=Backbone.Model.extend({menuId:1,title:"Total sms report",scope:"Global",sections:[{identifier:"PrimaryChartArea",visibility:true,resources:[{name:"Get total no of sms report",source:"/Reports/getTotalNoOfSms"}]},{identifier:"InfoBox",visibility:true,resources:[{name:"Total no of sms",source:"Reports/getNoOfSms"}]},{identifier:"AdditionalChartArea",visibility:false,resources:[]}]});var Transition=function(){var a={lines:13,length:7,width:4,radius:10,rotate:0,color:"#fff",speed:1,trail:60,shadow:true,hwaccel:false,className:"spinner",zIndex:2000000000,top:"auto",left:"auto"};var c=new Spinner(a);var b=document.getElementById("chartArea");this.startTransition=function(){c.spin(b);$("#overlay").show()};this.endTransition=function(){c.stop();$("#overlay").hide()}};var ReportsContentArea=Backbone.View.extend({el:$("#rightColumn"),initialize:function(){_.bindAll(this,"render","setupEnvironment","updateReport","renderSection");this.reportContentElement=$("#reportContent");window.app.areas=[];this.render()},render:function(){this.transition=new Transition();this.transition.startTransition();var c=_.template($("#report-template").html(),this.model.toJSON());$(this.el).html(c);$("#reportScope").html(" :: "+window.app.currentWorkingPointFriendlyName);var b=false;for(var a=0;a<this.model.get("sections").length;++a){if(this.model.get("sections")[a].visibility){this.renderSection("#"+this.model.get("sections")[a].identifier,this.model.get("sections")[a].uniqueId,this.model.get("sections")[a].sectionId,this.model.get("sections")[a].resources);if(this.model.get("sections")[a].resources[0].tooltip!=="no tooltip"){b=true}}}this.setupEnvironment(b);this.transition.endTransition();$(document).trigger("resize")},renderSection:function(f,g,d,e){var c=e[0];c.uniqueId=g;c.sectionId=d;var a=_.template($(f).html(),c);$("#reportContent").append(a);if(f==="#PrimaryChartArea"){var b=new FirstArea(e[0].source,"day",e[0].options,g,e[0].tooltip,e[0].name);b.drawArea();window.app.areas[g]=b}else{if(f==="#SecondaryChartArea"){window.app.thirdArea=new ThirdArea(e[0].source);window.app.thirdArea.drawArea();window.app.areas[g]=window.app.thirdArea}else{if(f==="#InfoBox"){window.app.secondArea=new SecondArea(e);window.app.secondArea.drawArea();window.app.areas[g]=window.app.secondArea}}}},setupEnvironment:function(f){if(f){var h=$(".chartAreaTitle");h.qtip({content:h.attr("tooltiptitle"),position:{corner:{target:"bottomMiddle",tooltip:"topMiddle"}},style:"dark"})}$(".chartAreaTitle").click(function(k){k.preventDefault();var j=$(this).attr("sectionId");var i=".chartAreaContent"+j;var l="#description"+j;if($(i).is(":visible")){$(i).hide();$(this).children(".sectionVisibility").attr("src","/Content/images/maximize_square.png");$(l).show();$(document).trigger("resize")}else{$(i).show();$(this).children(".sectionVisibility").attr("src","/Content/images/minimize_square.png");$(l).hide();$(document).trigger("resize")}});var e=$("#from");e.datepicker({defaultDate:"+1w",changeMonth:true,numberOfMonths:3,onSelect:function(j){window.app.newStartDate=e.datepicker("getDate");if(window.app.newStartDate!==window.app.startDate){window.app.startDate=window.app.newStartDate;window.app.endDate=window.app.newEndDate;var i=$.datepicker.formatDate(window.app.dateFormatForDatePicker,window.app.startDate);g.datepicker("option","minDate",i);$(document).trigger("intervalChanged")}}});var g=$("#to");g.datepicker({defaultDate:"+1w",changeMonth:true,numberOfMonths:3,onSelect:function(j){window.app.newEndDate=g.datepicker("getDate");if(window.app.newEndDate!==window.app.endDate){window.app.startDate=window.app.newStartDate;window.app.endDate=window.app.newEndDate;var i=$.datepicker.formatDate(window.app.dateFormatForDatePicker,window.app.endDate);e.datepicker("option","maxDate",i);$(document).trigger("intervalChanged")}}});var a=$.datepicker.formatDate(window.app.dateFormatForDatePicker,window.app.startDate);var d=$.datepicker.formatDate(window.app.dateFormatForDatePicker,window.app.endDate);var c=new Date();$.datepicker.regional[window.app.calendarCulture].dateFormat=window.app.dateFormatForDatePicker;var b=$.datepicker.regional[window.app.calendarCulture];$("#from").datepicker("option",b);$("#to").datepicker("option",b);$("#to").datepicker("option","minDate",a);$("#to").datepicker("option","maxDate",c);$("#from").datepicker("option","maxDate",d);$("#from").val(a);$("#to").val(d)},updateReport:function(){$("#reportScope").html(" :: "+window.app.currentWorkingPointFriendlyName);window.app.areas.forEach(drawThisArea)}});var ReportsArea=function(){var b=this;var c={};var d=new window.app.MenuView({el:$("#leftColumn"),eventToTriggerOnSelect:"switchReport",menuCollection:new window.app.MenuCollection({url:"/Reports/getReportsMenuItems"}),afterInitializeFunction:function(){$(".liItem2").addClass("menuItemSelected");$("ul.item1").css("display","block")}});var a;$(document).bind("workingPointChanged",function(f,e){b.changeWorkingPoint(e)});$(document).bind("intervalChanged",function(e){b.changeReportingInterval()});$(document).bind("switchReport",function(e,f){b.loadReport(f)});this.redrawContent=function(){a.render()};this.changeWorkingPoint=function(e){window.app.currentWorkingPoint=e;a.updateReport()};this.changeReportingInterval=function(){$("from").datepicker("hide");$("to").datepicker("hide");a.updateReport()};this.displayReport=function(e){a=new ReportsContentArea({model:e,el:$("#rightColumn")})};this.loadReport=function(e){if(c[e]===undefined){$.getJSON("Reports/getReportById",{reportId:e},function(g){var f=new ReportModel(g);c[e]=f;b.displayReport(c[e])})}else{b.displayReport(c[e])}};this.loadWorkingPoints=function(){$.getJSON(window.app.domainName+"/Messages/WorkingPointsPerUser",{},function(g){var e="<option value='Global'>Global</option>";for(var f=0;f<g.length;++f){e+="<option value='"+g[f].TelNumber+"'>"+g[f].Name+"</option>"}$("#workingPointSelector").append(e);$("#workingPointSelector").val("Global");$("#workingPointSelector").change(function(){window.app.currentWorkingPointFriendlyName=$("#workingPointSelector").children("option").filter(":selected").text();$(document).trigger("workingPointChanged",$(this).val())})})};this.loadReport(2);$("#overlay").hide();this.loadWorkingPoints()};