<%@ Page Title="SmsFeedback" Language="C#" MasterPageFile="~/Views/Shared/Site.Master"
   Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="SmsFeedback_Take4.Utilities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   <%: ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" ContentType="text/xml">
    <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/phonenumbers.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/messages.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/conversations.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/filtersStrip.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/tags.css") %>" />
   <link rel="stylesheet" type="text/css" media="all" href="<%: Url.UpdatedResourceLink("~/Content/jquery.tagsinput.css") %>" />

   <script src="<%: Url.UpdatedResourceLink("~/Scripts/spin.js") %>" type="application/javascript" ></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.cookie.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.simplemodal.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Utilities.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/WorkingPoints.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/Scripts/jquery.tagsinput.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Messages.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/contact.js") %>" type="application/javascript"></script>
   
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Conversations.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Filtering.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/ConversationTags.js") %>" type="application/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/Strophe/strophe.js") %>" type="application/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/flxhr/flXHR.js") %>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/flxhr/strophe.flxhr.js") %>" type="text/javascript"></script>

   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/XMPP.js") %>" type="application/javascript"></script>
   <script src="<%: Url.UpdatedResourceLink("~/MyScripts/Facade.js") %>" type="application/javascript"></script>

   
   <script type="text/template" id="tag-template">       
		<span class="tag"  >
			<span>{{ Name }}</span>
			<span class="removeTag">
				<img class="removeTagIcon" src="<%: Url.Content("~/Content/images/close14x14.png") %>"/>
			</span>
		</span>
   </script>
   <script type="text/template" id="phoneNumber-template">       
		<span >
            <embed src="<%: Url.Content("~/Content/images/check-white.svg") %>" type="image/svg+xml" class="wpItem wpSelectorIcon deletePhoneNumberIconSelected" />		
			<span class="wpItem" >{{ Name }}</span>						
		</span>
   </script>
   <script type="text/template" id="conversation-template">
           <div class="leftLiDiv convColumn">
                {% if (Read) { %}
                        <embed src="<%: Url.Content("~/Content/images/check-grey.svg") %>" type="image/svg+xml" class="images conversationImageRead" />
                {% } else {
                        var fromTo = getFromToFromConversation(ConvID);
                        if (fromTo[0] == From) {
                        %}
                            <embed src="<%: Url.Content("~/Content/images/exclamation-blue.svg") %>" type="image/svg+xml" class="images conversationImageUnread" />
                        {% } else { %}
                            <embed src="<%: Url.Content("~/Content/images/exclamation-green.svg") %>" type="image/svg+xml" class="images conversationImageUnread" />
                        {% }    
                }  %}
            </div>
            <div class="rightLiDiv convColumn">    
                   
               <div class="spanClassFrom rightSideMembers">
                    <span>
                        {% 
                           var fromTo = getFromToFromConversation(ConvID); 
                           var FromNumber = fromTo[0]; 
                           countryPrefix = FromNumber.substring(0,3);
                           localPrefix = FromNumber.substring(3, 5);
                           group1 = FromNumber.substring(5, 9);
                           group2 = FromNumber.substring(9,13); 
                        %} 
                        {{ countryPrefix }} ({{ localPrefix }}) {{ group1 }} {{ group2 }} <span class='conversationArrows'> >> </span>  {{ To }} </span>
                </div>
               <div class='clear'></div>
                <div class="spanClassText rightSideMembers">
                    <span>{{ Text }}</span>
                </div>
                <div class="conversationStarIcon">
                    {% if (Starred) { %}
                            <img src="<%: Url.Content("~/Content/images/star-selected.svg") %>" class="conversationStarIconImg" />
                    {% } else { %}
                            <img src="<%: Url.Content("~/Content/images/star.svg") %>" class="conversationStarIconImg" /> 
                    {% } %}
                </div>
            </div>                         
        <div class="clear"></div>
   </script>
   <script type="text/template" id="message-template">
      <div class="textMessage">
         <span>{{ Text }} </span> 
         <div class="clear"></div>
         {% var timeReceivedLocal = transformDate(TimeReceived, $(".currentCulture").val()); %}
         <span class="timeReceived">{{ timeReceivedLocal }} </span>
      </div>
      
      <div class="clear"></div>
      <div class="extramenu" hoverID="{{ Id }}">
       <div class="extraMenuWrapper"></div>  
       <div class="innerExtraMenu">
            <div class="actionButtons">
               <img class="favConversation" src="<%: Url.Content("~/Content/images/star.svg") %>" />      
             </div>
            <div class="actionButtons sendEmailButton">
               <img src="<%: Url.Content("~/Content/images/mail.png") %>" />
            </div>
         <div class="clear"></div>                       
         </div>               
        
      </div>
        <div class="arrow">
         <div class="arrowInner"> </div>
       </div>
   </script>
   <script type="text/javascript">
      $(function () {
          var newGUI = new InitializeGUI();
         
      });
   </script>
   <script type="text/javascript">
       function Culture(name) {
           this.culturesNames = new Array("ro-ro", "en-us");
           this.culturesNamesEscaped = new Array("ro_ro", "en_us");
           this.timeZones = new Array(3, -5);

           for (i = 0; i < this.culturesNames.length; ++i) {
               if (this.culturesNames[i] == name) return { cultureName: this.culturesNamesEscaped[i], timeZone: this.timeZones[i] };
           }
       }

       function Month(name, culture, year) {
           this.monthsNames_en_us = new Array("Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec");
           this.monthsNames_ro_ro = new Array("Ian", "Feb", "Mar", "Apr", "Mai", "Iun", "Iul", "Aug", "Sep", "Oct", "Nov", "Dec");
           this.monthsDays = new Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);
           this.checkLeapYear = function(iYear) {
               if (iYear % 4 == 0) {
                   if (iYear % 100 == 0) {
                       if (iYear % 400 != 0) {
                           return "false";
                       }
                       if (iYear % 400 == 0) {
                           return "true";
                       }
                   }
                   if (iYear % 100 != 0) {
                       return "true";
                   }
               }
               if (iYear % 4 != 0) {
                   return "false";
               }
           }
           
           for (i = 0; i < this.monthsNames_en_us.length; ++i) {
               if (this.monthsNames_en_us[i] == name)
                   if (!this.checkLeapYear(year)) {
                       return { monthName: name, monthDays: this.monthsDays[i], monthNameByCulture: eval("this.monthsNames_" + culture.cultureName + "[" + i + "]"), monthIndex: i, nextMonth: this.monthsNames_en_us[i + 1 % 12], previousMonth: this.monthsNames_en_us[i - 1 % 12] };
                   } else {
                       if (name == "Feb") {
                           return { monthName: name, monthDays: 29, monthNameByCulture: eval("this.monthsNames_" + culture.cultureName + "[" + i + "]"), monthIndex: i, nextMonth: this.monthsNames_en_us[i + 1 % 12], previousMonth: this.monthsNames_en_us[i - 1 % 12] };
                       } else {
                           return { monthName: name, monthDays: this.monthsDays[i], monthNameByCulture: eval("this.monthsNames_" + culture.cultureName + "[" + i + "]"), monthIndex: i, nextMonth: this.monthsNames_en_us[i + 1 % 12], previousMonth: this.monthsNames_en_us[i - 1 % 12] };
                       }
                   }
            }
       }

       function Day(name, culture) {
           this.daysNames_en_us = new Array("Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat");
           this.daysNames_ro_ro = new Array("D", "L", "Ma", "Mi", "J", "V", "S");

           for (i = 0; i < this.daysNames_en_us.length; ++i) {
               if (this.daysNames_en_us[i] == name) return { dayName: name, dayNameByCulture: eval("this.daysNames_" + culture.cultureName + "[" + i + "]"), dayIndex: i, nextDay: this.daysNames_en_us[i + 1 % 7], previousDay: this.daysNames_en_us[i - 1 % 7] };
           }           
       }

       function LocalDateTimeProcessor(iDate, iCulture) {
           self = this;
           this.currentCulture = iCulture;
           this.currentDate = iDate;
           
           this.setCulture = function (culture) {
               self.currentCulture = culture;
           }
           this.getCulture = function () {
               return self.currentCulture;
           }

           this.computeLocalDateAndTimeAsString = function() {
               var dateComponents = self.currentDate.split(" ");
               var dayName = dateComponents[0].substring(0, dateComponents[0].length - 1);
               var day = parseInt(dateComponents[1]);
               var month = dateComponents[2];
               var year = parseInt(dateComponents[3]);
               var time = dateComponents[4];
               var timeComponents = time.split(":");
               var hours = parseInt(timeComponents[0]);
               var minutes = parseInt(timeComponents[1]);
               var secundes = parseInt(timeComponents[2]);
               var zone = dateComponents[5];
               return self.computeLocalDateAndTime(dayName, day, month, year, hours, minutes, secundes);
           }
           
           this.computeLocalDateAndTime = function (iDayName, iDay, iMonthName, iYear, iHour, iMinutes, iSeconds) {
               var currentDay = new Day(iDayName, self.currentCulture);
               var currentMonth = new Month(iMonthName, self.currentCulture);

               var hoursOverLimit = false;
               var hoursUnderLimit = false;
               var localHour = iHour + self.currentCulture.timeZone;
               if (localHour > 23) {
                   hoursOverLimit = true;
               } else if (localHour < 0) {
                   hoursUnderLimit = true;
               }

               if (hoursOverLimit) {
                   localHour = localHour % 24;
                   var localDay = new Day(currentDay.nextDay, self.currentCulture);
                   var localDayIndex = iDay + 1;

                   var daysOverLimit = false;
                   if (localDayIndex > currentMonth.monthsDays) {
                       daysOverLimit = true;
                   }

                   if (daysOverLimit) {
                       localDayIndex = localDayIndex % currentMonth.monthsDays;
                       var localMonth = new Month(currentMonth.nextMonth, self.currentCulture);

                       var monthsOverLimit = false;
                       if (localMonth.monthName == "Jan") {
                           monthsOverLimit = true;
                       }

                       if (monthsOverLimit) {
                           var localYear = iYear + 1;
                           return self.buildDateAndTimeString(localDay.dayName, localDayIndex, localMonth.monthName, localYear, localHour, iMinutes, iSeconds, self.currentCulture);
                       } else {
                           return self.buildDateAndTimeString(localDay.dayName, localDayIndex, localMonth.monthName, iYear, localHour, iMinutes, iSeconds, self.currentCulture);
                       }
                   } else {
                       return self.buildDateAndTimeString(localDay.dayName, localDayIndex, currentMonth.monthName, iYear, localHour, iMinutes, iSeconds, self.currentCulture);
                   }
                   

               } else if (hoursUnderLimit) {
                   localHour = 24 + localHour;
                   var localDay = new Day(currentDay.previousDay, self.currentCulture);
                   var localDayIndex = iDay - 1;

                   var daysUnderLimit = false;
                   if (localDayIndex < 1) {
                       daysUnderLimit = true;
                   }

                   if (daysUnderLimit) {
                       var localMonth = new Month(currentMonth.previousMonth, self.currentCulture);
                       localDayIndex = localMonth.monthDays;

                       var monthsUnderLimit = false;
                       if (localMonth.monthName == "Dec") {
                           monthsUnderLimit = true;
                       } 

                       if (monthsUnderLimit) {
                           var localYear = iYear + 1;
                           return self.buildDateAndTimeString(localDay.dayName, localDayIndex, localMonth.monthName, localYear, localHour, iMinutes, iSeconds, self.currentCulture);
                       } else {
                           return self.buildDateAndTimeString(localDay.dayName, localDayIndex, localMonth.monthName, iYear, localHour, iMinutes, iSeconds, self.currentCulture);
                       }
                   } else {
                       return self.buildDateAndTimeString(localDay.dayName, localDayIndex, currentMonth.monthName, iYear, localHour, iMinutes, iSeconds, self.currentCulture);
                   }
               } else {
                   return self.buildDateAndTimeString(iDayName, iDay, iMonthName, iYear, localHour, iMinutes, iSeconds, self.currentCulture);
               }
           }

           this.buildDateAndTimeString = function (iDayName, iDay, iMonthName, iYear, iHour, iMinutes, iSeconds, iCulture) {
               var zoneFormatted = (iCulture.timeZone > 0) ? "+" + iCulture.timeZone : iCulture.timeZone;
               var hourFormatted = (iHour < 10) ? "0" + iHour : iHour;
               var minutesFormatted = (iMinutes < 10) ? "0" + iMinutes : iMinutes;
               var secondsFormatted = (iSeconds < 10) ? "0" + iSeconds : iSeconds;
               return iDayName + ", " + iDay + " " + iMonthName + " " + iYear + " " + hourFormatted + ":" + minutesFormatted + ":" + secondsFormatted + " GMT " + zoneFormatted;
           }
                      
       }

           
       function transformDate(date, currentCulture) {
           var dateCulture = new Culture(currentCulture);
           var dateProcessor = new LocalDateTimeProcessor(date, dateCulture);
           return dateProcessor.computeLocalDateAndTimeAsString();
       }
       
   </script>
    
    
    

   <div id="filtersStrip">
       <div class="grid_4_custom filterStripElement">
         <div id="dateFilterArea">
            <div id="dateLabel" class="filterLabel">
               <img id="includeDateInFilter" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
                  src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
               <span style="vertical-align: middle">
                  <%: Resources.Global.dateLabel %></span>
            </div>
            <div id="datePickersArea">
               <input type="dateTimePicker" id="startDateTimePicker" class="filterDate filterInputBox"
                  value="<%: Resources.Global.fromDate %>"> </input>
               <input type="dateTimePicker" id="endDateTimePicker" class="filterDate filterInputBox"
                  value="<%: Resources.Global.toDate %>"> </input>
            </div>
         </div>
         <div id="starredFilterArea" class="filterLabel">
            <img id="includeStarredInFilter" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
               src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
            <span style="vertical-align: middle">
               <%: Resources.Global.starredLabel %></span>
         </div>
         <div id="unreadFilterArea" class="filterLabel">
            <img id="includeUnreadInFilter" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
               src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
            <span style="vertical-align: middle">
               <%: Resources.Global.readLabel %></span>
         </div>
      </div>
      <div class="grid_6 filterStripElement tagFilterArea">
         <div id="tagsLabel" class="filterLabel">
            <img id="includeTagsInFilter" class="wpItem wpSelectorIcon deletePhoneNumberIconUnselected"
               src="<%: Url.Content("~/Content/images/transparent.gif") %>" />
            <span style="vertical-align: middle">
               <%: Resources.Global.tagsLabel %></span>
         </div>
         <div id="tagFiltering" class="filterInputBox">
            <input name="filterTag" id="filterTag" />
         </div>        
      </div>
   </div>
   <div class="clear"></div>
   <div id="phoneNumbersPool" class="wordwrap tagsPhoneNumbers grid_2">
   </div>
   <div id="conversationsArea" class="grid_4">
      <div id="scrollableconversations" class="conversationbox scrollablebox">
         <div id="conversations" class="conversationbox">
         </div>
         <div id="loadMoreConversations" class="readable">
            Load More Conversations
         </div>
      </div>
   </div>
   <div id="messagesArea" class="grid_6">
      <div id="scrollablemessagebox" class="messagesboxcontainerclass scrollablebox">
         <div id="messagesbox" class="messagesboxclass">
            <span>No conversation selected, please select one</span>
         </div>
      </div>
      <div id="messageTagsSeparator"></div>
      <div id="tagsContainer" class="tagArea invisible">
         <div id="tagsPool" class="tagsPhoneNumbers"></div>
         <input name="tags" id="tags" />
      </div>
      <div id="textareaContainer" class="invisible">
         <div id="replyFormArea">
            <form id="replyToMessageForm">
            <div id="inputTextContainer">
               <textarea id="limitedtextarea" onkeydown="limitText(this.form.limitedtextarea,this.form.countdown,160);"
                  onkeyup="limitText(this.form.limitedtextarea,this.form.countdown,160);" dir="ltr"></textarea>
               <br>
               <div class="clear"></div>
               <span><font size="0.5"><input readonly type="text" name="countdown" size="2" value="160"> </font>
               </span>
            </div>
            </form>
         </div>
         <div id="replyButtonArea">
            <button id="replyBtn"> <%: Resources.Global.sendButton %></button>
         </div>
         <input type="hidden" value="<%: ViewData["currentCulture"] %>" class="currentCulture" />
      </div>
   </div>
</asp:Content>
