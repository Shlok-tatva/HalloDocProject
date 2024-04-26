$(document).ready(function () {

    $("#resetPassword").on("click", function () {
        var password = $("#Adminpassword").val();
        var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\@\$!%*?&])[A-Za-z\d\@\$!%*?&]{8,}$/;


        if (!passwordRegex.test(password)) {
            showToaster("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.", "error");
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
                    url: "/changeAdminPassword",
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


    $('#statusSelect, #roleSelect').change(function () {
        $('#saveAccountInfo').show();
    });

    $('#saveAccountInfo').click(function () {
        debugger
        var adminId = $('#adminId').val();
        var status = $('#statusSelect').val();
        var roleId = $('#roleSelect').val();

        $.ajax({
            url: '/Admin/changeAdminRoleOrStatus',
            method: 'POST',
            data: { adminId: adminId, status: status, roleId: roleId },
            success: function (response) {
                showToaster("Account Info edited successfully!", "success");
            },
            error: function (xhr, status, error) {
                showToaster("Something Wrong !", "error");
            }
        });
    });


    $(".readonly").prop('readonly', true);

    EditSaveToggle("#editButton", ".togleInputAdminInfo");
    EditSaveToggle("#editButton2", ".togleInputMailingInfo")

    function EditSaveToggle(buttonId, toggleClass) {
        $(buttonId).click(function () {
            if ($('#adminProfile').valid()) {
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
            }
        });
    }

    $("#StateSelect").on("change", function () {
        $("#stateId").prop("value", $(this).val());
    })

    var initialCheckboxState = {};

    // Store initial checkbox state when page loads
    $('input[type="checkbox"]').each(function () {
        var regionId = $(this).val();
        var isChecked = $(this).is(':checked');
        initialCheckboxState[regionId] = isChecked;
    });



    $('#editButton').click(function (e) {
        e.preventDefault();
        if ($('#adminProfile').valid()) {
            handleSaveData("#adminProfile", "#editButton");
        }
    });

    $('#editButton2').click(function (e) {
        e.preventDefault();
        if ($('#adminProfile').valid()) {
            handleSaveData("#adminProfile", "#editButton2");
        }
    });



    function handleSaveData(formId, buttonId) {
        debugger
        var changedCheckboxData = [];

        $('input[type="checkbox"]').each(function () {
            var regionId = $(this).val();
            var finalIsChecked = $(this).is(':checked');
            var initialIsChecked = initialCheckboxState[regionId];


            if (initialIsChecked !== finalIsChecked) {
                changedCheckboxData.push({ regionId: regionId, isChecked: finalIsChecked });
            }
        });

        var formdata = $(formId).serializeArray();
        var formdataObject = {};
        formdata.forEach(function (item) {
            formdataObject[item.name] = item.value;
        })

        console.log(formdataObject);

        var requestData = {
            formdata: formdataObject,
            regions: changedCheckboxData
        };

        console.log(requestData);

        if ($(buttonId).text() === "Edit") {
            $.ajax({
                url: 'UpdateAdminInfo',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(requestData),
                success: function (response) {
                    showToaster("Profile Data change successfully!", "success");
                },
                error: function (xhr, status, error) {
                    showToaster("Error While changing Data!", "error");
                }
            });
        }
    }


})
