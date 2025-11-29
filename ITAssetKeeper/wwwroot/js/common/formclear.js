document.addEventListener("DOMContentLoaded", () => {
    bindClearSingleInputs();
    bindClearFormButton();
});

// ---------------------------------------
// 1. 単一項目クリア（× ボタン）
// ---------------------------------------
function bindClearSingleInputs() {

    // freeword (上部の検索)
    const freewordClear = document.querySelector(".freeword-clear");
    const freewordInput = document.querySelector(".freeword-input");

    if (freewordClear && freewordInput) {
        freewordClear.addEventListener("click", () => {
            freewordInput.value = "";
        });
    }

    // custom-clear-btn（詳細検索テキストボックス用：×ボタンすべて）
    document.querySelectorAll(".custom-clear-btn").forEach(btn => {
        btn.addEventListener("click", () => {

            // クリア対象の input を探してクリアする

            // search-detail の input
            const detailWrapper = btn.closest(".search-detail-input-wrapper");
            const detailInput = detailWrapper?.querySelector(".search-detail-input");
            if (detailInput) {
                detailInput.value = "";
                return;
            }

            // custom-input-wrapper の input
            const customWrapper = btn.closest(".custom-input-wrapper");
            const customInput = customWrapper?.querySelector("input");
            if (customInput) {
                customInput.value = "";
                return;
            }
        });
    });
}

// ---------------------------------------
// 2. 「入力内容をクリア」ボタン
// ---------------------------------------
function bindClearFormButton() {

    const clearBtn = document.querySelector(".search-detail-clear-form-btn");

    if (!clearBtn) return;

    clearBtn.addEventListener("click", () => {
        clearForm();
    });
}

// ---------------------------------------
// 詳細検索フォーム全体クリア（共通）
// ---------------------------------------
function clearForm() {

    const form = document.getElementById("searchFormDetail");
    if (!form) return;

    form.querySelectorAll("input[type=text], input[type=date]")
        .forEach(el => el.value = "");

    form.querySelectorAll("select")
        .forEach(el => el.selectedIndex = 0);
}