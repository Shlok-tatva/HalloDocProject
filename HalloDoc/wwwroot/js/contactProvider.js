$(document).ready(function () {

    physicianNotificationHandler();

    $(document).on('click', '.open-modal', function () {
        debugger
        var requestId = $(this).data('physicianid');
        var email = $(this).data('email');
        var phonenumber = $(this).data('phone');
        var adminId = $(this).data("adminid");
        console.log(email);
        $('#contactProviderModal').find('#physicianId').prop("value", requestId);
        $('#contactProviderModal').find('#email').prop("value", email);
        $('#contactProviderModal').find('#phoneNumber').prop("value", phonenumber);
        $("#contactProviderModal").find('#adminId').prop("value", adminId);

        // Show the modal
        $('#contactProviderModal').modal('show');
    });

    $('.close').on('click', function () {

        $('#contactProviderModal').modal('hide');

    })

    $("#contactProvider").on('submit', function (e) {
        debugger;
        e.preventDefault();
        var physicianId = +$("#physicianId").val();
        var selectedRadio = $("input:radio[name=channel]:checked").val();
        var email = $("#email").val();
        var phone = $("#phoneNumber").val();
        var adminId = +$("#adminId").val();
        var message = $("#message").val();

        var data = {
            physicianId,
            selectedRadio,
            email,
            phone,
            adminId,
           message
        };

        $.ajax({
            url: "contactProvider",
            method: "POST",
            data: data, 
            async: true,
            success: function (response) {
                console.log(response);
                Swal.fire({
                    title: "Done",
                    text: response.message,
                    icon: "success",
                    showConfirmButton: false,
                    timer: 2000,
                }).then(function () {
                    $('#contactProviderModal').modal('hide');

                })

              
            },
            error: function (xhr, status, error) {
                Swal.fire({
                    title: "Error while sending Message",
                    icon: "error",
                    showConfirmButton: false,
                    timer: 2000,
                })
            }
        })

    })



    function physicianNotificationHandler() {
        var initialCheckboxState = {};

        $('input[type="checkbox"]').each(function () {
            var physicianId = $(this).data('physicianid');
            var isChecked = $(this).is(':checked');
            initialCheckboxState[physicianId] = isChecked;
        });

        $('input[type="checkbox"]').change(function () {
            $('#saveBtn').show();
        });

        $('#saveBtn').click(function () {
            var changedCheckboxData = [];

            // Compare initial state with final state of checkboxes
            $('input[type="checkbox"]').each(function () {
                var physicianId = $(this).data('physicianid');
                var finalIsChecked = $(this).is(':checked');
                var initialIsChecked = initialCheckboxState[physicianId];
                if (initialIsChecked !== finalIsChecked) {
                    changedCheckboxData.push({ physicianId: physicianId, isChecked: finalIsChecked });
                }
            });

            console.log(changedCheckboxData);
            $.ajax({
                url: 'ChangeProviderNotificationStatus',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(changedCheckboxData),
                success: function (response) {

                    Swal.fire({
                        title: "Done",
                        text: response.message,
                        icon: "success",
                        showConfirmButton: false,
                        timer: 2000,
                    }).then(function () {
                        $('#saveBtn').hide();
                    })
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: "Error while Updating status",
                        icon: "error",
                        showConfirmButton: false,
                        timer: 2000,
                    })
                }
            });

        });
    }

})