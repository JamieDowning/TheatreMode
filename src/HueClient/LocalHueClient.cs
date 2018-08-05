using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace DowningSoft.TheatreMode.HueClient
{
    // Client for interacting with a local hue bridge
    public class LocalHueClient
    {
            private HueBridge bridge;
            
            private string appKey;

            protected string BaseUrl
            {
                get
                {
                    return string.Format("http://{0}/api/{1}/", this.bridge.IpAddress, this.appKey);
                }
            }

            /// <summary>
            /// Creates a new instance of the LocalHueClient class.
            /// </summary>
            /// <param name="bridge">Bridge to communicate with.</param>
            /// <param name="appKey">App Key (user) authorized to make calls to the bridge.</param>
            public LocalHueClient(HueBridge bridge, string appKey)
            {
                if (bridge == null)
                {
                    throw new ArgumentNullException("bridge");
                }

                if (string.IsNullOrEmpty(appKey))
                {
                    throw new ArgumentNullException("appKey");
                }

                this.bridge = bridge;
                this.appKey = appKey;
            }

            /// <summary>
            /// Sets a saved scene on a room (by id)
            /// </summary>
            public async Task SetSceneAsync(string scene, string room)
            {
                var payload = string.Format("{{\"scene\":\"{0}\"}}", scene);
                var apiUrlPart = string.Format("groups/{0}/action", room);

                using (var client = new HttpClient())
                {
                    var url = this.BaseUrl + apiUrlPart;
                    var content = new StringContent(payload);
                    await client.PutAsync(url, content);
                }
            }
    }
}