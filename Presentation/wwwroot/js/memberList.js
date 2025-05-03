/**
 * Member List Management System
 * Handles all member-related functionality including:
 * - Member card display and filtering
 * - Member CRUD operations (Create, Read, Update, Delete)
 * - Avatar selection and preview
 * - Admin role assignment
 * - Dropdown menu interactions
 * - Modal management
 */

/**
 * Utility Functions
 */

// Retrieves the anti-forgery token for secure form submissions
function getAntiForgeryToken() {
  const tokenInput = document.querySelector(
    'input[name="__RequestVerificationToken"]'
  );
  return tokenInput ? tokenInput.value : null;
}

/**
 * Member Filtering System
 * Filters member cards based on search input
 * @param {string} query - The search term to filter by
 */
function filterMembers(query) {
  const memberCards = document.querySelectorAll(".member-grid .member-card");
  memberCards.forEach((card) => {
    // Combine all text content for searching
    const allText = Array.from(card.querySelectorAll("td, div"))
      .map((el) => el.textContent.toLowerCase())
      .join(" ");

    // Show/hide cards based on search match
    if (allText.includes(query)) {
      card.style.display = "";
    } else {
      card.style.display = "none";
    }
  });
}

// Main initialization when DOM is loaded
document.addEventListener("DOMContentLoaded", function () {
  // Initialize avatar selection functionality for both add and edit forms
  initializeAvatarSelection(
    "#addMemberModal",
    "#addCurrentAvatar",
    "#addMemberImageUrl"
  );
  initializeAvatarSelection(
    "#editMemberModal",
    "#editCurrentAvatar",
    "#editMemberImageUrl"
  );

  // Initialize file upload preview for avatar images
  initializeAvatarUploadPreview("#add_MemberImage", "#addCurrentAvatar");
  initializeAvatarUploadPreview("#edit_MemberImage", "#editCurrentAvatar");

  /**
   * Dropdown Menu Management
   * Handles the ellipsis menu for each member card
   */
  const dropdownToggles = document.querySelectorAll(
    ".card.member .dropdown-toggle"
  );
  console.log("Found dropdown toggles:", dropdownToggles.length);

  // Set up dropdown toggle functionality
  dropdownToggles.forEach((toggle) => {
    toggle.addEventListener("click", function (e) {
      e.stopPropagation(); // Prevent event from bubbling to document

      // Close any other open dropdowns
      document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
        if (menu !== this.nextElementSibling) {
          menu.classList.remove("show");
        }
      });

      // Toggle current dropdown
      this.nextElementSibling.classList.toggle("show");
    });
  });

  // Close dropdowns when clicking outside
  document.addEventListener("click", function () {
    document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
      menu.classList.remove("show");
    });
  });

  /**
   * Edit Member Functionality
   * Handles populating and displaying the edit member modal
   */
  const editMemberButtons = document.querySelectorAll(".edit-member");
  console.log("Found edit member buttons:", editMemberButtons.length);

  editMemberButtons.forEach((button) => {
    button.addEventListener("click", function () {
      const memberId = this.getAttribute("data-member-id");
      document.getElementById("editMemberId").value = memberId;

      // Fetch and populate member data
      fetch(`/Members/GetMember/${memberId}`)
        .then((response) => response.json())
        .then((data) => {
          if (data.success) {
            // Populate form fields with member data
            document.querySelector(
              "#editMemberModal [name='FirstName']"
            ).value = data.member.firstName;
            document.querySelector("#editMemberModal [name='LastName']").value =
              data.member.lastName;
            document.querySelector("#editMemberModal [name='Email']").value =
              data.member.email;
            document.querySelector("#editMemberModal [name='JobTitle']").value =
              data.member.jobTitle || "";
            document.querySelector("#editMemberModal [name='Phone']").value =
              data.member.phone || "";
            document.querySelector(
              "#editMemberModal [name='IsActive']"
            ).checked = data.member.isActive;

            // Handle avatar display and selection
            if (data.member.imageUrl) {
              document.querySelector("#editCurrentAvatar").src =
                data.member.imageUrl;
              document.querySelector("#editMemberImageUrl").value =
                data.member.imageUrl;

              // Update avatar selection UI
              const avatarOptions = document.querySelectorAll(
                "#editMemberModal .avatar-option"
              );
              avatarOptions.forEach((option) => {
                option.classList.remove("selected");
                if (option.dataset.avatar === data.member.imageUrl) {
                  option.classList.add("selected");
                }
              });
            }

            // Display the edit modal
            const modal = document.getElementById("editMemberModal");
            modal.style.display = "flex";
          } else {
            console.error("Failed to fetch member data:", data.message);
          }
        })
        .catch((error) => console.error("Error fetching member data:", error));
    });
  });

  /**
   * Delete Member Functionality
   * Handles the delete confirmation modal and process
   */
  const deleteMemberButtons = document.querySelectorAll(".delete-member");
  console.log("Found delete member buttons:", deleteMemberButtons.length);

  deleteMemberButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.stopPropagation();

      // Close any open dropdown menu
      document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
        menu.classList.remove("show");
      });

      // Set up delete confirmation modal
      const memberId = this.getAttribute("data-member-id");
      const memberName = this.getAttribute("data-member-name");

      document.getElementById("deleteMemberId").value = memberId;
      document.getElementById("deleteMemberName").textContent = memberName;

      // Show the confirmation modal
      const modal = document.getElementById("deleteMemberModal");
      modal.style.display = "flex";
    });
  });

  /**
   * Admin Role Assignment
   * Handles the process of making a member an admin
   */
  const makeAdminButtons = document.querySelectorAll(".make-admin");
  makeAdminButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.stopPropagation();

      // Close any open dropdowns
      document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
        menu.classList.remove("show");
      });

      // Set up admin assignment modal
      const memberId = this.getAttribute("data-member-id");
      const memberName = this.getAttribute("data-member-name");

      const assignAdminMemberIdInput = document.getElementById(
        "assignAdminMemberId"
      );
      const assignAdminMemberNameSpan = document.getElementById(
        "assignAdminMemberName"
      );

      if (assignAdminMemberIdInput) assignAdminMemberIdInput.value = memberId;
      if (assignAdminMemberNameSpan)
        assignAdminMemberNameSpan.textContent = memberName;

      // Show the admin assignment modal
      const modal = document.getElementById("assignAdminModal");
      if (modal) {
        modal.style.display = "flex";
      } else {
        console.error("Assign Admin Modal not found!");
        alert("Error: Assign Admin confirmation dialog not found.");
      }
    });
  });

  /**
   * Modal Management
   * Handles closing of all modals used in member management
   */
  document
    .querySelectorAll(
      '#editMemberModal [data-close="true"], #deleteMemberModal [data-close="true"], #assignAdminModal [data-close="true"]'
    )
    .forEach((button) => {
      button.addEventListener("click", function () {
        const modal = this.closest(".modal");
        if (modal) {
          modal.style.display = "none";
        }
      });
    });
});

