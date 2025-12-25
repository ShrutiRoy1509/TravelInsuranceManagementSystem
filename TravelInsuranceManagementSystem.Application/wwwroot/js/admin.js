
//(function () {
//    const path = location.pathname.toLowerCase();
//    document.querySelectorAll('.nav-item').forEach(a => {
//        const href = a.getAttribute('href')?.toLowerCase() || '';
//        if (path.includes(href.toLowerCase())) {
//            a.style.background = 'var(--gray)';
//        }
//    });
//})();


/
// Tiny helpers
function textMatch(haystack, needle) {
    return haystack.toLowerCase().includes(needle.toLowerCase());
}

function visibleRowsToCSV($table) {
    const rows = [];
    // headers
    const headers = [];
    $table.find('thead th').each(function () { headers.push($(this).text().trim()); });
    rows.push(headers.join(','));

    // visible body rows
    $table.find('tbody tr:visible').each(function () {
        const cols = [];
        $(this).children('td').each(function () {
            // strip inner text (remove badge text noise)
            cols.push($(this).text().trim().replace(/\s+/g, ' '));
        });
        rows.push(cols.join(','));
    });
    return rows.join('\n');
}

function downloadCSV(content, filename) {
    const blob = new Blob([content], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', filename);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// Generic table text search
function wireSearch(inputSelector, tableSelector) {
    const $input = $(inputSelector);
    const $table = $(tableSelector);
    $input.on('input', function () {
        const q = $(this).val().trim();
        $table.find('tbody tr').each(function () {
            const rowText = $(this).text();
            $(this).toggle(textMatch(rowText, q) || q.length === 0);
        });
    });
}

// Generic status filter (buttons with data-status)
function wireStatusButtons(buttonSelector, tableSelector) {
    const $buttons = $(buttonSelector);
    const $table = $(tableSelector);
    $buttons.on('click', function () {
        const status = $(this).data('status');
        $buttons.removeClass('active');
        $(this).addClass('active');

        $table.find('tbody tr').each(function () {
            const rowStatus = ($(this).data('status') || '').toString();
            const show = (status === 'All') || (rowStatus.toLowerCase() === status.toLowerCase());
            $(this).toggle(show);
        });
    });
}

// Export visible rows to CSV
function wireExport(btnSelector, tableSelector, filename) {
    $(btnSelector).on('click', function () {
        const csv = visibleRowsToCSV($(tableSelector));
        downloadCSV(csv, filename);
    });
}

// Policies page: dropdown filters
function wirePolicyFilters() {
    const $table = $('#policyTable');
    const $status = $('#policyStatus');
    const $country = $('#policyCountry');
    const $coverage = $('#policyCoverage');

    function apply() {
        const st = ($status.val() || 'All').toLowerCase();
        const co = ($country.val() || 'All').toLowerCase();
        const cv = ($coverage.val() || 'All').toLowerCase();

        $table.find('tbody tr').each(function () {
            const rs = (($(this).data('status') || '') + '').toLowerCase();
            const rc = (($(this).data('country') || '') + '').toLowerCase();
            const rv = (($(this).data('coverage') || '') + '').toLowerCase();

            const okStatus = (st === 'all') || (rs === st);
            const okCountry = (co === 'all') || (rc === co);
            const okCoverage = (cv === 'all') || rv.includes(cv); // allow "Basic Medical" or "Premium Medical"

            $(this).toggle(okStatus && okCountry && okCoverage);
        });
    }

    $('#policyApply').on('click', apply);
    // also apply on change for instant feedback
    $status.on('change', apply);
    $country.on('change', apply);
    $coverage.on('change', apply);
}

// Page bootstrapping
$(function () {
    // Highlight current nav
    const path = location.pathname.toLowerCase();
    $('.nav-item').each(function () {
        const href = $(this).attr('href')?.toLowerCase() || '';
        if (path.includes(href.toLowerCase())) {
            $(this).addClass('active').css('background', 'var(--gray)');
        }
    });

    // Dashboard
    if ($('#dashboardTable').length) {
        wireSearch('#dashboardSearch', '#dashboardTable');
        wireStatusButtons('.status-btn', '#dashboardTable');
        wireExport('#exportDashboard', '#dashboardTable', 'dashboard.csv');
    }

    // Policies
    if ($('#policyTable').length) {
        wirePolicyFilters();
    }

    // Claims
    if ($('#claimsTable').length) {
        wireSearch('#claimsSearch', '#claimsTable');
        wireStatusButtons('.claims-status-btn', '#claimsTable');
        wireExport('#exportClaims', '#claimsTable', 'claims.csv');
    }

    // Payments
    if ($('#paymentsTable').length) {
        wireSearch('#paymentsSearch', '#paymentsTable');
        wireStatusButtons('.payments-status-btn', '#paymentsTable');
        wireExport('#exportPayments', '#paymentsTable', 'payments.csv');
    }
});

