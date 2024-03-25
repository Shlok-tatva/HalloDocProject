$(document).ready(function () {

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