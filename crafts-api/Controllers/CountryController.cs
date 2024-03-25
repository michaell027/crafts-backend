using crafts_api.Entities;
using crafts_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace crafts_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService) =>
            _countryService = countryService;


        [HttpGet ("get-countries")]
        public async Task<IActionResult> GetCountries()
        {
            return Ok(await _countryService.GetCountries());
        }

        [HttpPost("get-cities")]
        public async Task<IActionResult> GetCities(string country)
        {
            return Ok(await _countryService.GetCities(country));
        }
    }
}
