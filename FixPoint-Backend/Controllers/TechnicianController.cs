using FixPoint_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using FixPoint_Backend.Models;
using FixPoint_Backend.Services.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FixPoint_Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TechnicianController : ControllerBase
{
    private readonly TechnicianService _technicianService;
    private readonly IAuthService _authService;

    public TechnicianController(TechnicianService technicianService, IAuthService authService)
    {
        _technicianService = technicianService;
        _authService = authService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult AddTechnician([FromBody] TechnicianInputModel technicianInput)
    {
        // Hash the password with the salt using AuthService
        string hashedPassword = _authService.HashPassword(technicianInput.Password);

        // Create Technician object
        Technician technician = new Technician
        {
            Name = technicianInput.Name,
            Email = technicianInput.Email,
            Password = hashedPassword,
            Salt = null //field exists in db
        };

        _technicianService.AddTechnician(technician);
        return Ok(new
        {
            technician.ID,
            technician.Name,
            technician.Email
        });
    }

    [HttpGet("[action]")]
    public IActionResult GetTechnicianById(Guid id)
    {
        Technician t = _technicianService.GetTechnician(id);
        if (t == null || t.ID != id)
        {
            return NotFound("Technician not found");
        }
        return Ok(t);
    }

    [HttpGet("[action]")]
    public IActionResult GetTechnicians()
    {
        List<Technician> tlist = _technicianService.GetTechnicians();
        if (tlist.Count == 0)
        {
            return NotFound("No technicians found");
        }
        return Ok(tlist);
    }
}