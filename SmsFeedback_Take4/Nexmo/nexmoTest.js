$(function () {
   $("#btnSendMsg").bind("click", function (e) {
      e.preventDefault();
      var text = $("#txtBody").val();
      $.getJSON('NexmoTest/SendMessage',
                { msgText: text },
                function (data) {
                   //conversation starred status changed
                   console.log(data);
                });   
   });
});