using DateParser;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Niverobot.Interfaces;

namespace Niverobot.Services
{
    public class GrpcService : IGRPCService
    {
        private readonly IConfiguration _config;

        public GrpcService(IConfiguration config)
        {
            _config = config;
        }

        public ParseDateReply ParseDateTimeFromNl(string date)
        {
            // TODO: add config. + Change to containerized address.
            using var channel = GrpcChannel.ForAddress(_config.GetConnectionString("DateParser"));
            var client = new DateParser.DateParser.DateParserClient(channel);

            var reply = client.ParseDate(new ParseDateRequest {NaturalDate = date});

            channel.ShutdownAsync().Wait();

            return reply;
        }
    }
}