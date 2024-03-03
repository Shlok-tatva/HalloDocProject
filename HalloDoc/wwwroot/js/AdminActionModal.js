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
        console.log($('#requestIdcanleCase').val());
        formData.append('reason', $('#reasonForCancle').val());
        console.log($('#reasonForCancle').val());

        formData.append('adminId', 1); // change to 4 for Company
        formData.append('notes', $('#additionalNotes').val());

        debugger;

        $.ajax({
            url: "/CancelCase",
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                Swal.fire("Case Cancel Successfully!", "", "success").then(function () {
                    window.location.reload();
                });
            },
            error: function (xhr, status, error) {
                toastMixin.fire({
                    animation: true,
                    title: 'Error While Save Changes!',
                    icon: 'error'
                });
            }
        });
    })

    /*Assign Case Ajax Call */

    $('#assignCaseModal').on("submit", function (e) {
        e.preventDefault();

        debugger;
        var formData = new FormData();
        formData.append('requestId', $('#requestIdassignCase').val());
        formData.append('physicianId', $('#physicianSelect').val());

        console.log($('#requestIdassignCase').val());
        console.log($('#physicianSelect').val());

        $.ajax({
            url: "/AssignCase",
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                Swal.fire("Case Assign to Physician Successfully!", "", "success").then(function () {
                    window.location.reload();
                });
            },
            error: function (xhr, status, error) {
                toastMixin.fire({
                    animation: true,
                    title: 'Error While assign Case to Physician',
                    icon: 'error'
                });
            }
        });


    });


    var toastMixin = Swal.mixin({
        toast: true,
        icon: 'success',
        title: 'General Title',
        animation: false,
        position: 'top-right',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
          toast.addEventListener('mouseenter', Swal.stopTimer)
          toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
      });

});

