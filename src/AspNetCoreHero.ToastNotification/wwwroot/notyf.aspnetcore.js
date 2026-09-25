/*!
 * AspNetCoreHero.ToastNotification - Notyf integration
 * https://github.com/codewithmukesh/ToastNotification | MIT
 * No jQuery required.
 */
(function (window, document) {
    'use strict';

    var HEADER = 'X-Notyf-Notifications';
    var TYPES = ['success', 'error', 'warning', 'info', 'custom'];
    var clientOptions = { className: '', autoHandleAjax: true };
    var notyfConfig;

    // Returns the Notyf instance, creating it if needed. Also re-creates it when its container was
    // removed from the page (e.g. htmx hx-boost swapped the <body>).
    function ensureNotyf() {
        var instance = window.notyf;
        if (instance && instance.view && instance.view.container && !document.body.contains(instance.view.container)) {
            instance = null;
        }
        if (!instance) {
            instance = window.notyf = new Notyf(notyfConfig);
        }
        return instance;
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------
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

    function typeDefaults(type) {
        var types = (window.notyf && window.notyf.options && window.notyf.options.types) || [];
        for (var i = 0; i < types.length; i++) {
            if (types[i].type === type) { return types[i]; }
        }
        return {};
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

    function colourNameToHex(colour) {
        var colours = {
            "aliceblue": "#f0f8ff", "antiquewhite": "#faebd7", "aqua": "#00ffff", "aquamarine": "#7fffd4", "azure": "#f0ffff",
            "beige": "#f5f5dc", "bisque": "#ffe4c4", "black": "#000000", "blanchedalmond": "#ffebcd", "blue": "#0000ff", "blueviolet": "#8a2be2", "brown": "#a52a2a", "burlywood": "#deb887",
            "cadetblue": "#5f9ea0", "chartreuse": "#7fff00", "chocolate": "#d2691e", "coral": "#ff7f50", "cornflowerblue": "#6495ed", "cornsilk": "#fff8dc", "crimson": "#dc143c", "cyan": "#00ffff",
            "darkblue": "#00008b", "darkcyan": "#008b8b", "darkgoldenrod": "#b8860b", "darkgray": "#a9a9a9", "darkgreen": "#006400", "darkkhaki": "#bdb76b", "darkmagenta": "#8b008b", "darkolivegreen": "#556b2f",
            "darkorange": "#ff8c00", "darkorchid": "#9932cc", "darkred": "#8b0000", "darksalmon": "#e9967a", "darkseagreen": "#8fbc8f", "darkslateblue": "#483d8b", "darkslategray": "#2f4f4f", "darkturquoise": "#00ced1",
            "darkviolet": "#9400d3", "deeppink": "#ff1493", "deepskyblue": "#00bfff", "dimgray": "#696969", "dodgerblue": "#1e90ff",
            "firebrick": "#b22222", "floralwhite": "#fffaf0", "forestgreen": "#228b22", "fuchsia": "#ff00ff",
            "gainsboro": "#dcdcdc", "ghostwhite": "#f8f8ff", "gold": "#ffd700", "goldenrod": "#daa520", "gray": "#808080", "green": "#008000", "greenyellow": "#adff2f",
            "honeydew": "#f0fff0", "hotpink": "#ff69b4",
            "indianred": "#cd5c5c", "indigo": "#4b0082", "ivory": "#fffff0", "khaki": "#f0e68c",
            "lavender": "#e6e6fa", "lavenderblush": "#fff0f5", "lawngreen": "#7cfc00", "lemonchiffon": "#fffacd", "lightblue": "#add8e6", "lightcoral": "#f08080", "lightcyan": "#e0ffff", "lightgoldenrodyellow": "#fafad2",
            "lightgrey": "#d3d3d3", "lightgreen": "#90ee90", "lightpink": "#ffb6c1", "lightsalmon": "#ffa07a", "lightseagreen": "#20b2aa", "lightskyblue": "#87cefa", "lightslategray": "#778899", "lightsteelblue": "#b0c4de",
            "lightyellow": "#ffffe0", "lime": "#00ff00", "limegreen": "#32cd32", "linen": "#faf0e6",
            "magenta": "#ff00ff", "maroon": "#800000", "mediumaquamarine": "#66cdaa", "mediumblue": "#0000cd", "mediumorchid": "#ba55d3", "mediumpurple": "#9370d8", "mediumseagreen": "#3cb371", "mediumslateblue": "#7b68ee",
            "mediumspringgreen": "#00fa9a", "mediumturquoise": "#48d1cc", "mediumvioletred": "#c71585", "midnightblue": "#191970", "mintcream": "#f5fffa", "mistyrose": "#ffe4e1", "moccasin": "#ffe4b5",
            "navajowhite": "#ffdead", "navy": "#000080",
            "oldlace": "#fdf5e6", "olive": "#808000", "olivedrab": "#6b8e23", "orange": "#ffa500", "orangered": "#ff4500", "orchid": "#da70d6",
            "palegoldenrod": "#eee8aa", "palegreen": "#98fb98", "paleturquoise": "#afeeee", "palevioletred": "#d87093", "papayawhip": "#ffefd5", "peachpuff": "#ffdab9", "peru": "#cd853f", "pink": "#ffc0cb", "plum": "#dda0dd", "powderblue": "#b0e0e6", "purple": "#800080",
            "rebeccapurple": "#663399", "red": "#ff0000", "rosybrown": "#bc8f8f", "royalblue": "#4169e1",
            "saddlebrown": "#8b4513", "salmon": "#fa8072", "sandybrown": "#f4a460", "seagreen": "#2e8b57", "seashell": "#fff5ee", "sienna": "#a0522d", "silver": "#c0c0c0", "skyblue": "#87ceeb", "slateblue": "#6a5acd", "slategray": "#708090", "snow": "#fffafa", "springgreen": "#00ff7f", "steelblue": "#4682b4",
            "tan": "#d2b48c", "teal": "#008080", "thistle": "#d8bfd8", "tomato": "#ff6347", "turquoise": "#40e0d0",
            "violet": "#ee82ee",
            "wheat": "#f5deb3", "white": "#ffffff", "whitesmoke": "#f5f5f5",
            "yellow": "#ffff00", "yellowgreen": "#9acd32"
        };
        if (!hasValue(colour)) { return false; }
        var key = String(colour).trim().toLowerCase();
        return colours.hasOwnProperty(key) ? colours[key] : false;
    }

    // Returns 'text-dark' for light backgrounds and 'text-white' for dark ones.
    // Returns '' when the colour can't be parsed (gradients, css variables, ...).
    function pickTextColorBasedOnBgColorAdvanced(bgColor) {
        if (!hasValue(bgColor)) { return ''; }
        var value = String(bgColor).trim();
        var rgb = null;
        var hex = value.charAt(0) === '#' ? value.substring(1) : (colourNameToHex(value) || '').substring(1);

        if (/^[0-9a-f]{3}$/i.test(hex)) {
            hex = hex.charAt(0) + hex.charAt(0) + hex.charAt(1) + hex.charAt(1) + hex.charAt(2) + hex.charAt(2);
        }
        if (/^[0-9a-f]{6}/i.test(hex)) {
            rgb = [parseInt(hex.substring(0, 2), 16), parseInt(hex.substring(2, 4), 16), parseInt(hex.substring(4, 6), 16)];
        } else {
            var match = /^rgba?\(\s*(\d+)[\s,]+(\d+)[\s,]+(\d+)/i.exec(value);
            if (match) { rgb = [+match[1], +match[2], +match[3]]; }
        }
        if (!rgb) { return ''; }

        var c = [];
        for (var i = 0; i < 3; i++) {
            var col = rgb[i] / 255;
            c.push(col <= 0.03928 ? col / 12.92 : Math.pow((col + 0.055) / 1.055, 2.4));
        }
        var luminance = (0.2126 * c[0]) + (0.7152 * c[1]) + (0.0722 * c[2]);
        return (luminance > 0.179) ? 'text-dark' : 'text-white';
    }

    // ---------------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------------
    function open(type, message, duration, extra) {
        if (document.readyState === 'loading' || !document.body) {
            onReady(function () { open(type, message, duration, extra); });
            return undefined;
        }
        var notyf = ensureNotyf();
        extra = extra || {};
        var defaults = typeDefaults(type);
        var options = { type: type, message: hasValue(message) ? String(message) : '' };

        if (hasValue(duration)) {
            options.duration = Number(duration);
            if (options.duration === 0) {
                // Sticky toasts must be closable.
                options.dismissible = true;
            }
        }
        var className = joinClasses(defaults.className, clientOptions.className, extra.className);
        if (className) { options.className = className; }
        if (hasValue(extra.background)) { options.background = extra.background; }
        if (extra.icon) { options.icon = extra.icon; }

        return notyf.open(options);
    }

    function custom(message, duration, color, iconClass, className) {
        var background = hasValue(color) ? color : typeDefaults('custom').background;
        var textClass = pickTextColorBasedOnBgColorAdvanced(background);
        var icon = hasValue(iconClass) ? { className: joinClasses(iconClass, textClass), tagName: 'i' } : undefined;
        return open('custom', message, duration, {
            className: joinClasses(textClass, className),
            background: background,
            icon: icon
        });
    }

    function show(notification) {
        if (!notification) { return undefined; }
        var type = typeof notification.type === 'number'
            ? TYPES[notification.type]
            : String(notification.type || '').toLowerCase();
        if (type === 'custom') {
            return custom(notification.message, notification.duration, notification.backgroundColor, notification.icon, notification.className);
        }
        return open(type || 'info', notification.message, notification.duration, { className: notification.className });
    }

    function showAll(notifications) {
        if (!notifications || !notifications.length) { return; }
        onReady(function () {
            for (var i = 0; i < notifications.length; i++) {
                show(notifications[i]);
            }
        });
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
        clientOptions = Object.assign({ className: '', autoHandleAjax: true }, options || {});
        notyfConfig = config;
        if (document.body) { ensureNotyf(); } else { onReady(ensureNotyf); }

        if (clientOptions.autoHandleAjax) {
            window.AspNetCoreHeroToastAjax.register(HEADER, function (value) { showAll(parseHeader(value)); });
        }
    }

    window.AspNetCoreHeroNotyf = {
        init: init,
        show: show,
        showAll: showAll,
        handleResponse: handleResponse,
        success: function (message, duration) { return open('success', message, duration); },
        error: function (message, duration) { return open('error', message, duration); },
        warning: function (message, duration) { return open('warning', message, duration); },
        information: function (message, duration) { return open('info', message, duration); },
        custom: custom
    };

    // v1 globals, kept for backwards compatibility.
    window.toastNotifySuccess = window.AspNetCoreHeroNotyf.success;
    window.toastNotifyError = window.AspNetCoreHeroNotyf.error;
    window.toastNotifyWarning = window.AspNetCoreHeroNotyf.warning;
    window.toastNotifyInformation = window.AspNetCoreHeroNotyf.information;
    window.toastNotifyCustom = custom;
    window.colourNameToHex = colourNameToHex;
    window.pickTextColorBasedOnBgColorAdvanced = pickTextColorBasedOnBgColorAdvanced;

    // v1 workaround for AJAX calls. With AutoHandleAjax on (the default) notifications are already
    // shown automatically, so this does nothing to avoid showing them twice.
    window.getResponseHeaders = function (jqXHR) {
        if (clientOptions.autoHandleAjax) { return; }
        handleResponse(jqXHR);
    };

    // Boot from the JSON block rendered by the view component (no inline script, so CSP stays happy).
    // Each block is read once, so rendering the component twice never shows a toast twice.
    var blocks = document.querySelectorAll('script[type="application/json"][id="aspnetcorehero-notyf-data"]:not([data-consumed])');
    for (var b = 0; b < blocks.length; b++) {
        blocks[b].setAttribute('data-consumed', 'true');
        try {
            var data = JSON.parse(blocks[b].textContent || '{}');
            if (b === 0) { init(data.config, data.options); }
            showAll(data.notifications);
        } catch (e) {
            if (window.console) { window.console.error('AspNetCoreHero.ToastNotification: could not start Notyf', e); }
        }
    }
})(window, document);
