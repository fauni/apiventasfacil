using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserSeriesController : ControllerBase
    {
        private readonly IUserSeriesService _service;

        public UserSeriesController(IUserSeriesService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AssignSeries([FromBody] UserSeriesDto dto)
        {
            var result = await _service.AssignSeriesAsync(dto);
            return Ok(ApiResponse<UserSerie>.Ok(result, "Serie asignada correctamente"));
        }

        [HttpGet("/api/users/{id}/series")]
        public async Task<IActionResult> GetSeriesByUser(int id)
        {
            var result = await _service.GetSeriesByUserAsync(id);
            return Ok(ApiResponse<IEnumerable<UserSerie>>.Ok(result));
        }
    }
}
