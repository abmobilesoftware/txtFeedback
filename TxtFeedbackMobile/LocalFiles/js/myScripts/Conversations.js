var ConversationsPage = Backbone.View.extend({
	events: {
		"click #logOffBtn": "logOffBtnHandler"
	},
	initialize: function() {
		_.bindAll(this, "buildConversationsList",
				"messageReceivedHandler",
				"messageSentHandler",
				"carbonsMessageReceivedHandler",
				"logOffBtnHandler",
				"resizeHandler",
				"pageShowHandler",
				"pageHideHandler");
		this.dom = {
				$CONVERSATIONS_LIST : $("#convList", this.$el),
				$HEADER: $(".app-header", this.$el)
		};
		this.conversationsAreaModel = new ConversationsAreaModel();
		this.conversationsArea = new ConversationsArea(
				{
					el: this.dom.$CONVERSATIONS_LIST, 
					model: this.conversationsAreaModel
				});

		/*
		 *  on "pagebeforeshow" event - Prepare conversationsAreaModel 
		 * in this phase it loads the working points
		 * on "ready - conversationsAreaModel has the working points
		 * loaded and can build the conversations list
		 */
		
		this.$el.on("pagebeforeshow", 
				this.conversationsAreaModel.initialSetup);
		this.conversationsAreaModel.on(this.conversationsAreaModel.events.READY,
				this.buildConversationsList);
		
		this.$el.on("pageshow", this.pageShowHandler);
		this.$el.on("pagehide", this.pageHideHandler);
		
		/*
		 * Update the conversations list when a xmpp message is sent or received 
		 */
		
		this.options.xmppHandler.on(this.options.xmppHandler.events.MESSAGE_RECEIVED,
				this.messageReceivedHandler);
		this.options.xmppHandler.on(this.options.xmppHandler.events.MESSAGE_SENT,
				this.messageSentHandler);	
		this.options.xmppHandler.on(this.options.xmppHandler.events.CARBONS_MESSAGE_RECEIVED,
				this.carbonsMessageReceivedHandler);		
	},
	// OPTIMIZATION 1: Render just when a message arrived or was sent
	buildConversationsList: function() {
		this.conversationsArea.renderArea(false);		
	},
	messageReceivedHandler: function(message) {
		this.conversationsAreaModel.newMessageHandler(message, !this.pageActive);
	},
	messageSentHandler: function(message) {
		this.conversationsAreaModel.newMessageHandler(message, true);
	},
	carbonsMessageReceivedHandler: function(message) {
		this.conversationsAreaModel.newMessageHandler(message, !this.pageActive);
	},
	show: function() {
		this.$el.show();
	},
	hide: function() {
		this.$el.hide();
	},
	// this action will be handled by app and triggered by a menu button
	logOffBtnHandler: function() {		
		this.trigger("EVENT_LOG_OFF");
	},
	logOff: function() {
		this.conversationsAreaModel.logOff();
	},
	resizeHandler: function() {
		this.conversationsArea.setHeight(($(window).height() - 
				this.dom.$HEADER.outerHeight()) + "px");
	},
	pageShowHandler: function() {
		this.pageActive = true;
		this.conversationsAreaModel.activateScroll();
	},
	pageHideHandler: function() {
		this.pageActive = false;
		this.conversationsAreaModel.deactivateScroll();
	}
});

var ConversationsArea = Backbone.View.extend({
	initialize: function() {
		_.bindAll(this, "render", "renderArea",
				"scrollHandler");
		this.model.on(this.model.events.LIST_UPDATED,
				this.renderArea);
		this.conversationItemHeight;
		
		// infer by experiment
		$(window).scroll(this.scrollHandler);
		
	},
	renderArea: function(nextSetOfConversations) {
		this.model.getConversations(this.render, 
				nextSetOfConversations);	
	},
	render: function(conversationsList) {
		this.$el.empty();
		var currentGroupDate = null;
		var currentGroupCount = 0;
		var conversationItemView;
		var listDividerModel;		
		_.each(conversationsList, function(model) {
			if (currentGroupDate == null) {
				currentGroupDate = model.get("TimeUpdated");
				currentGroupCount = 1;
				listDividerModel = new ListDivider({
					GroupDate: currentGroupDate,
					ConversationsCount: currentGroupCount
				});
				listDividerView = new ListDividerView(
						{model: listDividerModel});
				this.$el.append(listDividerView.render().el);
			} else {
				var conversationDate = model.get("TimeUpdated");
				if (conversationDate.toDateString() == currentGroupDate.toDateString()) {
					++currentGroupCount;
				} else {
					listDividerModel.set("ConversationsCount", currentGroupCount);
					currentGroupDate = conversationDate;
					currentGroupCount = 1;
					listDividerModel = new ListDivider({
						GroupDate: currentGroupDate,
						ConversationsCount: currentGroupCount
					});
					listDividerView = new ListDividerView(
							{model: listDividerModel});
					this.$el.append(listDividerView.render().el);
					
				}
			}
			conversationItemView = new ConversationView({model: model});
			this.$el.append(conversationItemView.render().el);
		}, this);	
		this.$el.listview("refresh");
		this.conversationItemHeight = conversationItemView.getHeight();
	},
	scrollHandler: function () {
		/* Old approach, the list has overflow-y: scroll; 
		 * var listContentHeight = this.conversationItemHeight * this.model.getNoOfConversations();
		var errorThreshold = 0.03 * listContentHeight;
		if (this.$el.scrollTop() + errorThreshold >= 
			(listContentHeight - this.$el.height())) {
			this.renderArea(true);
		}*/
		
		if (this.model.isScrollActive()) {
			if ($(window).scrollTop() + $(window).height()/3 >= 
				$(document).height() - $(window).height()) {
				this.renderArea(true);
			}
		}
	},
	setHeight: function(height) {
		this.$el.css("height", height);
	}	
});

