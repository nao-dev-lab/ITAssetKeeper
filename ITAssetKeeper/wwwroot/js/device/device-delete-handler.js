// --- Device/Details : 削除モーダル処理 ---

document.addEventListener("DOMContentLoaded", () => {

    const openBtn = document.getElementById("btnOpenDeleteModal"); // 削除確認モーダル起動
    const execBtn = document.getElementById("btnDeleteExec");      // モーダル内の削除実行

    if (!openBtn || !execBtn) return;

    const deleteModal = new bootstrap.Modal(document.getElementById("deleteConfirmModal"));
    let selectedId = null;

    bindOpenDeleteModal(openBtn, deleteModal, (id) => {
        selectedId = id; // 選択された削除対象IDを記録
    });

    bindExecuteDelete(execBtn, () => selectedId, deleteModal);
});


// --------------------------------------
// 1. 削除ボタン → モーダル表示
// --------------------------------------
function bindOpenDeleteModal(openBtn, deleteModal, onIdSelected) {

    openBtn.addEventListener("click", () => {

        const id = openBtn.dataset.deviceId;

        // ID が取れない場合は中断
        if (!id) return;

        onIdSelected(id);

        // モーダルの表示情報をセット
        document.getElementById("delMgmtId").innerText = openBtn.dataset.deviceMgmtid;
        document.getElementById("delCategory").innerText = openBtn.dataset.deviceCategory;
        document.getElementById("delStatus").innerText = openBtn.dataset.deviceStatus;
        document.getElementById("delUser").innerText = openBtn.dataset.deviceUser;

        deleteModal.show();
    });
}

// --------------------------------------
// 2. 削除実行（fetch POST）
// --------------------------------------
function bindExecuteDelete(execBtn, getSelectedId, deleteModal) {

    execBtn.addEventListener("click", () => {

        const selectedId = getSelectedId();
        if (!selectedId) return;

        fetch(`/Device/DeleteConfirmed/${selectedId}`, {
            method: "POST",
            headers: {
                "X-Request-With": "XMLHttpRequest",
                "Content-Type": "application/json",
                "RequestVerificationToken": getCsrfToken()
            },
            body: null
        })
            .then(res => res.json())
            .then(data => {

                deleteModal.hide();

                if (data.success) {
                    window.location.href = "/Device/Index";
                } else {
                    window.location.href = `/Device/Details/${selectedId}`;
                }
            })
            .catch(() => {
                deleteModal.hide();
                showToast("通信エラーが発生しました。", false);
            });
    });
}

// --------------------------------------
// 3. CSRF トークン取得
// --------------------------------------
function getCsrfToken() {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    return tokenInput ? tokenInput.value : "";
}
