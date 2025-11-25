// ==============================
// Device/Details : 削除モーダル処理
// ==============================

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



//// DOMの読み込み完了後に処理を開始
//document.addEventListener("DOMContentLoaded", function () {

//    // モーダルのボタンのIDを取得
//    const openBtn = document.getElementById("btnOpenDeleteModal"); // 削除確認モーダル起動ボタン
//    const execBtn = document.getElementById("btnDeleteExec");      // モーダル上の削除ボタン

//    // 削除対象のIDを保持する変数
//    let selectedId = null;

//    // Bootstrapコンポーネントのインスタンス化
//    const deleteModal = new bootstrap.Modal(document.getElementById("deleteConfirmModal")); // 削除確認モーダル

//    // ----- 削除ボタン押下 → モーダル表示 -----
//    // 削除ボタンのクリックイベントを監視
//    if (openBtn) {
//        openBtn.addEventListener("click", () => {
//            // 削除対象のId(@Model.Id)を取得
//            selectedId = openBtn.dataset.deviceId;

//            // モーダルへ表示する値をセット
//            document.getElementById("delMgmtId").innerText = openBtn.dataset.deviceMgmtid;
//            document.getElementById("delCategory").innerText = openBtn.dataset.deviceCategory;
//            document.getElementById("delStatus").innerText = openBtn.dataset.deviceStatus;
//            document.getElementById("delUser").innerText = openBtn.dataset.deviceUser;

//            // モーダルを表示
//            deleteModal.show();
//        });
//    }

//    // ----- モーダル内の削除実行ボタン押下 → Ajax通信 -----
//    // モーダル上の削除ボタンのクリックイベントを監視
//    if (execBtn) {
//        execBtn.addEventListener("click", () => {

//            // 削除対象IDが未設定なら処理中断
//            if (!selectedId) return;

//            // 渡されたIDを引数にしてDeleteConfirmedアクションを呼び出す
//            fetch(`/Device/DeleteConfirmed/${selectedId}`, {
//                method: "POST",
//                headers: {
//                    "X-Request-With": "XMLHttpRequest",
//                    "Content-Type": "application/json",
//                    "RequestVerificationToken": getCsrfToken()  // CSRF対策トークンを送信
//                },
//                body: null
//            })
//                .then(res => res.json())    // レスポンスをJSONとしてパース
//                .then(data => {

//                    // モーダルを閉じる
//                    deleteModal.hide();

//                    if (data.success) {
//                        // 成功時：一覧に自動で遷移
//                        window.location.href = "/Device/Index";

//                    } else {
//                        // 失敗は画面内アラート表示の画面へ戻す
//                        window.location.href = `/Device/Details/${selectedId}`;
//                    }
//                })
//                .catch(() => {
//                    // // 通信エラー時：モーダルを閉じてエラートースト表示
//                    deleteModal.hide();
//                    showToast("通信エラーが発生しました。", false);
//                });
//        });
//    }

//    // ----- CSRF用Token取得関数 -----
//    function getCsrfToken() {
//        // @Html.AntiForgeryToken() で埋め込まれる hidden input から値を取得
//        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
//        return tokenInput ? tokenInput.value : "";
//    }
//});
