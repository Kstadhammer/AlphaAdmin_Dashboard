using System;
using System.Linq;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Domain.Models;

namespace Business.Factories
{
    public class ProjectFactory : IProjectFactory
    {
        public ProjectEntity CreateProjectEntity( // Reverted signature
            AddProjectForm form,
            string userId,
            string? imageUrl = null
        )
        {
            return new ProjectEntity
            {
                Name = form.Name,
                // ClientName = form.ClientName, // Removed as AddProjectForm no longer has ClientName
                Description = form.Description,
                StartDate = form.StartDate,
                EndDate = form.EndDate,
                Budget = form.Budget,
                CreatedAt = DateTime.UtcNow,
                IsActive = form.IsActive,
                ImageUrl = imageUrl,
                ClientId = form.ClientId, // Assign ClientId from form
                StatusId = form.StatusId, // Assign StatusId
                UserId = userId, // Assign UserId
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
            entity.Description = form.Description;
            entity.StartDate = form.StartDate;
            entity.EndDate = form.EndDate;
            entity.Budget = form.Budget;
            entity.IsActive = form.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            // Only update image if a new one is provided
            if (imageUrl != null)
            {
                entity.ImageUrl = imageUrl;
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
                MemberCount = entity.Members?.Count ?? 0,
                CreatedAt = entity.CreatedAt,
            };
        }

        public EditProjectForm CreateEditProjectForm(ProjectEntity entity)
        {
            return new EditProjectForm
            {
                Id = entity.Id,
                Name = entity.Name,
                ClientName = entity.ClientName,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Budget = entity.Budget ?? 0, // Handle nullable budget
                IsActive = entity.IsActive,
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
                            Email = m.Email,
                            JobTitle = m.JobTitle,
                            Phone = m.PhoneNumber,
                        })
                        .ToList() ?? new List<Member>(),
            };
        }
    }
}
