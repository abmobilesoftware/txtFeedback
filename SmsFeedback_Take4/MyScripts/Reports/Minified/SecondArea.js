window.app=window.app||{};function SecondArea(c){var b=c;var a=this;this.drawArea=function(){$("#infoBoxArea").empty();for(i=0;i<b.length;++i){this.buildInfoBox(b[i].name,b[i].source,b[i].tooltip)}$("#infoBoxArea").append("<div class='clear'></div>")};this.buildInfoBox=function(d,g,f){var e=$.ajax({data:{iIntervalStart:window.app.dateHelper.transformDate(window.app.startDate),iIntervalEnd:window.app.dateHelper.transformDate(window.app.endDate),culture:window.app.calendarCulture,scope:window.app.currentWorkingPoint},url:app.domainName+g,dataType:"json",async:false,success:function(h){a.fillInfoBox(d,f,h)}}).responseText};this.fillInfoBox=function(d,g,f){var e="<div class='boxArea' title='"+g+"'><div class='infoContent'><div class='infoContentMiddle'><div class='infoContentInner'><span class='boxContent'><span class='boxValue'>"+f.value+"</span><span class='boxUnit'> "+f.unit+"</span></span></div></div></div><div class='infoTitle'><div class='infoTitleMiddle'><div class='infoTitleInner'><span class='boxTitle'>"+d+"</span></div></div></div></div>";$("#infoBoxArea").append(e)}};