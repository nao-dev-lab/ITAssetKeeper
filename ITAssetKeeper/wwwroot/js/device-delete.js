// DOMの読み込み完了後に処理を開始
document.addEventListener("DOMContentLoaded", function () {

    // モーダルのボタンのIDを取得
    const openBtn = document.getElementById("btnOpenDeleteModal"); // 削除確認モーダル起動ボタン
    const execBtn = document.getElementById("btnDeleteExec");      // モーダル上の削除ボタン

    const toastEl = document.getElementById("resultToast");        // トースト全体の要素
    const toastBody = document.getElementById("toastMessage");     // トーストの本文(メッセージ表示部分)

    // 削除対象のIDを保持する変数
    let selectedId = null;

    // Bootstrapコンポーネントのインスタンス化
    const deleteModal = new bootstrap.Modal(document.getElementById("deleteConfirmModal")); // 削除確認モーダル
    const toast = new bootstrap.Toast(toastEl);     // トースト通知

    // ----- 削除ボタン押下 → モーダル表示 -----
    // 削除ボタンのクリックイベントを監視
    if (openBtn) {
        openBtn.addEventListener("click", () => {
            // 削除対象のId(@Model.Id)を取得
            selectedId = openBtn.dataset.deviceId;

            // モーダルへ表示する値をセット
            document.getElementById("delMgmtId").innerText = openBtn.dataset.deviceMgmtid;
            document.getElementById("delCategory").innerText = openBtn.dataset.deviceCategory;
            document.getElementById("delStatus").innerText = openBtn.dataset.deviceStatus;
            document.getElementById("delUser").innerText = openBtn.dataset.deviceUser;

            // モーダルを表示
            deleteModal.show();
        });
    }

    // ----- モーダル内の削除実行ボタン押下 → Ajax通信 -----
    // モーダル上の削除ボタンのクリックイベントを監視
    if (execBtn) {
        execBtn.addEventListener("click", () => {

            // 削除対象IDが未設定なら処理中断
            if (!selectedId) return;

            // 渡されたIDを引数にしてDeleteConfirmedアクションを呼び出す
            fetch(`/Device/DeleteConfirmed/${selectedId}`, {
                method: "POST",
                headers: {
                    "X-Request-With": "XMLHttpRequest",
                    "Content-Type": "application/json",
                    "RequestVerificationToken": getCsrfToken()  // CSRF対策トークンを送信
                },
                body: null
            })
                .then(res => res.json())    // レスポンスをJSONとしてパース
                .then(data => {

                    // モーダルを閉じる
                    deleteModal.hide();

                    if (data.success) {
                        // 成功時：トースト表示
                        showToast("削除が完了しました。", true);

                        // 1秒後に一覧画面へ遷移
                        setTimeout(() => {
                            window.location.href = "/Device/Index";
                        }, 1000);

                    } else {
                        // 失敗時：エラーメッセージをトースト表示
                        showToast("削除に失敗しました。管理者に問い合わせてください。", false);
                    }
                })
                .catch(() => {
                    // // 通信エラー時：モーダルを閉じてエラートースト表示
                    deleteModal.hide();
                    showToast("通信エラーが発生しました。", false);
                });
        });
    }


    // ----- トースト表示関数 -----
    function showToast(message, isSuccess) {
        toastBody.textContent = message;    // トーストにメッセージを設定

        if (isSuccess) {
            // 成功時は success クラスを付与
            toastEl.classList.remove("error");
            toastEl.classList.add("success");
        } else {
            // 失敗時は error クラスを付与
            toastEl.classList.remove("success");
            toastEl.classList.add("error");
        }

        // トーストを表示
        toast.show();
    }

    // ----- CSRF用Token取得関数 -----
    function getCsrfToken() {
        // @Html.AntiForgeryToken() で埋め込まれる hidden input から値を取得
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : "";
    }
});
