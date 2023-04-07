using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharpLS;
using System.Diagnostics;

// ReSharper disable UnusedParameter.Local

namespace SampleServer
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      MainAsync(args).Wait();
    }

    private static async Task MainAsync(string[] args)
    {
      Debugger.Launch();
      while (!Debugger.IsAttached)
      {
          await Task.Delay(100);
      }

      IObserver<WorkDoneProgressReport> workDone = null!;

      var server = await LanguageServer.From(
          options =>
              options
                 .WithInput(Console.OpenStandardInput())
                 .WithOutput(Console.OpenStandardOutput())
                 .ConfigureLogging(
                      x => x
                          //.AddSerilog(Log.Logger)
                          .AddLanguageProtocolLogging()
                      .SetMinimumLevel(LogLevel.Debug)
          ).WithHandler<TextDocumentHandler>()
          .WithHandler<FoldingRangeHandler>()
          .WithHandler<SemanticTokensHandler>()
          .WithHandler<DefinitionHandler>()
          .WithHandler<HoverHandler>()
                 .WithServices(x => x.AddLogging(b => b.SetMinimumLevel(LogLevel.Trace)))
                 .WithServices(
                      services =>
                      {
                        services.AddSingleton(
                                  provider =>
                              {
                                var loggerFactory = provider.GetService<ILoggerFactory>();
                                var logger = loggerFactory.CreateLogger<Foo>();

                                logger.LogInformation("Configuring");

                                return new Foo(logger);
                              }
                              );
                        services.AddSingleton(provider => new DocumentManager());
                        services.AddSingleton(
                                  new ConfigurationItem
                              {
                                Section = "typescript",
                              }
                              ).AddSingleton(
                                  new ConfigurationItem
                              {
                                Section = "terminal",
                              }
                              );
                      }
                  )
                 .OnStarted(
                      async (languageServer, token) =>
                      {
                        using var manager = await languageServer.WorkDoneManager.Create(new WorkDoneProgressBegin { Title = "Doing some work..." })
                                                                      .ConfigureAwait(false);

                        var logger = languageServer.Services.GetService<ILogger<Foo>>();
                        var configuration = await languageServer.Configuration.GetConfiguration(
                                  new ConfigurationItem
                              {
                                Section = "typescript",
                              }, new ConfigurationItem
                              {
                                Section = "terminal",
                              }
                              ).ConfigureAwait(false);

                        var baseConfig = new JObject();
                        foreach (var config in languageServer.Configuration.AsEnumerable())
                        {
                          baseConfig.Add(config.Key, config.Value);
                        }

                        logger.LogInformation("Base Config: {@Config}", baseConfig);

                        var scopedConfig = new JObject();
                        foreach (var config in configuration.AsEnumerable())
                        {
                          scopedConfig.Add(config.Key, config.Value);
                        }

                        logger.LogInformation("Scoped Config: {@Config}", scopedConfig);
                      }
                  )
      ).ConfigureAwait(false);

      await server.WaitForExit.ConfigureAwait(false);
    }
  }

  internal class Foo
  {
    private readonly ILogger<Foo> _logger;

    public Foo(ILogger<Foo> logger)
    {
      logger.LogInformation("inside ctor");
      _logger = logger;
    }

    public void SayFoo()
    {
      _logger.LogInformation("Fooooo!");
    }
  }
}
