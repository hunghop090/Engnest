
    // ---------------------------------------------------------------------------------
    // Sapo Js API - EngnestFlash
    //
    // Working with Global Notifications.
    // ---------------------------------------------------------------------------------

    $(document).on("ready page:load", function () {
        var $container = $(".ajax-notification-message");
        var hasError = $container.hasClass("error");

        var message = $container.text();
        if (message !== null && message != "")
            EngnestFlash.display(message, hasError);
    });

    EngnestFlash = {
        error: function (message, timeout, parent) {
            if (timeout == null)
                timeout = 5e3;
            if (parent == null)
                EngnestFlash.display(message, true, timeout);
            else
                EngnestFlash.displayparent(message, true, timeout);

        },
        notice: function (message, timeout, parent) {
            if (timeout == null)
                timeout = 3e3;
            if (parent == null)
                EngnestFlash.display(message, false, timeout);
            else
                EngnestFlash.displayparent(message, false, timeout);
        },
        display: function (message, hasError, timeout) {
            var $container = $(".ajax-notification");
            if (timeout == null)
                timeout = 3e3;

            clearTimeout(timeout);
            $(".ajax-notification-message").text(message);
            if (hasError != null) {
                $container.toggleClass("has-errors", hasError);
                if (hasError) {
                    $container.find(".ajax-notification-message").prepend('<i class="fa fa-times-circle" aria-hidden="true" style="padding-right: 4px;"></i>');
                } else {
                    $container.find(".ajax-notification-message").prepend('<i class="fa fa-check-square" aria-hidden="true" style="padding-right: 4px;"></i>');
                }
            }
            $container.addClass("is-visible");
			setTimeout(EngnestFlash.hide.bind(this), timeout);
        },
        displayparent: function (message, hasError, timeout) {
            var $container = $(".ajax-notification");

            if (hasError != null)
                $container.toggleClass("has-errors", hasError);
            if (timeout == null)
                timeout = 3e3;

            clearTimeout(timeout);
            $(".ajax-notification-message").text(message);
            $(".close-notification").attr("onlick", "EngnestFlash.hideparent()");
            $container.addClass("is-visible");

            setTimeout(EngnestFlash.hideparent.bind(this), timeout);
        },
        hide: function () {

            $(".ajax-notification").removeClass("is-visible");
            $(".ajax-notification-message").text("");
        },
        hideparent: function () {

            $(".ajax-notification").removeClass("is-visible");
            $(".ajax-notification-message").text("");
        },
        popup: function (url, width, height, top) {
            $(".popup-box").dialog("destroy");
            $(".popup-box").show();
            $(".popup-box").dialog({
                width: width,
                height: height,
                top: top,
                closeText: "",
                title: 'Pop up hệ thống',
                modal: true,
                padding: 0,
                resizable: false,
                draggable: false,
                show: { effect: "toggle", fold: 1000 },
                hide: { effect: "toggle", fold: 1000 },
                //buttons: [
                //            {
                //                text: "Lưu", class: "btn btn-success button-dialoge ", id: "btn-add-2-Prd", tabindex: "17", name: "savePrd", click: function () { document.getElementById("popup").contentWindow.$(".form-add-Prd #btn-add-2-Prd").click(); }
                //            },
                //            {
                //                text: "Lưu & Thêm mới", class: "btn btn-success button-dialoge", tabindex: "18", name: "savePrd", id: "btn-add-1-Prd", click: function () { document.getElementById("popup").contentWindow.$(".form-add-Prd #btn-add-1-Prd").click(); }
                //            }],
                left: 60,
                position: "fixed",
                open: function (ev, ui) {
                    $('.ui-dialog-buttonpane')
                        .find('button:nth-child(1)')
                        .prepend(' <span class=" glyphicon glyphicon-floppy-save"></span>');

                    $('#popup').attr('src', url);

                },
                close: function (ev, ui) {
                    $(".popup-box").dialog("destroy");
                }
            });
            $(".ui-widget-overlay").bind("click", function () {
                $('.popup-box').dialog().dialog("close");

            });
        }
};
function time_ago(time) {

	switch (typeof time) {
		case 'number':
			break;
		case 'string':
			time = +new Date(time);
			break;
		case 'object':
			if (time.constructor === Date) time = time.getTime();
			break;
		default:
			time = +new Date();
	}
	var time_formats = [
		[60, 'seconds', 1], // 60
		[120, '1 minute ago', '1 minute from now'], // 60*2
		[3600, 'minutes', 60], // 60*60, 60
		[7200, '1 hour ago', '1 hour from now'], // 60*60*2
		[86400, 'hours', 3600], // 60*60*24, 60*60
		[172800, 'Yesterday', 'Tomorrow'], // 60*60*24*2
		[604800, 'days', 86400], // 60*60*24*7, 60*60*24
		[1209600, 'Last week', 'Next week'], // 60*60*24*7*4*2
		[2419200, 'weeks', 604800], // 60*60*24*7*4, 60*60*24*7
		[4838400, 'Last month', 'Next month'], // 60*60*24*7*4*2
		[29030400, 'months', 2419200], // 60*60*24*7*4*12, 60*60*24*7*4
		[58060800, 'Last year', 'Next year'], // 60*60*24*7*4*12*2
		[2903040000, 'years', 29030400], // 60*60*24*7*4*12*100, 60*60*24*7*4*12
		[5806080000, 'Last century', 'Next century'], // 60*60*24*7*4*12*100*2
		[58060800000, 'centuries', 2903040000] // 60*60*24*7*4*12*100*20, 60*60*24*7*4*12*100
	];
	var seconds = (+new Date() - time) / 1000,
		token = 'ago',
		list_choice = 1;

	if (seconds == 0) {
		return 'Just now'
	}
	if (seconds < 0) {
		seconds = Math.abs(seconds);
		token = 'from now';
		list_choice = 2;
	}
	var i = 0,
		format;
	while (format = time_formats[i++])
		if (seconds < format[0]) {
			if (typeof format[2] == 'string')
				return format[list_choice];
			else
				return Math.floor(seconds / format[2]) + ' ' + format[1] + ' ' + token;
		}
	return time;
}

function getdate(d) {
	var curr_date = d.getDate();
	var curr_month = d.getMonth() + 1;
	var curr_year = d.getFullYear();
	return curr_date + "/" + curr_month + "/" + curr_year;
}
var Wo_Delay = (function () {
	var timer = 0;
	return function (callback, ms) {
		clearTimeout(timer);
		timer = setTimeout(callback, ms);
	};
})();
