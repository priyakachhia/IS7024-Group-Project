(function () {
    "use strict";

    var button = document.getElementById("history-save-button");
    var message = document.getElementById("history-save-message");

    if (!button || !message) {
        return;
    }

    function showMessage(text, isError) {
        message.textContent = text;
        message.className = "history-save-message " +
            (isError ? "history-save-message-error" : "history-save-message-success");
    }

    button.addEventListener("click", function () {
        var data = button.dataset;
        var payload = {
            neighborhood: data.neighborhood,
            businessType: data.businessType,
            reportedCrimes: parseInt(data.crimes, 10),
            buildingPermits: parseInt(data.permits, 10),
            safetyScore: parseFloat(data.safety),
            growthScore: parseFloat(data.growth),
            overallScore: parseInt(data.score, 10)
        };

        button.disabled = true;
        showMessage("Saving...", false);

        fetch("/api/history", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        })
            .then(function (response) {
                if (response.status === 201) {
                    showMessage("Analysis saved to history.", false);
                    return;
                }

                button.disabled = false;
                if (response.status === 400) {
                    showMessage("Could not save: the analysis values were not valid.", true);
                } else {
                    showMessage("Could not save the analysis. Please try again.", true);
                }
            })
            .catch(function () {
                button.disabled = false;
                showMessage("Could not save the analysis. Please check your connection and try again.", true);
            });
    });
})();