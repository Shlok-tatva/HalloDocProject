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

        var initialEmail = $("#email").val();
        var initialPhone = $("#phone").val();

        $('#saveBtn').on('click', function (e) {
            e.preventDefault();

            var newEmail = $("#email").val();
            var newPhone = $("#phone").val();

            // Check if any updates have been made
            if (newEmail === initialEmail && newPhone === initialPhone) {
                Swal.fire({
                    position: "top-end",
                    text: "No changes made",
                    icon: "warning",
                    showConfirmButton: false,
                    toast: true,
                    timer: 1500
                });
                return;
            }
            var emailRegex = /^[\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7}$/;
            if (!emailRegex.test(newEmail)) {
                Swal.fire({
                    position: "top-end",
                    text: "Invalid email format",
                    icon: "error",
                    showConfirmButton: false,
                    toast: true,
                    timer: 1500
                });
                return;
            }

            var phoneRegex = /^\d{10}$/;
            if (!phoneRegex.test(newPhone)) {
                Swal.fire({
                    position: "top-end",
                    text: "Invalid phone number",
                    icon: "error",
                    showConfirmButton: false,
                    toast: true,
                    timer: 1500
                });
                return;
            }

            var formData = new FormData();
            formData.append("requestId", $("#requestId").val());
            formData.append("email", newEmail);
            formData.append("phone", newPhone);

            $.ajax({
                url: '/closeCaseUpdate',
                method: 'POST',
                data: formData,
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
            });
        });

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
                        window.location.href = '/Admin/Dashboard'
                    }

                })
            }
        });
    })


})