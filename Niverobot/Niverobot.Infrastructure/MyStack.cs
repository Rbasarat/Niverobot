using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.Sql;

class MyStack : Stack
{
    public MyStack()
    {
        // Create an Azure Resource Group
        var resourceGroup = new ResourceGroup("Niverobot", new ResourceGroupArgs
        {
            Name = "Niverobot",
            Location = "westeurope"
        });

        // Init the config. These settings will be set by gitlab ci, which in turn we can find in the settings of gitlab.
        // We put in the passwords as variables for the ci and then we import them using the pulumi config.
        var config = new Config();

        // Retrieve all config we need.
        var dbAdminUsername = config.Get("sqlAdmin") ?? "niverhawk";
        var dbAdminPassword = config.RequireSecret("AdminSqlPassword");
        var botToken = config.RequireSecret("botToken");

        /*
         TODO: create private network for app + set firewall rules for outside access to db+actual bot
         This should be done in mysql? not in pulumi/azure?
         For our scope its fine to use admin login
        */

        // First create the SQL server.
        var sqlServer = new SqlServer("sql-server", new SqlServerArgs
        {
            Name = "niverobot-sql-server",
            ResourceGroupName = resourceGroup.Name,
            AdministratorLogin = dbAdminUsername,
            AdministratorLoginPassword = dbAdminPassword,
            Version = "12.0",
        });

        // Then the database.
        var database = new Database("sql-database", new DatabaseArgs
        {
            Name = "niverobot-sql-database",
            ResourceGroupName = resourceGroup.Name,
            ServerName = sqlServer.Name,
            RequestedServiceObjectiveName = "Basic",
            Edition = "Basic"
        }, new CustomResourceOptions
        {
            DependsOn = sqlServer
        });

        // Define the app service plan we want to use.
        // Select Linux since we have a python app. // nope.
        // App service plans cannot have different os running(restriction of azure).
        var appServicePlan = new Plan("AppServicePlan", new PlanArgs
        {
            Name = "Niverobot-AppServicePlan",
            ResourceGroupName = resourceGroup.Name,
            // Kind = "App",
            // Reserved = true,
            Sku = new PlanSkuArgs
            {
                Tier = "Basic",
                Size = "F1"
            }
        });

        // CI/CD should execute the proto command. Since this app service will not have access to the proto file.
        var dateParser = new AppService("Dateparser", new AppServiceArgs
        {
            Name = "Niverobot-Dateparser",
            ResourceGroupName = resourceGroup.Name,
            AppServicePlanId = appServicePlan.Id,
            // SiteConfig = new AppServiceSiteConfigArgs
            // {
            //     AppCommandLine = "python server.py"
            // }
        });

        // Create the web api.
        var webApi = new AppService("WebApi", new AppServiceArgs
        {
            Name = "Niverobot-WebApi",
            ResourceGroupName = resourceGroup.Name,
            AppServicePlanId = appServicePlan.Id,
            AppSettings = new InputMap<string>
            {
                {
                    "BotConfiguration__BotToken", botToken
                }
            },
            ConnectionStrings =
            {
                new AppServiceConnectionStringArgs
                {
                    Name = "SqlServer",
                    Type = "SQLAzure",
                    Value = Output.Tuple<string, string, string>(sqlServer.Name, database.Name, dbAdminPassword).Apply(
                        t =>
                        {
                            (string server, string db, string pwd) = t;
                            return
                                
                                $"Server=tcp:{server}.database.windows.net;initial catalog={db};user ID={dbAdminUsername};password={pwd};Persist Security Info=False;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                        }),
                },
                new AppServiceConnectionStringArgs
                {
                    Name = "DateParser",
                    Type = "Custom",
                    // TODO: fill this when we have the address of the python app.
                    Value = dateParser.DefaultSiteHostname,
                },
            },
        }, new CustomResourceOptions
        {
            // We want to wait for the parser and database to be online.
            DependsOn =
            {
                dateParser,
                database
            }
        });

        // Lastly create the consumer.
        var consumer = new AppService("Consumer", new AppServiceArgs
        {
            Name = "Niverobot-Consumer",
            ResourceGroupName = resourceGroup.Name,
            AppServicePlanId = appServicePlan.Id,
            AppSettings = new InputMap<string>
            {
                {"BotConfiguration__BotToken", botToken}
            },
            ConnectionStrings =
            {
                new AppServiceConnectionStringArgs
                {
                    Name = "SqlServer",
                    Type = "SQLAzure",
                    Value = Output.Tuple<string, string, string>(sqlServer.Name, database.Name, dbAdminPassword).Apply(
                        t =>
                        {
                            (string server, string db, string pwd) = t;
                            return
                                $"Server=tcp:{server}.database.windows.net;initial catalog={db};user ID={dbAdminUsername};password={pwd};Persist Security Info=False;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                        }),
                },
                new AppServiceConnectionStringArgs
                {
                    Name = "DateParser",
                    Type = "Custom",
                    // TODO: fill this when we have the address of the python app.
                    Value = dateParser.DefaultSiteHostname,
                },
            },
        });


        this.ApiEndpoint = webApi.DefaultSiteHostname;
        this.DateParserEndpoint = dateParser.DefaultSiteHostname;
    }

    [Output] public Output<string> ApiEndpoint { get; set; }
    [Output] public Output<string> DateParserEndpoint { get; set; }
}