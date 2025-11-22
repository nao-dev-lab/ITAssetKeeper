// --- 詳細検索フォーム 入力内容クリア ---

function clearForm() {

    const form = document.getElementById("searchFormDetail");

    if (!form) return;

    // 全 input をリセット
    form.querySelectorAll("input[type=text], input[type=date]").forEach(el => el.value = "");

    // 全 select をリセット
    form.querySelectorAll("select").forEach(el => el.selectedIndex = 0);
}

