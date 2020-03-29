using System;
using Niverobot.WebApi.Interfaces;
using Grpc.Core;
using Dateparser;

namespace Niverobot.WebApi.Services
{
    public class GrpcService : IGRPCService
    {
        public string ParseDateTimeFromNl(string date)
        {
            // TODO: add config. + Change to containerized address.
            Channel channel = new Channel("127.0.0.1:3001", ChannelCredentials.Insecure);

            var client = new DateParser.DateParserClient(channel);

            var reply = client.ParseDate(new ParseDateRequest{NaturalDate = date});
            var response = reply.ParsedDate;

            channel.ShutdownAsync().Wait();

            return response;
        }
    }
}