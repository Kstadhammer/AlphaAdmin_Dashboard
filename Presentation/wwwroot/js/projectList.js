/**
 * Project List functionality including card dropdown menu and delete confirmation
 */
document.addEventListener("DOMContentLoaded", function () {
  // Toggle dropdown when clicking on the ellipsis icon
  const dropdownToggles = document.querySelectorAll(
    ".project-grid .dropdown-toggle"
  );
  dropdownToggles.forEach((toggle) => {
    toggle.addEventListener("click", function (e) {
      e.stopPropagation();
      // Close all other dropdowns
      document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
        if (menu !== this.nextElementSibling) {
          menu.classList.remove("show");
        }
      });
      // Toggle this dropdown
      this.nextElementSibling.classList.toggle("show");
    });
  });

  // Close all dropdowns when clicking elsewhere on the page
  document.addEventListener("click", function () {
    document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
      menu.classList.remove("show");
    });
  });

  // Handle edit project action
  const editProjectButtons = document.querySelectorAll(".edit-project");
  editProjectButtons.forEach((button) => {
    button.addEventListener("click", function () {
      const projectId = this.getAttribute("data-project-id");
      document.getElementById("editProjectId").value = projectId;

      // Fetch project data and populate the form
      fetch(`/Projects/GetProject/${projectId}`)
        .then((response) => response.json())
        .then((data) => {
          if (data.success) {
            // Use edit_ prefixed IDs to target elements in the edit form specifically
            document.getElementById("edit_Name").value = data.project.name;
            document.getElementById("edit_ClientName").value =
              data.project.clientName;
            document.getElementById("edit_Description").value =
              data.project.description || "";

            // Set dates (convert from ISO to input format)
            const startDate = new Date(data.project.startDate);
            const endDate = new Date(data.project.endDate);

            document.getElementById("edit_StartDate").value = startDate
              .toISOString()
              .split("T")[0];
            document.getElementById("edit_EndDate").value = endDate
              .toISOString()
              .split("T")[0];

            document.getElementById("edit_Budget").value = data.project.budget;
            document.getElementById("edit_IsActive").checked =
              data.project.isActive;

            // Set the status dropdown
            if (
              data.project.statusId &&
              document.getElementById("edit_StatusId")
            ) {
              document.getElementById("edit_StatusId").value =
                data.project.statusId;
            }

            // Handle member selection if applicable
            if (
              data.project.memberIds &&
              document.getElementById("edit_MemberIds")
            ) {
              const memberSelect = document.getElementById("edit_MemberIds");

              // First, clear any existing member tags
              const memberSelectionInput = document.querySelector(
                "#editProjectModal .member-selection-input"
              );
              if (memberSelectionInput) {
                const existingTags =
                  memberSelectionInput.querySelectorAll(".member-tag");
                existingTags.forEach((tag) => tag.remove());
              }

              // Set the hidden select values
              for (let i = 0; i < memberSelect.options.length; i++) {
                const isSelected = data.project.memberIds.includes(
                  parseInt(memberSelect.options[i].value)
                );
                memberSelect.options[i].selected = isSelected;

                // If selected, add the member tag to the input
                if (isSelected && memberSelectionInput) {
                  const memberId = memberSelect.options[i].value;
                  const memberName = memberSelect.options[i].text;

                  // Find the member avatar from the member options
                  const memberOption = document.querySelector(
                    `#editProjectModal .member-option[data-id="${memberId}"]`
                  );
                  let memberAvatar = "/images/Avatar_male_1.svg"; // Default avatar

                  if (memberOption) {
                    memberAvatar = memberOption.dataset.avatar;
                  }

                  // Create the member tag
                  const tag = document.createElement("div");
                  tag.className = "member-tag";
                  tag.dataset.id = memberId;

                  tag.innerHTML = `
                    <img src="${memberAvatar}" alt="${memberName}" class="member-tag-avatar">
                    <span class="member-tag-name">${memberName}</span>
                    <span class="member-tag-remove">×</span>
                  `;

                  // Add remove event listener
                  tag
                    .querySelector(".member-tag-remove")
                    .addEventListener("click", function (e) {
                      e.stopPropagation();
                      tag.remove();

                      // Update the hidden select
                      for (let j = 0; j < memberSelect.options.length; j++) {
                        if (memberSelect.options[j].value === memberId) {
                          memberSelect.options[j].selected = false;
                          break;
                        }
                      }
                    });

                  // Insert before search input
                  const searchInput = memberSelectionInput.querySelector(
                    ".member-selection-search"
                  );
                  memberSelectionInput.insertBefore(tag, searchInput);
                }
              }
            }

            console.log("Form populated with project data:", data.project);
          } else {
            console.error("Failed to fetch project data:", data.message);
          }
        })
        .catch((error) => console.error("Error fetching project data:", error));
    });
  });

  // Handle delete project action
  const deleteProjectButtons = document.querySelectorAll(".delete-project");
  deleteProjectButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.stopPropagation();

      // Close dropdown menu
      document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
        menu.classList.remove("show");
      });

      const projectId = this.getAttribute("data-project-id");
      const projectName = this.getAttribute("data-project-name");

      document.getElementById("deleteProjectId").value = projectId;
      document.getElementById("deleteProjectName").textContent = projectName;

      // Show delete confirmation modal
      const modal = document.getElementById("deleteProjectModal");
      modal.style.display = "flex";
    });
  });

  // Handle close buttons for project list specific modals
  document
    .querySelectorAll('#deleteProjectModal [data-close="true"]')
    .forEach((button) => {
      button.addEventListener("click", function () {
        const modal = this.closest(".modal");
        if (modal) {
          modal.style.display = "none";
        }
      });
    });
});
