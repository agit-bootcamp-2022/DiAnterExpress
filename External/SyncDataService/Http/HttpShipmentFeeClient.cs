using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DiAnterExpress.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DiAnterExpress.External.SyncDataService.Http
{
    public class HttpShipmentFeeClient : IShipmentFeeClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HttpShipmentFeeClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        public async Task<ShipmentType> GetFee(int id)
        {
            var url = _config["GetFee"];
            var response = await _httpClient.GetAsync($"{url}/{id}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to Get Fee was OK !");
                var content = await response.Content.ReadAsStringAsync();
                ShipmentType value = JsonConvert.DeserializeObject<ShipmentType>(content);
                Console.WriteLine(value.Name);
                return value;
            }
            else
            {
                Console.WriteLine("--> Sync POST to Get Fee failed");
                return null;
            }
        }
    }
}