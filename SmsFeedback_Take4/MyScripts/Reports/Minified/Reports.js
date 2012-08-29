"use strict";window.app=window.app||{};window.app.dateFormatForDatePicker="dd/mm/yy";_.templateSettings={interpolate:/\{\{(.+?)\}\}/g,evaluate:/\{%([\s\S]+?)%\}/g,escape:/\{%-([\s\S]+?)%\}/g};var ReportModel=Backbone.Model.extend({menuId:1,title:"Total sms report",scope:"Global",sections:[{identifier:"PrimaryChartArea",visibility:true,resources:[{name:"Get total no of sms report",source:"/Reports/getTotalNoOfSms"}]},{identifier:"InfoBox",visibility:true,resources:[{name:"Total no of sms",source:"Reports/getNoOfSms"}]},{identifier:"AdditionalChartArea",visibility:false,resources:[]}]});var ReportsContentArea=Backbone.View.extend({el:$("#rightColumn"),initialize:function(){_.bindAll(this,"render","setupEnvironment","updateReport");this.render()},render:function(){var a=_.template($("#report-template").html(),this.model.toJSON());$(this.el).html(a);$("#reportScope").html(" :: "+window.app.currentWorkingPointFriendlyName);this.setupEnvironment();$(document).trigger("resize")},setupEnvironment:function(){this.transition=new Transition();this.transition.startTransition();window.app.areas=[];for(var g=0;g<this.model.get("sections").length;++g){if(this.model.get("sections")[g].visibility&&(this.model.get("sections")[g].identifier==="PrimaryChartArea")){window.app.firstArea=new FirstArea(this.model.get("sections")[g].resources[0].source,"day",this.model.get("sections")[g].resources[0].options);window.app.firstArea.drawArea();window.app.areas.push(window.app.firstArea)}else{if(this.model.get("sections")[g].visibility&&(this.model.get("sections")[g].identifier=="SecondaryChartArea")){window.app.thirdArea=new ThirdArea(this.model.get("sections")[g].resources[0].source);window.app.thirdArea.drawArea();window.app.areas.push(window.app.thirdArea)}else{if(this.model.get("sections")[g].visibility&&(this.model.get("sections")[g].identifier==="InfoBox")){window.app.secondArea=new SecondArea(this.model.get("sections")[g].resources);window.app.secondArea.drawArea();window.app.areas.push(window.app.secondArea)}}}}var f=$("#from");f.datepicker({defaultDate:"+1w",changeMonth:true,numberOfMonths:3,onSelect:function(k){window.app.newStartDate=f.datepicker("getDate");if(window.app.newStartDate!=window.app.startDate){window.app.startDate=window.app.newStartDate;window.app.endDate=window.app.newEndDate;var j=$.datepicker.formatDate(app.dateFormatForDatePicker,window.app.startDate);h.datepicker("option","minDate",j);$(document).trigger("intervalChanged")}}});var h=$("#to");h.datepicker({defaultDate:"+1w",changeMonth:true,numberOfMonths:3,onSelect:function(k){window.app.newEndDate=h.datepicker("getDate");if(window.app.newEndDate!=window.app.endDate){window.app.startDate=window.app.newStartDate;window.app.endDate=window.app.newEndDate;var j=$.datepicker.formatDate(app.dateFormatForDatePicker,window.app.endDate);f.datepicker("option","maxDate",j);$(document).trigger("intervalChanged")}}});var b=$.datepicker.formatDate(app.dateFormatForDatePicker,window.app.startDate);var e=$.datepicker.formatDate(app.dateFormatForDatePicker,window.app.endDate);var d=new Date();var a=$.datepicker.formatDate(app.dateFormatForDatePicker,d);$.datepicker.regional[window.app.calendarCulture].dateFormat=window.app.dateFormatForDatePicker;var c=$.datepicker.regional[window.app.calendarCulture];$("#from").datepicker("option",c);$("#to").datepicker("option",c);$("#to").datepicker("option","minDate",b);$("#to").datepicker("option","maxDate",d);$("#from").datepicker("option","maxDate",e);$("#from").val(b);$("#to").val(e);$(".radioOption").change(function(){$(this).parents("#granularitySelector").find(".active").removeClass("active");$(this).parents(".radioBtnWrapper").addClass("active");window.app.firstArea.setGranularity($(this).val());window.app.firstArea.drawArea()});this.transition.endTransition()},updateReport:function(){$("#reportScope").html(" :: "+window.app.currentWorkingPointFriendlyName);for(i=0;i<window.app.areas.length;++i){window.app.areas[i].drawArea()}}});var Transition=function(){var a={lines:13,length:7,width:4,radius:10,rotate:0,color:"#fff",speed:1,trail:60,shadow:true,hwaccel:false,className:"spinner",zIndex:2000000000,top:"auto",left:"auto"};var c=new Spinner(a);var b=document.getElementById("chartArea");this.startTransition=function(){c.spin(b);$("#overlay").show()};this.endTransition=function(){c.stop();$("#overlay").hide()}};var ReportsArea=function(){var b=this;var c={};var d=new window.app.MenuView({el:$("#leftColumn"),eventToTriggerOnSelect:"switchReport",menuCollection:new window.app.MenuCollection({url:"/Reports/getReportsMenuItems"}),afterInitializeFunction:function(){$(".liItem2").addClass("menuItemSelected");$("ul.item1").css("display","block")}});var a;$(document).bind("workingPointChanged",function(f,e){b.changeWorkingPoint(e)});$(document).bind("intervalChanged",function(e){b.changeReportingInterval()});$(document).bind("switchReport",function(e,f){b.loadReport(f)});this.redrawContent=function(){a.render()};this.changeWorkingPoint=function(e){window.app.currentWorkingPoint=e;a.updateReport()};this.changeReportingInterval=function(){$("from").datepicker("hide");$("to").datepicker("hide");a.updateReport()};this.displayReport=function(e){a=new ReportsContentArea({model:e,el:$("#rightColumn")})};this.loadReport=function(e){if(c[e]===undefined){$.getJSON("Reports/getReportById",{reportId:e},function(g){var f=new ReportModel(g);c[e]=f;b.displayReport(c[e])})}else{b.displayReport(c[e])}};this.loadWorkingPoints=function(){$.getJSON(window.app.domainName+"/Messages/WorkingPointsPerUser",{},function(g){var e="<option value='Global'>Global</option>";for(var f=0;f<g.length;++f){e+="<option value='"+g[f].TelNumber+"'>"+g[f].Name+"</option>"}$("#workingPointSelector").append(e);$("#workingPointSelector").val("Global");$("#workingPointSelector").change(function(){window.app.currentWorkingPointFriendlyName=$("#workingPointSelector").children("option").filter(":selected").text();$(document).trigger("workingPointChanged",$(this).val())})})};this.loadReport(2);$("#overlay").hide();this.loadWorkingPoints()};