using crafts_api.Entities;
using crafts_api.exceptions;
using crafts_api.Interfaces;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace crafts_api.Services
{
    class CountryData
    {
        public string Iso2 { get; set; }
        public string Iso3 { get; set; }
        public string Country { get; set; }
        public List<string> Cities { get; set; }
    }

    class CountriesResponse
    {
        public bool Error { get; set; }
        public string Msg { get; set; }
        public List<CountryData> Data { get; set; }
    }

    class CityResponse
    {
        public bool Error { get; set; }
        public string Msg { get; set; }
        public List<string> Data { get; set; }
    }

    public class CountryService : ICountryService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CountryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<string>> GetCities(string countryName)
        {
            var cities = new List<string>();
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Post,
                "https://countriesnow.space/api/v0.1/countries/cities")
            {
                Headers =
                {
                    {
                        HeaderNames.Accept, "application/json"
                    },
                    {
                        HeaderNames.UserAgent, "HttpClientFactory-Sample"
                    }
                },
                Content = new StringContent(JsonConvert.SerializeObject(new { country = countryName }), Encoding.UTF8, "application/json")
            };

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                var citiesResponse = JsonConvert.DeserializeObject<CityResponse>(responseContent);

                if (citiesResponse is null || citiesResponse.Error)
                {
                    throw new DefaultException
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorCode = 400,
                        Message = citiesResponse!.Msg
                    };
                }

                cities = citiesResponse.Data;
            }
            else
            {
                throw new DefaultException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = 400,
                    Message = "Error getting cities"
                };
            }
            return cities;
        }

        public async Task<IEnumerable<string>> GetCountries()
        {
            var countries = new List<string>();
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                "https://countriesnow.space/api/v0.1/countries/")
            {
                Headers =
                {
                    {
                        HeaderNames.Accept, "application/json"
                    },
                    {
                        HeaderNames.UserAgent, "HttpClientFactory-Sample"
                    }
                }
            };

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                var countriesResponse = JsonConvert.DeserializeObject<CountriesResponse>(responseContent);

                if (countriesResponse is null || countriesResponse.Error)
                {
                    throw new DefaultException
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorCode = 400,
                        Message = countriesResponse!.Msg
                    };
                }

                countries = countriesResponse.Data.Select(country => country.Country).ToList();
            }
            else
            {
                throw new DefaultException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = 400,
                    Message = "Error getting countries"
                };
            }
            return countries;
        }
    }
}