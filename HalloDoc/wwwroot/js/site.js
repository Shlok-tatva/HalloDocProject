// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
let anchartag = Array.from(document.getElementsByTagName("a"));
let blackelement = Array.from(document.getElementsByClassName("text-black"));
let bgbwhiteelement = Array.from(document.getElementsByClassName("bg-white"));

document.getElementById('btnSwitch').addEventListener('click', () => {
    let theme_mode_button = document.getElementById("theme_mode_button");
    let currentTheme = document.documentElement.getAttribute('data-bs-theme');

    console.log(anchartag);

   

    if (currentTheme == 'dark') {
        document.documentElement.setAttribute('data-bs-theme', 'light');
        theme_mode_button.classList.add('fa-moon');
        theme_mode_button.classList.remove('fa-sun');

        anchartag.forEach(element => {
            element.classList.remove("text-white");
        });
        blackelement.forEach(element => {
            element.classList.remove("text-white")
        })
        bgbwhiteelement.forEach(element => {
            element.classList.remove("bg-dark");
            element.classList.add("bg-white");
        })
        
    } else {
        document.documentElement.setAttribute('data-bs-theme', 'dark');
        theme_mode_button.classList.remove('fa-moon');
        theme_mode_button.classList.add('fa-sun');
        anchartag.forEach(element => {
            element.classList.add("text-white");
        });
        blackelement.forEach(element => {
            element.classList.add("text-white")
        })
        bgbwhiteelement.forEach(element => {
            element.classList.remove("bg-white");
            element.classList.add("bg-dark");
        })
    }

    // Save the current theme mode to localStorage
    localStorage.setItem('themeMode', document.documentElement.getAttribute('data-bs-theme'));

    // Save the current state of the theme mode button to localStorage
    localStorage.setItem('themeModeButtonState', theme_mode_button.classList.contains('fa-moon') ? 'light' : 'dark');

});

// Retrieve and apply the stored theme mode and theme mode button state on page load
document.addEventListener('DOMContentLoaded', () => {
    let storedTheme = localStorage.getItem('themeMode');
    let storedButtonState = localStorage.getItem('themeModeButtonState');

    if (storedTheme) {
        document.documentElement.setAttribute('data-bs-theme', storedTheme);
    }

    // Apply the stored theme mode button state
    let theme_mode_button = document.getElementById("theme_mode_button");
    if (storedButtonState === 'light') {
        theme_mode_button.classList.add('fa-moon');
        theme_mode_button.classList.remove('fa-sun');
    } else {
        theme_mode_button.classList.remove('fa-moon');
        theme_mode_button.classList.add('fa-sun');
        anchartag.forEach(element => {
            element.classList.add("text-white");
        });
        blackelement.forEach(element => {
            element.classList.add("text-white")
        })
        bgbwhiteelement.forEach(element => {
            element.classList.remove("bg-white");
            element.classList.add("bg-dark");
        })
    }
});

const initializeIntlTelInput = (input) => {
    window.intlTelInput(input, {
        initialCountry: "auto",
        geoIpLookup: callback => {
            fetch("https://ipapi.co/json")
                .then(res => res.json())
                .then(data => callback(data.country_code))
                .catch(() => callback("us"));
        },
        utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/14.0.1/js/utils.js"
    });
};

const input = document.querySelector("#phone");
const input2 = document.getElementById("phone1"); // Use getElementById instead of querySelector

if(input) {
initializeIntlTelInput(input);
}
if(input2) { // Check if input2 is not null
    initializeIntlTelInput(input2); // Run the function only if input2 exists
}

let inputFile = document.getElementById("inputFile");
let fileNameInput = document.getElementById('fileNameInput');

if (inputFile) {

inputFile.addEventListener("change", function () {
    // Get the file name from the files property
    let fileName = inputFile.files[0].name;
    // Get the paragraph element by query selector
    let fileLabel = document.querySelector("label[for=inputFile] p");
    // Update the text content with the file name
    fileLabel.textContent = fileName;

    fileNameInput.value = fileName;

});
}

