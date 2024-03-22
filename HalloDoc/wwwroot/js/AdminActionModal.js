/** For Modal of the request*/
$(document).ready(function () {

    /*Modal Click open the Modal  */
    $(document).on('click', '.open-modal', function () {

        var modalId = $(this).data('modal-id');
        var requestId = $(this).data('request-id');
        var data = getRequest(requestId);
        var patientName = data.firstname + " , " + data.lastname;
        console.log(data);
        debugger
        var patientPhone = data.phonenumber;
        var patientEmail = data.email;
        // Update the modal body to display the requestId
        $('#' + modalId).find('#requestIdassignCase').prop("value", requestId);
        $('#' + modalId).find('#requestIdblockCase').prop("value", requestId);
        $('#' + modalId).find('#requestIdcanleCase').prop("value", requestId);
        $('#' + modalId).find('#requestIdTransferCase').prop("value", requestId);
        $('#' + modalId).find('#requestIdsendAgreement').prop("value", requestId);
        $('#' + modalId).find('#requestIdencounter').prop("value", data.requestid);

        $('#' + modalId).find('#patientName').text(patientName);
        $('#' + modalId).find('#phoneNumberSendAgreement').prop("value", patientPhone);
        $('#' + modalId).find('#emailSendAgreement').prop("value", patientEmail);


        if (modalId == "sendAgreementModal") {
            var requestTypeId = $(this).data('request-type-id');
            var classMap = {
                1: 'patient',
                2: 'family',
                3: 'concierge',
                4: 'business'
            };
            
            var $modal = $('#sendAgreementModal');
            var $circle = $modal.find('.circle');
            var $requestType = $modal.find('#requesttype');

            if (classMap.hasOwnProperty(requestTypeId)) {
                var className = classMap[requestTypeId];
                $circle.removeClass('patient family concierge business').addClass(className);
                $requestType.text(className.charAt(0).toUpperCase() + className.slice(1));
            }
        }

        // Show the modal
        $('#' + modalId).modal('show');


        if (modalId == "clearCaseModal") {
            debugger;
            clearCase(requestId);
        }

    });


    /*Modal Click Close the Modal  */
    $('.close').on('click', function () {
        $('#blockPatientModal').modal('hide');
        $('#cancelCaseModal').modal('hide');
        $('#assignCaseModal').modal('hide');
        $('#transferModal').modal('hide');
        $('#sendAgreementModal').modal('hide');
        $("#encounterModal").modal('hide');
        $("#sendLinktModal").modal('hide');

    })

    /* Block Request Ajax Call */
    $('#blockRequest').validate({
            rules: {
                reasonForBlock: {
                    required: true
                }
            },
            messages: {
                reasonForBlock: {
                    required: "Please provide a reason for blocking."
                }
            },
            errorPlacement: function (error, element) {
                // Custom error placement for Bootstrap floating labels
                error.insertAfter(element.parent());
                // Add text-danger class to error message
                error.addClass('text-danger');
            },
            submitHandler: function (form) {
                var formData = new FormData();
                formData.append('requestId', $('#requestIdblockCase').val());
                formData.append('reason', $('#reasonForBlock').val());
                $.ajax({
                    url: "/BlockPatient",
                    method: "POST",
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        Swal.fire({
                            title: "Done",
                            text: "Patient blocked successfully",
                            icon: "success",
                            showConfirmButton: false,
                            timer: 1000
                        }).then(function () {
                            location.reload();
                        })
                    },
                    error: function (xhr, status, error) {
                        showToaster("Error while blocking patient", "error");
                    }
                });
            }
        });

    /* Cancel Case Ajax Call */
    $('#cancelCase').validate({
            rules: {
                reasonForCancel: {
                    required: true
                }
            },
            messages: {
                reasonForCancel: {
                    required: "Please provide a reason for cancellation."
                }
            },
            errorPlacement: function (error, element) {
                // Custom error placement for Bootstrap floating labels
                error.insertAfter(element.parent());
                // Add text-danger class to error message
                error.addClass('text-danger');
            },
            submitHandler: function (form) {
                var formData = new FormData();
                formData.append('requestId', $('#requestIdcanleCase').val());
                console.log($('#requestIdcanleCase').val());
                formData.append('reason', $('#reasonForCancel').val());
                console.log($('#reasonForCancel').val());
                formData.append('notes', $('#additionalNotes').val());

                $.ajax({
                    url: "CancelCase",
                    method: "POST",
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        Swal.fire({
                            title: "Done",
                            text: "Case canceled successfully",
                            icon: "success",
                            showConfirmButton: false,
                            timer: 1000
                        }).then(function () {
                            location.reload();
                        })
                    },
                    error: function (xhr, status, error) {
                        showToaster("Error while canceling case", "error");
                    }
                });
            }
        });

    /*Assign Case Ajax Call */
    $('#assignCaseModal').validate({
        rules: {
            selectregion: {
                required: true
            },
            physicianSelect: {
                required: true
            }
        },
        messages: {
            selectregion: {
                required: "Please select a Region"
            },
            physicianSelect: {
                required: "Please select a physician"
            }
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
            error.addClass('text-danger');
        },
        submitHandler: function (form) {
            debugger
            var formData = new FormData();
            formData.append('requestId', $('#requestIdassignCase').val());
            formData.append('physicianId', $('#physicianSelect').val());
            $.ajax({
                url: "AssignCase",
                method: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    Swal.fire({
                        title: "Done",
                        text: "Case assigned to physician successfully",
                        icon: "success",
                        showConfirmButton: false,
                        timer: 1000
                    }).then(function () {
                        location.reload();
                    });
                },
                error: function (xhr, status, error) {
                    showToaster("Error while assigning case to physician", "error");
                }
            });
        }
    });

    /* Transfer Case Ajax Call */
    $('#transferModal').validate({
            rules: {
                selectregionTransfer: {
                    required: true
                },
                Description: {
                    required: true
                }
            },
            messages: {
                selectregionTransfer: {
                    required: "Please select one region."
                },
                Description: {
                    required: "Please provide a description."
                }
            },
            errorPlacement: function (error, element) {
                error.insertAfter(element.parent());
                error.addClass('text-danger');
            },
            submitHandler: function (form) {
                var formData = new FormData();
                formData.append('requestId', $('#requestIdTransferCase').val());
                formData.append('physicianId', $('#physicianSelectTransfer').val());
                formData.append('note', $('#Description').val());
                $.ajax({
                    url: "/TransferCase",
                    method: "POST",
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        Swal.fire({
                            title: "Done",
                            text: "Case transferred to physician successfully",
                            icon: "success",
                            showConfirmButton: false,
                            timer: 1500
                        }).then(function () {
                            location.reload();
                        })
                    },
                    error: function (xhr, status, error) {
                        showToaster("Error while transferring case to physician", "error");
                    }
                });
            }
        });

    /*Send Agreement Ajax Call */

    $('#sendAgreement').validate({
        rules: {
            phoneNumberSendAgreement: {
                required: true,
                digits: true
            },
            emailSendAgreement: {
                required: true,
                email: true
            }
        },
        messages: {
            phoneNumberSendAgreement: {
                required: "Please enter a phone number.",
                digits: "Please enter a valid phone number."
            },
            emailSendAgreement: {
                required: "Please enter an email address.",
                email: "Please enter a valid email address."
            }
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
            error.addClass('text-danger');
        },
        submitHandler: function (form) {
            var formData = new FormData();
            formData.append('requestId', $('#requestIdsendAgreement').val());
            formData.append('phoneNumber', $('#phoneNumberSendAgreement').val());
            formData.append('email', $('#emailSendAgreement').val());

            $.ajax({
                url: "/SendAgreement",
                method: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    Swal.fire({
                        title: "Done",
                        text: "Agreement sent to patient successfully",
                        icon: "success",
                        showConfirmButton: false,
                        timer: 1000
                    }).then(function () {
                        location.reload();
                    })
                },
                error: function (xhr, status, error) {
                    showToaster("Error while sending agreement to patient", "error");
                }
            });
        }
    });


    /* Clear Case Function*/
    function clearCase(requestId) {
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
            cancelButtonText: "Cancel",
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
                        showToaster("Error While Clear Request" , "error");
                    }
                });
            }
        });
    }



});

