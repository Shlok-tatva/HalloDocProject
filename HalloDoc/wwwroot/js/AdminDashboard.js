
$(document).ready(function () {

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

    function reloadDataTable(statusID) {
        $.ajax({
            url: '/Admin/GetRequestByStatusId',
            type: "GET",
            data: { status_id: statusID },
            success: function (data) {
    
                $('#request-table tbody').empty();
                $('#request-table thead').empty();
                $('#RequestAccordion').empty();
    
                if (data.length > 0) {
                    data.sort((a, b) => a.requestId - b.requestId);
                    if (window.innerWidth < 900) {
                        renderMobileAccoridan(data);
                    } else {
                        renderDesktopTable(data, statusID);
                    }
                }
            },
            error: function () {
                alert("Error while fetching Data");
            }
    
        });
    }

    function renderDesktopTable(data, statusID) {
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

            if (["requestTyepid", "status", "requesterPhoneNumber", "requesterEmail", "requestId"].includes(header)) {
                return;
            }

            var columnName = headerMapping[header] || header;

            headerRow.append('<th class="py-4">' + columnName + '</th>');

        });
        $('#request-table thead').append(headerRow);
        data.forEach(function (request, index) {
            var newRow = $('<tr class="data-row">').attr('data-request-type-id', request.requestTyepid);
            var newAcordian = $('<div class="accordion-item">');


            for (var key in request) {
                /* For Desktop View Insert table*/

                if (!["menuOptions", "requestTyepid", "status", "requesterPhoneNumber", "requesterEmail", "requestId"].includes(key)) {
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
                        dropdownMenu.append(`<li><a class="dropdown-item menu-option" href="mailto:${request.patientEmail}">${request.patientEmail}</a></li>`);
                        dropdownMenu.append(`<li><a class="dropdown-item menu-option" href="mailto:${request.requesterEmail}">${request.requesterEmail}</a></li>`);
                        var dropdownButton = $('<div class="dropdown">');
                        dropdownButton.append('<button class="btn btn-outline-light dropdown-toggle" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-expanded="false"><i class="bi bi-envelope"></i></button>');
                        dropdownButton.append(dropdownMenu);
                        emailCell.append(dropdownButton);
                        newRow.append(emailCell);
                    } else {
                        if (request[key] == null) request[key] = '-';
                        newRow.append('<td class="' + key + '">' + request[key] + '</td>');
                    }
                }
            }

            var actionsCell = $('<td>');
            var dropdownMenu = $('<ul class="dropdown-menu" aria-labelledby="dropdownMenuButton">');

            request.menuOptions.forEach(function (option) {
                var enumName = mapNumberToEnumName(option);
                var imageUrl = menuOptionImageMapping[enumName];
                var modalId = toCamelCase(enumName) + 'Modal';

                if ([2, 3, 5, 9, 10, 11, 12].includes(option)) {
                    var link = toCamelCase(enumName) + '?request=' + request.requestId;
                    dropdownMenu.append(`<li><a class="dropdown-item menu-option" href="/admin/${link}" data-option="${enumName}" data-request-id="${request.requestId}"><image src="./images/${imageUrl}" class="menu-icon" />${enumName}</a></li>`);
                } else {
                    var id = toCamelCase(enumName);
                    dropdownMenu.append(`<li><div class="dropdown-item menu-option open-modal" data-option="${enumName}" data-request-id="${request.requestId}" data-modal-id="${modalId}"><image src="./images/${imageUrl}" class="menu-icon" />${enumName}</div></li>`);
                   
                }

            });

            var dropdownButton = $('<div class="dropdown">');
            dropdownButton.append('<button class="btn btn-outline-light dropdown-toggle" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>');
            dropdownButton.append(dropdownMenu);
            actionsCell.append(dropdownButton);
            newRow.append(actionsCell);
            $('#request-table tbody').append(newRow);

            $('#request-table tbody tr').each(function () {
                var requestTypeId = $(this).data('request-type-id');
                // var colors = ['#5fbc61', '#ffb153', '#fb91ca', '#007fc6e8'];
                var colors = ['forestgreen', 'darkorange', 'deeppink', 'dodgerblue'];
                if (requestTypeId >= 1 && requestTypeId <= 4) {
                    $(this).css('background-color', colors[requestTypeId - 1]);
                }
            });
            
            setupPagination(10);


        });
    }

    function renderMobileAccoridan(data) {
        $(".data-row").remove();
        var accordion = $('#RequestAccordion');
        data.forEach(function (request, index) {           
            var panelId = 'panel' + index;
            var panel = $('<div class="accordion-item data-row">').attr('data-request-type-id', request.requestTyepid);;
            var header = $('<h2 class="accordion-header" id="heading' + panelId + '">');
            var body = $('<div id="' + panelId + '" class="accordion-collapse collapse">');
            var actions = '';
            var viewCase = '';
            var menuOptionColors = {
                0: '#943DB8', 
                1: '#E42929', 
                2: 'white',  
                3: '#228B22',  
                4: '#228B22',  
                5: '#943DB8',
                6: '#00ADEF',
                7: '#00ADEF',
                8: '#E42929',
                9: '#228B22',
                10: '#EE9125',
                11: '#228B22',
            };

            request.menuOptions.forEach(function (option) {
                var enumName = mapNumberToEnumName(option);
                var backgroundColor = menuOptionColors[option] || 'blue';
                var modalId = toCamelCase(enumName) + 'Modal';

                if ([3, 5, 9, 10, 11, 12].includes(option)) {
                    var link = toCamelCase(enumName) + '?request=' + request.requestId;
                    actions += '<a class="btn w-50 btn-transparent border border-1 text-white rounded-5" href="/admin/' + link + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '" style="background-color: ' + backgroundColor + '">' + enumName + '</a>';
                }
                else if (option == 2) {
                    var link = toCamelCase(enumName) + '?request=' + request.requestId;
                    viewCase += '<a class="btn w-50 btn-transparent border border-1 border-primary text-primary rounded-5 viewCasebtn" href="/admin/' + link + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '" style="background-color: ' + backgroundColor + '">' + enumName + '</a>'
                }
                else {
                    var id = toCamelCase(enumName)
                    actions += '<a class="btn w-50 btn-transparent border border-1 text-white rounded-5 open-modal" id="' + id + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '" data-toggle="modal"' + ' data-modal-id="' + modalId + '" style="background-color: ' + backgroundColor + '">' + enumName + '</a>';
                }
            });

            var requestTypeText;
            var bgColor;
            switch (request.requestTyepid) {
                case 1:
                    requestTypeText = "Patient";
                    bgColor = "forestgreen";
                    break;
                case 2:
                    requestTypeText = "Family/Friend";
                    bgColor = "darkorange";
                    break;
                case 3:
                    requestTypeText = "Concierge";
                    bgColor = "deeppink";
                    break;
                case 4:
                    requestTypeText = "Business";
                    bgColor = "dodgerblue";
                    break;
                default:
                    requestTypeText = "Unknown";
                    bgColor = "black";
            }

            var header = $(`<button class="accordion-button bg-info-subtle border-bottom border-1 border-secondary-subtle " type="button" data-bs-toggle="collapse" data-bs-target="#${panelId}" aria-expanded="true" aria-controls="${panelId}">`);

            header.append(`<div class="w-100 gap-2 border-5 pt-3 pb-5 px-4"><div class="d-flex justify-content-between mb-3"><span class="mobilesearch fs-5 ">${request.patientName}</span><div class="d-flex align-items-center"><div class="border border-1 rounded-5 m-1" style="width: 15px; height:15px; background-color: ${bgColor};"></div><span class="fs-5">${requestTypeText}</span></div></div><div class="d-flex justify-content-between "><div class=""><span class="fs-5">${request.address}</span></div><div class="btn btn-transparent btn-sm border border-1 border-info text-info rounded-5">Map Location</div></div></div>`);

            var accordionBody = `<div class="accordion-body p-0"><div class="d-flex flex-column gap-2 bg-info-subtle mb-2 py-3 px-4 "><div class="text-end">${viewCase}</div><div><i class="fa-regular fa-calendar fs-5 border border-1 border-info rounded-5 p-2 me-2"></i><span class="fs-5">Date of Birth: ${request.dateOfBirth}</span></div><div><i class="fa-regular fa-envelope fs-5 border border-1 border-info rounded-5 p-2 me-2"></i><span class="fs-5">Email: ${request.patientEmail}</span></div><div><i class="fas fa-phone fs-5 border border-1 border-info rounded-5 p-2 me-2"></i><span class="fs-5">Phone: ${request.patientPhoneNumber}</span></div><div><i class="fa-regular fa-user fs-5 border border-1 border-info rounded-5 p-2 me-2"></i><span class="fs-5">Requestor: ${request.requesterName}</span></div>`;

            if (request.status === 2) {
                accordionBody += '<div><i class="fa-regular fa-user fs-5 border border-1 border-info rounded-5 p-2 me-2"></i>' + '<span class="fs-5">Doctor Name: ' + request.doctorName + '</span></div>';
            }

            accordionBody += '<div class="d-flex flex-column mt-3 gap-3">' +
                '<div class="row g-2">' + actions + '</div>' +
                '</div>' +
                '</div>' +
                '</div>';


            body.append(accordionBody);
            panel.append(header);
            panel.append(body);
            accordion.append(panel);

            setupPagination(5);

        });
    }

    function toCamelCase(text) {
        return text.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, function (match, index) {
            if (+match === 0) return ""; // Remove spaces if it's a zero
            return index === 0 ? match.toLowerCase() : match.toUpperCase();
        });
    }

    function mapNumberToEnumName(number) {
        return menuOptionEnumMapping[number] || "Unknown";
    }


    $('.AdminState').click(function () {
        var statusId = $(this).data("status-id");
        localStorage.setItem("lastStatusID" , statusId);
        reloadDataTable(statusId);
    });


    var lastStatusID = localStorage.getItem("lastStatusID")
    reloadDataTable(lastStatusID || 1);




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
        error: function () {
            alert("Error while fetching count");
        }
    });



    $('.filter-btn').click(function () {
        var filterValue = $(this).data('filter-value') || null;
        showSpinnerAndFilter(filterValue);
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

        $('#RequestAccordion .accordion-item').each(function () {
            var accordionRequestTypeId = $(this).data('request-type-id');
            if (requestTypeId === null || accordionRequestTypeId === requestTypeId) {
                $(this).show(); // Show accordion item if it matches the requestTypeId or if requestTypeId is null (show all)
            } else {
                $(this).hide();
            }
        });

    }



function setLastState(state) {
    localStorage.setItem('lastState', state);
}


function getLastState() {
    return localStorage.getItem('lastState');
}


function handleStateClick(state) {
    $('.active').not(this).removeClass('active');
    $('#requestState').text('(' + state.toUpperCase() + ')');
    $(this).toggleClass('active');
}

    var lastState = getLastState();

    // Set initial state based on last state or default to 'New'
    if (lastState) {
        $('#requestState').text('(' + lastState.toUpperCase() + ')');
        $('#' + lastState + 'State').addClass('active');
    } else {
        $('#requestState').text('(New)');
        $('#NewState').addClass('active');
    }

    $('#NewState').click(function () {
        handleStateClick.call(this, 'New');
        setLastState('New');
    });

    $('#ActiveState').click(function () {
        handleStateClick.call(this, 'Active');
        setLastState('Active');
    });

    $('#PendingState').click(function () {
        handleStateClick.call(this, 'Pending');
        setLastState('Pending');
    });

    $('#ConcludeState').click(function () {
        handleStateClick.call(this, 'Conclude');
        setLastState('Conclude');
    });

    $('#ToCloseState').click(function () {
        handleStateClick.call(this, 'To Close');
        setLastState('ToClose');
    });

    $('#UnpaidState').click(function () {
        handleStateClick.call(this, 'Unpaid');
        setLastState('Unpaid');
    });

});


