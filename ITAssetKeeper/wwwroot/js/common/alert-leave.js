// ページ離脱警告（未保存チェック）
document.addEventListener('DOMContentLoaded', function () {

    let isFormDirty = false;
    let isSubmitting = false;

    const forms = document.querySelectorAll('form');

    forms.forEach(function (form) {

        // 入力変更を検知
        form.addEventListener('change', function () {
            if (!isSubmitting) {
                isFormDirty = true;
            }
        });

        // submit 実行時は警告を無効化
        form.addEventListener('submit', function () {
            isSubmitting = true;
            isFormDirty = false;
        });
    });

    // no-warning の場合は警告を無効化
    document.querySelectorAll('.no-warning').forEach(btn => {
        btn.addEventListener('click', () => {
            isFormDirty = false;
        });
    });

    // ページ離脱（クリック・ブラウザバック・URL直打ち等）
    window.addEventListener('beforeunload', function (e) {
        if (isFormDirty && !isSubmitting) {
            e.preventDefault();
            e.returnValue = '';
            return '';
        }
    });
});
