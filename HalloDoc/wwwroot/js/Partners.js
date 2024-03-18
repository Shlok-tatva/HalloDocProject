$(document).ready(function () {
    $(".open-modal").on("click", function () {
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
                        toastMixin.fire({
                            animation: true,
                            title: 'Error While Delete Vendor',
                            icon: 'error'
                        });
                    }
                });
            }
        });
    })

    $('#professionSelect').change(function () {
        console.log($(this).val());
        var professionId = +$(this).val();

        $.ajax({
            url: 'vendorDataByProfession', 
            type: 'GET',
            data: { professionId },
            success: function (data) {
                console.log(data);
                $('#vendorTable tbody').empty();
               // data.forEach(function (item) {
               //     var newRow = $('<tr class="data-row">');

               //     for (key in item) {
               //         newRow.append('<td class="' + key + '">' + item[key] + '</td>');
               //     }

               //$('#vendorTable tbody').append(newRow);
               // })


            },
            error: function () {
                alert("error");
            }
        });
    });
})

var toastMixin = Swal.mixin({
    toast: true,
    icon: 'success',
    title: 'General Title',
    animation: false,
    position: 'top-right',
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
});

setupPagination(10);
