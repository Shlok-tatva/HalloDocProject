
$(document).ready(function () {
    setupPaginationBasedOnDevice();


    $(".deleteSearchRecords").on("click", function () {
        debugger
        var requestid = $(this).data("requestid");

        const swalWithBootstrapButtons = Swal.mixin({
            customClass: {
                confirmButton: "btn btn-info mx-2 btn-lg text-white my-2 mb-2",
                cancelButton: "btn btn-outline-info btn-lg hover_white"
            },
            buttonsStyling: false
        });

        swalWithBootstrapButtons.fire({
            title: "Confirmation of Delete Request",
            text: `Are you sure you want to Delete this request ${requestid}`,
            iconHtml: "<div class='warning_icon'><i class='bi bi-exclamation-circle-fill'></i></div>",
            showCancelButton: true,
            confirmButtonText: "Delete",
            cancelButtonText: "Cancel",
        }).then((result) => {
            if (result.isConfirmed) {
                deletePatientRequest(requestid);
            }
        });

    })

    $("#SearchFilter").on("click", function () {
        var email = $("#Email").val();
        var fromdos = $("#FromDoS").val();
        var phone = $("#Phone").val();
        var todos = $("#ToDoS").val();
        var patient = $("#PatientName").val().toLowerCase();
        var provider = $("#ProviderName").val().toLowerCase();
        var status = $("#RequestStatus").val();
        var type = $("#RequestType").val();

        if (!email && !fromdos && !phone && !todos && !patient && !provider && status == 0 && type == 0) {
            showToaster("Please Enter some Search Fields", "error");
        } else {
            fetchSearchRecords(email, fromdos, phone, todos, patient, provider, status, type);
        }
    });

    $("#ClearFilter").on("click", function () {
        clearFiltersAndFetchRecords();
    });

});

function deletePatientRequest(requestid) {
    $.ajax({
        method: "POST",
        url: "DeletePatientRequest",
        data: { requestid: requestid },
        success: function () {
            showToaster("Request Deleted Successfully", "success");
            fetchSearchRecords();
        },
        error: function () {
            showToaster("Failed to delete request.", "error");
        }
    });
}

function fetchSearchRecords(email, fromdos, phone, todos, patient, provider, status, type) {
    $.ajax({
        method: "GET",
        url: "GetSearchRecords",
        data: {
            Email: email,
            FromDoS: fromdos,
            Phone: phone,
            Patient: patient,
            Provider: provider,
            RequestStatus: status,
            RequestType: type,
            ToDoS: todos
        },
        success: function (response) {
            if (response.code == 401) {
                location.reload();
            } else {
                console.log("Function Success");
                $('#SearchRecords').html(response);
                setupPaginationBasedOnDevice();
            }
        },
        error: function () {
            showToaster("Failed to fetch records.", "error");
        }
    });
}

function clearFiltersAndFetchRecords() {
    $("#Email, #Phone, #PatientName, #ProviderName").val("");
    $("#FromDoS, #ToDoS").val("");
    $("#RequestStatus, #RequestType").val(0);
    fetchSearchRecords("", "", "", "", "", "", 0, 0);
}