$(document).ready(function () {
    var adminbtn = $("#adminbtn");
    var physicianbtn = $("#physicianbtn");
    let userData = $(".user-data");

    adminbtn.hide();
    physicianbtn.hide();

 
    $('#UserSearch').on('change', function () {
        var userType = $(this).val();
        var userAccountType = $('#UserSearch option:selected').text(); // Dynamic retrieval of user account type

        if (userType == 1) {
            adminbtn.show();
            physicianbtn.hide();
        } else if (userType == 2) {
            physicianbtn.show();
            adminbtn.hide();
        } else {
            adminbtn.hide();
            physicianbtn.hide();
        }

        userData.each(function () {
            var $this = $(this);
            var accountType = $this.data("accounttype");

            if (accountType === userAccountType || userType == 0) {
                $this.show();
            } else {
                $this.hide();
            }
        });

    });
});
