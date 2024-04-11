$(document).ready(function () {

    setupPagination(10 , ".data-row");

    $('#patientRecordSearch').on('submit', function (e) {
        e.preventDefault();

        // Check if any field value is not empty
        var anyFieldFilled = false;
        $(this).find('input[type="text"]').each(function () {
            if ($(this).val().trim().length > 0) {
                anyFieldFilled = true;
                return false; // Exit the loop early if any field is filled
            }
        });

        // If no field is filled, show error message
        if (!anyFieldFilled) {
            showToaster("Please fill out at least one field before Search." , "error");
            return;
        }

        // At least one field is filled, proceed with AJAX request
        var formData = $(this).serialize();
        // Make AJAX request to the controller
        $.ajax({
            url: 'PatientRecords',
            type: 'GET',
            data: formData,
            success: function (response) {
                debugger;
                console.log(response);
                $('#patientRecordData').html(response);
                setupPagination(10 , ".data-row");
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText); // Log error to console
            }
        });
    });



})