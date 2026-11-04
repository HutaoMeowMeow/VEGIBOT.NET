/* VegiBot — Dashboard interactions */
(function () {
    'use strict';

    /* ── Sidebar toggle (mobile) ─────────────────────── */
    var sidebar   = document.getElementById('sidebar');
    var toggleBtn = document.getElementById('sidebarToggle');
    var backdrop  = document.getElementById('sidebarBackdrop');

    function openSidebar() {
        sidebar.classList.add('open');
        backdrop.classList.add('active');
        toggleBtn.setAttribute('aria-expanded', 'true');
        document.body.style.overflow = 'hidden';
    }

    function closeSidebar() {
        sidebar.classList.remove('open');
        backdrop.classList.remove('active');
        toggleBtn.setAttribute('aria-expanded', 'false');
        document.body.style.overflow = '';
    }

    if (toggleBtn) {
        toggleBtn.addEventListener('click', function () {
            sidebar.classList.contains('open') ? closeSidebar() : openSidebar();
        });
    }

    if (backdrop) {
        backdrop.addEventListener('click', closeSidebar);
    }

    // Close sidebar on Escape key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && sidebar && sidebar.classList.contains('open')) {
            closeSidebar();
        }
    });

    /* ── Task completion toggle (demo) ───────────────── */
    var taskList = document.getElementById('tasks-section');

    if (taskList) {
        taskList.addEventListener('click', function (e) {
            var btn = e.target.closest('.task-check-btn');
            if (!btn) return;

            var item = btn.closest('.task-item');
            if (!item) return;

            var isDone = item.classList.toggle('task-done');

            btn.setAttribute(
                'aria-label',
                isDone ? 'Mark incomplete' : 'Mark complete'
            );
            btn.setAttribute(
                'title',
                isDone ? 'Mark incomplete' : 'Mark complete'
            );

            // Brief visual feedback
            item.style.transition = 'opacity 0.1s';
            item.style.opacity = '0.6';
            setTimeout(function () { item.style.opacity = ''; }, 120);
        });
    }

    /* ── Close sidebar when a nav link is clicked on mobile ─── */
    var navItems = document.querySelectorAll('.nav-item');
    navItems.forEach(function (item) {
        item.addEventListener('click', function () {
            if (window.innerWidth <= 768) closeSidebar();
        });
    });

    /* ── Animate progress bars on load ──────────────── */
    var fills = document.querySelectorAll('.plant-progress-fill');
    fills.forEach(function (fill) {
        var target = fill.style.width;
        fill.style.width = '0%';
        requestAnimationFrame(function () {
            setTimeout(function () { fill.style.width = target; }, 80);
        });
    });

})();
