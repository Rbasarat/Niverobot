using System;
using System.Text.Json.Serialization;

namespace Domain
{
    public class DadJokeResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("joke")]
        public string Joke { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
