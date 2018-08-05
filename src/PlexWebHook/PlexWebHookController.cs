using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plex.Server.Webhooks.Events;
using DowningSoft.TheatreMode.HueClient;
using Microsoft.Extensions.Configuration;

namespace DowningSoft.TheatreMode.PlexWebHook
{
    [Route("/")]
    [ApiController]
    public class PlexWebHookController : ControllerBase
    {
        private readonly string hueUser;
        private readonly string movieScene;
        private readonly string stoppedScene;
        private readonly string theatreRoom;
        private readonly string deviceUuid;

        public PlexWebHookController(IConfiguration configuration)
        {
            // Read app configuration settings
            this.hueUser = configuration["hue-user"];
            this.movieScene = configuration["scene-to-show-when-playing"];
            this.stoppedScene = configuration["scene-to-show-when-stopped"];
            this.theatreRoom = configuration["theatre-room"];
            this.deviceUuid = configuration["device-uuid"];
        }

        // GET api/values
        [HttpGet]
        [Route("/start")]
        public async Task<ActionResult<string>> Start()
        {
            await SetTheatreLighting();
            Console.WriteLine("Done");
            return $"Turned on {movieScene}";
        }

        [HttpGet]
        [Route("/stop")]
        public async Task<ActionResult<string>> Stop()
        {
            await SetPausedLighting();
            return $"Turned on {stoppedScene}";
        }
        
        // POST api/values
        [HttpPost]
        [DisableFormValueModelBinding]
        public async Task Post()
        {
            string payloadJson = await Request.ExtractJsonString("payload");
            
            var parser = new Plex.Server.Webhooks.Service.WebhookParser();
            var events = parser.ParseEvent(payloadJson);

            if (events.Player.Uuid == deviceUuid && 
                (events.Metadata.Type == "movie" || events.Metadata.Type == "show"))
            {
                if (events is MediaPlay || events is MediaResume)
                {
                    await SetTheatreLighting();
                }
                else if (events is MediaPause || events is MediaStop)
                {
                    await SetPausedLighting();
                }
            }
        }

        private async Task SetTheatreLighting()
        {
            var bridge = (await new BridgeDiscoveryService().DiscoverBridges()).First();
            var client = new LocalHueClient(bridge, hueUser);

            await client.SetSceneAsync(movieScene, theatreRoom);
        }

        private async Task SetPausedLighting()
        {
            var bridge = (await new BridgeDiscoveryService().DiscoverBridges()).First();
            var client = new LocalHueClient(bridge, hueUser);

            await client.SetSceneAsync(stoppedScene, theatreRoom);
        }
    }
}
