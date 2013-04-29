var MessagesPage = Backbone.View.extend({
	initialize:function() {
		_.bindAll(this, "buildMessagesList", 
				"setConversation", 
				"messageReceivedHandler",
				"messageSentHandler",
				"serverAcknowledgeHandler",
				"clientAcknowledgeHandler",
				"scrollToBottom");
		this.dom = {
			$MESSAGES_LIST: $("#messagesList", this.$el),	
			$FOOTER: $(".msg-footer", this.$el)
		};
		this.messagesAreaModel = new MessagesAreaModel();
		this.messagesArea = new MessagesArea(
				{
					el: this.dom.$MESSAGES_LIST, 
					model: this.messagesAreaModel
				});
		this.sendMessageModel = new SendMessageModel({}, 
				{
					xmppHandler: this.options.xmppHandler
				});
		this.sendMessageArea = new SendMessageView(
				{
					el: this.dom.$FOOTER,
					model: this.sendMessageModel
				});
		this.$el.on("pagebeforeshow", this.buildMessagesList);
		this.$el.on("pageshow", this.scrollToBottom);
		
		this.options.xmppHandler.on(this.options.xmppHandler.events.MESSAGE_RECEIVED,
				this.messageReceivedHandler);
		this.options.xmppHandler.on(this.options.xmppHandler.events.MESSAGE_SENT,
				this.messageSentHandler);
		this.options.xmppHandler.on(this.options.xmppHandler.events.CLIENT_ACKNOWLEDGE,
				this.clientAcknowledgeHandler);
		this.options.xmppHandler.on(this.options.xmppHandler.events.SERVER_ACKNOWLEDGE,
				this.serverAcknowledgeHandler);		
		/*this.messagesArea.on(this.messagesArea.constants.EVENT_MLIST_RENDERED,
				this.scrollToBottom);*/
	},
	buildMessagesList:function() {
		this.messagesArea.renderArea();
	},
	setConversation: function(conversation) {
		this.messagesAreaModel.setConversation(conversation);
		this.sendMessageModel.setConversation(conversation);
	},
	messageReceivedHandler: function(message) {
		// TODO: Doesn't work on tablet
		navigator.notification.vibrate(2000);
		this.messagesAreaModel.addMessage(message, 
				this.messagesAreaModel.messageDirection.INCOMING);
	},
	messageSentHandler: function(message) {
		this.messagesAreaModel.addMessage(message, 
				this.messagesAreaModel.messageDirection.OUTGOING);
	},
	serverAcknowledgeHandler: function(messageId) {
		this.messagesAreaModel.serverAcknowledgeHandler(messageId);
	},
	clientAcknowledgeHandler: function(messageId) {
		this.messagesAreaModel.clientAcknowledgeHandler(messageId);
	},
	logOff: function() {
		this.messagesAreaModel.logOff();
	},
	scrollToBottom: function() {
		$("body").animate(
				{scrollTop: $(document).height()},
				2000);		
	}
});

// TODO : Extend this class with a global events class
var MessagesArea = Backbone.View.extend({
	initialize: function() {
		_.bindAll(this, "renderArea", "render",
				"enhanceList");
		this.constants = {
			EVENT_MLIST_RENDERED: "messagesListRenderedEvent"
		};	
		this.model.on(this.model.events.ADD_MESSAGE,
				this.renderArea);
	},
	renderArea: function() {
		this.model.getMessages(this.render);
	},
	render: function(messagesList, event) {
		var self = this;
		this.$el.empty();
		_.each(messagesList, function(model) {
			var messageItemView = new MessageView({model: model});
			self.$el.append(messageItemView.render().el);
		}, this);
		this.enhanceList();
		this.trigger(this.constants.EVENT_MLIST_RENDERED);
	},
	enhanceList: function() {
		this.$el.listview("refresh");
	}
});

