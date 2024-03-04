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
        $('#' + modalId).find('#requestIdTransferCase').prop("value", requestId);
        $('#' + modalId).find('#patientName').text(patientName);
        // Show the modal
        $('#' + modalId).modal('show');

        if (modalId == "clearCaseModal") {
            debugger;
            clearCase(requestId);
        }

    });


    $('.close').on('click', function () {
        $('#blockPatientModal').modal('hide');
        $('#cancelCaseModal').modal('hide');
        $('#assignCaseModal').modal('hide');
        $('#transferModal').modal('hide');
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
                Swal.fire({
                    title: "Done",
                    text: "Patient block Sucessfully",
                    icon: "success",
                    showConfirmButton: false,
                    timer: 1000
                }).then(function () {
                    location.reload();
                })
            },
            error: function (xhr, status, error) {
                Swal.fire("Error While Save Changes!", "", "error");
            }
        });
    })

    /* Cancle Case Ajax Call*/
    $('#cancelCase').on('submit', function (e) {
        e.preventDefault();

        var formData = new FormData();
        formData.append('requestId', $('#requestIdcanleCase').val());
        console.log($('#requestIdcanleCase').val());
        formData.append('reason', $('#reasonForCancle').val());
        console.log($('#reasonForCancle').val());

        formData.append('adminId', 4); // change to 4 for Company
        formData.append('notes', $('#additionalNotes').val());

        $.ajax({
            url: "/CancelCase",
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                Swal.fire({
                    title: "Done",
                    text: "Case Cancel Successfully",
                    icon: "success",
                    showConfirmButton: false,
                    timer: 1000
                }).then(function () {
                    location.reload();
                })
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
                Swal.fire({
                    title: "Done",
                    text: "Case assign to physcian Successfully",
                    icon: "success",
                    showConfirmButton: false,
                    timer: 1000
                }).then(function () {
                    location.reload();
                })
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

    /* Transfer Case Ajax Call */
    $('#transferModal').on("submit", function (e) {
        e.preventDefault();
        var formData = new FormData();
        formData.append('requestId', $('#requestIdTransferCase').val());
        formData.append('physicianId', $('#physicianSelectTransfer').val());
        formData.append('note' , $('#Description').val());

        $.ajax({
            url: "/TransferCase",
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                Swal.fire({
                    title: "Done",
                    text: "Case Transfer to physcian Successfully",
                    icon: "success",
                    showConfirmButton: false,
                    timer: 1500
                }).then(function () {
                    location.reload();
                })
            },
            error: function (xhr, status, error) {
                toastMixin.fire({
                    animation: true,
                    title: 'Error While Trasnfer Case to Physician',
                    icon: 'error'
                });
            }
        });


    });

    /* Clear Case Function*/
    function clearCase(requestId){
        const swalWithBootstrapButtons = Swal.mixin({
            customClass: {
              confirmButton: "btn btn-info mx-2 btn-lg text-white my-2 mb-2",
              cancelButton: "btn btn-outline-info btn-lg hover_white"
            },
            buttonsStyling: false
          });

        swalWithBootstrapButtons.fire({
            title: "Confirmation of Clear Case",
            text: `Are you sure you want to clear this request? Once clear this  request ${requestId} then you are not able to see this request.`,
            iconHtml: "<div class='warning_icon'><i class='bi bi-exclamation-circle-fill'></i></div>",
            showCancelButton: true,
            confirmButtonText: "Clear",
            cancelButtonText: "Cancle",
        }).then((result) => {
            console.log(requestId);
            if (result.isConfirmed) {
                console.log(requestId);
                $.ajax({
                    url: "/ClearCase",
                    method: "POST",
                    data: { requestId },
                    success: function (response) {
                        Swal.fire({
                            title: "Cleared",
                            text: "Request Cleared Sucessfully",
                            icon: "success",
                            showConfirmButton: false,
                        }).then(function () {
                            location.reload();
                        })
                    },
                    error: function (xhr, status, error) {
                        toastMixin.fire({
                            animation: true,
                            title: 'Error While Clear Request',
                            icon: 'error'
                        });
                    }
                });
            }
          });
    }




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

