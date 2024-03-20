$(document).ready(function () {
    var _URL = window.URL || window.webkitURL;

    $("#photofile").change(function (e) {
        debugger
        var image, file;
        if ((file = this.files[0])) {
            image = new Image();
            image.onload = function () {
                src = this.src;
                $('#uploadPreview').html('<img class="w-100 h-100" src="' + src + '"></div>');
                e.preventDefault();
            }
        };
        image.src = _URL.createObjectURL(file);
    });


    $('.fileInput').on('change', function () {
        var file = $(this)[0].files[0];
        var $viewButton = $(this).closest('.inputWrapper').siblings('.view-button');
        console.log($viewButton);

        if (file) {
            $viewButton.show();
        }
    });


    $('#create-signature-button').on('click', function () {
        $('#drawModal').modal('show');
    });

    var drawCanvas = $('#drawCanvas')[0];
    var drawCtx = drawCanvas.getContext('2d');
    var drawing = false;
    var signatureFileInput = $('#signature');

    // Event listeners for drawing on the canvas
    $('#drawCanvas').on('mousedown', function (e) {
        drawing = true;
        drawCtx.beginPath();
        drawCtx.moveTo(e.clientX - drawCanvas.getBoundingClientRect().left, e.clientY - drawCanvas.getBoundingClientRect().top);
    });

    $('#drawCanvas').on('mousemove', function (e) {
        if (drawing) {
            drawCtx.lineTo(e.clientX - drawCanvas.getBoundingClientRect().left, e.clientY - drawCanvas.getBoundingClientRect().top);
            drawCtx.stroke();
        }
    });

    $('#drawCanvas').on('mouseup', function () {
        drawing = false;
    });

    // Event listener for clearing the canvas
    $('#clearDrawingBtn').on('click', function () {
        drawCtx.clearRect(0, 0, drawCanvas.width, drawCanvas.height);
    });



    // Event listener for saving the drawn image
    $('#saveDrawingBtn').on('click', function () {
        var dataURL = drawCanvas.toDataURL("image/png");
        $('#saved').show();
        $('#saved').attr('src', dataURL);
        var blob = dataURLToBlob(dataURL);
        var file = new File([blob], "signature.png", { type: "image/png" });
        var dataTransfer = new DataTransfer();
        dataTransfer.items.add(file);
        signatureFileInput.prop('files', dataTransfer.files);
        $('#signature-data').val(dataURL); // Set the value of the hidden input field
        $('#drawModal').modal('hide');
    });

    function dataURLToBlob(dataURL) {
        var byteString = atob(dataURL.split(',')[1]);
        var ab = new ArrayBuffer(byteString.length);
        var ia = new Uint8Array(ab);
        for (var i = 0; i < byteString.length; i++) {
            ia[i] = byteString.charCodeAt(i);
        }
        return new Blob([ab], { type: 'image/png' });
    }





    $('.agreement-checkbox').on('change', function () {
        var isChecked = $(this).prop('checked');
        var $fileInput = $(this).closest('.document').find('.fileInput');
        $fileInput.prop('disabled', !isChecked);
    });

    $("form").validate({
        rules: {
            PhotoFile: {
                required: true
            },
            SignatureFile: {
                required: true
            }
        },
        messages: {
            PhotoFile: {
                required: "Please upload a photo."
            },
            SignatureFile: {
                required: "Please upload a Singature"
            }
        }
    });


    const form = $('form');
    form.on('submit', function (event) {
        const checkboxes = $('.agreement-checkbox');
        checkboxes.each(function () {
            // Check if the checkbox is checked
            if ($(this).is(':checked')) {
                const fileInput = $(this).closest('.document').find('input[type="file"]');
                if (!fileInput[0].files.length) {
                    event.preventDefault();
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: 'You have selected a checkbox but did not provide a file!',
                    });
                    return false;
                }
            }
        });
    });


});

