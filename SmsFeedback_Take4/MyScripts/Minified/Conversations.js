"use strict";window.app=window.app||{};window.app.selectedConversation={};window.app.globalMessagesRep={};window.app.defaultNrOfConversationsToDisplay=2;window.app.defaultNrOfConversationsToSkip=0;window.app.cummulativeSkip=window.app.defaultNrOfConversationsToSkip;window.app.Conversation=Backbone.Model.extend({defaults:{TimeUpdated:Date.now(),Read:false,Text:"some data",From:"defaultNumber",To:"defaultRecipient",Starred:false},parse:function(b,a){b.TimeUpdated=b.TimeReceived;return b},idAttribute:"ConvID"});window.app.ConversationsList=Backbone.Collection.extend({model:app.Conversation,convID:null,url:function(){return"Messages/ConversationsList"}});$(function(){window.app=window.app||{};_.templateSettings={interpolate:/\{\{(.+?)\}\}/g,evaluate:/\{%([\s\S]+?)%\}/g,escape:/\{%-([\s\S]+?)%\}/g};window.app.ConversationView=Backbone.View.extend({model:app.Conversation,tagName:"div",conversationTemplate:_.template($("#conversation-template").html()),initialize:function(){_.bindAll(this,"render");this.model.on("change",this.render);return this.render},render:function(){this.$el.html(this.conversationTemplate(this.model.toJSON()));var a="unreadconversation";if(this.model.attributes.Read===true){a="readconversation"}this.$el.addClass("conversation");this.$el.addClass(a);$(this.el).attr("conversationId",this.model.attributes.ConvID);return this}})});function ConversationArea(f,d){var a=this;var e={lines:13,length:7,width:4,radius:10,rotate:0,color:"#000",speed:1,trail:60,shadow:true,hwaccel:false,className:"spinner",zIndex:2000000000,top:"auto",left:"auto"};var h=new Spinner(e);var g={lines:13,length:7,width:4,radius:4,rotate:0,color:"#fff",speed:1,trail:60,shadow:true,hwaccel:false,className:"spinner",zIndex:2000000000,top:"auto",left:"auto"};var b=new Spinner(g);var c=Backbone.View.extend({el:$("#conversations"),initialize:function(){this.filters=f;this.workingPoints=d;_.bindAll(this,"render","getConversations","getAdditionalConversations","addConversationWithEffect","addConversationBasicEffect","updateConversation","newMessageReceived","gatherFilterOptions");this.convsList=new app.ConversationsList();this.convsList.bind("reset",this.render);this._convViews=[];this.addConversationAsNewElement=true;this.refreshsInProgress=0},getConversations:function(){this._convViews=[];var j=this;app.cummulativeSkip=app.defaultNrOfConversationsToSkip;this.refreshsInProgress++;var k=document.getElementById("scrollableconversations");$("#loadMoreConversations").hide();$("#conversations").html("");h.spin(k);var i=this.gatherFilterOptions();this.convsList.fetch({data:i,traditional:true,success:function(l){if(j.refreshsInProgress<=1){h.stop()}if(app.firstCall){app.updateNrOfUnreadConversations(false);app.firstCall=false}app.requestIndex++}})},gatherFilterOptions:function(){var m={};var l=[];if(this.filters.tagFilteringEnabled){l=this.filters.tagsForFiltering}var n=this.filters.starredFilteringEnabled;var i=this.workingPoints.checkedPhoneNumbersArray;var j,o;if(this.filters.dateFilteringEnabled){j=this.filters.startDate;o=this.filters.endDate}var k=this.filters.unreadFilteringEnabled;var p=app.defaultNrOfConversationsToDisplay;var q=app.cummulativeSkip;m.onlyFavorites=n;m.tags=l;m.workingPointsNumbers=i;m.startDate=j;m.endDate=o;m.onlyUnread=k;m.skip=q;m.top=p;m.requestIndex=app.requestIndex;return m},getAdditionalConversations:function(){var k=document.getElementById("loadMoreConversations");$(k).removeClass("readable");$(k).addClass("unreadable");b.spin(k);app.cummulativeSkip=app.cummulativeSkip+app.defaultNrOfConversationsToDisplay;var i=this.gatherFilterOptions();var j=this;$.ajax({url:"Messages/ConversationsList",data:i,traditional:true,success:function(l){b.stop();$(k).removeClass("unreadable");$(k).addClass("readable");$.each(l,function(){var m=new app.Conversation({From:$(this).attr("From"),ConvID:$(this).attr("ConvID"),TimeReceived:$(this).attr("TimeReceived"),Text:$(this).attr("Text"),Read:$(this).attr("Read"),To:$(this).attr("To"),Starred:$(this).attr("Starred")});j.convsList.add(m,{silent:true});j.addConversationBasicEffect(m,false)});if(l.length===0||l.length<app.defaultNrOfConversationsToDisplay){$(k).hide("slow")}}})},render:function(){this._rendered=true;this.refreshsInProgress--;if(this.refreshsInProgress==0){var j=$("#conversations");j.html("");var i=this;this.convsList.each(function(k){i.addConversationBasicEffect(k)})}},addConversationWithEffect:function(l,k,j){if(k===null){k=true}if(j===null){j=false}var i=this.addConversationNoEffect(l,k);var m=300;if(j){$(i).addClass("ui-selected");gSelectedElement=i;resetTimer();startTimer(3000)}$(i).hide().fadeIn(m).fadeOut(m).fadeIn(m).fadeOut(m).fadeIn(m).fadeOut(m).fadeIn(m)},addConversationBasicEffect:function(k,j){if(j===null){j=true}var i=this.addConversationNoEffect(k,j);$(i).hide().fadeIn("slow")},addConversationNoEffect:function(m,l){var j=new app.ConversationView({model:m});this._convViews.push(j);var k=j.render().el;if(l){$(this.el).prepend(k)}else{$(this.el).append(k)}var i=this;m.on("change",function(n){if(n.hasChanged("Read")&&n.get("Read")){}else{if(n.hasChanged("Starred")){}else{i.updateConversation(n)}}});if(this.convsList.models.length>=app.defaultNrOfConversationsToDisplay){$("#loadMoreConversations").show("slow")}return k},updateConversation:function(i){var j=this;var l=_(this._convViews).select(function(n){return n.model.get("ConvID")===i.get("ConvID")})[0];if(l!=undefined&&l!==null){this._convViews=_(this._convViews).without(l);if(this._rendered){var m=false;if(gSelectedElement===l.el){m=true}var k=$(l.el);k.fadeOut("slow",function(){k.remove();i.off("change");j.addConversationWithEffect(i,true,m)})}}},newMessageReceived:function(k,i,o,j,m){var l=false;var n=a.convsView.convsList.get(o);if(n){n.set({Text:m},{silent:true});n.set("Read",l)}else{if(!a.convsView.filters.IsFilteringEnabled()){var p=new app.Conversation({From:k,To:i,ConvID:o,TimeReceived:j,Text:m});a.convsView.convsList.add(p)}}}});$("#loadMoreConversations").bind("click",function(){a.convsView.getAdditionalConversations()});$(".conversationStarIconImg").live("click",function(k){k.preventDefault();var l=$(this).parents(".conversation").attr("conversationId");var i=false;if(app.globalMessagesRep[l]!=undefined){app.globalMessagesRep[l].each(function(m){m.set("Starred",!m.attributes.Starred);i=m.attributes.Starred})}var j=a.convsView.convsList.get(l).get("Starred");a.convsView.convsList.get(l).set("Starred",!j);$.getJSON("Messages/ChangeStarredStatusForConversation",{conversationId:l,newStarredStatus:!j},function(m){console.log(m)})});this.convsView=new c()};