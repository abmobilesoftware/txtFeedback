window.app = window.app || {};
window.app.vouchersPanel = null;

function displayVouchersPanel(voucherBtnView) {
    if (window.app.vouchersPanel != null) {
        if (window.app.vouchersPanel.isVisible()) {
            window.app.vouchersPanel.hide();
        } else {
            window.app.vouchersPanel.show();
        }
    } else {
        window.app.vouchersPanel = new window.app.VouchersListView({
            el: $("#vPanel"),
            voucherBtnView: voucherBtnView
        });
    }
}

window.app.Button = Backbone.Model.extend({

});

window.app.ButtonView = Backbone.View.extend({
    events: {
        "click a.button": "click"
    },
    initialize: function () {
        this.template = _.template($("#button").html());
        _.bindAll(this, "render", "click", "showLoadingIcon",
            "hideLoadingIcon", "setBackgroundColor", "setForegroundColor", 
            "setBorder", "goToActiveState", "goToInactiveState"
            );
        this.btnViewEvents = {
            EVENT_SHOW_LOADING_ICON: "showLoadingIcon",
            EVENT_HIDE_LOADING_ICON: "hideLoadingIcon",
            EVENT_ACTIVE_STATE: "goToActiveState",
            EVENT_INACTIVE_STATE: "goToInactiveState",
            CLASS_BUTTON_CONTENT: ".buttonContent"
        }
        this.CLASS_LOADER_ICON = ".loader";
        this.on(this.btnViewEvents.EVENT_SHOW_LOADING_ICON,
            this.showLoadingIcon);
        this.on(this.btnViewEvents.EVENT_HIDE_LOADING_ICON,
            this.hideLoadingIcon);
        this.on(this.btnViewEvents.EVENT_ACTIVE_STATE,
            this.goToActiveState);
        this.on(this.btnViewEvents.EVENT_INACTIVE_STATE,
            this.goToInactiveState);
        this.render();
    },    
    render: function () {
        this.$el.html(this.template(this.model.toJSON()));
        return this;
    },
    click: function (event) {
        event.preventDefault();
        this.model.get("Action")(this);
    },
    showLoadingIcon: function () {
        $(this.CLASS_LOADER_ICON, this.$el).show();
    },
    hideLoadingIcon: function () {
        $(this.CLASS_LOADER_ICON, this.$el).hide();
    },
    setBackgroundColor: function (color) {
        $(this.btnViewEvents.CLASS_BUTTON_CONTENT, this.$el).css("background", color); 
    },
    setForegroundColor: function (color) {
        $(this.btnViewEvents.CLASS_BUTTON_CONTENT, this.$el).css("color", color);
    },
    setBorder:function(borderStyle) {
        $(this.btnViewEvents.CLASS_BUTTON_CONTENT, this.$el).css("border", borderStyle);
    },
    goToActiveState: function () {
        this.setBackgroundColor("#183540");
        this.setForegroundColor("#D7EFFF");
        this.setBorder("1px solid #183540");
    },
    goToInactiveState: function () {
        this.setBackgroundColor("#CFDFE3");
        this.setForegroundColor("#000000");
        this.setBorder("1px solid #D0C3BE");
    }
});

window.app.ButtonsBarView = Backbone.View.extend({
    initialize: function () {
        var buttonsList = [];
        if ($("#hasVoucherRole").val() === "yes") {
            var giveVoucherBtn = new window.app.Button({
                Title: $("#titleBtnGiveVoucher").val(),
                Action: function (voucherBtn) { displayVouchersPanel(voucherBtn) }
            });
            buttonsList.push(giveVoucherBtn);
        }
        this.buttons = new window.app.ButtonsList(buttonsList);
        _.bindAll(this, "render", "addButton", "dispose", "setVisible");
        this.render();
    },
    render: function () {
        if (_.size(this.buttons) > 0) {
            _.each(this.buttons, this.addButton, this);
        } else {
            this.$el.remove();
        }
    },
    addButton: function (value, key, list) {
        var buttonView = new window.app.ButtonView({ model: list.at(key) });
        this.$el.append(buttonView.render().el);
    },
    dispose: function () {
        this.$el.empty();
    },
    setVisible: function (visible) {
        if (visible) {
            this.$el.removeClass().addClass("visible");
        } else {
            this.$el.removeClass().addClass("hidden");
        }
    }
});

window.app.ButtonsList = Backbone.Collection.extend({
    model: window.app.Button
});

