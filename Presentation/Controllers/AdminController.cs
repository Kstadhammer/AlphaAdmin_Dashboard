using System.Diagnostics; // Add for Debug.WriteLine
using System.Diagnostics; // Keep for logging
using System.Linq; // Add for Linq methods like Count()
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

    // GET
    public async Task<IActionResult> Index()
    {
        await SetCurrentUserAsync();
        return View();
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
