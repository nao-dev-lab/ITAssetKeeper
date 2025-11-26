// ================================
// 一覧ページ（Device / History 共通）
// ソート、ページング、詳細検索開閉
// ================================

$(function () {
    bindSortChange();       // ソート時の処理
    bindPaging();           // ページング処理
    initSearchToggle();     // 詳細検索UIの開閉処理
});


// ======================================
// 1. ソート変更
// ======================================
function bindSortChange() {

    $("#SortKeyValue, #SortOrderValue").on("change", function () {

        // hidden に反映（検索ボタン引き継ぎ用）
        $("#SortKeyValueHidden").val($("#SortKeyValue").val());
        $("#SortOrderValueHidden").val($("#SortOrderValue").val());

        const confirmedData = collectConfirmedConditions();
        const sortKey = $("#SortKeyValue").val();
        const sortOrder = $("#SortOrderValue").val();

        const data =
            confirmedData +
            "&sortKey=" + sortKey +
            "&sortOrder=" + sortOrder +
            "&PageNumber=1";

        const ajaxUrl = $("#sortForm").data("ajax-url");

        $.ajax({
            url: ajaxUrl,
            type: "GET",
            data: data,
            success: function (html) {
                $("#tableArea").html(html);
            },
            error: function () {
                console.error("ソート更新でエラーが発生しました");
            }
        });
    });
}


// ======================================
// 2. ページング
// ======================================
function bindPaging() {

    $(document).on("click", ".paging-link", function (event) {
        event.preventDefault();

        const page = $(this).data("page");
        const confirmedData = collectConfirmedConditions();

        const sortKey = $("#SortKeyValue").val();
        const sortOrder = $("#SortOrderValue").val();

        const data =
            confirmedData +
            "&PageNumber=" + page +
            "&SortKeyValue=" + sortKey +
            "&SortOrderValue=" + sortOrder;

        const ajaxUrl = $('nav[aria-label="Page navigation"]').data("ajax-url");

        $.ajax({
            url: ajaxUrl,
            type: "GET",
            data: data,
            success: function (html) {
                $("#tableArea").html(html);
            },
            error: function () {
                console.error("ページング処理でエラーが発生しました");
            }
        });
    });
}


// ======================================
// 3. 詳細検索フォーム 開閉UI
// ======================================
function initSearchToggle() {

    const toggleBtn = document.getElementById("toggleSearchBtn");
    const collapseEl = document.getElementById("searchCollapse");

    if (!(toggleBtn && collapseEl)) return;

    const isSearchExecuted = toggleBtn.getAttribute("data-is-search") === "true";
    const hasAnyFilter = toggleBtn.getAttribute("data-has-filter") === "true";

    // 初期展開状態
    if (isSearchExecuted && hasAnyFilter) {
        collapseEl.classList.add("show");
        setToggleOpen();
    } else {
        collapseEl.classList.remove("show");
        setToggleClosed();
    }

    // Bootstrap collapse イベント
    collapseEl.addEventListener("shown.bs.collapse", () => setToggleOpen());
    collapseEl.addEventListener("hidden.bs.collapse", () => setToggleClosed());


    function setToggleOpen() {
        toggleBtn.innerHTML =
            `<img src="/img/arrow_up.png" class="search-detail-toggle-icon">
             <span class="search-detail-toggle-text">閉じる</span>`;
        toggleBtn.classList.add("open");
    }

    function setToggleClosed() {
        toggleBtn.innerHTML =
            `<img src="/img/arrow_down.png" class="search-detail-toggle-icon">
             <span class="search-detail-toggle-text">詳細検索</span>`;
        toggleBtn.classList.remove("open");
    }
}


// ======================================
//  confirmed hidden を集約
// ======================================
function collectConfirmedConditions() {

    let query = "";

    $("[id^='confirmed_']").each(function () {
        const name = $(this).attr("name");
        const value = $(this).val();
        if (name) {
            query += `&${name}=${encodeURIComponent(value)}`;
        }
    });

    return query;
}