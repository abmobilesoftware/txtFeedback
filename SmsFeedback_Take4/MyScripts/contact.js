//#region Defines to stop jshint from complaining about "undefined objects"
/*global window */
/*global Strophe */
/*global document */
/*global console */
/*global $pres */
/*global $iq */
/*global $msg */
/*global Persist */
/*global DOMParser */
/*global ActiveXObject */
/*global Backbone */
/*global _ */
/*global alert */
/*global gSelectedConversationID */
/*global gSelectedMessage */
//#endregion

jQuery(function ($) {
   var contact = {
      message: null,
      init: function () {
         // $('div.sendEmailButton').unbind("click");
         $('div.sendEmailButton').live("click", function (e) {
            e.preventDefault();
            var textToDisplay = gSelectedMessage;
            var conversationId = gSelectedConversationID;
            // load the contact form using ajax
            $.ajax({
               url: "EmailSend/GetEmailMessageForm",
               data: { 'emailText': textToDisplay, 'convID': conversationId },
               cache: true,
               success: function (data) {
                  // create a modal dialog with the data
                  $(data).modal({
                     appendTo: 'form',
                     closeHTML: "<a href='#' title='Close' class='modal-close'>x</a>",
                     minWidth: 600,
                     position: ["15%", "30%"],
                     overlayId: 'contact-overlay',
                     containerId: 'contact-container',
                     onOpen: contact.open,
                     onShow: contact.show,
                     onClose: contact.close
                  });
               }
            });
         });
         //#region Feedback buttons
         var sendPosFeedbackButton = $("#sendPositiveFeedback");
         sendPosFeedbackButton.qtip({
            content: sendPosFeedbackButton.attr('tooltiptitle'),
            position: {
               corner: {
                  target: 'leftBottom',
                  tooltip: 'rightTop'
               }
            },
            style: 'light'
         });

         var sendNegFeedbackButton = $("#sendNegativeFeedback");
         sendNegFeedbackButton.qtip({
            content: sendNegFeedbackButton.attr('tooltiptitle'),
            position: {
               corner: {
                  target: 'leftBottom',
                  tooltip: 'rightTop'
               }
            },
            style: 'light'
         });

         $('#sendPositiveFeedback').live("click", function (e) {
            e.preventDefault();
            $.ajax({
               url: "EmailSend/GetFeedbackForm",
               data: { 'positiveFeedback': true, 'url': document.URL },
               cache: true,
               success: function (data) {
                  // create a modal dialog with the data
                  $(data).modal({
                     appendTo: 'body',
                     closeHTML: "<a href='#' title='Close' class='modal-close'>x</a>",
                     minWidth: 600,
                     position: ["15%", "30%"],
                     overlayId: 'contact-overlay',
                     containerId: 'contact-container',
                     onOpen: contact.open,
                     onShow: contact.show,
                     onClose: contact.close
                  });
               }
            });
         });
         $('#sendNegativeFeedback').live("click", function (e) {
            e.preventDefault();
            $.ajax({
               url: "EmailSend/GetFeedbackForm",
               data: { 'positiveFeedback': false, 'url': document.URL },
               cache: true,
               success: function (data) {
                  // create a modal dialog with the data
                  $(data).modal({
                     appendTo: 'body',
                     closeHTML: "<a href='#' title='Close' class='modal-close'>x</a>",
                     minWidth: 600,
                     position: ["15%", "30%"],
                     overlayId: 'contact-overlay',
                     containerId: 'contact-container',
                     onOpen: contact.open,
                     onShow: contact.show,
                     onClose: contact.close
                  });
               }
            });
         });
      },
      //#endregion
      //#region Open
      open: function (dialog) {
         // add padding to the buttons in firefox/mozilla
         if ($.browser.mozilla) {
            $('#contact-container .contact-button').css({
               'padding-bottom': '2px'
            });
         }
         // input field font size
         if ($.browser.safari) {
            $('#contact-container .contact-input').css({
               'font-size': '.9em'
            });
         }

         // dynamically determine height
         var h = 300;
         if ($('#contact-subject').length) {
            h += 26;
         }
         if ($('#contact-cc').length) {
            h += 22;
         }

         var title = $('#contact-container .contact-title').html();
         var loadingMessage = $('#sendEmailLoadingMsg').val();
         $('#contact-container .contact-title').html(loadingMessage);
         dialog.overlay.fadeIn(200, function () {
            dialog.container.fadeIn(200, function () {
               dialog.data.fadeIn(200, function () {
                  $('#contact-container .contact-content').animate({
                     height: h
                  }, function () {
                     $('#contact-container .contact-title').html(title);
                     $('#contact-container form').fadeIn(200, function () {
                        $('#contact-container #contact-name').focus();

                        $('#contact-container .contact-cc').click(function () {
                           var cc = $('#contact-container #contact-cc');
                           cc.is(':checked') ? cc.attr('checked', '') : cc.attr('checked', 'checked');
                        });

                        // fix png's for IE 6
                        if ($.browser.msie && $.browser.version < 7) {
                           $('#contact-container .contact-button').each(function () {
                              if ($(this).css('backgroundImage').match(/^url[("']+(.*\.png)[)"']+$/i)) {
                                 var src = RegExp.$1;
                                 $(this).css({
                                    backgroundImage: 'none',
                                    filter: 'progid:DXImageTransform.Microsoft.AlphaImageLoader(src="' + src + '", sizingMethod="crop")'
                                 });
                              }
                           });
                        }
                     });
                  });
               });
            });
         });
      },
      //#endregion
      //#region Show
      show: function (dialog) {
         $('#contact-container .contact-send').click(function (e) {
            e.preventDefault();
            // validate form
            if (contact.validate()) {
               var msgValidated = $('#contact-container .contact-message');
               msgValidated.fadeOut(function () {
                  msgValidated.removeClass('contact-error').empty();
               });
               var sendingMsg = $('#sendEmailSendingEmailMsg').val();
               $('#contact-container .contact-title').html(sendingMsg);
               $('#contact-container form').fadeOut(200);
               $('#contact-container .contact-content').animate({
                  height: '80px'
               }, function () {
                  //var serializedInfo = $('#contact-container form').serialize();
                  $('#contact-container .contact-loading').fadeIn(200, function () {
                     $.ajax({
                        url: 'EmailSend/SendEmail',
                        data: $('#contact-container form').serialize(),
                        type: 'post',
                        cache: false,
                        dataType: 'html',
                        success: function (data) {
                           $('#contact-container .contact-loading').fadeOut(200, function () {
                              var emailSentMsg = $('#sendEmailEmailSentMsg').val();
                              $('#contact-container .contact-title').html(emailSentMsg);
                              msgValidated.html(data).fadeIn(200);
                           });
                        },
                        error: contact.error
                     });
                  });
               });
            }
            else {
               if ($('#contact-container .contact-message:visible').length > 0) {
                  var msg = $('#contact-container .contact-message div');
                  msg.fadeOut(200, function () {
                     msg.empty();
                     contact.showError();
                     msg.fadeIn(200);
                  });
               }
               else {
                  $('#contact-container .contact-message').animate({
                     height: '30px'
                  }, contact.showError);
               }

            }
         });
      },
      //#endregion
      //#region Close
      close: function (dialog) {
         $('#contact-container').fadeOut(400, function () {
            $.modal.close();
         });

         //$('#contact-container .contact-message').fadeOut();
         //$('#contact-container .contact-title').html('Goodbye...');

         //$('#contact-container .contact-content').animate({
         //    height: 40
         //}, function () {
         //    dialog.data.fadeOut(200, function () {
         //        dialog.container.fadeOut(200, function () {
         //            dialog.overlay.fadeOut(200, function () {
         //                $.modal.close();
         //            });
         //        });
         //    });
         //});
      },
      //#endregion
      error: function (xhr) {
         alert(xhr.statusText);
      },
      validate: function () {
         contact.message = '';
         if (!$('#contact-container #contact-subject').val()) {
            contact.message += $('#sendEmailValidationSubjectRequiredMsg').val();
         }

         var email = $('#contact-container #contact-email').val();
         if (!email) {
            contact.message += $('#sendEmailValidationEmailRequiredMsg').val();
         }
         else {
            if (!contact.validateEmail(email)) {
               contact.message += $('#sendEmailValidationEmailInvalidMsg').val();
            }
         }

         if (!$('#contact-container #contact-message').val()) {
            contact.message += $('#sendEmailValidationMessageRequiredMsg').val();
         }

         if (contact.message.length > 0) {
            return false;
         }
         else {
            return true;
         }
      },
      validateEmail: function (email) {
         var at = email.lastIndexOf("@");

         // Make sure the at (@) sybmol exists and
         // it is not the first or last character
         if (at < 1 || (at + 1) === email.length) {
            return false;
         }

         // Make sure there aren't multiple periods together
         if (/(\.{2,})/.test(email)) {
            return false;
         }

         // Break up the local and domain portions
         var local = email.substring(0, at);
         var domain = email.substring(at + 1);

         // Check lengths
         if (local.length < 1 || local.length > 64 || domain.length < 4 || domain.length > 255) {
            return false;
         }

         // Make sure local and domain don't start with or end with a period
         if (/(^\.|\.$)/.test(local) || /(^\.|\.$)/.test(domain)) {
            return false;
         }

         // Check for quoted-string addresses
         // Since almost anything is allowed in a quoted-string address,
         // we're just going to let them go through
         if (!/^"(.+)"$/.test(local)) {
            // It's a dot-string address...check for valid characters
            if (!/^[-a-zA-Z0-9!#$%*\/?|\^{}`~&'+=_\.]*$/.test(local)) {
               return false;
            }
         }

         // Make sure domain contains only valid characters and at least one period
         if (!/^[-a-zA-Z0-9\.]*$/.test(domain) || domain.indexOf(".") === -1) {
            return false;
         }

         return true;
      },
      showError: function () {
         $('#contact-container .contact-message')
         .html($('<div class="contact-error"></div>').append(contact.message))
         .fadeIn(200);
      }
   };
   window.ContactWindow = contact;
   contact.init();
});