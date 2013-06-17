var Transition = Backbone.View.extend({
	initialize: function() {
		/*this.dom = {
			$LOADING_TEXT: $("span", this.$el)	
		};*/
	},
	start: function() {
		this.$el.height($(document).height());
		this.$el.width($(document).width());
		/*this.dom.$LOADING_TEXT.css("margin-top", 
				$(window).height() / 2 - 2 * this.dom.$LOADING_TEXT.height());*/
		//this.$el.fadeIn('fast');
		this.$el.show();
	},
	end: function() {
		//this.$el.fadeOut('slow');
		this.$el.hide();
	}
});