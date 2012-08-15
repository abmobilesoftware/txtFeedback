function Report() {
    var self = this, chartArea;    

    this.initialize = function () {
       self.initializeCalendar();
       self.initializeButtons();

       window.app.chartArea = new ChartArea();
       window.app.chartArea.drawChart("15.07.2012", "18.07.2012", "day");
    };

       var now4 = new Date();
       var now3 = new Date();
       now3.setDate(now4.getDate() - 4);

       // Calendar setup
       $('#widgetCalendar').DatePicker({
          flat: true,
          format: 'd B, Y',
          date: [new Date(now3), new Date(now4)],
          calendars: 3,
          mode: 'range',
          starts: 1,
          onChange: function (formated) {
             $('#widgetField span').get(0).innerHTML = formated.join(' &divide; ');
          }
       });
       var state = false;
       $('#widgetField>a').bind('click', function () {
          $('#widgetCalendar').stop().animate({ height: state ? 0 : $('#widgetCalendar div.datepicker').get(0).offsetHeight }, 500);
          state = !state;
          return false;
       });
       $('#reportRange div.datepicker').css('position', 'absolute');
    };

    this.initializeButtons = function () {
        // Radio buttons setup
        self = this;
        $("#radio").buttonsetv();
        $(".radioOption").change(function () {
            window.app.chartArea.drawChart("15.08.2012", "19.08.2012", $(this).val());
        });
    };
}