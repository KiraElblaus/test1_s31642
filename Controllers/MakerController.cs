using APBD_TEST_TEMPLATE.DTOs;
using APBD_TEST_TEMPLATE.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_TEST_TEMPLATE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MakersController : ControllerBase
{
    private readonly IMakerService _service;

    public MakersController(IMakerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetMakers([FromQuery] string? name)
    {
        var makers = await _service.GetMakersAsync(name);
        return Ok(makers);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMaker([FromBody] CreateMakerRequest request)
    {
        await _service.CreateMakerAsync(request);
        return Created();
    }
}