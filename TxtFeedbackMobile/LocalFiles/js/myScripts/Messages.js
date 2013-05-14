var MessagesPage = Backbone.View.extend({
	events: {
		"click .back": "backButtonHandler"
	},
	initialize:function() {
		_.bindAll(this, "buildMessagesList", 
				"loadConversation", 
				"messageReceivedHandler",
				"messageSentHandler",
				"carbonsMessageReceivedHandler",
				"serverAcknowledgeHandler",
				"clientAcknowledgeHandler",
				"scrollToBottom",
				"backButtonHandler",
				"pageHideHandler",
				"pageBeforeShowHandler",
				"pageShowHandler",
				"pageReady");
		this.dom = {
			$MESSAGES_LIST: $("#messagesList", this.$el),	
			$FOOTER: $(".msg-footer", this.$el),
			$BACK_BUTTON: $(".back", this.$el)
		};
		this.pageEvents = {
				LOAD_PAGE: "loadEvent",
				READY: "readyEvent",
				BACK: "backEvent"
		}
		if (mosync.isAndroid) {
			this.dom.$BACK_BUTTON.hide();			
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
		this.$el.on("pagebeforeshow", this.pageBeforeShowHandler);
		this.$el.on("pageshow", this.pageShowHandler);
		this.$el.on("pagehide", this.pageHideHandler);
		
		this.options.xmppHandler.on(this.options.xmppHandler.events.MESSAGE_RECEIVED,
				this.messageReceivedHandler);
		this.options.xmppHandler.on(this.options.xmppHandler.events.MESSAGE_SENT,
				this.messageSentHandler);
		this.options.xmppHandler.on(this.options.xmppHandler.events.CLIENT_ACKNOWLEDGE,
				this.clientAcknowledgeHandler);
		this.options.xmppHandler.on(this.options.xmppHandler.events.SERVER_ACKNOWLEDGE,
				this.serverAcknowledgeHandler);	
		this.options.xmppHandler.on(this.options.xmppHandler.events.CARBONS_MESSAGE_RECEIVED,
				this.carbonsMessageReceivedHandler);
		/*this.messagesArea.on(this.messagesArea.constants.EVENT_MLIST_RENDERED,
				this.pageReady);*/
	},
	buildMessagesList:function() {
		this.messagesArea.renderArea();
	},
	loadConversation: function(conversation) {
		this.messagesAreaModel.getMessages(conversation, false);
		this.sendMessageModel.setConversation(conversation);
	},
	messageReceivedHandler: function(message) {
		// TODO: Doesn't work on tablet
		navigator.notification.vibrate(1000);
		this.messagesAreaModel.addMessage(message, 
				this.messagesAreaModel.messageDirection.INCOMING, 
				!this.pageActive);
	},
	messageSentHandler: function(message) {
		this.messagesAreaModel.addMessage(message, 
				this.messagesAreaModel.messageDirection.OUTGOING, 
				false);
	},
	serverAcknowledgeHandler: function(messageId) {
		this.messagesAreaModel.serverAcknowledgeHandler(messageId);
	},
	clientAcknowledgeHandler: function(messageId) {
		this.messagesAreaModel.clientAcknowledgeHandler(messageId);
	},
	carbonsMessageReceivedHandler: function(message) {
		this.messagesAreaModel.addMessage(message,
				this.messagesAreaModel.messageDirection.INCOMING,
				!this.pageActive, true);
	},
	logOff: function() {
		this.messagesAreaModel.logOff();
	},
	scrollToBottom: function() {
		// Not displaying the last 1-2 items
		$(window).scrollTop(
				$(document).height());		
	},
	backButtonHandler: function(event) {
		event.preventDefault();
		this.trigger(this.pageEvents.BACK_EVENT);
	},
	pageHideHandler: function() {
		this.pageActive = false;
		this.messagesArea.emptyList();
		this.messagesAreaModel.deactivateConversation();
	},
	pageBeforeShowHandler: function() {
		this.messagesArea.enhanceList();
	},
	pageShowHandler: function() {
		this.scrollToBottom();
		this.pageActive = true;	
		this.pageReady();
	},
	pageReady: function() {
		this.trigger(this.pageEvents.READY);
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
		this.model.on(this.model.events.MESSAGES_LOADED, 
				this.renderArea);
	},
	renderArea: function(messagesList) {
		var self = this;
		this.$el.empty();
		_.each(messagesList, function(model) {
			var messageItemView = new MessageView({model: model});
			self.$el.append(messageItemView.render().el);
		}, this);
		this.scrollToBottom();
		Backbone.trigger(this.constants.EVENT_MLIST_RENDERED);
	},
	enhanceList: function() {
		this.$el.listview("refresh");
	},
	scrollToBottom: function() {
		$(window).scrollTop(
				$(document).height());
	},
	emptyList: function() {
		this.$el.empty();
	}
});

var MessagesAreaModel = Backbone.Model.extend(
{
	messageDirection: {
		INCOMING: "incoming",
		OUTGOING: "outgoing"
	},
	events: {
		ADD_MESSAGE: "addMessageEvent",
		MESSAGES_LOADED: "readyEvent",
		UNAUTHORIZED: "unauthorizedEvent",
		PRELOAD_MESSAGES: "preloadMessagesEvent"
	},
	initialize: function() {
		_.bindAll(this, "getMessages", "addMessage");		
		this.messagesRepository = {};
		this.messagesWaitingForAck = [];
		Backbone.on(this.events.PRELOAD_MESSAGES, this.getMessages);
		this.skip = 0;
		this.top = 10;
	},
	getMessages: function(conversation, silent) {
		/**
		 * silent - TRUE: load the messages in background
		 * FALSE: load the messages and then display
		 */
		var self = this;
		this.conversation = conversation;
		if (!silent) {
			this.conversation.set("Read", true);
			this.conversation.set("Animate", false);
		}
		var messagesListFromRepository = this.messagesRepository[this.conversation.get("ConvID")];
		if (messagesListFromRepository != undefined) {
			if (!silent) {
				this.trigger(this.events.MESSAGES_LOADED, 
						messagesListFromRepository.models);
			}
		} else {
			var messagesList = new MessagesList();
			var data = {
					conversationId : this.conversation.get("ConvID"),
					skip : this.skip,
					top: this.top
			};
			messagesList.fetch({
				data: data,
				headers: {
					"X-Requested-With": "XMLHttpRequest"
				},
				success: function(collection, response, options) {
					self.messagesRepository[options.data.conversationId] = collection;	
					if (!silent) {
						self.trigger(self.events.MESSAGES_LOADED, 
								collection.models);
					}
				}, 
				error: function(collection, response, options) {
					alert("Conversations loading process failed. Response " + 
							response + ".Options " + options);
				},
				statusCode: {
					401: function() {
						Backbone.trigger(self.events.UNAUTHORIZED);
					}
				}
			});
		}
	},
	addMessage: 
	/**
	 * @param message JSON object
	 * @param direction INCOMING/OUTGOING 
	 * @param silent TRUE: add the message without rendering 
	 * the list. Used when the page is not visible.
	 * @param isCarbonsMessage 
	 */
	function(message, direction, silent, isCarbonsMessage) {
		var messagesListFromRepository = this.messagesRepository[message.ConvID];
		// Parse incoming messages, except carbons messages 
		var messageModel = new Message(message, {parse: 
			direction == this.messageDirection.INCOMING ? 
					isCarbonsMessage ? false : true : false});
		if (messagesListFromRepository != undefined) {
			messagesListFromRepository.add(messageModel);
		}
		if (direction == this.messageDirection.OUTGOING || isCarbonsMessage) {
			this.messagesWaitingForAck.push(messageModel);
		}
		/* if ((Is on messages page) AND (The message belongs to this
		 * conversation)) then update UI
		 */ 
		if (!silent && message.ConvID == this.conversation.get("ConvID")) {
			this.trigger(this.events.ADD_MESSAGE, 
					messagesListFromRepository.models);
		}		
	},
	serverAcknowledgeHandler: function(messageId) {
		_.each(this.messagesWaitingForAck, function(message) {
			if (message.get("ClientId") == messageId) {
				message.set("ServerAcknowledge", true);
			}
		});
	}, 
	clientAcknowledgeHandler: function(messageId) {
		var indexOfTheMessage = -1;
		_.each(this.messagesWaitingForAck, function(message, index) {
			if (message.get("ClientId") == messageId) {
				message.set("ClientAcknowledge", true);
				indexOfTheMessage = index;
			}
		});
		if (indexOfTheMessage != -1) 
			this.messagesWaitingForAck.splice(indexOfTheMessage, 1);
	},
	logOff: function() {
		this.messagesRepository = [];
		this.messagesWaitingForAck = [];
	},
	/*
	 * IsActive TRUE the conversation's messages are displayed
	 */
	deactivateConversation: function() {
		this.conversation.set("IsActive", false);
	}
});

var Message = Backbone.Model.extend({
	defaults: {
		ServerAcknowledge: false,
		ClientAcknowledge: false,
		IsSmsBased: false,
		CarbonsMessage: false,
		TemporaryId: "temporaryId"
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
	url: domain + "/Messages/MessagesList",
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
		"click button": "clickHandler",
		"keyup": "keyUpHandler"
	},
	initialize: function() {
		_.bindAll(this, "clickHandler", "messageSentHandler",
				"keyUpHandler");
		this.dom = {
			$MESSAGE_TEXTAREA : $("#sendMsgTextArea", this.$el) 	
		};
		//this.
		this.constants = {
				TWO_SPACES : "  ",
				EMPTY_STRING : ""		
		};
		this.key = {
			ENTER : 13
		};
		this.model.on(this.model.events.MESSAGE_SENT, 
				this.messageSentHandler);
		// Keeps the textarea above the soft keyboard
		//this.dom.$MESSAGE_TEXTAREA.val(this.constants.TWO_SPACES);
	},
	clickHandler: function(event) {
		this.model.sendMessage(this.dom.$MESSAGE_TEXTAREA.val());
	},
	keyUpHandler: function(event) {
		if (event.keyCode == this.key.ENTER) {
			this.model.sendMessage(
					this.dom.$MESSAGE_TEXTAREA.val());
		}
	},
	messageSentHandler: function() {
		this.dom.$MESSAGE_TEXTAREA.val(this.constants.EMPTY_STRING);
		window.focus();
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
		if (this.isMessageValid(message)) {
			this.xmppHandler.sendMessage(
					generateUUID(), // Message Id
					getFromToFromConversation(
							this.conversation.get("ConvID"))[1] + this.componentDomain,
					getFromToFromConversation(
							this.conversation.get("ConvID"))[0] + this.userDomain, // Xmpp To address, final destination 
					this.conversation.get("ConvID"), // Conversation id
					message, // Body
					this.conversation.get("IsSmsBased"));
		}
	},
	setConversation: function(conversation) {
		this.conversation = conversation;
	},
	isMessageValid: function(message) {
		return message.length > 0 ? true : false; 
	}
});

var XMPPHandler = Backbone.Model.extend({
	events: {
		MESSAGE_RECEIVED: "messageReceivedEvent",
		MESSAGE_SENT: "messageSentEvent",
		CARBONS_MESSAGE_RECEIVED: "carbonsReceivedEvent",
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
	sendMessage: function(clientId, from, to, convId,
			messageBody, isSmsBased) {
		var dateSent = new Date();
		if (this.conn.connected) {
			this.trigger(this.events.MESSAGE_SENT, 
					{
						ClientId: clientId, 
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
			var xmppMessage = formatXmppMessage(clientId, this.componentAddress,
					formattedMsgBody);
			this.conn.send(xmppMessage);			
			return "sending";
		} else {
			alert("Check your internet connection and try again later");
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
	sendInitialPresence: function() {
		var available = $pres();
		this.conn.send(available);
	},
	sendUnavailablePresence: function() {
		var unavailable = $pres({type: "unavailable"});
		this.conn.send(unavailable);
	},
	connectCallback: function(status) {
		var reconnect = false;
		if (status === Strophe.Status.CONNECTED) {
			alert("Connected to XMPP");
			this.conn.addHandler(this.handleMessage, 
					null, "message", null, null, null, 
					{matchBare: true});
			this.conn.addHandler(this.handleInfoQuery,
					null, "iq", null, null, null,
					{matchBare: true});
			this.serverDomain = Strophe.getDomainFromJid(
					this.conn.jid);
			this.getServerInfo();
			this.sendInitialPresence();
		} else if (status === Strophe.Status.CONNECTING) {
			//alert("CONNECTING");
		} else if (status === Strophe.Status.AUTHENTICATING) {
			//alert("AUTHENTICATING");
		} else if (status === Strophe.Status.DISCONNECTING) {
			//alert("DISCONNECTING");
		} else if (status === Strophe.Status.DISCONNECTED) {
			alert("Disconnected from XMPP");
			reconnect = true;
			this.conn.reset();
		} else if (status === Strophe.Status.CONNFAIL) {
			alert("CONNFAIL");
			//reconnect = true;
		} else if (status === Strophe.Status.AUTHFAIL) {
			alert("AUTHFAIL");
			//reconnect = true;
		} else if (status === Strophe.Status.ERROR) {
			alert("ERROR");	
			//reconnect = true;
		}	
		/* 1. Disconnected by network issues
		 * reconnect = true, gracefullyDisconnect = false
		 * (reconnect && !this.gracefullyDisconnect) = true
		 * 2. Disconnected by user will 
		 * reconnect = true, gracefullyDisconnect = true
		 * (reconnect && !this.gracefullyDisconnect) = false
		 */
		if (reconnect && !this.gracefullyDisconnect) {
			setTimeout(this.connect, 2000, 
					this.userId, this.password);
		}				
	},
	handleMessage: function(xmppMessage) {
		// Handle client and server acknowledges
		var subject = getXmppMessageSubject(xmppMessage);
		if (subject === this.acknowledge.CLIENT) {
			this.trigger(this.events.CLIENT_ACKNOWLEDGE, 
						getAcknowledgeId(xmppMessage));
		} else if (subject === this.acknowledge.SERVER) {
			this.trigger(this.events.SERVER_ACKNOWLEDGE,
						getAcknowledgeId(xmppMessage));
		}			
		// Handle carbon and normal messages
		var type = getXmppMessageType(xmppMessage); 
		if (type === "chat") {
			var body = getXmppMessageBody(xmppMessage);			
			var acknowledgeDestination = getXmppMessageAckDestination(xmppMessage);
			var carbonNs = getXmppMessageNs(xmppMessage);
			if (carbonNs === this.xmppNs.CARBON) {
				var message = parseMessage(body);
				var clientId = getXmppMessageId(xmppMessage);
				message.CarbonsMessage = true;
				message.ClientId = clientId;
				this.trigger(this.events.CARBONS_MESSAGE_RECEIVED, 
						message);	
			} else {
				var id = getXmppMessageId(xmppMessage).split("##");
				var clientId = id[0];
				var serverId = id[1];
				if (isMessageDelayed(xmppMessage)) {
					this.sendAcknowledgeMessage(this.componentAddress,
							clientId, acknowledgeDestination);
				} else {
					var message = parseMessage(body);
					message.Id = serverId;
					this.sendAcknowledgeMessage(this.componentAddress,
							clientId, acknowledgeDestination);
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
   