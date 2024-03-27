$(document).ready(function () {
  
    function updateUsername() {
        var firstName = $('#FirstName').val().trim();
        var lastName = $('#LastName').val().trim();
        var baseUsername = 'AD.' + (lastName.length > 0 ? lastName.toLowerCase() + '.' : '') + (firstName.length > 0 ? firstName.charAt(0).toLowerCase() : '');

        function checkUsernameAvailability(username, count) {
            $.ajax({
                url: '/admin/CheckUsernameAvailability',
                type: 'POST',
                data: { username: username },
                success: function (response) {
                    if (response.available) {
                        $('#UserName').val(username);
                    } else {
                        checkUsernameAvailability(baseUsername + count, count + 1);
                    }
                },
                error: function () {
                    console.error('Error checking username availability.');
                }
            });
        }

        checkUsernameAvailability(baseUsername, 1);
    }

    $('#FirstName, #LastName').on('input', function () {
        updateUsername();
    });


    updateUsername();
});
