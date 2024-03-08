// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
let anchartag = Array.from(document.getElementsByTagName("a"));

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
        
    } else {
        document.documentElement.setAttribute('data-bs-theme', 'dark');
        theme_mode_button.classList.remove('fa-moon');
        theme_mode_button.classList.add('fa-sun');
        anchartag.forEach(element => {
            element.classList.add("text-white");
        });
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
        debugger
        var regionId = document.getElementById("selectregion").value;
        getPhysicians(regionId, "physicianSelect");
    })

    $('#selectregionTransfer').on('change', function () {
        debugger
        var regionId = document.getElementById("selectregionTransfer").value;
        getPhysicians(regionId, "physicianSelectTransfer");
    })

    function getPhysicians(regionId, physiciandropId) {
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
            },
            error: function () {
                console.error('Failed to fetch physicians.');
            }
        });
    }
})


