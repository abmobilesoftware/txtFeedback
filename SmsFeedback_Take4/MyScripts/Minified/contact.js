jQuery(function(b){var a={message:null,init:function(){b("div.sendEmailButton").live("click",function(f){f.preventDefault();var c=gSelectedMessage;var d=gSelectedConversationID;b.get("EmailSend/GetEmailMessageForm",{emailText:c,convID:d},function(e){b(e).modal({appendTo:"form",closeHTML:"<a href='#' title='Close' class='modal-close'>x</a>",position:["15%",],overlayId:"contact-overlay",containerId:"contact-container",onOpen:a.open,onShow:a.show,onClose:a.close})})})},open:function(c){if(b.browser.mozilla){b("#contact-container .contact-button").css({"padding-bottom":"2px"})}if(b.browser.safari){b("#contact-container .contact-input").css({"font-size":".9em"})}var d=280;if(b("#contact-subject").length){d+=26}if(b("#contact-cc").length){d+=22}var e=b("#contact-container .contact-title").html();b("#contact-container .contact-title").html("Loading...");c.overlay.fadeIn(200,function(){c.container.fadeIn(200,function(){c.data.fadeIn(200,function(){b("#contact-container .contact-content").animate({height:d},function(){b("#contact-container .contact-title").html(e);b("#contact-container form").fadeIn(200,function(){b("#contact-container #contact-name").focus();b("#contact-container .contact-cc").click(function(){var f=b("#contact-container #contact-cc");f.is(":checked")?f.attr("checked",""):f.attr("checked","checked")});if(b.browser.msie&&b.browser.version<7){b("#contact-container .contact-button").each(function(){if(b(this).css("backgroundImage").match(/^url[("']+(.*\.png)[)"']+$/i)){var f=RegExp.$1;b(this).css({backgroundImage:"none",filter:'progid:DXImageTransform.Microsoft.AlphaImageLoader(src="'+f+'", sizingMethod="crop")'})}})}})})})})})},show:function(c){b("#contact-container .contact-send").click(function(d){d.preventDefault();if(a.validate()){var f=b("#contact-container .contact-message");f.fadeOut(function(){f.removeClass("contact-error").empty()});b("#contact-container .contact-title").html("Sending...");b("#contact-container form").fadeOut(200);b("#contact-container .contact-content").animate({height:"80px"},function(){b("#contact-container .contact-loading").fadeIn(200,function(){b.ajax({url:"EmailSend/SendEmail",data:b("#contact-container form").serialize(),type:"post",cache:false,dataType:"html",success:function(e){b("#contact-container .contact-loading").fadeOut(200,function(){b("#contact-container .contact-title").html("Message sent!");f.html(e).fadeIn(200)})},error:a.error})})})}else{if(b("#contact-container .contact-message:visible").length>0){var f=b("#contact-container .contact-message div");f.fadeOut(200,function(){f.empty();a.showError();f.fadeIn(200)})}else{b("#contact-container .contact-message").animate({height:"30px"},a.showError)}}})},close:function(c){b("#contact-container").fadeOut(400,function(){b.modal.close()})},error:function(c){alert(c.statusText)},validate:function(){a.message="";if(!b("#contact-container #contact-subject").val()){a.message+="Subject is required. "}var c=b("#contact-container #contact-email").val();if(!c){a.message+="Email is required. "}else{if(!a.validateEmail(c)){a.message+="Email is invalid. "}}if(!b("#contact-container #contact-message").val()){a.message+="Message is required."}if(a.message.length>0){return false}else{return true}},validateEmail:function(d){var c=d.lastIndexOf("@");if(c<1||(c+1)===d.length){return false}if(/(\.{2,})/.test(d)){return false}var e=d.substring(0,c);var f=d.substring(c+1);if(e.length<1||e.length>64||f.length<4||f.length>255){return false}if(/(^\.|\.$)/.test(e)||/(^\.|\.$)/.test(f)){return false}if(!/^"(.+)"$/.test(e)){if(!/^[-a-zA-Z0-9!#$%*\/?|^{}`~&'+=_\.]*$/.test(e)){return false}}if(!/^[-a-zA-Z0-9\.]*$/.test(f)||f.indexOf(".")===-1){return false}return true},showError:function(){b("#contact-container .contact-message").html(b('<div class="contact-error"></div>').append(a.message)).fadeIn(200)}};window.ContactWindow=a;a.init()});