var MessagesAreaModel = Backbone.Model.extend({
	messageDirection: {
		INCOMING: "incoming",
		OUTGOING: "outgoing"
	},
	events: {
		ADD_MESSAGE: "addMessageEvent"
	},
	initialize: function() {
		_.bindAll(this, "getMessages", "setConversation",
				"addMessage");		
		this.messagesRepository = {};
		this.messagesWaitingForAck = [];
	},
	getMessages: function(callback) {
		var self = this;
		var messagesListFromRepository = this.messagesRepository[this.conversation.get("ConvID")];
		if (messagesListFromRepository != undefined) {
			callback.apply(this, [messagesListFromRepository.models]);
		} else {
			var messagesList = new MessagesList();
			var data = {
					conversationId : this.conversation.get("ConvID")	
			};
			messagesList.fetch({
				data: data,
				headers: {
					"X-Requested-With": "XMLHttpRequest"
				},
				success: function(collection, response, options) {
					self.messagesRepository[self.conversation.get("ConvID")] = messagesList;
					callback.apply(this, [collection.models, event])
				}, 
				error: function(collection, response, options) {
					alert("Conversations loading process failed. Response " + 
							response + ".Options " + options);
				}	
			});
		}
	},
	setConversation: function(conversation) {
		this.conversation = conversation;
		this.conversation.set("Read", true);
	},
	addMessage: function(message, direction) {
		var messagesListFromRepository = this.messagesRepository[message.ConvID];
		var messageModel = new Message(message, {parse: true});
		if (messagesListFromRepository != undefined) {
			messagesListFromRepository.add(messageModel);
		}
		if (direction == this.messageDirection.OUTGOING) {
			this.messagesWaitingForAck.push(messageModel);
		}
		this.trigger(this.events.ADD_MESSAGE, "addMessageEvent");
	},
	serverAcknowledgeHandler: function(messageId) {
		_.each(this.messagesWaitingForAck, function(message) {
			if (message.get("Id") == messageId) {
				message.set("ServerAcknowledge", true);
			}
		});
	}, 
	clientAcknowledgeHandler: function(messageId) {
		_.each(this.messagesWaitingForAck, function(message) {
			if (message.get("Id") == messageId) {
				message.set("ClientAcknowledge", true);
			}
		})
	},
	logOff: function() {
		this.messagesRepository = [];
		this.messagesWaitingForAck = [];
	}
});

var Message = Backbone.Model.extend({
	defaults: {
		ServerAcknowledge: false,
		ClientAcknowledge: false,
		IsSmsBased: false,
		CarbonsMessage: false
	},
	parse: function(data) {
		data.TimeReceived = new Date(Date.UTC(data.Year,
				data.Month-1, data.Day, data.Hours, data.Minutes,
				data.Seconds));
		if (!isNaN(data.Id)) data.ServerAcknowledge = true;
		else data.ServerAcknowledge = false;
		return data;
	},
	idAttribute: "Id"
});

var MessagesList = Backbone.Collection.extend({
	model: Message,
	url: "http://www.dev.txtfeedback.net/Messages/MessagesList",
	sync: function(method, collection, options) {
		options = options || {};
		options.xhrFields = {
			withCredentials : true	
		};
		Backbone.sync(method, collection, options);
	}		
});

var MessageView = Backbone.View.extend({
	tagName: "li",
	initialize: function() {
		_.bindAll(this, "render");
		this.template = _.template($("#message-template").html());
		// add class for inbound and outbound messages
		var customer = getFromToFromConversation(this.model.get("ConvID"))[0];
		var fromParsed = this.model.get("IsSmsBased") ? cleanupPhoneNumber(this.model.get("From")) : 
				extractUser(this.model.get("From"));
		this.$el.addClass(customer == fromParsed ? "customer" : 
			this.model.get("CarbonsMessage") ? "carbons" : "staff");	
		this.model.on("change:ClientAcknowledge", this.render);
		this.model.on("change:ServerAcknowledge", this.render);
	},
	render: function() {
		this.$el.html(this.template(this.model.toJSON()));
		return this;
	}
});

var SendMessageView = Backbone.View.extend({
	events: {
		"click button": "clickHandler"
	},
	initialize: function() {
		_.bindAll(this, "clickHandler", "messageSentHandler");
		this.dom = {
			$MESSAGE_TEXTAREA : $("#sendMsgTextArea", this.$el) 	
		};
		this.model.on(this.model.events.MESSAGE_SENT, 
				this.messageSentHandler);		
	},
	clickHandler: function(event) {
		this.model.sendMessage(this.dom.$MESSAGE_TEXTAREA.val());
	},
	messageSentHandler: function() {
		this.dom.$MESSAGE_TEXTAREA.val("");
	}
});

