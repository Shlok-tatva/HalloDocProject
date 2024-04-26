$(document).ready(function () {
    // Function to toggle input fields for editing
    $("#editButton").click(function () {
        $(".togleInput").toggleClass("readonly").prop('readonly', function (i, readonly) {
            return !readonly;
        });
        var buttonText = $(this).text();
        $(this).text(buttonText === "Edit" ? "Save" : "Edit");
    });

    // Function to handle form submission
    $("#updateUser").submit(function (event) {
        event.preventDefault(); // Prevent default form submission
        if ($("#editButton").text() === "Edit") {
            var isValid = validateForm(); // Validate form fields
            if (isValid) {
                swal.fire({
                    title: "Are you sure?",
                    text: "Once saved, your changes will be updated in the database.",
                    icon: "warning",
                    showDenyButton: true,
                    showCancelButton: false,
                    confirmButtonText: "Save Changes",
                    denyButtonText: "Cancel Changes",
                    confirmButtonColor: "#01BBE7",
                    dangerMode: true,
                }).then((result) => {
                    saveChanges(); // If form is valid, save changes
                })
            }
        }
    });

    // Function to validate form fields
    function validateForm() {
        debugger;
        var isValid = true;
        $(".validation-error").remove(); // Remove existing validation error messages
        $(".togleInput").each(function () {
            if (!$(this).val()) {
                $(this).after("<div class='text-danger validation-error'>This field is required.</div>");
                isValid = false;
            }
        });
        return isValid;
    }

    // Function to save changes via AJAX
    function saveChanges() {
        var formData = $("#updateUser").serialize(); // Serialize form data
        $.ajax({
            url: "UpdateUser",
            method: "POST",
            data: formData,
            success: function (response) {
                // Display success message
                Swal.fire("Saved!", "", "success");
            },
            error: function (xhr, status, error) {
                // Display error message
                Swal.fire("Error While Save Changes!", "", "error");
            }
        });
    }
});