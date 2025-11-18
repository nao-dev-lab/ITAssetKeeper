// --- トースト通知：共通用 ---

// DOMの読み込み完了後に処理を開始
document.addEventListener("DOMContentLoaded", function () {

    // TempData のメッセージを受け取る要素
    const holder = document.getElementById("toast-message");

    // メッセージが未設定なら処理中断
    if (!holder) return;

    // メッセージを取得
    const message = holder.dataset.message;

    // typeをセット
    const type = holder.dataset.type || "success"; // success/error/warn を想定

    // トーストの要素を取得
    const toastEl = document.getElementById("resultToast");
    const toastBody = document.getElementById("toastMessage");

    // 要素を取得できない場合、処理中断
    if (!toastEl || !toastBody) return;

    // Bootstrap Toast のインスタンスを生成 (自動非表示 true、7秒)
    const toast = new bootstrap.Toast(toastEl);

    // トースト本文にメッセージを設定
    toastBody.textContent = message;

    // 結果に応じてスタイルを付与
    toastEl.classList.remove("success", "error", "warn");
    toastEl.classList.add(type);

    // トーストを表示
    toast.show();
});
