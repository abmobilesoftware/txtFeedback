window.app=window.app||{};function FirstArea(f){var b=this;var e=null;var g=f.title;var a=f.chartSource;var d={animation:{duration:1000,easing:"out"},vAxis:{gridlines:{count:4}},backgroundColor:"#F5F8FA"};if(f.options!=null){d.seriesType=f.options.seriesType;d.colors=f.options.colors}var c=f.id;$(".radioOption"+c).change(function(h){$(this).parents("#granularitySelector"+c).find(".active").removeClass("active");$(this).parents(".radioBtnWrapper").addClass("active");window.app.areas[c].loadWithGranularity($(this).val())});$(".toCsv"+c).click(function(h){h.preventDefault();DownloadJSON2CSV(JSON.stringify(e),g)});this.loadWithGranularity=function(i){b=this;var h=$.ajax({data:{iIntervalStart:window.app.dateHelper.transformDate(window.app.startDate),iIntervalEnd:window.app.dateHelper.transformDate(window.app.endDate),iScope:window.app.currentWorkingPoint,iGranularity:i},url:window.app.domainName+a,dataType:"json",async:false,success:function(j){b.load(j)}}).responseText};this.load=function(j){$(".granularitySelector").show();if(d.seriesType==="bars"){$(".granularitySelector").hide()}var h;if(d.seriesType==undefined){h=new google.visualization.AreaChart(document.getElementById("chart_div"+c));d.pointSize=6}else{if(d.seriesType==="area"){h=new google.visualization.AreaChart(document.getElementById("chart_div"+c));d.pointSize=6}else{if(d.seriesType==="bars"){h=new google.visualization.ComboChart(document.getElementById("chart_div"+c))}}}e=j;var i=new google.visualization.DataTable(e);h.draw(i,d)};this.setGranularity=function(h){granularity=h}};