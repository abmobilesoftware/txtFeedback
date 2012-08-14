function ThirdArea(iResource) {
    var self = this;
    var resource = iResource;
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
    
    var data = null;
    var options = {
        title: 'Incoming vs Outgoing sms'
    };

    var chart = new google.visualization.PieChart(document.getElementById('chart_div1'));

    this.drawArea = function () {
        //spinner.spin(target);
        $("#overlay").show();
        var jsonData = $.ajax({
            data: { iIntervalStart: window.app.dateHelper.transformDate(window.app.startDate), iIntervalEnd: window.app.dateHelper.transformDate(window.app.endDate), culture: window.app.calendarCulture, scope: window.app.currentWorkingPoint },
            url: app.domainName + resource,
            dataType: "json",
            async: false,
            success: function (data) {
                self.buildTable(data);
                self.drawChart(data);
                //spinner.stop();
                $("#overlay").hide();
            }
        }).responseText;
               
    }

    this.drawChart = function(jsonData) {
        // Create our data table out of JSON data loaded from server.
        data = new google.visualization.DataTable(jsonData);
        chart.draw(data, options);
    }

    this.buildTable = function(data) {
        
        var tableString = "<table class='tbl'>";

        var tableHeader = "<thead class='tblHead'>";
        for (i = 0; i < data.cols.length; ++i) {
            tableHeader += "<td>" + data.cols[i].label + "</td>";
        }
        tableHeader += "</thead>";

        var tableContent = "";
        for (i = 0; i < data.rows.length; ++i) {
            tableContent += "<tr class='tblRow'>"
                              
            for (j = 0; j < data.rows[i].c.length; ++j) {
                tableContent += "<td>" + data.rows[i].c[j].v + "</td>";
            }
            tableContent += "</tr>";
        }
        tableString += tableHeader + tableContent + "</table>";
                
        $("#tableContent").html(tableString);
    }

}