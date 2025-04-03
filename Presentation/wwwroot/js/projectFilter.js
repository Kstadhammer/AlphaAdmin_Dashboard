/**
 * Project filtering functionality for status tabs
 */
document.addEventListener("DOMContentLoaded", function () {
  // Get all filter tabs and project cards
  const filterTabs = document.querySelectorAll(".filter-tab");
  const projectCards = document.querySelectorAll(".project-card");

  // Add click event to each filter tab
  filterTabs.forEach((tab) => {
    tab.addEventListener("click", function (e) {
      e.preventDefault();

      // Remove active class from all tabs
      filterTabs.forEach((t) => t.classList.remove("active"));

      // Add active class to clicked tab
      this.classList.add("active");

      // Get the status to filter by
      const statusFilter = this.getAttribute("data-status");

      // Show/hide project cards based on status with animation
      let visibleCount = 0;
      projectCards.forEach((card) => {
        const cardStatus = card.getAttribute("data-status");

        if (statusFilter === "all" || statusFilter === cardStatus) {
          // Show the card with animation
          card.style.opacity = "0";
          card.style.transform = "translateY(10px)";
          card.style.display = "";
          visibleCount++;

          // Use setTimeout to create a staggered animation effect
          setTimeout(() => {
            card.style.opacity = "1";
            card.style.transform = "translateY(0)";
          }, 50);
        } else {
          // Hide the card
          card.style.opacity = "0";
          card.style.transform = "translateY(10px)";

          // Wait for animation to complete before hiding
          setTimeout(() => {
            card.style.display = "none";
          }, 300);
        }
      });

      // Show/hide the "no filtered projects" message
      const noFilteredProjects = document.querySelector(
        ".no-filtered-projects"
      );
      if (noFilteredProjects) {
        if (visibleCount === 0 && projectCards.length > 0) {
          // We have projects but none match the filter
          setTimeout(() => {
            noFilteredProjects.style.display = "block";
            noFilteredProjects.style.opacity = "0";
            noFilteredProjects.style.transform = "translateY(10px)";

            setTimeout(() => {
              noFilteredProjects.style.opacity = "1";
              noFilteredProjects.style.transform = "translateY(0)";
            }, 50);
          }, 300);
        } else {
          noFilteredProjects.style.display = "none";
        }
      }

      // Update the URL with the selected filter (without page reload)
      const url = new URL(window.location.href);
      if (statusFilter === "all") {
        url.searchParams.delete("status");
      } else {
        url.searchParams.set("status", statusFilter);
      }
      window.history.pushState({}, "", url);
    });
  });

  // Check if there's a status filter in the URL on page load
  const urlParams = new URLSearchParams(window.location.search);
  const statusParam = urlParams.get("status");

  if (statusParam) {
    // Find the tab with the matching status and click it
    const matchingTab = document.querySelector(
      `.filter-tab[data-status="${statusParam}"]`
    );
    if (matchingTab) {
      matchingTab.click();
    }
  } else {
    // If no filter in URL, ensure all cards are visible with animation
    projectCards.forEach((card, index) => {
      // Set initial state
      card.style.opacity = "0";
      card.style.transform = "translateY(10px)";

      // Stagger the animations
      setTimeout(() => {
        card.style.opacity = "1";
        card.style.transform = "translateY(0)";
      }, 50 * index); // Stagger based on index
    });
  }

  // Log for debugging
  console.log(
    `Found ${filterTabs.length} filter tabs and ${projectCards.length} project cards`
  );
});
