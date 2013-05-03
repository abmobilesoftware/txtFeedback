var AppView = Backbone.View.extend({
	states: {
		LOGON: 1,
		CONVERSATIONS: 2,
		MESSAGES: 3
	},
	initialize: function() {
		_.bindAll(this, "goToConversationsPage",
				"goToMessagesPage", "back", 
				"goToLogOnPage", "logOffHandler",
				"selectConversationHandler",
				"onlineHandler",
				"offlineHandler",
				"pauseHandler",
				"resumeHandler");
		$.ajaxSetup({ cache: false });
		this.dom = {
				$PAGE_LOGON : $("#logOnPage", this.$el),
				$PAGE_CONVERSATIONS : $("#conversationsPage", this.$el),
				$PAGE_MESSAGES: $("#messagesPage", this.$el),
				$TRANSITION: $("#transition", this.$el)
		};
		this.currentState = this.states.LOGON;
		this.constants = {
				EVENT_SELECTED_CONVERSATION : "conversationsSelectedEvent",
				EVENT_MLIST_RENDERED: "messagesListRenderedEvent",
				EVENT_UNAUTHORIZED: "unauthorizedEvent"
		};
		this.xmppHandler = new XMPPHandler();
		// Pages
		this.logOnModel = new LogOnModel({},
				{xmppHandler: this.xmppHandler})
		this.logOnPage = new LogOnPage({model: this.logOnModel, 
			el: this.dom.$PAGE_LOGON});
		this.conversationsPage = new ConversationsPage(
				{
					el: this.dom.$PAGE_CONVERSATIONS, 
					xmppHandler: this.xmppHandler
				}); 
		this.messagesPage = new MessagesPage(
				{
					el:this.dom.$PAGE_MESSAGES,
					xmppHandler: this.xmppHandler
				});
		this.transition = new Transition({el: this.dom.$TRANSITION});
		// Events
		this.logOnModel.on(this.logOnModel.events.LOGON_SUCCESS, 
				this.goToConversationsPage);
		this.logOnModel.on(this.logOnModel.events.LOGOFF_SUCCESS,
				this.goToLogOnPage);	
		this.conversationsPage.on("EVENT_LOG_OFF", this.logOffHandler);
		this.messagesPage.on(this.messagesPage.pageEvents.BACK_EVENT, 
				this.back);
		this.messagesPage.on(this.messagesPage.pageEvents.READY,
				this.goToMessagesPage);		
		Backbone.on(this.constants.EVENT_SELECTED_CONVERSATION,
				this.selectConversationHandler);
		Backbone.on(this.constants.EVENT_MLIST_RENDERED,
				this.goToMessagesPage);
		Backbone.on(this.constants.EVENT_UNAUTHORIZED,
				this.goToLogOnPage);
		document.addEventListener("backbutton", this.back);	
		document.addEventListener("online", this.onlineHandler);
		document.addEventListener("offline", this.offlineHandler);
		document.addEventListener("pause", this.pauseHandler);
		document.addEventListener("resume", this.resumeHandler);
	},
	goToConversationsPage: function() {
		this.currentState = this.states.CONVERSATIONS;
		$.mobile.changePage(this.dom.$PAGE_CONVERSATIONS);				
	},
	goToMessagesPage: function() {		
		this.currentState = this.states.MESSAGES;
		this.transition.end();
		$.mobile.changePage(this.dom.$PAGE_MESSAGES);
	},
	goToLogOnPage: function() {
		this.currentState = this.states.LOGON;
		this.logOnPage.show();
		$.mobile.changePage(this.dom.$PAGE_LOGON);
	},
	back: function() {
		switch (this.currentState) {
		case this.states.MESSAGES: 
			$.mobile.changePage(this.dom.$PAGE_CONVERSATIONS);		
		}	
	},
	logOffHandler: function() {
		this.logOnModel.logOff();
		this.conversationsPage.logOff();
		this.messagesPage.logOff();
		this.xmppHandler.disconnect();
	}, 
	selectConversationHandler: function(conversation) {
		this.transition.start();
		this.messagesPage.loadConversation(conversation);		
	},
	onlineHandler: function() {
		alert("Application is online");
	},
	offlineHandler: function() {
		alert("No network connection");
	},
	pauseHandler: function() {
		alert("Application is in background");
	},
	resumeHandler: function() {
		alert("Application is active");
	}
	
});