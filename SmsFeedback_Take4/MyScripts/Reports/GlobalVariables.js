/*global window */
/*global DateHelper */
window.app = window.app || {};
window.app.calendarCulture = "en-GB";
window.app.currentWorkingPoint = "Global";
window.app.currentWorkingPointFriendlyName = "Global";
window.app.areas = [];
var startDate = new Date();
var endDate = new Date();
startDate.setDate(endDate.getDate() - 31);
window.app.startDate = startDate;
window.app.endDate = endDate;
window.app.dateHelper = new DateHelper();
window.app.newStartDate = startDate;
window.app.newEndDate = endDate;