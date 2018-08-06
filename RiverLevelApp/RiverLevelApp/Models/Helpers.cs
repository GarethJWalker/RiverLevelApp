using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RiverLevelApp
{
    public static class Helpers
    {
        public static async Task<string> GetResponse(string endpoint)
        {




            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(3);
                var data = await httpClient.GetStringAsync(endpoint);
                return data;

            }
        }

    }
}