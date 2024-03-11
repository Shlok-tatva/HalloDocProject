$(document).ready(function () {
    $.ajax({
        url: '/admin/getProfessions',
        type: 'GET',
        success: function (response) {
            var options = '<option value="" selected disabled>Select Profession</option>';
            $.each(response, function (index, profession) {
                options += '<option value="' + profession.healthprofessionalid + '">' + profession.professionname + '</option>';
            });
            $('#ProfessionSelect').html(options);
        }
    });
});


// AJAX request to fetch businesses for selected profession
$('#ProfessionSelect').change(function () {
    var professionId = $(this).val();
    $.ajax({
        url: '/admin/getBusinesses',
        data: { professionId },
        type: 'GET',
        success: function (response) {
            var options = '<option value="">Select Business</option>';
            $.each(response, function (index, business) {
                options += '<option value="' + business.vendorid + '">' + business.vendorname + '</option>';
            });
            $('#BusinessSelect').html(options);
        }
    });
});

$('#BusinessSelect').change(function () {
    var Vendorid = $(this).val();
    $.ajax({
        url: '/admin/getBusinessDetails/',
        data: { Vendorid },
        type: 'GET',
        success: function (response) {
            console.log(response)
            $('#BusinessContactInput').val(response.businesscontact);
            $('#EmailInput').val(response.email);
            $('#FaxNumberInput').val(response.faxnumber);
        }
    });
});