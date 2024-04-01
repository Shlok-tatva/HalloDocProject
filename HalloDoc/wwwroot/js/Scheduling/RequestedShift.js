$(document).ready(function () {
    // Function to enable/disable the approve button
    function updateApproveButton() {
        if ($('#selectAllCheckbox').prop('checked') || $('.shiftcheckBox:checked').length > 0) {
            $('#approvedShift').prop('disabled', false);
        } else {
            $('#approvedShift').prop('disabled', true);
        }
    }

    $('#selectAllCheckbox').change(function () {
        $('.shiftcheckBox').prop('checked', $(this).prop('checked'));
        updateApproveButton(); // Update approve button status
    });

    $('.shiftcheckBox').change(function () {
        updateApproveButton(); // Update approve button status
    });

    $('#approvedShift').click(function () {
        if ($('.shiftcheckBox:checked').length == 0) {
            showToaster("Please Select At least One Shift!", "error");
        } else {
            $('.shiftcheckBox:checked').each(function () {
                //var filePath = $(this).closest('tr').find('.download-btn').data('file');
                //downloadFile(filePath);
            });
        }
    });

    $('#deleteShift').click(function () {
        if ($('.shiftcheckBox:checked').length == 0) {
            showToaster("Please Select At least One Shift!", "error");
        }
        else {
            $('.shiftcheckBox:checked').each(function () {
                //var filePath = $(this).closest('tr').find('.delete-btn').data('file');
                //var fileId = $(this).closest('tr').find('.delete-btn').data('fileid');
                //deleteFile(filePath, fileId);
            });
        }
    });

    // Initially disable the approve button
    $('#approvedShift').prop('disabled', true);

    $("#regionRequestedShift").on("change", function () {
        var regionId = $(this).val();
        $.ajax({
            url: "ShiftReview?regionId=" + regionId,
            type: "GET",
            success: function (data) {
                $("#requestedShiftTbody").empty(); // Remove existing content
                $("#requestedShiftTbody").html(data);
                console.log(data);
            },
            error: function () {
                console.error('Failed to fetch physicians.');
            }
        });
    });
});
