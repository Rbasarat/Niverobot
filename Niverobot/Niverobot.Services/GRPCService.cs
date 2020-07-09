using Dateparser;
using Grpc.Core;
using Niverobot.Interfaces;
using Microsoft.Extensions.Configuration;

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
            Channel channel = new Channel(_config.GetConnectionString("DateParser"), ChannelCredentials.Insecure);

            var client = new DateParser.DateParserClient(channel);

            var reply = client.ParseDate(new ParseDateRequest{NaturalDate = date});

            channel.ShutdownAsync().Wait();

            return reply;
        }
    }
}