// Vouchers Panel
window.app.Voucher = Backbone.Model.extend({});

window.app.VouchersList = Backbone.Collection.extend({
    model: window.app.Voucher,
    url: function () {
        return "http://rest.txtfeedback.net/" + window.app.wpShortId + "/api/vouchers";
    }
});

window.app.VoucherView = Backbone.View.extend({
    vouchersListView: {},
    className: "voucherItem",
    events: {
        "click a": "selectVoucher"
    },
    initialize: function () {
        this.template = _.template($("#voucher").html());
        this.ID_CONFIRM_MESSAGE = "#messageChooseVoucher";
        _.bindAll(this, "render", "selectVoucher");
        this.render();
    },
    render: function () {
        this.$el.html(this.template(this.model.toJSON()));
        return this;
    },
    selectVoucher: function (event) {
        event.preventDefault();
        var voucherChoosed = confirm($(this.ID_CONFIRM_MESSAGE).val() + " " +
            this.model.get("code") + " : "
            + this.model.get("description") + "?");
        if (voucherChoosed) {
            this.options.vouchersListView.trigger(
                this.options.vouchersListView.vouchersListEvents.EVENT_VOUCHER_SELECTED,
                this.model.get("code"));
        }
    }
});

window.app.VouchersListView = Backbone.View.extend({
    events: {
        "click .panelClose": "hide"
    },
    initialize: function () {
        _.bindAll(this, "render", "getVouchersList",
            "addVoucher", "show", "hide", "voucherSelected",
            "isVisible", "initializeConstants");
        this.initializeConstants();
        this.vouchers = new window.app.VouchersList();
        this.on(this.vouchersListEvents.EVENT_VOUCHER_SELECTED, this.voucherSelected);
        this.getVouchersList();
    },
    initializeConstants: function () {
        this.ID_PANEL_CONTENT = "#panelContent";
        this.ID_PANEL_CONTAINER = "#vouchersPanel";
        this.ID_NO_VOUCHERS_MESSAGE = "#messageNoVouchers";
        this.vouchersListEvents = { EVENT_VOUCHER_SELECTED: "voucherSelected" }
    },
    getVouchersList: function () {
        $(this.ID_PANEL_CONTENT, this.$el).empty();
        this.options.voucherBtnView.trigger(
            this.options.voucherBtnView.btnViewEvents.EVENT_ACTIVE_STATE);
        this.options.voucherBtnView.trigger(
            this.options.voucherBtnView.btnViewEvents.EVENT_SHOW_LOADING_ICON);
        var self = this;
        this.vouchers.fetch({
            success: function (collection, response, options) {
                if (_.size(collection) > 1) {
                    self.render();
                } else if (_.size(collection) == 1) {
                    this.voucherSelected(collection.at(0).get("code"));
                    this.options.voucherBtnView.trigger(
                        this.options.voucherBtnView.btnViewEvents.EVENT_HIDE_LOADING_ICON);
                } else {
                    this.options.voucherBtnView.trigger(
                        this.options.voucherBtnView.btnViewEvents.EVENT_HIDE_LOADING_ICON);
                    alert($(ID_NO_VOUCHERS_MESSAGE).val());
                }
            },
            error: function (collection, xhr, options) { }
        });
    },
    render: function () {
        this.template = _.template($(this.ID_PANEL_CONTAINER).html());
        this.$el.html(this.template())
        _.each(this.vouchers, this.addVoucher, this);
        this.options.voucherBtnView.trigger(
            this.options.voucherBtnView.btnViewEvents.EVENT_HIDE_LOADING_ICON);        
    },
    addVoucher: function (value, key, list) {
        var voucherView = new window.app.VoucherView({
            model: list.at(key),
            vouchersListView: this
        });
        $(this.ID_PANEL_CONTENT, this.$el).append(voucherView.render().el);
    },
    show: function () {
        this.getVouchersList();
        this.$el.show();
    },
    hide: function (event) {
        if (event != undefined) {
            event.preventDefault();
        }
        this.$el.hide();
        this.options.voucherBtnView.trigger(
            this.options.voucherBtnView.btnViewEvents.EVENT_INACTIVE_STATE);
    },
    voucherSelected: function (voucherCode) {
        $(document).trigger("giveVoucher", { voucherCode: voucherCode });
        this.hide();
    },
    isVisible: function () {
        return this.$el.css("display") == "none" ? false : true;
    }
});

