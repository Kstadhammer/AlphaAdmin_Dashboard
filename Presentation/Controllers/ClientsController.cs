using Business.Forms;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class ClientsController : Controller
{
    private readonly IClientService _clientService;
    private readonly IMemberService _memberService;
    private readonly UserManager<MemberEntity> _userManager;

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

    [HttpGet]
    [Route("[controller]/GetClient/{id}")]
    public async Task<IActionResult> GetClient(int id)
    {
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

    [HttpPost]
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

    [HttpPost]
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

    [HttpPost]
    public async Task<IActionResult> DeleteClient(int id)
    {
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
