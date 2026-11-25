// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


(function () {
    // Idle countdown (display only, resets on activity)
    var idle = document.getElementById('idle');
    if (idle) {
        var left = 15 * 60;
        setInterval(function () {
            left = Math.max(0, left - 1);
            idle.textContent = Math.floor(left / 60) + ':' + String(left % 60).padStart(2, '0');
        }, 1000);
        ['click', 'keydown'].forEach(function (e) {
            document.addEventListener(e, function () { left = 15 * 60; });
        });
    }

    // 30-second code window bar
    var win = document.getElementById('win'), wl = document.getElementById('wl');
    if (win) {
        setInterval(function () {
            var r = 30 - ((Date.now() / 1000) % 30);
            win.style.width = (r / 30 * 100) + '%';
            wl.textContent = Math.ceil(r) + 's';
        }, 1000);
    }

    // Verify page: switch between authenticator code and recovery code
    var toggle = document.getElementById('toggle');
    if (toggle) {
        toggle.addEventListener('click', function () {
            var totp = document.getElementById('mode-totp'), rec = document.getElementById('mode-rec');
            var showRec = rec.classList.contains('d-none');
            rec.classList.toggle('d-none', !showRec);
            totp.classList.toggle('d-none', showRec);
            document.getElementById('verify-title').textContent = showRec ? 'Use a recovery code' : 'Enter your code';
            toggle.textContent = showRec ? 'Back to authenticator code' : 'Lost your device? Use a recovery code';
        });
    }

    // Recovery codes page: enable Continue only after confirming
    var saved = document.getElementById('saved'), go = document.getElementById('goBtn');
    if (saved && go) {
        saved.addEventListener('change', function () { go.classList.toggle('disabled', !saved.checked); });
    }
})();