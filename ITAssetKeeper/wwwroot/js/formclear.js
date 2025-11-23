// --- 全ての「×」ボタンで入力値をクリア --- //
document.addEventListener("DOMContentLoaded", () => {

    // freeword (上部の検索)
    const freewordClear = document.querySelector(".freeword-clear");
    const freewordInput = document.querySelector(".freeword-input");

    if (freewordClear && freewordInput) {
        freewordClear.addEventListener("click", () => {
            freewordInput.value = "";
        });
    }

    // custom-clear-btn（他テキストボックス用：×ボタンすべて）
    document.querySelectorAll(".custom-clear-btn").forEach(btn => {
        btn.addEventListener("click", () => {

            // クリア対象の input を探す（ボタンの兄弟要素）
            const wrapper = btn.closest(".custom-input-wrapper");
            const input = wrapper?.querySelector(".custom-input");

            if (input) {
                input.value = "";
            }
        });
    });
});

// --- 詳細検索フォーム 入力内容クリア ---
function clearForm() {

    const form = document.getElementById("searchFormDetail");

    if (!form) return;

    // 全 input をリセット
    form.querySelectorAll("input[type=text], input[type=date]").forEach(el => el.value = "");

    // 全 select をリセット
    form.querySelectorAll("select").forEach(el => el.selectedIndex = 0);
}

