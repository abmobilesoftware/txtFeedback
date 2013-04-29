var AppView = Backbone.View.extend({
	states: {
		LOGON: 1,
		CONVERSATIONS: 2,
		MESSAGES: 3
	},
	initialize: function() {
		_.bindAll(this, "goToConversationsPage",
				"goToMessagesPage", "back", 
				"goToLogOnPage", "logOffHandler");
		$.ajaxSetup({ cache: false });
		this.dom = {
				$PAGE_LOGON : $("#logOnPage", this.$el),
				$PAGE_CONVERSATIONS : $("#conversationsPage", this.$el),
				$PAGE_MESSAGES: $("#messagesPage", this.$el)
		};
		this.currentState = this.states.LOGON;
		this.constants = {
				EVENT_SELECTED_CONVERSATION : "conversationsSelectedEvent",
				EVENT_MLIST_RENDERED: "messagesListRenderedEvent"
		};
		this.xmppHandler = new XMPPHandler();
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
		this.logOnModel.on(this.logOnModel.events.LOGON_SUCCESS, 
				this.goToConversationsPage);
		this.logOnModel.on(this.logOnModel.events.LOGOFF_SUCCESS,
				this.goToLogOnPage);	
		this.conversationsPage.on("EVENT_LOG_OFF", this.logOffHandler);
		Backbone.on(this.constants.EVENT_SELECTED_CONVERSATION,
				this.goToMessagesPage);
		Backbone.on(this.constants.EVENT_MLIST_RENDERED,
				this.goToMessagesPage);
		document.addEventListener("backbutton", this.back);				
	},
	goToConversationsPage: function() {
		this.currentState = this.states.CONVERSATIONS;
		$.mobile.changePage(this.dom.$PAGE_CONVERSATIONS);				
	},
	goToMessagesPage: function(conversation) {
		this.messagesPage.setConversation(conversation);
		this.currentState = this.states.MESSAGES;
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
	}
});