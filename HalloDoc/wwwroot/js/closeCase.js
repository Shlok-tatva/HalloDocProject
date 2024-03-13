$(document).ready(function () {
    $('#editBtn').on('click', function (e) {
        e.preventDefault();
        $('.toggleInput').removeClass('notallowed');
        $('.toggleInput').removeAttr('readonly');
        $('.closeCaseBtns').addClass('d-none');
        $('.editBtns').removeClass('d-none');

    })
    $('#cancelBtn').on('click', function (e) {
        e.preventDefault();
        $('.closeCaseBtns').removeClass('d-none');
        $('.toggleInput').attr('readonly', true);
        $('.editBtns').addClass('d-none');
        $('.toggleInput').addClass('notallowed');
    })

    $('#saveBtn').on('click', function (e) {
        debugger
        e.preventDefault();
        var formdata = new FormData();
        formdata.append("requestId", $("#requestId").val());
        formdata.append("email", $("#email").val());
        formdata.append("phone", $("#phone").val());

        $.ajax({
            url: '/closeCaseUpdate',
            method: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            success: function () {
                $('.editBtns').addClass('d-none');
                $('.closeCaseBtns').removeClass('d-none');
                $('.toggleInput').addClass('notallowed');
            },
            error: function () {
                Swal.fire({
                    position: "top-end",
                    text: "Error While updating data",
                    icon: "error",
                    showConfirmButton: false,
                    toast: true,
                    timer: 1500
                });
            }
        })
    })

    $('#closeCaseBtn').on('click', function (e) {
        e.preventDefault();
        var requestId = $('#requestId').val();
        Swal.fire({
            title: "Are you sure?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#01BBE7",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/HandleCloseCase',
                    method: 'POST',
                    data: { requestId: requestId },
                    success: function () {
                        window.location.href = '/admin'
                    }

                })
            }
        });
    })


})