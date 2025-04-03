using System;
using System.Collections.Generic;
using Business.Models;

namespace Presentation.ViewModels;

public class DashboardViewModel
{
    // Summary counts
    public int TotalActiveProjects { get; set; }
    public int TotalClients { get; set; }
    public int TotalMembers { get; set; }

    // Project status distribution
    public List<StatusCount> ProjectStatusDistribution { get; set; } = new List<StatusCount>();

    // Upcoming deadlines
    public List<ProjectDeadline> UpcomingDeadlines { get; set; } = new List<ProjectDeadline>();

    // Team member workload
    public List<MemberWorkload> TeamWorkload { get; set; } = new List<MemberWorkload>();

    // Budget summary
    public decimal TotalBudget { get; set; }
    public decimal AverageBudget { get; set; }
}

public class StatusCount
{
    public string StatusId { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ProjectDeadline
{
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime EndDate { get; set; }
    public int DaysLeft { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}

public class MemberWorkload
{
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int ProjectCount { get; set; }
}
