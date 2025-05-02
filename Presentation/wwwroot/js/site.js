// Main event listener that runs when the DOM is fully loaded
document.addEventListener("DOMContentLoaded", () => {
  // Initialize modal functionality - finds all elements with data-modal="true" attribute
  // These are buttons that trigger modal windows
  const modalButtons = document.querySelectorAll('[data-modal="true"]');
  modalButtons.forEach((button) => {
    button.addEventListener("click", () => {
      const modalTarget = button.getAttribute("data-target");
      const modal = document.querySelector(modalTarget);
      if (modal) {
        modal.style.display = "flex";
      }
    });
  });

  // Utility function to close all modals and dropdowns
  // This ensures only one modal/dropdown is open at a time
  function closeAllModals() {
    const modals = [
      document.getElementById("userDropdown"),
      document.getElementById("notificationDropdown"),
      document.getElementById("profileSettingsModal"),
    ];

    modals.forEach((modal) => {
      if (modal && modal.style.display === "block") {
        modal.style.display = "none";
      }
    });
  }

  // User Settings Dropdown Implementation
  // Handles the user settings menu that appears when clicking the settings button
  const settingsButton = document.getElementById("settingsButton");
  const userDropdown = document.getElementById("userDropdown");

  if (settingsButton && userDropdown) {
    settingsButton.addEventListener("click", (e) => {
      e.stopPropagation(); // Prevents event from bubbling up to document

      // Toggle dropdown visibility
      if (userDropdown.style.display === "block") {
        userDropdown.style.display = "none";
        return;
      }

      closeAllModals(); // Close any other open modals/dropdowns
      userDropdown.style.display = "block";
    });

    // Close dropdown when clicking anywhere outside the dropdown or button
    document.addEventListener("click", (e) => {
      if (
        userDropdown.style.display === "block" &&
        !userDropdown.contains(e.target) &&
        !settingsButton.contains(e.target)
      ) {
        userDropdown.style.display = "none";
      }
    });

    // Dark Mode Implementation
    // Handles the dark mode toggle functionality including localStorage persistence
    const darkModeToggle = document.getElementById("darkModeToggle");
    if (darkModeToggle) {
      // Check if user previously enabled dark mode
      const darkMode = localStorage.getItem("darkMode") === "true";
      darkModeToggle.checked = darkMode;
      if (darkMode) {
        // Apply dark mode theme and update icons
        document.documentElement.setAttribute("data-theme", "dark");
        document.querySelector(".search-icon").src = "/images/Search_white.svg";
        const notificationIcon = document.querySelector(".notification-icon");
        if (notificationIcon)
          notificationIcon.src = "/images/Notification-white.svg";
      }

      // Handle dark mode toggle changes
      darkModeToggle.addEventListener("change", () => {
        if (darkModeToggle.checked) {
          // Enable dark mode
          document.documentElement.setAttribute("data-theme", "dark");
          document.querySelector(".search-icon").src =
            "/images/Search_white.svg";
          const notificationIcon = document.querySelector(".notification-icon");
          if (notificationIcon)
            notificationIcon.src = "/images/Notification-white.svg";
          localStorage.setItem("darkMode", "true");
        } else {
          // Disable dark mode
          document.documentElement.removeAttribute("data-theme");
          document.querySelector(".search-icon").src = "/images/Search.svg";
          const notificationIcon = document.querySelector(".notification-icon");
          if (notificationIcon)
            notificationIcon.src = "/images/Notification.svg";
          localStorage.setItem("darkMode", "false");
        }
      });
    }
  }

  // Notification System Implementation
  // Handles the notification dropdown that shows user notifications
  const notificationButton = document.getElementById("notificationButton");
  const notificationDropdown = document.getElementById("notificationDropdown");

  if (notificationButton && notificationDropdown) {
    notificationButton.addEventListener("click", (e) => {
      e.stopPropagation();

      // Toggle notification dropdown
      if (notificationDropdown.style.display === "block") {
        notificationDropdown.style.display = "none";
        return;
      }

      closeAllModals();
      notificationDropdown.style.display = "block";
    });

    // Close notification dropdown when clicking outside
    document.addEventListener("click", (e) => {
      if (
        notificationDropdown.style.display === "block" &&
        !notificationDropdown.contains(e.target) &&
        !notificationButton.contains(e.target)
      ) {
        notificationDropdown.style.display = "none";
      }
    });
  }

  // Profile Settings Modal Implementation
  // Handles the profile settings modal where users can update their profile
  const profileButton = document.getElementById("profileButton");
  const profileSettingsModal = document.getElementById("profileSettingsModal");

  if (profileButton && profileSettingsModal) {
    profileButton.addEventListener("click", (e) => {
      e.stopPropagation();

      // Toggle profile settings modal
      if (profileSettingsModal.style.display === "block") {
        profileSettingsModal.style.display = "none";
        return;
      }

      closeAllModals();
      profileSettingsModal.style.display = "block";
    });

    // Close profile settings when clicking outside
    document.addEventListener("click", (e) => {
      if (
        profileSettingsModal.style.display === "block" &&
        !profileSettingsModal.contains(e.target) &&
        !profileButton.contains(e.target)
      ) {
        profileSettingsModal.style.display = "none";
      }
    });

    // Sync Dark Mode Toggles
    // Ensures the dark mode toggle in settings matches the main toggle
    const darkModeToggleSettings = document.getElementById(
      "darkModeToggleSettings"
    );
    const darkModeToggle = document.getElementById("darkModeToggle");

    if (darkModeToggleSettings && darkModeToggle) {
      darkModeToggleSettings.checked = darkModeToggle.checked;

      // Keep both toggles synchronized
      darkModeToggleSettings.addEventListener("change", () => {
        darkModeToggle.checked = darkModeToggleSettings.checked;
        darkModeToggle.dispatchEvent(new Event("change"));
      });

      darkModeToggle.addEventListener("change", () => {
        darkModeToggleSettings.checked = darkModeToggle.checked;
      });
    }
  }

  // Project Form Debugging
  // Logs form submissions for debugging purposes
  console.log("Document loaded, checking for project forms");

  const addProjectForm = document.getElementById("addProjectForm");
  if (addProjectForm) {
    console.log("Add Project form found");
    addProjectForm.addEventListener("submit", function (e) {
      console.log("Add Project form submitted", {
        action: this.action,
        method: this.method,
        controller: this.getAttribute("asp-controller"),
      });
    });
  } else {
    console.log("Add Project form not found");
  }

  const editProjectForm = document.getElementById("editProjectForm");
  if (editProjectForm) {
    console.log("Edit Project form found");
    editProjectForm.addEventListener("submit", function (e) {
      console.log("Edit Project form submitted", {
        action: this.action,
        method: this.method,
        controller: this.getAttribute("asp-controller"),
      });
    });
  } else {
    console.log("Edit Project form not found");
  }

  // Contextual Search Implementation
  // Handles the search functionality that adapts based on the current page
  const searchInput = document.querySelector(".search-input");
  if (searchInput) {
    searchInput.addEventListener("input", function () {
      const query = this.value.toLowerCase();
      const path = window.location.pathname.toLowerCase();

      // Call appropriate filter function based on current page
      if (path.includes("/projects")) {
        if (typeof filterProjects === "function") {
          filterProjects(query);
        }
      } else if (path.includes("/clients")) {
        if (typeof filterClients === "function") {
          filterClients(query);
        }
      } else if (path.includes("/members")) {
        if (typeof filterMembers === "function") {
          filterMembers(query);
        }
      }
    });
  }

  // Rich Text Editor Implementation using Quill
  // Handles rich text editing for project descriptions
  let addQuill;
  let editQuill;

  // Configure Quill editor options
  const quillOptions = {
    theme: "snow",
    modules: {
      toolbar: [
        [{ header: [1, 2, 3, false] }],
        ["bold", "italic", "underline", "strike"],
        [{ list: "ordered" }, { list: "bullet" }],
        [{ script: "sub" }, { script: "super" }],
        [{ indent: "-1" }, { indent: "+1" }],
        [{ direction: "rtl" }],
        [{ size: ["small", false, "large", "huge"] }],
        [{ color: [] }, { background: [] }],
        [{ font: [] }],
        [{ align: [] }],
        ["link", "image"],
        ["clean"],
      ],
    },
  };

  // Initialize Quill editor for adding new projects
  const addEditorContainer = document.getElementById("add-description-editor");
  const addHiddenInput = document.getElementById("add-description-hidden");
  if (addEditorContainer && addHiddenInput) {
    try {
      addQuill = new Quill(addEditorContainer, quillOptions);
      // Sync Quill content with hidden input for form submission
      addQuill.on("text-change", function (delta, oldDelta, source) {
        if (source === "user") {
          addHiddenInput.value = addQuill.root.innerHTML;
        }
      });
    } catch (error) {
      console.error("Failed to initialize Add Project Quill editor:", error);
    }
  }

  // Initialize Quill editor for editing existing projects
  const editEditorContainer = document.getElementById(
    "edit-description-editor"
  );
  const editHiddenInput = document.getElementById("edit-description-hidden");
  if (editEditorContainer && editHiddenInput) {
    try {
      editQuill = new Quill(editEditorContainer, quillOptions);
      editQuill.on("text-change", function (delta, oldDelta, source) {
        if (source === "user") {
          editHiddenInput.value = editQuill.root.innerHTML;
        }
      });
    } catch (error) {
      console.error("Failed to initialize Edit Project Quill editor:", error);
    }
  }

  // Function to initialize edit Quill editor with existing content
  window.initializeEditQuill = function (descriptionHtml) {
    if (editQuill) {
      try {
        const delta = editQuill.clipboard.convert(descriptionHtml || "");
        editQuill.setContents(delta, "silent");
        editHiddenInput.value = descriptionHtml || "";
      } catch (error) {
        console.error("Failed to set Edit Quill content:", error);
        // Fallback to raw HTML if delta conversion fails
        editQuill.root.innerHTML = descriptionHtml || "";
        editHiddenInput.value = descriptionHtml || "";
      }
    }
  };

  // Date Input Formatting Implementation
  // Handles the formatting of date inputs to show month names
  function formatDateForDisplay(dateString) {
    if (!dateString) return "";

    const date = new Date(dateString);
    const options = { year: "numeric", month: "long", day: "2-digit" };
    return date.toLocaleDateString("en-US", options);
  }

  // Initialize and format all date inputs
  function initializeDateInputs() {
    const dateInputs = document.querySelectorAll(".date-actual-input");

    dateInputs.forEach((input) => {
      const displayInput = input.parentElement.querySelector(
        ".date-display-input"
      );

      // Format initial value
      if (input.value) {
        displayInput.value = formatDateForDisplay(input.value);
      }

      // Update display when date changes
      input.addEventListener("change", function () {
        displayInput.value = formatDateForDisplay(this.value);
      });

      // Show date picker when clicking display input
      displayInput.addEventListener("click", function () {
        input.showPicker();
      });
    });
  }

  // Initialize date inputs on page load
  initializeDateInputs();

  // Re-initialize date inputs when modals are opened
  modalButtons.forEach((button) => {
    button.addEventListener("click", function () {
      setTimeout(initializeDateInputs, 100);
    });
  });
});

