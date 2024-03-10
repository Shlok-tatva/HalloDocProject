
$(document).ready(function () {
        $("#btnGeneratePDF").click(function () {
            var requestId = $("#requestIdencounter").val(); // Assuming you have an input field with ID requestId to input the request ID
            $.ajax({
                type: "GET",
                url: "/admin/GetEncounterFormDetails",
                data: { requestId: requestId },
                success: function (data) {
                    // Dynamically generate HTML content
              
                    const { jsPDF } = window.jspdf;
                    console.log(data)

                    var htmlContent = `
                        <div>
                            <h2>Encounter Form Details</h2>
                            <p>First Name: ${data.firstName}</p>
                            <p>Last Name: ${data.lastName}</p>
                            <p>Location: ${data.location}</p>
                            <p>Date of Birth: ${data.dateOfBirth}</p>
                            <p>Date of Request: ${data.dateOfRequest}</p>
                            <p>Phone: ${data.phone}</p>
                            <p>Email: ${data.email}</p>
                            <p>History of Present Illness or Injury: ${data.historyOfPresentIllnessOrInjury}</p>
                            <p>Medical History: ${data.medicalHistory}</p>
                            <p>Medications: ${data.medications}</p>
                            <p>Allergies: ${data.allergies}</p>
                            <!-- Include other fields as needed -->
                        </div>
                    `;

                    // Generate PDF
                    var pdf = new jsPDF();
                    pdf.html(htmlContent, {
                        callback: function () {
                            pdf.save("encounter_form.pdf");
                        }
                    });
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        });
    });
