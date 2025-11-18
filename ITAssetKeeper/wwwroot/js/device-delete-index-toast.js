// DOMの読み込み完了後に処理を開始
document.addEventListener("DOMContentLoaded", function () {

    // TempData のメッセージを受け取る要素
    const toastHolder = document.getElementById("toast-message");

    // メッセージが未設定なら処理中断
    if (!toastHolder) return;

    // メッセージを取得
    const message = toastHolder.dataset.message;

    // トーストの要素を取得
    const toastEl = document.getElementById("resultToast");     // トースト全体
    const toastBody = document.getElementById("toastMessage");  // トースト本文部分

    // 要素を取得できない場合、処理中断
    if (!toastEl || !toastBody) return;

    // Bootstrap Toast のインスタンスを生成
    // delay: 6000 → 6秒間表示して自動で閉じる設定
    const toast = new bootstrap.Toast(toastEl, { delay: 6000 });

    // トースト本文にメッセージを設定
    toastBody.textContent = message;

    // 成功メッセージ用のスタイルを付与
    toastEl.classList.add("success");

    // トーストを表示
    toast.show();
});