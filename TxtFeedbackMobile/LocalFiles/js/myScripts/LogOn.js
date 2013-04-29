var LogOnPage = Backbone.View.extend({
	events: {
		"submit form": "logOn"
	},
	initialize: function () {
		_.bindAll(this, "logOn", "checkLoggedOnStatus",
				"hide", "clearFields");
		var self = this;
		this.domElements = {
				$INPUT_USERNAME : $("#username", this.$el),
				$INPUT_PASSWORD : $("#password", this.$el),
				$INPUT_SUBMIT: $("#logOnBtn", this.$el),
				$FORM: $("#logOnForm", this.$el)
		};
		this.model.on(this.model.events.LOGON_SUCCESS,
				this.clearFields);	
		this.$el.on("pagebeforecreate", this.hide);
		this.$el.on("pageinit", this.checkLoggedOnStatus);		
	},
	logOn: function(event) {
		event.preventDefault();
		this.model.logOn(this.domElements.$INPUT_USERNAME.val(),
				this.domElements.$INPUT_PASSWORD.val(),
				this.domElements.$FORM.prop("action"),
				this.domElements.$FORM.prop("method"));
	},
	checkLoggedOnStatus: function() {
		var self = this;
		if (this.model.isLoggedOn()) {
			this.model.getXmppCredentials();
		} else {
			this.show();
		}
	},
	hide: function() {
		//this.$el.hide();
		this.$el.css("visibility", "hidden");
	},
	show: function() {
		//this.$el.show();
		this.$el.css("visibility", "visible");
	},
	clearFields: function() {
		this.domElements.$INPUT_USERNAME.val("");
		this.domElements.$INPUT_PASSWORD.val("");
	}
});

var LogOnModel = Backbone.Model.extend({
	initialize: function(attributes, options) {
		// first - firefox, second - chrome
		//this.JSON_LOGON_SUCCESS = "\"success\"";
		this.JSON_LOGON_SUCCESS = "success";
		this.JSON_LOGOFF_SUCCESS = "success";
		this.events = {
				LOGON_SUCCESS : "logon_success_event",
				LOGOFF_SUCCESS : "logoff_success_event"
		};		
		this.xmppHandler = options.xmppHandler;
		this.xmppCredentialsUrl = "http://www.dev.txtfeedback.net/Xmpp/GetConnectionDetailsForLoggedInUser";
		//this.xmppCredentialsUrl = "http://localhost:4631/Xmpp/GetConnectionDetailsForLoggedInUser";
		this.checkLoggedOnStatusUrl = "http://www.dev.txtfeedback.net/Conversations/NrOfUnreadConversations";
		this.logOffUrl = "http://www.dev.txtfeedback.net/Account/AjaxLogOff"
	},
	logOn: function(username, password, action, method) {
		var self = this;
		var credentials= {};
		credentials.UserName = username;
		credentials.Password = password;
		$.ajax({
			data: "{model:" + JSON.stringify(credentials) + "}",
			url: action,
			xhrFields: {
				withCredentials: true
			},
			crossDomain:true,
			type: "POST",
			contentType: "application/json; charset=utf-8",
			// In emulator data equals success, in firefox equals "success"
			success: function(data, textStatus, jqXHR) {									
				if (data === self.JSON_LOGON_SUCCESS) {
					//alert("ok login");
					self.getXmppCredentials();
				} else {
					alert("Invalid credentials");
				}  				
			}, 
			error: function(jqXHR, textStatus, errorThrown) {
				alert("Status " + textStatus + " Error " + errorThrown);
			}    		
		});
	},
	getXmppCredentials: function()  {
		var self = this;
		$.ajax({
			url: this.xmppCredentialsUrl,
			type: "GET",
			headers: {
				"X-Requested-With": "XMLHttpRequest"				
			},
			xhrFields: {
				withCredentials: true
			},
			crossDomain: true,
			contentType: "application/json; charset=utf-8",
			// In emulator data is an object, in Firefox is a string
			success: function(credentials) {
				//alert("ok credentials");
				//var credentials = JSON.parse(data);
				self.xmppHandler.connect(credentials.XmppUser, credentials.XmppPassword);
				self.trigger(self.events.LOGON_SUCCESS);
			}, 
			error: function(jqXHR, textStatus, errorThrown) {
				alert("Status " + textStatus + " Error " + errorThrown);
			}
			
		});		
	},
	isLoggedOn: function() {
		var loggedOn = false;
		$.ajax({
			url: this.checkLoggedOnStatusUrl,
			method: "POST",
			data: "{performUpdateBefore: false}",
			headers: {
				"X-Requested-With": "XMLHttpRequest"
			},
			xhrFields: {
				withCredentials: true
			},
			async: false,
			crossDomain: true,
			contentType: "application/json; charset=utf-8",
			statusCode: {
				403: function() {
					loggedOn = false;
				}, 
				200: function() {
					loggedOn = true;
				}
			}
		});
		return loggedOn;
	}, 
	logOff: function() {
		var self = this;
		$.ajax({
			url: this.logOffUrl,
			type: "POST",
			cache: false,
			xhrFields: {
				withCredentials: true
			},
			crossDomain: true,
			success: function(data) {
				if (data == self.JSON_LOGOFF_SUCCESS) {
					self.trigger(self.events.LOGOFF_SUCCESS);
				}
			},
			error: function(jqXHR, textStatus, errorThrown) {
				alert("Error log off" + textStatus + " " + errorThrown);
			}
		});
	}
});