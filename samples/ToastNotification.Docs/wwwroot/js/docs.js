// Docs site behaviour: hero animation, playground, compare, setup wizard, copy buttons.
// None of this is needed in your app - the toast library works on its own.
(function () {
    'use strict';

    var $ = function (selector, root) { return (root || document).querySelector(selector); };
    var $$ = function (selector, root) { return Array.prototype.slice.call((root || document).querySelectorAll(selector)); };
    var reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    function escapeHtml(value) {
        return String(value).replace(/[&<>"']/g, function (c) {
            return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c];
        });
    }

    function highlight(element) {
        if (window.hljs) {
            element.removeAttribute('data-highlighted');
            window.hljs.highlightElement(element);
        }
    }

    // Label table cells so they can stack as cards on small screens.
    $$('.table-scroll table').forEach(function (table) {
        var labels = $$('thead th', table).map(function (th) { return th.textContent.trim(); });
        $$('tbody tr', table).forEach(function (row) {
            $$('td', row).forEach(function (cell, i) { if (labels[i]) { cell.setAttribute('data-label', labels[i]); } });
        });
    });

    // ------------------------------------------------------------------
    // Theme toggle (light by default, remembers your choice)
    // ------------------------------------------------------------------
    var root = document.documentElement;
    try {
        var savedTheme = localStorage.getItem('toast-docs-theme');
        if (savedTheme) { root.setAttribute('data-theme', savedTheme); }
    } catch (e) { /* storage blocked */ }
    var themeToggle = $('.theme-toggle');
    if (themeToggle) {
        themeToggle.addEventListener('click', function () {
            var current = root.getAttribute('data-theme') || 'light';
            var next = current === 'dark' ? 'light' : 'dark';
            root.setAttribute('data-theme', next);
            try { localStorage.setItem('toast-docs-theme', next); } catch (e) { /* storage blocked */ }
        });
    }

    // ------------------------------------------------------------------
    // Mobile menu
    // ------------------------------------------------------------------
    var nav = $('.nav');
    var navToggle = $('.nav-toggle');
    if (nav && navToggle) {
        var setMenu = function (open) {
            nav.classList.toggle('is-open', open);
            navToggle.setAttribute('aria-expanded', String(open));
            navToggle.setAttribute('aria-label', open ? 'Close menu' : 'Open menu');
        };
        navToggle.addEventListener('click', function () { setMenu(!nav.classList.contains('is-open')); });
        $$('.nav-links a').forEach(function (link) { link.addEventListener('click', function () { setMenu(false); }); });
        document.addEventListener('keydown', function (event) { if (event.key === 'Escape') { setMenu(false); } });
    }

    // ------------------------------------------------------------------
    // Docs library toggle: switches snippets, tables, forms and "Try" buttons
    // ------------------------------------------------------------------
    var docsLib = 'notyf';

    function withLibrary(url) {
        return url + (url.indexOf('?') === -1 ? '?' : '&') + 'library=' + docsLib;
    }

    function applyDocsLib(lib) {
        docsLib = lib === 'toastify' ? 'toastify' : 'notyf';
        $$('[data-lib]').forEach(function (el) {
            el.classList.toggle('is-hidden', el.getAttribute('data-lib') !== docsLib);
        });
        $$('[data-lib-input]').forEach(function (input) { input.value = docsLib; });
        $$('[data-docs-lib] button').forEach(function (button) {
            button.setAttribute('aria-pressed', String(button.getAttribute('data-value') === docsLib));
        });
        try { localStorage.setItem('toast-docs-lib', docsLib); } catch (e) { /* storage blocked */ }
    }

    var docsToggle = $('[data-docs-lib]');
    if (docsToggle) {
        docsToggle.addEventListener('click', function (event) {
            var button = event.target.closest('button');
            if (button) { applyDocsLib(button.getAttribute('data-value')); }
        });
        var savedLib = null;
        try { savedLib = localStorage.getItem('toast-docs-lib'); } catch (e) { /* storage blocked */ }
        applyDocsLib(savedLib || 'notyf');
    }

    // The htmx demo follows the toggle too.
    document.addEventListener('htmx:configRequest', function (event) {
        if (event.detail.elt && event.detail.elt.id === 'htmx-demo') {
            event.detail.path = withLibrary('/demo/htmx');
        }
    });

    // Every demo goes through the same Minimal API endpoint. The server raises the toast
    // with INotyfService / IToastifyService and the library shows it from the response header.
    function fire(payload) {
        return fetch('/demo/toast', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });
    }

    // ------------------------------------------------------------------
    // Hero: type a C# call, then pop the toast in the browser mock
    // ------------------------------------------------------------------
    var heroLine = $('#hero-line');
    var mockToasts = $('#mock-toasts');
    if (heroLine && mockToasts) {
        var svc = heroLine.getAttribute('data-service') || 'notyf';
        var script = [
            { call: svc + '.Success("Order placed!");', text: 'Order placed!', color: '#16a34a', icon: '✓' },
            { call: svc + '.Error("Payment failed.");', text: 'Payment failed.', color: '#dc2626', icon: '✕' },
            { call: svc + '.Warning("Stock is running low.");', text: 'Stock is running low.', color: '#d97706', icon: '!' },
            { call: svc + '.Information("Shipping starts Monday.");', text: 'Shipping starts Monday.', color: '#0284c7', icon: 'i' },
            { call: svc + '.Custom("Deployed to production", 5, "#4c33d8");', text: 'Deployed to production', color: '#4c33d8', icon: '↑' }
        ];
        var index = 0;

        var showMock = function (item) {
            var toast = document.createElement('div');
            toast.className = 'mock-toast';
            toast.style.background = item.color;
            toast.innerHTML = '<span class="ico" style="color:' + item.color + '">' + item.icon + '</span>' + escapeHtml(item.text);
            mockToasts.appendChild(toast);
            var all = $$('.mock-toast:not(.is-leaving)', mockToasts);
            if (all.length > 3) {
                all[0].classList.add('is-leaving');
                setTimeout(function () { all[0].remove(); }, 350);
            }
        };

        var typeNext = function () {
            var item = script[index % script.length];
            index++;
            if (reduceMotion) {
                heroLine.textContent = item.call;
                showMock(item);
                setTimeout(typeNext, 2600);
                return;
            }
            var i = 0;
            heroLine.textContent = '';
            var tick = setInterval(function () {
                heroLine.textContent = item.call.slice(0, ++i);
                if (i >= item.call.length) {
                    clearInterval(tick);
                    setTimeout(function () { showMock(item); }, 250);
                    setTimeout(typeNext, 2200);
                }
            }, 32);
        };
        typeNext();
    }

    // ------------------------------------------------------------------
    // Playground
    // ------------------------------------------------------------------
    var pg = $('#pg-controls');
    if (pg) {
        var state = { library: 'notyf', type: 'Success', message: 'Order placed!', duration: null, color: '#4c33d8' };
        var codeEl = $('#pg-code');
        var messageInput = $('#pg-message');
        var colorField = $('#pg-color-field');

        var csharpString = function (value) { return '"' + String(value).replace(/\\/g, '\\\\').replace(/"/g, '\\"') + '"'; };

        var render = function () {
            var service = state.library === 'notyf' ? 'notyf' : 'toastify';
            var iface = state.library === 'notyf' ? 'INotyfService' : 'IToastifyService';
            var args = [csharpString(state.message || '')];
            var durationComment = '';
            if (state.type === 'Custom') {
                args.push(state.duration === null ? 'null' : String(state.duration));
                args.push(csharpString(state.color));
                if (state.library === 'notyf') { args.push('"fa fa-bolt"'); }
            } else if (state.duration !== null) {
                args.push(String(state.duration));
            }
            if (state.duration === 0) { durationComment = ' // 0 = sticky'; }
            else if (state.duration === null) { durationComment = ' // default duration'; }

            var code =
                'public class OrdersController(' + iface + ' ' + service + ') : Controller\n' +
                '{\n' +
                '    [HttpPost]\n' +
                '    public IActionResult Create()\n' +
                '    {\n' +
                '        ' + service + '.' + state.type + '(' + args.join(', ') + ');' + durationComment + '\n' +
                '        return RedirectToAction("Index");\n' +
                '    }\n' +
                '}';
            codeEl.textContent = code;
            highlight(codeEl);
            colorField.classList.toggle('is-hidden', state.type !== 'Custom');
        };

        $$('[data-pg]', pg).forEach(function (group) {
            group.addEventListener('click', function (event) {
                var button = event.target.closest('button');
                if (!button) { return; }
                $$('button', group).forEach(function (b) { b.setAttribute('aria-pressed', String(b === button)); });
                var key = group.getAttribute('data-pg');
                var value = button.getAttribute('data-value');
                state[key] = key === 'duration' ? (value === '' ? null : Number(value)) : value;
                render();
            });
        });

        // Start from whatever the markup marks as pressed (each sample picks its own library).
        $$('[data-pg]', pg).forEach(function (group) {
            var pressed = $('[aria-pressed="true"]', group);
            if (!pressed) { return; }
            var key = group.getAttribute('data-pg');
            var value = pressed.getAttribute('data-value');
            state[key] = key === 'duration' ? (value === '' ? null : Number(value)) : value;
        });

        messageInput.addEventListener('input', function () { state.message = messageInput.value; render(); });

        $('#pg-fire').addEventListener('click', function () {
            fire({
                library: state.library,
                type: state.type,
                message: state.message,
                duration: state.duration,
                color: state.type === 'Custom' ? state.color : null
            });
        });

        render();
    }

    // ------------------------------------------------------------------
    // Buttons with data-fire="library:type" (compare section, docs)
    // ------------------------------------------------------------------
    document.addEventListener('click', function (event) {
        var button = event.target.closest('[data-fire]');
        if (!button) { return; }
        var parts = button.getAttribute('data-fire').split(':');
        if (parts[0] === '@') { parts[0] = docsLib; } // docs buttons follow the library toggle
        var duration = button.getAttribute('data-duration');
        fire({
            library: parts[0],
            type: parts[1],
            message: button.getAttribute('data-message') || null,
            duration: duration === null ? null : Number(duration)
        });
    });

    // fetch / jQuery demos
    document.addEventListener('click', function (event) {
        var fetchButton = event.target.closest('[data-fetch]');
        if (fetchButton) {
            fetch(withLibrary(fetchButton.getAttribute('data-fetch')), { method: 'POST' });
            return;
        }
        var jqueryButton = event.target.closest('[data-jquery]');
        if (jqueryButton && window.jQuery) {
            window.jQuery.post(withLibrary(jqueryButton.getAttribute('data-jquery')));
        }
    });

    // ------------------------------------------------------------------
    // Setup wizard
    // ------------------------------------------------------------------
    var wizard = $('#wizard');
    if (wizard) {
        var choice = { workload: 'mvc', library: 'notyf' };

        var templates = function () {
            var notyf = choice.library === 'notyf';
            var add = notyf ? 'AddNotyf' : 'AddToastify';
            var use = notyf ? 'UseNotyf' : 'UseToastify';
            var component = notyf ? 'Notyf' : 'Toastify';
            var iface = notyf ? 'INotyfService' : 'IToastifyService';
            var svc = notyf ? 'notyf' : 'toastify';
            var framework = choice.workload === 'mvc' ? 'AddControllersWithViews' : 'AddRazorPages';
            var map = choice.workload === 'mvc' ? 'app.MapDefaultControllerRoute();' : 'app.MapRazorPages();';
            var layoutPath = choice.workload === 'mvc' ? 'Views/Shared/_Layout.cshtml' : 'Pages/Shared/_Layout.cshtml';

            var program =
                'var builder = WebApplication.CreateBuilder(args);\n\n' +
                'builder.Services.' + framework + '();\n' +
                'builder.Services.' + add + '();\n\n' +
                'var app = builder.Build();\n\n' +
                'app.UseStaticFiles();\n' +
                'app.UseRouting();\n' +
                'app.' + use + '();\n\n' +
                map + '\n';

            var usage, usageFile;
            if (choice.workload === 'mvc') {
                usageFile = 'Controllers/OrdersController.cs';
                usage =
                    'public class OrdersController(' + iface + ' ' + svc + ') : Controller\n{\n' +
                    '    [HttpPost]\n    public IActionResult Create()\n    {\n' +
                    '        ' + svc + '.Success("Order placed!");\n' +
                    '        return RedirectToAction(nameof(Index));\n    }\n}';
            } else if (choice.workload === 'pages') {
                usageFile = 'Pages/Orders/Create.cshtml.cs';
                usage =
                    'public class CreateModel(' + iface + ' ' + svc + ') : PageModel\n{\n' +
                    '    public IActionResult OnPost()\n    {\n' +
                    '        ' + svc + '.Success("Order placed!");\n' +
                    '        return RedirectToPage("Index");\n    }\n}';
            } else {
                usageFile = 'Program.cs';
                program = program.replace(map + '\n', map + '\n\napp.MapPost("/api/orders", (' + iface + ' ' + svc + ') =>\n{\n    ' + svc + '.Success("Order placed!");\n    return Results.Created();\n});\n');
                usage =
                    '// Call it from any page - the toast shows up on its own\n' +
                    'await fetch("/api/orders", { method: "POST" });';
            }

            return [
                { title: 'Install the package', hint: 'One package - scripts and styles ship inside it.', file: 'Terminal', lang: 'bash', code: 'dotnet add package AspNetCoreHero.ToastNotification' },
                { title: 'Register it', hint: choice.workload === 'minimal' ? 'Minimal APIs still need a Razor layout to render toasts on the page.' : 'No extra using lines needed.', file: 'Program.cs', lang: 'csharp', code: program },
                { title: 'Render it in your layout', hint: 'Once, just before </body>. No jQuery needed.', file: layoutPath, lang: 'html', code: '@await Component.InvokeAsync("' + component + '")' },
                { title: 'Raise a toast', hint: choice.workload === 'minimal' ? 'The endpoint raises it, the page shows it.' : 'Inject the service and call it.', file: usageFile, lang: choice.workload === 'minimal' ? 'javascript' : 'csharp', code: usage }
            ];
        };

        var renderWizard = function () {
            var host = $('#wizard-steps');
            host.innerHTML = templates().map(function (step, i) {
                return '<div class="step"><div class="step-num">' + (i + 1) + '</div><div>' +
                    '<h3>' + escapeHtml(step.title) + '</h3><p>' + escapeHtml(step.hint) + '</p>' +
                    '<div class="code"><div class="code-head"><span>' + escapeHtml(step.file) + '</span>' +
                    '<button class="copy" type="button" data-copy="wizard-' + i + '">copy</button></div>' +
                    '<pre><code id="wizard-' + i + '" class="language-' + step.lang + '">' + escapeHtml(step.code) + '</code></pre></div>' +
                    '</div></div>';
            }).join('');
            $$('code', host).forEach(highlight);
        };

        $$('[data-wizard]', wizard).forEach(function (group) {
            group.addEventListener('click', function (event) {
                var button = event.target.closest('button');
                if (!button) { return; }
                $$('button', group).forEach(function (b) { b.setAttribute('aria-pressed', String(b === button)); });
                choice[group.getAttribute('data-wizard')] = button.getAttribute('data-value');
                renderWizard();
            });
        });

        $$('[data-wizard]', wizard).forEach(function (group) {
            var pressed = $('[aria-pressed="true"]', group);
            if (pressed) { choice[group.getAttribute('data-wizard')] = pressed.getAttribute('data-value'); }
        });
        renderWizard();
    }

    // ------------------------------------------------------------------
    // Copy buttons (event delegation so wizard blocks work too)
    // ------------------------------------------------------------------
    document.addEventListener('click', function (event) {
        var button = event.target.closest('[data-copy]');
        if (!button) { return; }
        var target = document.getElementById(button.getAttribute('data-copy'));
        if (!target || !navigator.clipboard) { return; }
        navigator.clipboard.writeText(target.innerText.trim()).then(function () {
            button.textContent = 'copied';
            button.classList.add('is-done');
            setTimeout(function () { button.textContent = 'copy'; button.classList.remove('is-done'); }, 1400);
        });
    });

    // ------------------------------------------------------------------
    // Scroll reveal + active links
    // ------------------------------------------------------------------
    if ('IntersectionObserver' in window) {
        var revealer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    revealer.unobserve(entry.target);
                }
            });
        }, { rootMargin: '0px 0px -10% 0px' });
        $$('.reveal').forEach(function (el) { revealer.observe(el); });

        var docLinks = $$('.docs-nav a');
        var spy = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) { return; }
                docLinks.forEach(function (link) {
                    link.classList.toggle('is-active', link.getAttribute('href') === '#' + entry.target.id);
                });
            });
        }, { rootMargin: '-35% 0px -60% 0px' });
        $$('.doc[id]').forEach(function (el) { spy.observe(el); });
    } else {
        $$('.reveal').forEach(function (el) { el.classList.add('is-visible'); });
    }

    $$('pre code[class*="language-"]').forEach(highlight);
})();
