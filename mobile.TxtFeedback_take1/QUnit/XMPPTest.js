function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
}



$(function () {
    module("XMPPTEst");
    test("100UsersSendingMessages", 4, function () {
        window.app.xmppHandlerInstance = new window.app.XMPPhandler();
        var allUsers = "";
        var xmppHandlers = new Array();
        for (i=0; i < 100; ++i) {
            var xmppConn = new window.app.XMPPhandler();
            xmppConn.register();
            sleep(2000);
            allUsers += "username[" + i + "]=" + xmppConn.getUsername() + "; password[" + i + "]=" + xmppConn.getPassword();
            xmppHandlers.push(xmppConn);
        }
        alert(allUsers);
    });
});