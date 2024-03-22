$(document).ready(function () {


    function getmenu() {
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
    }
    


    $("#createRole").on("click" , function(){
        var roleName = $('#roleName').val(); 
        var accountType = $('#AccountType').val(); 
        var selectedMenu = [];
        $('input[type="checkbox"]:checked').each(function () {
            selectedMenu.push(+$(this).attr('id'));
        });
        console.log(selectedMenu);
        console.log(roleName);
        console.log(accountType);

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

            },
            error: function () {
                showToaster("Error while creating role", "error");
            }
        });


    })


    let roleId = +$("#roleId").val();
    if (roleId != undefined) {

        debugger
    $.ajax({
        url: '/Admin/getRole',
        type: "GET",
        data: { roleId: roleId },
        success: function (data) {
            console.log(data);
            $('#roleName').val(data.name);
            $('#AccountType').val(data.accounttype);
            var menuIds = data.menuIds.split(',').map(Number); 
            menuIds.forEach(function (menuId) {
                $('#' + menuId).prop('checked', true);
            });
        },
        error: function (error) {
            debugger
            showToaster("Error while fetching menu", "error");
        }
    })

    }

})