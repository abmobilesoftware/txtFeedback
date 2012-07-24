"use strict";window.app=window.app||{};window.app.WorkingPoint=Backbone.Model.extend({defaults:{TelNumber:"defaultNumber",Name:"defaultNumber",Description:"defaultDescription",CheckedStatus:true},idAttribute:"TelNumber"});window.app.WorkingPointPool=Backbone.Collection.extend({model:app.WorkingPoint,url:function(){return"Messages/WorkingPointsPerUser"}});window.app.nrOfCheckedWorkingPoints=0;$(function(){window.app=window.app||{};_.templateSettings={interpolate:/\{\{(.+?)\}\}/g,evaluate:/\{%([\s\S]+?)%\}/g,escape:/\{%-([\s\S]+?)%\}/g};window.app.WorkingPointView=Backbone.View.extend({model:app.WorkingPoint,tagName:"span",phoneNumberTemplate:_.template($("#phoneNumber-template").html()),events:{"click .wpSelectorIcon":"selectedChanged"},initialize:function(){_.bindAll(this,"render","selectedChanged");this.model.bind("destroy",this.unrender,this);return this.render},render:function(){this.$el.html(this.phoneNumberTemplate(this.model.toJSON()));this.$el.addClass("phoneNumber");this.$el.addClass("phoneNumberSelected");return this},unrender:function(){this.$el.remove()},selectedChanged:function(){if((app.nrOfCheckedWorkingPoints>=2)||!this.model.attributes.CheckedStatus){this.model.set("CheckedStatus",!this.model.get("CheckedStatus"));var a=$("img",this.$el);if(this.model.get("CheckedStatus")===true){this.$el.removeClass("phoneNumberUnselected");this.$el.addClass("phoneNumberSelected");setCheckboxState(a,true)}else{this.$el.removeClass("phoneNumberSelected");this.$el.addClass("phoneNumberUnselected");setCheckboxState(a,false)}}}})});function WorkingPointsArea(){var a=this;a.checkedPhoneNumbersArray=[];var b={lines:13,length:7,width:4,radius:10,rotate:0,color:"#fff",speed:1,trail:60,shadow:true,hwaccel:false,className:"spinner",zIndex:2000000000,top:"auto",left:"auto"};var d=new Spinner(b);var c=Backbone.View.extend({el:$("#phoneNumbersPool"),initialize:function(){_.bindAll(this,"render","appendWorkingPoint","getWorkingPoints","triggerFilteringOnCheckedStatusChange");this.phoneNumbersPool=new app.WorkingPointPool();this.phoneNumbersPool.bind("add",this.appendWorkingPoint,this);this.phoneNumbersPool.bind("reset",this.render)},getWorkingPoints:function(){app.nrOfCheckedWorkingPoints=0;var e=document.getElementById("phoneNumbersPool");d.spin(e);this.phoneNumbersPool.fetch({success:function(){d.stop()}})},render:function(){var e=this;this.phoneNumbersPool.each(function(f){e.appendWorkingPoint(f)})},appendWorkingPoint:function(g){var f=this;g.on("change",function(h){f.triggerFilteringOnCheckedStatusChange(h)});var e=new app.WorkingPointView({model:g});app.nrOfCheckedWorkingPoints++;$(this.el).append(e.render().el)},triggerFilteringOnCheckedStatusChange:function(e){if(e.get("CheckedStatus")===true){app.nrOfCheckedWorkingPoints++}else{app.nrOfCheckedWorkingPoints--}a.checkedPhoneNumbersArray=[];_.each(this.phoneNumbersPool.models,function(f){if(f.get("CheckedStatus")===true){a.checkedPhoneNumbersArray.push(f.get("TelNumber"))}});$(document).trigger("refreshConversationList")}});a.wpPoolView=new c()};