$(document).ready(function () {

    $(document).ready(function () {


        $(".open-modal").on("click", function () {
            let roleId = +$(this).data("roleid");
            let rolename = $(this).data("rolename");

            const swalWithBootstrapButtons = Swal.mixin({
                customClass: {
                    confirmButton: "btn btn-info mx-2 btn-lg text-white my-2 mb-2",
                    cancelButton: "btn btn-outline-info btn-lg hover_white"
                },
                buttonsStyling: false
            });

            swalWithBootstrapButtons.fire({
                title: "Confirmation of Delete Role",
                text: `Are you sure you want to Delete this Role :- ${rolename} ?`,
                iconHtml: "<div class='warning_icon'><i class='bi bi-exclamation-circle-fill'></i></div>",
                showCancelButton: true,
                confirmButtonText: "Delete",
                cancelButtonText: "Cancel",
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: "DeleteRole",
                        method: "POST",
                        data: { roleId },
                        success: function (response) {
                            Swal.fire({
                                title: "Deleted",
                                text: "Role Delete Successfully",
                                icon: "success",
                                timer: 1500,
                                showConfirmButton: false,
                            }).then(function () {
                                location.reload();
                            })
                        },
                        error: function (xhr, status, error) {
                            showToaster("Error While Delete Role", "error");
                        }
                    });
                }
            });
        })


    })

    $("#AccountType").on("change", function () {
            var roleId = $(this).val();

            $.ajax({
                url: '/Admin/GetMenuByRole',
                type: "GET",
                data: { roleId: roleId },
                success: function (data) {
                    console.log(data);
                    $('.menu-container').empty();
                    $.each(data, function (index, item) {
                        $('.menu-container').append(`
                            <div class="d-flex align-items-center m-1">
                                <input type="checkbox" id="${item.menuid}" name="${item.name}"/>
                                <span>${item.name}</span>
                            </div>
                        `);
                    });

                },
                error: function () {
                    showToaster("Error while fetching menu", "error");
                }
            });

        })

    $("#createRole").on("submit", function (e) {
        e.preventDefault();
        var roleName = $('#roleName').val(); 
        var accountType = +$('#AccountType').val(); 
        var selectedMenu = [];
        $('input[type="checkbox"]:checked').each(function () {
            selectedMenu.push(+$(this).attr('id'));
        });

        $.ajax({
            url: '/Admin/CreateRole',
            type: "POST",
            data: {
                roleName: roleName,
                accountType: accountType,
                selectedMenu: selectedMenu
            },
            success: function (response) {
                console.log(response);
                showToaster("Role Created Successfully !", "success");
                window.location.href = "/Admin/Access";

            },
            error: function () {
                showToaster("Error while creating role", "error");
            }
        });


    })    



})

$(document).on("submit", "#EditRole", function (e) {
    e.preventDefault();
    debugger
    var roleId = $('#roleId').val();
    var roleName = $('#roleName').val();
    var accountType = +$('#AccountType').val();
    var selectedMenu = [];
    $('input[type="checkbox"]:checked').each(function () {
        selectedMenu.push(+$(this).attr('id'));
    });

    $.ajax({
        url: '/Admin/EditRole',
        type: "POST",
        data: {
            roleId: roleId,
            roleName: roleName,
            accountType: accountType,
            selectedMenu: selectedMenu
        },
        success: function (response) {
            showToaster("Role Edited Successfully !", "success");
        },
        error: function () {
            showToaster("Error while editing role", "error");
        }
    });
});