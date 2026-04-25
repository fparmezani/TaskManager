using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Users;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(request, cancellationToken);
        return result.IsSuccess ? Created("api/auth/me", result.Value) : BadRequest(new { message = result.Error });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.LoginAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { message = result.Error });
    }

    [Authorize]
    [HttpGet("secure")]
    public IActionResult SecureEndpoint()
    {
        return Ok(new { message = "You are authenticated." });
    }

    [AllowAnonymous]
    [HttpGet("public")]
    public IActionResult PublicEndpoint()
    {
        return Ok(new { message = "This endpoint is public." });
    }
}
