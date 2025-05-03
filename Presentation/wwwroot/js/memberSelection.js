/**
 * Member Selection Component
 * A custom UI component that provides an enhanced member selection interface with the following features:
 * - Avatar-based member selection
 * - Tag-based selected member display
 * - Real-time search filtering
 * - Multiple member selection support
 * - Synchronization with hidden select element
 *
 * Component Structure:
 * - Container (.member-selection-container)
 *   - Input Container (.member-selection-input)
 *     - Member Tags (.member-tag)
 *     - Search Input (.member-selection-search)
 *   - Dropdown (.member-selection-dropdown)
 *     - Member Options (.member-option)
 *   - Hidden Select (.member-selection-hidden)
 */

document.addEventListener("DOMContentLoaded", function () {
  // Initialize all member selection components on page load
  initializeMemberSelections();

  /**
   * Main initialization function
   * Sets up all member selection components found on the page
   */
  function initializeMemberSelections() {
    // Find all member selection containers in the document
    const memberSelectionContainers = document.querySelectorAll(
      ".member-selection-container"
    );

    memberSelectionContainers.forEach((container) => {
      // Get component elements
      const input = container.querySelector(".member-selection-input");
      const dropdown = container.querySelector(".member-selection-dropdown");
      const searchInput = container.querySelector(".member-selection-search");
      const hiddenSelect = container.querySelector(".member-selection-hidden");

      // Initialize member data storage
      const members = [];
      const memberOptions = container.querySelectorAll(".member-option");

      // Build members array from DOM options
      memberOptions.forEach((option) => {
        members.push({
          id: option.dataset.id,
          name: option.dataset.name,
          avatar: option.dataset.avatar,
        });
      });

      /**
       * Event Handlers
       */

      // Handle input container click - show dropdown and focus search
      input.addEventListener("click", function (e) {
        if (e.target === input || e.target === searchInput) {
          showDropdown();
          searchInput.focus();
        }
      });

      // Handle real-time search filtering
      searchInput.addEventListener("input", function () {
        filterMembers(this.value);
      });

      // Show dropdown when search input is focused
      searchInput.addEventListener("focus", function () {
        showDropdown();
      });

      // Close dropdown when clicking outside component
      document.addEventListener("click", function (e) {
        if (!container.contains(e.target)) {
          hideDropdown();
        }
      });

      // Handle member selection from dropdown
      memberOptions.forEach((option) => {
        option.addEventListener("click", function () {
          const memberId = this.dataset.id;
          const memberName = this.dataset.name;
          const memberAvatar = this.dataset.avatar;

          addMemberTag(memberId, memberName, memberAvatar);
          updateHiddenSelect();
          hideDropdown();
          searchInput.value = "";
          searchInput.focus();
        });
      });

      /**
       * Dropdown Management Functions
       */

      // Display the member selection dropdown
      function showDropdown() {
        dropdown.classList.add("show");
        filterMembers(searchInput.value);
      }

      // Hide the member selection dropdown
      function hideDropdown() {
        dropdown.classList.remove("show");
      }

      /**
       * Member Filtering System
       * Filters and displays member options based on search input
       * Also handles visibility of already selected members
       * @param {string} query - Search term to filter members
       */
      function filterMembers(query) {
        const lowerQuery = query.toLowerCase();

        memberOptions.forEach((option) => {
          const name = option.dataset.name.toLowerCase();

          // Show/hide based on search match
          if (name.includes(lowerQuery)) {
            option.style.display = "flex";
          } else {
            option.style.display = "none";
          }

          // Handle already selected members
          const isSelected = Array.from(hiddenSelect.selectedOptions).some(
            (selected) => selected.value === option.dataset.id
          );

          if (isSelected) {
            option.classList.add("selected");
          } else {
            option.classList.remove("selected");
          }
        });
      }

      /**
       * Member Tag Management
       * Creates and manages the visual tags for selected members
       * @param {string} id - Member ID
       * @param {string} name - Member name
       * @param {string} avatar - URL to member's avatar
       */
      function addMemberTag(id, name, avatar) {
        // Prevent duplicate selection
        if (
          Array.from(hiddenSelect.selectedOptions).some(
            (option) => option.value === id
          )
        ) {
          return;
        }

        // Create the member tag element
        const tag = document.createElement("div");
        tag.className = "member-tag";
        tag.dataset.id = id;

        tag.innerHTML = `
          <img src="${avatar}" alt="${name}" class="member-tag-avatar">
          <span class="member-tag-name">${name}</span>
          <span class="member-tag-remove">×</span>
        `;

        // Add remove functionality
        tag
          .querySelector(".member-tag-remove")
          .addEventListener("click", function (e) {
            e.stopPropagation();
            tag.remove();
            updateHiddenSelect();
          });

        // Add tag to input container
        input.insertBefore(tag, searchInput);

        // Update hidden select
        for (let i = 0; i < hiddenSelect.options.length; i++) {
          if (hiddenSelect.options[i].value === id) {
            hiddenSelect.options[i].selected = true;
            break;
          }
        }
      }

      /**
       * Hidden Select Synchronization
       * Keeps the hidden select element in sync with the visual interface
       * Triggers change events for form handling
       */
      function updateHiddenSelect() {
        // Reset all selections
        for (let i = 0; i < hiddenSelect.options.length; i++) {
          hiddenSelect.options[i].selected = false;
        }

        // Update selections based on visible tags
        const tags = input.querySelectorAll(".member-tag");
        tags.forEach((tag) => {
          const id = tag.dataset.id;
          for (let i = 0; i < hiddenSelect.options.length; i++) {
            if (hiddenSelect.options[i].value === id) {
              hiddenSelect.options[i].selected = true;
              break;
            }
          }
        });

        // Trigger change event for form handling
        const event = new Event("change");
        hiddenSelect.dispatchEvent(event);
      }

      /**
       * Initialize Pre-selected Members
       * Handles setup of any members that were pre-selected in the hidden select
       */
      if (hiddenSelect.multiple) {
        Array.from(hiddenSelect.selectedOptions).forEach((option) => {
          const member = members.find((m) => m.id === option.value);
          if (member) {
            addMemberTag(member.id, member.name, member.avatar);
          }
        });
      }
    });
  }
});
