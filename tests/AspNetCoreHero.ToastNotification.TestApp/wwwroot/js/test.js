// External file on purpose: the ?csp=1 pages block inline scripts.
function on(id, handler) {
    var element = document.getElementById(id);
    if (element) { element.addEventListener('click', handler); }
}

on('btn-fetch', function () { fetch('/Home/NotyfAjax?client=fetch'); });
on('btn-fetch-error', function () { fetch('/Home/NotyfAjaxError'); });
on('btn-xhr', function () {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', '/Home/NotyfAjax?client=xhr');
    xhr.send();
});
on('btn-jquery', function () { window.jQuery.get('/Home/NotyfAjax', { client: 'jquery' }); });
on('btn-toastify-fetch', function () { fetch('/Home/ToastifyAjax'); });
on('btn-ajax-redirect', function () { fetch('/Home/NotyfAjaxRedirect'); });
on('btn-cross-origin', function () {
    var url = new URL('/cors-evil', location.href);
    url.hostname = url.hostname === 'localhost' ? '127.0.0.1' : 'localhost';
    fetch(url).then(function () { document.body.setAttribute('data-cross-done', '1'); });
});
