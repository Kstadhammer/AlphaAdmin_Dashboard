/**
 * Project Form Validation
 * Handles client-side validation for Add Project and Edit Project forms
 */

(function () {
  // Field validation rules
  const validationRules = {
    Name: {
      required: true,
      minLength: 2,
      maxLength: 100,
      errorMessages: {
        required: "Project name is required",
        minLength: "Project name must be at least 2 characters",
        maxLength: "Project name must be less than 100 characters",
      },
    },
    ClientId: {
      required: true,
      errorMessages: {
        required: "Client selection is required",
      },
    },
    ClientName: {
      // For edit form
      required: true,
      errorMessages: {
        required: "Client name is required",
      },
    },
    StatusId: {
      required: true,
      errorMessages: {
        required: "Status selection is required",
      },
    },
    StartDate: {
      required: true,
      validator: function (value) {
        return (
          value && new Date(value) !== "Invalid Date" && !isNaN(new Date(value))
        );
      },
      errorMessages: {
        required: "Start date is required",
        validator: "Please enter a valid date",
      },
    },
    EndDate: {
      required: true,
      validator: function (value, form) {
        if (
          !value ||
          new Date(value) === "Invalid Date" ||
          isNaN(new Date(value))
        ) {
          return false;
        }

        // Check that end date is after start date
        const startDateInput = form.querySelector('[name="StartDate"]');
        if (startDateInput && startDateInput.value) {
          const startDate = new Date(startDateInput.value);
          const endDate = new Date(value);
          return endDate >= startDate;
        }

        return true;
      },
      errorMessages: {
        required: "End date is required",
        validator: "End date must be equal to or after start date",
      },
    },
    Budget: {
      validator: function (value) {
        if (value === "" || value === null) return true; // Optional
        const budget = parseFloat(value);
        return !isNaN(budget) && budget >= 0;
      },
      errorMessages: {
        validator: "Budget must be a positive number",
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

    // Find error element - could be inside field-group or after it
    let errorElement = field.parentElement.querySelector(
      "[data-valmsg-for='" +
        fieldName +
        "'], [asp-validation-for='" +
        fieldName +
        "'], .validation-error"
    );
    if (!errorElement) {
      // Try to find validation span by field name
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
        errorElement.className = "validation-error text-danger";
        // Try to add it after the field or to its parent
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

    // Check maxLength
    if (rules.maxLength && value.length > rules.maxLength) {
      field.classList.add("invalid");
      field.style.borderColor = "red";
      errorElement.textContent = rules.errorMessages.maxLength;
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

  // Handle Quill editor content before form submission
  function handleQuillContent(form) {
    const addEditor = window.addProjectQuill;
    const editEditor = window.editProjectQuill;

    if (form.id === "addProjectForm" && addEditor) {
      document.getElementById("add-description-hidden").value =
        addEditor.root.innerHTML;
    } else if (form.id === "editProjectForm" && editEditor) {
      document.getElementById("edit-description-hidden").value =
        editEditor.root.innerHTML;
    }
  }

  // Validate whole form
  function validateForm(form) {
    let isValid = true;
    const formFields = form.querySelectorAll(
      "input:not([type=hidden]), select, textarea"
    );

    // Handle Quill editor content
    handleQuillContent(form);

    // Clear any previous error summary
    const existingErrorMessage = form.querySelector(".form-error-message");
    if (existingErrorMessage) {
      existingErrorMessage.remove();
    }

    // Validate each field
    formFields.forEach((field) => {
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

    // Update validation status if debug element exists
    const formValidationStatus = form.querySelector("#formValidationStatus");
    if (formValidationStatus) {
      formValidationStatus.textContent = isValid ? "Valid" : "Invalid";
      formValidationStatus.style.color = isValid ? "green" : "red";
    }

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

    // Validate on change for select elements
    form.querySelectorAll("select").forEach((field) => {
      field.addEventListener("change", function () {
        validateField(this, form);
      });
    });

    // Set up live validation on input
    form.querySelectorAll("input, select, textarea").forEach((field) => {
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
        const errorElement =
          this.parentElement.querySelector(".validation-error");
        if (errorElement) {
          errorElement.textContent = "";
        }
      });
    });

    // Special date validation - validate end date when start date changes
    const startDateInput = form.querySelector('[name="StartDate"]');
    const endDateInput = form.querySelector('[name="EndDate"]');
    if (startDateInput && endDateInput) {
      startDateInput.addEventListener("change", function () {
        validateField(endDateInput, form);
      });
    }

    return form;
  }

  // Initialize validation
  function init() {
    // Set up both forms
    setupForm("#addProjectForm", "Add Project");
    setupForm("#editProjectForm", "Edit Project");
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
      (e.target.getAttribute("data-target") === "#addProjectModal" ||
        e.target.getAttribute("data-target") === "#editProjectModal")
    ) {
      // Wait for modal to open
      setTimeout(init, 100);
    }
  });
})();
