document.addEventListener("DOMContentLoaded", () => {

    const logoutLink = document.getElementById("logoutLink");
    if (!logoutLink) return;

    logoutLink.addEventListener("click", function (e) {
        e.preventDefault();

        const modalEl = document.getElementById("logoutModal");
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
    });

});
