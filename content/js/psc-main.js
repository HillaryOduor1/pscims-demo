/* ============================================================
   PSCIMS — psc-main.js
   ES5 only. No transpilation. Targets IE11+ and all modern browsers.
   Moved from inline Site.Master block. Public API surface unchanged:
   PSCIMS.toggleSidebar, toggleMobile, toggleTheme, toast, btnLoading, init.
   ============================================================ */
(function () {
    'use strict';

    /* ---------- Shims (feature-detection + fallback) ---------- */
    function on(el, evt, fn) {
        if (!el) { return; }
        if (el.addEventListener) {
            el.addEventListener(evt, fn, false);
        } else if (el.attachEvent) {
            el.attachEvent('on' + evt, function () { fn.call(el, window.event); });
        }
    }

    function off(el, evt, fn) {
        if (!el) { return; }
        if (el.removeEventListener) {
            el.removeEventListener(evt, fn, false);
        } else if (el.detachEvent) {
            el.detachEvent('on' + evt, fn);
        }
    }

    function addClass(el, c) {
        if (!el) { return; }
        if (el.classList) {
            el.classList.add(c);
        } else {
            if (el.className.indexOf(c) === -1) {
                el.className += ' ' + c;
            }
        }
    }

    function removeClass(el, c) {
        if (!el) { return; }
        if (el.classList) {
            el.classList.remove(c);
        } else {
            el.className = el.className
                .replace(new RegExp('(^|\\s)' + c + '(\\s|$)'), ' ')
                .replace(/^\s+|\s+$/g, '');
        }
    }

    function hasClass(el, c) {
        if (!el) { return false; }
        if (el.classList) { return el.classList.contains(c); }
        return new RegExp('(^|\\s)' + c + '(\\s|$)').test(el.className);
    }

    function setAttr(el, name, value) {
        if (!el) { return; }
        el.setAttribute(name, value);
    }

    function getAttr(el, name) {
        if (!el) { return null; }
        return el.getAttribute(name);
    }

    function removeChild(el) {
        if (el && el.parentNode) {
            el.parentNode.removeChild(el);
        }
    }

    /* setTimeout-based debounce; uses rAF if present only to schedule the
       timer dispatch (never to run user work inside rAF callbacks). */
    function debounce(fn, wait) {
        var t = null;
        return function () {
            var ctx = this, args = arguments;
            if (t) { clearTimeout(t); }
            t = setTimeout(function () {
                t = null;
                fn.apply(ctx, args);
            }, wait);
        };
    }

    /* Safe localStorage wrapper */
    function lsGet(key) {
        try { return window.localStorage.getItem(key); } catch (e) { return null; }
    }
    function lsSet(key, val) {
        try { window.localStorage.setItem(key, val); return true; } catch (e) { return false; }
    }

    /* matchMedia guard (IE9 doesn't have it; IE10+ does) */
    function isMobileViewport() {
        if (window.matchMedia) {
            return window.matchMedia('(max-width: 900px)').matches;
        }
        return window.innerWidth <= 900;
    }

    /* ---------- Drawer state (single source of truth) ---------- */
    var FOCUSABLE_SEL = 'a[href], button:not([disabled]), textarea:not([disabled]), ' +
        'input:not([disabled]), select:not([disabled]), [tabindex]:not([tabindex="-1"])';

    var savedScrollY = 0;
    var drawerOpen = false;

    function getShell()    { return document.getElementById('shell'); }
    function getDrawer()   { return document.getElementById('sidebar'); }
    function getOverlay()  { return document.getElementById('sidebarOverlay'); }
    function getToggle()   { return document.getElementById('sidebarToggle'); }

    function focusablesIn(el) {
        if (!el || !el.querySelectorAll) { return []; }
        var nodes = el.querySelectorAll(FOCUSABLE_SEL);
        var out = [];
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].offsetParent !== null) { out.push(nodes[i]); }
        }
        return out;
    }

    function openMobile() {
        var shell = getShell();
        var overlay = getOverlay();
        var toggle = getToggle();
        var drawer = getDrawer();
        if (!shell || !overlay || !drawer) { return; }
        if (drawerOpen) { return; }
        drawerOpen = true;

        savedScrollY = (window.pageYOffset !== undefined)
            ? window.pageYOffset
            : (document.documentElement && document.documentElement.scrollTop) || 0;

        setAttr(shell, 'data-mobile-open', 'true');
        setAttr(overlay, 'aria-hidden', 'false');
        setAttr(overlay, 'tabindex', '0');
        setAttr(toggle, 'aria-expanded', 'true');

        addClass(document.body, 'is-drawer-open');
        document.body.style.top = '-' + savedScrollY + 'px';

        /* Focus the first focusable inside the drawer */
        var f = focusablesIn(drawer);
        if (f.length) {
            try { f[0].focus(); } catch (e) {}
        }
    }

    function closeMobile() {
        var shell = getShell();
        var overlay = getOverlay();
        var toggle = getToggle();
        if (!shell) { return; }
        if (!drawerOpen) {
            setAttr(shell, 'data-mobile-open', 'false');
            setAttr(toggle, 'aria-expanded', 'false');
            return;
        }
        drawerOpen = false;

        setAttr(shell, 'data-mobile-open', 'false');
        setAttr(overlay, 'aria-hidden', 'true');
        setAttr(overlay, 'tabindex', '-1');
        setAttr(toggle, 'aria-expanded', 'false');

        removeClass(document.body, 'is-drawer-open');
        document.body.style.top = '';

        /* Restore scroll */
        window.scrollTo(0, savedScrollY);
        savedScrollY = 0;

        /* Restore focus to toggle */
        if (toggle) {
            try { toggle.focus(); } catch (e) {}
        }
    }

    function toggleMobile() {
        if (drawerOpen) { closeMobile(); } else { openMobile(); }
    }

    function toggleDesktopSidebar() {
        var shell = getShell();
        var toggle = getToggle();
        if (!shell) { return; }
        var collapsed = getAttr(shell, 'data-collapsed') === 'true';
        setAttr(shell, 'data-collapsed', collapsed ? 'false' : 'true');
        lsSet('pscims-sidebar-collapsed', collapsed ? 'false' : 'true');
        /* aria-expanded reflects "is the sidebar expanded" on desktop */
        setAttr(toggle, 'aria-expanded', collapsed ? 'true' : 'false');
    }

    /* ---------- Focus trap ---------- */
    function trapFocus(e) {
        if (!drawerOpen) { return; }
        var key = e.key || e.keyCode;
        var isTab = (key === 'Tab' || key === 9);
        if (!isTab) { return; }
        var drawer = getDrawer();
        var f = focusablesIn(drawer);
        if (!f.length) { return; }
        var first = f[0], last = f[f.length - 1];
        var active = document.activeElement;
        if (e.shiftKey) {
            if (active === first || active === drawer) {
                e.preventDefault();
                last.focus();
            }
        } else {
            if (active === last) {
                e.preventDefault();
                first.focus();
            }
        }
    }

    /* ---------- Key handler ---------- */
    function onKeydown(e) {
        var key = e.key || e.keyCode;
        if (key === 'Escape' || key === 27) {
            if (drawerOpen) {
                closeMobile();
            }
            return;
        }
        trapFocus(e);
    }

    /* ---------- Resize / orientation guard ---------- */
    var onResize = debounce(function () {
        if (isMobileViewport()) { return; }
        if (drawerOpen) {
            /* Crossed 900px while drawer open — force-close without focus
               jump (focus is now irrelevant because toggle is on desktop). */
            var shell = getShell();
            var overlay = getOverlay();
            setAttr(shell, 'data-mobile-open', 'false');
            setAttr(overlay, 'aria-hidden', 'true');
            setAttr(overlay, 'tabindex', '-1');
            removeClass(document.body, 'is-drawer-open');
            document.body.style.top = '';
            window.scrollTo(0, savedScrollY);
            savedScrollY = 0;
            drawerOpen = false;
        }
    }, 150);

    /* ---------- Public API ---------- */
    window.PSCIMS = window.PSCIMS || {};

    window.PSCIMS.toggleSidebar = function () {
        if (isMobileViewport()) {
            toggleMobile();
        } else {
            toggleDesktopSidebar();
        }
    };

    window.PSCIMS.toggleMobile = function () {
        toggleMobile();
    };

    window.PSCIMS.toggleTheme = function () {
        var html = document.documentElement;
        var current = html.getAttribute('data-theme') || 'light';
        var next = current === 'dark' ? 'light' : 'dark';
        html.setAttribute('data-theme', next);
        lsSet('pscims-theme', next);
    };

    window.PSCIMS.toast = function (type, title, desc, timeout) {
        var stack = document.getElementById('toastStack');
        if (!stack) { return; }
        var el = document.createElement('div');
        el.className = 'toast toast--' + (type || 'info');
        el.setAttribute('role', 'status');

        /* Build children with DOM API (avoids innerHTML XSS surface) */
        if (title) {
            var tp = document.createElement('p');
            tp.className = 'toast__title';
            tp.appendChild(document.createTextNode(title));
            el.appendChild(tp);
        }
        if (desc) {
            var dp = document.createElement('p');
            dp.className = 'toast__desc';
            dp.appendChild(document.createTextNode(desc));
            el.appendChild(dp);
        }
        stack.appendChild(el);

        var ms = timeout || 4000;
        setTimeout(function () {
            el.style.transition = 'opacity .2s, transform .2s';
            el.style.opacity = '0';
            el.style.transform = 'translateX(8px)';
            setTimeout(function () { removeChild(el); }, 220);
        }, ms);
    };

    window.PSCIMS.btnLoading = function (btn, message) {
        if (!btn) { return true; }
        var now = new Date().getTime();
        var last = parseInt(btn.getAttribute('data-last-submit') || '0', 10);
        if (last && (now - last) < 5000) {
            return false;
        }

        /* Synchronous disable — prevents double-submit on slow networks */
        btn.setAttribute('data-last-submit', String(now));
        var original = btn.value || btn.textContent || 'Submit';
        btn.setAttribute('data-original-text', original);

        if (btn.form && typeof btn.form.checkValidity === 'function') {
            if (!btn.form.checkValidity()) {
                /* Let the browser show validation UI; don't disable */
                return true;
            }
        }

        btn.disabled = true;
        addClass(btn, 'is-loading');

        /* Deferred visual swap (does not affect submission) */
        setTimeout(function () {
            if (btn.tagName === 'INPUT') {
                btn.value = message || 'Please wait\u2026';
            } else {
                /* Replace content with spinner + text */
                while (btn.firstChild) { btn.removeChild(btn.firstChild); }
                var sp = document.createElement('span');
                sp.className = 'spinner';
                sp.setAttribute('aria-hidden', 'true');
                btn.appendChild(sp);
                btn.appendChild(document.createTextNode(' ' + (message || 'Please wait\u2026')));
            }

            /* Disable other submit buttons in the same form to prevent
               parallel submissions */
            var others = document.querySelectorAll('input[type=submit], button[type=submit]');
            for (var i = 0; i < others.length; i++) {
                if (others[i] !== btn) {
                    others[i].disabled = true;
                }
            }
        }, 0);

        return true;
    };

    window.PSCIMS.init = function () {
        if (window.PSCIMS._inited) { return; }
        window.PSCIMS._inited = true;

        var shell = getShell();
        if (!shell) { return; }

        /* Restore desktop collapsed state from localStorage */
        if (lsGet('pscims-sidebar-collapsed') === 'true') {
            setAttr(shell, 'data-collapsed', 'true');
        } else {
            setAttr(shell, 'data-collapsed', 'false');
        }

        /* Bind toggle */
        var toggle = getToggle();
        on(toggle, 'click', function (e) {
            if (e && e.preventDefault) { e.preventDefault(); }
            window.PSCIMS.toggleSidebar();
        });

        /* Bind overlay (click + keyboard) */
        var overlay = getOverlay();
        on(overlay, 'click', function (e) {
            if (e && e.preventDefault) { e.preventDefault(); }
            closeMobile();
        });
        on(overlay, 'keydown', function (e) {
            var key = e.key || e.keyCode;
            if (key === 'Enter' || key === 13 || key === ' ' || key === 32) {
                if (e && e.preventDefault) { e.preventDefault(); }
                closeMobile();
            }
        });

        /* Close drawer when a sidebar link is tapped (mobile only). Use a
           delegated handler so postback-rendered links are covered. */
        var drawer = getDrawer();
        on(drawer, 'click', function (e) {
            var target = e.target || e.srcElement;
            while (target && target !== drawer) {
                if (target.className && /\bsidebar__link\b/.test(target.className)) {
                    if (isMobileViewport()) {
                        /* Do not preventDefault — allow navigation/postback */
                        closeMobile();
                    }
                    return;
                }
                target = target.parentNode;
            }
        });

        /* Escape + focus trap */
        on(document, 'keydown', onKeydown);

        /* Resize / orientation */
        on(window, 'resize', onResize);
        on(window, 'orientationchange', onResize);

        /* Initial aria sync */
        setAttr(getToggle(), 'aria-expanded',
            getAttr(shell, 'data-collapsed') === 'true' ? 'false' : 'true');
    };

    /* Boot */
    if (document.readyState === 'complete' || document.readyState === 'interactive') {
        /* Defer so inline per-page scripts (if any) can define things first */
        setTimeout(window.PSCIMS.init, 0);
    } else {
        on(document, 'DOMContentLoaded', window.PSCIMS.init);
    }
})();