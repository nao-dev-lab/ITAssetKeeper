// --- サイドバー モーダル管理 ---


// --- 共通：バックドロップ除去 ---
function clearModalBackdrop() {
    document.querySelectorAll(".modal-backdrop").forEach(b => b.remove());
}

// --- プロフィール表示 ---
async function handleProfileModal(e) {
    e.preventDefault();
    clearModalBackdrop();

    const response = await fetch(e.target.href);
    const html = await response.text();

    document.getElementById("profileModalContainer").innerHTML = html;

    new bootstrap.Modal(document.getElementById("profileModal")).show();
}

// --- ログアウト確認 ---
function handleLogoutModal(e) {
    e.preventDefault();
    clearModalBackdrop();

    new bootstrap.Modal(document.getElementById("logoutModal")).show();
}

// --- パスワード変更確認 ---
function handleChangePasswordConfirm(e) {
    e.preventDefault();
    clearModalBackdrop();

    // 既存モーダルを閉じる
    document.querySelectorAll(".modal.show").forEach(m => {
        const instance = bootstrap.Modal.getInstance(m);
        if (instance) instance.hide();
    });

    new bootstrap.Modal(document.getElementById("changePasswordConfirmModal")).show();
}


// --- イベント delegating ---
document.addEventListener("click", async (e) => {

    if (e.target.id === "profileLink") {
        await handleProfileModal(e);
        return;
    }

    if (e.target.id === "logoutLink") {
        handleLogoutModal(e);
        return;
    }

    if (e.target.id === "openChangePasswordConfirm") {
        handleChangePasswordConfirm(e);
        return;
    }
});


