
$(document).ready(function () {
        
    $("#btnGeneratePDF").click(function () {
        var requestId = $("#requestIdencounter").val();
        window.location.href = "/admin/GeneratePDF?requestId=" + requestId;
    })

    });
