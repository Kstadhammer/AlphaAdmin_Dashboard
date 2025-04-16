function filterClients(query) {
  const clientCards = document.querySelectorAll(
    ".client-grid .client-card, .client-list .client-card"
  );
  clientCards.forEach((card) => {
    const allText = Array.from(card.querySelectorAll("td, div"))
      .map((el) => el.textContent.toLowerCase())
      .join(" ");

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
  // Toggle dropdown when clicking on the ellipsis icon
  const dropdownToggles = document.querySelectorAll(".dropdown-toggle");
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

  // Handle edit client action
  const editClientButtons = document.querySelectorAll(".edit-client");
  editClientButtons.forEach((button) => {
    button.addEventListener("click", function () {
      const clientId = this.getAttribute("data-client-id");

      // Get the modal and show it
      const modal = document.getElementById("editClientModal");
      modal.style.display = "flex";

      // Set the ID in the hidden field
      const idField = document.getElementById("editClientId");
      if (idField) {
        idField.value = clientId;
      }

      // Fetch client data
      fetch(`/Clients/GetClient/${clientId}`)
        .then((response) => response.json())
        .then((data) => {
          if (data.success) {
            // Get form and inputs
            const form = modal.querySelector("form");
            const inputs = form.querySelectorAll(
              'input:not([type="hidden"]):not([type="file"])'
            );

            // Populate form fields using placeholder text to identify fields
            inputs.forEach((input) => {
              const placeholder = input.getAttribute("placeholder") || "";

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

            // Additional approach - try to find by label text if needed
            if (!inputs.length) {
              const labels = form.querySelectorAll("label");
              labels.forEach((label) => {
                const labelText = label.textContent.trim();

                // Find the nearest input after this label
                let input = null;
                let el = label.nextElementSibling;
                while (el && !input) {
                  input = el.querySelector(
                    'input:not([type="hidden"]):not([type="file"])'
                  );
                  if (!input) el = el.nextElementSibling;
                }

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

  // Handle delete client action
  const deleteClientButtons = document.querySelectorAll(".delete-client");
  deleteClientButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.stopPropagation();

      // Close dropdown menu
      document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
        menu.classList.remove("show");
      });

      const clientId = this.getAttribute("data-client-id");
      const clientName = this.getAttribute("data-client-name");

      document.getElementById("deleteClientId").value = clientId;
      document.getElementById("deleteClientName").textContent = clientName;

      // Show delete confirmation modal
      const modal = document.getElementById("deleteClientModal");
      modal.style.display = "flex";
    });
  });

  // Handle close buttons for client list specific modals
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
