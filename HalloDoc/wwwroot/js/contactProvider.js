$(document).ready(function () {
    $(document).on('click', '.open-modal', function () {

        var requestId = $(this).data('request-id');

        //var patientPhone = data.phonenumber;
        //var patientEmail = data.email;

        //$('#' + modalId).find('#phoneNumberSendAgreement').prop("value", patientPhone);
        //$('#' + modalId).find('#emailSendAgreement').prop("value", patientEmail);

        // Show the modal
        $('#contactProviderModal').modal('show');
    });

    $('.close').on('click', function () {

        $('#contactProviderModal    ').modal('hide');

    })

})