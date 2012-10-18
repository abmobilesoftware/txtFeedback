function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
}



$(function () {
    alert("Starting users creation");
    window.app.xmppHandlerInstance = new window.app.XMPPhandler();
    for (i = 0; i < 10; ++i) {
        var xmppConn = new window.app.XMPPhandler();
        xmppConn.register();
        sleep(500);
    }
});