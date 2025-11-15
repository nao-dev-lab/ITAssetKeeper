$(document).on("click", ".password-toggle img", function () {
    const $img = $(this);
    const $input = $img.closest(".input-group").find("input");

    if ($input.attr("type") === "password") {
        // パスワード → 表示へ
        $input.attr("type", "text");
        $img.attr("src", "/img/eye_on.png");
    } else {
        // 表示 → パスワードへ
        $input.attr("type", "password");
        $img.attr("src", "/img/eye_off.png");
    }
});
