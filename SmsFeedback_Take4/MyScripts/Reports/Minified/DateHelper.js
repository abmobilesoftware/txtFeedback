var DateHelper=function(){this.transformDate=function(c){var a=c.getDate();a=(a<10)?"0"+a:a;var d=c.getMonth()+1;d=(d<10)?"0"+d:d;var b=c.getFullYear();return b+"-"+d+"-"+a};this.transformDateToLocal=function(c){var b="DD, MM d, yy";if(window.app.calendarCulture==="ro"){b="DD, d MM, yy"}var a=$.datepicker.formatDate(b,c,{dayNamesShort:$.datepicker.regional[window.app.calendarCulture].dayNamesShort,dayNames:$.datepicker.regional[window.app.calendarCulture].dayNames,monthNamesShort:$.datepicker.regional[window.app.calendarCulture].monthNamesShort,monthNames:$.datepicker.regional[window.app.calendarCulture].monthNames});return a}};