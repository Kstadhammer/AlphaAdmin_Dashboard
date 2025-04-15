// Helper function to get anti-forgery token
function getAntiForgeryToken() {
  const tokenInput = document.querySelector(
    'input[name="__RequestVerificationToken"]'
  );
  return tokenInput ? tokenInput.value : null;
}

function filterMembers(query) {
  const memberCards = document.querySelectorAll(".member-grid .member-card");
  memberCards.forEach((card) => {
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
 * Member List functionality including dropdown menu and edit/delete operations
 */
document.addEventListener("DOMContentLoaded", function () {
  // Toggle dropdown when clicking on the ellipsis icon
  const dropdownToggles = document.querySelectorAll(
    ".card.member .dropdown-toggle"
  );
  console.log("Found dropdown toggles:", dropdownToggles.length);
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

  // Handle edit member action
  const editMemberButtons = document.querySelectorAll(".edit-member");
  console.log("Found edit member buttons:", editMemberButtons.length);
  editMemberButtons.forEach((button) => {
    button.addEventListener("click", function () {
      const memberId = this.getAttribute("data-member-id");
      document.getElementById("editMemberId").value = memberId;

      // Fetch member data and populate the form
      fetch(`/Members/GetMember/${memberId}`)
        .then((response) => response.json())
        .then((data) => {
          if (data.success) {
            // Match the form field IDs with the asp-for attributes in the form
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

            // Show edit modal
            const modal = document.getElementById("editMemberModal");
            modal.style.display = "flex";
          } else {
            console.error("Failed to fetch member data:", data.message);
          }
        })
        .catch((error) => console.error("Error fetching member data:", error));
    });
  });

  // Handle delete member action
  const deleteMemberButtons = document.querySelectorAll(".delete-member");
  console.log("Found delete member buttons:", deleteMemberButtons.length);
  deleteMemberButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.stopPropagation();

      // Close dropdown menu
      document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
        menu.classList.remove("show");
      });

      const memberId = this.getAttribute("data-member-id");
      const memberName = this.getAttribute("data-member-name");

      document.getElementById("deleteMemberId").value = memberId;
      document.getElementById("deleteMemberName").textContent = memberName;

      // Show delete confirmation modal
      const modal = document.getElementById("deleteMemberModal");
      modal.style.display = "flex";
    });
  });

  // Handle Make Admin action (using modal)
  const makeAdminButtons = document.querySelectorAll(".make-admin");
  makeAdminButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.stopPropagation(); // Prevent dropdown from closing immediately

      // Close other dropdowns first
      document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
        menu.classList.remove("show");
      });

      const memberId = this.getAttribute("data-member-id");
      const memberName = this.getAttribute("data-member-name");

      // Populate the modal
      const assignAdminMemberIdInput = document.getElementById(
        "assignAdminMemberId"
      );
      const assignAdminMemberNameSpan = document.getElementById(
        "assignAdminMemberName"
      );

      if (assignAdminMemberIdInput) assignAdminMemberIdInput.value = memberId;
      if (assignAdminMemberNameSpan)
        assignAdminMemberNameSpan.textContent = memberName;

      // Show the modal
      const modal = document.getElementById("assignAdminModal");
      if (modal) {
        modal.style.display = "flex";
      } else {
        console.error("Assign Admin Modal not found!");
        alert("Error: Assign Admin confirmation dialog not found."); // User feedback
      }
    });
  });

  // Handle close buttons for ALL modals used on this page
  document
    .querySelectorAll(
      '#editMemberModal [data-close="true"], #deleteMemberModal [data-close="true"], #assignAdminModal [data-close="true"]' // Added assignAdminModal
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
