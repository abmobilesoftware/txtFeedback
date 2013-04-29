var WorkingPoint = Backbone.Model.extend({});

var WorkingPointsList = Backbone.Collection.extend({
	model:WorkingPoint,
	url: "http://www.dev.txtfeedback.net/WorkingPoints/WorkingPointsPerUser",
	sync: function(method, collection, options) {
		options = options || {};
		options.xhrFields = {
				withCredentials: true
		};
		Backbone.sync(method, collection, options);
	}
});

var WorkingPointsManager = Backbone.Model.extend({
	events: {
		WORKING_POINTS_LOADED: "workingPointsLoadedEvent",
		WORKING_POINTS_NOT_LOADED: "workingPointsNotLoadedEvent"
	},
	initialize: function() {
		_.bindAll(this, "reset", "getWorkingPointName");
		this.workingPointsList = new WorkingPointsList();		
	},
	getWorkingPoints: function() {
		var self = this;
		if (_.isEmpty(this.workingPointsList.models)) {
			this.workingPointsList.fetch({
				async: false,
				headers: {
					"X-Requested-With": "XMLHttpRequest"
				},
				success: function(collection, response, options) {
					if (!_.isEmpty(collection.models)) {
						self.trigger(self.events.WORKING_POINTS_LOADED, 
								collection.models); 
					} else {
						self.trigger(self.events.WORKING_POINTS_NOT_LOADED);
					}
				},
				error: function(collection, response, options) {
					alert("Error WP" + response);
					self.trigger(self.events.WORKING_POINTS_NOT_LOADED);
				}
			
			});
		} else {
			this.trigger(self.events.WORKING_POINTS_LOADED, 
					this.workingPointsList.models); 
		}
	},
	reset: function() {
		this.workingPointsList.reset();
	},
	getWorkingPointName: function(identifier, isSms) {
		if (isSms) {
			var workingPoint = this.workingPointsList.findWhere(
					{TelNumber: identifier});
			return workingPoint != null ? workingPoint.get("Name")
					:"Unknown WP";
		}else {
			var workingPoint = this.workingPointsList.findWhere(
					{ShortID: identifier});
			return workingPoint != null ? workingPoint.get("Name")
					: "Unknown WP";
		}
	}
});