// Theme preference lives in a cookie (not localStorage) so it's readable
// server-side during static rendering too -- Account pages like Login can't
// use JS interop (no live circuit yet), but they can read a request cookie.
window.cleanserTheme = {
    get: function () {
        const match = document.cookie.match(/(?:^|; )cleanser-theme=([^;]*)/);
        return match ? decodeURIComponent(match[1]) : null;
    },
    set: function (value) {
        document.documentElement.dataset.theme = value;
        document.cookie = "cleanser-theme=" + encodeURIComponent(value) + ";path=/;max-age=31536000;SameSite=Lax";
    }
};
