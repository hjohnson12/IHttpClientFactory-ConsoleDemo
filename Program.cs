using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using IHttpClientFactory_ConsoleDemo.Handlers;

// Example program for using IHttpClientFactory 
// 
namespace IHttpClientFactory_ConsoleDemo
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Transient operations are always different, a new instance is created with every retrieval of the service.
            // Scoped operations change only with a new scope, but are the same instance within a scope.
            // Singleton operations are always the same, a new instance is only created once.

            // Demo 1
            var serviceHost1 = CreateDefaultHostBuilder();
            try
            {
                // Resolve service and test a http request call
                var myService = serviceHost1.Services.GetRequiredService<IMyService>();
                var pageContent = await myService.GetPageInformation();

                Console.WriteLine(pageContent.Substring(0, 250));
            }
            catch (Exception ex)
            {
                var logger = serviceHost1.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred.");
            }

            // Demo 2 - Typed Client
            var serviceHost2 = CreateHostWithTypedClient();
            try
            {
                // Resolve service and test a http request call
                var myService = serviceHost2.Services.GetRequiredService<ITypedClient>();

                var cancellationToken = new CancellationTokenSource().Token;
                var exampleModel = await myService.GetSomething(cancellationToken);

                var outputString = string.Format(
                    "Title: {0}, UserId: {1}",
                    exampleModel.Title,
                    exampleModel.UserId);

                Console.WriteLine($"Some returned data is: {outputString}");
            }
            catch (Exception ex)
            {
                var logger = serviceHost1.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred.");
            }

            return 0;
        }

        static IHost CreateDefaultHostBuilder()
        {
            // Create IHostBuilder instance with default settings
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();

                    // Injects an instance of httpclientfactory into the service class
                    services.AddTransient<IMyService, MyService>();
                }).UseConsoleLifetime();

            return builder.Build();
        }

        static IHost CreateHostWithTypedClient()
        {
            // Create a typed instance
            var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient<ITypedClient, TypedClient>(); // Client now handles default config
            });

            return builder.Build();
        }

        static IHost CreateHostWithNamedClient()
        {
            // Create a named client
            // ex call: var httpClient = _httpClientFactory.CreateClient("nasa");
            var bulider = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient("nasa", c =>
                    {
                        c.BaseAddress = new Uri("https://api.nasa.gov/");
                    })
                    // Add delegate handlers to client
                    .AddHttpMessageHandler(handler => new TimeoutDelegatingHandler(TimeSpan.FromSeconds(20)))
                    .AddHttpMessageHandler(handler => new RetryDelegatingHandler(2));

                    services.AddTransient<IMyService, MyService>();
                }).UseConsoleLifetime();

            return bulider.Build();
        }
    }
}