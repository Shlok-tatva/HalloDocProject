/*provider On Call*/
$(document).ready(function () {

    //Provider On Call Js 
    $('#regionProviderOncall').on('change', function () {
        debugger;
        var regionId = $(this).val();
        $.ajax({
            type: "GET",
            url: 'ProviderOnCall?regionId=' + regionId,
            cache: false,
            success: function (response) {
                $(".oncalldiv").empty();
                $(".offcalldiv").empty();

                console.log(response);
                response.forEach(function (item) {
                    var div = document.createElement('div');
                    div.className = 'col-md-4 my-2';
                    var img = document.createElement('img');
                    img.src = '/Upload/physician/' + item.providerId + '/photo.png';
                    img.classList.add("ph-img");
                    var span = document.createElement('span');
                    span.textContent = item.firstName + ' ' + item.lastName;
                    div.appendChild(img);
                    div.appendChild(span);

                    if (item.onCallStatus === 1) {
                        $(".oncalldiv").append(div);
                    } else if (item.onCallStatus === null) {
                        $(".offcalldiv").append(div);
                    }
                });
            },
            error: function (error) {
                console.log(error);
                alert("Error while checking email.");
            }
        });
    })

    $('#shiftSubmit').prop('disabled', true);


    /* Existing shift Found or not */
    $('#EndTime').on('change', function () {
        checkExistingShifts();
    });

    $('#physicianSelect').on('change', function () {
        debugger;
        var selectedDate = $('#StartDate').val();
        var selectedStartTime = $('#StartTime').val();
        var selectedEndTime = $('#EndTime').val();
        if (selectedDate.length != 0 && selectedEndTime.length != 0 && selectedStartTime.length != 0) {
            checkExistingShifts()
        }
    })

    $('#StartDate').on('change', function () {
        debugger
        var physicianId = $('#physicianSelect').val();
        var selectedStartTime = $('#StartTime').val();
        var selectedEndTime = $('#EndTime').val();
        if (physicianId.length != 0 && selectedEndTime.length != 0 && selectedStartTime.length != 0) {
            checkExistingShifts()
        }
    })

    function checkExistingShifts() {

        var physicianId = $('#physicianSelect').val();
        var selectedDate = $('#StartDate').val();
        var selectedStartTime = $('#StartTime').val();
        var selectedEndTime = $('#EndTime').val();

        $.ajax({
            url: '/Admin/CheckExistingShifts',
            type: 'GET',
            data: {
                physicianId: physicianId,
                date: selectedDate,
                startTime: selectedStartTime,
                endTime: selectedEndTime
            },
            success: function (result) {
                if (result) {
                    // There are existing shifts
                    $('#shiftavlmsg').hide();
                    $('#shiftunavlmsg').show();
                    showToaster('Existing shifts found.', "error");
                    $('#shiftSubmit').prop('disabled', true);
                } else {
                    // There are no existing shifts
                    $('#shiftavlmsg').show();
                    $('#shiftunavlmsg').hide();
                    $('#shiftSubmit').prop('disabled', false);
                }
            },
            error: function () {
                console.error('Error occurred while checking existing shifts.');
                $('#shiftSubmit').prop('disabled', true);
            }
        });
    }

})




var today = new Date();
var formattedDate = today.toISOString().split('T')[0];


/* Create Shift Logic */
document.getElementById('StartDate').min = formattedDate;

addEndTimeChangeListener('StartTime', 'EndTime');


window.onload = toggleCheckboxes;
function toggleCheckboxes() {
    var repeatCheckbox = document.getElementById('Isrepeat');
    var refile = document.getElementById('Refile');
    var checkboxes = document.querySelectorAll('.Every');

    if (!repeatCheckbox.checked) {
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
            checkbox.disabled = true;
            refile.disabled = true;
        });
    } else {
        checkboxes.forEach(function (checkbox) {
            checkbox.disabled = false;
            refile.disabled = false;
        });
    }
}
function menubox() {
    event.preventDefault();
    let checkboxes = document.querySelectorAll('.Every:checked');
    let repeatDays = [];
    checkboxes.forEach((checkbox) => {
        repeatDays.push(checkbox.value);
    });
    document.querySelector('#checkWeekday').value = repeatDays.join(',');
    console.log(document.querySelector('#checkWeekday').value);
};



