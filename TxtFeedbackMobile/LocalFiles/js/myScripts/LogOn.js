/**
 * LogOnPage view is mapped on #logOnPage DOM element.
 * In case a user is logged in the system this page is skipped.
 * This behavior is implemented in checkLoggedOnStatus. 
 * TODO: Move checkLoggedOnStatus logic in LogOnPage model.
 */
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
		this.$el.css("visibility", "hidden");
	},
	show: function() {
		this.$el.css("visibility", "visible");
	},
	clearFields: function() {
		this.domElements.$INPUT_USERNAME.val("");
		this.domElements.$INPUT_PASSWORD.val("");
	}
});

/**
 * This class implements logic for:
 * 1. log on/log off
 * 2. check for logged in user
 * 3. get xmpp credentials for logged user
 * 4. register/unregister device GCM id in the system
 */
var LogOnModel = Backbone.Model.extend({
	initialize: function(attributes, options) {
		// first - firefox, second - chrome
		//this.JSON_LOGON_SUCCESS = "\"success\"";
		this.response = {
			LOGON_SUCCESS : "success",
			LOGOFF_SUCCESS : "success",
			REGISTER_SUCCESS : "addDevice success",
			REGISTER_FAIL : "addDevice fail",
			UNREGISTER_SUCCESS: "removeDevice success",
			UNREGISTER_FAIL: "removeDevice fail"
		}		
		this.events = {
				LOGON_SUCCESS : "logon_success_event",
				LOGOFF_SUCCESS : "logoff_success_event"
		};		
		this.xmppHandler = options.xmppHandler;
		this.pushNotificationHandler = options.pushNotificationHandler;
		this.url = {
			xmppCredentials: domain + "/Xmpp/GetConnectionDetailsForLoggedInUser",
			checkLoggedOnStatus : domain + "/Conversations/NrOfUnreadConversations",
			logOn: domain + "/Account/AjaxLogOn",
			logOff : domain + "/Account/AjaxLogOff",
			registerDevice : domain + "/Devices/AddDevice",
			unregisterDevice : domain + "/Devices/RemoveDevice"
		}		
	},
	logOn: function(username, password, action, method) {
		if (this.pushNotificationHandler.getGCMKey() != null) {
			var self = this;
			var credentials= {};
			credentials.UserName = username;
			credentials.Password = password;
			$.ajax({
				data: "{model:" + JSON.stringify(credentials) + "}",
				url: this.url.logOn,
				xhrFields: {
					withCredentials: true
				},
				crossDomain:true,
				type: "POST",
				contentType: "application/json; charset=utf-8",
				// In emulator data equals success, in firefox equals "success"
				success: function(data, textStatus, jqXHR) {									
					if (data === self.response.LOGON_SUCCESS) {
						self.registerDevice(
								self.pushNotificationHandler.getGCMKey());
						self.getXmppCredentials();
					} else {
						alert("Invalid credentials");
					}  				
				}, 
				error: function(jqXHR, textStatus, errorThrown) {
					alert("Check your internet connection and try again");
				}    		
			});
		} else {
			alert("Try again in few moments.")
		}
	},
	logOff: function() {
		var self = this;
		this.unregisterDevice(
				this.pushNotificationHandler.getGCMKey());
		$.ajax({
			url: this.url.logOff,
			type: "POST",
			cache: false,
			xhrFields: {
				withCredentials: true
			},
			crossDomain: true,
			success: function(data) {
				if (data == self.response.LOGOFF_SUCCESS) {
					self.trigger(self.events.LOGOFF_SUCCESS);
				}
			},
			error: function(jqXHR, textStatus, errorThrown) {
				alert("Error log off" + textStatus + " " + errorThrown);
			}
		});
	},
	isLoggedOn: function() {
		var loggedOn = false;
		$.ajax({
			url: this.url.checkLoggedOnStatus,
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
	getXmppCredentials: function()  {
		var self = this;
		$.ajax({
			url: this.url.xmppCredentials,
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
				self.xmppHandler.connect(credentials.XmppUser, credentials.XmppPassword);
				self.trigger(self.events.LOGON_SUCCESS);
			}, 
			error: function(jqXHR, textStatus, errorThrown) {
				alert("Status " + textStatus + " Error " + errorThrown);
			}
			
		});		
	},
	registerDevice: function(deviceId) {
		var self = this;
		$.ajax({
			url: this.url.registerDevice,
			type: "POST",
			data: "{deviceId: \"" + deviceId + "\"}",
			xhrFields: {
				withCredentials: true
			},
			async: false,
			crossDomain: true,
			contentType: "application/json; charset=utf-8",
			success: function(data) {
				if (data == self.response.REGISTER_SUCCESS) {
					return true;
				} else if (data == self.response.REGISTER_FAIL) {
					return false;
				}
			},
			error: function(jqXHR, textStatus, errorThrown) {
				alert("Register device error " + textStatus);
				return false;
			}
		});
	},
	unregisterDevice: function(deviceId) {
		var self = this;
		$.ajax({
			url: this.url.unregisterDevice,
			type: "POST",
			data: "{deviceId: \"" + deviceId + "\"}",
			xhrFields: {
				withCredentials: true
			},
			async: false,
			crossDomain: true,
			contentType: "application/json; charset=utf-8",
			success: function(data) {
				if (data == self.response.UNREGISTER_SUCCESS) {
					return true;
				} else if (data == self.response.UNREGISTER_FAIL) {
					return false;
				}
			}
		});
	}
});