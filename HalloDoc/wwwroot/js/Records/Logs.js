$(document).ready(function () {
    setupPagination(10 , ".data-row");
    $('#emailLogsForm').submit(function (e) {
        e.preventDefault(); 
        searchEmailLogs(); 
    });
})

function searchEmailLogs() {

    var formData = {
        accountType: $('#AccountType').val(),
        receiverName: $('#ReceiverName').val(),
        emailId: $('#Email').val(),
        createdDate: $('#createdDate').val(),
        sentDate: $('#sentDate').val()
    };

    $.ajax({
        url: 'EmailLogsBySearch',
        type: 'GET',
        data: formData,
        success: function (response) {
            $('#emaillogTable').html(response);
        },
        error: function (xhr, status, error) {
            // Handle errors
            console.error(error);
        }
    });
}