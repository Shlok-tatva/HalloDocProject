$(document).ready(function () {
    $(".edit-button").click(function () {
        var section = $(this).data("section");
        debugger
        $(this).hide();
        $(this).siblings(".save-button").show();
        $(`input[data-section="${section}"]`).removeAttr("readonly");
        $(`input[data-section="${section}"]`).removeClass("readonly");
        $("#regionsearch").removeAttr("disabled");
        $("#regionsearch").removeClass("readonly");

    });

    $(".save-button").click(function () {
        var section = $(this).data("section");
        $(this).siblings(".edit-button").show();
        $(this).hide();
        $(`input[data-section="${section}"]`).attr("readonly", "readonly");
        $(`input[data-section="${section}"]`).addClass("readonly");
        $("#regionsearch").attr("disabled", "disabled");
        $("#regionsearch").addClass("readonly");

    });

    $("#regionsearch").on("change", function () {
        $("#regionid").val($(this).val());
    })

    $('#photofile').change(function () {
        $('#photoPreview').hide(); 
    });

    $('#signature').change(function () {
        $('#signaturePreview').hide(); 
    });

    var downloadLinks = document.querySelectorAll('.download-link');

    // Loop through each download link
    downloadLinks.forEach(function (link) {
        link.addEventListener('click', function (event) {
            event.preventDefault();
            var fileName = this.getAttribute('href').split('/').pop();
            var fileInput = this.closest('.document').querySelector('.fileInput');
            fileInput.nextElementSibling.textContent = fileName;
        });
    });


});