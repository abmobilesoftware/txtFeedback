$(function () {
   $("#btnSendMsg").bind("click", function (e) {
      e.preventDefault();
      var text = $("#txtBody").val();
      $.getJSON('NexmoTest/SendMessage',
                { msgText: text, from: $("#fromNumber").val(), to: $("#toNumber").val() },
                function (data) {
                   //conversation starred status changed
                   console.log(data);
                });   
   });
});