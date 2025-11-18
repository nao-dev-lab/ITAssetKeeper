// トースト通知：共通用

document.addEventListener("DOMContentLoaded", function () {

    const holder = document.getElementById("toast-message");
    if (!holder) return;

    const message = holder.dataset.message;
    const type = holder.dataset.type || "success"; // success/error/warn を想定

    const toastEl = document.getElementById("resultToast");
    const toastBody = document.getElementById("toastMessage");

    if (!toastEl || !toastBody) return;

    // Bootstrap toast settings (autohide true, 7s)
    const toast = new bootstrap.Toast(toastEl);

    // apply message
    toastBody.textContent = message;

    // apply type class
    toastEl.classList.remove("success", "error", "warn");
    toastEl.classList.add(type);

    // show
    toast.show();
});