/**
 * Avatar Selection System
 * Handles the selection and preview of member avatars
 * @param {string} modalSelector - The modal containing avatar options
 * @param {string} previewSelector - The preview image element
 * @param {string} inputSelector - The hidden input storing the avatar URL
 */
function initializeAvatarSelection(
  modalSelector,
  previewSelector,
  inputSelector
) {
  const modal = document.querySelector(modalSelector);
  if (!modal) return;

  const avatarOptions = modal.querySelectorAll(".avatar-option");
  const previewImg = modal.querySelector(previewSelector);
  const hiddenInput = modal.querySelector(inputSelector);

  avatarOptions.forEach((option) => {
    option.addEventListener("click", function () {
      // Update selection UI
      avatarOptions.forEach((opt) => opt.classList.remove("selected"));
      this.classList.add("selected");

      // Update preview and form value
      const avatarUrl = this.dataset.avatar;
      previewImg.src = avatarUrl;
      hiddenInput.value = avatarUrl;
    });
  });
}

/**
 * Avatar Upload Preview System
 * Handles the preview of uploaded avatar images
 * @param {string} fileInputSelector - The file input element
 * @param {string} previewSelector - The preview image element
 */
function initializeAvatarUploadPreview(fileInputSelector, previewSelector) {
  const fileInput = document.querySelector(fileInputSelector);
  const previewImg = document.querySelector(previewSelector);

  if (!fileInput || !previewImg) return;

  fileInput.addEventListener("change", function () {
    // Handle file selection and preview
    if (this.files && this.files[0]) {
      const reader = new FileReader();
      reader.onload = function (e) {
        previewImg.src = e.target.result;
      };
      reader.readAsDataURL(this.files[0]);
    }
  });
}
