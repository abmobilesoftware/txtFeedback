"use strict";window.app=window.app||{};var tagsRep={};window.app.Tag=Backbone.Model.extend({defaults:{Name:"tag",Description:"description",TagType:"none",IsDefault:false},idAttribute:"Name"});window.app.TagsPool=Backbone.Collection.extend({model:window.app.Tag,url:function(){return"Tags/GetTagsForConversation"}});window.app.SpecialTagsPool=Backbone.Collection.extend({model:window.app.Tag,url:function(){return"Tags/GetSpecialTags"}});function TagsArea(){var c={lines:9,length:5,width:3,radius:3,rotate:0,color:"black",speed:1,trail:60,shadow:true,hwaccel:false,className:"spinner",zIndex:2000000000,top:"auto",left:"auto"};var f=new Spinner(c);var b=$("#messagesAddTagPlaceHolderMessage").val();var d=$("#messagesRemoveTagPlaceHolderMessage").val();var e=Backbone.View.extend({el:$("#tagsPool"),initialize:function(){this.conversationID="";app.removeTagTitle=d;$("#tags").tagsInput({height:"22px",width:"auto",onAddTag:this.onAddTag,onRemoveTag:this.onRemoveTag,autocomplete_url:"Tags/FindMatchingTags",defaultText:b,placeholder:b,interactve:true});_.bindAll(this,"render","appendTag","onAddTag","onRemoveTag","getTags")},onAddTag:function(h){var g=new app.Tag({Name:h});tagsRep[gSelectedConversationID].add(g);$.getJSON("Tags/AddTagToConversations",{tagName:h,convID:gSelectedConversationID},function(i){})},onRemoveTag:function(i){var g=tagsRep[gSelectedConversationID];var h=_(g.models).select(function(j){return j.get("Name")===i})[0];g.remove(h);$.getJSON("Tags/RemoveTagFromConversation",{tagName:i,convID:gSelectedConversationID},function(j){})},getTags:function(j){var g=$("#tags");g.importTags("");var i=document.getElementById("tagsContainer");this.conversationID=j;if(j in tagsRep){f.stop();this.render()}else{var h=new app.TagsPool();h.bind("reset",this.render);h.fetch({data:{conversationID:j},success:function(){}});tagsRep[j]=h}},render:function(){var h=$("#tags");h.importTags("");var g=this;tagsRep[this.conversationID].each(function(i){g.appendTag(i)})},appendTag:function(g){$("#tags").addTag(g.get("Name"),{callback:false})}});var a=new e();return a};