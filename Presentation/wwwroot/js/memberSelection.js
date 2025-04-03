/**
 * Member Selection Component
 * Provides a custom member selection interface with avatars
 */

document.addEventListener("DOMContentLoaded", function () {
  // Initialize all member selection components
  initializeMemberSelections();

  function initializeMemberSelections() {
    // Find all member selection containers
    const memberSelectionContainers = document.querySelectorAll(".member-selection-container");
    
    memberSelectionContainers.forEach(container => {
      const input = container.querySelector(".member-selection-input");
      const dropdown = container.querySelector(".member-selection-dropdown");
      const searchInput = container.querySelector(".member-selection-search");
      const hiddenSelect = container.querySelector(".member-selection-hidden");
      
      // Store member data
      const members = [];
      const memberOptions = container.querySelectorAll(".member-option");
      
      // Populate members array from options
      memberOptions.forEach(option => {
        members.push({
          id: option.dataset.id,
          name: option.dataset.name,
          avatar: option.dataset.avatar
        });
      });
      
      // Handle input container click
      input.addEventListener("click", function(e) {
        if (e.target === input || e.target === searchInput) {
          showDropdown();
          searchInput.focus();
        }
      });
      
      // Handle search input
      searchInput.addEventListener("input", function() {
        filterMembers(this.value);
      });
      
      // Handle search input focus
      searchInput.addEventListener("focus", function() {
        showDropdown();
      });
      
      // Handle document click to close dropdown
      document.addEventListener("click", function(e) {
        if (!container.contains(e.target)) {
          hideDropdown();
        }
      });
      
      // Handle member option click
      memberOptions.forEach(option => {
        option.addEventListener("click", function() {
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
      
      // Function to show dropdown
      function showDropdown() {
        dropdown.classList.add("show");
        filterMembers(searchInput.value);
      }
      
      // Function to hide dropdown
      function hideDropdown() {
        dropdown.classList.remove("show");
      }
      
      // Function to filter members based on search
      function filterMembers(query) {
        const lowerQuery = query.toLowerCase();
        
        memberOptions.forEach(option => {
          const name = option.dataset.name.toLowerCase();
          
          if (name.includes(lowerQuery)) {
            option.style.display = "flex";
          } else {
            option.style.display = "none";
          }
          
          // Hide already selected members
          const isSelected = Array.from(hiddenSelect.selectedOptions).some(
            selected => selected.value === option.dataset.id
          );
          
          if (isSelected) {
            option.classList.add("selected");
          } else {
            option.classList.remove("selected");
          }
        });
      }
      
      // Function to add member tag
      function addMemberTag(id, name, avatar) {
        // Check if member is already selected
        if (Array.from(hiddenSelect.selectedOptions).some(option => option.value === id)) {
          return;
        }
        
        // Create member tag
        const tag = document.createElement("div");
        tag.className = "member-tag";
        tag.dataset.id = id;
        
        tag.innerHTML = `
          <img src="${avatar}" alt="${name}" class="member-tag-avatar">
          <span class="member-tag-name">${name}</span>
          <span class="member-tag-remove">×</span>
        `;
        
        // Add remove event listener
        tag.querySelector(".member-tag-remove").addEventListener("click", function(e) {
          e.stopPropagation();
          tag.remove();
          updateHiddenSelect();
        });
        
        // Insert before search input
        input.insertBefore(tag, searchInput);
        
        // Select the option in the hidden select
        for (let i = 0; i < hiddenSelect.options.length; i++) {
          if (hiddenSelect.options[i].value === id) {
            hiddenSelect.options[i].selected = true;
            break;
          }
        }
      }
      
      // Function to update hidden select based on tags
      function updateHiddenSelect() {
        // Clear all selections
        for (let i = 0; i < hiddenSelect.options.length; i++) {
          hiddenSelect.options[i].selected = false;
        }
        
        // Select based on tags
        const tags = input.querySelectorAll(".member-tag");
        tags.forEach(tag => {
          const id = tag.dataset.id;
          for (let i = 0; i < hiddenSelect.options.length; i++) {
            if (hiddenSelect.options[i].value === id) {
              hiddenSelect.options[i].selected = true;
              break;
            }
          }
        });
        
        // Trigger change event
        const event = new Event("change");
        hiddenSelect.dispatchEvent(event);
      }
      
      // Initialize with any pre-selected members
      if (hiddenSelect.multiple) {
        Array.from(hiddenSelect.selectedOptions).forEach(option => {
          const member = members.find(m => m.id === option.value);
          if (member) {
            addMemberTag(member.id, member.name, member.avatar);
          }
        });
      }
    });
  }
});
