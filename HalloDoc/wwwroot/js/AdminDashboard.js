
$(document).ready(function () {


    var lastFilter;
    var lastregion;
    //get requestCount

    var URLFORCOUNT = "getRequestCountPerStatusId";

    $.ajax({
        url: URLFORCOUNT,
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
        10: "Encounter",
        11: "Close Case",
        12: "Accept",
        13: "Conclude Care"
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
        'Encounter': 'encounter.png',
        'Close Case': 'closeCase.png',
        'Accept': 'clearCase.png',
        'Conclude Care': 'encounter.png',
    };
    function reloadDataTable(statusID, reqtype, filter) {
        var tableData;
        $.ajax({
            url: '/Admin/GetRequestByStatusId',
            type: "GET",
            data: { statusID: statusID, reqtype: reqtype, regionFilter: filter },
            async: false,
            success: function (data) {

                console.log(data)

                tableData = data;
                $('#request-table tbody').empty();
                $('#request-table thead').empty();
                $('#RequestAccordion').empty();

                if (data.length == 0) {
                    var norow = `<tr><td class="px-5 py-2"><h2 class="p-4 alert alert-info"> No request found.</h2></td></tr>`;
                    $('#request-table tbody').append(norow);
                    $('#RequestAccordion').append(`<h2 class="p-4 text-center alert alert-info m-4"> No request found.</h2>`);

                }

                if (data.length > 0) {
                    data.sort((a, b) => a.requestId - b.requestId);
                    if (window.innerWidth < 900) {
                        renderMobileAccoridan(data, data[0].isPhysicianDashboard);
                    } else {
                        renderDesktopTable(data, statusID, data[0].isPhysicianDashboard);
                    }
                }
            },
            error: function () {
                alert("Error while fetching Data");
            }
        });
        return tableData;

    }

    function renderDesktopTable(data, statusID, isPhysicianDashboard) {
        debugger
        var headers = Object.keys(data[0]);

        if (isPhysicianDashboard) {
            headers = ["patientName", "patientEmail", "patientPhoneNumber", "address", "callStatus", "menuOptions"];
        }

        var headerMapping = {
            "requestId": "Request ID",
            "patientName": "Name",
            "patientEmail": "Email",
            "physicianName": "Physician",
            "patientPhoneNumber": "Phone Number",
            "status": "Status",
            "dateOfBirth": "DateOfBirth",
            "requesterName": "Requester",
            "requestedDate": "Requested Date",
            "requesterPhoneNumber": " Requester Phone Number",
            "address": "Address",
            "notes": "Notes",
            "callStatus": "Status",
            "menuOptions": "Action"
        };



        var headerRow = $('<tr>');
        headers.forEach(function (header) {


            if (header === "physicianName" && statusID == 1) {
                return;
            }

            if (isPhysicianDashboard == false && header === "callStatus") {
                return;
            }

            if (isPhysicianDashboard == true && header === "callStatus" && [1, 2, 4].includes(+statusID)) {
                return;
            }

            if (["requestTyepid", "status", "requesterPhoneNumber", "requesterEmail", "requestId", "regionId", "isPhysicianDashboard"].includes(header)) {
                return;
            }

            var columnName = headerMapping[header] || header;
            headerRow.append('<th class="py-4">' + columnName + '</th>');
        });


        $('#request-table thead').append(headerRow);
        
        data.forEach(function (request, index) {

            var newRow = $('<tr class="data-row">').attr('data-request-type-id', request.requestTyepid);
            var newAccordionItem = $('<div class="accordion-item">');

            for (var key in request) {

                // Exclude certain keys from being displayed
                if (!["menuOptions", "requestTyepid", "status", "requesterPhoneNumber", "requesterEmail", "requestId", "regionId", "isPhysicianDashboard"].includes(key)) {

                    if (statusID == 1 && key === 'physicianName') {
                        // Handle special case if statusID is 1 and key is 'physicianName'
                    } else if (key === "patientPhoneNumber") {
                        // Display patient phone number
                        var phoneNumbers = '<button class="btn btn-sm btn-outline-light my-1"> <i class="bi bi-telephone"></i> ' + request.patientPhoneNumber + '</button>' + (request.requesterPhoneNumber && request.patientPhoneNumber != request.requesterPhoneNumber ? ' <br />' + '<button class="btn btn-sm btn-outline-light my-1"> <i class="bi bi-telephone"></i> ' + request.requesterPhoneNumber + '</button><br />' : '');
                        if ([2, 3, 4].includes(request.requestTyepid)) {
                            phoneNumbers += "<br/>(" + ["familyfriend", "concierge", "business"][request.requestTyepid - 2] + ")";
                        } else {
                            phoneNumbers += " <br/>(Patient)";
                        }
                        newRow.append('<td class="' + key + '">' + phoneNumbers + '</td>');
                    } else if (key === 'patientEmail') {
                        // Display patient email with dropdown menu
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
                        // Display other fields
                        if (request[key] == null) request[key] = '-';
                        if (isPhysicianDashboard && ["patientName", "patientEmail", "patientPhoneNumber", "address", "callStatus"].includes(key)) {
                            debugger
                            if (key === "callStatus" && [1, 2, 4].includes(+statusID)) {
                                continue;
                            }
                            else {
                                if (request[key] == 1) {
                                    newRow.append(`<td class="houseCall"> <button class="btn btn-info text-white">House call</button> </td>`);
                                    newRow.find('.houseCall button').click(function () {
                                        HouseCallEventHandler(request.requestId);
                                    });

                                    continue;
                                }
                                else {
                                    newRow.append('<td class="' + key + '">' + request[key] + '</td>');
                                    continue;
                                }
                            }

                            newRow.append('<td class="' + key + '">' + request[key] + '</td>');
                        }

                        if (!isPhysicianDashboard) {

                            if (key === "callStatus") {
                                continue;
                            }

                            // Replace null values with '-'
                            if (key === 'dateOfBirth') {
                                // Format date of birth
                                const date = new Date(request[key]);
                                const options = { month: 'short', day: 'numeric', year: 'numeric' };
                                const formatter = new Intl.DateTimeFormat('en-US', options);
                                request[key] = formatter.format(date).substring(0, 4) + " " + date.getDay() + " , " + date.getFullYear();
                            }
                            newRow.append('<td class="' + key + '">' + request[key] + '</td>');
                        }
                    }
                }
            }

            var actionsCell = $('<td>');
            var dropdownMenu = $('<ul class="dropdown-menu" aria-labelledby="dropdownMenuButton">');

            // Populate dropdown menu with options
            request.menuOptions.forEach(function (option) {
                var enumName = mapNumberToEnumName(option);
                var imageUrl = menuOptionImageMapping[enumName];
                var modalId = toCamelCase(enumName) + 'Modal';

                // Here we check if the option is either 2,3,5,9,11 then we have to open particular modal so we append link which open respectively modal other wise we append a link which redirect to particular page

                if ([2, 3, 5, 9, 11, 13].includes(option)) {
                    var link = toCamelCase(enumName) + '?request=' + request.requestId;
                    dropdownMenu.append(`<li><a class="dropdown-item menu-option" href="${link}" data-option="${enumName}" data-request-id="${request.requestId}"><image src="/images/${imageUrl}" class="menu-icon" />${enumName}</a></li>`);

                } else if (option == 10) {

                    // Here we do some complex task like first we fetch the status and Request call type of particular request and if the requestcalltype is undefined(it means it is null in database then still request has not any call type then by default we show the select call type modal)

                    let status = getEncounterFormstatus(request.requestId);
                    let requestCallType = getRequestCallType(request.requestId);
                    console.log(requestCallType);

                    if (requestCallType == undefined && isPhysicianDashboard == true) {
                        dropdownMenu.append(`<li><div class="dropdown-item menu-option open-modal" data-option="${enumName}" data-request-id="${request.requestId}" data-modal-id="selectCallType" data-request-type-id="${request.requestTyepid}"><image src="/images/${imageUrl}" class="menu-icon" />${enumName}</div></li>`);
                    }
                    else {
                        // Once the reqeustCall type is not undefined that means the request has call type then we make one more condition like if the status is 1 then the encounter form is finalized so we show modal that allow provider to download the encounter form other wise we shown the encounter page to physician.

                        // Note:- This is only work for provider admin every time see the encounter page 

                        if (status == 1 && isPhysicianDashboard == true) {
                            var id = toCamelCase(enumName);
                            dropdownMenu.append(`<li><div class="dropdown-item menu-option open-modal" data-option="${enumName}" data-request-id="${request.requestId}" data-modal-id="${modalId}" data-request-type-id="${request.requestTyepid}"><image src="/images/${imageUrl}" class="menu-icon" />${enumName}</div></li>`);
                        } else {
                            var link = toCamelCase(enumName) + '?request=' + request.requestId;
                            dropdownMenu.append(`<li><a class="dropdown-item menu-option" href="${link}" data-option="${enumName}" data-request-id="${request.requestId}"><image src="/images/${imageUrl}" class="menu-icon" />${enumName}</a></li>`);
                        }
                    }
                } else {
                    var id = toCamelCase(enumName);
                    dropdownMenu.append(`<li><div class="dropdown-item menu-option open-modal" data-option="${enumName}" data-request-id="${request.requestId}" data-modal-id="${modalId}" data-request-type-id="${request.requestTyepid}"><image src="/images/${imageUrl}" class="menu-icon" />${enumName}</div></li>`);
                }
            });

            var dropdownButton = $('<div class="dropdown">');
            dropdownButton.append('<button class="btn btn-outline-light dropdown-toggle" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>');
            dropdownButton.append(dropdownMenu);
            actionsCell.append(dropdownButton);
            newRow.append(actionsCell);
            $('#request-table tbody').append(newRow);

            // Colorize rows based on request type
            $('#request-table tbody tr').each(function () {
                var requestTypeId = $(this).data('request-type-id');
                var colors = ['forestgreen', 'darkorange', 'deeppink', 'dodgerblue'];
                if (requestTypeId >= 1 && requestTypeId <= 4) {
                    $(this).css('background-color', colors[requestTypeId - 1]);
                }
            });

            // Setup pagination with 10 items per page
            setupPagination(10 , ".data-row");
        });

    }



    function renderMobileAccoridan(data, isPhysicianDashboard) {
        $(".data-row").remove();
        var accordion = $('#RequestAccordion');
        data.forEach(function (request, index) {
            console.log(request);
            var panelId = 'panel' + index;
            var panel = $('<div class="accordion-item data-row">').attr('data-request-type-id', request.requestTyepid);
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

                if ([3, 5, 9, 11, 13].includes(option)) {
                    var link = toCamelCase(enumName) + '?request=' + request.requestId;
                    actions += '<a class="btn w-50 btn-transparent border border-1 text-white rounded-5" href="' + link + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '" style="background-color: ' + backgroundColor + '">' + enumName + '</a>';
                }
                else if (option == 10) {

                    let status = getEncounterFormstatus(request.requestId);
                    let requestCallType = getRequestCallType(request.requestId);
                    var id = toCamelCase(enumName)

                    if (requestCallType == undefined && isPhysicianDashboard == true) {

                        actions += '<a class="btn w-50 btn-transparent border border-1 text-white rounded-5 open-modal" id="' + id + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '" data-toggle="modal"' + ' data-modal-id="selectCallType" style="background-color: ' + backgroundColor + '"' + 'data-request-type-id = "' + request.requestTyepid + '" >' + enumName + '</a>';
                    }
                    else {

                        // Once the reqeustCall type is not undefined that means the request has call type then we make one more condition like if the status is 1 then the encounter form is finalized so we show modal that allow provider to download the encounter form other wise we shown the encounter page to physician.

                        // Note:- This is only work for provider admin every time see the encounter page 

                        if (status == 1 && isPhysicianDashboard == true) {
                            actions += '<a class="btn w-50 btn-transparent border border-1 text-white rounded-5 open-modal" id="' + id + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '" data-toggle="modal"' + ' data-modal-id="' + modalId + '" style="background-color: ' + backgroundColor + '"' + 'data-request-type-id = "' + request.requestTyepid + '" >' + enumName + '</a>';

                        } else {
                            var link = toCamelCase(enumName) + '?request=' + request.requestId;
                            actions += '<a class="btn w-50 btn-transparent border border-1 text-white rounded-5" href="' + link + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '" style="background-color: ' + backgroundColor + '">' + enumName + '</a>';
                        }

                    }
                }

                else if (option == 2) {
                        var link = toCamelCase(enumName) + '?request=' + request.requestId;
                        viewCase += '<a class="btn w-50 btn-transparent border border-1 border-primary text-primary rounded-5 viewCasebtn" href="' + link + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '" style="background-color: ' + backgroundColor + '">' + enumName + '</a>'
                }
                else {
                   
                        var id = toCamelCase(enumName)
                        actions += '<a class="btn w-50 btn-transparent border border-1 text-white rounded-5 open-modal" id="' + id + '" data-option="' + enumName + '" data-request-id="' + request.requestId + '" data-toggle="modal"' + ' data-modal-id="' + modalId + '" style="background-color: ' + backgroundColor + '"' + 'data-request-type-id = "' + request.requestTyepid + '" >' + enumName + '</a>';
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

            var header = $(`<button class="accordion-button bg-info-subtle border-bottom border-1 border-secondary-subtle" type="button" data-bs-toggle="collapse" data-bs-target="#${panelId}" aria-expanded="true" aria-controls="${panelId}">`);

            header.append(`<div class="w-100 gap-2 border-5 pt-3 pb-5 px-4"><div class="d-flex justify-content-between mb-3"><span class="mobilesearch fs-5 ">${request.patientName}</span><div class="d-flex align-items-center"><div class="border border-1 rounded-5 m-1" style="width: 15px; height:15px; background-color: ${bgColor};"></div><span class="fs-5">${requestTypeText}</span></div></div><div class="d-flex justify-content-between "><div class=""><span class="fs-5">${request.address}</span></div><div class="btn btn-transparent btn border border-1 border-info text-info rounded-5"><i class="bi bi-geo-alt-fill fs-5"></i></div></div></div>`);

            var accordionBody = `<div class="accordion-body p-0"><div class="d-flex flex-column gap-2 bg-info-subtle mb-2 py-3 px-4 "><div class="text-end">${viewCase}</div><div><i class="fa-regular fa-calendar fs-5 border border-1 border-info rounded-5 p-2 me-2"></i><span class="fs-5">Date of Birth: ${request.dateOfBirth}</span></div><div><i class="fa-regular fa-envelope fs-5 border border-1 border-info rounded-5 p-2 me-2"></i><span class="fs-5">Email: ${request.patientEmail}</span></div><div><i class="fas fa-phone fs-5 border border-1 border-info rounded-5 p-2 me-2"></i><span class="fs-5">Phone: ${request.patientPhoneNumber}</span></div>`;

            if (!isPhysicianDashboard) {
                accordionBody += `<div><i class="fa-regular fa-user fs-5 border border-1 border-info rounded-5 p-2 me-2"></i><span class="fs-5">Requestor: ${request.requesterName}</span></div>`;
            }

            if (request.status === 2 && !isPhysicianDashboard) {
                accordionBody += '<div><i class="fa-regular fa-user fs-5 border border-1 border-info rounded-5 p-2 me-2"></i>' + '<span class="fs-5">Doctor Name: ' + request.physicianName + '</span></div>';
            }


            if (request.status === 4 && isPhysicianDashboard && request.callStatus == 1) {
                actions += '<button class="btn w-50 btn-transparent border border-1 text-white rounded-5 btn btn-info text-white houseCall"> House Call </button>';
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

            if (request.status === 4 && isPhysicianDashboard && request.callStatus == 1) {
                var houseCallButton = body.find('.houseCall');
                houseCallButton.click(function () {
                    HouseCallEventHandler(request.requestId);
                });
            }

            

            setupPagination(5 , ".data-row");

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
        localStorage.setItem("lastStatusID", statusId);
        reloadDataTable(statusId, 0, 0);
        $('.filter-btn').css("border", "none");
        $('.filter-btn:first').css("border", "1px solid gray");
    });

    var lastStatusID = localStorage.getItem("lastStatusID")
    if (lastStatusID == null) {
        reloadDataTable(1, 0, 0)
        $('.filter-btn:first').css("border", "1px solid gray");
    } else {
        reloadDataTable(lastStatusID, 0, 0);
        $('.filter-btn:first').css("border", "1px solid gray");
    }


    function getEncounterFormstatus(requestId) {

        var status;
        $.ajax({
            url: '/Admin/EcounterFormStatus',
            type: "GET",
            data: { requestId },
            async: false,
            success: function (data) {
                status = data;
            },
            error: function () {
                alert("While featching count");
            }
        })

        return status;
    }

    function getRequestCallType(requestId) {

        var type;
        $.ajax({
            url: '/Admin/RequestCallType',
            type: "GET",
            data: { requestId },
            async: false,
            success: function (data) {
                type = data;
                console.log(data);
            },
            error: function () {
                alert("While featching count");
            }
        })

        return type;
    }


    /* Export Data to CSV */
    $("#export").on("click", function () {
        var lastStatusID = localStorage.getItem("lastStatusID");
        let data = reloadDataTable(lastStatusID, lastFilter, lastregion);
        exportDataToCSV(data, "data.csv");
    })

    /* Export All Data to CSV */
    $('#exportAll').click(function () {
        debugger
        var lastStatusID = localStorage.getItem("lastStatusID")
        let data = reloadDataTable(lastStatusID, 0, 0);
        $('.filter-btn').css("border", "none");
        $('.filter-btn:first').css("border", "1px solid gray");
        exportDataToCSV(data, "data.csv");
    })

    /* Send Link Modal*/
    $('#open-sendLinkModal').on("click", function () {
        $("#sendLinkModal").modal('show');
    })
    /* Send Link Logic */
    $('#sendLink').validate({
        rules: {
            firstNameSendLink: {
                required: true,
            },
            lastNameSendLink: {
                required: true,
            },
            phoneSendLink: {
                required: true,
                digits: true,
            },
            emailSendLink: {
                required: true,
                email: true
            }
        },
        messages: {
            firstNameSendLink: {
                required: "Please enter a First Name",
            },
            lastNameSendLink: {
                required: "Please enter a Last Name",
            },
            phoneSendLink: {
                required: "Please enter a Phone Number",
                digits: "Please enter a valid phone number."
            },
            emailSendLink: {
                required: "Please enter an email address.",
                email: "Please enter a valid email address."
            }
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
            error.addClass('text-danger');
        },
        submitHandler: function (form) {
            var formData = new FormData(form);
            $.ajax({
                url: "/Admin/SendLink",
                method: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    Swal.fire({
                        title: "Done",
                        text: "Link sent to patient successfully",
                        icon: "success",
                        showConfirmButton: false,
                        timer: 1000
                    }).then(function () {
                        location.reload();
                    })
                },
                error: function (xhr, status, error) {
                    showToaster("Error while sending Link to patient", "error");
                }
            });
        }
    });

    /* Request Support Modal */
    $('#open-requestSupportModal').on("click", function () {
        $("#requestSupportModal").modal('show');
    })

    $('#requestSupport').validate({
        rules: {
            supportMessage: {
                required: true,
            }
        },
        messages: {
            supportMessage: {
                required: "Please Enter Support Message !",
            }
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
            error.addClass('text-danger');
        },
        submitHandler: function (form) {
            var formData = new FormData(form);
            $.ajax({
                url: "/Admin/RequestSupport",
                method: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    console.log(response);
                },
                error: function (xhr, status, error) {
                    showToaster("Error while sending Request Message Providers !", "error");
                }
            });
        }
    });



    /* RequestType ID Filter */
    $('.filter-btn').click(function () {
        var filterValue = +$(this).data('filter-value') || 0;
        $('.filter-btn').css("border", "none");
        $(this).css("border", "1px solid gray");
        lastFilter = filterValue;
        showSpinnerAndFilter(filterValue);
    });

    function showSpinnerAndFilter(requestTypeId) {
        $('#spinner').show();
        setTimeout(function () {
            $('#spinner').hide();
            filterDataTable(requestTypeId);
        }, 0);
    }
    function filterDataTable(requestTypeId) {
        var lastState = localStorage.getItem("lastStatusID");
        reloadDataTable(lastState, requestTypeId, lastregion);
    }

    /*Region Filter */
    $('#regionsearch').on("change", function () {
        let reigonId = +$(this).val();
        lastregion = reigonId;
        var lastState = localStorage.getItem("lastStatusID");
        reloadDataTable(lastState, lastFilter, reigonId);
    })

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



    /* For Provider site house call button event handler */
    function HouseCallEventHandler(requestId) {
        const swalWithBootstrapButtons = Swal.mixin({
            customClass: {
                confirmButton: "btn btn-info mx-2 btn-lg text-white my-2 mb-2",
                cancelButton: "btn btn-outline-info btn-lg hover_white"
            },
            buttonsStyling: false
        });

        swalWithBootstrapButtons.fire({
            title: "Confirmation of Complete Request",
            text: `Are you sure you want to Complete this request? Once complete this  request ${requestId} then this request is goes to conclude state`,
            iconHtml: "<div class='warning_icon'><i class='bi bi-exclamation-circle-fill'></i></div>",
            showCancelButton: true,
            confirmButtonText: "Complete",
            cancelButtonText: "Cancel",
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "houseCallevenetHandler",
                    method: "POST",
                    data: { requestId },
                    success: function (response) {
                        Swal.fire({
                            title: "Completed",
                            text: "Request completed Sucessfully",
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false,
                        }).then(function () {
                            location.reload();
                        })
                    },
                    error: function (xhr, status, error) {
                        showToaster("Error While completing Request", "error");
                    }
                });
            }
        });
    }



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
