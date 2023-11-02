using System;
using System.Net.Http.Json;
using ThymeToPlant.Models;

namespace ThymeToPlant.Services
{
    // Following optimizations for httpclient per service : https://devblogs.microsoft.com/dotnet/performance-improvements-in-dotnet-maui/#remove-microsoftextensionshttp-usage
    public class PlantZoneService
	{
        private readonly HttpClient httpClient;

        public PlantZoneService()
		{
            this.httpClient = new HttpClient() { BaseAddress = new Uri("https://phzmapi.org/") };
        }

        public async Task<PlantZoneDataItem> GetZoneByZip(string zipCode)
        {
            var response = await httpClient.GetAsync($"{zipCode}.json");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<PlantZoneDataItem>();
                return responseData;
            }

            return null;
        }
    }
}

