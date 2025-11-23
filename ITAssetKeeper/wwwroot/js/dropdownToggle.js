// --- ドロップダウンリスト の矢印切り替え処理 ---
document.addEventListener("DOMContentLoaded", () => {
    initializeDropdownToggle();
});

function initializeDropdownToggle() {
    document.querySelectorAll(".custom-dropdown").forEach(select => {

        // ドロップダウン展開前のクリック（mousedown）
        select.addEventListener("mousedown", () => {
            select.classList.toggle("open");
        });

        // フォーカス外れたら必ず閉じる
        select.addEventListener("blur", () => {
            select.classList.remove("open");
        });
    });
}