// Modal Close Button Implementation
// Handles the closing of modals and form resets
const closeButtons = document.querySelectorAll('[data-close="true"]');
closeButtons.forEach((button) => {
  button.addEventListener("click", () => {
    const modal = button.closest(".modal");
    if (modal) {
      modal.style.display = "none";

      // Reset all forms in the modal
      modal.querySelectorAll("form").forEach((form) => {
        form.reset();
        // Clear Quill editors if present
        if (
          modal.id === "addProjectModal" &&
          typeof addQuill !== "undefined" &&
          addQuill
        ) {
          addQuill.setContents([], "silent");
          if (addHiddenInput) addHiddenInput.value = "";
        } else if (
          modal.id === "editProjectModal" &&
          typeof editQuill !== "undefined" &&
          editQuill
        ) {
          editQuill.setContents([], "silent");
          if (editHiddenInput) editHiddenInput.value = "";
        }

        // Reset image preview to default camera icon
        const profileLabel = form.querySelector(".profile-label");
        if (profileLabel) {
          profileLabel.innerHTML = `
            <div class="camera-icon">
              <i class="fa-regular fa-camera"></i>
            </div>
          `;
        }
      });
    }
  });

  // Image Preview Implementation
  // Handles the preview of uploaded profile images
  document.querySelectorAll(".profile-input").forEach((input) => {
    input.addEventListener("change", async (event) => {
      const file = event.target.files[0];
      if (file) {
        const label = event.target.nextElementSibling;
        try {
          await processImage(file, label, label);
        } catch (error) {
          console.error("Failed to preview image:", error);
        }
      }
    });
  });
});

