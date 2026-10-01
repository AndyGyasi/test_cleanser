// Sidebar behaviour that has to work on statically rendered pages too (no
// circuit needed): section collapse, menu search, mobile drawer, "/" shortcut.
// Everything is delegated from document, so it survives Blazor's enhanced
// navigation swapping the DOM underneath it.
(function () {
    const COOKIE = "cleanser-nav-closed";

    function readClosed() {
        const m = document.cookie.match(new RegExp("(?:^|; )" + COOKIE + "=([^;]*)"));
        return m ? decodeURIComponent(m[1]).split("|").filter(Boolean) : [];
    }
    function writeClosed(ids) {
        document.cookie = COOKIE + "=" + encodeURIComponent(ids.join("|")) + ";path=/;max-age=31536000;SameSite=Lax";
    }
    function shell() { return document.getElementById("cx-shell"); }

    document.addEventListener("click", function (e) {
        const link = e.target.closest(".cx-nav-item");
        if (link) shell()?.classList.remove("cx-nav-open");

        const t = e.target.closest("[data-cx]");
        if (!t) return;
        const action = t.dataset.cx;

        if (action === "toggle-group") {
            const group = t.closest(".cx-nav-group");
            const closed = group.classList.toggle("closed");
            t.setAttribute("aria-expanded", closed ? "false" : "true");
            const ids = new Set(readClosed());
            if (closed) ids.add(group.dataset.group); else ids.delete(group.dataset.group);
            writeClosed([...ids]);
        } else if (action === "open-nav") {
            shell()?.classList.add("cx-nav-open");
        } else if (action === "close-nav") {
            shell()?.classList.remove("cx-nav-open");
        }
    });

    document.addEventListener("input", function (e) {
        if (e.target.id !== "cx-nav-search") return;
        const q = e.target.value.trim().toLowerCase();
        const closed = new Set(readClosed());
        document.querySelectorAll(".cx-nav-group").forEach(function (g) {
            let any = false;
            g.querySelectorAll(".cx-nav-item").forEach(function (a) {
                const hit = !q || a.dataset.label.includes(q);
                a.style.display = hit ? "" : "none";
                any = any || hit;
            });
            g.style.display = any ? "" : "none";
            if (q) g.classList.remove("closed");
            else g.classList.toggle("closed", closed.has(g.dataset.group));
        });
    });

    document.addEventListener("keydown", function (e) {
        if (e.key === "/" && !/INPUT|TEXTAREA|SELECT/.test(document.activeElement.tagName)) {
            const s = document.getElementById("cx-nav-search");
            if (s) { e.preventDefault(); s.focus(); }
        }
        if (e.key === "Escape") shell()?.classList.remove("cx-nav-open");
    });
})();
