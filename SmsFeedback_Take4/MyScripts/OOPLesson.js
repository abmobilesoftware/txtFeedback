function XMPPConnect() { 
    
}

XMPPConnect.prototype.connect = function(userid, password) {
    this.id = userid;
    this.pass = password;
}

XMPPConnect.prototype.disconnect() = function() {
    alert("My user is " + this.id + " & password = " + this.pass);
}

var conn = new XMPPConnect();
conn.connect("Mike", 123456);
conn.disconnect();