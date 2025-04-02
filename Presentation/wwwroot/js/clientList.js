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
      document.getElementById("editClientId").value = clientId;

      // Fetch client data and populate the form
      fetch(`/Clients/GetClient/${clientId}`)
        .then((response) => response.json())
        .then((data) => {
          if (data.success) {
            document.getElementById("ClientName").value =
              data.client.clientName;
            document.getElementById("Email").value = data.client.email;
            document.getElementById("Location").value =
              data.client.location || "";
            document.getElementById("Phone").value = data.client.phone || "";
          }
        })
        .catch((error) => console.error("Error fetching client data:", error));
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
