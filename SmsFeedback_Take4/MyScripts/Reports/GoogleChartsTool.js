function ChartArea() {
    // Load the Visualization API and the piechart package.
    google.load('visualization', '1.0', { 'packages': ['corechart'] });

    // Set a callback to run when the Google Visualization API is loaded.
    //google.setOnLoadCallback(function () { this.drawChart("15.07.2012", "30.07.2012", "day") });


    this.drawChart = function (intervalStart, intervalEnd, granularity) {
        /*var data = google.visualization.arrayToDataTable([
          ['Year', 'Incoming', 'Outgoing'],
          ['2004', 1000, 400],
          ['2005', 1170, 460],
          ['2006', 660, 1120],
          ['2007', 1030, 540]
        ]);
        */
        var jsonData = $.ajax({
            data: { iIntervalStart: intervalStart, iIntervalEnd: intervalEnd, iGranularity: granularity },
            url: app.domainName + "/Reports/getTotalNoOfSms",
            dataType: "json",
            async: false
        }).responseText;

        // Create our data table out of JSON data loaded from server.
        var data = new google.visualization.DataTable(jsonData);
        var options = {
            // hAxis: { title: 'Year', titleTextStyle: { color: 'black' } }
        };

        var chart = new google.visualization.AreaChart(document.getElementById('chart_div'));
        chart.draw(data, options);
    }
}