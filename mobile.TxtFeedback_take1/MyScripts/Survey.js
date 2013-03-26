// TODO: Jump to invalid answer through anchor
// TODO: Add numbers to questions
var Question = Backbone.Model.extend({
    validate: function (attributes, options) {
        if (attributes.Compulsory &&
            (attributes.PickedAnswer === "noValue" || attributes.PickedAnswer === "")) {
            this.set({ "ValidAnswer": false }, { silent: true });
        } else {
            this.set({ "ValidAnswer": true }, { silent: true });
        }
    }
});
var QuestionView = Backbone.View.extend({
    initialize: function () {
        _.bindAll(this, "render", "toggleDisplay", "updateQuestionDisplay");
        this.model.on("change:ValidAnswer", this.updateQuestionDisplay);
        this.template = _.template($("#questionTemplate").html());
    },
    render: function () {
        this.$el.append(this.template(this.model.toJSON()));
        this.renderAnswer();
        return this;
    },
    renderAnswer: function () {
        if (this.model.get("QuestionType") === "rating") {
            var starBar = new StarBarView({ el: $(".answerArea", this.$el) });
        }
    },
    updateQuestionDisplay: function () {
        if (this.model.get("ValidAnswer")) {
            this.$el.removeClass("invalidAnswer");
        } else {
            this.$el.addClass("invalidAnswer");
        }
    },
    toggleDisplay: function (event) {
        if (!_.isUndefined(event)) {
            event.preventDefault();
        }
        if ($(".answerArea", this.$el).css("display") == "none") {
            $(".answerArea", this.$el).show();
        } else {
            $(".answerArea", this.$el).hide();
        }
    },
    updateAnswer: function () {
        this.model.set(
            {
                "PickedAnswer": $(".answer", this.$el).val(),
            });
    }
});

var QuestionSet = Backbone.Collection.extend({
    model: Question
});

var SurveyPageModel = Backbone.Model.extend({
    defaults: {
        PageAttributes: {

        },
        QuestionSet:
            [
            {
                QuestionLabel: "",
                QuestionType: "",
                Answers: [{
                    AnswerLabel: "",
                    AnswerValue: ""
                }],
                PickedAnswer: "noValue",
                Compulsory: true,
                ValidAnswer: false
            }
            ]
    }
});
var PageView = Backbone.View.extend({
    initialize: function () {
        var self = this;
        _.bindAll(this, "render", "saveSurvey",
            "hide", "getHeight");
        this.pageElements = {
            $DONE_BTN: $("#doneBtn", this.$el),
            $QUESTIONS_AREA: $("#questions", this.$el),
            $PAGE_TITLE: $("#pageTitle", this.$el)
        };
        this.pageEvents = {
            THANK_YOU_PAGE: "goToThankYouPageEvent"
        };
        this.questionSet = new QuestionSet(this.model.get("QuestionSet"));
        this.questionsViews = [];
        this.doneBtn = new ButtonView({
            el: this.pageElements.$DONE_BTN
        });
        this.doneBtnTitle = $("#doneBtnTitle").val();
        this.doneBtn.enable();
        this.doneBtn.setTitle(this.doneBtnTitle);
        this.questionSet.on("change:PickedAnswer", function () {
            if (self.isSurveyComplete()) {
                self.doneBtn.enable();
            } else {
                self.doneBtn.disable();
            }
        });
        this.doneBtn.on("click", this.saveSurvey);
        this.render();
    },
    render: function () {
        this.pageElements.$PAGE_TITLE.html(this.model.get("PageAttributes").Title);
        _.each(this.questionSet.models, function (value, key, list) {
            var questionView = new QuestionView({ model: value });
            this.questionsViews.push(questionView);
            this.pageElements.$QUESTIONS_AREA.append(questionView.render().el);
        }, this);
        return this;
    },
    isSurveyComplete: function () {
        var answeredQuestions = 0;
        _.each(this.questionSet.models, function (value, key, list) {
            if (value.get("ValidAnswer"))++answeredQuestions;
        }, this);
        return {
            isDone:
                answeredQuestions == this.questionSet.length ? true : false,
            status: answeredQuestions + "/" + this.questionSet.length
        };
    },
    saveSurvey: function () {
        var answeredQuestions = 0;
        _.each(this.questionsViews, function (value) {
            value.updateAnswer();
        }, this);
        var surveyStatus = this.isSurveyComplete();
        if (surveyStatus.isDone) {
            this.doneBtn.setTitle(
                this.doneBtnTitle + " (" +
                    surveyStatus.status +
                ")"
                );
            this.trigger(this.pageEvents.THANK_YOU_PAGE);
        } else {
            this.doneBtn.setTitle(
                this.doneBtnTitle + " (" +
                    surveyStatus.status +
                ")"
                );
        }
    },
    getWidth: function () {
        return this.$el.outerWidth();
    },
    setWidth: function (value) {
        this.$el.css("width", value);
    },
    hide: function () {
        this.$el.hide();
    },
    show: function() {
        this.$el.show();
    },
    getHeight: function () {
        return this.$el.outerHeight();
    }
});

