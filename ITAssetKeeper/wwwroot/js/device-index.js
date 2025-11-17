$(function () {

    // ソート変更部分
    // SortKey または SortOrder が変わったら Ajax 実行
    $('#SortKeyValue, #SortOrderValue').on('change', function () {

        var form = $('#searchForm');
        //var sortKey = $('#SortKeyValue').val();
        //var sortOrder = $('#SortOrderValue').val();

        // ソートに変更があったら、最初のページに戻す
        var data = form.serialize()
            //+ '&sortKey=' + sortKey
            //+ '&sortOrder=' + sortOrder
            + '&PageNumber=1';

        $.ajax({
            url: '/Device/GetSortedList',
            type: 'GET',
            data: data,
            success: function (html) {
                $('#deviceTableArea').html(html);   // テーブルだけを更新
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

        $.ajax({
            url: '/Device/GetPagedList',
            type: 'GET',
            data: data,
            success: function (html) {
                $('#deviceTableArea').html(html);  // テーブルだけを更新
            },
            error: function () {
                console.error("ページング処理でエラーが発生しました");
            }
        });
    });
});
