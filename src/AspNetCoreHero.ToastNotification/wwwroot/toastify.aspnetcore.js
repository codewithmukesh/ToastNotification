/*!
 * AspNetCoreHero.ToastNotification - Toastify integration
 * https://github.com/codewithmukesh/ToastNotification | MIT
 * No jQuery required.
 */
(function (window, document) {
    'use strict';

    var HEADER = 'X-Toastify-Notifications';
    var TYPES = ['success', 'error', 'warning', 'info', 'custom'];
    var baseConfig = {};
    var clientOptions = { autoHandleAjax: true, types: {} };

    function onReady(fn) {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', fn);
        } else {
            fn();
        }
    }

    function hasValue(value) {
        return value !== null && value !== undefined && value !== '';
    }

    function joinClasses() {
        var result = [];
        for (var i = 0; i < arguments.length; i++) {
            if (hasValue(arguments[i])) {
                var value = String(arguments[i]).trim();
                if (value) { result.push(value); }
            }
        }
        return result.join(' ');
    }

    function parseHeader(value) {
        if (!value) { return []; }
        try {
            return JSON.parse(decodeURIComponent(value));
        } catch (e) {
            if (window.console) { window.console.warn('AspNetCoreHero.ToastNotification: could not read ' + HEADER, e); }
            return [];
        }
    }

    function open(type, message, duration, extra) {
        extra = extra || {};
        var style = (clientOptions.types && clientOptions.types[type]) || {};
        var options = Object.assign({}, baseConfig);
        options.text = hasValue(message) ? String(message) : '';

        if (hasValue(duration)) {
            options.duration = Number(duration);
        }
        if (options.duration === 0) {
            // Sticky toasts must be closable.
            options.close = true;
        }
        options.backgroundColor = hasValue(extra.backgroundColor) ? extra.backgroundColor : style.backgroundColor;
        options.className = joinClasses(baseConfig.className, style.className, extra.className);

        var toast = window.Toastify(options);
        onReady(function () { toast.showToast(); });
        return toast;
    }

    function show(notification) {
        if (!notification) { return undefined; }
        var type = typeof notification.type === 'number'
            ? TYPES[notification.type]
            : String(notification.type || '').toLowerCase();
        return open(type || 'info', notification.message, notification.duration, {
            backgroundColor: type === 'custom' ? notification.backgroundColor : null,
            className: notification.className
        });
    }

    function showAll(notifications) {
        if (!notifications || !notifications.length) { return; }
        for (var i = 0; i < notifications.length; i++) {
            show(notifications[i]);
        }
    }

    // Manually show notifications from a fetch Response, an XMLHttpRequest or a jqXHR.
    // Only needed when AutoHandleAjax is turned off.
    function handleResponse(response) {
        if (!response) { return; }
        var value = null;
        if (response.headers && typeof response.headers.get === 'function') {
            value = response.headers.get(HEADER);
        } else if (typeof response.getResponseHeader === 'function') {
            value = response.getResponseHeader(HEADER);
        }
        showAll(parseHeader(value));
    }

    function init(config, options) {
        baseConfig = Object.assign({}, config || {});
        clientOptions = Object.assign({ autoHandleAjax: true, types: {} }, options || {});
        if (clientOptions.autoHandleAjax && window.AspNetCoreHeroToastAjax) {
            window.AspNetCoreHeroToastAjax.register(HEADER, function (value) { showAll(parseHeader(value)); });
        }
    }

    window.AspNetCoreHeroToastify = {
        init: init,
        show: show,
        showAll: showAll,
        handleResponse: handleResponse,
        success: function (message, duration) { return open('success', message, duration); },
        error: function (message, duration) { return open('error', message, duration); },
        warning: function (message, duration) { return open('warning', message, duration); },
        information: function (message, duration) { return open('info', message, duration); },
        custom: function (message, duration, backgroundColor, className) {
            return open('custom', message, duration, { backgroundColor: backgroundColor, className: className });
        }
    };

    // Boot from the JSON block rendered by the view component (no inline script, so CSP stays happy).
    // Each block is read once, so rendering the component twice never shows a toast twice.
    var blocks = document.querySelectorAll('script[type="application/json"][id="aspnetcorehero-toastify-data"]:not([data-consumed])');
    for (var b = 0; b < blocks.length; b++) {
        blocks[b].setAttribute('data-consumed', 'true');
        try {
            var data = JSON.parse(blocks[b].textContent || '{}');
            if (b === 0) { init(data.config, data.options); }
            showAll(data.notifications);
        } catch (e) {
            if (window.console) { window.console.error('AspNetCoreHero.ToastNotification: could not start Toastify', e); }
        }
    }
})(window, document);
