
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
