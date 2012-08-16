window.app = window.app || {};
function SecondArea(iData) {

    var data = iData;
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
    var target = document.getElementById('chartArea');
    var self = this;

    this.drawArea = function () {
        $("#infoBoxArea").empty();
        var boxWidth = 100 / data.length - 1;
        for (i = 0; i < data.length; ++i) {
            this.buildInfoBox(data[i].name, data[i].source, boxWidth);
        }

        // enable tooltips
        var infoBoxElement = $(".boxTitle");
        infoBoxElement.qtip({
            content: infoBoxElement.attr('tooltiptitle'),
            position: {
                corner: {
                    target: 'topRight',
                    tooltip: 'bottomLeft'
                }
            },
            style: 'light'
        });

        $("#infoBoxArea").append("<div class='clear'></div>");
     }

    this.buildInfoBox = function (name, resource, width) {
        spinner.spin(target);
        $("#overlay").show();        
        var jsonData = $.ajax({
            data: { iIntervalStart: window.app.dateHelper.transformDate(window.app.startDate), iIntervalEnd: window.app.dateHelper.transformDate(window.app.endDate), culture: window.app.calendarCulture, scope: window.app.currentWorkingPoint },
            url: app.domainName + resource,
            dataType: "json",
            async: false,
            success: function (data) {
                self.fillInfoBox(name, width, data);
                spinner.stop();
                $("#overlay").hide();
            }
        }).responseText;        
    }

    this.fillInfoBox = function (name, width, data) {
        var infoBoxString = "<div class='boxArea'>" +
                                "<div class='infoContent'><div class='infoContentMiddle'><div class='infoContentInner'><span class='boxContent'><span class='boxValue'>" + data.value + "</span><span class='boxUnit'> " + data.unit + "</span></span></div></div></div>" +
                                "<div class='infoTitle'><div class='infoTitleMiddle'><div class='infoTitleInner'><span class='boxTitle' title='Tooltip1'>" + name + "</span></div></div></div>" +
                            "</div>";

        $("#infoBoxArea").append(infoBoxString);
    }

}