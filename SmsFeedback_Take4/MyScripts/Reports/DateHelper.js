var DateHelper = function () {
    // in format yyyy-mm-dd
    this.transformDate = function(date) {
        var curr_date = date.getDate();
        curr_date = (curr_date < 10) ? "0" + curr_date : curr_date;
        var curr_month = date.getMonth() + 1; //Months are zero based
        curr_month = (curr_month < 10) ? "0" + curr_month : curr_month;
        var curr_year = date.getFullYear();
        return curr_year + "-" + curr_month + "-" + curr_date;
    }

    this.transformDateToLocal = function (date) {
        var displayPattern = 'DD, MM d, yy';
        if (window.app.calendarCulture == "ro") displayPattern = 'DD, d MM, yy';
        var dateLocal = $.datepicker.formatDate(displayPattern, date,
                                    {
                                        dayNamesShort: $.datepicker.regional[window.app.calendarCulture].dayNamesShort, dayNames: $.datepicker.regional[window.app.calendarCulture].dayNames,
                                        monthNamesShort: $.datepicker.regional[window.app.calendarCulture].monthNamesShort, monthNames: $.datepicker.regional[window.app.calendarCulture].monthNames
                                    });
        return dateLocal;
    }
}