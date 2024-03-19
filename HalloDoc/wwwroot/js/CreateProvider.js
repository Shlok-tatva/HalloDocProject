$(document).ready(function () {
    $('.agreement-checkbox').on('change', function () {
        var isChecked = $(this).prop('checked');
        var $fileInput = $(this).closest('.document').find('.fileInput');

        console.log($fileInput);

        $fileInput.prop('disabled', !isChecked);
    });


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
            $fileDetails.find('.file-name').text(file.name);
        }
    });




    var canvas = $('#signature-pad')[0];
    var ctx = canvas.getContext('2d');
    var drawing = false;
    var strokes = [];
    var saveButton = $('#save-button');
    var signatureFileInput = $('#signature');


    $('#create-signature-button').on('click', function () {
        $('#create-signature-button').hide();
        $('#clear-button').show();
        $('#undo-button').show();
        $('#save-button').show();

        canvas.style.display = 'block';
    });

    $('#signature-pad').on('mousedown', function (e) {
        drawing = true;
        ctx.beginPath();
        ctx.moveTo(e.clientX - canvas.getBoundingClientRect().left, e.clientY - canvas.getBoundingClientRect().top);
    });

    $('#signature-pad').on('mousemove', function (e) {
        if (drawing) {
            ctx.lineTo(e.clientX - canvas.getBoundingClientRect().left, e.clientY - canvas.getBoundingClientRect().top);
            ctx.stroke();
        }
    });

    $('#signature-pad').on('mouseup', function () {
        if (drawing) {
            drawing = false;
            strokes.push(ctx.getImageData(0, 0, canvas.width, canvas.height));
            updateSaveButtonState();
        }
    });

    $('#clear-button').on('click', function () {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        strokes = [];
        updateSaveButtonState();
    });

    $('#save-button').on('click', function () {
        $('#saved').show();
        scaleUp(canvas, 1);
        var dataURL = canvas.toDataURL("image/png");
        $('#signature-data').val(dataURL); // Set the value of the hidden input field
        // $('#save-form').submit(); // Submit the form
    });

    function updateSignatureFileInput() {
        if (strokes.length > 0) {
            var dataURL = canvas.toDataURL("image/png");
            var blob = dataURLToBlob(dataURL);
            var file = new File([blob], "signature.png", { type: "image/png" });
            signatureFileInput.prop('files', [file]);
            saveButton.prop('disabled', false);
        } else {
            saveButton.prop('disabled', true);
        }
    }

    function dataURLToBlob(dataURL) {
        var byteString = atob(dataURL.split(',')[1]);
        var ab = new ArrayBuffer(byteString.length);
        var ia = new Uint8Array(ab);
        for (var i = 0; i < byteString.length; i++) {
            ia[i] = byteString.charCodeAt(i);
        }
        return new Blob([ab], { type: 'image/png' });
    }

    function updateSaveButtonState() {
        if (strokes.length > 0) {
            var dataURL = canvas.toDataURL("image/png");
            var blob = dataURLToBlob(dataURL);
            var file = new File([blob], "signature.png", { type: "image/png" });
            var dataTransfer = new DataTransfer();
            dataTransfer.items.add(file);
            signatureFileInput.prop('files', dataTransfer.files);
            saveButton.prop('disabled', false);
        } else {
            saveButton.prop('disabled', true);
        }
    }

    function scaleUp(c, scale) {

        var newCanvas = $("<canvas>")
            .attr("width", c.width * scale)
            .attr("height", c.height * scale)[0];

        var ctx2 = newCanvas.getContext("2d");
        ctx2.imageSmoothingEnabled = false;
        ctx2.drawImage(c, 0, 0, newCanvas.width, newCanvas.height);

        var dataURL = newCanvas.toDataURL("image/png");
        document.getElementById("saved").src = dataURL;

    }

});