/* Edit Shift Logic */
function updateShiftData() {
    var shiftid = $("#shiftid").val();
    $.ajax({
        type: "POST",
        url: '/Admin/UpdateshiftStatus?shiftId=' + shiftid,
        cache: false,
        success: function (response) {
            location.reload();
        },
        error: function () {
            showToaster("Error while change status", "error");
        }
    });
}

function deleteshift() {
    debugger;
    var shiftid = $("#shiftid").val();
    $.ajax({
        type: "POST",
        url: '/Admin/DeleteShift?shiftid=' + shiftid,
        cache: false,
        success: function (response) {
            location.reload();
        },
        error: function () {
            alert("Error while checking email.");
        }
    });
}


document.getElementById('delete').addEventListener('click', (event) => {
    event.preventDefault();
    Swal.fire({
        icon: "warning",
        title: "Do you want delete shift ?",
        showDenyButton: false,
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#01BBE7",
        confirmButtonText: "Yes",
        cancelButtonText: `Don't delete`
    }).then((result) => {
        debugger;
        if (result.isConfirmed) {
            deleteshift();
        }
    });
});


document.getElementById('StartDateEdit').min = formattedDate;
document.getElementById('StartTimeEdit').addEventListener('change', function () {
    var startTime = this.value;
    document.getElementById('EndTimeEdit').min = startTime;
    if (document.getElementById('EndTimeEdit').value < startTime) {
        document.getElementById('EndTimeEdit').value = startTime;
    }
});

document.getElementById('EndTimeEdit').addEventListener('change', function () {
    debugger;
    const startTimeInput = document.getElementById('StartTimeEdit');
    const endTimeInput = document.getElementById('EndTimeEdit');
    const startTime = startTimeInput.value; // Get the value of start time
    const endTime = endTimeInput.value; // Get the value of end time

    // Convert start and end time strings to Date objects for comparison
    const startDate = new Date('2000-01-01T' + startTime + ':00');
    const endDate = new Date('2000-01-01T' + endTime + ':00');

    // Check if end time is earlier than start time
    if (endDate < startDate) {
        Swal.fire({
            icon: "error",
            title: "Wrong End time...",
            text: "End time cannot be earlier than start time!"
        });
        endTimeInput.value = ''; // Clear the end time input
    }
});

// Usage example:
addEndTimeChangeListener('StartTimeEdit', 'EndTimeEdit');

function toggleDisabled() {
    var StartDate = document.getElementById('StartDateEdit');
    var StartTime = document.getElementById('StartTimeEdit');
    var EndTime = document.getElementById('EndTimeEdit');
    var save = document.getElementById('save');
    var edit = document.getElementById('edit');

    if (StartDate.disabled) {
        // Show the Save button and enable input fields
        save.style.display = 'inline-block';
        edit.style.display = 'none';
        StartDate.disabled = false;
        StartTime.disabled = false;
        EndTime.disabled = false;
    } else {
        // Hide the Save button and disable input fields
        edit.style.display = 'inline-block';
        save.style.display = 'none';
        StartDate.disabled = true;
        StartTime.disabled = true;
        EndTime.disabled = true;
    }
}


function addEndTimeChangeListener(startTimeId, endTimeId) {
    const startTimeInput = document.getElementById(startTimeId);
    const endTimeInput = document.getElementById(endTimeId);

    // Listener for start time change
    startTimeInput.addEventListener('change', function () {
        if (endTimeInput.value !== '') {
            validateTime(startTimeInput, endTimeInput);
        }
    });

    // Listener for end time change
    endTimeInput.addEventListener('change', function () {
        validateTime(startTimeInput, endTimeInput);
    });
}

function validateTime(startTimeInput, endTimeInput) {
    const startTime = startTimeInput.value;
    const endTime = endTimeInput.value;

    if (startTime === '' || endTime === '') {
        return;
    }

    const startParts = startTime.split(':');
    const endParts = endTime.split(':');
    const startDate = new Date(2000, 0, 1, startParts[0], startParts[1], startParts[2] || 0);
    const endDate = new Date(2000, 0, 1, endParts[0], endParts[1], endParts[2] || 0);

    // Check if end time is earlier than start time
    if (endDate < startDate) {
        Swal.fire({
            icon: "error",
            title: "Wrong End time...",
            text: "End time cannot be earlier than start time!"
        });
        endTimeInput.value = '';
        $('#shiftSubmit').prop('disabled', true);

    }
}