// Close modal when clicking outside content area
document.addEventListener("click", (event) => {
  if (event.target.classList.contains("modal")) {
    event.target.style.display = "none";
  }
});

// Image Processing Implementation
// Handles the processing and preview of uploaded images
async function processImage(file, imagePreview, previewer, previewSize = 150) {
  try {
    const img = await loadImage(file);
    const canvas = document.createElement("canvas");
    canvas.width = previewSize;
    canvas.height = previewSize;

    // Draw image on canvas for preview
    const ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0, previewSize, previewSize);

    imagePreview.innerHTML = "";

    // Create and style preview image
    const previewImg = document.createElement("img");
    previewImg.src = canvas.toDataURL("image/jpeg");
    previewImg.style.width = "100%";
    previewImg.style.height = "100%";
    previewImg.style.borderRadius = "30%";
    previewImg.style.objectFit = "cover";

    imagePreview.appendChild(previewImg);
    previewer.classList.add("selected");
  } catch (error) {
    console.error("Failed on image-processing:", error);
  }
}

// Utility function to load images as promises
function loadImage(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = (e) => {
      const img = new Image();
      img.onload = () => resolve(img);
      img.onerror = reject;
      img.src = e.target.result;
    };
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}

// Alert System Implementation
// Handles the display and automatic dismissal of alert messages
document.addEventListener("DOMContentLoaded", function () {
  const alertCloseButtons = document.querySelectorAll(".alert .btn-close");

  // Manual alert dismissal
  alertCloseButtons.forEach((button) => {
    button.addEventListener("click", function () {
      const alert = this.closest(".alert");
      if (alert) {
        alert.style.display = "none";
      }
    });
  });

  // Auto-hide alerts after 5 seconds
  setTimeout(function () {
    const alerts = document.querySelectorAll(".alert");
    alerts.forEach((alert) => {
      alert.style.display = "none";
    });
  }, 5000);
});

