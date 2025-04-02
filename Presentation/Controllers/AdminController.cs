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
    private readonly UserManager<MemberEntity> _userManager;

    public AdminController(
        IMemberService memberService,
        IClientService clientService,
        IProjectService projectService,
        UserManager<MemberEntity> userManager
    )
    {
        _memberService = memberService;
        _clientService = clientService;
        _projectService = projectService;
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

        // Get all members for the dropdowns
        var members = await _memberService.GetAllMembers();
        ViewBag.Members =
            members != null
                ? members
                    .Where(m => m != null)
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id?.ToString() ?? "0",
                        Text = $"{m.FirstName ?? ""} {m.LastName ?? ""}".Trim(),
                    })
                    .ToList()
                : new List<SelectListItem>();

        // Get all projects
        var projects = await _projectService.GetAllProjectsAsync();
        return View(projects);
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
