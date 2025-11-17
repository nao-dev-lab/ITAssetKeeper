document.addEventListener("DOMContentLoaded", function () {

    const openBtn = document.getElementById("btnOpenDeleteModal");
    const execBtn = document.getElementById("btnDeleteExec");
    const toastEl = document.getElementById("resultToast");
    const toastBody = document.getElementById("toastMessage");

    let selectedId = null;

    // モーダル制御
    const deleteModal = new bootstrap.Modal(document.getElementById("deleteConfirmModal"));
    const toast = new bootstrap.Toast(toastEl);

    // ----- 削除ボタン押下 → モーダル表示 -----
    if (openBtn) {
        openBtn.addEventListener("click", () => {
            selectedId = openBtn.dataset.deviceId;

            // モーダルへ値表示
            document.getElementById("delMgmtId").innerText = openBtn.dataset.deviceMgmtid;
            document.getElementById("delCategory").innerText = openBtn.dataset.deviceCategory;
            document.getElementById("delStatus").innerText = openBtn.dataset.deviceStatus;

            deleteModal.show();
        });
    }

    // ----- 削除実行（Ajax） -----
    if (execBtn) {
        execBtn.addEventListener("click", () => {

            if (!selectedId) return;

            fetch(`/Device/DeleteAjax/${selectedId}`, {
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
                        showToast("削除が完了しました。", true);

                        // 遷移（1.5秒後）
                        setTimeout(() => {
                            window.location.href = "/Device/Index";
                        }, 1500);

                    } else {
                        showToast("削除に失敗しました。管理者に問い合わせてください。", false);
                    }
                })
                .catch(() => {
                    deleteModal.hide();
                    showToast("通信エラーが発生しました。", false);
                });
        });
    }


    // ----- Toast 表示関数 -----
    function showToast(message, isSuccess) {
        toastBody.textContent = message;

        if (isSuccess) {
            toastEl.classList.remove("error");
            toastEl.classList.add("success");
        } else {
            toastEl.classList.remove("success");
            toastEl.classList.add("error");
        }

        toast.show();
    }

    // ----- CSRF用Token取得 -----
    function getCsrfToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : "";
    }
});
