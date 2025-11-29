// --- キャンセルボタン押下時の確認アラート表示 ---
document.addEventListener('DOMContentLoaded', function () {
    // フォーム変更フラグ
    let isFormChenged = false;

    // フォーム要素を取得
    const forms = document.querySelectorAll('form');

    // 各フォームに対して変更検知イベントを設定
    forms.forEach(function (form) {

        // 入力変更を検知したらフラグを立てる
        form.addEventListener('change', function () {
            isFormChenged = true;
        });
    });

    // キャンセルボタン押下時の処理
    const cancelBtn = document.getElementById("cancel");

    // キャンセルボタンが存在しない場合は処理を終了
    if (!cancelBtn) return;

    // キャンセルボタンにクリックイベントを設定
    cancelBtn.addEventListener("click", () => {

        // キャンセル元の画面を取得
        const from = cancelBtn.dataset.from;

        // フォーム未変更の場合は確認なしで遷移
        if (!isFormChenged) {
            if (from === "create") {
                return window.location.href = "/Device/Create/";
            }
            else if (from === "edit") {
                const id = cancelBtn.dataset.hiddenid;
                return window.location.href = "/Device/Edit/" + id;
            }
        };

        // フォーム変更済みの場合は確認アラートを表示
        if (window.confirm("変更内容が破棄されます。キャンセルしてよろしいですか？")) {
            if (from === "create") {
                return window.location.href = "/Device/Create/";
            }
            else if (from === "edit") {
                const id = cancelBtn.dataset.hiddenid;
                return window.location.href = "/Device/Edit/" + id;
            }
        }
        else {
            return;
        }
    })
});

