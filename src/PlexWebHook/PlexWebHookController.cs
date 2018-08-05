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
        private readonly TimeSpan sunset;
        private readonly TimeSpan sunrise;

        public PlexWebHookController(IConfiguration configuration)
        {
            // Read app configuration settings
            this.hueUser = configuration["hue-user"];
            this.movieScene = configuration["scene-to-show-when-playing"];
            this.stoppedScene = configuration["scene-to-show-when-stopped"];
            this.theatreRoom = configuration["theatre-room"];
            this.deviceUuid = configuration["device-uuid"];
            
            this.sunset = TryParseTime(configuration["sunset-time"]);
            this.sunrise = TryParseTime(configuration["sunrise-time"]);
        }

        private static TimeSpan TryParseTime(string time)
        {
            if (string.IsNullOrEmpty(time) || !time.Contains(":"))
            {
                return TimeSpan.MinValue;
            }

            var splitTime = time.Split(":");
            int hours;
            int minutes;

            if (int.TryParse(splitTime[0], out hours) && int.TryParse(splitTime[1], out minutes))
            {
                return new TimeSpan(hours, minutes, 0);
            }

            return TimeSpan.MinValue;
        }

        // GET api/values
        [HttpGet]
        [Route("/start")]
        public async Task<ActionResult<string>> Start()
        {
            await SetTheatreLighting();
            return $"Turned on {movieScene}";
        }

        [HttpGet]
        [Route("/stop")]
        public async Task<ActionResult<string>> Stop()
        {
            await SetPausedLighting();
            return $"Turned on {stoppedScene}";
        }

        [HttpGet]
        [Route("/IsDark")]
        public async Task<ActionResult<string>> IsDark()
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var isDark = sunset == TimeSpan.MinValue || currentTime > sunset || currentTime < sunrise;
            return $"Sunrise: {sunrise} Sunset: {sunset} Time: {currentTime} isDark: {isDark}";
        }
        
        // POST api/values
        [HttpPost]
        [DisableFormValueModelBinding]
        public async Task Post()
        {
            string payloadJson = await Request.ExtractJsonString("payload");
            
            var parser = new Plex.Server.Webhooks.Service.WebhookParser();
            var events = parser.ParseEvent(payloadJson);

            var currentTime = DateTime.Now.TimeOfDay;
            var isDark = sunset == TimeSpan.MinValue || currentTime > sunset || currentTime < sunrise;

            if (events.Player.Uuid == deviceUuid && 
                (events.Metadata.Type == "movie" || events.Metadata.Type == "show") &&
                isDark)
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