var ThankYouPageView = Backbone.View.extend({
    events: {
        "click button": "sendEmail"
    },
    initialize: function () {
        _.bindAll(this, "setWidth", "show",
            "getHeight");
        this.sendBtn = new ButtonView({ el: $("#sendBtn", this.$el) });
        this.sendBtn.setTitle("Send");
        this.sendBtn.enable();
    },
    setWidth: function (value) {
        this.$el.css("width", value);
    },
    show: function () {
        this.$el.show();
    },
    enableSendBtn: function (event) {
        if (event.target.checked) {
            this.sendBtn.enable();
        } else {
            this.sendBtn.disable();
        }
    },
    sendEmail: function (event) {
        alert("Send email");
    },
    getHeight: function () {
        return this.$el.outerHeight();
    },
    setTop: function (value) {
        this.$el.css("top", value);
    }
});

var SurveyView = Backbone.View.extend({
    initialize: function () {
        _.bindAll(this, "goToThankYouPage");
        this.questionsPage = new PageView({ el: $("#questionsPage"), model: this.model});
        this.thankYouPage = new ThankYouPageView({ el: $("#thankYouPage") });
        this.questionsPage.on(this.questionsPage.pageEvents.THANK_YOU_PAGE,
            this.goToThankYouPage);
    },
    goToThankYouPage: function () {
        var self = this;
        var pageWidthInPixels = this.questionsPage.getWidth() + "px";
        var expandedWidthPercent = "200%"; // width of two pages side by side
        var normalWidthPercent = "100%";
        this.questionsPage.setWidth(pageWidthInPixels);
        this.setWidth(expandedWidthPercent);
        this.thankYouPage.show();
        this.thankYouPage.setWidth(pageWidthInPixels);
        /*
            thankYouPage.setTop - make thank you page visible during the transition.
            For devices with small display, where the survey doesn't fit entirely 
            on the screen and you have to scroll down.
        */
        var topPadding = this.questionsPage.getHeight() - window.innerHeight;
        this.thankYouPage.setTop(topPadding > 0 ? topPadding + "px" : 0);
        this.$el.animate({
            right: this.questionsPage.getWidth()
        }, {
            duration: 600,
            complete: function () {
                self.questionsPage.hide();
                self.thankYouPage.setTop(0);
                self.$el.css("right", "0px");
                self.thankYouPage.setWidth(normalWidthPercent);
                self.setWidth(normalWidthPercent);
            }
        });
    },
    setWidth: function(value) {
        this.$el.css("width", value);
    }
});

var Star = Backbone.Model.extend({});
var StarView = Backbone.View.extend({
    className: "star",
    events: {
        "click a": "click"
    },
    initialize: function () {
        _.bindAll(this, "click", "render");
        this.template = _.template($("#starTemplate").html());
        this.model.on("change:Active", this.render);
    },
    render: function () {
        this.$el.html(this.template(this.model.toJSON()));
        return this;
    },
    click: function (event) {
        event.preventDefault();
        this.model.trigger("starClickedEvent", this.model);
    }
});

var StarsCollection = Backbone.Collection.extend({
    model: Star,
    initialize: function () {
        this.on("starClickedEvent", this.starClicked);
    },
    starClicked: function (model) {
        for (var i = 0; i <= model.get("id") ; ++i) {
            this.at(i).set("Active", true);
        }
        for (var i = model.get("id") + 1; i < this.models.length; ++i) {
            this.at(i).set("Active", false);
        }
        this.trigger("resultEvent", model.get("id"));
    }
});

