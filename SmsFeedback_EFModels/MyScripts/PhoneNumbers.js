$(function () {
    var PhoneNumber = Backbone.Model.extend({
        defaults: {
            number: "no number",
            label: "no label"
        },
        initialize: function () {
            if (!this.get("number")) {
                this.set({ number: this.defaults.number });
            }
        },
        clear: function () {
            this.destroy();
        }
    });
    var PhoneNumbersPool = Backbone.Collection.extend({
        model: PhoneNumber
    });
    var phoneNumber1 = new PhoneNumber({
        number: "0758-412985",
        label: "All"
    });
    var phoneNumber2 = new PhoneNumber({
        number: "0758-123894",
        label: "Sucursala 2"
    });
    var phoneNumber3 = new PhoneNumber({
        number: "07580-47823",
        label: "Sucursala 3"
    });
    var phoneNumber4 = new PhoneNumber({
        number: "07580-47811",
        label: "Sucursala 4"
    });
    var phoneNumber5 = new PhoneNumber({
        number: "07580-41223",
        label: "Sucursala 5"
    });
    var phoneNumber6 = new PhoneNumber({
        number: "07580-66623",
        label: "Sucursala 6"
    });
    var phoneNumbers = new PhoneNumbersPool([phoneNumber1, phoneNumber2, phoneNumber3, phoneNumber4, phoneNumber5, phoneNumber6]);

    _.templateSettings = {
        interpolate: /\{\{(.+?)\}\}/g
    };
    var PhoneNumberView = Backbone.View.extend({
        tagName: "span",
        phoneNumberTemplate: _.template($('#phoneNumber-template').html()),
        events: {
            "click .deletePhoneNumber": "deletePhoneNumber"
        },
        initialize: function () {
            _.bindAll(this, 'render');
            this.model.bind('destroy', this.unrender, this);
            this.render;

        },

        render: function () {
            this.$el.html(this.phoneNumberTemplate(this.model.toJSON()));
            return this;
        },

        unrender: function () {
            this.$el.remove();
        },

        deletePhoneNumber: function () {
            this.model.destroy();
        }
    });

    var PhoneNumbersPoolView = Backbone.View.extend({
        el: $("#phoneNumbersPool"),
        //allConversations: null,
        initialize: function () {
            _.bindAll(this, 'render');
            this.phoneNumbersPool = phoneNumbers;
            this.phoneNumbersPool.bind("remove", this.phoneNumbersPoolChanged, this)
            this.render();
        },
        render: function () {
            var self = this;
            _(this.phoneNumbersPool.models).each(function (phoneNumber) {
                phoneNumberView = new PhoneNumberView({ model: phoneNumber });
                $(self.el).append(phoneNumberView.render().el);
            }, this);
        },
        phoneNumbersPoolChanged: function () {
            if (this.phoneNumbersPool.models.length == 1) {
                //this.$el.hide();
                $(".deletePhoneNumber").hide();
            }
        }
    });

    var phoneNumbersPoolView = new PhoneNumbersPoolView();
});