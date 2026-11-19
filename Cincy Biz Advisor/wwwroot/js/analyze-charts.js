// Draws the three charts on the Analyze page. Runs only when the page has chart data.
(function () {
    var dataElement = document.getElementById("analyze-chart-data");
    if (!dataElement || typeof Chart === "undefined") {
        return;
    }

    var data = JSON.parse(dataElement.textContent);
    var palette = ["#3b82f6", "#ef4444", "#7e3fbf", "#2a9d9f", "#c084d6", "#43a047"];

    function hasData(values) {
        return Array.isArray(values) && values.length > 0;
    }

    // If there is nothing to draw, show a short message instead of an empty chart.
    function showEmpty(canvasId) {
        var box = document.getElementById(canvasId).parentElement;
        box.innerHTML = '<p class="analyze-chart-empty">No data for this period.</p>';
    }

    // "Jan 2025" becomes "Jan" when every month is in the same year.
    function shortLabels(labels) {
        var sameYear = labels.every(function (l) { return l.slice(-4) === labels[0].slice(-4); });
        return sameYear ? labels.map(function (l) { return l.slice(0, 3); }) : labels;
    }

    function barChart(canvasId, labels, values, color, label) {
        if (!hasData(values)) {
            showEmpty(canvasId);
            return;
        }

        new Chart(document.getElementById(canvasId), {
            type: "bar",
            data: {
                labels: shortLabels(labels),
                datasets: [{
                    label: label,
                    data: values,
                    backgroundColor: color,
                    borderRadius: 3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    x: { grid: { display: false } },
                    y: { beginAtZero: true, ticks: { precision: 0 } }
                }
            }
        });
    }

    barChart("analyzeCrimeTrend", data.crimeLabels, data.crimeValues, "#e53935", "Reported crimes");
    barChart("analyzePermitTrend", data.permitLabels, data.permitValues, "#3b82f6", "Building permits");

    if (!hasData(data.typeValues)) {
        showEmpty("analyzeCrimeType");
        return;
    }

    var total = data.typeValues.reduce(function (sum, n) { return sum + n; }, 0);
    var colors = data.typeValues.map(function (_, i) { return palette[i % palette.length]; });

    // Writes the total in the middle of the donut.
    var centerText = {
        id: "centerText",
        afterDraw: function (chart) {
            var area = chart.chartArea;
            var x = (area.left + area.right) / 2;
            var y = (area.top + area.bottom) / 2;
            var ctx = chart.ctx;
            ctx.save();
            ctx.textAlign = "center";
            ctx.textBaseline = "middle";
            ctx.fillStyle = "#0b1633";
            ctx.font = "bold 26px sans-serif";
            ctx.fillText(total.toLocaleString(), x, y - 8);
            ctx.font = "13px sans-serif";
            ctx.fillStyle = "#6b7280";
            ctx.fillText("Total", x, y + 16);
            ctx.restore();
        }
    };

    new Chart(document.getElementById("analyzeCrimeType"), {
        type: "doughnut",
        data: {
            labels: data.typeLabels,
            datasets: [{ data: data.typeValues, backgroundColor: colors }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: "65%",
            plugins: {
                legend: {
                    position: "bottom",
                    labels: {
                        boxWidth: 12,
                        padding: 10,
                        font: { size: 12 },
                        // Shows "THEFT  28%" next to each color.
                        generateLabels: function (chart) {
                            var dataset = chart.data.datasets[0];
                            return chart.data.labels.map(function (name, i) {
                                var percent = Math.round(dataset.data[i] / total * 100);
                                return {
                                    text: name + "  " + percent + "%",
                                    fillStyle: dataset.backgroundColor[i],
                                    strokeStyle: dataset.backgroundColor[i],
                                    hidden: !chart.getDataVisibility(i),
                                    index: i
                                };
                            });
                        }
                    }
                }
            }
        },
        plugins: [centerText]
    });
})();

