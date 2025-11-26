// --- デバイス編集画面の権限設定 (Roleに応じた編集不可項目の制御) ---  

// フラグに応じて、disable の状態を変える
document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".disable-target").forEach(el => {
        // Razor側で指定した data_disable(= el.dataset.disable)をフラグに応じて変更
        const flag = el.dataset.disable;
        if (flag === "True" || flag === "true") {
            el.disabled = true;
        }
    });
});