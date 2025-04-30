/**
 * Client Form Validation
 * Handles client-side validation for Add Client and Edit Client forms
 * Self-initializing script that runs immediately and also sets up event listeners
 */

// Function to set up form validation
function setupClientFormValidation() {
    console.log("Setting up client form validation");
    
    // Function to validate a form
    function validateForm(form) {
        if (!form.checkValidity()) {
            console.log("Form validation failed");
            
            // Highlight invalid fields
            form.querySelectorAll(":invalid").forEach((field) => {
                field.style.borderColor = "red";
                
                // Add error class to parent field-group
                const fieldGroup = field.closest(".field-group");
                if (fieldGroup) {
                    fieldGroup.classList.add("has-error");
                }
            });
            
            // Show validation message at the top of the form
            if (!form.querySelector(".form-error-message")) {
                const errorMessage = document.createElement("div");
                errorMessage.className = "alert alert-danger form-error-message";
                errorMessage.innerHTML = "<span>Please fix the form errors before submitting.</span>";
                form.prepend(errorMessage);
                
                // Automatically remove after 5 seconds
                setTimeout(() => {
                    errorMessage.remove();
                }, 5000);
            }
            
            return false;
        }
        
        return true;
    }
    
    // Function to reset validation styling
    function resetValidationStyling(input) {
        input.style.borderColor = "";
        
        // Remove error class from parent field-group
        const fieldGroup = input.closest(".field-group");
        if (fieldGroup) {
            fieldGroup.classList.remove("has-error");
        }
    }
    
    // Function to set up a specific form
    function setupForm(formSelector, formName) {
        const form = document.querySelector(formSelector);
        if (form) {
            console.log(`${formName} form found, setting up validation`);
            
            // Remove any existing event listeners (to prevent duplicates)
            const newForm = form.cloneNode(true);
            form.parentNode.replaceChild(newForm, form);
            
            // Prevent submission if form is invalid
            newForm.addEventListener("submit", function(e) {
                if (!validateForm(this)) {
                    e.preventDefault();
                    e.stopPropagation(); // Prevent modal from closing
                } else {
                    console.log(`${formName} form is valid, submitting...`);
                }
            });
            
            // Reset validation styling on input
            newForm.querySelectorAll("input, select, textarea").forEach((input) => {
                input.addEventListener("input", function() {
                    resetValidationStyling(this);
                });
            });
            
            return newForm;
        }
        return null;
    }
    
    // Set up both forms
    setupForm("#addClientModal form", "Add Client");
    setupForm("#editClientModal form", "Edit Client");
}

// Run immediately
setupClientFormValidation();

// Also set up when DOM is loaded (in case script loads before DOM)
document.addEventListener("DOMContentLoaded", setupClientFormValidation);

// Set up when modals are opened
document.addEventListener("click", function(e) {
    if (e.target && e.target.hasAttribute("data-modal") && 
        (e.target.getAttribute("data-target") === "#addClientModal" || 
         e.target.getAttribute("data-target") === "#editClientModal")) {
        // Wait a bit for the modal to open
        setTimeout(setupClientFormValidation, 100);
    }
});
