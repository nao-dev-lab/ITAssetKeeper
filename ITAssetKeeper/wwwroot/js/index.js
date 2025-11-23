$(function () {

    // --- 検索ボタン押下 → 確定条件 hidden を更新 ---
    $("#searchFormSimple, #searchFormDetail").on("submit", function () {

        // simple or detail のフォーム
        const form = $(this);

        // 入力欄を走査して confirmed_◯◯ hidden に反映
        form.find("input, select").each(function () {

            const name = $(this).attr("name");
            const value = $(this).val();

            // hidden の ID は "confirmed_項目名" の形式
            const hidden = $("#confirmed_" + name);

            if (hidden.length > 0) {
                hidden.val(value);   // ⇒ 確定
            }
        });

        // activeSearchType を更新（simple/detail）
        const type = form.find("input[name=activeSearchType]").val();
        $("#activeSearchType").val(type);
    });

    // --- ソート変更部分 ---
    // SortKey または SortOrder が変わったら Ajax 実行
    $("#SortKeyValue, #SortOrderValue").on("change", function () {

        // 検索フォーム側 hidden に反映 (検索ボタン押下時の引き継ぎ用)
        $("#SortKeyValueHidden").val($("#SortKeyValue").val());
        $("#SortOrderValueHidden").val($("#SortOrderValue").val());

        // confirmed hidden のみからデータを作成
        const confirmedData = collectConfirmedConditions();

        //var form = getActiveForm();
        var sortKey = $("#SortKeyValue").val();
        var sortOrder = $("#SortOrderValue").val();

        var data =
            confirmedData
            + "&sortKey=" + sortKey         // Ajax用
            + "&sortOrder=" + sortOrder     // Ajax用
            + "&PageNumber=1";              // ソート変更時は必ず1ページ目

        // 呼び出し元から渡された値で呼び出し先を指定
        var ajaxUrl = $("#sortForm").data("ajax-url");

        $.ajax({
            url: ajaxUrl,
            type: "GET",
            data: data,
            success: function (html) {
                $("#tableArea").html(html);   // テーブル更新
            },
            error: function () {
                console.error("ソート更新でエラーが発生しました");
            }
        });

    });

    // --- ページング部分 ---
    $(document).on("click", ".paging-link", function (event) {
        // リンクのデフォルト動作をキャンセル
        event.preventDefault();

        var page = $(this).data("page");
        const confirmedData = collectConfirmedConditions();
        //var form = getActiveForm();

        // ページング処理をしてもソートの設定を引き継ぐようにする
        var sortKey = $("#SortKeyValue").val();
        var sortOrder = $("#SortOrderValue").val();

        var data =
            confirmedData
            + "&PageNumber=" + page
            + "&SortKeyValue=" + sortKey
            + "&SortOrderValue=" + sortOrder;

        // 呼び出し元から渡された値で呼び出し先を指定
        var ajaxUrl = $('nav[aria-label="Page navigation"]').data("ajax-url");

        $.ajax({
            url: ajaxUrl,
            type: "GET",
            data: data,
            success: function (html) {
                $("#tableArea").html(html);  // テーブル更新
            },
            error: function () {
                console.error("ページング処理でエラーが発生しました");
            }
        });
    });


    // --- 詳細検索 開閉ボタン ---
    // 上部の「閉じる / 詳細検索」バー（開閉用）
    const toggleBtn = document.getElementById("toggleSearchBtn");
    const collapseEl = document.getElementById("searchCollapse");

    if (toggleBtn && collapseEl) {

        // ViewModel側から2つの状態を取得
        const isSearchExecuted = toggleBtn.getAttribute("data-is-search") === "true";
        const hasAnyFilter = toggleBtn.getAttribute("data-has-filter") === "true";

        // 初期展開状態の制御
        // ・初回：閉じる
        // ・検索実行済み かつ フィルタ入力あり → 開く
        if (isSearchExecuted && hasAnyFilter) {
            collapseEl.classList.add("show");
            setToggleOpen();
        } else {
            collapseEl.classList.remove("show");
            setToggleClosed();
        }

        // --- Bootstrap のイベントフック ---
        // 開いた時
        collapseEl.addEventListener("shown.bs.collapse", () => {
            setToggleOpen();
        });

        // 閉じた時
        collapseEl.addEventListener("hidden.bs.collapse", () => {
            setToggleClosed();
        });
    }

    // トグルボタン UI：開いた時
    function setToggleOpen() {
        toggleBtn.innerHTML =
            `<img src="/img/arrow_up.png" class="detail-toggle-icon"> <span class="detail-toggle-text">閉じる</span>`;
        toggleBtn.classList.add("open");
    }

    // トグルボタン UI：閉じた時
    function setToggleClosed() {
        toggleBtn.innerHTML =
            `<img src="/img/arrow_down.png" class="detail-toggle-icon"> <span class="detail-toggle-text">詳細検索</span>`;
        toggleBtn.classList.remove("open");
    }
});

// confirmed hidden をすべて集める
function collectConfirmedConditions() {

    let query = "";

    // name 属性を持つ confirmed hidden をすべて列挙
    $("[id^='confirmed_']").each(function () {
        // hidden の name 属性 と value を取得
        const name = $(this).attr("name");
        const value = $(this).val();

        if (name) {
            // クエリ文字列へ変換
            query += `&${name}=${encodeURIComponent(value)}`;
        }
    });

    // activeSearchType も送る
    const type = $("#activeSearchType").val();
    query += `&activeSearchType=${type}`;

    return query;
}