var StarBarView = Backbone.View.extend({
    events: {
        "keyup .additionalInfo": "inputAdditionalInfo"
    },
    className: "starsBar",
    initialize: function () {
        _.bindAll(this, "starClicked", "inputAdditionalInfo");
        var starsArray = [];
        this.options.noOfElements = this.options.noOfElements != undefined
            ? this.options.noOfElements : 5;
        for (var i = 0; i < this.options.noOfElements; ++i) {
            starsArray.push({ id: i, Active: false });
        }
        this.starsCollection = new StarsCollection(starsArray);
        this.starsCollection.on("resultEvent", this.starClicked);
        this.result = -1;
        this.render();
        this.domElements = {
            $ADDITIONAL_INFO: $(".additionalInfo", this.$el),
            $ANSWER: $(".answer", this.$el)
        };
    },
    render: function () {
        _.each(this.starsCollection.models, function (value, index, list) {
            var starView = new StarView({ model: value });
            this.$el.append(starView.render().el);
        }, this);
        this.$el.append("<fieldset class='additionalInfo invisible'>" +
                "<legend>Why?</legend><textarea class='comment'></textarea></fieldset>" +
                "<input type='hidden' class='answer' value='noValue' />");
    },
    starClicked: function (value) {
        if (value < 2) {
            this.domElements.$ADDITIONAL_INFO.show();
            this.saveResult(value,
                this.domElements.$ADDITIONAL_INFO.val());
        } else {
            this.domElements.$ADDITIONAL_INFO.hide();
            this.saveResult(value, "");
        }
        this.result = value;
    },
    inputAdditionalInfo: function (event) {
        this.saveResult(this.result, event.target.value);
    },
    saveResult: function (pValue, pAdditionalInfo) {
        /* save the result in .answer input field value attribute */
        var result = {};
        result.additionalInfo = pAdditionalInfo;
        result.value = pValue;
        this.domElements.$ANSWER.val(JSON.stringify(result));
    }
});

var ButtonView = Backbone.View.extend({
    events: {
        "click": "click"
    },
    initialize: function () {
        _.bindAll(this, "click");
        this.constants = {
            PROP_DISABLED: "disabled",
            CLASS_DISABLED: "disabled",
            CLASS_ENABLED: "enabled",
            EVENT_CLICK: "click"
        }
        this.on(this.constants.EVENT_DISABLE, this.disableBtn);
        this.on(this.constants.EVENT_ENABLE, this.enableBtn);
    },
    enable: function () {
        this.$el.prop(this.constants.PROP_DISABLED, false);
        if (this.$el.hasClass(this.constants.CLASS_DISABLED)) {
            this.$el.removeClass(this.constants.CLASS_DISABLED);
        }
        this.$el.addClass(this.constants.CLASS_ENABLED);
    },
    disable: function () {
        this.$el.prop(this.constants.PROP_DISABLED, true);
        if (this.$el.hasClass(this.constants.CLASS_ENABLED)) {
            this.$el.removeClass(this.constants.CLASS_ENABLED);
        }
        this.$el.addClass(this.constants.CLASS_DISABLED);
    },
    click: function (event) {
        event.preventDefault();
        this.trigger(this.constants.EVENT_CLICK);
    },
    getTitle: function () {
        return this.$el.html();
    },
    setTitle: function (title) {
        this.$el.html(title);
    }
});
var PageCollection = Backbone.Collection.extend({
    model: SurveyPageModel
});

function Survey() {
    this.surveyPages = new PageCollection();
    this.loadPage = function (pageNumber) {
        /*var pageModel = this.surveyPages.get(pageNumber);
        if (!_.isNull(pageModel)) {
            var pageView = new PageView(pageModel);
            this.$el.empty();
            this.$el.append(pageView.render().el);
        } else {
            // TODO: Fetch the page model with id pageNumber
        }*/
        var surveyPageModel = new SurveyPageModel(
            {
                PageAttributes: {
                    Title: "Survey"
                },
                QuestionSet:
                    [
                        {
                            QuestionLabel: "1. Write about your hobbies and your house income during the last year",
                            QuestionType: "comment",
                            Answers: [],
                            Compulsory: true
                        },
                        {
                            QuestionLabel: "2. What's your favourite animal?",
                            QuestionType: "list_single_selection",
                            Answers: [
                                {
                                    AnswerLabel: "Pig",
                                    AnswerValue: "pig"
                                },
                                {
                                    AnswerLabel: "Cat",
                                    AnswerValue: "cat"
                                },
                                {
                                    AnswerLabel: "Horse",
                                    AnswerValue: "horse"
                                }
                            ],
                            Compulsory: true
                        },
                        {
                            QuestionLabel: "3. Rate the movie 'Titanic'",
                            QuestionType: "rating",
                            Answers: [],
                            Compulsory: true
                        },
                        {
                            QuestionLabel: "4. How often do you go to gym?",
                            QuestionType: "list_single_selection",
                            Answers: [
                                {
                                    AnswerLabel: "once per week",
                                    AnswerValue: "1 time"
                                },
                                {
                                    AnswerLabel: "twice per week",
                                    AnswerValue: "2 times"
                                },
                                {
                                    AnswerLabel: "three times per week",
                                    AnswerValue: "3 times"
                                }
                            ],
                            Compulsory: true
                        },
                        {
                            QuestionLabel: "5. Rate your physical shape",
                            QuestionType: "rating",
                            Answers: [],
                            Compulsory: true
                        }
                    ]
            }
            );
        var surveyView = new SurveyView({el: $("#survey"), model: surveyPageModel});

    };
};

$(document).ready(function () {
    var survey = new Survey();
    survey.loadPage(7);
});