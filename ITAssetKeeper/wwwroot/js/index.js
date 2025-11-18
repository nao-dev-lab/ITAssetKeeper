$(function () {

    // ソート変更部分
    // SortKey または SortOrder が変わったら Ajax 実行
    $('#SortKeyValue, #SortOrderValue').on('change', function (event) {

        // 検索フォーム側 hidden に反映 (検索ボタン押下時の引き継ぎ用)
        $('#SortKeyValueHidden').val($('#SortKeyValue').val());
        $('#SortOrderValueHidden').val($('#SortOrderValue').val());

        var form = $('#searchForm');
        var sortKey = $('#SortKeyValue').val();
        var sortOrder = $('#SortOrderValue').val();

        var data = form.serialize()
            + '&sortKey=' + sortKey         // Ajax用
            + '&sortOrder=' + sortOrder     // Ajax用
            + '&PageNumber=1';              // ソート変更時は必ず1ページ目

        // 呼び出し元から渡された値で呼び出し先を指定
        var ajaxUrl = $('#sortForm').data('ajax-url');

        $.ajax({
            url: ajaxUrl,
            type: 'GET',
            data: data,
            success: function (html) {
                $('#tableArea').html(html);   // テーブル更新
            },
            error: function () {
                console.error("ソート更新でエラーが発生しました");
            }
        });

    });

    // ページング部分
    $(document).on('click', '.paging-link', function (event) {
        // リンクのデフォルト動作をキャンセル
        event.preventDefault();

        var page = $(this).data('page');
        var form = $('#searchForm');

        // ページング処理をしてもソートの設定を引き継ぐようにする
        var sortKey = $('#SortKeyValue').val();
        var sortOrder = $('#SortOrderValue').val();

        var data = form.serialize()
            + '&PageNumber=' + page
            + '&SortKeyValue=' + sortKey
            + '&SortOrderValue=' + sortOrder;

        // 呼び出し元から渡された値で呼び出し先を指定
        var ajaxUrl = $('nav[aria-label="Page navigation"]').data('ajax-url');

        $.ajax({
            url: ajaxUrl,
            type: 'GET',
            data: data,
            success: function (html) {
                $('#tableArea').html(html);  // テーブル更新
            },
            error: function () {
                console.error("ページング処理でエラーが発生しました");
            }
        });
    });
});
