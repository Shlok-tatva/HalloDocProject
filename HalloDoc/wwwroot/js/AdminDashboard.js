$(document).ready(function () {
    function reloadDataTable(statusID) {
        $.ajax({
            url: '/Admin/GetRequest',
            type: "GET",
            data: { status_id: statusID },
            success: function (data) {
                // Clear existing table body and header
                $('#request-table tbody').empty();
                $('#request-table thead').empty();

                if (data.length > 0) {
                    // Create table headers based on the first object in data
                    var headers = Object.keys(data[0]);
                    var headerRow = $('<tr>');
                    headers.forEach(function (header) {

                        headerRow.append('<th>' + header + '</th>');
                    });
                    $('#request-table thead').append(headerRow);
                }

                // Iterate over data and create table rows dynamically
                data.forEach(function (request) {
                    var newRow = $('<tr>');

                    for (var key in request) {
                        if (key != "menuOptions") {
                        newRow.append('<td>' + request[key] + '</td>');
                        }
                    }
                    var actionsCell = $('<td>');
                    request.menuOptions.forEach(function (option) {
                        actionsCell.append('<button class="menu-option" data-option="' + option.toString() + '" data-request-id="' + request.requestId + '">' + option.toString() + '</button>');
                    });
                    newRow.append(actionsCell);
                    $('#request-table tbody').append(newRow);
                });

                $('#request-table').DataTable({
                    paging: false,
                    searching: false,
                    retrieve: true,
                    "rowCallback": function (row, data) {
                        console.log(data);
                        if (data[1] === '') { // change the color of the row if the requestor is 'familyfriend'
                            $(row).css('background-color', 'yellow');
                        } else if (data[2] === 'Dabhi') { // change the color of the row if the requestor is 'concierge'
                            $(row).css('background-color', 'green');
                        }
                    },
                    "pageLength": 20
                });
            },
            error: function () {
                alert("error while fetching Data");
            }
        });
    }
    // Initial call to load data
    reloadDataTable(1);
    // Event handler for status click
    $('.AdminState').click(function () {
        var statusId = $(this).data("status-id");
        reloadDataTable(statusId);
    });



    // Update Count in Card
    $.ajax({
        url: 'Admin/getRequestCountPerStatusId',
        type: "GET",
        success: function (data) {
            $('#newCount').text(data[1]);
            $('#pandingCount').text(data[2]);
            $('#activeCount').text(data[3]);
            $('#concludeCount').text(data[4]);
            $('#closeCount').text(data[5]);
            $('#unpaidCount').text(data[6]);

        },
        error: function (e) {

        }
    })

});