﻿document.addEventListener("DOMContentLoaded", () => {
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

  // Function to close all modals/dropdowns
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

  // Add user dropdown functionality
  const settingsButton = document.getElementById("settingsButton");
  const userDropdown = document.getElementById("userDropdown");

  if (settingsButton && userDropdown) {
    settingsButton.addEventListener("click", (e) => {
      e.stopPropagation();

      // If this dropdown is already open, just close it
      if (userDropdown.style.display === "block") {
        userDropdown.style.display = "none";
        return;
      }

      // Close all modals before opening this one
      closeAllModals();

      // Open this dropdown
      userDropdown.style.display = "block";
    });

    // Close dropdown when clicking outside
    document.addEventListener("click", (e) => {
      if (
        userDropdown.style.display === "block" &&
        !userDropdown.contains(e.target) &&
        !settingsButton.contains(e.target)
      ) {
        userDropdown.style.display = "none";
      }
    });

    // Dark mode toggle functionality
    const darkModeToggle = document.getElementById("darkModeToggle");
    if (darkModeToggle) {
      // Check for saved preference
      const darkMode = localStorage.getItem("darkMode") === "true";
      darkModeToggle.checked = darkMode;
      if (darkMode) {
        document.documentElement.setAttribute("data-theme", "dark");
        document.querySelector(".search-icon").src = "/images/Search_white.svg";
        const notificationIcon = document.querySelector(".notification-icon");
        if (notificationIcon)
          notificationIcon.src = "/images/Notification-white.svg";
      }

      darkModeToggle.addEventListener("change", () => {
        if (darkModeToggle.checked) {
          document.documentElement.setAttribute("data-theme", "dark");
          document.querySelector(".search-icon").src =
            "/images/Search_white.svg";
          const notificationIcon = document.querySelector(".notification-icon");
          if (notificationIcon)
            notificationIcon.src = "/images/Notification-white.svg";
          localStorage.setItem("darkMode", "true");
        } else {
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

  // Add notification dropdown functionality
  const notificationButton = document.getElementById("notificationButton");
  const notificationDropdown = document.getElementById("notificationDropdown");

  if (notificationButton && notificationDropdown) {
    notificationButton.addEventListener("click", (e) => {
      e.stopPropagation(); // Prevent click from immediately closing dropdown

      // If this dropdown is already open, just close it
      if (notificationDropdown.style.display === "block") {
        notificationDropdown.style.display = "none";
        return;
      }

      // Close all modals before opening this one
      closeAllModals();

      // Open this dropdown
      notificationDropdown.style.display = "block";
    });

    // Close notification dropdown when clicking outside
    document.addEventListener("click", (e) => {
      if (
        notificationDropdown.style.display === "block" &&
        !notificationDropdown.contains(e.target) &&
        !notificationButton.contains(e.target) // Also check if click was on the button itself
      ) {
        notificationDropdown.style.display = "none";
      }
    });
  }

  // Add profile settings modal functionality
  const profileButton = document.getElementById("profileButton");
  const profileSettingsModal = document.getElementById("profileSettingsModal");

  if (profileButton && profileSettingsModal) {
    profileButton.addEventListener("click", (e) => {
      e.stopPropagation(); // Prevent click from immediately closing modal

      // If this modal is already open, just close it
      if (profileSettingsModal.style.display === "block") {
        profileSettingsModal.style.display = "none";
        return;
      }

      // Close all modals before opening this one
      closeAllModals();

      // Open this modal
      profileSettingsModal.style.display = "block";
    });

    // Close profile settings modal when clicking outside
    document.addEventListener("click", (e) => {
      if (
        profileSettingsModal.style.display === "block" &&
        !profileSettingsModal.contains(e.target) &&
        !profileButton.contains(e.target) // Also check if click was on the button itself
      ) {
        profileSettingsModal.style.display = "none";
      }
    });

    // Sync the dark mode toggle in the settings with the main toggle
    const darkModeToggleSettings = document.getElementById(
      "darkModeToggleSettings"
    );
    const darkModeToggle = document.getElementById("darkModeToggle");

    if (darkModeToggleSettings && darkModeToggle) {
      // Initialize the settings toggle to match the main toggle
      darkModeToggleSettings.checked = darkModeToggle.checked;

      // Keep the toggles in sync
      darkModeToggleSettings.addEventListener("change", () => {
        darkModeToggle.checked = darkModeToggleSettings.checked;
        darkModeToggle.dispatchEvent(new Event("change"));
      });

      darkModeToggle.addEventListener("change", () => {
        darkModeToggleSettings.checked = darkModeToggle.checked;
      });
    }
  }

  // Add error logging for project form submissions
  console.log("Document loaded, checking for project forms");

  // Log form submissions for debugging
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

  // We're now using our custom member selection component instead of Choices.js
  // Contextual navbar search
  const searchInput = document.querySelector(".search-input");
  if (searchInput) {
    searchInput.addEventListener("input", function () {
      const query = this.value.toLowerCase();
      const path = window.location.pathname.toLowerCase();

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

  // Generated by Google Gemini
  // --- Quill Editor Initialization ---
  let addQuill;
  let editQuill;

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

  // Initialize Add Project Quill Editor
  const addEditorContainer = document.getElementById("add-description-editor");
  const addHiddenInput = document.getElementById("add-description-hidden");
  if (addEditorContainer && addHiddenInput) {
    try {
      addQuill = new Quill(addEditorContainer, quillOptions);
      addQuill.on("text-change", function (delta, oldDelta, source) {
        if (source === "user") {
          addHiddenInput.value = addQuill.root.innerHTML; // Store HTML content
        }
      });
    } catch (error) {
      console.error("Failed to initialize Add Project Quill editor:", error);
    }
  }

  // Initialize Edit Project Quill Editor (content set when modal opens)
  const editEditorContainer = document.getElementById(
    "edit-description-editor"
  );
  const editHiddenInput = document.getElementById("edit-description-hidden");
  if (editEditorContainer && editHiddenInput) {
    try {
      editQuill = new Quill(editEditorContainer, quillOptions);
      editQuill.on("text-change", function (delta, oldDelta, source) {
        if (source === "user") {
          editHiddenInput.value = editQuill.root.innerHTML; // Store HTML content
        }
      });
    } catch (error) {
      console.error("Failed to initialize Edit Project Quill editor:", error);
    }
  }

  // Function to set content in Edit Quill editor (called when modal opens)
  window.initializeEditQuill = function (descriptionHtml) {
    if (editQuill) {
      try {
        const delta = editQuill.clipboard.convert(descriptionHtml || "");
        editQuill.setContents(delta, "silent"); // Set content without triggering text-change
        editHiddenInput.value = descriptionHtml || ""; // Ensure hidden input is also set initially
      } catch (error) {
        console.error("Failed to set Edit Quill content:", error);
        // Fallback: Try setting raw HTML if conversion fails
        editQuill.root.innerHTML = descriptionHtml || "";
        editHiddenInput.value = descriptionHtml || "";
      }
    }
  };

  // Format date inputs with month name
  function formatDateForDisplay(dateString) {
    if (!dateString) return "";

    const date = new Date(dateString);
    const options = { year: "numeric", month: "long", day: "2-digit" };
    return date.toLocaleDateString("en-US", options);
  }

  function initializeDateInputs() {
    const dateInputs = document.querySelectorAll(".date-actual-input");

    dateInputs.forEach((input) => {
      const displayInput = input.parentElement.querySelector(
        ".date-display-input"
      );

      // Set initial formatted value
      if (input.value) {
        displayInput.value = formatDateForDisplay(input.value);
      }

      // Update display when date changes
      input.addEventListener("change", function () {
        displayInput.value = formatDateForDisplay(this.value);
      });

      // Handle click on the display input (focus the actual input)
      displayInput.addEventListener("click", function () {
        input.showPicker();
      });
    });
  }

  // Initialize date inputs when DOM loads
  initializeDateInputs();

  // Initialize date inputs when add/edit project modals are opened
  modalButtons.forEach((button) => {
    button.addEventListener("click", function () {
      setTimeout(initializeDateInputs, 100); // Short delay to ensure DOM is updated
    });
  });
});

// Add event listeners for close buttons
const closeButtons = document.querySelectorAll('[data-close="true"]');
closeButtons.forEach((button) => {
  button.addEventListener("click", () => {
    // Find the closest parent modal
    const modal = button.closest(".modal");
    if (modal) {
      modal.style.display = "none";

      // Reset all forms in the modal
      modal.querySelectorAll("form").forEach((form) => {
        form.reset();
        // Clear Quill editors if they exist in the modal
        if (
          modal.id === "addProjectModal" &&
          typeof addQuill !== "undefined" &&
          addQuill
        ) {
          addQuill.setContents([], "silent"); // Clear content using setContents with empty delta
          if (addHiddenInput) addHiddenInput.value = "";
        } else if (
          modal.id === "editProjectModal" &&
          typeof editQuill !== "undefined" &&
          editQuill
        ) {
          editQuill.setContents([], "silent"); // Clear content
          if (editHiddenInput) editHiddenInput.value = "";
        }

        // Reset image preview
        const profileLabel = form.querySelector(".profile-label");
        if (profileLabel) {
          // Restore the original camera icon content
          profileLabel.innerHTML = `
            <div class="camera-icon">
              <i class="fa-regular fa-camera"></i>
            </div>
          `;
        }
      });
    }
  });
  // Handle image preview
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

// Optional: Close modal when clicking outside the content
document.addEventListener("click", (event) => {
  if (event.target.classList.contains("modal")) {
    event.target.style.display = "none";
  }
});

// Add event listener for profile image upload (code from Hans Tips och Trix)

async function processImage(file, imagePreview, previewer, previewSize = 150) {
  try {
    const img = await loadImage(file);
    const canvas = document.createElement("canvas");
    canvas.width = previewSize;
    canvas.height = previewSize;

    const ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0, previewSize, previewSize);

    // Clear existing content
    imagePreview.innerHTML = "";

    // Create image element
    const previewImg = document.createElement("img");
    previewImg.src = canvas.toDataURL("image/jpeg");
    previewImg.style.width = "100%";
    previewImg.style.height = "100%";
    previewImg.style.borderRadius = "30%";
    previewImg.style.objectFit = "cover";

    // Add the image to the preview
    imagePreview.appendChild(previewImg);

    previewer.classList.add("selected");
  } catch (error) {
    console.error("Failed on image-processing:", error);
  }
}

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

// Handle alert dismissal
document.addEventListener("DOMContentLoaded", function () {
  // Get all alert close buttons
  const alertCloseButtons = document.querySelectorAll(".alert .btn-close");

  // Add click event to each button
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

// Add form validation debugging
document.addEventListener("DOMContentLoaded", function () {
  const addProjectForm = document.getElementById("addProjectForm");
  if (addProjectForm) {
    const validationStatus = document.getElementById("formValidationStatus");

    if (validationStatus) {
      // Check form validity on input change
      addProjectForm
        .querySelectorAll("input, select, textarea")
        .forEach((input) => {
          input.addEventListener("input", function () {
            updateFormValidation();
          });
        });

      // Initial check
      updateFormValidation();

      function updateFormValidation() {
        const isValid = addProjectForm.checkValidity();
        validationStatus.textContent = isValid ? "Valid" : "Invalid";
        validationStatus.style.color = isValid ? "green" : "red";

        // Check individual fields
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

    // Prevent submission if form is invalid
    addProjectForm.addEventListener("submit", function (e) {
      if (!this.checkValidity()) {
        e.preventDefault();

        // Highlight all invalid fields
        this.querySelectorAll(":invalid").forEach((field) => {
          field.style.borderColor = "red";
        });

        validationStatus.textContent = "Form validation failed";
        validationStatus.style.color = "red";

        // Show user a message
        if (!document.querySelector(".form-error-message")) {
          const errorMessage = document.createElement("div");
          errorMessage.className = "alert alert-danger form-error-message";
          errorMessage.innerHTML =
            "<span>Please fix the form errors before submitting.</span>";
          addProjectForm.prepend(errorMessage);

          // Automatically remove after a few seconds
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
