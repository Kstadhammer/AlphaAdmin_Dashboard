/**
 * Client Form Validation System
 *
 * A comprehensive client-side validation system for client forms that provides:
 * - Real-time validation feedback
 * - Custom validation rules and error messages
 * - Form-level and field-level validation
 * - Visual error indicators
 * - Automatic error message handling
 *
 * Supported Forms:
 * - Add Client Modal Form
 * - Edit Client Modal Form
 *
 * Validation Features:
 * - Required field validation
 * - Minimum length validation
 * - Pattern/regex validation
 * - Custom error messaging
 * - Live validation on blur
 * - Form submission validation
 */

(function () {
  /**
   * Validation Rules Configuration
   * Defines the validation rules and error messages for each field
   */
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

  /**
   * Field Validation
   * Validates a single form field against its defined rules
   *
   * @param {HTMLElement} field - The form field to validate
   * @returns {boolean} - Whether the field is valid
   */
  function validateField(field) {
    const fieldName = field.getAttribute("name") || field.id;
    const rules = validationRules[fieldName];

    if (!rules) return true; // Skip validation if no rules defined

    // Get and trim field value
    const value = field.value.trim();

    // Create or get error message element
    let errorElement = field.parentElement.querySelector(".validation-error");
    if (!errorElement) {
      errorElement = document.createElement("span");
      errorElement.className = "validation-error text-danger";
      field.parentElement.appendChild(errorElement);
    }

    // Required field validation
    if (rules.required && !value) {
      field.classList.add("invalid");
      field.style.borderColor = "red";
      errorElement.textContent = rules.errorMessages.required;
      return false;
    }

    // Minimum length validation
    if (rules.minLength && value.length < rules.minLength) {
      field.classList.add("invalid");
      field.style.borderColor = "red";
      errorElement.textContent = rules.errorMessages.minLength;
      return false;
    }

    // Pattern/regex validation
    if (rules.pattern && value && !rules.pattern.test(value)) {
      field.classList.add("invalid");
      field.style.borderColor = "red";
      errorElement.textContent = rules.errorMessages.pattern;
      return false;
    }

    // Clear validation state for valid field
    field.classList.remove("invalid");
    field.style.borderColor = "";
    errorElement.textContent = "";
    return true;
  }

  /**
   * Form Validation
   * Validates an entire form and manages error display
   *
   * @param {HTMLFormElement} form - The form to validate
   * @returns {boolean} - Whether the form is valid
   */
  function validateForm(form) {
    let isValid = true;
    const formFields = form.querySelectorAll("input, select, textarea");

    // Remove any existing error summary
    const existingErrorMessage = form.querySelector(".form-error-message");
    if (existingErrorMessage) {
      existingErrorMessage.remove();
    }

    // Validate all form fields
    formFields.forEach((field) => {
      const fieldValid = validateField(field);
      isValid = isValid && fieldValid;

      // Update field group styling
      const fieldGroup = field.closest(".field-group");
      if (fieldGroup) {
        if (!fieldValid) {
          fieldGroup.classList.add("has-error");
        } else {
          fieldGroup.classList.remove("has-error");
        }
      }
    });

    // Display form-level error message if invalid
    if (!isValid) {
      const errorMessage = document.createElement("div");
      errorMessage.className = "alert alert-danger form-error-message";
      errorMessage.innerHTML =
        "<span>Please fix the form errors before submitting.</span>";
      form.prepend(errorMessage);

      // Scroll to form top for visibility
      form.scrollIntoView({ behavior: "smooth", block: "start" });

      // Auto-remove error message after 5 seconds
      setTimeout(() => {
        errorMessage.remove();
      }, 5000);
    }

    return isValid;
  }

  /**
   * Form Setup
   * Configures validation handlers for a specific form
   *
   * @param {string} formSelector - CSS selector for the form
   * @param {string} formName - Name of the form for logging
   * @returns {HTMLFormElement|null} - The configured form or null if not found
   */
  function setupForm(formSelector, formName) {
    const form = document.querySelector(formSelector);
    if (!form) return null;

    console.log(`${formName} form found, setting up validation`);

    // Form submission validation
    form.addEventListener("submit", function (e) {
      if (!validateForm(this)) {
        e.preventDefault();
        e.stopPropagation(); // Prevent modal from closing
        console.log(`${formName} form validation failed`);
      } else {
        console.log(`${formName} form is valid, submitting...`);
      }
    });

    // Set up field-level validation
    form.querySelectorAll("input, select, textarea").forEach((field) => {
      // Validate when field loses focus
      field.addEventListener("blur", function () {
        validateField(this);
      });

      // Clear validation state on input
      field.addEventListener("input", function () {
        this.style.borderColor = "";
        this.classList.remove("invalid");

        // Clear field group error state
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

  /**
   * Initialization
   * Sets up validation for all client forms
   */
  function init() {
    // Initialize both client forms
    setupForm("#addClientModal form", "Add Client");
    setupForm("#editClientModal form", "Edit Client");
  }

  // Initialize on page load
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }

  // Re-initialize when modals are opened
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
