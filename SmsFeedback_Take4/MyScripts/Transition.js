var Transition = function (iSpinnerTarget, iOverlay) {
    "use strict";
    var spinnerTarget = iSpinnerTarget;
    var overlay = iOverlay;
    var opts = {
        lines: 13, // The number of lines to draw
        length: 7, // The length of each line
        width: 4, // The line thickness
        radius: 10, // The radius of the inner circle
        rotate: 0, // The rotation offset
        color: '#fff', // #rgb or #rrggbb
        speed: 1, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: true, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: 'auto', // Top position relative to parent in px
        left: 'auto' // Left position relative to parent in px
    };
    var spinner = new Spinner(opts);

    this.startTransition = function () {
        spinner.spin(spinnerTarget);
        $(overlay).show();
    };

    this.endTransition = function () {
        spinner.stop();
        $(overlay).hide();
    };
};