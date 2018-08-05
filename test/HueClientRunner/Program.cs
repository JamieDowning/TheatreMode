using System;
using System.Linq;
using System.Threading.Tasks;
using DowningSoft.TheatreMode.HueClient;

namespace DowningSoft.TheatreMode.HueClientRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var task = DoWork();
            task.Wait();
        }

        async static Task DoWork()
        {
            var user = "";
            var movieScene = "";
            var lightScene = "";
            var theatreRoom = "";

            var bridge = (await new BridgeDiscoveryService().DiscoverBridges()).First();
            var client = new LocalHueClient(bridge, user);

            System.Threading.Thread.Sleep(5000);
            await client.SetSceneAsync(movieScene, theatreRoom);
            System.Threading.Thread.Sleep(5000);
            await client.SetSceneAsync(lightScene, theatreRoom);
            System.Threading.Thread.Sleep(5000);
        }
    }
}
