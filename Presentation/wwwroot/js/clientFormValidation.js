/**
 * Client Form Validation
 * Handles client-side validation for Add Client and Edit Client forms
 */
document.addEventListener("DOMContentLoaded", function () {
    // Get the Add Client form
    const addClientForm = document.querySelector("#addClientModal form");
    if (addClientForm) {
        console.log("Add Client form found, setting up validation");
        
        // Prevent submission if form is invalid
        addClientForm.addEventListener("submit", function (e) {
            if (!this.checkValidity()) {
                e.preventDefault();
                console.log("Add Client form validation failed");
                
                // Highlight invalid fields
                this.querySelectorAll(":invalid").forEach((field) => {
                    field.style.borderColor = "red";
                    
                    // Add error class to parent field-group
                    const fieldGroup = field.closest(".field-group");
                    if (fieldGroup) {
                        fieldGroup.classList.add("has-error");
                    }
                });
                
                // Show validation message at the top of the form
                if (!document.querySelector(".form-error-message")) {
                    const errorMessage = document.createElement("div");
                    errorMessage.className = "alert alert-danger form-error-message";
                    errorMessage.innerHTML = "<span>Please fix the form errors before submitting.</span>";
                    this.prepend(errorMessage);
                    
                    // Automatically remove after 5 seconds
                    setTimeout(() => {
                        errorMessage.remove();
                    }, 5000);
                }
                
                // Prevent modal from closing
                e.stopPropagation();
            } else {
                console.log("Add Client form is valid, submitting...");
            }
        });
        
        // Reset validation styling on input
        addClientForm.querySelectorAll("input, select, textarea").forEach((input) => {
            input.addEventListener("input", function () {
                this.style.borderColor = "";
                
                // Remove error class from parent field-group
                const fieldGroup = this.closest(".field-group");
                if (fieldGroup) {
                    fieldGroup.classList.remove("has-error");
                }
            });
        });
    }
    
    // Get the Edit Client form
    const editClientForm = document.querySelector("#editClientModal form");
    if (editClientForm) {
        console.log("Edit Client form found, setting up validation");
        
        // Prevent submission if form is invalid
        editClientForm.addEventListener("submit", function (e) {
            if (!this.checkValidity()) {
                e.preventDefault();
                console.log("Edit Client form validation failed");
                
                // Highlight invalid fields
                this.querySelectorAll(":invalid").forEach((field) => {
                    field.style.borderColor = "red";
                    
                    // Add error class to parent field-group
                    const fieldGroup = field.closest(".field-group");
                    if (fieldGroup) {
                        fieldGroup.classList.add("has-error");
                    }
                });
                
                // Show validation message at the top of the form
                if (!document.querySelector(".form-error-message")) {
                    const errorMessage = document.createElement("div");
                    errorMessage.className = "alert alert-danger form-error-message";
                    errorMessage.innerHTML = "<span>Please fix the form errors before submitting.</span>";
                    this.prepend(errorMessage);
                    
                    // Automatically remove after 5 seconds
                    setTimeout(() => {
                        errorMessage.remove();
                    }, 5000);
                }
                
                // Prevent modal from closing
                e.stopPropagation();
            } else {
                console.log("Edit Client form is valid, submitting...");
            }
        });
        
        // Reset validation styling on input
        editClientForm.querySelectorAll("input, select, textarea").forEach((input) => {
            input.addEventListener("input", function () {
                this.style.borderColor = "";
                
                // Remove error class from parent field-group
                const fieldGroup = this.closest(".field-group");
                if (fieldGroup) {
                    fieldGroup.classList.remove("has-error");
                }
            });
        });
    }
});
