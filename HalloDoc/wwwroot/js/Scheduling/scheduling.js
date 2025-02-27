$(document).ready(function () {
    
    var isprovider = ($('#monthcontainer').data('isprovider') == 1 ? true : false);


    $(document).on('click', '.open-modal', function () {
        debugger
        var modalId = $(this).data("modal-id");

        if (modalId == "editShiftModal") {
            var shiftId = $(this).data("shiftid");
            var data = getShiftData(shiftId);

            $("#edit").hide();
            $("#delete").hide();
            $("#return").hide();
            $("#shiftid").val(shiftId);
            $("#regioneditshift").val(data.regionid);
            $('#StartDateEdit').val(data.shiftdate.split('T')[0]); // Extracting date part only
            $('#StartTimeEdit').val(data.starttime);
            $('#EndTimeEdit').val(data.endtime);
            $("#status").val(data.status);

            var shiftDate = new Date(data.shiftdate.split('T')[0]);
            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            if (shiftDate >= currentDate) {
                $("#edit").show();
                $("#delete").show();
                $("#return").show();
            }


            if (!isprovider) {
                getPhysicians(data.regionid, 'phyiscianeditshift', function () {
                    $("#phyiscianeditshift").val(data.physicianid);
                    $("#editShiftModal").modal("show");
                });
            }


            $("#editShiftModal").modal("show");
        }

        else if (modalId == "#exampleModal") {
            let index = parseInt($(this).data('index'));
            let month = parseInt($(this).data('month'));
            let shiftdate = new Date($(this).data('shiftdate'));

            if (!isNaN(month)) {
                $('#exampleModal').find('.model-head').empty();
                $('#model-head').html(index + ' / ' + months[month] + ' / ' + year);
                // Retrieve the corresponding dayDataArrayfinal
                let foundItem = extractedDataArray.find(o => parseInt(o.date) === index);
                let dayDataArrayfinal = foundItem ? foundItem.dayDataArray : [];

                // Clear previous modal content
                $('#exampleModal').find('.modal-body').empty();

                // Populate modal with shift details
                dayDataArrayfinal.forEach(item => {
                    let shiftDetails = `
                            <div class=" curser-pointer my-1 p-2 ${'Status-' + item.status} open-modal text-black"  data-modal-id="editShiftModal" data-shiftid="${item.shiftid}">
                            <div>Physician Name: ${item.physicianName}</div>
                            <div>Start Time: ${item.startTime}</div>
                            <div>End Time: ${item.endTime}</div>
                            <div>
                            `;
                    console.log(shiftDetails);
                    console.log($('#exampleModal').find('.modal-body'));
                    $('#exampleModal').find('.modal-body').append(shiftDetails);
                });
                $(modalId).modal("show");
            }
            if ($(this).data('shiftdate') != undefined) {
                $('#exampleModal').find('.model-head').empty();
                $('#model-head').html(shiftdate.getDate() + ' / ' + months[shiftdate.getMonth() - 1] + ' / ' + shiftdate.getFullYear());
                const matchingDates = providerList[index].dayList.filter(item => {
                    const rshiftdate = new Date(item.shiftdate);
                    return shiftdate.getDate() == rshiftdate.getDate() && shiftdate.getMonth() == rshiftdate.getMonth() + 1 && shiftdate.getFullYear() == rshiftdate.getFullYear();
                });

                $('#exampleModal').find('.modal-body').empty();

                matchingDates.forEach((item, index) => {
                    // Construct HTML for request details
                    const requestHTML = `<div class=" my-1 p-2 ${'Status-' + item.status} open-modal text-black" data-modal-id="editShiftModal" data-shiftid="${item.shiftid}" >
                                    <div>Physician Name: ${item.physicianName}</div>
                                            <div>Start Time: ${item.starttime}</div>
                                    <div>End Time: ${item.endtime}</div>
                                    <div>`;

                    // Append request details to modal body
                    $('#exampleModal .modal-body').append(requestHTML);
                });

                // Open the modal
                $('#exampleModal').modal('show');
            }

        }

        else {
            var date = $(this).data('date');
            var shiftDate = new Date(date);
            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            if (shiftDate < currentDate) {
                showToaster("Can't create Shift in past", "error");
            }
            else {
                $('#StartDate').val(date);
                console.log(date);
                $("#createShiftModal").modal("show");
            }

        }
    })



    var x = 1; // for check either featch Provider by region or month 
    var providerList = [];
    var extractedDataArray = [];

    console.log(isprovider);

    //Get Provider List in array
    if (!isprovider) {
        function getProviderList() {
            var regionId = $('#fregion').val();
            console.log(regionId);
            $.ajax({
                type: "GET",
                url: 'GetPhyscianDataForShift',
                data: { region: regionId },
                dataType: "json",
                success: function (response) {
                    providerList = [];
                    // Array to store converted objects
                    response.forEach(function (item) {
                        var dayList = []; // Array to store converted dayList
                        if (item.dayList) {
                            item.dayList.forEach(function (dayItem) {
                                var dayItemObj = {
                                    shiftid: dayItem.shiftid,
                                    physicianid: dayItem.physicianid,
                                    physicianName: dayItem.physicianName,
                                    physicianPhoto: dayItem.physicianPhoto,
                                    regionid: dayItem.regionid,
                                    startdate: dayItem.startdate,
                                    shiftdate: dayItem.shiftdate,
                                    starttime: dayItem.starttime,
                                    endtime: dayItem.endtime,
                                    isrepeat: dayItem.isrepeat,
                                    regionname: dayItem.regionName,
                                    checkWeekday: dayItem.checkWeekday,
                                    repeatupto: dayItem.repeatupto,
                                    status: dayItem.status,
                                    dayList: dayItem.dayList,
                                    submit: dayItem.submit
                                };
                                dayList.push(dayItemObj);
                            });
                        }
                        var providerObj = {
                            shiftid: item.shiftid,
                            id: item.physicianid,
                            name: item.physicianName,
                            photo: item.physicianPhoto,
                            regionid: item.regionid,
                            startdate: item.startdate,
                            shiftdate: item.shiftdate,
                            starttime: item.starttime,
                            endtime: item.endtime,
                            regionname: item.regionName,
                            isrepeat: item.isrepeat,
                            checkWeekday: item.checkWeekday,
                            repeatupto: item.repeatupto,
                            status: item.status,
                            dayList: dayList,
                            submit: item.submit
                        };
                        providerList.push(providerObj);
                    });
                    if (x == 2) {
                        $("#week").click();
                    }
                    else if (x == 3) {
                        $("#day").click();
                    }
                },
                error: function () {
                    // Handle error
                    console.error("Error fetching provider list");
                }
            });
        }
    }
    function GetShiftForMonth(month, year) {
        var regionId = $('#fregion').val();
        $.ajax({
            type: "GET",
            url: 'GetShiftByMonth',
            data: { month: month, year: year, regionId: regionId },
            dataType: "json",
            success: function (response) {
                // Initialize an array to store extracted data
                extractedDataArray = [];

                // Iterate over the array
                response.forEach(function (shift) {
                    // Extract and store data from each shift object
                    var extractedData = {
                        //  date: shift.shidtDate.match(/\d{4}-(\d{2})-\d{2}/)[1],
                        date: shift.shiftdate.match(/\d{4}-\d{2}-(\d{2})/)[1],
                        shiftId: shift.shiftid,
                        physicianId: shift.physicianid,
                        physicianName: shift.physicianName,
                        shiftDate: shift.shidtDate, // Correct the typo to "shiftDate"
                        startTime: shift.starttime,
                        endTime: shift.endtime,
                        isRepeat: shift.isrepeat
                    };

                    // Check if dayList exists and is an array
                    if (Array.isArray(shift.dayList)) {
                        // Create an array to store day data
                        var dayDataArray = [];

                        // Iterate over dayList and store data
                        shift.dayList.forEach(function (day) {
                            var dayData = {
                                shiftid: day.shiftid,
                                physicianName: day.physicianName,
                                startTime: day.starttime,
                                endTime: day.endtime,
                                status: day.status,
                            };
                            // Push day data into dayDataArray
                            dayDataArray.push(dayData);
                        });
                        // Add dayDataArray to extractedData object
                        extractedData.dayDataArray = dayDataArray;
                    }

                    // Push extracted data into the array
                    extractedDataArray.push(extractedData);
                });
                console.log(extractedDataArray);
                $('#month').click();
                manipulate();
            },
            error: function () {
                // Handle error
                console.error("Error fetching provider list");
            }
        });
    }

    let date = new Date();
    let year = date.getFullYear();
    let month = date.getMonth();
    console.log(month);
    console.log(year);

    GetShiftForMonth(month + 1, year);

    const day = document.querySelector(".calendar-dates");
    const weekdays = document.querySelector(".calendar-weekdays");
    const currdate = document.querySelector(".calendar-current-date");
    const prenexIcons = document.querySelectorAll(".calendar-navigation span");
    const months = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    const daysOfWeek = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

    const manipulate = () => {

        weekdays.innerHTML = "";
        daysOfWeek.forEach(day => {
            weekdays.innerHTML += `<th class="text-center">${day}</th>`;
        });
        let dayone = new Date(year, month, 1).getDay();

        // Get the last date of the month
        let lastdate = new Date(year, month + 1, 0).getDate();

        // Get the day of the last date of the month
        let dayend = new Date(year, month, lastdate).getDay();

        // Get the last date of the previous month
        let monthlastdate = new Date(year, month, 0).getDate();

        // Variable to store the generated calendar HTML
        let lit = "<tbody><tr>";
        let dayCount = 0;
        // Loop to add the last dates of the previous month
        for (let i = dayone; i > 0; i--) {
            lit += `<td class="inactive"><div></div><div></div><div></div><div></div><div></div></td>
                        `;
            dayCount++;
        }

        // Loop to add the dates of the current month
        for (let i = 1; i <= lastdate; i++) {

            if (dayCount > 6) {
                lit += `</tr><tr>`;
                dayCount = 0;
            }
            let isToday = i === date.getDate() && month === new Date().getMonth() && year === new Date().getFullYear() ? "active" : "";

            let foundItem = extractedDataArray.find(o => {
                return parseInt(o.date) === i;
            });
            let dayDataArrayfinal = foundItem ? foundItem.dayDataArray : "";
            let ProviderNames = [];
            let Status = [];
            let buttonId = `viewAllButton_${i}`;

            // If dayDataArrayfinal is found, iterate over each item and push ProviderName to ProviderNames array
            if (dayDataArrayfinal) {
                dayDataArrayfinal.forEach(item => {
                    let providerObj = {
                        name: "Name: " + item.physicianName + ' <br/> ' + "Time: " + item.startTime + " to " + item.endTime,
                        status: item.status,
                        shiftid: item.shiftid
                    };
                    ProviderNames.push(providerObj);
                    Status.push(item.status);
                });
            }

            lit += `<td class="table-text ${isToday} p-0"> 
                        <div class="first">${i}</div>
                        <div class="open-modal ${Status.length > 0 ? 'Status-' + Status[0] : ''}" ${Status.length > 0 ? 'data-modal-id="editShiftModal"' : ''} ${ProviderNames.length > 0 ? 'data-shiftid="' + ProviderNames[0].shiftid + '"' : ''} data-date="${year}-${(month + 1).toString().padStart(2, '0')}-${i.toString().padStart(2, '0')}">${ProviderNames.length > 0 ? ProviderNames[0].name : ''}</div>
                        <div class="open-modal ${Status.length > 1 ? 'Status-' + Status[1] : ''}" ${Status.length > 1 ? 'data-modal-id="editShiftModal"' : ''} ${ProviderNames.length > 1 ? 'data-shiftid="' + ProviderNames[1].shiftid + '"' : ''} data-date="${year}-${(month + 1).toString().padStart(2, '0')}-${i.toString().padStart(2, '0')}">${ProviderNames.length > 1 ? ProviderNames[1].name : ''}</div>
                        <div class="open-modal ${Status.length > 2 ? 'Status-' + Status[2] : ''}" ${Status.length > 2 ? 'data-modal-id="editShiftModal"' : ''} ${ProviderNames.length > 2 ? 'data-shiftid="' + ProviderNames[2].shiftid + '"' : ''} data-date="${year}-${(month + 1).toString().padStart(2, '0')}-${i.toString().padStart(2, '0')}">${ProviderNames.length > 2 ? ProviderNames[2].name : ''}</div>
                        <div type="button" id="${buttonId}" ${ProviderNames.length > 3 ? 'data-modal-id="#exampleModal"' : ''} data-month="${month}" data-index="${i}" class="${Status.length > 3 ? 'btn btn-info w-100 text-white rounded-0 open-modal' : ''}">${ProviderNames.length > 3 ? 'View All' : ''}</div>
                    </td>`;
            dayCount++;
        }

        // Loop to add the first dates of the next month
        for (let i = dayend; i < 6; i++) {
            lit += `<td class="inactive">
                                    <div></div><div></div><div></div><div></div><div></div>
                        </td>`;
        }
        lit += "</tr></tbody>";
        // Update the text of the current date element with the formatted current month and year
        currdate.innerText = `${months[month] == undefined ? "Januaray" : months[month]} ${year}`;

        // Update the HTML of the dates element with the generated calendar
        day.innerHTML = lit;
    };

    manipulate();

    prenexIcons.forEach(icon => {
        icon.addEventListener("click", () => {
            if (x == 1) {

                if (icon.id === "calendar-prev") {
                    month--;
                    if (month < 1) {
                        month = 12;
                        year--;
                    }
                } else if (icon.id === "calendar-next") {
                    month++;
                    console.log(month);
                    if (month > 12) {
                        month = 1;
                        year++;
                    }
                }
                // Call the manipulate function to update the calendar display
                GetShiftForMonth(month + 1, year);
            }

        });
    });



    /* Open Calender When USer click on Calneder Icon */
    const centerSpan = document.querySelector(".center-span");
    const datepicker = document.getElementById("datepicker");

    datepicker.addEventListener("change", (event) => {
        if (x == 1) {
            debugger;
            const selectedDate = new Date(event.target.value);
            month = selectedDate.getMonth();
            year = selectedDate.getFullYear();
            console.log(month, year);
            GetShiftForMonth(month, year);
            centerSpan.click();
        }
    });


    if (!isprovider) {
        getProviderList();

        // Event listener for the "Day" button
        $("#day").click(function () {
            x = 3;
            $('#monthcontainer').hide();
            $('#weekviewcontainer').hide();
            $('#daycontainer').show();

            // Define variables for current date and month
            let currentDate = new Date();
            let currentMonth = currentDate.getMonth();

            // Event listener for the "Next" button
            $('#calendar-next').on('click', function () {
                if (x == 3) {

                    currentDate.setDate(currentDate.getDate() + 1);

                    updateEmployeeSchedules();
                }

            });

            $('#calendar-prev').on('click', function () {
                if (x == 3) {
                    currentDate.setDate(currentDate.getDate() - 1);

                    updateEmployeeSchedules();
                }
            });

            updateEmployeeSchedules();

            function updateEmployeeSchedules() {
                $('.calendar-current-date').text(currentDate.toLocaleDateString('en-US', { month: 'short', year: 'numeric', day: 'numeric' }));

                $('#daycontainer .schedule .employee-schedules tbody').empty();
                const employeeSchedulesContainer = document.querySelector('.employee-schedules tbody');
                const shiftArrays = [];
                for (let index = 0; index < providerList.length; index++) {
                    const employee = providerList[index];
                    const employeeRow = document.createElement('tr');
                    const employeeCell = document.createElement('td');
                    employeeCell.classList.add("w-25");

                    // Create the image element
                    const image = document.createElement('img');
                    image.classList.add("img-fluid");
                    image.classList.add("ph-img");
                    image.src = '/Upload/Physician/' + employee.id + '/photo.png';
                    image.alt = employee.name;
                    employeeCell.appendChild(image);

                    employeeCell.appendChild(document.createTextNode(employee.name));
                    const shiftArray = Array(24).fill(0);
                    employeeRow.appendChild(employeeCell);
                    const cellDate = new Date(currentDate);

                    const matchingDates = providerList[index].dayList.filter(item => {
                        const shiftdate = new Date(item.shiftdate);
                        return cellDate.getDate() === shiftdate.getDate() &&
                            cellDate.getMonth() === shiftdate.getMonth() &&
                            cellDate.getFullYear() === shiftdate.getFullYear()
                    });

                    const timeRanges = matchingDates.map(item => {
                        const startTime = item.starttime;
                        const endTime = item.endtime;
                        let status = 0; // Default status is 0 (not available)
                        const shiftid = item.shiftid;
                        const regionname = item.regionname;

                        // Check the status of the shift
                        if (item.status === 1) {
                            status = 1; // Available
                        } else if (item.status === 0) {
                            status = 0.5; // Partially available
                        }

                        return { startTime, endTime, status, shiftid, regionname };
                    });

                    const hoursArray = Array(24).fill(0);
                    const startTimeArray = Array(24).fill(0);
                    const endTimeArray = Array(24).fill(0);
                    const shiftidArray = Array(24).fill(0);
                    const regionArray = Array(24).fill(0);

                    timeRanges.forEach(({ startTime, endTime, status, shiftid, regionname }) => {
                        // Parse start and end times to obtain hours
                        const startParts = startTime.split(' ')[0].split(':');
                        const endParts = endTime.split(' ')[0].split(':');

                        // Extract hours
                        const startHour = parseInt(startParts[0]);
                        const endHour = parseInt(endParts[0]);

                        // Determine the value to store based on status
                        const valueToStore = status;
                        for (let hour = startHour; hour <= endHour; hour++) {
                            shiftidArray[hour] = shiftid;
                        }

                        // Loop through hours and mark the corresponding index in the array if it falls within the range
                        for (let hour = startHour; hour <= endHour; hour++) {
                            hoursArray[hour] = valueToStore;
                            regionArray[hour] = regionname;
                            startTimeArray[hour] = startTime;
                            endTimeArray[hour] = endTime;
                        }
                    });



                    shiftArrays.push(shiftArray);

                    console.log(shiftidArray);
                    console.log(regionArray);

                    for (let i = 0; i < 24; i++) {
                        const timeSlot = document.createElement('td');

                        if (hoursArray[i] === 1 || hoursArray[i] === 0.5) {
                            const startTime = i;
                            let endTime = i;
                            const currentShiftId = shiftidArray[i];

                            // Find the end time for the continuous slot
                            while (endTime < 23 && (hoursArray[endTime + 1] === 1 || hoursArray[endTime + 1] === 0.5) && shiftidArray[endTime + 1] === currentShiftId) {
                                endTime++;
                            }

                            const colspan = endTime - startTime + 1;
                            timeSlot.colSpan = colspan;

                            // Use start time for the link to edit shift
                            (hoursArray[i] == 0.5) ? hoursArray[i] = 0 : hoursArray[i]; // just for color of the shift change the class name for that !
                            timeSlot.innerHTML += `<div data-modal-id="editShiftModal"  data-shiftid="${currentShiftId}" class="mt-1 d-block text-dark text-center open-modal"> ${regionArray[i]} <br /> ${startTimeArray[i]}-${endTimeArray[i]}</div>`;
                            timeSlot.classList.add("p-2");
                            timeSlot.classList.add(`Status-${hoursArray[i]}`);
                            (hoursArray[i] == 0) ? hoursArray[i] = 0.5 : hoursArray[i];
                            // Skip the next cells as they are merged
                            i = endTime;
                        } else {
                            timeSlot.textContent = '';
                            timeSlot.classList.add('shift');
                        }
                        employeeRow.appendChild(timeSlot);
                    }


                    employeeSchedulesContainer.appendChild(employeeRow);
                }
            }

            // Update the employee schedules table
            updateEmployeeSchedules();
        });

        // Event listener for the "Week" button
        $("#week").click(function () {
            x = 2;
            $('#monthcontainer').hide();
            $('#weekviewcontainer').show();
            $('#daycontainer').hide();

            if ($('#weekviewcontainer').length > 0) {
                Date.prototype.GetFirstDayOfWeek = function () {
                    return new Date(this.setDate(this.getDate() - this.getDay() + (this.getDay() == 0 ? -6 : 1)));
                };
                Date.prototype.getWeek = function () {
                    var onejan = new Date(this.getFullYear(), 0, 1);
                    return Math.ceil((((this - onejan) / 86400000) + onejan.getDay() + 1) / 7);
                };

                var nav = 0;
                var cnav = 0;
                var inc = 1;
                var dec = 1;
                var wnav = 0;
                var wlnav = 0;
                var nd = new Date();
                var ndate = nd.getDate();
                var nday = nd.getDay();
                var weekOfMonth = Math.ceil((ndate - 1 - nday) / 7);
                nav = weekOfMonth;
                var fdweek = nd.GetFirstDayOfWeek();
                var getweekdt = nd.GetFirstDayOfWeek();

                // Define click event handler for the previous button
                $('#calendar-prev').click(function () {
                    if (x == 2) {
                        if (nav == 5) {
                            wnav = 5;
                        }
                        nav--;
                        if (nav == -1) {
                            nav = 4;
                            fdweek.setDate(fdweek.getDate() - 5);
                            getweekdt.setDate(getweekdt.getDate() - 5);
                            inc = 0;
                            dec = 1;
                            cnav--;
                        } else {
                            fdweek.setDate(fdweek.getDate() - 12);
                            getweekdt.setDate(getweekdt.getDate() - 12);
                        }
                        loadWeekCalendar();
                    }
                    // Decrement week index

                });

                // Define click event handler for the next button
                $('#calendar-next').click(function () {
                    if (x == 2) {
                        nav++;
                        wlnav = 5;
                        if ($('thead th[data-week="5"]').length > 0) {
                            wlnav = 6;
                        }
                        if (nav == wlnav) {
                            nav = 0;
                            fdweek.setDate(fdweek.getDate() - 5);
                            getweekdt.setDate(getweekdt.getDate() - 5);
                            dec = 0;
                            inc = 1;
                            cnav++;
                        } else {
                            fdweek.setDate(fdweek.getDate() + 2);
                            getweekdt.setDate(getweekdt.getDate() + 2);
                            wnav = 5;
                        }
                        loadWeekCalendar();
                        wnav = 0;
                    }

                });

                // Define function to load the week calendar--------------------------WEEk-------------------------TABLE-----------------------------------------------
                function loadWeekCalendar() {
                    x = 2;
                    var tbody = document.querySelector('.main');
                    tbody.innerHTML = "";
                    // Check if tbody exists
                    if (tbody) {
                        // Add rows dynamically
                        for (var i = 0; i < providerList.length; i++) {
                            var row = document.createElement('tr');
                            for (var j = 0; j < 8; j++) {
                                var cell = document.createElement('td');
                                if (j === 0) {
                                    cell.classList.add("w-25");
                                    var image = document.createElement('img');
                                    image.classList.add("img-fluid");
                                    image.classList.add("ph-img");
                                    image.src = '/Upload/physician/' + providerList[i].id + '/' + 'photo.png'; // Set the image URL from ProviderData
                                    image.alt = providerList[i].name; // Set the alt text for accessibility
                                    cell.appendChild(image);
                                    cell.appendChild(document.createTextNode(providerList[i].name)); // Add text content after the image

                                    // First cell, add some content or leave it empty
                                    // cell.textContent = ProviderData[i].name; + (i + 1);
                                } else {
                                    // Other cells, you can add your content or leave them empty
                                    const currentDate = new Date();
                                    const cellDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate() + j - 3);
                                    // cell.textContent = fdweek.getDate() + j - 2;
                                    //cell.textContent += fdweek.getMonth() + 1;
                                    //cell.textContent += fdweek.getFullYear();
                                    //cell.textContent += cellDate;
                                    // console.log(fdweek);
                                    const matchingDates = providerList[i].dayList.filter(item => {
                                        const shiftdate = new Date(item.shiftdate);
                                        return fdweek.getDate() + j - 2 == shiftdate.getDate() && fdweek.getMonth() + 1 == shiftdate.getMonth() + 1 && fdweek.getFullYear() == shiftdate.getFullYear();
                                    });
                                    // If matching dates are found, count them and print the count in the cell
                                    if (matchingDates.length > 0) {
                                        if (matchingDates.length <= 2) {
                                            matchingDates.forEach((item, index) => {
                                                cell.classList.add('p-0');
                                                cell.innerHTML += `<div class="mt-1 d-block text-dark p-1 Status-${item.status} open-modal" data-modal-id="editShiftModal" data-shiftid = "${item.shiftid}" >  ${item.regionname} <br/> ${item.starttime} - ${item.endtime}</div>`;
                                            });
                                        } else {
                                            let buttonId = `viewAllButton_${i}${j}`;
                                            for (let i = 0; i < 2; i++) {
                                                const item = matchingDates[i];
                                                cell.classList.add('p-0');
                                                cell.innerHTML += `<div class="mt-1 d-block text-dark p-1 Status-${item.status} open-modal" data-modal-id="editShiftModal" data-shiftid = "${item.shiftid}" >  ${item.regionname} <br/> ${item.starttime} - ${item.endtime}</div>`;
                                            }
                                            const newdate = new Date(fdweek.getFullYear(), fdweek.getMonth() + 1, fdweek.getDate() + j - 2);

                                            cell.innerHTML += `<div type="button" id="${buttonId}" data-modal-id="#exampleModal"" data-shiftdate="${newdate}" data-index="${i}" class="btn btn-info w-100 text-white rounded-0 open-modal">View All</div>`;
                                        }
                                        const count = matchingDates.length;
                                    }
                                }
                                row.appendChild(cell);
                            }
                            tbody.appendChild(row);
                        }
                    }

                    // Above function to load the week calendar--------------------------WEEk-------------------------TABLE-----------------------------------------------

                    const formatDate = new Intl.DateTimeFormat("en", { day: "2-digit" });
                    const headwdate = new Intl.DateTimeFormat("en", { year: 'numeric', month: 'short', day: 'numeric' });
                    var options = { weekday: 'short', year: 'numeric', month: 'long', day: 'numeric' };
                    const dt = new Date();
                    if (cnav !== 0) { dt.setMonth(new Date().getMonth() + cnav); }
                    const year = dt.getFullYear();
                    const month = dt.getMonth();
                    const day = dt.getDate();
                    const week = dt.getWeek();
                    const firstDayOfMonth = new Date(year, month, 1).toLocaleDateString('en-US', options);
                    const lastDayOfMonth = new Date(year, month + 1, 0).toLocaleDateString('en-US', options);
                    var firstday = firstDayOfMonth.split(',')[0];
                    var lastday = lastDayOfMonth.split(',')[0];
                    var daytype = new Array('Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat');
                    var i = daytype.indexOf(firstday);
                    var j = daytype.indexOf(lastday) + 1;
                    var removefirst = daytype.slice(0, i);
                    var removelast = daytype.slice(j, daytype.length);
                    $('td,th').css("background-color", "");
                    const firstweek = new Date(year, month, 1).getWeek();
                    const lastweek = new Date(year, month + 1, 0).getWeek();

                    var used = new Date(year, month, 1).getDay() + new Date(year, month + 1, 0).getDate();
                    var numofweeks = Math.ceil(used / 7);

                    removefirst.forEach(removefirstcol);
                    removelast.forEach(removelastcol);
                    function removefirstcol(item, index) { $('td[data-week="0"][data-name="' + item + '"').css({ "background-image": "none", "background-color": "#c9c9c9", "cursor": "not-allowed" }); }
                    function removelastcol(item, index) {
                        if ($('thead th[data-week="5"]').length > 0) {
                            $('td[data-week="5"][data-name="' + item + '"').css({ "background-image": "none", "background-color": "#c9c9c9", "cursor": "not-allowed" });
                        } else {
                            $('td[data-week="4"][data-name="' + item + '"').css({ "background-image": "none", "background-color": "#c9c9c9", "cursor": "not-allowed" });
                        }
                    }
                    if (nav === 4 && $('thead th[data-week="5"]').length > 0 && wnav != 5) { nav = 5; }
                    $('th[data-week="' + nav + '"').css({ "background-image": "none", "background-color": "#fff" });
                    $('td[data-week="' + nav + '"').css({ "background-image": "none", "background-color": "#fff" });
                    if (removelast.length === 0) {
                        if (inc === 0) {
                            fdweek.setDate(fdweek.getDate() - 7);
                            getweekdt.setDate(getweekdt.getDate() - 7);
                            inc++;
                        }
                    }
                    if (removefirst.length === 0) {
                        if (dec === 0) {
                            fdweek.setDate(fdweek.getDate() + 7);
                            getweekdt.setDate(getweekdt.getDate() + 7);
                            dec++;
                        }
                    }

                    const fweekdt = headwdate.format(getweekdt.setDate(getweekdt.getDate() - 1));
                    console.log(fweekdt);
                    const lweekdt = headwdate.format(getweekdt.setDate(getweekdt.getDate() + 6));
                    var fmondt = fweekdt.split(' ')[0];
                    var lmondt = lweekdt.split(' ')[0];
                    var fdaydt = fweekdt.split(' ')[1];
                    fdaydt = fdaydt.split(',')[0];
                    var ldaydt = lweekdt.split(' ')[1];
                    var fyeardt = fweekdt.split(',')[1];
                    var lyeardt = lweekdt.split(',')[1];
                    if (fyeardt == lyeardt && fmondt == lmondt) { $('.calendar-current-date').html(fmondt + " " + fdaydt + " - " + ldaydt + fyeardt); }
                    if (fyeardt == lyeardt && fmondt != lmondt) { $('.calendar-current-date').html(fmondt + " " + fdaydt + " - " + lmondt + " " + ldaydt + fyeardt); }
                    if (fyeardt != lyeardt && fmondt != lmondt) { $('.calendar-current-date').html(fweekdt + " - " + lweekdt); }
                    $('.title[data-name="Sun"').html("Sun - " + formatDate.format(fdweek.setDate(fdweek.getDate() - 1)));
                    $('.title[data-name="Mon"').html("Mon " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                    $('.title[data-name="Tue"').html("Tue " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                    $('.title[data-name="Wed"').html("Wed " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                    $('.title[data-name="Thu"').html("Thu " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                    $('.title[data-name="Fri"').html("Fri " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                    $('.title[data-name="Sat"').html("Sat " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                }

                // Initial load of the week calendar
                loadWeekCalendar();
            }
        });
    }





    // Event listener for the "Month" button
    $("#month").click(function () {
        $('#monthcontainer').show();
        $('#weekviewcontainer').hide();
        $('#daycontainer').hide();
        x = 1
        manipulate();
    });

    $('#fregion').on('change', function () {
        if (x == 1) {
            GetShiftForMonth(month + 1, year);
        } else if (x == 2 || x == 3) {
            getProviderList();
        } else {
        }
    });

});


/* function for get one Shift data and return it  */
function getShiftData(shiftid) {
    var shiftdata;
    $.ajax({
        url: '/Admin/getShiftData',
        type: "GET",
        data: { shiftid },
        async: false,
        success: function (data) {
            shiftdata = data;
        },
        error: function () {
            alert("Error while fetching Shift");
        }
    });
    return shiftdata;
}
