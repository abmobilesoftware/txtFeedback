function ThirdArea(c){var a=this;var f=c;var e=null;var b={};var d=new google.visualization.PieChart(document.getElementById("chart_div1"));this.drawArea=function(){var g=$.ajax({data:{iIntervalStart:window.app.dateHelper.transformDate(window.app.startDate),iIntervalEnd:window.app.dateHelper.transformDate(window.app.endDate),culture:window.app.calendarCulture,scope:window.app.currentWorkingPoint},url:app.domainName+f,dataType:"json",async:false,success:function(h){a.buildTable(h);a.drawChart(h)}}).responseText};this.drawChart=function(g){e=new google.visualization.DataTable(g);d.draw(e,b)};this.buildTable=function(l){var k="<table class='tbl'>";var g="<thead class='tblHead'>";for(i=0;i<l.cols.length;++i){g+="<td>"+l.cols[i].label+"</td>"}g+="</thead>";var h="";for(i=0;i<l.rows.length;++i){h+="<tr class='tblRow'>";for(j=0;j<l.rows[i].c.length;++j){h+="<td>"+l.rows[i].c[j].v+"</td>"}h+="</tr>"}k+=g+h+"</table>";$("#tableContent").html(k)}};