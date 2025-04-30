/**
 * Used Claude AI to debug member form validation
 * Debug script for Member Form Validation
 * Helps diagnose issues with form submission
 */

(function () {
  console.log("Member validation debug script loaded");

  function attachFormSubmitDebugger() {
    console.log("Attaching form submit debugger");

    // Find all member forms
    const addForm = document.querySelector("#addMemberModal form");
    const editForm = document.querySelector("#editMemberModal form");

    if (addForm) {
      console.log("Add member form found");

      // Create a copy of the original submit handler
      const originalSubmitHandler = addForm.onsubmit;

      // Replace with our debug handler
      addForm.addEventListener(
        "submit",
        function (e) {
          console.log("Add member form submission intercepted");
          console.log("Form state:", {
            valid: addForm.checkValidity(),
            dirty: isDirty(addForm),
            formData: getFormValues(addForm),
          });

          // Check novalidate attribute
          console.log(
            "Form has novalidate:",
            addForm.getAttribute("novalidate") !== null
          );

          // Continue normal submission
          if (addForm.checkValidity()) {
            console.log("Form is valid, allowing submission");
            // The form will be submitted normally
          } else {
            console.log("Form validation failed");
            console.log("Invalid fields:", getInvalidFields(addForm));
            // Let the normal validation handle this
          }
        },
        true
      ); // Use capturing to run before other handlers
    }

    if (editForm) {
      console.log("Edit member form found");
      editForm.addEventListener(
        "submit",
        function (e) {
          console.log("Edit member form submission", {
            valid: editForm.checkValidity(),
            formData: getFormValues(editForm),
          });
        },
        true
      );
    }
  }

  // Helper to get form values for debugging
  function getFormValues(form) {
    const values = {};
    const formData = new FormData(form);
    for (const [key, value] of formData.entries()) {
      if (key === "Password" || key === "ConfirmPassword") {
        values[key] = value ? "[REDACTED]" : "";
      } else {
        values[key] = value;
      }
    }
    return values;
  }

  // Helper to check if form has been modified
  function isDirty(form) {
    let dirty = false;
    form.querySelectorAll("input, select, textarea").forEach((field) => {
      if (field.type === "file" && field.files.length > 0) dirty = true;
      if (field.type === "checkbox" && field.checked) dirty = true;
      if (field.value && field.value !== field.defaultValue) dirty = true;
    });
    return dirty;
  }

  // Helper to get invalid fields
  function getInvalidFields(form) {
    const invalidFields = [];
    form.querySelectorAll("input, select, textarea").forEach((field) => {
      if (!field.checkValidity()) {
        invalidFields.push({
          name: field.name,
          value: field.value,
          validationMessage: field.validationMessage,
        });
      }
    });
    return invalidFields;
  }

  // Fix the validation issue by restoring default form behavior
  function fixValidationIssue() {
    console.log("Attempting to fix validation issues");

    // Fix for Add Member form
    const addForm = document.querySelector("#addMemberModal form");
    if (addForm) {
      // Remove novalidate if it exists
      if (addForm.hasAttribute("novalidate")) {
        console.log(
          "Removing novalidate attribute to restore browser validation"
        );
        addForm.removeAttribute("novalidate");
      }

      // Ensure form is not intercepted
      addForm.addEventListener("submit", function (e) {
        if (this.checkValidity()) {
          console.log("Form is valid, ensuring submission");
          return true; // Allow form to submit
        }
      });
    }
  }

  // Initialize debug helpers when page loads
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", function () {
      attachFormSubmitDebugger();
      fixValidationIssue();
    });
  } else {
    attachFormSubmitDebugger();
    fixValidationIssue();
  }

  // Also initialize when modals are shown
  document.addEventListener("click", function (e) {
    if (
      e.target &&
      (e.target.getAttribute("data-target") === "#addMemberModal" ||
        e.target.getAttribute("data-target") === "#editMemberModal")
    ) {
      setTimeout(function () {
        attachFormSubmitDebugger();
        fixValidationIssue();
      }, 100);
    }
  });
})();
