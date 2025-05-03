/**
 * Client List Management System
 * Handles all client-related functionality including:
 * - Client card display and filtering
 * - Client CRUD operations (Create, Read, Update, Delete)
 * - Dropdown menu interactions
 * - Modal management for edit and delete operations
 * - Form population and data handling
 */

/**
 * Client Filtering System
 * Filters client cards based on search input
 * @param {string} query - The search term to filter by (case-insensitive)
 */
function filterClients(query) {
  // Get all client cards from both grid and list views
  const clientCards = document.querySelectorAll(
    ".client-grid .client-card, .client-list .client-card"
  );

  clientCards.forEach((card) => {
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

/**
 * Client List functionality including dropdown menu and delete confirmation
 */
document.addEventListener("DOMContentLoaded", function () {
  /**
   * Dropdown Menu Management
   * Handles the ellipsis menu for each client card
   */
  const dropdownToggles = document.querySelectorAll(".dropdown-toggle");
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
   * Edit Client Functionality
   * Handles populating and displaying the edit client modal
   */
  const editClientButtons = document.querySelectorAll(".edit-client");
  editClientButtons.forEach((button) => {
    button.addEventListener("click", function () {
      const clientId = this.getAttribute("data-client-id");

      // Initialize and display edit modal
      const modal = document.getElementById("editClientModal");
      modal.style.display = "flex";

      // Set client ID in hidden field
      const idField = document.getElementById("editClientId");
      if (idField) {
        idField.value = clientId;
      }

      // Fetch and populate client data
      fetch(`/Clients/GetClient/${clientId}`)
        .then((response) => response.json())
        .then((data) => {
          if (data.success) {
            // Get form elements
            const form = modal.querySelector("form");
            const inputs = form.querySelectorAll(
              'input:not([type="hidden"]):not([type="file"])'
            );

            /**
             * Form Population Strategy 1: Using Placeholders
             * Identifies form fields using their placeholder text
             */
            inputs.forEach((input) => {
              const placeholder = input.getAttribute("placeholder") || "";

              // Map client data to form fields based on placeholders
              if (
                placeholder.includes("Client Name") ||
                placeholder.includes("Enter Client Name")
              ) {
                input.value = data.client.clientName;
              } else if (
                placeholder.includes("Email") ||
                placeholder.includes("Enter Email")
              ) {
                input.value = data.client.email;
              } else if (
                placeholder.includes("Location") ||
                placeholder.includes("Enter Location")
              ) {
                input.value = data.client.location || "";
              } else if (
                placeholder.includes("Phone") ||
                placeholder.includes("Enter Phone")
              ) {
                input.value = data.client.phone || "";
              }
            });

            /**
             * Form Population Strategy 2: Using Labels
             * Fallback approach that identifies fields by their label text
             * Used when placeholder strategy doesn't find all fields
             */
            if (!inputs.length) {
              const labels = form.querySelectorAll("label");
              labels.forEach((label) => {
                const labelText = label.textContent.trim();

                // Find the nearest input after each label
                let input = null;
                let el = label.nextElementSibling;
                while (el && !input) {
                  input = el.querySelector(
                    'input:not([type="hidden"]):not([type="file"])'
                  );
                  if (!input) el = el.nextElementSibling;
                }

                // Map client data to form fields based on label text
                if (input) {
                  if (labelText.includes("Client Name")) {
                    input.value = data.client.clientName;
                  } else if (labelText.includes("Email")) {
                    input.value = data.client.email;
                  } else if (labelText.includes("Location")) {
                    input.value = data.client.location || "";
                  } else if (labelText.includes("Phone")) {
                    input.value = data.client.phone || "";
                  }
                }
              });
            }
          }
        })
        .catch((error) => {
          console.error("Error fetching client data:", error);
        });
    });
  });

  /**
   * Delete Client Functionality
   * Handles the delete confirmation modal and process
   */
  const deleteClientButtons = document.querySelectorAll(".delete-client");
  deleteClientButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.stopPropagation();

      // Close any open dropdown menu
      document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
        menu.classList.remove("show");
      });

      // Set up delete confirmation modal
      const clientId = this.getAttribute("data-client-id");
      const clientName = this.getAttribute("data-client-name");

      document.getElementById("deleteClientId").value = clientId;
      document.getElementById("deleteClientName").textContent = clientName;

      // Show the confirmation modal
      const modal = document.getElementById("deleteClientModal");
      modal.style.display = "flex";
    });
  });

  /**
   * Modal Management
   * Handles closing of client-specific modals
   */
  document
    .querySelectorAll('#deleteClientModal [data-close="true"]')
    .forEach((button) => {
      button.addEventListener("click", function () {
        const modal = this.closest(".modal");
        if (modal) {
          modal.style.display = "none";
        }
      });
    });
});
