// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.getElementById('btnSwitch').addEventListener('click', () => {
    let theme_mode_button = document.getElementById("theme_mode_button");
    let currentTheme = document.documentElement.getAttribute('data-bs-theme');

    if (currentTheme == 'dark') {
        document.documentElement.setAttribute('data-bs-theme', 'light');
        theme_mode_button.classList.add('fa-moon');
        theme_mode_button.classList.remove('fa-sun');
        
    } else {
        document.documentElement.setAttribute('data-bs-theme', 'dark');
        theme_mode_button.classList.remove('fa-moon');
        theme_mode_button.classList.add('fa-sun');

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