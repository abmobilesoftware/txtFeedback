function ThirdArea(c){var a=this;var f=c;var e=null;var b={backgroundColor:"#F5F8FA"};var d=new google.visualization.PieChart(document.getElementById("comboChart_div"));this.drawArea=function(){var g=$.ajax({data:{iIntervalStart:window.app.dateHelper.transformDate(window.app.startDate),iIntervalEnd:window.app.dateHelper.transformDate(window.app.endDate),culture:window.app.calendarCulture,scope:window.app.currentWorkingPoint},url:window.app.domainName+f,dataType:"json",async:false,success:function(h){a.buildTable(h);a.drawChart(h)}}).responseText};this.drawChart=function(g){e=new google.visualization.DataTable(g);d.draw(e,b)};this.buildTable=function(n){var m="<table class='tbl'>";var g="<thead class='tblHead'>";for(var l=0;l<n.cols.length;++l){g+="<td>"+n.cols[l].label+"</td>"}g+="</thead>";var k="";for(l=0;l<n.rows.length;++l){k+="<tr class='tblRow'>";for(var h=0;h<n.rows[l].c.length;++h){k+="<td>"+n.rows[l].c[h].v+"</td>"}k+="</tr>"}m+=g+k+"</table>";$("#tableContent").html(m)}};