function getRequest(requestId) {
    var requestdata;
    $.ajax({
        url: '/Admin/GetRequestClient',
        type: "GET",
        data: { requestId: requestId },
        async: false, // synchronous request to ensure data is returned before proceding 
        success: function (data) {
            requestdata = data;
        },
        error: function (e) {
            alert("Error while fetching Data");
        }
    });
    return requestdata;
}

$(document).ready(function () {
    $('#selectregion').on('change', function () {
        var regionId = document.getElementById("selectregion").value;
        getPhysicians(regionId, "physicianSelect");
    })

    $('#selectregionTransfer').on('change', function () {
        var regionId = document.getElementById("selectregionTransfer").value;
        getPhysicians(regionId, "physicianSelectTransfer");
    })

    $('#regioneditshift').on('change', function () {
        var regionId = document.getElementById("regioneditshift").value;
        getPhysicians(regionId, "phyiscianeditshift");
    })

   
})


/* Initial Function that do Pagination */
function setupPagination(items , className) {
    const itemsPerPage = items;
    let currentPage = 1;
    let totalPages = Math.ceil($(className).length / itemsPerPage);
    const maxPageButtons = 3;
    let startPage = 1;

    function showRowsForPage(page) {
        const startIndex = (page - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;
        $(className).hide();
        $(className).slice(startIndex, endIndex).show();
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



/* For Table Set the pagination */
function setupPaginationDesktop() {
    setupPagination(10, ".data-row");
}

/* For accordian set the pagination */
function setupPaginationMobile() {
    setupPagination(5, ".data-row-mobile");
}

/* Check Window Size and return true false */
function isMobileWindowSize() {
    return window.innerWidth <= 900; 
}

/* now Run only that function based on the window size */
function setupPaginationBasedOnDevice() {
    if (isMobileWindowSize()) {
        setupPaginationMobile();
    } else {
        setupPaginationDesktop();
    }
}

/* On window Resize run that function so that it will do dynamic pagination based on that */
window.addEventListener('resize', setupPaginationBasedOnDevice);


function exportDataToCSV(arrayOfObjects, filename) {
    const csv = [];

    const headers = Object.keys(arrayOfObjects[0]).filter(header => header !== "menuOptions");
    csv.push(headers.join(','));

    // Extract values for each object
    arrayOfObjects.forEach(obj => {
        const values = headers.map(header => {
            // Fix the date of birth format
            if (header === "dateOfBirth") {
                return obj[header].replace(/,/g, ''); // Remove commas
            } else {
                return obj[header];
            }
        });
        csv.push(values.join(','));
    });

    // Download CSV file
    downloadCSV(csv.join('\n'), filename);
}

function downloadCSV(csv, filename) {
    const csvFile = new Blob([csv], { type: 'text/csv' });
    const downloadLink = document.createElement('a');
    downloadLink.download = filename;
    downloadLink.href = window.URL.createObjectURL(csvFile);
    downloadLink.style.display = 'none';
    document.body.appendChild(downloadLink);
    downloadLink.click();
}


function showToaster(msg , icon) {
    $(document).ready(function () {
        toastMixin.fire({
            animation: true,
            title: `${msg}`,
            icon: `${icon}`
        });
    })

    var toastMixin = Swal.mixin({
        toast: true,
        icon: 'success',
        title: 'General Title',
        animation: false,
        position: 'top-right',
        showConfirmButton: false,
        timer: 2000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });
}

function getPhysicians(regionId, physiciandropId, callback) {
    $.ajax({
        url: '/Admin/GetPhysiciansByRegion', // Replace with your actual controller and action method
        type: 'GET',
        data: { regionId: regionId },
        success: function (data) {
            // Update the physician select dropdown with fetched data
            console.log(data);
            var physicianDropdown = document.getElementById(physiciandropId);
            physicianDropdown.innerHTML = '';
            data.forEach(function (physician) {
                var option = document.createElement("option");
                option.value = physician.id;
                option.text = physician.name;
                physicianDropdown.appendChild(option);
            });

            if (typeof callback === 'function') {
                callback();
            }

        },
        error: function () {
            console.error('Failed to fetch physicians.');
        }
    });
}



$(document).ready(function () {
    $('#navBartoggleBtn').click(function () {
        $('.mobile_nav').toggleClass('show-nav');
    });
});
