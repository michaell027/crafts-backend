using crafts_api.Entities.Dto;
using crafts_api.Entities.Models;
using crafts_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace crafts_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CraftersController : ControllerBase
    {
        private readonly ICraftersService _craftsmanService;

        public CraftersController(ICraftersService craftsmanService) => _craftsmanService = craftsmanService;
        

        [HttpGet ("get-craftsman")]
        public async Task<IActionResult> GetCraftsman(Guid craftsmanPublicId)
        {
            // var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var craftsmanProfile = await _craftsmanService.GetCraftsmanProfile(craftsmanPublicId);
            return Ok(craftsmanProfile);
        }

        [HttpPost("add-service")]
        public async Task<IActionResult> AddService(AddServiceRequest addServiceRequest)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _craftsmanService.AddService(addServiceRequest, token);
            return Ok();
        }
        
        [HttpPost("update-craftsman-profile")]
        public async Task<IActionResult> UpdateCraftsmanProfile(UpdateCraftsmanProfileRequest updateCraftsmanProfileRequest)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await _craftsmanService.UpdateCraftsmanProfile(updateCraftsmanProfileRequest, token);
            return Ok();
        }
    }
}
