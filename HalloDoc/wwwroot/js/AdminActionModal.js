/** For Modal of the request*/
$(document).ready(function () {

    /*Modal Click open the Modal  */
    $(document).on('click', '.open-modal', function () {
        var modalId = $(this).data('modal-id');
        var requestId = $(this).data('request-id');
        var data = getRequest(requestId);
        var patientName = data.firstname + " , " + data.lastname;
        // Update the modal body to display the requestId
        $('#' + modalId).find('#requestIdassignCase').prop("value", requestId);
        $('#' + modalId).find('#requestIdblockCase').prop("value", requestId);
        $('#' + modalId).find('#requestIdcanleCase').prop("value", requestId);

        $('#' + modalId).find('#patientName').text(patientName);
        // Show the modal
        $('#' + modalId).modal('show');
    });


    $('.close').on('click', function () {
        debugger
        $('#blockPatientModal').modal('hide');
        $('#cancelCaseModal').modal('hide');
        $('#assignCaseModal').modal('hide');

    })

    function getRequest(requestId) {
        var requestdata;
        $.ajax({
            url: '/Admin/GetRequest',
            type: "GET",
            data: { requestId: requestId },
            async: false, // synchronous request to ensure data is returned before proceding 
            success: function (data) {
                requestdata = data;
            },
            error: function (e) {
                alert("Error while fetching Data");
            }
        });
        return requestdata;
    }

    /* Block Request Ajax Call */
    $('#blockRequest').on('submit', function (e) {
        e.preventDefault();

        var formData = new FormData();
        formData.append('requestId', $('#requestIdblockCase').val());
        formData.append('reason', $('#reasonForBlock').val());
        formData.append('adminId', 4);

        $.ajax({
            url: "/BlockPatient",
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                Swal.fire("Saved!", "", "success").then(function () {
                    location.reload();
                });
            },
            error: function (xhr, status, error) {
                Swal.fire("Error While Save Changes!", "", "error");
            }
        });
    })

    /* Cancle Case Ajax Call*/
    $('#cancelCase').on('submit', function (e) {
        e.preventDefault();
        debugger

        var formData = new FormData();
        formData.append('requestId', $('#requestIdcanleCase').val());
        formData.append('reason', $('#reasonForCancle').val());
        formData.append('adminId', 4);
        formData.append('notes', $('#additionalNotes').val());

        $.ajax({
            url: "/CancelCase",
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                Swal.fire("Saved!", "", "success").then(function () {
                    location.reload();
                });
            },
            error: function (xhr, status, error) {
                Swal.fire("Error While Save Changes!", "", "error");
            }
        });
    })


});