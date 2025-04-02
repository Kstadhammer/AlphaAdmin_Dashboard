using System.Diagnostics; // Add for Debug.WriteLine
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // Get Members for dropdown
        var members = await _memberService.GetAllMembers();
        ViewBag.Members =
            members
                ?.Where(m => m != null)
                .Select(m => new SelectListItem
                {
                    Value = m.Id?.ToString() ?? string.Empty,
                    Text = $"{m.FirstName ?? ""} {m.LastName ?? ""}".Trim(),
                })
                .ToList() ?? new List<SelectListItem>();

        // Get Clients for dropdown
        var clients = await _clientService.GetAllClientsAsync(); // Assuming this returns List<ClientListItem> or similar
        ViewBag.Clients =
            clients
                ?.Where(c => c != null)
                .Select(c => new SelectListItem
                {
                    Value = c.Id?.ToString() ?? string.Empty,
                    Text = c.ClientName ?? "Unnamed Client", // Use ClientName property
                })
                .ToList() ?? new List<SelectListItem>();

        // Get Statuses for dropdown
        var statusResult = await _statusService.GetStatusesAsync(); // Use correct method name
        if (statusResult.Succeeded && statusResult.Result != null)
        {
            ViewBag.Statuses = statusResult
                .Result.Where(s => s != null)
                .Select(s => new SelectListItem
                {
                    Value = s.Id?.ToString() ?? string.Empty, // Use Status model properties
                    Text = s.Name ?? "Unnamed Status",
                })
                .ToList();
        }
        else
        {
            ViewBag.Statuses = new List<SelectListItem>();
            // Optionally add error handling/logging if statuses fail to load
            // Example: TempData["Error"] = "Failed to load project statuses.";
            Debug.WriteLine(
                $"AdminController.Projects: Failed to load statuses. Succeeded={statusResult.Succeeded}, Error={statusResult.Error ?? "None"}"
            );
        }

        // Get all projects for the main view model
        Debug.WriteLine(
            $"AdminController.Projects: Populated ViewBag.Statuses with {((List<SelectListItem>)ViewBag.Statuses)?.Count ?? 0} items."
        );
        var projects = await _projectService.GetAllProjectsAsync();
        return View(projects); // Pass the list of projects to the view
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
