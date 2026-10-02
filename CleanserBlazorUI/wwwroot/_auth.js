// Sign-in page behaviour that needs no circuit (these pages are static):
//  - show/hide password buttons   (data-cx="show-pw" data-target="<input id>")
//  - live password strength + rules checklist on forms marked data-cx-pwform
// The server enforces every rule again; this only gives instant feedback.
(function () {
    document.addEventListener("click", function (e) {
        const btn = e.target.closest('[data-cx="show-pw"]');
        if (!btn) return;
        const input = document.getElementById(btn.dataset.target);
        if (!input) return;
        const show = input.type === "password";
        input.type = show ? "text" : "password";
        btn.setAttribute("aria-label", show ? "Hide password" : "Show password");
    });

    function initForm(form) {
        if (form.dataset.cxReady) return;
        form.dataset.cxReady = "1";

        const temp = form.querySelector('[data-pw="temp"]');
        const nw = form.querySelector('[data-pw="new"]');
        const confirm = form.querySelector('[data-pw="confirm"]');
        const submit = form.querySelector("[data-pw-submit]");
        const label = form.querySelector("[data-pw-strength]");
        const bars = form.querySelectorAll(".cx-meter i");
        if (!nw || !confirm) return;

        function update() {
            const t = temp ? temp.value : "";
            const p = nw.value;
            const c = confirm.value;
            const r = {
                len: p.length >= 8,
                case: /[a-z]/.test(p) && /[A-Z]/.test(p),
                num: /\d/.test(p),
                sym: /[^A-Za-z0-9]/.test(p),
                match: p.length > 0 && p === c
            };
            if (temp) r.diff = p.length > 0 && p !== t;

            form.querySelectorAll(".cx-rules li").forEach(function (li) {
                li.classList.toggle("ok", !!r[li.dataset.rule]);
            });

            const score = ["len", "case", "num", "sym"].filter(function (k) { return r[k]; }).length;
            bars.forEach(function (bar, i) { bar.className = i < score ? "on-" + score : ""; });
            if (label) {
                label.textContent = ["Too weak", "Weak", "Fair", "Good", "Strong"][score];
                label.classList.toggle("ok", score === 4);
            }

            if (submit) {
                const all = Object.keys(r).every(function (k) { return r[k]; });
                submit.disabled = !(all && (!temp || t.length > 0));
            }
        }

        [temp, nw, confirm].forEach(function (el) { if (el) el.addEventListener("input", update); });
        update();
    }

    // Sign-in style forms: show a spinner and block a second click while the request is in flight.
    document.addEventListener("submit", function (e) {
        const form = e.target.closest(".cx-auth-form");
        const btn = form && form.querySelector('button[type="submit"].cx-btn');
        if (!btn || btn.getAttribute("aria-busy") === "true") return;
        setTimeout(function () { btn.setAttribute("aria-busy", "true"); }, 0);
    });
    // Coming back with the browser's Back button must not leave the button stuck in its busy state.
    window.addEventListener("pageshow", function (e) {
        if (e.persisted) document.querySelectorAll('.cx-btn[aria-busy="true"]').forEach(function (b) { b.removeAttribute("aria-busy"); });
    });

    function boot() { document.querySelectorAll("[data-cx-pwform]").forEach(initForm); }

    if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", boot); else boot();
    if (window.Blazor && Blazor.addEventListener) Blazor.addEventListener("enhancedload", boot);
})();
