(function () {
    var banner = document.getElementById("pdm-cookie-consent");
    if (!banner) {
        return;
    }

    var acceptButton = banner.querySelector("[data-cookie-accept]");
    var storageKey = "pdm_cookie_consent_v1";

    var hideBanner = function () {
        banner.setAttribute("hidden", "hidden");
        banner.setAttribute("aria-hidden", "true");
    };

    var hasConsent = function () {
        try {
            return window.localStorage.getItem(storageKey) === "accepted";
        } catch (err) {
            return false;
        }
    };

    if (hasConsent()) {
        hideBanner();
        return;
    }

    if (acceptButton) {
        acceptButton.addEventListener("click", function () {
            try {
                window.localStorage.setItem(storageKey, "accepted");
            } catch (err) {
                // Ignore storage errors and still hide the banner.
            }
            hideBanner();
        });
    }
})();
