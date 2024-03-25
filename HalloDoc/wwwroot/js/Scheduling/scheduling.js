document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');

    var shifts = @Html.Raw(Json.Serialize(Model));

    var events = shifts.shifts.map(function (shift) {
        return {
            title: 'Shift',
            start: new Date(shift.startDate),
            backgroundColor: shift.isPending ? 'red' : 'green',
            // display: 'background',
            borderColor: 'white',
        };
    });

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        events: events,
        eventClick: function (info) {
            $('#shiftDetailsModal').modal('show');
            $('#shiftDetailsModalTitle').text('Shift Details');
            $('#shiftDetailsModalBody').html('<p>Start Date: ' + info.event.start + '</p><p>Status: ' + (info.event.backgroundColor === 'red' ? 'Pending' : 'Approved') + '</p>');
        },
        views: {
            timeGridWeek: {
                type: 'timeGridWeek',
                columnHeaderFormat: { weekday: 'long' }
            }
        },
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'timeGridWeek,dayGridMonth,timeGridDay'
        },
        eventContent: function (arg) {
            var eventEl = document.createElement('div');
            eventEl.innerText = arg.event.title;
            return { domNodes: [eventEl] };
        },
        slotLabelFormat: {
            hour: 'numeric',
            minute: '2-digit',
            omitZeroMinute: false,
            meridiem: 'short'
        },
        slotLabelContent: function (arg) {
            var providerImage = '';
            var matchingShifts = events.filter(function (event) {
                return event.start.getDate() === arg.date.getDate();
            });

            if (matchingShifts.length > 0) {
                var providers = [];
                matchingShifts.forEach(function (shift) {
                    if (!providers.includes(shift.providerId)) {
                        providers.push(shift.providerId);
                        providerImage += '<div class="provider-image"><img src="' + shift.imagePath + '" alt="' + shift.title + '"></div>';
                    }
                });
            }

            return providerImage;
        }

    });

    calendar.render();

    $('#monthButton').click(function () {
        calendar.changeView('dayGridMonth');
    });

    $('#dateButton').click(function () {
        calendar.changeView('timeGridDay');
    });

    $('#weekButton').click(function () {
        calendar.changeView('timeGridWeek');
    });
});