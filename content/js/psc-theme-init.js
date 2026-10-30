/* PSCIMS — Theme Initialization & Toggle */
(function () {
    'use strict';

    try {
        var saved = localStorage.getItem('pscims-theme');
        var prefersDark = false;

        if (window.matchMedia) {
            prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        }

        var theme = (saved === 'light' || saved === 'dark')
            ? saved
            : (prefersDark ? 'dark' : 'light');

        document.documentElement.setAttribute('data-theme', theme);
    } catch (e) {
        document.documentElement.setAttribute('data-theme', 'light');
    }

    // Expose the toggle function globally for the event delegation in psc-main.js
    window.PSCIMS = window.PSCIMS || {};
    window.PSCIMS.toggleTheme = function () {
        var html = document.documentElement;
        var current = html.getAttribute('data-theme') || 'light';
        var next = current === 'dark' ? 'light' : 'dark';
        html.setAttribute('data-theme', next);
        try { localStorage.setItem('pscims-theme', next); } catch (e) {}
    };
})();