// Form Validation Implementation
// Handles real-time form validation and error display
document.addEventListener("DOMContentLoaded", function () {
  const addProjectForm = document.getElementById("addProjectForm");
  if (addProjectForm) {
    const validationStatus = document.getElementById("formValidationStatus");

    if (validationStatus) {
      // Monitor input changes for validation
      addProjectForm
        .querySelectorAll("input, select, textarea")
        .forEach((input) => {
          input.addEventListener("input", function () {
            updateFormValidation();
          });
        });

      updateFormValidation();

      // Update validation status and highlight invalid fields
      function updateFormValidation() {
        const isValid = addProjectForm.checkValidity();
        validationStatus.textContent = isValid ? "Valid" : "Invalid";
        validationStatus.style.color = isValid ? "green" : "red";

        // Track invalid fields
        const invalidFields = [];
        addProjectForm
          .querySelectorAll("input, select, textarea")
          .forEach((input) => {
            if (!input.checkValidity()) {
              invalidFields.push(input.name);
            }
          });

        if (invalidFields.length > 0) {
          validationStatus.textContent += ` (Issues: ${invalidFields.join(
            ", "
          )})`;
        }
      }
    }

    // Handle form submission validation
    addProjectForm.addEventListener("submit", function (e) {
      if (!this.checkValidity()) {
        e.preventDefault();

        // Highlight invalid fields
        this.querySelectorAll(":invalid").forEach((field) => {
          field.style.borderColor = "red";
        });

        validationStatus.textContent = "Form validation failed";
        validationStatus.style.color = "red";

        // Show error message
        if (!document.querySelector(".form-error-message")) {
          const errorMessage = document.createElement("div");
          errorMessage.className = "alert alert-danger form-error-message";
          errorMessage.innerHTML =
            "<span>Please fix the form errors before submitting.</span>";
          addProjectForm.prepend(errorMessage);

          // Auto-remove error message
          setTimeout(() => {
            errorMessage.remove();
          }, 5000);
        }
      } else {
        console.log("Form is valid, submitting...");
      }
    });
  }
});
