if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(function (position) {
        var latitude = position.coords.latitude;
        var longitude = position.coords.longitude;
        var address = getAddress(latitude, longitude);

        $.ajax({
            url: '/Provider/UpdateproviderLocation',
            type: "POST",
            data: {
                latitude: latitude,
                longitude: longitude,
                address: address
            },
            success: function (response) {

            },
            error: function (xhr, status, error) {
                console.error("Error updating location:", error);
            }
        });

    });
}

function getAddress(latitude, longitude) {
    var locationAddress;
    $.ajax({
        url: `https://api.tomtom.com/search/2/reverseGeocode/${latitude},${longitude}.json?key=ZqMpgY5pcMO91rJZYniixt54YVSFR0aw`,
        type: "GET",
        async: false,
        success: function (data) {
            if (data.addresses && data.addresses.length > 0) {
                var address = data.addresses[0].address.freeformAddress;
                console.log(address);
                locationAddress = address;
            } else {
                console.log("Address not found");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching address:", error);
        }
    });
    return locationAddress;
}