var ConversationsAreaModel = Backbone.Model.extend({
	events: {
		LIST_UPDATED: "listUpdatedEvent",
		READY: "readyEvent",
		NOT_READY: "notReadyEvent",
		UNAUTHORIZED: "unauthorizedEvent"
	},
	initialize: function() {
		var self = this;
		_.bindAll(this, "initialSetup");
		this.conversationsList = new ConversationsList();
		this.workingPointsManager = new WorkingPointsManager();
		this.workingPointsManager.on(
				this.workingPointsManager.events.WORKING_POINTS_LOADED, 
				function() {
					self.conversationsList.setWorkingPointsManager(
							self.workingPointsManager);
					self.trigger(self.events.READY);
				});
		this.workingPointsManager.on(
				this.workingPointsManager.events.WORKING_POINTS_NOT_LOADED,
				function() {
					self.trigger(self.events.NOT_READY);
				});
		this.scrollStatus = false;
		this.currentSet = 0;
		this.setSize = 18;
		this.noMoreConversations = false;
	},
	initialSetup : function() {
		this.workingPointsManager.getWorkingPoints();
	},
	/* A set contains 10 conversations */
	getConversations: function(callback, 
			nextSetOfConversations) {
		var self = this;
		if ((_.isEmpty(this.conversationsList.models) 
				|| nextSetOfConversations) && !this.noMoreConversations) {
			var skip = 10 * this.currentSet;
			var options = {
					onlyFavorites : false,
					onlyUnread : false,
					popUpSupport : false,
					requestIndex : 0,
					skip : skip,
					top : this.setSize
			};
			this.conversationsList.fetch({
				data: options,
				remove: false,
				headers: {
					"X-Requested-With": "XMLHttpRequest"					
				},			
				success: function(collection, response, options) {
					callback.apply(self, [collection.models]);
					++self.currentSet;
					self.noMoreConversations = response.length < 10 ? true : false;
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
		} else {
			callback.apply(this, [this.conversationsList.models]);
		}
	},
	// handles the sent and received messages
	newMessageHandler: function(message, silent) {
		var conversation = this.conversationsList.findWhere({ConvID: message.ConvID});
		if (conversation != undefined) {
			this.conversationsList.remove(conversation);
		} else {
			conversation = new Conversation();
		}
		this.conversationsList.add(conversation, {at: 0});
		conversation.set("Text", message.Text);
		conversation.set("TimeReceived", message.TimeReceived);
		conversation.set("From", message.From);	
		conversation.set("ConvID", message.ConvID);
		conversation.set("IsSmsBased", message.IsSmsBased);
		conversation.set("Read", false);
		conversation.transformAttributes();
		if (!silent) {
			this.trigger(this.events.LIST_UPDATED, false);
		}
	},
	logOff: function() {
		this.loadedFromServer = false;
		this.workingPointsManager.reset();
		this.conversationsList.reset();
		this.currentSet = 0;
		this.noMoreConversations = false;
	},
	getNoOfConversations: function() {
		return this.conversationsList.models.length;
	},
	activateScroll: function() {
		this.scrollStatus = true;
	},
	deactivateScroll: function() {
		this.scrollStatus = false;
	},
	isScrollActive: function() {
		return this.scrollStatus;
	}	
});

var Conversation = Backbone.Model.extend({
	defaults: {
		TimeUpdated: Date.now(),
		Read: false,
		Text: "some data", //content text
		From: "defaultDirectionNumber", //used for indication if the last message was from or to
		To: "defaultRecipient",
		ConvID: "defaultConversationID", //unique identifier for a conversation
		Starred: false, //favorite conversation or not
		ClientDisplayName: "defaultClient",
		ClientIsSupportBot: false,
		IsSmsBased: false,  
		WorkingPointDisplayName: "defaultWP"
	},
	initialize: function() {
		_.bindAll(this, "parse");		
		this.on("change:Read", this.updateReadStateOnServer);
		this.updateReadStateUrl = "http://www.dev.txtfeedback.net/Conversations/MarkConversationAsRead";
	},
	// automatic parse of raw data received from server
	parse: function (data, xhc) {
		var workingPointsManager = this.collection.getWorkingPointsManager();
		data.TimeUpdated = new Date(Number(data.TimeReceived.substring(data.TimeReceived.indexOf("(") + 1,
				data.TimeReceived.indexOf(")"))));
		var fromTo = getFromToFromConversation(data.ConvID);
		data.IsSmsBased = (data.IsSmsBased === false || data.IsSmsBased === "false") ? false : true;
		data.ClientDisplayName = fromTo[0].substring(0,7) + "...";
		data.WorkingPointDisplayName = 
				workingPointsManager.getWorkingPointName(fromTo[1], data.IsSmsBased)		
		return data;
	},
	// transform of current attributes
	transformAttributes: function() {
		var self = this;
		this.set("IsSmsBased", 
				this.get("IsSmsBased") === false || this.get("IsSmsBased") === "false" ? false : true);
		var fromTo = getFromToFromConversation(this.get("ConvID"));
		var workingPointsManager = this.collection.getWorkingPointsManager();
		var workingPointName = workingPointsManager.getWorkingPointName(fromTo[1], this.get("IsSmsBased"));
		this.set({"TimeUpdated": this.get("TimeReceived")}, {silent: true});
		this.set({"ClientDisplayName": fromTo[0].substring(0,7) + "..."}, {silent: true});
		this.set({"WorkingPointDisplayName": workingPointName}, {silent: true});
		return true;
	},
	updateReadStateOnServer: function() {
		$.ajax({
			url: this.updateReadStateUrl,
			type: "GET",
			data: "&conversationId=" + this.get("ConvID"), 
			crossDomain: true,
			xhrFields: {
				withCredentials: true
			}			
		})
	},
	idAttribute: "ConvID" //the id shold be the combination from-to
});

var ConversationView = Backbone.View.extend({	
	events: {
		"click a": "selectConversation"
	},
	model: Conversation,
	tagName: "li",
	self: this,
	initialize: function () {
		_.bindAll(this, 'render', "selectConversation",
				"getHeight");
		this.constanst = {
			EVENT_CONVERSATION_SELECTED : "conversationsSelectedEvent"	
		};
		this.conversationTemplate = 
			_.template($('#conversation-template').html());	
		this.model.on("change:Read", this.render);
		this.$el.addClass("conversation");
	},
	render: function () {
		this.$el.addClass(this.model.get("Read")? "read-conversation" 
				: "unread-conversation");		
		this.$el.html(this.conversationTemplate(this.model.toJSON()));
		//$("div", this.$el).click(this.selectConversation);
		return this;
	},
	selectConversation: function(event) {
		event.preventDefault();
		Backbone.trigger(this.constanst.EVENT_CONVERSATION_SELECTED,
				this.model);
		
	},
	getHeight: function() {
		return this.$el.height();
	}
});

var ConversationsList = Backbone.Collection.extend({
	model: Conversation,
	initialize: function() {
		_.bindAll(this, "setWorkingPointsManager",
				"getWorkingPointsManager",
				"reset");
	},
	methodUrl: {
		"read": "http://www.dev.txtfeedback.net/Conversations/ConversationsList",
		//"read": "http://10.0.2.2:4631/Conversations/ConversationsList",
		"delete": "http://www.dev.txtfeedback.net/Conversations/Delete"
	},
	sync: function (method, collection, options) {
		if (collection.methodUrl && collection.methodUrl[method]) {
			options = options || {};
			options.url = collection.methodUrl[method];
			options.xhrFields = {
					withCredentials : true
			};            
		}
		var parseMethod = (method === "delete") ? "create" : method;
		Backbone.sync(parseMethod, collection, options);
	},
	setWorkingPointsManager: function(workingPointsManager) {
		this.workingPointsManager = workingPointsManager;
	},
	getWorkingPointsManager: function() {
		return this.workingPointsManager;
	}
});

var ListDivider = Backbone.Model.extend({});

var ListDividerView = Backbone.View.extend({
	tagName: "li",
	initialize: function() {
		_.bindAll(this, "render");
		this.template = _.template($("#listDivider-template").html());
		this.model.on("change:ConversationsCount", this.render);
		this.$el.attr("data-role", "list-divider");
		this.$el.attr("role", "heading");
	},
	render: function() {
		this.$el.html(this.template(this.model.toJSON()));
		return this;
	}
});