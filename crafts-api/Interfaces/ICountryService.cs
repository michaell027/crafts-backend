using crafts_api.Entities;

namespace crafts_api.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<string>> GetCountries();

        Task<IEnumerable<string>> GetCities(string countryName);
    }
}
