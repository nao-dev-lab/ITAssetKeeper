document.addEventListener("DOMContentLoaded", function () {

    // --- 過去7日間の更新履歴（棒グラフ） ---
    const historyCanvas = document.getElementById("historyCountChart");
    if (historyCanvas) {
        const raw = historyCanvas.dataset.history;
        const updateData = JSON.parse(raw);

        const labels = updateData.map(item => item.displayDate);
        const data = updateData.map(item => item.count);

        const ctx = historyCanvas.getContext("2d");

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: '更新件数',
                    data: data,
                    backgroundColor: 'rgba(170, 140, 255, 0.7)',
                    borderColor: 'rgba(170, 140, 255, 0.5)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 5,     // 目盛りの間隔を1に固定
                            precision: 0    // 小数点以下を表示しない
                        },
                        title: {
                            display: false,
                            text: '件数'
                        }
                    },
                    x: {
                        title: {
                            display: false,
                            text: '日付'
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    title: {
                        display: false,
                        text: '過去7日間の更新履歴件数'
                    }
                }
            }
        });
    }

    // --- 状態別 台数集計（ドーナツ） ---
    const statusCanvas = document.getElementById("statusChart");
    if (statusCanvas) {

        const labels = JSON.parse(statusCanvas.dataset.labels);
        const values = JSON.parse(statusCanvas.dataset.values);

        const ctx = statusCanvas.getContext("2d");

        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    label: '状態別台数',
                    data: values,
                    backgroundColor: [
                        'rgba(170, 140, 255, 0.7)', // 稼働中
                        'rgba(140, 167, 255, 0.7)', // 予備
                        'rgba(140, 199, 255, 0.7)', // 故障
                        'rgba(140, 224, 255, 0.7)', // 廃棄予定
                        'rgba(207, 235, 234, 0.7)'  // 廃棄済
                    ]
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                        labels: {
                            padding: 20 // ラベル同士の余白
                        },
                    },
                    title: {
                        display: false,
                        text: '状態別の台数集計'
                    }
                }
            }
        });
    }
});
