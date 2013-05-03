var Transition = Backbone.View.extend({
	start: function() {
		this.$el.height($(window).height());
		this.$el.width($(window).width());
		this.$el.show();
	},
	end: function() {
		this.$el.hide();
	}
});