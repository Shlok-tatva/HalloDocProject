$(document).ready(function () {

    $("#resetPassword").on("click", function () {
        var password = $("#Adminpassword").val();
        if (password.length < 5) {
            toastMixin.fire({
                animation: true,
                title: 'minimum length of password should be 5',
                icon: 'error'
            });
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
                var adminId = +$("#adminId").val();
                var formdata = new FormData();
                formdata.append("adminId", adminId);
                formdata.append("password", $("#Adminpassword").val());
                $.ajax({
                    url: "/changePassword",
                    type: 'POST',
                    data: formdata,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        debugger
                        console.log(response);
                        toastMixin.fire({
                            animation: true,
                            title: 'Password change successfully!',
                            icon: 'success'
                        });
                    },
                    error: function (error) {
                        debugger
                        console.log(error)
                        toastMixin.fire({
                            animation: true,
                            title: 'Failed to change Password',
                            icon: 'error'
                        });
                    }
                });


            }
        });
    })



    $(".readonly").prop('readonly', true);

    EditSaveToggle("#editButton", ".togleInputAdminInfo");
    EditSaveToggle("#editButton2", ".togleInputMailingInfo")

    function EditSaveToggle(buttonId, toggleClass) {
        $(buttonId).click(function () {
            $(toggleClass).toggleClass("readonly").prop('readonly', function (i, readonly) {
                return !readonly;
            });

            if ($(".togleStateMailginfo") && toggleClass === ".togleInputMailingInfo") {
                $(".togleStateMailginfo").prop('disabled', function (i, disabled) {
                    return !disabled;
                });
            }

            if (toggleClass === ".togleInputAdminInfo") {
                $(".checkbox").prop('disabled', function (i, disabled) {
                    return !disabled;
                });
            }

            var buttonText = $(this).text();
            $(this).text(buttonText === "Edit" ? "Save" : "Edit");
        });
    }

    $("#StateSelect").on("change", function () {
        $("#stateId").prop("value", $(this).val());
    })

    var initialCheckboxState = {}; // Object to store initial checkbox state

    // Store initial checkbox state when page loads
    $('input[type="checkbox"]').each(function () {
        var regionId = $(this).val();
        var isChecked = $(this).is(':checked');
        initialCheckboxState[regionId] = isChecked;
    });



    $('#editButton').click(function (e) {
        e.preventDefault();
        handleSaveData("#adminProfile", "#editButton");
    });

    $('#editButton2').click(function (e) {
        e.preventDefault();
        handleSaveData("#adminProfile", "#editButton2");
    });



    //function handleSaveData(buttonId) {
    //    $(buttonId).on("click", function (e) {
    //        e.preventDefault();

    //        console.log($(buttonId).text());

    //        if ($(this).text() === "Edit") {

    //            $("#adminProfile").submit();
    //        }
    //    })
    //}

    function handleSaveData(formId , buttonId) {
        debugger
        var formData = new FormData($(formId)[0]);
        var changedCheckboxData = [];

        // Compare initial state with final state of checkboxes
        $('input[type="checkbox"]').each(function () {
            var regionId = $(this).val();
            var finalIsChecked = $(this).is(':checked');
            var initialIsChecked = initialCheckboxState[regionId];

            // If checkbox state has changed, include it in changedCheckboxData
            if (initialIsChecked !== finalIsChecked) {
                changedCheckboxData.push({ regionId: regionId, isChecked: finalIsChecked });
            }
        });

        var formdata = $(formId).serializeArray();

        var requestData = {
            formdata: formdata,
            regions: changedCheckboxData
        };

            console.log(requestData);

        if ($(buttonId).text() === "Edit") {
            debugger;
             //Send form data to the server via AJAX
            $.ajax({
                url: 'UpdateAdminInfo',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(requestData), // Convert requestData to JSON
                success: function (response) {
                    // Handle success response here
                    alert("datasend")
                },
                error: function (xhr, status, error) {
                    // Handle error response here
                    alert("error")
                }
            });
        }
    }


})







var toastMixin = Swal.mixin({
    toast: true,
    icon: 'success',
    title: 'General Title',
    animation: false,
    position: 'top-right',
    showConfirmButton: false,
    timer: 2000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
});