$(document).ready(function () {
    $(".edit-button").click(function () {
        var section = $(this).data("section");
        debugger
        $(this).hide();
        $(this).siblings(".save-button").show();
        $(`input[data-section="${section}"]`).removeAttr("readonly");
        $(`input[data-section="${section}"]`).removeClass("readonly");
        $("#regionsearch").removeAttr("disabled");
        $("#regionsearch").removeClass("readonly");

    });

    $(".save-button").click(function () {
        var section = $(this).data("section");
        $(this).siblings(".edit-button").show();
        $(this).hide();
        $(`input[data-section="${section}"]`).attr("readonly", "readonly");
        $(`input[data-section="${section}"]`).addClass("readonly");
        $("#regionsearch").attr("disabled", "disabled");
        $("#regionsearch").addClass("readonly");

    });

    $("#regionsearch").on("change", function () {
        $("#regionid").val($(this).val());
    })

    $('#photofile').change(function () {
        $('#photoPreview').hide(); 
    });

    $('#signature').change(function () {
        $('#signaturePreview').hide(); 
    });

    $(".open-modal").on("click", function () {
        debugger
        let providerId = +$(this).data("providerid");
        const swalWithBootstrapButtons = Swal.mixin({
            customClass: {
                confirmButton: "btn btn-info mx-2 btn-lg text-white my-2 mb-2",
                cancelButton: "btn btn-outline-info btn-lg hover_white"
            },
            buttonsStyling: false
        });

        swalWithBootstrapButtons.fire({
            title: "Confirmation of Delete Provider",
            text: `Are you sure you want to Delete this Provider ?`,
            iconHtml: "<div class='warning_icon'><i class='bi bi-exclamation-circle-fill'></i></div>",
            showCancelButton: true,
            confirmButtonText: "Delete",
            cancelButtonText: "Cancel",
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "DeleteProvider",
                    method: "POST",
                    data: { providerId },
                    success: function (response) {
                        Swal.fire({
                            title: "Deleted",
                            text: "Provider Delete Successfully",
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false,
                        }).then(function () {
                            window.location.href = "/Admin/Provider";
                        })
                    },
                    error: function (xhr, status, error) {
                        showToaster("Error While Delete Provider", "error");
                    }
                });
            }
        });
    })





    var downloadLinks = document.querySelectorAll('.download-link');

    // Loop through each download link
    downloadLinks.forEach(function (link) {
        link.addEventListener('click', function (event) {
            event.preventDefault();
            var fileName = this.getAttribute('href').split('/').pop();
            var fileInput = this.closest('.document').querySelector('.fileInput');
            fileInput.nextElementSibling.textContent = fileName;
        });
    });

    // Change Password of Provider

    $("#resetPassword").on("click", function () {
        var password = $("#providerPassword").val();
        if (password.length < 5) {
            showToaster("minimum length of password should be 5", "error");
            return;
        }

        const swalWithBootstrapButtons = Swal.mixin({
            customClass: {
                confirmButton: "btn btn-info mx-2 btn-lg text-white my-2 mb-2",
                cancelButton: "btn btn-outline-info btn-lg hover_white"
            },
            buttonsStyling: false
        });

        swalWithBootstrapButtons.fire({
            title: "Are you sure you want to Update Password",
            icon: "warning",
            buttons: true,
            showCancelButton: true,
            confirmButtonText: "Yes",
            dangerMode: true,
        }).then((willAgree) => {
            if (willAgree.isConfirmed) {
                var providerID = +$("#providerID").val();
                var formdata = new FormData();
                formdata.append("providerId", providerID);
                formdata.append("password", $("#providerPassword").val());
                $.ajax({
                    url: "/changeProviderPassword",
                    type: 'POST',
                    data: formdata,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        debugger
                        console.log(response);
                        showToaster("Password change successfully!", "success");
                    },
                    error: function (error) {
                        debugger
                        console.log(error)
                        showToaster("Failed to change Password", "error");
                    }
                });


            }
        });
    })




});