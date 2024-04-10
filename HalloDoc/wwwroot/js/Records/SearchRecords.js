
$(document).ready(function () {
    $("#DeleteSuccess").on("click", function () {
        var requestid = $("#DelRequestId").val();
        deletePatientRequest(requestid);
    });

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
           showToaster("Please Enter some Search Fields" , "error");
        } else {
            fetchSearchRecords(email, fromdos, phone, todos, patient, provider, status, type);
        }
    });

    $("#ClearFilter").on("click", function () {
        clearFiltersAndFetchRecords();
    });

    fetchSearchRecords("", "", "", "", "", "", 0, 0);
});

function deletePatientRequest(requestid) {
    $.ajax({
        method: "POST",
        url: "DeletePatientRequest",
        data: { requestid: requestid },
        success: function (response) {
            if (response.code == 401) {
                location.reload();
            } else {
                if (response == true) {
                    $("#DeleteConfirmationModal").modal('hide');
                    showToaster("Request Deleted Successfully" , "success");
                    fetchSearchRecords();
                } else {
                    showToaster("Error deleting Request" , "error");
                }
            }
        },
        error: function () {
            showToaster("Failed to delete request." , "error");
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
                setupPagination(10);
            }
        },
        error: function () {
            showToaster("Failed to fetch records." , "error");
        }
    });
}

function clearFiltersAndFetchRecords() {
    $("#Email, #Phone, #PatientName, #ProviderName").val("");
    $("#FromDoS, #ToDoS").val("");
    $("#RequestStatus, #RequestType").val(0);
    fetchSearchRecords("", "", "", "", "", "", 0, 0);
}