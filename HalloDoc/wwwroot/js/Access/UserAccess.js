$(document).ready(function () {

    var adminbtn = $("#adminbtn");
    var physicianbtn = $("#physicianbtn");

    adminbtn.hide();
    physicianbtn.hide();

    $('#UserSearch').on('change', function () {
        if ($(this).val() == 1) {
            adminbtn.show();
            physicianbtn.hide();
        }
        else if ($(this).val() == 2) {
            physicianbtn.show();
            adminbtn.hide();
        }
        else {
            adminbtn.hide();
            physicianbtn.hide();
        }
    })
})