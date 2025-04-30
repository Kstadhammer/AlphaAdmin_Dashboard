/**
 * Client Form Validation
 * Handles client-side validation for Add Client and Edit Client forms
 * This is the primary validation file for client forms
 */

(function () {
  // Field validation rules
  const validationRules = {
    ClientName: {
      required: true,
      minLength: 2,
      errorMessages: {
        required: "Client name is required",
        minLength: "Client name must be at least 2 characters",
      },
    },
    Email: {
      required: true,
      pattern: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
      errorMessages: {
        required: "Email is required",
        pattern: "Please enter a valid email address",
      },
    },
    Phone: {
      pattern: /^(\+\d{1,3}[- ]?)?\d{10,}$/,
      errorMessages: {
        pattern: "Please enter a valid phone number",
      },
    },
  };

  // Validate a specific field
  function validateField(field) {
    const fieldName = field.getAttribute("name") || field.id;
    const rules = validationRules[fieldName];

    if (!rules) return true; // No rules for this field

    // Get field value and trim it
    const value = field.value.trim();

    // Get or create validation message element
    let errorElement = field.parentElement.querySelector(".validation-error");
    if (!errorElement) {
      errorElement = document.createElement("span");
      errorElement.className = "validation-error text-danger";
      field.parentElement.appendChild(errorElement);
    }

    // Check required
    if (rules.required && !value) {
      field.classList.add("invalid");
      field.style.borderColor = "red";
      errorElement.textContent = rules.errorMessages.required;
      return false;
    }

    // Check minLength
    if (rules.minLength && value.length < rules.minLength) {
      field.classList.add("invalid");
      field.style.borderColor = "red";
      errorElement.textContent = rules.errorMessages.minLength;
      return false;
    }

    // Check pattern
    if (rules.pattern && value && !rules.pattern.test(value)) {
      field.classList.add("invalid");
      field.style.borderColor = "red";
      errorElement.textContent = rules.errorMessages.pattern;
      return false;
    }

    // Field is valid
    field.classList.remove("invalid");
    field.style.borderColor = "";
    errorElement.textContent = "";
    return true;
  }

  // Validate whole form
  function validateForm(form) {
    let isValid = true;
    const formFields = form.querySelectorAll("input, select, textarea");

    // Clear any previous error summary
    const existingErrorMessage = form.querySelector(".form-error-message");
    if (existingErrorMessage) {
      existingErrorMessage.remove();
    }

    // Validate each field
    formFields.forEach((field) => {
      const fieldValid = validateField(field);
      isValid = isValid && fieldValid;

      // Add or remove error class to parent field-group
      const fieldGroup = field.closest(".field-group");
      if (fieldGroup) {
        if (!fieldValid) {
          fieldGroup.classList.add("has-error");
        } else {
          fieldGroup.classList.remove("has-error");
        }
      }
    });

    // If form is invalid, show summary error message
    if (!isValid) {
      const errorMessage = document.createElement("div");
      errorMessage.className = "alert alert-danger form-error-message";
      errorMessage.innerHTML =
        "<span>Please fix the form errors before submitting.</span>";
      form.prepend(errorMessage);

      // Scroll to the top of the form
      form.scrollIntoView({ behavior: "smooth", block: "start" });

      // Automatically remove after 5 seconds
      setTimeout(() => {
        errorMessage.remove();
      }, 5000);
    }

    return isValid;
  }

  // Function to set up a specific form
  function setupForm(formSelector, formName) {
    const form = document.querySelector(formSelector);
    if (!form) return null;

    console.log(`${formName} form found, setting up validation`);

    // Set up form submit validation
    form.addEventListener("submit", function (e) {
      if (!validateForm(this)) {
        e.preventDefault();
        e.stopPropagation(); // Prevent modal from closing
        console.log(`${formName} form validation failed`);
      } else {
        console.log(`${formName} form is valid, submitting...`);
      }
    });

    // Set up live validation on input
    form.querySelectorAll("input, select, textarea").forEach((field) => {
      // Validate on blur (when field loses focus)
      field.addEventListener("blur", function () {
        validateField(this);
      });

      // Clear error styling on input
      field.addEventListener("input", function () {
        this.style.borderColor = "";
        this.classList.remove("invalid");

        // Remove error class from parent field-group
        const fieldGroup = this.closest(".field-group");
        if (fieldGroup) {
          fieldGroup.classList.remove("has-error");
        }

        // Clear error message
        const errorElement =
          this.parentElement.querySelector(".validation-error");
        if (errorElement) {
          errorElement.textContent = "";
        }
      });
    });

    return form;
  }

  // Initialize validation
  function init() {
    // Set up both forms
    setupForm("#addClientModal form", "Add Client");
    setupForm("#editClientModal form", "Edit Client");
  }

  // Run on page load
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }

  // Also set up when modals are shown
  document.addEventListener("click", function (e) {
    if (
      e.target &&
      (e.target.getAttribute("data-target") === "#addClientModal" ||
        e.target.getAttribute("data-target") === "#editClientModal")
    ) {
      // Wait for modal to open
      setTimeout(init, 100);
    }
  });
})();
