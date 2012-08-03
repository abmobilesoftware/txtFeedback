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

var ReportMenuItemModel = Backbone.Model.extend({
    defaults: {
        itemIdentifier: 1,
        itemName: "Conversation",
        leaf: false,
        parent: 1
    }
});



var ReportsMenu = Backbone.Collection.extend({
    model:ReportMenuItemModel
});

var ReportsMenuItemView = Backbone.View.extend({
    tagName: 'li',
    initialize: function () {
        _.bindAll(this, 'render');
    },
    renderParent: function() {
        $(this.el).html("<span class='reportMenuParentItem'>" + this.model.get("itemName") + "</span>" + "<ul class='item" + this.model.get("itemIdentifier") + "'></ul>");
        return this;
    },
    renderLeaf: function () {
        $(this.el).addClass('innerLi')
        $(this.el).html("<span class='reportMenuLeafItem' reportId='" + this.model.get("itemIdentifier") + "'>" + this.model.get("itemName") + "</span>");
        return this;
    }
});

var ReportsMenuView = Backbone.View.extend({
    el: $("#leftColumn"),
    initialize: function () {
        _.bindAll(this, 'render');
        var reportMenuItemModel1 = new ReportMenuItemModel({itemIdentifier: 1, itemName: "Conversation", leaf: false, parent: 1});
        var reportMenuItemModel2 = new ReportMenuItemModel({ itemIdentifier: 2, itemName: "Overview", leaf: true, parent: 1 });
        var reportMenuItemModel3 = new ReportMenuItemModel({ itemIdentifier: 3, itemName: "Incoming vs Outgoing", leaf: true, parent: 1 });
        var reportMenuItemModel4 = new ReportMenuItemModel({ itemIdentifier: 4, itemName: "Clients", leaf: false, parent: 4 });
        var reportMenuItemModel5 = new ReportMenuItemModel({ itemIdentifier: 5, itemName: "Overview", leaf: true, parent: 4 });
        var reportMenuItemModel6 = new ReportMenuItemModel({ itemIdentifier: 6, itemName: "New vs Returning", leaf: true, parent: 4 });
        this.menuItems = new ReportsMenu([reportMenuItemModel1, reportMenuItemModel2, reportMenuItemModel3, reportMenuItemModel4, reportMenuItemModel5, reportMenuItemModel6]);
        this.render();
    },
    render: function () {
        self = this;
        $(this.el).append("<ul class='primaryList collapsibleList'></ul>");
        _(this.menuItems.models).each(function (menuItemModel) {
            var reportsMenuItemView = new ReportsMenuItemView({ model: menuItemModel });
            if (!menuItemModel.get("leaf")) {
                $("ul.primaryList", self.el).append(reportsMenuItemView.renderParent().el);
            } else {
                var selector = ".item" + menuItemModel.get("parent");
                $(selector, self.el).append(reportsMenuItemView.renderLeaf().el);
            }
        });
        $(".reportMenuLeafItem").click(function () {
            $("*").removeClass("reportMenuItemSelected");
            $(this).addClass("reportMenuItemSelected");
        });
    }
});

