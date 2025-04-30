/**
 * Member Form Validation
 * Handles client-side validation for Add Member and Edit Member forms
 */

(function () {
  // Field validation rules
  const validationRules = {
    FirstName: {
      required: true,
      minLength: 2,
      errorMessages: {
        required: "First name is required",
        minLength: "First name must be at least 2 characters",
      },
    },
    LastName: {
      required: true,
      minLength: 2,
      errorMessages: {
        required: "Last name is required",
        minLength: "Last name must be at least 2 characters",
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
    Password: {
      required: true,
      minLength: 6,
      errorMessages: {
        required: "Password is required",
        minLength: "Password must be at least 6 characters",
      },
    },
    ConfirmPassword: {
      required: true,
      validator: function (value, form) {
        // Find the password field
        const passwordField = form.querySelector('[name="Password"]');
        if (!passwordField) return true;

        // Check if passwords match
        return value === passwordField.value;
      },
      errorMessages: {
        required: "Please confirm your password",
        validator: "Passwords do not match",
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
  function validateField(field, form) {
    const fieldName = field.getAttribute("name") || field.id;
    const rules = validationRules[fieldName];

    if (!rules) return true; // No rules for this field

    // Get field value and trim it if it's a string
    let value = field.value;
    if (typeof value === "string") {
      value = value.trim();
    }

    // Find validation message element
    let errorElement =
      field.parentElement.querySelector(".text-danger") ||
      field.parentElement.querySelector(
        "[data-valmsg-for='" +
          fieldName +
          "'], [asp-validation-for='" +
          fieldName +
          "']"
      );

    // If not found, try to find it by other means
    if (!errorElement) {
      errorElement = form.querySelector(
        "[data-valmsg-for='" +
          fieldName +
          "'], [asp-validation-for='" +
          fieldName +
          "']"
      );

      // If still not found, create a new one
      if (!errorElement) {
        errorElement = document.createElement("span");
        errorElement.className = "text-danger";

        if (field.parentElement) {
          field.parentElement.appendChild(errorElement);
        } else {
          field.insertAdjacentElement("afterend", errorElement);
        }
      }
    }

    // Check required
    if (rules.required && (!value || value.length === 0)) {
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

    // Check custom validator
    if (rules.validator && !rules.validator(value, form)) {
      field.classList.add("invalid");
      field.style.borderColor = "red";
      errorElement.textContent = rules.errorMessages.validator;
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
    const formFields = form.querySelectorAll(
      "input:not([type=hidden]), select, textarea"
    );

    // Clear any previous error summary
    const existingErrorMessage = form.querySelector(".form-error-message");
    if (existingErrorMessage) {
      existingErrorMessage.remove();
    }

    // Validate each field
    formFields.forEach((field) => {
      // Skip non-validation fields like checkboxes for IsActive
      if (field.type === "checkbox" && field.name === "IsActive") return;

      const fieldValid = validateField(field, form);
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

    // DO NOT add novalidate attribute to allow browser validation as backup

    // Validate fields on blur
    form.querySelectorAll("input, select, textarea").forEach((field) => {
      // Skip non-validation fields
      if (field.type === "checkbox" && field.name === "IsActive") return;
      if (field.type === "file") return;
      if (field.type === "hidden") return;

      // Validate on blur (when field loses focus)
      field.addEventListener("blur", function () {
        validateField(this, form);
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
        const errorElement = this.parentElement.querySelector(".text-danger");
        if (errorElement) {
          errorElement.textContent = "";
        }
      });
    });

    // Special validation for password confirmation
    const passwordField = form.querySelector('[name="Password"]');
    const confirmPasswordField = form.querySelector('[name="ConfirmPassword"]');

    if (passwordField && confirmPasswordField) {
      // Validate confirm password when password changes
      passwordField.addEventListener("input", function () {
        if (confirmPasswordField.value) {
          validateField(confirmPasswordField, form);
        }
      });
    }

    // Set up a validation check before form submission, but DON'T prevent submission if valid
    form.addEventListener("submit", function (e) {
      const isValid = validateForm(this);
      if (!isValid) {
        e.preventDefault();
        e.stopPropagation(); // Prevent modal from closing
        console.log(`${formName} form validation failed`);
      } else {
        console.log(`${formName} form is valid, allowing submission`);
        // DO NOT prevent default - let the form submit to the server
      }
    });

    return form;
  }

  // Initialize validation
  function init() {
    // Set up both forms
    setupForm("#addMemberModal form", "Add Member");
    setupForm("#editMemberModal form", "Edit Member");
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
      (e.target.getAttribute("data-target") === "#addMemberModal" ||
        e.target.getAttribute("data-target") === "#editMemberModal")
    ) {
      // Wait for modal to open
      setTimeout(init, 100);
    }
  });
})();
