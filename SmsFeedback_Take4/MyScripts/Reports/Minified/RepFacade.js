function updateChartsDimensions(){window.app.reportsPage.redrawContent()}function InitializeGUI(){$(window).smartresize(function(){updateChartsDimensions()});resizeTriggered()}$(document).ready(function(){var a=$(".currentCulture").val().substring(0,2).toLowerCase();if(a==="en"){window.app.calendarCulture="en-GB"}else{window.app.calendarCulture=a}window.app.reportsPage=new ReportsArea();$(document).bind("resize",resizeTriggered)});