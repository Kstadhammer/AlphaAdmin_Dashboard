using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Business.Interfaces;
using Business.Models; // Contains ProjectListItem, Status
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.ViewModels; // Add using for ViewModel

namespace WebApp.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly IMemberService _memberService;
    private readonly IClientService _clientService;
    private readonly IProjectService _projectService;
    private readonly IStatusService _statusService; // Add Status Service field
    private readonly UserManager<MemberEntity> _userManager;

    public AdminController(
        IMemberService memberService,
        IClientService clientService,
        IProjectService projectService,
        IStatusService statusService, // Add Status Service parameter
        UserManager<MemberEntity> userManager
    )
    {
        _memberService = memberService;
        _clientService = clientService;
        _projectService = projectService;
        _statusService = statusService; // Assign Status Service
        _userManager = userManager;
    }

    private async Task SetCurrentUserAsync()
    {
        var userId = _userManager.GetUserId(User);
        if (userId != null)
        {
            var currentUser = await _memberService.GetCurrentUserAsync(userId);
            if (currentUser != null)
            {
                ViewBag.CurrentUser = currentUser;
            }
        }
    }

    // GET: /Admin/ or /
    [Route("")] // Make this the default route for the controller
    [Route("dashboard")] // Optional explicit route
    public async Task<IActionResult> Index()
    {
        await SetCurrentUserAsync();

        // Fetch data for dashboard
        var projects = await _projectService.GetAllProjectsAsync();
        var clients = await _clientService.GetAllClientsAsync();
        var members = await _memberService.GetAllMembers();
        var statuses = await _statusService.GetStatusesAsync();

        // Create status dictionary for easy lookup
        var statusDict = new Dictionary<string, Business.Models.Status>();
        if (statuses.Succeeded && statuses.Result != null)
        {
            foreach (var status in statuses.Result)
            {
                statusDict[status.Id] = status;
            }
        }

        // Populate ViewModel
        var viewModel = new DashboardViewModel
        {
            // Basic counts
            TotalActiveProjects = projects?.Count(p => p.IsActive) ?? 0,
            TotalClients = clients?.Count ?? 0,
            TotalMembers = members?.Count ?? 0,

            // Project status distribution
            ProjectStatusDistribution =
                projects != null
                    ? projects
                        .GroupBy(p => p.StatusId)
                        .Select(g => new StatusCount
                        {
                            StatusId = g.Key,
                            StatusName = statusDict.TryGetValue(g.Key, out var status)
                                ? status.Name
                                : "Unknown",
                            StatusColor = statusDict.TryGetValue(g.Key, out var statusColor)
                                ? statusColor.Color
                                : "#cccccc",
                            Count = g.Count(),
                        })
                        .OrderBy(s =>
                        {
                            statusDict.TryGetValue(s.StatusId, out var orderStatus);
                            return orderStatus?.Order ?? 999;
                        })
                        .ToList()
                    : new List<StatusCount>(),

            // Upcoming deadlines (projects ending in the next 30 days)
            UpcomingDeadlines =
                projects != null
                    ? projects
                        .Where(p =>
                            p.IsActive
                            && (p.EndDate - DateTime.Now).TotalDays <= 30
                            && (p.EndDate - DateTime.Now).TotalDays >= 0
                        )
                        .OrderBy(p => p.EndDate)
                        .Take(5)
                        .Select(p => new ProjectDeadline
                        {
                            ProjectId = p.Id,
                            ProjectName = p.Name,
                            ClientName = p.ClientName,
                            EndDate = p.EndDate,
                            DaysLeft = (int)(p.EndDate - DateTime.Now).TotalDays,
                            StatusName = statusDict.TryGetValue(p.StatusId, out var nameStatus)
                                ? nameStatus.Name
                                : "Unknown",
                            StatusColor = statusDict.TryGetValue(p.StatusId, out var colorStatus)
                                ? colorStatus.Color
                                : "#cccccc",
                        })
                        .ToList()
                    : new List<ProjectDeadline>(),

            // Team member workload
            TeamWorkload =
                members != null
                    ? members
                        .Select(m => new MemberWorkload
                        {
                            MemberId = m.Id,
                            MemberName = $"{m.FirstName} {m.LastName}",
                            ImageUrl = string.IsNullOrEmpty(m.ImageUrl)
                                ? "/images/Avatar_male_1.svg"
                                : m.ImageUrl,
                            // Since ProjectListItem doesn't have Members property, we'll just count active projects
                            // In a real implementation, you would need to fetch the project-member relationships
                            ProjectCount = projects != null ? projects.Count(p => p.IsActive) : 0,
                        })
                        .OrderByDescending(m => m.ProjectCount)
                        .Take(5)
                        .ToList()
                    : new List<MemberWorkload>(),

            // Budget summary
            TotalBudget = projects != null ? projects.Where(p => p.IsActive).Sum(p => p.Budget) : 0,
            AverageBudget =
                projects != null && projects.Any(p => p.IsActive)
                    ? projects.Where(p => p.IsActive).Average(p => p.Budget)
                    : 0,
        };

        return View(viewModel); // Pass ViewModel to the view
    }

    [Route("projects")]
    public async Task<IActionResult> Projects()
    {
        await SetCurrentUserAsync();

        // Prepare data for dropdowns (needed for Add/Edit modals)
        var members = await _memberService.GetAllMembers();
        ViewBag.Members =
            members
                ?.Select(m => new SelectListItem
                {
                    Value = m.Id,
                    Text = $"{m.FirstName} {m.LastName}",
                })
                .ToList() ?? new List<SelectListItem>();

        var clients = await _clientService.GetAllClientsAsync();
        ViewBag.Clients =
            clients?.Select(c => new SelectListItem { Value = c.Id, Text = c.ClientName }).ToList()
            ?? new List<SelectListItem>();

        var statusResult = await _statusService.GetStatusesAsync();
        var statuses = statusResult.Succeeded
            ? statusResult.Result ?? Enumerable.Empty<Status>()
            : Enumerable.Empty<Status>();
        ViewBag.Statuses = statuses
            .Select(s => new SelectListItem { Value = s.Id, Text = s.Name })
            .ToList();

        // Get all projects for the list and counts
        var projects = await _projectService.GetAllProjectsAsync(); // This returns List<ProjectListItem>
        Debug.WriteLine(
            $"AdminController.Projects: Fetched {projects?.Count ?? 0} projects from service."
        ); // Log fetched project count

        // Calculate counts for the filter bar
        var statusFilters = statuses
            .Select(s => new StatusFilterInfo
            {
                StatusId = s.Id,
                StatusName = s.Name,
                Count = projects.Count(p => p.StatusId == s.Id), // Calculate count using StatusId
                // Removed duplicate placeholder Count assignment
            })
            .ToList();

        // Create the ViewModel
        var viewModel = new ProjectIndexViewModel
        {
            Projects = projects, // Pass the full list for now
            StatusFilters = statusFilters,
            TotalProjectCount = projects.Count,
        };

        Debug.WriteLine(
            $"AdminController.Projects: Passing {viewModel.Projects?.Count() ?? 0} projects to the view via ViewModel."
        ); // Log count passed to view
        return View(viewModel); // Pass the ViewModel to the view
    }

    [Route("members")]
    public async Task<IActionResult> Members()
    {
        await SetCurrentUserAsync();
        var members = await _memberService.GetAllMembers();
        return View(members);
    }

    [Route("clients")]
    public async Task<IActionResult> Clients()
    {
        await SetCurrentUserAsync();
        var clients = await _clientService.GetAllClientsAsync();
        return View(clients);
    }
}
