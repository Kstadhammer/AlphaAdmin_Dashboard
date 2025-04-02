document.addEventListener("DOMContentLoaded", () => {
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

  // Add user dropdown functionality
  const settingsButton = document.getElementById("settingsButton");
  const userDropdown = document.getElementById("userDropdown");

  if (settingsButton && userDropdown) {
    settingsButton.addEventListener("click", (e) => {
      e.stopPropagation();
      userDropdown.style.display =
        userDropdown.style.display === "block" ? "none" : "block";
    });

    // Close dropdown when clicking outside
    document.addEventListener("click", (e) => {
      if (
        userDropdown.style.display === "block" &&
        !userDropdown.contains(e.target)
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
      }

      darkModeToggle.addEventListener("change", () => {
        if (darkModeToggle.checked) {
          document.documentElement.setAttribute("data-theme", "dark");
          document.querySelector(".search-icon").src =
            "/images/Search_white.svg";
          localStorage.setItem("darkMode", "true");
        } else {
          document.documentElement.removeAttribute("data-theme");
          document.querySelector(".search-icon").src = "/images/Search.svg";
          localStorage.setItem("darkMode", "false");
        }
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

  // Initialize Choices.js for member multi-select in Add Project form
  const addProjectMemberSelect = document.querySelector(
    "#addProjectForm .member-select"
  );
  if (addProjectMemberSelect) {
    console.log("Initializing Choices.js for Add Project members");
    new Choices(addProjectMemberSelect, {
      removeItemButton: true, // Add 'x' button to remove selected items
      // placeholder: true, // Optional: Use if you want a placeholder
      // placeholderValue: 'Select members...', // Optional: Placeholder text
      // searchEnabled: true, // Optional: Enable searching within choices
    });
  }
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
