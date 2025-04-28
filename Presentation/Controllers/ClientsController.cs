using Business.Forms;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

/// <summary>
/// Controller responsible for handling client-related CRUD operations.
/// Actions typically redirect back to the main client list in the AdminController.
/// </summary>
[Route("[controller]")] // Route: /Clients
public class ClientsController : Controller
{
    private readonly IClientService _clientService;
    private readonly IMemberService _memberService;
    private readonly UserManager<MemberEntity> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientsController"/> class.
    /// </summary>
    /// <param name="clientService">Service for client data operations.</param>
    /// <param name="memberService">Service for member data (used for layout).</param>
    /// <param name="userManager">ASP.NET Core Identity UserManager (used for layout).</param>
    public ClientsController(
        IClientService clientService,
        IMemberService memberService,
        UserManager<MemberEntity> userManager
    )
    {
        _clientService = clientService;
        _memberService = memberService;
        _userManager = userManager;
    }

    /// <summary>
    /// Helper method to set the current user information in ViewBag.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// API endpoint to retrieve client details for editing (GET: /Clients/GetClient/{id}).
    /// </summary>
    /// <param name="id">The unique identifier of the client.</param>
    /// <returns>JSON response containing client details or an error message.</returns>
    [HttpGet]
    [Route("GetClient/{id}")] // Keep explicit route for clarity
    public async Task<IActionResult> GetClient(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return Json(new { success = false, error = "Invalid client ID" });
        }

        var client = await _clientService.GetClientForEditAsync(id);

        if (client == null)
        {
            return Json(new { success = false, error = "Client not found" });
        }

        return Json(
            new
            {
                success = true,
                client = new
                {
                    clientName = client.ClientName,
                    email = client.Email,
                    location = client.Location,
                    phone = client.Phone,
                },
            }
        );
    }

    /// <summary>
    /// Handles the HTTP POST request to add a new client (POST: /Clients/AddClient).
    /// </summary>
    /// <param name="form">The form containing the new client information.</param>
    /// <returns>Redirects to the clients list page (/Admin/Clients) with a status message.</returns>
    [HttpPost]
    [Route("AddClient")] // Explicit route
    public async Task<IActionResult> AddClient(AddClientForm form)
    {
        if (!ModelState.IsValid)
        {
            // Return to the clients page with validation errors
            return RedirectToAction("Clients", "Admin");
        }

        var success = await _clientService.AddClientAsync(form);
        if (!success)
        {
            TempData["Error"] = "Failed to add client. Please try again.";
            return RedirectToAction("Clients", "Admin");
        }

        TempData["Success"] = "Client added successfully!";
        return RedirectToAction("Clients", "Admin");
    }

    /// <summary>
    /// Handles the HTTP POST request to edit an existing client (POST: /Clients/EditClient).
    /// </summary>
    /// <param name="form">The form containing the updated client information.</param>
    /// <returns>Redirects to the clients list page (/Admin/Clients) with a status message.</returns>
    [HttpPost]
    [Route("EditClient")] // Explicit route
    public async Task<IActionResult> EditClient(EditClientForm form)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Clients", "Admin");
        }

        var success = await _clientService.EditClientAsync(form);
        if (!success)
        {
            TempData["Error"] = "Failed to update client. Please try again.";
            return RedirectToAction("Clients", "Admin");
        }

        TempData["Success"] = "Client updated successfully!";
        return RedirectToAction("Clients", "Admin");
    }

    /// <summary>
    /// Handles the HTTP POST request to delete a client (POST: /Clients/DeleteClient).
    /// </summary>
    /// <param name="id">The unique identifier of the client to delete.</param>
    /// <returns>Redirects to the clients list page (/Admin/Clients) with a status message.</returns>
    [HttpPost]
    [Route("DeleteClient")] // Explicit route
    public async Task<IActionResult> DeleteClient(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            TempData["Error"] = "Invalid client ID.";
            return RedirectToAction("Clients", "Admin");
        }

        var success = await _clientService.DeleteClientAsync(id);
        if (!success)
        {
            // Provide a more specific error message
            TempData["Error"] =
                "Failed to delete client. Ensure the client has no associated projects.";
        }
        else
        {
            TempData["Success"] = "Client deleted successfully!";
        }

        return RedirectToAction("Clients", "Admin");
    }
}
