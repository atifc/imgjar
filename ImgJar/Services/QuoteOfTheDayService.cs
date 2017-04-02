using System.Linq;
using System.Net.Http;
using ImgJar.Services.Models;
using Newtonsoft.Json;

namespace ImgJar.Services
{
    public static class QuoteOfTheDayService
    {
        public static Quote GetQotd()
        {
            using (var httpClient = new HttpClient())
            {
                var json = httpClient.GetStringAsync("http://quotes.rest/qod.json");

                var payload = JsonConvert.DeserializeObject<QotdResponse>(json.Result);
                return payload.contents.quotes.First();
            }
        }
    }
}