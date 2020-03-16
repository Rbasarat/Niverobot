using Microsoft.Extensions.Configuration;
using Niverobot.WebApi.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Domain;
using Serilog;
using System.Net;

namespace Niverobot.WebApi.Services
{
    class DadJokeService : IDadJokeService
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
