document.querySelectorAll('[data-compare-form]').forEach((form) => {
    form.addEventListener('submit', () => {
        const submit = form.querySelector('[data-compare-submit]');
        const label = form.querySelector('[data-compare-submit-label]');
        const loading = form.querySelector('[data-compare-loading]');

        if (!submit || !label || !loading) {
            return;
        }

        submit.disabled = true;
        submit.setAttribute('aria-busy', 'true');
        label.hidden = true;
        loading.hidden = false;
        loading.setAttribute('aria-hidden', 'false');
    });
});