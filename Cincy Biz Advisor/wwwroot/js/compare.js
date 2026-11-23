const businessTypePicker = document.querySelector('[data-business-type-picker]');

if (businessTypePicker) {
    const businessTypeSelect = businessTypePicker.querySelector('select');
    const businessTypeIcon = businessTypePicker.querySelector('[data-business-type-icon] use');
    const businessTypeIcons = {
        restaurant: 'compare-icon-utensils',
        'coffee-shop': 'compare-icon-coffee',
        retail: 'compare-icon-storefront'
    };

    const updateBusinessTypeIcon = () => {
        if (!businessTypeSelect || !businessTypeIcon) {
            return;
        }

        const iconId = businessTypeIcons[businessTypeSelect.value] ?? 'compare-icon-business';
        businessTypeIcon.setAttribute('href', `#${iconId}`);
    };

    businessTypeSelect?.addEventListener('change', updateBusinessTypeIcon);
    updateBusinessTypeIcon();
}

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