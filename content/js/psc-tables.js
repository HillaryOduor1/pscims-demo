/* ============================================================
   PSCIMS — psc-tables.js
   ES5 only. Targets IE9+.
   Adds class="js-stack" and data-label="<th text>" to every
   table.table inside .table-wrap, so the responsive CSS in
   psc-theme.css can render the table as a stacked-card list
   below 768px.

   Does NOT:
   - clone or move nodes
   - touch server-control IDs
   - bind or unbind any events
   - parse innerHTML
   - depend on any library
   ============================================================ */
(function () {
    'use strict';

    function trim(s) {
        return (s || '').replace(/^\s+|\s+$/g, '');
    }

    function addClass(el, c) {
        if (!el) { return; }
        if (el.classList) {
            el.classList.add(c);
        } else if (el.className.indexOf(c) === -1) {
            el.className += ' ' + c;
        }
    }

    function processTables() {
        if (!document.querySelectorAll) { return; }
        var wraps = document.querySelectorAll('.table-wrap');
        for (var w = 0; w < wraps.length; w++) {
            var tables = wraps[w].getElementsByTagName('table');
            for (var t = 0; t < tables.length; t++) {
                var tbl = tables[t];
                /* Only process tables with class "table" (the site's
                   data-grid class), and only once per element. */
                if (!tbl.className || !/\btable\b/.test(tbl.className)) { continue; }
                if (/\bjs-stack\b/.test(tbl.className)) { continue; }

                var heads = tbl.getElementsByTagName('thead');
                if (!heads.length) { continue; }
                var ths = heads[0].getElementsByTagName('th');
                if (!ths.length) { continue; }

                var labels = [];
                for (var h = 0; h < ths.length; h++) {
                    var txt = ths[h].textContent || ths[h].innerText || '';
                    labels.push(trim(txt));
                }

                var bodies = tbl.getElementsByTagName('tbody');
                for (var b = 0; b < bodies.length; b++) {
                    var rows = bodies[b].getElementsByTagName('tr');
                    for (var r = 0; r < rows.length; r++) {
                        var cells = rows[r].children || rows[r].getElementsByTagName('td');
                        for (var c = 0; c < cells.length; c++) {
                            var label = labels[c];
                            if (typeof label === 'undefined' || label === null) { label = ''; }
                            cells[c].setAttribute('data-label', label);
                        }
                    }
                }

                addClass(tbl, 'js-stack');
            }
        }
    }

    function init() {
        if (init._inited) { return; }
        init._inited = true;
        processTables();
    }

    if (document.readyState === 'complete' || document.readyState === 'interactive') {
        setTimeout(init, 0);
    } else if (document.addEventListener) {
        document.addEventListener('DOMContentLoaded', init, false);
    } else if (document.attachEvent) {
        document.attachEvent('onreadystatechange', function () {
            if (document.readyState === 'complete') { init(); }
        });
    }
})();