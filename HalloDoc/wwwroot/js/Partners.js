$(document).ready(function () {

    contactModalHandler();

    $('#professionSelect').change(function () {
        console.log($(this).val());
        var professionId = +$(this).val();

        $.ajax({
            url: 'Partners', 
            type: 'GET',
            data: { professionId },
            success: function (data) {
                debugger
                console.log(data);
                $('#vendorTable').empty();
                $('#vendorTable').html(data);
                contactModalHandler();
                setupPagination(10);
            },
            error: function () {
                alert("error");
            }
        });
    });
})

function contactModalHandler() {
    $(".open-modal").unbind("click").on("click", function () {
        let vendorid = +$(this).data("vendorid");
        let vendorName = $(this).data("vendorname");

        const swalWithBootstrapButtons = Swal.mixin({
            customClass: {
                confirmButton: "btn btn-info mx-2 btn-lg text-white my-2 mb-2",
                cancelButton: "btn btn-outline-info btn-lg hover_white"
            },
            buttonsStyling: false
        });

        swalWithBootstrapButtons.fire({
            title: "Confirmation of Delete Vendor",
            text: `Are you sure you want to Delete this Vendor :- ${vendorName} ?`,
            iconHtml: "<div class='warning_icon'><i class='bi bi-exclamation-circle-fill'></i></div>",
            showCancelButton: true,
            confirmButtonText: "Delete",
            cancelButtonText: "Cancel",
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "DeletePartner",
                    method: "POST",
                    data: { vendorid },
                    success: function (response) {
                        Swal.fire({
                            title: "Deleted",
                            text: "Vendor Delete Successfully",
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false,
                        }).then(function () {
                            location.reload();
                        })
                    },
                    error: function (xhr, status, error) {
                        showToaster("Error While Delete Vendor", "error");
                    }
                });
            }
        });
    })
}

setupPagination(10);
