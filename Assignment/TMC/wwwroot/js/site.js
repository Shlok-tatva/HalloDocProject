// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).on('click', '.open-modal', function () {

    debugger;

    var modalId = $(this).data('modal-id');

    if (modalId == "edittask") {
        var taskId = $(this).data("task-id");
        console.log(taskId);
        var taskData = getTaskData(taskId);
        $("#taskName").val(taskData.taskName);
        $("#assignee").val(taskData.assignee);
        $("#Discription").val(taskData.discription);
        $("#dueDate").val(taskData.duedate.split('T')[0]);
        $("#city").val(taskData.city);
        $("#category").val(taskData.category);
        $("#taskId").val(taskData.id);
        $("#createTask").prop("action", "/Task/editTask");
        console.log(taskData);
    }
    else {
        $("#taskName").val("");
        $("#assignee").val("");
        $("#Discription").val("");
        $("#dueDate").val("");
        $("#city").val("");
        $("#category").val("");
        $("#taskId").val(-1);
        $("#createTask").prop("action", "/Task/createTask");

    }

    $('#createTaskModal').modal('show');
});


$(".close").on("click", function () {
    $('#createTaskModal').modal('hide');
})


function getTaskData(taskId) {
    var taskdata;
    $.ajax({
        url: '/Task/GetTask',
        type: "GET",
        data: { id: taskId },
        async: false, // synchronous request to ensure data is returned before proceding 
        success: function (data) {
            taskdata = data;
        },
        error: function (e) {
            alert("Error while fetching Data");
        }
    });
    return taskdata;
}

$(document).ready(function () {
    $('#taskdata').DataTable();
});

(function () {
    'use strict'

    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    var forms = document.querySelectorAll('.needs-validation')

    // Loop over them and prevent submission
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault()
                    event.stopPropagation()
                }

                form.classList.add('was-validated')
            }, false)
        })
})()





function showToaster(msg, icon) {
    $(document).ready(function () {
        toastMixin.fire({
            animation: true,
            title: `${msg}`,
            icon: `${icon}`
        });
    })

    var toastMixin = Swal.mixin({
        toast: true,
        icon: 'success',
        title: 'General Title',
        animation: false,
        position: 'top-right',
        showConfirmButton: false,
        timer: 2000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });
}

$(".delete-Task").unbind("click").on("click", function () {
    debugger
    let taskId = +$(this).data("taskid");
  
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            cancelButton: "btn btn-success bg-opacity-75 mx-2 btn-lg text-white my-2 mb-2",
            confirmButton: "btn btn-outline-danger btn-lg hover_white"
        },
        buttonsStyling: false
    });

    swalWithBootstrapButtons.fire({
        text: `Are you sure you want to Delete this task`,
        showCancelButton: true,
        confirmButtonText: "Delete",
        cancelButtonText: "Cancel",
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `Task/DeleteTask?id=${taskId}`,
                method: "POST",
                success: function (response) {
                    Swal.fire({
                        title: "Deleted",
                        text: "Task Delete Successfully",
                        icon: "success",
                        timer: 1500,
                        showConfirmButton: false,
                    }).then(function () {
                        location.reload();
                    })
                },
                error: function (xhr, status, error) {
                    showToaster("Error While Delete Task", "error");
                }
            });
        }
    });
})