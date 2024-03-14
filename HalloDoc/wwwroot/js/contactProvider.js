$(document).ready(function () {
    $(document).on('click', '.open-modal', function () {
        debugger
        var requestId = $(this).data('physicianid');
        var email = $(this).data('email');
        var phonenumber = $(this).data('phone');
        console.log(email);
        $('#contactProviderModal').find('#physicianId').prop("value", requestId);
        $('#contactProviderModal').find('#email').prop("value", email);
        $('#contactProviderModal').find('#phoneNumber').prop("value", phonenumber);

        // Show the modal
        $('#contactProviderModal').modal('show');
    });

    $('.close').on('click', function () {

        $('#contactProviderModal    ').modal('hide');

    })

    $("#contactProvider").on('submit', function (e) {
        debugger;
        e.preventDefault();
        var physicianId = +$("#physicianId").val();
        var selectedRadio = $("input:radio[name=channel]:checked").val();
        var email = $("#email").val();
        var phone = $("#phoneNumber").val();
        var data = new Object({
            physicianId,
            selectedRadio,
            email,
            phone,
        })
        console.log(data);

    })

})