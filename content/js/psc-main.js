/* ============================================================
   PSCIMS — psc-main.js
   ES5 only. Targets IE11+ and all modern browsers.
   ============================================================ */
(function () {
    'use strict';

    /* ---------- Shims ---------- */
    function on(el, evt, fn) {
        if (!el) { return; }
        if (el.addEventListener) {
            el.addEventListener(evt, fn, false);
        } else if (el.attachEvent) {
            el.attachEvent('on' + evt, function () { fn.call(el, window.event); });
        }
    }

    function addClass(el, c) {
        if (!el) { return; }
        if (el.classList) {
            el.classList.add(c);
        } else {
            if (el.className.indexOf(c) === -1) { el.className += ' ' + c; }
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

    function getAttr(el, name) {
        if (!el) { return null; }
        return el.getAttribute(name);
    }

    function setAttr(el, name, value) {
        if (!el) { return; }
        el.setAttribute(name, value);
    }

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

    function isMobileViewport() {
        if (window.matchMedia) {
            return window.matchMedia('(max-width: 900px)').matches;
        }
        return window.innerWidth <= 900;
    }

    /* ---------- Drawer state ---------- */
    var FOCUSABLE_SEL = 'a[href], button:not([disabled]), textarea:not([disabled]), input:not([disabled]), select:not([disabled]), [tabindex]:not([tabindex="-1"])';
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
        var shell = getShell(), overlay = getOverlay(), toggle = getToggle(), drawer = getDrawer();
        if (!shell || !overlay || !drawer || drawerOpen) { return; }
        drawerOpen = true;
        savedScrollY = (window.pageYOffset !== undefined) ? window.pageYOffset : (document.documentElement && document.documentElement.scrollTop) || 0;

        setAttr(shell, 'data-mobile-open', 'true');
        setAttr(overlay, 'aria-hidden', 'false');
        setAttr(overlay, 'tabindex', '0');
        setAttr(toggle, 'aria-expanded', 'true');

        addClass(document.body, 'is-drawer-open');
        document.body.style.top = '-' + savedScrollY + 'px';

        var f = focusablesIn(drawer);
        if (f.length) { try { f[0].focus(); } catch (e) {} }
    }

    function closeMobile() {
        var shell = getShell(), overlay = getOverlay(), toggle = getToggle();
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
        window.scrollTo(0, savedScrollY);
        savedScrollY = 0;

        if (toggle) { try { toggle.focus(); } catch (e) {} }
    }

    function toggleMobile() {
        if (drawerOpen) { closeMobile(); } else { openMobile(); }
    }

    function toggleDesktopSidebar() {
        var shell = getShell(), toggle = getToggle();
        if (!shell) { return; }
        var collapsed = getAttr(shell, 'data-collapsed') === 'true';
        setAttr(shell, 'data-collapsed', collapsed ? 'false' : 'true');
        try { localStorage.setItem('pscims-sidebar-collapsed', collapsed ? 'false' : 'true'); } catch (e) {}
        setAttr(toggle, 'aria-expanded', collapsed ? 'true' : 'false');
    }

    function trapFocus(e) {
        if (!drawerOpen) { return; }
        var key = e.key || e.keyCode;
        if (key !== 'Tab' && key !== 9) { return; }
        var drawer = getDrawer();
        var f = focusablesIn(drawer);
        if (!f.length) { return; }
        var first = f[0], last = f[f.length - 1], active = document.activeElement;
        if (e.shiftKey) {
            if (active === first || active === drawer) { e.preventDefault(); last.focus(); }
        } else {
            if (active === last) { e.preventDefault(); first.focus(); }
        }
    }

    function onKeydown(e) {
        var key = e.key || e.keyCode;
        if (key === 'Escape' || key === 27) {
            if (drawerOpen) { closeMobile(); }
            return;
        }
        trapFocus(e);
    }

    var onResize = debounce(function () {
        if (isMobileViewport()) { return; }
        if (drawerOpen) {
            var shell = getShell(), overlay = getOverlay();
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
        if (isMobileViewport()) { toggleMobile(); } else { toggleDesktopSidebar(); }
    };
    window.PSCIMS.toggleMobile = function () { toggleMobile(); };

    window.PSCIMS.toggleTheme = function () {
        var html = document.documentElement;
        var current = html.getAttribute('data-theme') || 'light';
        var next = current === 'dark' ? 'light' : 'dark';
        html.setAttribute('data-theme', next);
        try { localStorage.setItem('pscims-theme', next); } catch (e) {}
    };

    window.PSCIMS.toast = function (type, title, desc, timeout) {
        var stack = document.getElementById('toastStack');
        if (!stack) { return; }
        var el = document.createElement('div');
        el.className = 'toast toast--' + (type || 'info');
        el.setAttribute('role', 'status');
        
        if (title) {
            var tp = document.createElement('p'); tp.className = 'toast__title';
            tp.appendChild(document.createTextNode(title)); el.appendChild(tp);
        }
        if (desc) {
            var dp = document.createElement('p'); dp.className = 'toast__desc';
            dp.appendChild(document.createTextNode(desc)); el.appendChild(dp);
        }
        stack.appendChild(el);
        setTimeout(function () {
            el.style.transition = 'opacity .2s, transform .2s';
            el.style.opacity = '0';
            el.style.transform = 'translateX(8px)';
            setTimeout(function () { if (el.parentNode) { el.parentNode.removeChild(el); } }, 220);
        }, timeout || 4000);
    };

    window.PSCIMS.btnLoading = function (btn, message) {
        if (!btn) { return true; }
        
        // If the form is invalid, let the browser handle validation natively without locking the button
        if (btn.form && typeof btn.form.checkValidity === 'function' && !btn.form.checkValidity()) {
            return true;
        }

        var now = new Date().getTime();
        var last = parseInt(btn.getAttribute('data-last-submit') || '0', 10);
        
        // Block double-clicks within 1.5 seconds ONLY
        if (last && (now - last) < 1500) { return false; }

        btn.setAttribute('data-last-submit', String(now));
        var original = btn.value || btn.textContent || 'Submit';
        btn.setAttribute('data-original-text', original);

        btn.disabled = true;
        addClass(btn, 'is-loading');

        // Show spinner text
        setTimeout(function () {
            if (btn.tagName === 'INPUT') {
                btn.value = message || 'Please wait\u2026';
            } else {
                while (btn.firstChild) { btn.removeChild(btn.firstChild); }
                var sp = document.createElement('span'); sp.className = 'spinner'; sp.setAttribute('aria-hidden', 'true');
                btn.appendChild(sp);
                btn.appendChild(document.createTextNode(' ' + (message || 'Please wait\u2026')));
            }
            // Disable other submit buttons to prevent conflicting posts
            var others = document.querySelectorAll('input[type=submit], button[type=submit]');
            for (var i = 0; i < others.length; i++) { if (others[i] !== btn) { others[i].disabled = true; } }
        }, 0);

        // FAILSAFE: Re-enable the button after 8 seconds if the page hasn't redirected
        setTimeout(function () {
            if (btn.disabled) {
                btn.disabled = false;
                removeClass(btn, 'is-loading');
                var orig = btn.getAttribute('data-original-text');
                if (btn.tagName === 'INPUT') {
                    btn.value = orig;
                } else {
                    while (btn.firstChild) { btn.removeChild(btn.firstChild); }
                    btn.appendChild(document.createTextNode(orig));
                }
            }
        }, 8000);

        return true;
    };

    /* ---------- Init ---------- */
    window.PSCIMS.init = function () {
        if (window.PSCIMS._inited) { return; }
        window.PSCIMS._inited = true;

        var shell = getShell();
        if (!shell) { return; }

        try {
            if (localStorage.getItem('pscims-sidebar-collapsed') === 'true') {
                setAttr(shell, 'data-collapsed', 'true');
            } else {
                setAttr(shell, 'data-collapsed', 'false');
            }
        } catch(e) {}

        on(document, 'click', function (e) {
            var target = e.target || e.srcElement;
            while (target && target !== document) {
                var action = getAttr(target, 'data-action');
                if (action === 'toggle-theme') {
                    if (window.PSCIMS.toggleTheme) { window.PSCIMS.toggleTheme(); }
                    return;
                }
                if (action === 'toggle-sidebar') {
                    window.PSCIMS.toggleSidebar();
                    return;
                }
                target = target.parentNode;
            }
        });

        var overlay = getOverlay();
        on(overlay, 'click', function (e) { if (e && e.preventDefault) { e.preventDefault(); } closeMobile(); });
        on(overlay, 'keydown', function (e) {
            var key = e.key || e.keyCode;
            if (key === 'Enter' || key === 13 || key === ' ' || key === 32) {
                if (e && e.preventDefault) { e.preventDefault(); } closeMobile();
            }
        });

        var drawer = getDrawer();
        on(drawer, 'click', function (e) {
            var target = e.target || e.srcElement;
            while (target && target !== drawer) {
                if (target.className && /\bsidebar__link\b/.test(target.className)) {
                    if (isMobileViewport()) { closeMobile(); }
                    return;
                }
                target = target.parentNode;
            }
        });

        on(document, 'keydown', onKeydown);
        on(window, 'resize', onResize);
        on(window, 'orientationchange', onResize);

        setAttr(getToggle(), 'aria-expanded', getAttr(shell, 'data-collapsed') === 'true' ? 'false' : 'true');
    };

    if (document.readyState === 'complete' || document.readyState === 'interactive') {
        setTimeout(window.PSCIMS.init, 0);
    } else {
        on(document, 'DOMContentLoaded', window.PSCIMS.init);
    }
})();

/*
/* ============================================================
   PSCIMS — psc-main.js
   ES5 only. Targets IE11+ and all modern browsers.
   ============================================================ /
(function () {
    'use strict';

    /* ---------- Shims ---------- *
    function on(el, evt, fn) {
        if (!el) { return; }
        if (el.addEventListener) {
            el.addEventListener(evt, fn, false);
        } else if (el.attachEvent) {
            el.attachEvent('on' + evt, function () { fn.call(el, window.event); });
        }
    }

    function addClass(el, c) {
        if (!el) { return; }
        if (el.classList) {
            el.classList.add(c);
        } else {
            if (el.className.indexOf(c) === -1) { el.className += ' ' + c; }
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

    function getAttr(el, name) {
        if (!el) { return null; }
        return el.getAttribute(name);
    }

    function setAttr(el, name, value) {
        if (!el) { return; }
        el.setAttribute(name, value);
    }

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

    function isMobileViewport() {
        if (window.matchMedia) {
            return window.matchMedia('(max-width: 900px)').matches;
        }
        return window.innerWidth <= 900;
    }

    /* ---------- Drawer state ---------- /
    var FOCUSABLE_SEL = 'a[href], button:not([disabled]), textarea:not([disabled]), input:not([disabled]), select:not([disabled]), [tabindex]:not([tabindex="-1"])';
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
        var shell = getShell(), overlay = getOverlay(), toggle = getToggle(), drawer = getDrawer();
        if (!shell || !overlay || !drawer || drawerOpen) { return; }
        drawerOpen = true;
        savedScrollY = (window.pageYOffset !== undefined) ? window.pageYOffset : (document.documentElement && document.documentElement.scrollTop) || 0;

        setAttr(shell, 'data-mobile-open', 'true');
        setAttr(overlay, 'aria-hidden', 'false');
        setAttr(overlay, 'tabindex', '0');
        setAttr(toggle, 'aria-expanded', 'true');

        addClass(document.body, 'is-drawer-open');
        document.body.style.top = '-' + savedScrollY + 'px';

        var f = focusablesIn(drawer);
        if (f.length) { try { f[0].focus(); } catch (e) {} }
    }

    function closeMobile() {
        var shell = getShell(), overlay = getOverlay(), toggle = getToggle();
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
        window.scrollTo(0, savedScrollY);
        savedScrollY = 0;

        if (toggle) { try { toggle.focus(); } catch (e) {} }
    }

    function toggleMobile() {
        if (drawerOpen) { closeMobile(); } else { openMobile(); }
    }

    function toggleDesktopSidebar() {
        var shell = getShell(), toggle = getToggle();
        if (!shell) { return; }
        var collapsed = getAttr(shell, 'data-collapsed') === 'true';
        setAttr(shell, 'data-collapsed', collapsed ? 'false' : 'true');
        try { localStorage.setItem('pscims-sidebar-collapsed', collapsed ? 'false' : 'true'); } catch (e) {}
        setAttr(toggle, 'aria-expanded', collapsed ? 'true' : 'false');
    }

    function trapFocus(e) {
        if (!drawerOpen) { return; }
        var key = e.key || e.keyCode;
        if (key !== 'Tab' && key !== 9) { return; }
        var drawer = getDrawer();
        var f = focusablesIn(drawer);
        if (!f.length) { return; }
        var first = f[0], last = f[f.length - 1], active = document.activeElement;
        if (e.shiftKey) {
            if (active === first || active === drawer) { e.preventDefault(); last.focus(); }
        } else {
            if (active === last) { e.preventDefault(); first.focus(); }
        }
    }

    function onKeydown(e) {
        var key = e.key || e.keyCode;
        if (key === 'Escape' || key === 27) {
            if (drawerOpen) { closeMobile(); }
            return;
        }
        trapFocus(e);
    }

    var onResize = debounce(function () {
        if (isMobileViewport()) { return; }
        if (drawerOpen) {
            var shell = getShell(), overlay = getOverlay();
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

    /* ---------- Public API ---------- /
    window.PSCIMS = window.PSCIMS || {};

    window.PSCIMS.toggleSidebar = function () {
        if (isMobileViewport()) { toggleMobile(); } else { toggleDesktopSidebar(); }
    };
    window.PSCIMS.toggleMobile = function () { toggleMobile(); };

    window.PSCIMS.toggleTheme = function () {
        var html = document.documentElement;
        var current = html.getAttribute('data-theme') || 'light';
        var next = current === 'dark' ? 'light' : 'dark';
        html.setAttribute('data-theme', next);
        try { localStorage.setItem('pscims-theme', next); } catch (e) {}
    };

    window.PSCIMS.toast = function (type, title, desc, timeout) {
        var stack = document.getElementById('toastStack');
        if (!stack) { return; }
        var el = document.createElement('div');
        el.className = 'toast toast--' + (type || 'info');
        el.setAttribute('role', 'status');
        
        if (title) {
            var tp = document.createElement('p'); tp.className = 'toast__title';
            tp.appendChild(document.createTextNode(title)); el.appendChild(tp);
        }
        if (desc) {
            var dp = document.createElement('p'); dp.className = 'toast__desc';
            dp.appendChild(document.createTextNode(desc)); el.appendChild(dp);
        }
        stack.appendChild(el);
        setTimeout(function () {
            el.style.transition = 'opacity .2s, transform .2s';
            el.style.opacity = '0';
            el.style.transform = 'translateX(8px)';
            setTimeout(function () { if (el.parentNode) { el.parentNode.removeChild(el); } }, 220);
        }, timeout || 4000);
    };

    window.PSCIMS.btnLoading = function (btn, message) {
        if (!btn) { return true; }
        var now = new Date().getTime();
        var last = parseInt(btn.getAttribute('data-last-submit') || '0', 10);
        if (last && (now - last) < 5000) { return false; }

        btn.setAttribute('data-last-submit', String(now));
        var original = btn.value || btn.textContent || 'Submit';
        btn.setAttribute('data-original-text', original);

        if (btn.form && typeof btn.form.checkValidity === 'function' && !btn.form.checkValidity()) {
            return true;
        }

        btn.disabled = true;
        addClass(btn, 'is-loading');

        setTimeout(function () {
            if (btn.tagName === 'INPUT') {
                btn.value = message || 'Please wait\u2026';
            } else {
                while (btn.firstChild) { btn.removeChild(btn.firstChild); }
                var sp = document.createElement('span'); sp.className = 'spinner'; sp.setAttribute('aria-hidden', 'true');
                btn.appendChild(sp);
                btn.appendChild(document.createTextNode(' ' + (message || 'Please wait\u2026')));
            }
            var others = document.querySelectorAll('input[type=submit], button[type=submit]');
            for (var i = 0; i < others.length; i++) { if (others[i] !== btn) { others[i].disabled = true; } }
        }, 0);

        return true;
    };

    /* ---------- Init ---------- /
    window.PSCIMS.init = function () {
        if (window.PSCIMS._inited) { return; }
        window.PSCIMS._inited = true;

        var shell = getShell();
        if (!shell) { return; }

        try {
            if (localStorage.getItem('pscims-sidebar-collapsed') === 'true') {
                setAttr(shell, 'data-collapsed', 'true');
            } else {
                setAttr(shell, 'data-collapsed', 'false');
            }
        } catch(e) {}

        on(document, 'click', function (e) {
            var target = e.target || e.srcElement;
            while (target && target !== document) {
                var action = getAttr(target, 'data-action');
                if (action === 'toggle-theme') {
                    if (window.PSCIMS.toggleTheme) { window.PSCIMS.toggleTheme(); }
                    return;
                }
                if (action === 'toggle-sidebar') {
                    window.PSCIMS.toggleSidebar();
                    return;
                }
                target = target.parentNode;
            }
        });

        var overlay = getOverlay();
        on(overlay, 'click', function (e) { if (e && e.preventDefault) { e.preventDefault(); } closeMobile(); });
        on(overlay, 'keydown', function (e) {
            var key = e.key || e.keyCode;
            if (key === 'Enter' || key === 13 || key === ' ' || key === 32) {
                if (e && e.preventDefault) { e.preventDefault(); } closeMobile();
            }
        });

        var drawer = getDrawer();
        on(drawer, 'click', function (e) {
            var target = e.target || e.srcElement;
            while (target && target !== drawer) {
                if (target.className && /\bsidebar__link\b/.test(target.className)) {
                    if (isMobileViewport()) { closeMobile(); }
                    return;
                }
                target = target.parentNode;
            }
        });

        on(document, 'keydown', onKeydown);
        on(window, 'resize', onResize);
        on(window, 'orientationchange', onResize);

        setAttr(getToggle(), 'aria-expanded', getAttr(shell, 'data-collapsed') === 'true' ? 'false' : 'true');
    };

    if (document.readyState === 'complete' || document.readyState === 'interactive') {
        setTimeout(window.PSCIMS.init, 0);
    } else {
        on(document, 'DOMContentLoaded', window.PSCIMS.init);
    }
})();*/