function Search() {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("patientsearch");
    filter = input.value.toUpperCase();
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

function setupPagination(items) {
    const itemsPerPage = items;
    let currentPage = 1;
    let totalPages = Math.ceil($('.data-row').length / itemsPerPage);
    const maxPageButtons = 3;
    let startPage = 1;

    function showRowsForPage(page) {
        const startIndex = (page - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;
        $('.data-row').hide();
        $('.data-row').slice(startIndex, endIndex).show();
    }

    function generatePaginationButtons() {
        $('#page-buttons').empty(); // Clear existing buttons
        let endPage = Math.min(totalPages, startPage + maxPageButtons - 1);
        for (let i = startPage; i <= endPage; i++) {
            const buttonClass = (i === currentPage) ? 'page-button current-page' : 'page-button';
            $('#page-buttons').append(`<button class="${buttonClass}" data-page="${i}">${i}</button>`);
        }
    }

    $('#next-button').on('click', function () {
        if (currentPage < totalPages) {
            currentPage++;
            startPage = Math.max(currentPage - Math.floor(maxPageButtons / 2), 1);
            showRowsForPage(currentPage);
            generatePaginationButtons();
            updateButtonStates();
        }
    });

    $('#prev-button').on('click', function () {
        if (currentPage > 1) {
            currentPage--;
            startPage = Math.max(currentPage - Math.floor(maxPageButtons / 2), 1);
            showRowsForPage(currentPage);
            generatePaginationButtons();
            updateButtonStates();
        }
    });

    $('#first-button').on('click', function () {
        currentPage = 1;
        startPage = 1;
        showRowsForPage(currentPage);
        generatePaginationButtons();
        updateButtonStates();
    });

    $('#last-button').on('click', function () {
        currentPage = totalPages;
        startPage = Math.max(totalPages - maxPageButtons + 1, 1);
        showRowsForPage(currentPage);
        generatePaginationButtons();
        updateButtonStates();
    });

    $(document).on('click', '.page-button', function () {
        const page = $(this).data('page');
        currentPage = page;
        showRowsForPage(currentPage);
        generatePaginationButtons();
        updateButtonStates();
    });

    // Function to update button states
    function updateButtonStates() {
        $('#prev-button').prop('disabled', currentPage === 1);
        $('#next-button').prop('disabled', currentPage === totalPages);
    }

    // Initialize page with first set of rows and pagination buttons
    showRowsForPage(currentPage);
    generatePaginationButtons();
    updateButtonStates();
}

