using System;
using System.Collections.Generic; // Add for List
using System.Linq;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Domain.Models;

namespace Business.Factories
{
    public class ProjectFactory : IProjectFactory
    {
        // List of predefined gradient styles
        private static readonly List<string> _projectGradients = new List<string>
        {
            "linear-gradient(140deg, #FEC887 0%, #F9A486 100%)", // Orange/Peach
            "linear-gradient(140deg, #A091FB 0%, #C08AFB 100%)", // Purple
            "linear-gradient(140deg, #90E6AA 0%, #A3F0BE 100%)", // Green
            "linear-gradient(140deg, #84D9F8 0%, #90E0EF 100%)", // Blue
            "linear-gradient(140deg, #F6A0B6 0%, #F8B5C8 100%)", // Pink
        };
        private static readonly Random _random = new Random();

        public ProjectEntity CreateProjectEntity(
            AddProjectForm form,
            string userId,
            string clientName,
            string? imageUrl = null // Keep allowing specific uploads
        )
        {
            string finalImageUrl = imageUrl;
            // If no image uploaded, use the default project icon
            if (string.IsNullOrEmpty(finalImageUrl))
            {
                finalImageUrl = "/images/projectimage.svg"; // Default icon path
            }

            // Select a random gradient
            string randomGradient = string.Empty;
            if (_projectGradients.Any())
            {
                int randomIndex = _random.Next(_projectGradients.Count);
                randomGradient = _projectGradients[randomIndex];
            }

            return new ProjectEntity
            {
                Name = form.Name,
                ClientName = clientName,
                Description = form.Description ?? string.Empty,
                StartDate = form.StartDate,
                EndDate = form.EndDate,
                Budget = form.Budget,
                CreatedAt = DateTime.UtcNow,
                IsActive = form.IsActive,
                ImageUrl = finalImageUrl, // Use default or uploaded image
                ClientId = form.ClientId,
                StatusId = form.StatusId,
                UserId = userId,
                GradientCss = randomGradient, // Assign the random gradient (New Property)
            };
        }

        public ProjectEntity UpdateProjectEntity(
            ProjectEntity entity,
            EditProjectForm form,
            string? imageUrl = null
        )
        {
            entity.Name = form.Name;
            entity.ClientName = form.ClientName;
            entity.Description = form.Description ?? string.Empty; // Handle null description
            entity.StartDate = form.StartDate;
            entity.EndDate = form.EndDate;
            entity.Budget = form.Budget;
            entity.IsActive = form.IsActive;
            entity.StatusId = form.StatusId; // Update the status ID
            entity.UpdatedAt = DateTime.UtcNow;

            // Only update image if a new one is provided
            if (imageUrl != null)
            {
                entity.ImageUrl = imageUrl ?? string.Empty; // Handle null image URL
            }

            return entity;
        }

        public ProjectListItem CreateProjectListItem(ProjectEntity entity)
        {
            return new ProjectListItem
            {
                Id = entity.Id,
                Name = entity.Name,
                ClientName = entity.ClientName,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Budget = entity.Budget ?? 0, // Handle nullable budget
                IsActive = entity.IsActive,
                ImageUrl = entity.ImageUrl,
                // MemberCount = entity.Members?.Count ?? 0, // Replaced by avatar list
                CreatedAt = entity.CreatedAt,
                StatusId = entity.StatusId ?? string.Empty, // Assign StatusId
                // Ensure entity.Status is loaded for this to work
                StatusName = entity.Status?.Name ?? "Unknown", // Assign StatusName (handle potential null navigation property)
                GradientCss = entity.GradientCss ?? string.Empty, // Assign GradientCss
                // Ensure entity.Members and their ImageUrl are loaded
                MemberAvatarUrls =
                    entity
                        .Members?.Select(m => m.ImageUrl ?? "/images/Avatar.png") // Use default if null
                        .ToList() ?? new List<string>(),
            };
        }

        public EditProjectForm CreateEditProjectForm(ProjectEntity entity)
        {
            return new EditProjectForm
            {
                Id = entity.Id,
                Name = entity.Name,
                ClientName = entity.ClientName,
                Description = entity.Description ?? string.Empty, // Handle potential null from DB? (Unlikely but safe)
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Budget = entity.Budget ?? 0, // Handle nullable budget
                IsActive = entity.IsActive,
                StatusId = entity.StatusId, // Include the status ID
                MemberIds = entity.Members?.Select(m => m.Id).ToList() ?? new List<string>(),
            };
        }

        public Project CreateProjectModel(ProjectEntity entity)
        {
            return new Project
            {
                Id = entity.Id,
                Name = entity.Name,
                ClientName = entity.ClientName,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Budget = entity.Budget ?? 0, // Handle nullable budget
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                IsActive = entity.IsActive,
                Members =
                    entity
                        .Members?.Select(m => new Member
                        {
                            Id = m.Id,
                            FirstName = m.FirstName,
                            LastName = m.LastName,
                            Email = m.Email ?? string.Empty,
                            JobTitle = m.JobTitle ?? string.Empty,
                            Phone = m.PhoneNumber ?? string.Empty,
                        })
                        .ToList() ?? new List<Member>(),
            };
        }
    }
}
