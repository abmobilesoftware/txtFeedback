"use strict";window.app=window.app||{};var gSelectedMessage=null;var gSelectedConversationID=null;var gSelectedElement=null;var timer;var timer_is_on=0;function markConversationAsRead(){$(gSelectedElement).removeClass("unreadconversation");$(gSelectedElement).addClass("readconversation");app.selectedConversation.set({Read:true});$.getJSON("Messages/MarkConversationAsRead",{conversationId:gSelectedConversationID},function(a){app.updateNrOfUnreadConversations(false);console.log("MarkConversationAsRead done")})}function resetTimer(){if(timer_is_on){clearTimeout(timer);timer_is_on=false}}function startTimer(a){if(!timer_is_on){if(!$(gSelectedElement).hasClass("readconversation")){timer=setTimeout(markConversationAsRead,a);timer_is_on=true}}}window.app.Message=Backbone.Model.extend({defaults:{From:"0752345678",To:"0751569435",Text:"defaulttext",TimeReceived:Date.now(),ConvID:1,Direction:"from",Read:false,Starred:false},parse:function(c,a){var d=c.TimeReceived.substring(6,19);c.TimeReceived=(new Date(Date.UTC(c.Year,c.Month-1,c.Day,c.Hours,c.Minutes,c.Seconds)));var b=cleanupPhoneNumber(c.From)+"-"+cleanupPhoneNumber(c.To);if(b===c.ConvID){b="from"}else{b="to"}c.Direction=b;return c},idAttribute:"Id"});window.app.MessagesList=Backbone.Collection.extend({model:app.Message,identifier:null,url:function(){return"Messages/MessagesList"}});window.app.defaultMessagesOptions={messagesRep:{},currentConversationId:""};function MessagesArea(i,f){var j=this;var h=$("#replyBtn");h.qtip({content:h.attr("tooltiptitle"),position:{corner:{target:"leftMiddle",tooltip:"rightMiddle"}},style:"dark"});$.extend(this,app.defaultMessagesOptions);this.convView=i;this.tagsArea=f;$("#conversations").selectable({filter:".conversation",selected:function(l,m){gSelectedElement=m.selected;resetTimer();var n=m.selected.getAttribute("conversationid");gSelectedConversationID=n;app.selectedConversation=j.convView.convsList.get(n);j.messagesView.getMessages(n);j.tagsArea.getTags(n)},cancel:".conversationStarIconImg"});var b=412536;var c=function(){var l=$("#limitedtextarea");b++;var o=l.val();var m=getFromToFromConversation(j.currentConversationId);var q=m[0];var p=m[1];$.getJSON("Messages/SendMessage",{from:p,to:q,convId:j.currentConversationId,text:o},function(r){console.log(r)});var n=new Date();$(document).trigger("msgReceived",{fromID:p,toID:q,convID:j.currentConversationId,msgID:b,dateReceived:n,text:o,readStatus:false,messageIsSent:true});$("#replyToMessageForm")[0].reset()};$("#replyBtn").click(function(){c()});$("#limitedtextarea").keydown(function(l){if(l.which===13&&l.shiftKey){c();l.preventDefault()}});_.templateSettings={interpolate:/\{\{(.+?)\}\}/g,evaluate:/\{%([\s\S]+?)%\}/g,escape:/\{%-([\s\S]+?)%\}/g};var k=Backbone.View.extend({model:app.Message,tagName:"div",messageTemplate:_.template($("#message-template").html()),initialize:function(){_.bindAll(this,"render","updateView");this.model.on("change",this.updateView);return this.render},render:function(){this.$el.html(this.messageTemplate(this.model.toJSON()));var p="messagefrom";var m="arrowFrom";var n="arrowInnerFrom";var l="arrowInnerLeft";var q="extraMenuWrapperLeft";if(this.model.attributes.Direction==="to"){p="messageto";m="arrowTo";n="arrowInnerTo";l="arrowInnerRight";q="extraMenuWrapperRight"}this.$el.addClass("message");this.$el.addClass(p);$(".arrow",this.$el).addClass(m);$(".arrowInner",this.$el).addClass(n);$(".innerExtraMenu",this.$el).addClass(l);$(".extraMenuWrapper",this.$el).addClass(q);var o=$("div.sendEmailButton img",this.$el);setTooltipOnElement(o,o.attr("tooltiptitle"),"dark");return this},updateView:function(){return this}});var a={lines:13,length:7,width:4,radius:10,rotate:0,color:"#000",speed:1,trail:60,shadow:true,hwaccel:false,className:"spinner",zIndex:2000000000,top:"auto",left:"auto"};var g=new Spinner(a);var e=false;var d=Backbone.View.extend({el:$("#messagesbox"),initialize:function(){_.bindAll(this,"render","getMessages","appendMessage","appendMessageToDiv","resetViewToDefault","newMessageReceived");this.messages=new app.MessagesList();this.messages.bind("reset",this.render)},resetViewToDefault:function(){$("#messagesbox").html($("#noConversationSelectedMessage").val());$("#textareaContainer").addClass("invisible");$("#tagsContainer").addClass("invisible");j.currentConversationId=""},getMessages:function(n){console.log("getting conversations with id:"+n);$("#messagesbox").html("");var m=document.getElementById("scrollablemessagebox");g.spin(m);j.currentConversationId=n;if(j.currentConversationId in app.globalMessagesRep){e=false;g.stop();startTimer(3000);this.render();$("#textareaContainer").removeClass("invisible");$("textareaContainer").fadeIn("slow");$("#tagsContainer").removeClass("invisible");$("#tagsContainer").fadeIn("slow")}else{var l=new app.MessagesList();l.identifier=n;l.bind("reset",this.render);l.bind("add",this.appendMessage);e=true;l.fetch({data:{conversationId:l.identifier},success:function(){startTimer(3000);g.stop();$("#textareaContainer").removeClass("invisible");$("textareaContainer").fadeIn("slow");$("#tagsContainer").removeClass("invisible");$("#tagsContainer").fadeIn("slow")}});app.globalMessagesRep[j.currentConversationId]=l;$.each(l,function(o,p){p.set("Starred",app.selectedConversation.get("Starred"))})}},render:function(){$("#messagesbox").html("");var l=this;app.globalMessagesRep[j.currentConversationId].each(function(m){l.appendMessageToDiv(m,e,false)});g.stop();return this},appendMessage:function(l){if(l.get("ConvID")===j.currentConversationId){console.log("Adding new message: "+l.get("Text"));this.appendMessageToDiv(l,true,true)}},newMessageReceived:function(q,n,o,s,t){console.log("new message received: "+t+" with ID:"+o);var m=new app.Message({Id:o});var l=getFromToFromConversation(j.currentConversationId);var r=l[0];var p="from";if(!comparePhoneNumbers(q,r)){p="to"}m.set("Direction",p);m.set("From",q);m.set("ConvID",n);m.set("Text",t);m.set("TimeReceived",new Date(Date.parse(s)));if(app.globalMessagesRep[n]!==undefined){app.globalMessagesRep[n].add(m)}},appendMessageToDiv:function(q,n,l){var p=new k({model:q});var o=p.render().el;$(this.el).append(o);$(o).hover(function(){var r=$(this).find("div.extramenu")[0];gSelectedMessage=$($(this).find("div span")[0]).html();$(r).show()},function(){var r=$(this).find("div.extramenu")[0];$(r).hide()});if(n){$(o).hide().fadeIn("2000")}if(l){var m=$("#scrollablemessagebox");m.animate({scrollTop:m.prop("scrollHeight")},3000)}}});this.messagesView=new d()}function limitText(c,b,a){if(c.value.length>a){c.value=c.value.substring(0,a)}else{b.value=a-c.value.length}};