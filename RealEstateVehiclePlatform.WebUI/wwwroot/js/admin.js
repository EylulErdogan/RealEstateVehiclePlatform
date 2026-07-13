// ===============================
// Success
// ===============================

function showSuccess(message) {

    Swal.fire({
        icon: "success",
        title: "Basarılı",
        text: message,
        confirmButtonColor: "#2563eb"
    });

}

// ===============================
// Error
// ===============================

function showError(message) {

    Swal.fire({
        icon: "error",
        title: "Hata",
        text: message,
        confirmButtonColor: "#dc2626"
    });

}

// ===============================
// Info
// ===============================

function showInfo(message) {

    Swal.fire({
        icon: "info",
        title: "Bilgi",
        text: message,
        confirmButtonColor: "#2563eb"
    });

}

// ===============================
// Warning
// ===============================

function showWarning(message) {

    Swal.fire({
        icon: "warning",
        title: "Uyarı",
        text: message,
        confirmButtonColor: "#f59e0b"
    });

}

// ===============================
// Delete Confirm
// ===============================

function confirmDelete(url, message) {

    Swal.fire({

        title: "Silme İslemi",

        text: message,

        icon: "warning",

        showCancelButton: true,

        reverseButtons: true,

        focusCancel: true,

        confirmButtonText: "Evet, Sil",

        cancelButtonText: "İptal",

        confirmButtonColor: "#dc2626",

        cancelButtonColor: "#64748b"

    }).then((result) => {

        if (result.isConfirmed) {

            window.location.href = url;

        }

    });

}

// ===============================
// Auto Bind Delete Buttons
// ===============================

document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll(".delete-btn").forEach(function (button) {

        button.addEventListener("click", function (e) {

            e.preventDefault();

            const url = this.dataset.url;

            const message = this.dataset.title;

            confirmDelete(url, message);

        });

    });

});