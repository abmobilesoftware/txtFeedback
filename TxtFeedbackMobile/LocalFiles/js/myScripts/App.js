/**
 * Use Chrome when debugging and testing the application for 
 * Android, and Safari for iPhone.
 * An actual screen on mobile is powered by:
 *  1. a DOM element, representing the structure of the page
 *  2. a Backbone View object handling the events coming from
 *  DOM and any aspect linked to the DOM.
 *  3 a Backbone Model object which takes care of the communication
 *  and computation process in order to retrieve data from server
 *  and then pass to view to display it.
 */
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
				"selectConversationStartHandler",
				"selectConversationEndHandler",
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
		this.pushNotificationHandler = new PushNotificationHandler();
		/**
		 * Mapping between page view, dom element and model.
		 */ 
		this.logOnModel = new LogOnModel({},
				{
					xmppHandler: this.xmppHandler,
					pushNotificationHandler: this.pushNotificationHandler
				});
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
		/**
		 *  Handle events at application level
		 */ 
		this.logOnModel.on(this.logOnModel.events.LOGON_SUCCESS, 
				this.goToConversationsPage);
		this.logOnModel.on(this.logOnModel.events.LOGOFF_SUCCESS,
				this.goToLogOnPage);	
		this.conversationsPage.on("EVENT_LOG_OFF", this.logOffHandler);
		this.messagesPage.on(this.messagesPage.pageEvents.BACK_EVENT, 
				this.back);
		
		/** Select conversation event in 3 steps
		 * - 1 the conversation is selected, the messages are fetched and 
		 * added in DOM. The overlay is displayed
		 * - 2 the messages list DOM is ready. Page transition from conversations
		 * to messages starts
		 * - 3 the messages page is displayed and the overlay is hidden
		 */
		Backbone.on(this.constants.EVENT_SELECTED_CONVERSATION,
				this.selectConversationStartHandler);
		Backbone.on(this.constants.EVENT_MLIST_RENDERED,
				this.goToMessagesPage);
		this.messagesPage.on(this.messagesPage.pageEvents.READY,
				this.selectConversationEndHandler);
				
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
	selectConversationStartHandler: function(conversation) {
		this.transition.start();
		this.messagesPage.loadConversation(conversation);		
	},
	selectConversationEndHandler: function() {
		this.transition.end();
	},
	onlineHandler: function() {
		//alert("Application is online");
	},
	offlineHandler: function() {
		alert("No network connection detected. " +
				"Please enable Wi-Fi or Internet Data.");
	},
	pauseHandler: function() {
		//alert("Application is in background");
	},
	resumeHandler: function() {
		//alert("Application is active");
	}
	
});