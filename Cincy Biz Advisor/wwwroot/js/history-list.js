(function () {
    "use strict";

    var PAGE_SIZE = 10;

    var status = document.getElementById("history-status");
    var errorBox = document.getElementById("history-error");
    var results = document.getElementById("history-results");
    var emptyBox = document.getElementById("history-empty");
    var wrapper = document.getElementById("history-table-wrapper");
    var body = document.getElementById("history-table-body");
    var footer = document.getElementById("history-footer");
    var summary = document.getElementById("history-summary");
    var pager = document.getElementById("history-pager");
    var searchInput = document.getElementById("history-search");
    var typeSelect = document.getElementById("history-type");
    var dateSelect = document.getElementById("history-date");

    if (!status || !errorBox || !results || !emptyBox || !wrapper || !body ||
        !footer || !summary || !pager || !searchInput || !typeSelect || !dateSelect) {
        return;
    }

    var allEntries = [];
    var filtered = [];
    var page = 1;

    function parseDate(value) {
        // The API sends UTC times without a "Z", so add it before converting to local time.
        var text = /(Z|[+-]\d\d:\d\d)$/.test(value) ? value : value + "Z";
        return new Date(text);
    }

    function formatDate(date) {
        return isNaN(date.getTime())
            ? ""
            : date.toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" });
    }

    function pillClass(score) {
        if (score >= 70) {
            return "history-pill-high";
        }
        return score >= 50 ? "history-pill-medium" : "history-pill-low";
    }

    function addCell(row, text, className) {
        var cell = document.createElement("td");
        cell.textContent = text;
        if (className) {
            cell.className = className;
        }
        row.appendChild(cell);
    }

    function addScoreCell(row, score) {
        var cell = document.createElement("td");
        cell.className = "history-num";
        var pill = document.createElement("span");
        pill.className = "history-pill " + pillClass(score);
        pill.textContent = Number(score).toLocaleString(undefined, { maximumFractionDigits: 1 });
        cell.appendChild(pill);
        row.appendChild(cell);
    }

    function populateTypes() {
        var seen = {};
        allEntries.forEach(function (entry) {
            seen[entry.businessType] = true;
        });

        Object.keys(seen).sort().forEach(function (name) {
            var option = document.createElement("option");
            option.value = name;
            option.textContent = name;
            typeSelect.appendChild(option);
        });
    }

    function matchesDate(entry, range) {
        if (range === "all") {
            return true;
        }

        var now = new Date();
        if (range === "year") {
            return entry.savedDate.getFullYear() === now.getFullYear();
        }

        var cutoff = new Date(now.getTime() - parseInt(range, 10) * 24 * 60 * 60 * 1000);
        return entry.savedDate >= cutoff;
    }

    function applyFilters() {
        var term = searchInput.value.trim().toLowerCase();
        var type = typeSelect.value;
        var range = dateSelect.value;

        filtered = allEntries.filter(function (entry) {
            var textMatch = !term ||
                entry.neighborhood.toLowerCase().indexOf(term) !== -1 ||
                entry.businessType.toLowerCase().indexOf(term) !== -1;
            var typeMatch = !type || entry.businessType === type;
            return textMatch && typeMatch && matchesDate(entry, range);
        });

        page = 1;
        render();
    }

    function makePagerButton(label, ariaLabel, targetPage, disabled, isCurrent) {
        var button = document.createElement("button");
        button.type = "button";
        button.className = "history-pager-button" + (isCurrent ? " history-pager-current" : "");
        button.textContent = label;
        button.setAttribute("aria-label", ariaLabel);
        button.disabled = disabled;
        if (isCurrent) {
            button.setAttribute("aria-current", "page");
        }
        button.addEventListener("click", function () {
            page = targetPage;
            render();
        });
        return button;
    }

    function renderPager(pages) {
        pager.textContent = "";
        pager.appendChild(makePagerButton("\u2039", "Previous page", page - 1, page <= 1, false));
        for (var i = 1; i <= pages; i++) {
            pager.appendChild(makePagerButton(String(i), "Page " + i, i, false, i === page));
        }
        pager.appendChild(makePagerButton("\u203A", "Next page", page + 1, page >= pages, false));
    }

    function render() {
        body.textContent = "";
        var total = filtered.length;

        if (total === 0) {
            wrapper.hidden = true;
            footer.hidden = true;
            emptyBox.hidden = false;
            return;
        }

        emptyBox.hidden = true;
        wrapper.hidden = false;
        footer.hidden = false;

        var pages = Math.ceil(total / PAGE_SIZE);
        if (page > pages) {
            page = pages;
        }

        var start = (page - 1) * PAGE_SIZE;
        var end = Math.min(start + PAGE_SIZE, total);

        filtered.slice(start, end).forEach(function (entry) {
            var row = document.createElement("tr");
            addCell(row, formatDate(entry.savedDate));
            addCell(row, entry.businessType);
            addCell(row, entry.neighborhood);
            addCell(row, Number(entry.reportedCrimes).toLocaleString(), "history-num");
            addCell(row, Number(entry.buildingPermits).toLocaleString(), "history-num");
            addScoreCell(row, entry.overallScore);
            body.appendChild(row);
        });

        summary.textContent = "Showing " + (start + 1) + " to " + end + " of " + total + " results";
        renderPager(pages);
    }

    function showError() {
        status.hidden = true;
        errorBox.textContent = "Could not load saved analyses. Please try again later.";
        errorBox.hidden = false;
    }

    searchInput.addEventListener("input", applyFilters);
    typeSelect.addEventListener("change", applyFilters);
    dateSelect.addEventListener("change", applyFilters);

    fetch("/api/history", { headers: { Accept: "application/json" } })
        .then(function (response) {
            if (!response.ok) {
                throw new Error("Request failed");
            }
            return response.json();
        })
        .then(function (entries) {
            allEntries = entries.map(function (entry) {
                entry.savedDate = parseDate(entry.savedAtUtc);
                return entry;
            });
            status.hidden = true;
            results.hidden = false;
            populateTypes();
            applyFilters();
        })
        .catch(showError);
})();