using Microsoft.Extensions.DependencyInjection;

namespace Lox;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection ConfigureLox(this IServiceCollection @this, string? filePath)
	{
		@this.AddSingleton<IErrorReporter, ConsoleErrorReporter>();

		@this.AddTransient<IScanner, Scanner>();
		@this.AddTransient<IScannerCursor, ScannerCursor>();
		@this.AddTransient<IInterpreter, Interpreter>();
		if (!string.IsNullOrWhiteSpace(filePath))
		{
			@this.AddTransient<IExecutionSource>(sp => new FileExecutionSource(filePath));
		}
		return @this;
	}
}
