$(document).ready(function () {

    $('#selectAllCheckbox').change(function () {
        $('.shiftcheckBox').prop('checked', $(this).prop('checked'));
       
    });

    $('#approvedShift').click(function () {
        debugger
        if ($('.shiftcheckBox:checked').length == 0) {
            showToaster("Please Select At least One Shift!", "error");
        } else {
            $('.shiftcheckBox:checked').each(function () {
                var shiftid = $(this).closest('tr').data('shiftid');
                approvedShift(shiftid);
            });
        }
    });

    $('#deleteShift').click(function () {
        if ($('.shiftcheckBox:checked').length == 0) {
            showToaster("Please Select At least One Shift!", "error");
        }
        else {
            $('.shiftcheckBox:checked').each(function () {
                var shiftid = $(this).closest('tr').data('shiftid');
                deleteShift(shiftid);
            });
        }
    });

    $("#CurrentMonthShift").on('click', function () {
        var regionId = $('#regionRequestedShift').val();
        $.ajax({
            url: "CuurentMonthUnApprovedShift?regionId=" + regionId,
            type: "GET",
            success: function (data) {
                $("#requestedShiftTbody").empty(); // Remove existing content
                $("#requestedShiftTbody").html(data);
            },
            error: function () {
                showToaster('Failed to fetch Shifts.', 'error');
            }
        });
    });


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



    function approvedShift(shiftid) {
        $.ajax({
            type: "POST",
            url: 'UpdateshiftStatus?shiftId=' + shiftid,
            cache: false,
            success: function (response) {
                Swal.fire({
                    title: "Done",
                    text: "Shift Approved Sucessfully",
                    icon: "success",
                    showConfirmButton: false,
                    timer: 1000
                }).then(function () {
                    location.reload();
                })
            },
            error: function () {
                showToaster("Error while change status", "error");
            }
        });
    }

    function deleteShift(shiftid) {
        $.ajax({
            type: "POST",
            url: 'DeleteShift?shiftid=' + shiftid,
            cache: false,
            success: function (response) {
                Swal.fire({
                    title: "Done",
                    text: "Shift Deleted Sucessfully",
                    icon: "success",
                    showConfirmButton: false,
                    timer: 1000
                }).then(function () {
                    location.reload();
                })
            },
            error: function () {
                alert("Error while checking email.");
            }
        });
    }

});
