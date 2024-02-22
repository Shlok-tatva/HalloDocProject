$(document).ready(function () {
    function reloadDataTable(statusID) {
        $.ajax({
            url: '/Admin/GetRequest',
            type: "GET",
            data: { status_id: statusID },
            success: function (data) {
                $('#request-table tbody').empty();
                $('#request-table thead').empty();


                if (data.length > 0) {
                    var headers = Object.keys(data[0]);
                    var headerMapping = {
                        "requestId": "Request ID",
                        "patientName": "Name",
                        "patientEmail": "Email",
                        "patientPhoneNumber": "Phone Number",
                        "status": "Status",
                        "dateOfBirth": "DateOfBirth",
                        "requesterName": "Requester",
                        "requestedDate": "Requested Date",
                        "requesterPhoneNumber": " Requester Phone Number",
                        "address": "Address",
                        "notes": "Notes",
                        "menuOptions": "Action"
                    };

                    var headerRow = $('<tr>');
                    headers.forEach(function (header) {
                        if (statusID === 1 && header === "phydicanName") {
                            return;
                        }
                        if (["requestTyepid", "status", "requesterPhoneNumber", "requesterEmail"].includes(header)) {
                            return;
                        }
                        var columnName = headerMapping[header] || header;
                        headerRow.append('<th>' + columnName + '</th>');
                    });
                    $('#request-table thead').append(headerRow);
                }

                data.forEach(function (request) {
                    var newRow = $('<tr>').attr('data-request-type-id', request.requestTyepid);
                    for (var key in request) {
                        if (!["menuOptions", "requestTyepid", "status", "requesterPhoneNumber", "requesterEmail"].includes(key)) {
                            if (key === "patientPhoneNumber") {
                                var phoneNumbers = '<button class="btn btn-outline-light my-1"> <i class="bi bi-telephone"></i> ' + request.patientPhoneNumber + '</button>' + (request.requesterPhoneNumber ? ' <br />' + '<button class="btn btn-outline-light my-1"> <i class="bi bi-telephone"></i> ' + request.requesterPhoneNumber + '</button><br />' : '');
                                if ([2, 3, 4].includes(request.requestTyepid)) {
                                    phoneNumbers += "(" + ["familyfriend", "concierge", "business"][request.requestTyepid - 2] + ")";
                                } else {
                                    phoneNumbers += "(Patient)";
                                }
                                newRow.append('<td class="' + key + '">' + phoneNumbers + '</td>');
                            } else if (key === 'patientEmail') {
                                var emailCell = $('<td>');
                                var dropdownMenu = $('<ul class="dropdown-menu" aria-labelledby="dropdownMenuButton">');
                                dropdownMenu.append('<li><a class="dropdown-item menu-option" href="mailto:' + request.patientEmail + '">' + request.patientEmail + '</a></li>');
                                dropdownMenu.append('<li><a class="dropdown-item menu-option" href="mailto:' + request.requesterEmail + '">' + request.requesterEmail + '</a></li>');
                                var dropdownButton = $('<div class="dropdown">');
                                dropdownButton.append('<button class="btn btn-outline-light dropdown-toggle" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-expanded="false"><i class="bi bi-envelope"></i></button>');
                                dropdownButton.append(dropdownMenu);
                                emailCell.append(dropdownButton);
                                newRow.append(emailCell);
                            } else {
                                newRow.append('<td class="' + key + '">' + request[key] + '</td>');
                            }
                        }
                    }
                    var actionsCell = $('<td>');
                    var dropdownMenu = $('<ul class="dropdown-menu" aria-labelledby="dropdownMenuButton">');

                    const menuOptionEnumMapping = {
                        0: 'Assign Case',
                        1: "Cancel Case",
                        2: "View Case",
                        3: "View Notes",
                        4: "Block Patient",
                        5: "View Upload",
                        6: "Transfer",
                        7: "Clear Case",
                        8: "Send Agreement",
                        9: "Orders",
                        10: "Doctors Note",
                        11: "Encounter",
                        12: "Close Case",
                    };

                    // Object mapping menu options to image URLs
                    const menuOptionImageMapping = {
                        'Assign Case': 'assignCase.png',
                        'Cancel Case': 'cancelCase.png',
                        'View Case': 'viewCase.png',
                        'View Notes': 'viewNotes.png',
                        'Block Patient': 'blockPatient.png',
                        'View Upload': 'viewUpload.png',
                        'Transfer': 'transferCase.png',
                        'Clear Case': 'clearCase.png',
                        'Send Agreement': 'sendAgreement.png',
                        'Orders': 'orders.png',
                        'Doctors Note': 'doctors-note.png',
                        'Encounter': 'encounter.png',
                        'Close Case': 'closeCase.png',
                    };

                    function mapNumberToEnumName(number) {
                        return menuOptionEnumMapping[number] || "Unknown";
                    }

                    function toCamelCase(text) {
                        return text.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, function (match, index) {
                            if (+match === 0) return ""; // Remove spaces if it's a zero
                            return index === 0 ? match.toLowerCase() : match.toUpperCase();
                        });
                    }

                    request.menuOptions.forEach(function (option) {
                        var enumName = mapNumberToEnumName(option);
                        var link = toCamelCase(enumName);
                        var imageUrl = menuOptionImageMapping[enumName];
                        dropdownMenu.append('<li><a class="dropdown-item menu-option" href="/admin/' + link + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '">' + '<image src="./images/' + imageUrl + '" class="menu-icon" />' + enumName + '</a></li>');
                    });

                    var dropdownButton = $('<div class="dropdown">');
                    dropdownButton.append('<button class="btn btn-outline-light dropdown-toggle" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>');
                    dropdownButton.append(dropdownMenu);
                    actionsCell.append(dropdownButton);
                    newRow.append(actionsCell);
                    $('#request-table tbody').append(newRow);

                });

                $('#request-table').DataTable({
                    paging: true,
                    searching: false,
                    ordering: false,
                    retrieve: true,
                    info: true,
                    "pageLength": 10,
                    "language": {
                        "paginate": {
                            "next": "&#8594;",
                            "previous": "&#8592;"
                        }
                    }
                });
                $('#request-table_length').css('display', 'none');


                $('#request-table tbody tr').each(function () {
                    var requestTypeId = $(this).data('request-type-id');
                    //var colors = ['#5fbc61', '#ffb153', '#fb91ca', '#007fc6e8'];
                    var colors = ['forestgreen', 'darkorange', 'deeppink', 'dodgerblue'];
                    if (requestTypeId >= 1 && requestTypeId <= 4) {
                        $(this).css('background-color', colors[requestTypeId - 1]);
                    }
                });

            },

            error: function () {
                alert("Error while fetching Data");
            }

        });
    }

    reloadDataTable(1);

    $('.AdminState').click(function () {
        var statusId = $(this).data("status-id");
        reloadDataTable(statusId);
    });

    $.ajax({
        url: 'Admin/getRequestCountPerStatusId',
        type: "GET",
        success: function (data) {
            console.log(data);
            $('#newCount').text(data[1]);
            $('#pandingCount').text(data[2]);
            $('#activeCount').text(data[3]);
            $('#concludeCount').text(data[4]);
            $('#closeCount').text(data[5]);
            $('#unpaidCount').text(data[6]);

        },
        error: function () {
            alert("Error while fetching count");
        }
    });


    $('.btn-all').click(function () {
        showSpinnerAndFilter(null);
    });
    $('.btn-patient').click(function () {

        showSpinnerAndFilter(1);
    });

    $('.btn-family').click(function () {
        showSpinnerAndFilter(2);
    });

    $('.btn-concierge').click(function () {
        showSpinnerAndFilter(3);
    });

    $('.btn-business').click(function () {
        showSpinnerAndFilter(4);
    });




    $('#NewState').click(function () {
        $('.click').not(this).removeClass('click');
        $('#requestState').text('(New)');
        $(this).toggleClass('click');
    });

    $('#ActiveState').click(function () {
        $('.click').not(this).removeClass('click');
        $('#requestState').text('(ACTIVE)');
        $(this).toggleClass('click');
    });
   
    $('#PendingState').click(function () {
        $('.click').not(this).removeClass('click');
        $('#requestState').text('(PANDING)');
        $(this).toggleClass('click');
    });

    $('#ConcludeState').click(function () {
        $('.click').not(this).removeClass('click');
        $('#requestState').text('(CONCLUDE)');
        $(this).toggleClass('click');
    });

    $('#ToCloseState').click(function () {
        $('.click').not(this).removeClass('click');
        $('#requestState').text('(TO CLOSE)');
        $(this).toggleClass('click');
    });

    $('#UnpaidState').click(function () {
        $('.click').not(this).removeClass('click');
        $('#requestState').text('(UNPAID)');
        $(this).toggleClass('click');
    });






    function showSpinnerAndFilter(requestTypeId) {
        $('#spinner').show();
        setTimeout(function () {
            $('#spinner').hide();
            filterDataTable(requestTypeId);
        }, 500);
    }

    function filterDataTable(requestTypeId) {
        $('#request-table tbody tr').each(function () {
            var rowRequestTypeId = $(this).data('request-type-id');
            if (requestTypeId === null || rowRequestTypeId === requestTypeId) {
                $(this).show(); // Show row if it matches the requestTypeId or if requestTypeId is null (show all)
            } else {
                $(this).hide();
            }
        });
    }

});




