using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UsersController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            var createdUser = await _userService.CreateAsync(user);
            return Ok(ApiResponse<User>.Ok(createdUser, "Usuario creado exitosamente."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.LoginAsync(request.Username, request.Password);

            if (user == null) return Unauthorized(ApiResponse<string>.Fail("Nombre de usuario o contraseña no válidos."));

            var token = _jwtService.GenerateToken(user);
            
            return Ok(ApiResponse<Object>.Ok(new 
            {
                token,
                user = new {
                    user.Id,
                    user.Name,
                    user.Username,
                    user.Email,
                    user.Type
                }
            }, "Inicio de sesión exitosa."));
        }
    }
}
