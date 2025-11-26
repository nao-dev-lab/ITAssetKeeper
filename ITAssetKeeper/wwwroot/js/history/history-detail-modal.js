// --- 履歴詳細モーダル表示用 --- 

// 履歴一覧の履歴IDがクリックされたら Ajax 実行
$(document).on("click", ".history-detail-link", function (e) {

    // リンクのデフォルト動作をキャンセル
    e.preventDefault();

    // data-id で渡された値(@item.Id)を取得
    const id = $(this).data("id");

    // 取得したIDをパラメータとしてDeviceHistory/Detailsを呼び出す
    $.ajax({
        url: "/DeviceHistory/Details/" + id,
        type: "GET"
    })
        // 完了したらモーダルのbodyに結果を差し込む
        .done(function (html) {
        $("#historyDetailContainer").html(html);

        // モーダル表示
        const modal = new bootstrap.Modal(document.getElementById("historyDetailModal"));
        modal.show();
        })
        .fail(function () {
            alert("履歴の取得に失敗しました。");
        });
    });