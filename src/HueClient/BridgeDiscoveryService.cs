using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace DowningSoft.TheatreMode.HueClient
{
    public class BridgeDiscoveryService
    {
        private static readonly Uri locatorUri = new Uri("https://discovery.meethue.com");

        /// <summary>
		/// Locate bridges
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns>All bridge end points</returns>
        public async Task<IEnumerable<HueBridge>> DiscoverBridges()
        {
            HttpClient client = new HttpClient();
            
            string response = await client.GetStringAsync(locatorUri).ConfigureAwait(false);

            DiscoveryResponse[] responseModel = JsonConvert.DeserializeObject<DiscoveryResponse[]>(response);
            return responseModel.Select(x => new HueBridge() { Id = x.Id, IpAddress = x.InternalIpAddress }).ToList();;
        }

        private class DiscoveryResponse
        {
            public string Id { get; set; }
            public string InternalIpAddress { get; set; }
            public string MacAddress { get; set; }
        }
    }
}
