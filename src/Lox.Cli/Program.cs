using System.CommandLine;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Lox.Cli;

class Program
{
	static async Task<int> Main(string[] args)
	{
		var configFileOption = new Option<string>("--config")
		{
			Description = "Path to the configuration file",
			DefaultValueFactory = parseResult => "appsettings.json",
		};

		var debugOption = new Option<bool>("--debug")
		{
			Description = "Enable debug mode",
			DefaultValueFactory = parseResult => false,
		};

		var filePathOption = new Option<string>("--file")
		{
			Description = "Run the fun",
			DefaultValueFactory = parseResult => string.Empty,
			Required = false,
		};

		// Create root command
		var rootCommand = new RootCommand("lox")
		{
			configFileOption,
			debugOption,
			filePathOption,
		};

		rootCommand.SetAction((parseResult) =>
		{
			var filePath = parseResult.GetValue(filePathOption);
			var configFile = parseResult.GetValue(configFileOption)!;
			var debug = parseResult.GetValue(debugOption);
			var task = RunAsync(filePath, configFile, debug);
			task.Wait();
			return task.Result;
		});

		var parseResult = rootCommand.Parse(args);
		return await parseResult.InvokeAsync();
	}

	static async Task<int> RunAsync(string? filePath, string configFile, bool debug)
	{
		try
		{
			// Build host with DI container.
			using var host = CreateHostBuilder(filePath, configFile, debug).Build();
			debug = host.Services.GetRequiredService<IOptions<AppSettings>>().Value.Debug;

			await host.Services.GetRequiredService<IAppState>().RunAsync();

			return 0;
		}
		catch (Exception ex)
		{
			if (debug)
			{
				Console.Error.WriteLine(ex);
			}
			else
			{
				Console.Error.WriteLine($"Error: {ex.Message}");
			}
			return 1;
		}
	}

	static IHostBuilder CreateHostBuilder(string? filePath, string configFile, bool debug)
	{
		return Host.CreateDefaultBuilder()
			.ConfigureAppConfiguration((hostContext, config) =>
			{
				config.Sources.Clear();
				config.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location) ?? Directory.GetCurrentDirectory());
				config.AddJsonFile(configFile, optional: false, reloadOnChange: false);

				// Add command line overrides
				var commandLineConfig = new Dictionary<string, string?>();
				if (debug)
				{
					commandLineConfig["Debug"] = "true";
				}

				config.AddInMemoryCollection(commandLineConfig);
			})
			.ConfigureLogging((hostContext, logging) =>
			{
				logging.ClearProviders();

				// Use simple console for better performance in production.
				if (hostContext.Configuration.GetValue<bool>("Debug"))
				{
					// Add console logging with more options.
					logging.AddSimpleConsole(options =>
					{
						options.IncludeScopes = true;
						options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
					});
				}
				else
				{
					logging.AddConsole(options =>
					{
						options.FormatterName = ConsoleFormatterNames.Simple;
					});
				}

				// Configure log levels from configuration.
				logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));

				// Override with debug flag
				var debugEnabled = hostContext.Configuration.GetValue<bool>("Debug");
				if (debugEnabled)
				{
					logging.SetMinimumLevel(LogLevel.Debug);
				}
			})
			.ConfigureServices((hostContext, services) =>
			{
				services.Configure<AppSettings>(hostContext.Configuration);
				services.ConfigureLox(filePath);

				if (string.IsNullOrWhiteSpace(filePath))
				{
					services.AddTransient<IAppState, RunPromptAppState>();
				}
				else
				{
					services.AddTransient<IAppState, RunFileAppState>();
				}

				// services.AddTransient<IAppState>(sp => new RunFileAppState(sp, sp.GetRequiredService<IActivationFunctionFactory>()));
				// services.AddTransient<IActivationFunctionFactory>(sp => new ActivationFunctionFactory(
				// 	sp.GetRequiredService<IOptions<AppSettings>>().Value.DefaultActivationFunction
				// ));
				// services.AddTransient(sp => sp.GetRequiredService<IActivationFunctionFactory>().GetDefaultActivationFunction());
			});
	}
}
