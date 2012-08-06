var ReportModel = Backbone.Model.extend({
    defaults: {
        name: "Total sms report",
        menuItemName: "Overview",
        sections: [
                    {
                        identifier: "PrimaryChartArea", visibility: true, resources: [
                                                                                       {name: "Get total no of sms report", source: "Reports/getTotalNoOfSms"}
                                                                                     ]
                    },
                    {
                        identifier: "InfoBox", visibility: true, resources: [
                                                                                { name: "Total no of sms", source: "Reports/getNoOfSms" }
                                                                            ]
                    },
                    {
                        identifier: "AdditionalChartArea", visibility: false, resources: []
                    }
                  ]
    }
});

var ReportsMenu = Backbone.Collection.extend({
    model:ReportModel
});

var ReportsMenuItemView = Backbone.View.extend({
    tagName: 'li',
    initialize: function() {
        _.bindAll(this, 'render');
    },
    render: function() {
        $(this.el).html("<span>" + this.model.get("menuItemName") + "</span>");
        return this;
    }
});

var ReportsMenuView = Backbone.View.extend({
    el: $('#leftColumn'),
    defaults: {
        menuItems: {}
    },
    initialize: function () {
        _.bindAll(this, 'render');
        var reportModel1 = new ReportModel();
        var reportModel2 = new ReportModel();
        this.menuItems = new ReportsMenu([reportModel1, reportModel2]);
        this.render();
    },
    render: function () {
        self = this;
        $(this.el).append("<ul></ul>");
        _(this.menuItems.models).each(function (menuItemModel) {
            var reportsMenuItemView = new ReportsMenuItemView({ model: menuItemModel });
            $("ul", self.el).append(reportsMenuItemView.render().el);
        });
    }
});

function ReportsPage() {
    
    var reportsMenuView = new ReportsMenuView();
}