var SendMessageModel = Backbone.Model.extend({
	initialize: function(attributes, options) {
		var self = this;
		_.bindAll(this, "sendMessage",
				"setConversation");
		this.xmppHandler = options.xmppHandler;
		this.events = {
			MESSAGE_SENT : this.xmppHandler.events.MESSAGE_SENT	
		};
		this.userDomain = "@txtfeedback.net";
		this.componentDomain = "@devxmpp.txtfeedback.net";
		this.xmppHandler.on(this.xmppHandler.events.MESSAGE_SENT, 
				function() {
					self.trigger(self.events.MESSAGE_SENT);
				});
	},
	sendMessage: function(message) {		
		this.xmppHandler.sendMessage(
				generateUUID(), // Message Id
				getFromToFromConversation(
						this.conversation.get("ConvID"))[1] + this.componentDomain,
				getFromToFromConversation(
						this.conversation.get("ConvID"))[0] + this.userDomain, // Xmpp To address, final destination 
				this.conversation.get("ConvID"), // Conversation id
				message, // Body
				this.conversation.get("IsSmsBased"));		
	},
	setConversation: function(conversation) {
		this.conversation = conversation;
	}
});

var XMPPHandler = Backbone.Model.extend({
	events: {
		MESSAGE_RECEIVED: "messageReceivedEvent",
		MESSAGE_SENT: "messageSentEvent",
		CLIENT_ACKNOWLEDGE: "clientAcknowledgeReceivedEvent",
		SERVER_ACKNOWLEDGE: "serverAcknowledgeReceivedEvent"
	},
	initialize: function() {
		_.bindAll(this, "connect", "connectCallback", "disconnect",
				"sendMessage", "handleMessage", "getServerInfo",
				"handleInfoQuery");
		this.gracefullyDisconnect = false;
		/* Constants */
		this.xmppNs = {
			CARBON : "urn:xmpp:carbons:1"					
		};
		this.acknowledge = {
			CLIENT : "ClientMsgDeliveryReceipt",
			SERVER : "ServerMsgDeliveryReceipt"
		};
		this.serverInfoIqId = "serverInfo";
		this.carbonsIqId = "enableCarbonFeature";
		/* Connection settings */
		this.serverDomain = {};
		this.componentAddress = "broker@devxmpp.txtfeedback.net";
		this.xmppServerAddress = "http://46.137.26.124:5280/http-bind/";
		this.conn = new Strophe.Connection(this.xmppServerAddress);
		/* Listeners */
		this.conn.addHandler(this.handleMessage, null, "message", null, null);
		this.conn.addHandler(this.handleInfoQuery, null, "iq", null, null);
	},
	connect: function(userId, password) {
		this.gracefullyDisconnect = false;
		this.userId = userId;
		this.password = password;		
		this.conn.connect(userId, password, this.connectCallback);
	},
	disconnect: function() {
		this.gracefullyDisconnect = true;
		//this.conn.sync = true;
		this.conn.flush();
		this.conn.disconnect();
	},
	sendMessage: function(msgId, from, to, convId,
			messageBody, isSmsBased) {
		var dateSent = new Date();
		if (this.conn.connected) {
			this.trigger(this.events.MESSAGE_SENT, 
					{
						Id: msgId, 
						ConvID: convId,
						From: from,
						To: to,
						Text: messageBody,
						TimeReceived: dateSent,
						IsSmsBased: isSmsBased,
						Year: dateSent.getFullYear(),
						/* Javascript getMonth is 0 based (0-11)
						 * Server side getMonth is 1 based (1-12) */						 
						Month: (dateSent.getMonth() + 1),
						Day: dateSent.getDate(),
						Hours: dateSent.getHours(),
						Minutes: dateSent.getMinutes(),
						Seconds: dateSent.getSeconds()
					});
			var formattedMsgBody = formatXmppMsgBody(
					from,			
					to, // final recipient of the message
					dateSent, // Message send date 
					convId, messageBody, 
					false, // Direction, to client
					isSmsBased);
			var xmppMessage = formatXmppMessage(msgId, this.componentAddress,
					formattedMsgBody);
			this.conn.send(xmppMessage);			
			return "sending";
		} else {
			this.disconnect();
			return "disconnected";
		}		
	},
	sendAcknowledgeMessage: function(xmppTo, acknowledgeId,
			acknowledgeDestination) {
		if (this.conn.connected) {
			var acknowledgeMessage = formatAckowledgeMessage(xmppTo, 
					acknowledgeId, acknowledgeDestination);
			this.conn.send(acknowledgeMessage);
		}
	},
	enableCarbonsFeature: function() {
		var enableCarbonsStanza = formatCarbonsStanza(this.carbonsIqId);
		this.conn.send(enableCarbonsStanza);
	},
	getServerInfo: function() {
		var infoQuery = formatInfoQueryMessage(this.serverDomain, 
				this.serverInfoIqId);
		this.conn.send(infoQuery);
	},
	connectCallback: function(status) {
		var reconnect = false;
		if (status === Strophe.Status.CONNECTED) {
			//vibrate();		
			alert("Connected to XMPP");		
			this.serverDomain = Strophe.getDomainFromJid(
					this.conn.jid);
			this.getServerInfo();
		} else if (status === Strophe.Status.CONNECTING) {
			//alert("CONNECTING");
		} else if (status === Strophe.Status.AUTHENTICATING) {
			//alert("AUTHENTICATING");
		} else if (status === Strophe.Status.DISCONNECTED) {
			alert("Disconnected from XMPP");
			reconnect = true;
		} else if (status === Strophe.Status.CONNFAIL) {
			alert("CONNFAIL");
			this.disconnect();
		} else if (status === Strophe.Status.AUTHFAIL) {
			alert("AUTHFAIL");
			this.disconnect();
		} else if (status === Strophe.Status.ERROR) {
			alert("ERROR");	
			this.disconnect();
		}	
		/* 1. Disconnected by network issues
		 * reconnect = true, gracefullyDisconnect = false
		 * (reconnect && !this.gracefullyDisconnect) = true
		 * 2. Disconnected by user will 
		 * reconnect = true, gracefullyDisconnect = true
		 * (reconnect && !this.gracefullyDisconnect) = false
		 */
		if (reconnect && !this.gracefullyDisconnect) {
			this.conn.reset();
			setTimeout(this.connect, 2000, 
					this.userId, this.password);
		}				
	},
	handleMessage: function(xmppMessage) {
		// Handle client and server acknowledges
		var subject = getXmppMessageSubject(xmppMessage);
		if (subject === this.acknowledge.CLIENT) {
			this.trigger(this.events.CLIENT_ACKNOWLEDGE, 
					getClientAcknowledgeId(xmppMessage));
		} else if (subject === this.acknowledge.SERVER) {
			this.trigger(this.events.SERVER_ACKNOWLEDGE,
					getServerAcknowledgeId(xmppMessage));
		}			
		// Handle carbon and normal messages
		var type = getXmppMessageType(xmppMessage); 
		if (type === "chat") {
			var body = getXmppMessageBody(xmppMessage);
			var id = getXmppMessageId(xmppMessage);
			var acknowledgeDestination = getXmppMessageAckDestination(xmppMessage);
			var carbonNs = getXmppMessageNs(xmppMessage);
			if (carbonNs === this.xmppNs.CARBON) {
				var message = parseMessage(body);
				message.CarbonsMessage = true;
				this.trigger(this.events.MESSAGE_RECEIVED, 
						message);	
			} else {
				if (isMessageDelayed(xmppMessage)) {
					this.sendAcknowledgeMessage(this.componentAddress,
							id, acknowledgeDestination);
				} else {
					var message = parseMessage(body);
					this.sendAcknowledgeMessage(this.componentAddress,
							id, acknowledgeDestination);
					this.trigger(this.events.MESSAGE_RECEIVED, 
							message);				
				}
			}		
		}
		return true;
	},
	handleInfoQuery: function(infoQuery) {
		// Iq is the response for "Get server info" info query
		if ($(infoQuery).attr("id") == this.serverInfoIqId) {
			var $features = $(infoQuery).children("query").children("feature");
			_.each($features, function(feature) {
				if ($(feature).attr("var") == this.xmppNs.CARBON) {
					this.enableCarbonsFeature();					
				}
			}, this);			
		} else if ($(infoQuery).attr("id") == this.carbonsIqId) {
			//alert("Carbons feature enabled");
		}
		return true;
	}
});
   