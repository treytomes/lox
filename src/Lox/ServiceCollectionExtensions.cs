using Lox.Parsing;
using Lox.Reporting;
using Lox.Scanning;
using Lox.Visitors;
using Microsoft.Extensions.DependencyInjection;

namespace Lox;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection ConfigureLox(this IServiceCollection @this, string? filePath)
	{
		@this.AddSingleton<IErrorReporter, ConsoleErrorReporter>();
		@this.AddTransient<IScannerCursor, ScannerCursor>();
		@this.AddTransient<IScanner, Scanner>();
		@this.AddTransient<IParserCursor, ParserCursor>();
		@this.AddTransient<IParser, Parser>();
		@this.AddTransient<IEnvironment, Environment>();
		@this.AddSingleton<IOutputWriter, ConsoleOutputWriter>();
		@this.AddTransient<IInterpreter, Interpreter>();
		@this.AddTransient<ILox, Lox>();
		if (!string.IsNullOrWhiteSpace(filePath))
		{
			@this.AddTransient<IExecutionSource>(sp => new FileExecutionSource(filePath));
		}
		return @this;
	}
}
