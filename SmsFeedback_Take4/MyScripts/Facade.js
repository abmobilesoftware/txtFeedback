function newMessageReceivedGUI(convView, msgView, fromID, toId, convID, msgID, dateReceived, text, readStatus) {
    console.log("inside newMessageReceived");

    convView.newMessageReceived(fromID, convID, dateReceived, text);
    msgView.newMessageReceived(fromID, convID, msgID, dateReceived, text);
};

function selectedWPsChanged(convView, msgView, checkedWorkingPoints) {
   console.log('selectedWPsChanged triggered');
   convView.getConversations(checkedWorkingPoints.checkedPhoneNumbers);
   msgView.resetViewToDefault();
}

function InitializeGUI() {
    if (window.Prototype) {
        delete Object.prototype.toJSON;
        delete Array.prototype.toJSON;
        delete Hash.prototype.toJSON;
        delete String.prototype.toJSON;
    }
    //putting it all together

    //$("#MySplitter").splitter({
    //   type: "v",
    //   outline: true,
    //   minLeft: 100, sizeLeft: 150, minRight: 100,
    //   resizeToWidth: true,
    //   cookie: "vsplitter",
    //   accessKey: 'I'
    //});
    
   //build the areas
    var wpsArea = WorkingPointsArea();
    var convView = ConversationArea();
    var msgView = MessagesArea(convView);   
   //get the initial working points
    wpsArea.getWorkingPoints();
    //get the initial conversations
    convView.getConversations();

    //the xmpp handler for new messages
    var xmppHandler = CreateXMPPHandler(convView, msgView);
    xmppHandler.connect("smsapp@smsfeedback.com/07541237895", "123456");

    var msgID = 1;
    $("#updateMessage").bind("click", function () {
        var convID = '0754654213-0745103618';
        var fromID = '0754654213';
        var newText = "And another thing, where are your wine bottles?"
        var dateReceived = "Mon, 15 Aug 2005 15:52:01 +0000";
        newMessageReceivedGUI(convView, msgView, fromID, convID, msgID, dateReceived, newText);
        msgID++;
    });

    $("#addMessage").bind("click", function () {
        var convID = '0753214212-0745103618';
        var fromID = '0753214212';
        var dateReceived = "Mon, 15 Aug 2005 15:52:01 +0000";
        var newText = "Something new just got added, aren't you curious?"
        newMessageReceivedGUI(convView, msgView, fromID, convID, msgID, dateReceived, newText);
    });

    $(document).bind('msgReceived', function (ev, data) {
        $.getJSON('Messages/MessageReceived',
                    { from: data.fromID, to: data.toID, text: data.text, receivedTime: data.dateReceived, readStatus: data.readStatus },
                    function (data) {
                        //delivered successfully? if yes - indicate this
                        console.log(data);
                    });
        newMessageReceivedGUI(convView, msgView, data.fromID, data.toID, data.convID, data.msgID, data.dateReceived, data.text);
     });

    $(document).bind('selectedWPsChanged', function (ev, data) {
       selectedWPsChanged(convView, msgView, data);
    });

    window.addEventListener("resize", resizeTriggered, false);
}

function resizeTriggered() {
   var window_height = window.innerHeight;
   var headerHeight = $('header').height()
   $('.container_12').height(window_height - headerHeight);
   //document.getElementById('loading_screen_logo').style.paddingTop
   //       = (window_height * .43) + "px";
}