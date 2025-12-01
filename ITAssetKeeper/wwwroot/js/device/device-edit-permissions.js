// --- デバイス編集画面の権限設定 (Roleに応じた編集不可項目の制御) ---
// 必須入力マークもそれに合わせて表示・非表示を切り返す

// フラグに応じて、disable の状態を変える
document.addEventListener("DOMContentLoaded", () => {

    document.querySelectorAll(".disable-target").forEach(el => {
        
        // data_disable(= el.dataset.disable)をフラグに応じて変更
        const flag = el.dataset.disable;
        if (flag === "True" || flag === "true") {
            el.disabled = true;

            // form-group 単位まで戻って required-mark を探して
            // 必須マークを非表示にする
            const wrapper = el.closest(".form-group");
            if (wrapper) {
                const requiredMark = wrapper.querySelector(".required-mark");
                if (requiredMark) {
                    requiredMark.style.display = "none";
                }
            }
        }
    });
});