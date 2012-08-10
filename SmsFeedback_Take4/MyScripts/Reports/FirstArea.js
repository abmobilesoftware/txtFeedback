window.app = window.app || {};

function FirstArea(iResource, iGranularity) {
        
    var resource = iResource;
    var granularity = iGranularity;
    var opts = {
        lines: 13, // The number of lines to draw
        length: 7, // The length of each line
        width: 4, // The line thickness
        radius: 10, // The radius of the inner circle
        rotate: 0, // The rotation offset
        color: '#fff', // #rgb or #rrggbb
        speed: 1, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: true, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: 'auto', // Top position relative to parent in px
        left: 'auto' // Left position relative to parent in px
    };
    var spinner = new Spinner(opts);
    var target = document.getElementById('overlay');
    var self = this;

    this.drawArea = function () {
        //spinner.spin(target);
        $("#overlay").show();
        var jsonData = $.ajax({
            data: { iIntervalStart: window.app.dateHelper.transformDate(window.app.startDate), iIntervalEnd: window.app.dateHelper.transformDate(window.app.endDate), iGranularity: granularity, culture: window.app.calendarCulture, scope: window.app.currentWorkingPoint },
            url: app.domainName + resource,
            dataType: "json",
            async: false,
            success: function (data) {
                self.drawChart(data);
                //spinner.stop();
                $("#overlay").hide();
            }
        }).responseText;

        //this.drawChart(jsonData);
    }

    this.drawChart = function(jsonData) {
        // Create our data table out of JSON data loaded from server.
        var data = new google.visualization.DataTable(jsonData);
        var options = {
            // hAxis: { title: 'Year', titleTextStyle: { color: 'black' } }
        };
        
        var chart = new google.visualization.AreaChart(document.getElementById('chart_div'));
        chart.draw(data, options);
    }

    this.setGranularity = function (iGranularity) {
        granularity = iGranularity;
    }

}