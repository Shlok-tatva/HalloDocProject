$(document).ready(function () {

    setupPagination(5 , ".data-row");

    $('#BloclpatientSearch').on('submit', function (e) {
        e.preventDefault();

        var anyFieldFilled = false;
        $(this).find('.form-control').each(function () {
            if ($(this).val().trim().length > 0) {
                anyFieldFilled = true;
                return false;
            }
        });

        if (!anyFieldFilled) {
            showToaster("Please fill out at least one field before Search.", "error");
            return;
        }

        var formData = $(this).serialize();
        $.ajax({
            url: 'BlockHistory',
            type: 'GET',
            data: formData,
            success: function (response) {
                $('#blockUserData').html(response);
                setupPagination(5 , ".data-row");
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText); 
            }
        });
    });


})

function unblock(blockRequestId, requestId) {
    $.ajax({
        url: "unBlock",
        method: "POST",
        data: { Id: blockRequestId, requestId: requestId },
        success: function (response) {
            Swal.fire({
                title: "Done",
                text: "User unblock sucessfully",
                icon: "success",
                showConfirmButton: false,
                timer: 1500
            }).then(function () {
                location.reload();
            })
        },
        error: function (xhr, status, error) {
            showToaster("Error while unblocking user", "error");
        }
    });
}