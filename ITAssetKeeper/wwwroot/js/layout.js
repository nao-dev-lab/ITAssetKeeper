// --- モーダルのバックドロップを強制除去 ---
function clearModalBackdrop() {
    document.querySelectorAll('.modal-backdrop').forEach(b => b.remove());
}

// --- イベント delegating（リンククリック対応） ---
document.addEventListener("click", async (e) => {

    // --- アカウント情報用モーダルの表示 ---
    // アカウント情報表示リンクがクリックされたら実行
    if (e.target.id === "profileLink") {
        e.preventDefault();

        clearModalBackdrop();

        // Profile 部分ビューを取得
        const response = await fetch(e.target.href);
        const html = await response.text();

        // モーダル差し替え
        document.getElementById("profileModalContainer").innerHTML = html;

        // モーダルを開く
        const modalEl = document.getElementById("profileModal");
        new bootstrap.Modal(modalEl).show();
    }

    // --- ログアウト確認用モーダルの表示 ---
    // ログアウトリンクがクリックされたら実行
    if (e.target.id === "logoutLink") {
        e.preventDefault();

        clearModalBackdrop();

        // ログアウト用モーダルの要素を取得して、モーダル表示
        const modalEl = document.getElementById("logoutModal");
        new bootstrap.Modal(modalEl).show();
    }

    // --- パスワード変更 確認モーダルの表示 ---
    if (e.target.id === "openChangePasswordConfirm") {
        e.preventDefault();
        clearModalBackdrop();

        // 今開いてるモーダルを閉じる
        document.querySelectorAll('.modal.show').forEach(m => {
            const instance = bootstrap.Modal.getInstance(m);
            if (instance) instance.hide();
        });

        // 小さな確認モーダルを表示
        const modalEl = document.getElementById("changePasswordConfirmModal");
        new bootstrap.Modal(modalEl).show();
        return;
    }
});

