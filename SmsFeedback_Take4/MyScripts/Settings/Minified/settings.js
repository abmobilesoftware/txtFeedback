"use strict";window.app=window.app||{};window.app.showChangePassword=function(){$.ajax({url:"Settings/GetChangePasswordForm",success:function(a){$("#rightColumn").html(a);$("#btnChangePassword").live("click",function(b){b.preventDefault();$.ajax({url:"Settings/GetChangePasswordForm",data:$("#rightColumn form").serialize(),type:"post",cache:true,dataType:"html",success:function(d){var c=d;$("#rightColumn").html(d)},error:function(c){var d=c;$("#rightColumn").html(c)}})})}})};window.app.SettingsArea=function(){var a=this;var b=new window.app.MenuView({el:$("#leftColumn"),eventToTriggerOnSelect:"switchSetting",menuCollection:new window.app.MenuCollection({url:"/Settings/GetMenuItems"}),afterInitializeFunction:function(){$(".liItem21").addClass("menuItemSelected");$("ul.item20").css("display","block");window.app.showChangePassword()}});$(document).bind("switchSetting",function(c,d){switch(d){case"21":window.app.showChangePassword();default:}})};