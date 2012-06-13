﻿

function WorkingPointsArea() {
    var PhoneNumber = Backbone.Model.extend({
       defaults: {
          TelNumber: "defaultNumber",
          Name: "defaultNumber",
          Description: "defaultDescription",
          CheckedStatus: true
        },              
        idAttribute: "TelNumber"     
     });

    var PhoneNumbersPool = Backbone.Collection.extend({
       model: PhoneNumber,    
       url: function () {
          return "Messages/WorkingPointsPerUser";
       }
    });
    var checkedPhoneNumbers;

    _.templateSettings = {
        interpolate: /\{\{(.+?)\}\}/g
    };
    var PhoneNumberView = Backbone.View.extend({
        model: PhoneNumber,
        tagName: "span",
        phoneNumberTemplate: _.template($('#phoneNumber-template').html()),
        events: {
            "click ": "triggerFiltering"
        },
        initialize: function () {
           _.bindAll(this, 'render', 'triggerFiltering');
            this.model.bind('destroy', this.unrender, this);
            return this.render;
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
        },
        triggerFiltering: function () {
           //change the checkedStatus
           this.model.attributes['CheckedStatus'] = !this.model.get('CheckedStatus');          
           //make sure we start from the initial view where all the phone numbers are selected         
           var checkedPhoneNumbersArray = new Array();
           _.each(checkedPhoneNumbers.models, function (wp) {
              if (wp.get('CheckedStatus') == true)
               checkedPhoneNumbersArray.push(wp.get('TelNumber'));
           });
           $(document).trigger('selectedWPsChanged', { checkedPhoneNumbers: checkedPhoneNumbersArray });
        }
     });

    var opts = {
       lines: 13, // The number of lines to draw
       length: 7, // The length of each line
       width: 4, // The line thickness
       radius: 10, // The radius of the inner circle
       rotate: 0, // The rotation offset
       color: '#fff', // #rgb or #rrggbb
       speed: 1, // Rounds per second
       trail: 60, // Afterglow percentage
       shadow: true, // Whether to render a shadow
       hwaccel: false, // Whether to use hardware acceleration
       className: 'spinner', // The CSS class to assign to the spinner
       zIndex: 2e9, // The z-index (defaults to 2000000000)
       top: 'auto', // Top position relative to parent in px
       left: 'auto' // Left position relative to parent in px
    };
    var spinner = new Spinner(opts);

    var PhoneNumbersPoolView = Backbone.View.extend({
        el: $("#phoneNumbersPool"),
        //allConversations: null,
        initialize: function () {
           _.bindAll(this, 'render', 'appendWorkingPoint', 'phoneNumbersPoolChanged','getWorkingPoints');
           this.phoneNumbersPool = new PhoneNumbersPool();
           checkedPhoneNumbers = this.phoneNumbersPool;
           this.phoneNumbersPool.bind("add", this.appendWorkingPoint, this);
           this.phoneNumbersPool.bind("reset", this.render);
           //this.phoneNumbersPool.bind("remove", this.phoneNumbersPoolChanged, this);           
        },
        getWorkingPoints: function () {
           var target = document.getElementById('phoneNumbersPool');
           spinner.spin(target);           
           this.phoneNumbersPool.fetch({
              success: function () {                                
                 spinner.stop();
              }
           })
        },
        render: function () {
           var self = this;
           this.phoneNumbersPool.each(function (wp) {
              self.appendWorkingPoint(wp);
           });            
        },
        appendWorkingPoint: function (wp) {
           var phoneNumberView = new PhoneNumberView({ model: wp });
           $(this.el).append(phoneNumberView.render().el);
        },
        phoneNumbersPoolChanged: function () {
            if (this.phoneNumbersPool.models.length == 1) {
                //this.$el.hide();
                $(".deletePhoneNumber").hide();
            }
        }
    });

    var phoneNumbersPoolView = new PhoneNumbersPoolView();
    return phoneNumbersPoolView;
};