/* VegiBot — Login page interactions */
(function () {
    'use strict';

    /* ── Password visibility toggle ── */
    var passwordInput  = document.getElementById('passwordInput');
    var toggleBtn      = document.getElementById('togglePassword');
    var eyeIcon        = document.getElementById('eyeIcon');
    var eyeOffIcon     = document.getElementById('eyeOffIcon');

    if (toggleBtn && passwordInput) {
        toggleBtn.addEventListener('click', function () {
            var isPassword = passwordInput.type === 'password';
            passwordInput.type  = isPassword ? 'text' : 'password';
            eyeIcon.style.display    = isPassword ? 'none'  : '';
            eyeOffIcon.style.display = isPassword ? ''      : 'none';
            toggleBtn.setAttribute('aria-label', isPassword ? 'Hide password' : 'Show password');
            toggleBtn.setAttribute('title',      isPassword ? 'Hide password' : 'Show password');
        });
    }

    /* ── Submit: loading state ── */
    var form      = document.getElementById('loginForm');
    var submitBtn = document.getElementById('submitBtn');

    if (form && submitBtn) {
        form.addEventListener('submit', function () {
            var btnText    = submitBtn.querySelector('.btn-text');
            var btnSpinner = submitBtn.querySelector('.btn-spinner');

            submitBtn.disabled = true;

            if (btnText)    btnText.textContent    = 'Signing in…';
            if (btnSpinner) btnSpinner.style.display = '';
        });
    }

    /* ── Add error class to invalid inputs after submit attempt ── */
    var inputs = document.querySelectorAll('.field-input');
    inputs.forEach(function (input) {
        /* Check initial server-side error state */
        var group = input.closest('.field-group');
        if (group) {
            var errorSpan = group.querySelector('.field-error');
            if (errorSpan && errorSpan.textContent.trim() !== '') {
                input.classList.add('input-error');
            }
        }

        input.addEventListener('input', function () {
            if (input.value.trim() !== '') {
                input.classList.remove('input-error');
            }
        });
    });

})();
