using Microsoft.Extensions.Configuration;
using Serilog;

namespace Niverobot
{
    class ConsoleApplication
    {

        private readonly IConfiguration _config;
        public ConsoleApplication(IConfiguration config)
        {

            _config = config;
        }

        public void Run()
        {
            Log.Information("Hello world");
        }

    }
}
