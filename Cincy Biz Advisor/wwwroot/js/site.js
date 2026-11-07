// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(() => {
    const search = document.getElementById("neighborhood-search");
    const hiddenId = document.getElementById("neighborhood-id");
    const resultsContainer = document.getElementById("neighborhood-results");
    const status = document.getElementById("neighborhood-status");

    if (!search || !hiddenId || !resultsContainer || !status) {
        return;
    }

    const results = Array.from(resultsContainer.querySelectorAll("[data-neighborhood-result]"));
    let activeIndex = -1;

    const visibleResults = () => results.filter(result => !result.hidden);

    const setExpanded = expanded => {
        search.setAttribute("aria-expanded", expanded ? "true" : "false");
        resultsContainer.hidden = !expanded;
        if (!expanded) {
            search.removeAttribute("aria-activedescendant");
        }
    };

    const setActiveResult = result => {
        for (const otherResult of results) {
            otherResult.classList.remove("is-active");
        }

        if (!result) {
            search.removeAttribute("aria-activedescendant");
            return;
        }

        result.classList.add("is-active");
        search.setAttribute("aria-activedescendant", result.id);
    };

    const renderResults = () => {
        const searchText = search.value.trim().toLocaleLowerCase();
        let visibleCount = 0;

        for (const result of results) {
            const name = result.dataset.neighborhoodName.toLocaleLowerCase();
            const isVisible = searchText.length === 0 || name.includes(searchText);
            result.hidden = !isVisible;
            if (isVisible) {
                result.style.removeProperty("display");
            }
            else {
                result.style.setProperty("display", "none", "important");
            }
            if (isVisible) {
                visibleCount++;
            }
        }

        status.textContent = visibleCount === 0
            ? "No neighborhoods found"
            : `${visibleCount} neighborhood${visibleCount === 1 ? "" : "s"} found`;

        const currentVisibleResults = visibleResults();
        if (activeIndex >= currentVisibleResults.length) {
            activeIndex = -1;
            setActiveResult(null);
        }
    };

    search.addEventListener("input", () => {
        hiddenId.value = "";
        activeIndex = -1;
        setActiveResult(null);
        setExpanded(true);
        for (const result of results) {
            result.removeAttribute("aria-selected");
        }
        renderResults();
    });

    search.addEventListener("focus", () => setExpanded(true));

    search.addEventListener("keydown", event => {
        const currentVisibleResults = visibleResults();

        if (event.key === "ArrowDown" || event.key === "ArrowUp") {
            event.preventDefault();
            setExpanded(true);
            if (currentVisibleResults.length === 0) {
                return;
            }

            const direction = event.key === "ArrowDown" ? 1 : -1;
            activeIndex = (activeIndex + direction + currentVisibleResults.length) % currentVisibleResults.length;
            setActiveResult(currentVisibleResults[activeIndex]);
        }
        else if (event.key === "Enter" && activeIndex >= 0) {
            event.preventDefault();
            currentVisibleResults[activeIndex]?.click();
        }
        else if (event.key === "Escape") {
            event.preventDefault();
            setExpanded(false);
            activeIndex = -1;
            setActiveResult(null);
        }
    });

    for (const result of results) {
        result.addEventListener("click", () => {
            hiddenId.value = result.dataset.neighborhoodId;
            search.value = result.dataset.neighborhoodName;

            for (const otherResult of results) {
                otherResult.removeAttribute("aria-selected");
            }

            result.setAttribute("aria-selected", "true");
            activeIndex = visibleResults().indexOf(result);
            setActiveResult(result);
            setExpanded(true);
            renderResults();
        });
    }

    renderResults();
})();
