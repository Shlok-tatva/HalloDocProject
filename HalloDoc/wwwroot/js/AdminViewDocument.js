$('#selectAllCheckbox').change(function () {
    $('.fileCheckbox').prop('checked', $(this).prop('checked'));
});

$('#selectAllCheckboxMobile').change(function () {
    $('.fileCheckbox').prop('checked', $(this).prop('checked'));
});


$('.downloadAll').click(function () {
    if ($('.fileCheckbox:checked').length == 0) {
        showToaster("Please Select At least One File!" , "error");
    } else {
    $('.fileCheckbox:checked').each(function () {
        var filePath = $(this).closest('tr').find('.download-btn').data('file');
        downloadFile(filePath);
    });
    }
});


$('.deleteAll').click(function () {
    if ($('.fileCheckbox:checked').length == 0) {
        showToaster("Please Select At least One File!" , "error");

    }
    else {
    $('.fileCheckbox:checked').each(function () {
        var filePath = $(this).closest('tr').find('.delete-btn').data('file');
        var fileId = $(this).closest('tr').find('.delete-btn').data('fileid');
        deleteFile(filePath, fileId);
    });
    }
});

$('.download-btn').click(function () {
    downloadFile($(this).data('file'));
})

$('.delete-btn').click(function () {
    deleteFile($(this).data('file'), $(this).data('fileid'));
})

$('.sendMail').click(function () {
    getEmailDataAndSend();
});

function downloadFile(filePath) {
    if (filePath != undefined) {

    debugger
    $.ajax({
        url: '/Admin/DownloadFile',
        type: 'GET',
        data: { filePath: filePath },
        xhrFields: {
            responseType: 'blob'
        },
        success: function (response) {

            var url = window.URL.createObjectURL(new Blob([response]));
            var a = document.createElement('a');
            a.href = url;
            a.download = filePath.substring(filePath.lastIndexOf('/') + 1); // Set the download attribute to the file name
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);
        },
        error: function () {

            swal.fire({
                title: "Oops..",
                text: "Oops! Something went wrong while downloading your file. ☹️ It could be that the file was deleted from the server or your browser crashed. Please check your internet connection and try again later.",
                icon: "error",
                showCancelButton: true,
                showConfirmButton: false,
                cancelButtonText: "Okay",
                cancelButtonColor: "#01BBE7",
            })
        }
    });

    }
}
function deleteFile(filePath, fileId) {
    $.ajax({
        url: '/Admin/DeleteFile',
        type: 'GET',
        data: { filePath: filePath, fileId: fileId },
        xhrFields: {
            responseType: 'blob'
        },
        success: function (response) {
            Swal.fire({
                title: "Done",
                text: "Document Delete SuccessFully.. !",
                icon: "success",
                showConfirmButton: false,
                timer: 1000
            }).then(function () {
                location.reload();
            })
        },
        error: function () {

            swal.fire({
                title: "Oops..",
                text: "Oops! Something went wrong while downloading your file. ☹️ It could be that the file was deleted from the server or your browser crashed. Please check your internet connection and try again later.",
                icon: "error",
                showCancelButton: true,
                showConfirmButton: false,
                cancelButtonText: "Okay",
                cancelButtonColor: "#01BBE7",
            })
        }
    });
}


$("#uploadForm").submit(function (e) {
    e.preventDefault();
    var formData = new FormData();

    if ($('#file')[0].files[0] == undefined) {
        Swal.fire({
            icon: "error",
            title: "Oops!☹️ Please Select One File ",
            showConfirmButton: false,
            timer: 1000
        });
        return;
    }

    formData.append('file', $('#file')[0].files[0]);
    formData.append('requestId', $('#requestId').val());
    console.log($('#requestId').val());

    $.ajax({
        url: '/uploadfile',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (reponse) {
            Swal.fire({
                title: "Done",
                text: "Your Document Upload SuccessFully.. !",
                icon: "success",
                showConfirmButton: false,
                timer: 1000
            }).then(function () {
                location.reload();
            })

        },
        error: function () {
            swal.fire({
                title: "Oops..",
                text: "Oops! Something went wrong while Uplaoding your file. ☹️ Please Select One File ",
                icon: "error",
                showCancelButton: true,
                showConfirmButton: false,
                cancelButtonText: "Okay",
                cancelButtonColor: "#01BBE7",
            })
        }
    })

})

function getEmailDataAndSend() {
    var attachmentFilePaths = [];

    // Loop through table rows to find selected files
    $('.fileCheckbox:checked').each(function () {
        var filePath = $(this).closest('tr').find('.download-btn').data('file');
        if (filePath !== '') {
            var trimpath = "wwwroot/" + filePath.substring(2);
            attachmentFilePaths.push(trimpath);
        }
    });

    var request = getRequest($('#requestId').val());
    var toEmail = request.email;
    console.log(toEmail);
    console.log(attachmentFilePaths);
    if (attachmentFilePaths.length == 0) {
        showToaster("Please Select At least One File!" , "error");

        return;
    }
    else {
    sendEmail(toEmail, attachmentFilePaths);
    }
}


function sendEmail(toEmail, attachmentFilePaths) {
    var apiUrl = '/Admin/SendfilesonMail';
    var formdata = new FormData();
    formdata.append('receverEmail', toEmail);
    formdata.append('filePaths', attachmentFilePaths);
    var data = {
        'receverEmail': toEmail,
        'filePaths': attachmentFilePaths
    }

    $.ajax({
        url: apiUrl,
        type: 'POST',
        data: data,
        success: function (response) {
            console.log(response);
            showToaster("Email sent successfully!" , "success");
        },
        error: function (error) {
            showToaster("Failed to send email" , "error");
        }
    });
}
