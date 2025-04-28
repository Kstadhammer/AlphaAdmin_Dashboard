using System;
using System.Collections.Generic;
using Business.Models;
using Domain.Models;

namespace Presentation.ViewModels;

/// <summary>
/// ViewModel for the main dashboard of the application. Contains all necessary data
/// to render summary information, charts, and statistics for the admin dashboard.
/// </summary>
public class DashboardViewModel
{
    /// <summary>
    /// Total number of active projects in the system
    /// </summary>
    public int TotalActiveProjects { get; set; }

    /// <summary>
    /// Total number of clients registered in the system
    /// </summary>
    public int TotalClients { get; set; }

    /// <summary>
    /// Total number of team members (users) registered in the system
    /// </summary>
    public int TotalMembers { get; set; }

    /// <summary>
    /// Total number of administrators with elevated privileges
    /// </summary>
    public int TotalAdmins { get; set; }

    /// <summary>
    /// Distribution of projects by status - used for generating pie/doughnut charts
    /// </summary>
    public List<StatusCount> ProjectStatusDistribution { get; set; } = new List<StatusCount>();

    /// <summary>
    /// List of projects with approaching deadlines - sorted by closest deadline first
    /// </summary>
    public List<ProjectDeadline> UpcomingDeadlines { get; set; } = new List<ProjectDeadline>();

    /// <summary>
    /// Workload information for team members based on project assignments
    /// </summary>
    public List<MemberWorkload> TeamWorkload { get; set; } = new List<MemberWorkload>();

    /// <summary>
    /// List of users with administrative privileges
    /// </summary>
    public List<Member> AdminMembers { get; set; } = new List<Member>();

    /// <summary>
    /// Total budget across all active projects
    /// </summary>
    public decimal TotalBudget { get; set; }

    /// <summary>
    /// Average budget per project
    /// </summary>
    public decimal AverageBudget { get; set; }
}

/// <summary>
/// Represents the count of projects for each status category
/// Used for generating charts and statistics on the dashboard
/// </summary>
public class StatusCount
{
    /// <summary>
    /// Unique identifier for the status
    /// </summary>
    public string StatusId { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the status (e.g., "In Progress", "Completed")
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// Color code used for visual representation in charts/UI
    /// </summary>
    public string StatusColor { get; set; } = string.Empty;

    /// <summary>
    /// Number of projects that have this status
    /// </summary>
    public int Count { get; set; }
}

/// <summary>
/// Contains deadline information for a project, used to display upcoming deadlines
/// on the dashboard and alert administrators to projects that need attention
/// </summary>
public class ProjectDeadline
{
    /// <summary>
    /// Unique identifier for the project
    /// </summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the project
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the client associated with the project
    /// </summary>
    public string ClientName { get; set; } = string.Empty;

    /// <summary>
    /// The deadline/end date of the project
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Number of days remaining until the deadline
    /// </summary>
    public int DaysLeft { get; set; }

    /// <summary>
    /// Current status name of the project
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// Color code associated with the current status
    /// </summary>
    public string StatusColor { get; set; } = string.Empty;

    /// <summary>
    /// URLs to avatar images of team members assigned to this project
    /// </summary>
    public List<string> MemberAvatarUrls { get; set; } = new List<string>();
}

/// <summary>
/// Represents the workload of a team member based on project assignments
/// Used to track resource allocation and identify overloaded team members
/// </summary>
public class MemberWorkload
{
    /// <summary>
    /// Unique identifier for the team member
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the team member
    /// </summary>
    public string MemberName { get; set; } = string.Empty;

    /// <summary>
    /// URL to the team member's profile image/avatar
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Number of projects the team member is currently assigned to
    /// </summary>
    public int ProjectCount { get; set; }
}
