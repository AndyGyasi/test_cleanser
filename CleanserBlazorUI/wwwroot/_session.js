// Idle-session handling for signed-in pages (a running job also keeps the session alive; see JobTracker). The server is the authority: it ends
// a session when no request arrives within the admin-set window (SessionIdle).
// This script adds the two things the server cannot see on its own:
//  1. Real user activity -- on an interactive page, clicks and typing travel over the
//     live connection, not as HTTP requests, so the script tells the server the user is
//     still here by calling /session/keepalive (at most every few minutes).
//  2. A page left open and unattended -- once the window passes with no activity the
//     page is sent to /session/timeout, which signs the user out.
// Activity is shared across tabs through localStorage, so working in one tab keeps the others alive.
(function () {
    const meta = document.querySelector('meta[name="cx-idle-minutes"]');
    if (!meta) return; // not signed in

    const idleMs = Math.max(1, parseInt(meta.content, 10) || 30) * 60000;
    const pingEvery = Math.min(5 * 60000, Math.floor(idleMs / 3));
    const KEY = "cx-last-activity";
    let last = Date.now();
    let lastPing = Date.now();
    let lastWrite = 0;
    let ended = false;

    function sharedLast() {
        try { return parseInt(localStorage.getItem(KEY), 10) || 0; } catch (e) { return 0; }
    }

    function end() {
        if (ended) return;
        ended = true;
        location.assign("/session/timeout");
    }

    async function ping() {
        lastPing = Date.now();
        try {
            const r = await fetch("/session/keepalive", { method: "POST", credentials: "same-origin" });
            if (r.status === 401) end();
        } catch (e) { /* offline: try again on the next activity */ }
    }

    function onActivity() {
        const now = Date.now();
        last = now;
        if (now - lastWrite > 5000) {
            lastWrite = now;
            try { localStorage.setItem(KEY, String(now)); } catch (e) { /* storage unavailable */ }
        }
        if (now - lastPing > pingEvery) ping();
    }

    ["mousemove", "mousedown", "keydown", "wheel", "scroll", "touchstart", "click"].forEach(function (name) {
        window.addEventListener(name, onActivity, { passive: true, capture: true });
    });

    // Once this page has seen no activity for the whole window, ask the server before
    // ending anything: it also knows about running jobs (cleaning, referencing...), which
    // keep a session alive without any mouse or keyboard activity. 401 = really over.
    let checking = false;
    async function confirmIdle() {
        if (checking) return;
        checking = true;
        try {
            const r = await fetch("/session/check", { method: "POST", credentials: "same-origin" });
            if (r.status === 401) end();
            else last = Date.now() - idleMs + 60000; // still active on the server: look again in a minute
        } catch (e) { /* offline: try again on the next tick */ }
        finally { checking = false; }
    }

    setInterval(function () {
        if (Date.now() - Math.max(last, sharedLast()) >= idleMs) confirmIdle();
    }, 15000);

    onActivity();
})();
