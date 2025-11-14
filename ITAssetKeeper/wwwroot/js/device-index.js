$(function () {

    // SortKey または SortOrder が変わったら Ajax 実行
    $('#SortKeyValue, #SortOrderValue').on('change', function () {

        var form = $('#searchForm');
        var sortKey = $('#SortKeyValue').val();
        var sortOrder = $('#SortOrderValue').val();

        $.ajax({
            url: '/Device/GetSortedList',
            type: 'GET',
            data: form.serialize() + '&sortKey=' + sortKey + '&sortOrder=' + sortOrder,
            success: function (html) {
                $('#deviceTableArea').html(html);   // テーブルだけを更新
            },
            error: function () {
                console.error("ソート更新でエラーが発生しました");
            }
        });

    });

});
