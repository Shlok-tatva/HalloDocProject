document.getElementById('agreeButton').addEventListener('click', function (event) {

    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: "btn btn-info mx-2 btn-lg text-white my-2 mb-2",
            cancelButton: "btn btn-outline-info btn-lg hover_white"
        },
        buttonsStyling: false
    });


    event.preventDefault(); // Prevent default form submission behavior
    debugger;

    swalWithBootstrapButtons.fire({
        title: "Are you sure you want to agree?",
        text: "Once you agree, you cannot undo this action!",
        icon: "warning",
        buttons: true,
        showCancelButton: true,
        dangerMode: true,
    }).then((willAgree) => {
        if (willAgree.isConfirmed) {
            document.getElementById('agreeForm').submit();
        }
    });
});

document.getElementById('cancelButton').addEventListener('click', function (event) {
    event.preventDefault(); // Prevent default form submission behavior
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: "btn btn-info mx-2 btn-lg text-white my-2 mb-2",
            cancelButton: "btn btn-outline-info btn-lg hover_white"
        },
        buttonsStyling: false
    });


    swalWithBootstrapButtons.fire({
        title: "Are you sure you want to cancel?",
        text: "Once you cancel, you cannot undo this action!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Okay",
        cancelButtonText: "Cancel",
        input: 'text',
        inputPlaceholder: 'Enter cancellation note'
    }).then((result) => {
        if (result.isConfirmed) {
            let cancellationNote = result.value;
            // Append cancellation note to the cancel form
            let cancelForm = document.getElementById('cancelForm');
            let noteInput = document.createElement('input');
            noteInput.type = 'hidden';
            noteInput.name = 'cancellationNote';
            noteInput.value = cancellationNote;
            cancelForm.appendChild(noteInput);
            // Submit the form
            cancelForm.submit();
        }
    });
});