using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Domain;
using Niverobot.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Niverobot.Services
{
    public class DadJokeService : IDadJokeService
    {
        private readonly IConfiguration _config;

        public DadJokeService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GetDadJokeAsync()
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), _config.GetSection("Urls:DadJokeApi").Value))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");

                    var response = await httpClient.SendAsync(request);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Log.Error("Error retrieving dadjoke: {0}", await response.Content.ReadAsStringAsync());
                        return "No dad joke for you.";
                    }
                    var joke = JsonSerializer.Deserialize<DadJokeResponse>(await response.Content.ReadAsStringAsync());

                    return joke.Joke;
                }
            }

        }
    }
}
