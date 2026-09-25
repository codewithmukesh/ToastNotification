/*!
 * AspNetCoreHero.ToastNotification - AJAX / fetch integration
 * https://github.com/codewithmukesh/ToastNotification | MIT
 *
 * Adds `X-Requested-With: XMLHttpRequest` to same-origin fetch / XHR calls so the server
 * returns notifications in a response header, then hands that header to the registered toast library.
 * Only same-origin responses are read - a header from another site is never trusted.
 * Safe to load more than once.
 */
(function (window) {
    'use strict';

    function installAjaxHooks() {
        if (window.__aspNetCoreHeroToastAjax) {
            return window.__aspNetCoreHeroToastAjax;
        }
        var registry = window.__aspNetCoreHeroToastAjax = { handlers: [] };

        function isSameOrigin(url) {
            if (!url) { return false; }
            try {
                return new URL(url, window.location.href).origin === window.location.origin;
            } catch (e) {
                return false;
            }
        }

        function dispatch(getHeader) {
            for (var i = 0; i < registry.handlers.length; i++) {
                var handler = registry.handlers[i];
                try {
                    var value = getHeader(handler.header);
                    if (value) {
                        handler.handle(value);
                    }
                } catch (e) {
                    // Never let a toast problem break the app's own request.
                    if (window.console) { window.console.error('AspNetCoreHero.ToastNotification:', e); }
                }
            }
        }

        if (typeof window.fetch === 'function') {
            var originalFetch = window.fetch;
            window.fetch = function (input, init) {
                var isRequest = typeof Request !== 'undefined' && input instanceof Request;
                var url = isRequest ? input.url : String(input);

                if (isSameOrigin(url)) {
                    var headers = new Headers((init && init.headers) || (isRequest ? input.headers : undefined));
                    if (!headers.has('X-Requested-With')) {
                        headers.set('X-Requested-With', 'XMLHttpRequest');
                        if (isRequest && !init) {
                            // Keep the Request's own settings (referrer policy, body, signal...).
                            input = new Request(input, { headers: headers });
                        } else {
                            init = Object.assign({}, init, { headers: headers });
                        }
                    }
                }

                return originalFetch.call(window, input, init).then(function (response) {
                    if (isSameOrigin(response.url || url)) {
                        dispatch(function (name) { return response.headers.get(name); });
                    }
                    return response;
                });
            };
        }

        if (window.XMLHttpRequest) {
            var proto = window.XMLHttpRequest.prototype;
            var originalOpen = proto.open;
            var originalSend = proto.send;
            var originalSetRequestHeader = proto.setRequestHeader;

            proto.open = function (method, url) {
                this.__toastUrl = url;
                this.__toastSameOrigin = isSameOrigin(url);
                this.__toastHasRequestedWith = false;
                return originalOpen.apply(this, arguments);
            };
            proto.setRequestHeader = function (name) {
                if (String(name).toLowerCase() === 'x-requested-with') {
                    this.__toastHasRequestedWith = true;
                }
                return originalSetRequestHeader.apply(this, arguments);
            };
            proto.send = function () {
                var xhr = this;
                if (xhr.__toastSameOrigin && !xhr.__toastHasRequestedWith) {
                    try { originalSetRequestHeader.call(xhr, 'X-Requested-With', 'XMLHttpRequest'); } catch (e) { /* ignore */ }
                }
                if (!xhr.__toastListening) {
                    xhr.__toastListening = true;
                    xhr.addEventListener('load', function () {
                        if (!isSameOrigin(xhr.responseURL || xhr.__toastUrl)) { return; }
                        // Only read headers that are present, so the browser doesn't log "unsafe header" errors.
                        var all = (xhr.getAllResponseHeaders() || '').toLowerCase();
                        dispatch(function (name) {
                            return all.indexOf(name.toLowerCase() + ':') === -1 ? null : xhr.getResponseHeader(name);
                        });
                    });
                }
                return originalSend.apply(this, arguments);
            };
        }

        return registry;
    }

    window.AspNetCoreHeroToastAjax = window.AspNetCoreHeroToastAjax || {
        // Calls handle(headerValue) whenever a same-origin response carries the given header. One handler per header.
        register: function (header, handle) {
            var registry = installAjaxHooks();
            for (var i = 0; i < registry.handlers.length; i++) {
                if (registry.handlers[i].header === header) {
                    registry.handlers[i].handle = handle;
                    return;
                }
            }
            registry.handlers.push({ header: header, handle: handle });
        }
    };
})(window);
