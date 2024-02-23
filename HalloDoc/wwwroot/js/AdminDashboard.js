
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

                        if (["requestTyepid", "status", "requesterPhoneNumber", "requesterEmail" , "requestId"].includes(header)) {
                            return;
                        }

                        var columnName = headerMapping[header] || header;

                        headerRow.append('<th class="py-4">' + columnName + '</th>');

                    });
                    $('#request-table thead').append(headerRow);
                }

                data.forEach(function (request) {
                    var newRow = $('<tr>').attr('data-request-type-id', request.requestTyepid);

                    for (var key in request) {
                        if (!["menuOptions", "requestTyepid", "status", "requesterPhoneNumber", "requesterEmail" , "requestId"].includes(key)) {
                            if (key === "patientPhoneNumber") {
                                var phoneNumbers = '<button class="btn btn-sm  btn-outline-light my-1"> <i class="bi bi-telephone"></i> ' + request.patientPhoneNumber + '</button>' + (request.requesterPhoneNumber && request.patientPhoneNumber != request.requesterPhoneNumber ? ' <br />' + '<button class="btn btn-sm  btn-outline-light my-1"> <i class="bi bi-telephone"></i> ' + request.requesterPhoneNumber + '</button><br />' : '');
                                if ([2, 3, 4].includes(request.requestTyepid)) {
                                    phoneNumbers += "(" + ["familyfriend", "concierge", "business"][request.requestTyepid - 2] + ")";
                                } else {
                                    phoneNumbers += " <br/>(Patient)";
                                }
                                newRow.append('<td class="' + key + '">' + phoneNumbers + '</td>');
                            } else if (key === 'patientEmail') {
                                var emailCell = $('<td class="scale-1">');
                                var dropdownMenu = $('<ul class="dropdown-menu" aria-labelledby="dropdownMenuButton">');
                                dropdownMenu.append('<li><a class="dropdown-item menu-option" href="mailto:' + request.patientEmail + '">' + request.patientEmail + '</a></li>');
                                dropdownMenu.append('<li><a class="dropdown-item menu-option" href="mailto:' + request.requesterEmail + '">' + request.requesterEmail + '</a></li>');
                                var dropdownButton = $('<div class="dropdown">');
                                dropdownButton.append('<button class="btn btn-outline-light dropdown-toggle" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-expanded="false"><i class="bi bi-envelope"></i></button>');
                                dropdownButton.append(dropdownMenu);
                                emailCell.append(dropdownButton);
                                newRow.append(emailCell);
                            } else {
                                if(request[key] == null) request[key] = '-';
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
                        'Doctors Note': 'doctorNote.png',
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
                        var imageUrl = menuOptionImageMapping[enumName];

                        if ([2 , 3 , 5 , 9 , 10 , 11 , 12].includes(option)) {
                            var link = toCamelCase(enumName) + '?request=' + request.requestId;
                            dropdownMenu.append('<li><a class="dropdown-item menu-option" href="/admin/' + link + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '">' + '<image src="./images/' + imageUrl + '" class="menu-icon" />' + enumName + '</a></li>');
                        } else {
                            var id = toCamelCase(enumName)
                            dropdownMenu.append('<li><button class="dropdown-item menu-option" id="' + id + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '">' + '<image src="./images/' + imageUrl + '" class="menu-icon" />' + enumName + '</button></li>');
                        }

                    });

                    var dropdownButton = $('<div class="dropdown">');
                    dropdownButton.append('<button class="btn btn-outline-light dropdown-toggle" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>');
                    dropdownButton.append(dropdownMenu);
                    actionsCell.append(dropdownButton);
                    newRow.append(actionsCell);
                    $('#request-table tbody').append(newRow);

                });


                // $('#request-table').DataTable({
                //     paging: false,
                //      searching: true,
                //      ordering: false,
                //      retrieve: true,
                //      info: false,
                //      "initComplete": function (settings, json) {

                //         $('#patientsearch').val(settings.oPreviousSearch.sSearch);

                //         $('#patientsearch').on('keyup', function () {
                //             var searchValue = $(this).val();
                //             settings.oPreviousSearch.sSearch = searchValue;
                //             settings.oApi._fnReDraw(settings);
                //         });
                //     },
                //  });



                $('#request-table_length').css('display', 'none');


                $('#request-table tbody tr').each(function () {
                    var requestTypeId = $(this).data('request-type-id');
                    // var colors = ['#5fbc61', '#ffb153', '#fb91ca', '#007fc6e8'];
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

    reloadDataTable(1);


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




    $('#NewState').click(function () {
        $('.active').not(this).removeClass('active');
        $('#requestState').text('(New)');
        $(this).toggleClass('active');
    });

    $('#ActiveState').click(function () {
        $('.active').not(this).removeClass('active');
        $('#requestState').text('(ACTIVE)');
        $(this).toggleClass('active');
    });
   
    $('#PendingState').click(function () {
        $('.active').not(this).removeClass('active');
        $('#requestState').text('(PANDING)');
        $(this).toggleClass('active');
    });

    $('#ConcludeState').click(function () {
        $('.active').not(this).removeClass('active');
        $('#requestState').text('(CONCLUDE)');
        $(this).toggleClass('active');
    });

    $('#ToCloseState').click(function () {
        $('.active').not(this).removeClass('active');
        $('#requestState').text('(TO CLOSE)');
        $(this).toggleClass('active');
    });
    $('#UnpaidState').click(function () {
        $('.active').not(this).removeClass('active');
        $('#requestState').text('(UNPAID)');
        $(this).toggleClass('active');
    });
});

function Search() {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("patientsearch");
    console.log(input)
    filter = input.value.toUpperCase();
    console.log(filter)
    table = document.getElementById("request-table");
    tr = table.getElementsByTagName("tr");
    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[1];
        if (td) {
            txtValue = td.textContent || td.innerText;
            //indexOf will return the position of first matched character
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            }
            else {
                tr[i].style.display = "none";
            }
        }
    }
}

