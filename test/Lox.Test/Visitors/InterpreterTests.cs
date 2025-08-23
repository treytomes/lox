using Lox.Parsing;
using Lox.Scanning;
using Lox.Visitors;

namespace Lox.Test.Visitors;

public class InterpreterTests
{
	[Fact]
	public void CanConcatenateStrings()
	{
		var sourceText = "\"Hello\" + \" \" + \"world!\"";
		var errorReporter = new TestErrorReporter();
		var interpreter = new Interpreter(errorReporter);
		var parser = new Parser(new ParserCursor(), errorReporter);
		var scanner = new Scanner(new ScannerCursor(), errorReporter);
		var tokens = scanner.ScanTokens(sourceText);
		var expr = parser.Parse(tokens);
		Assert.NotNull(expr);

		var expectedResult = "Hello world!";
		var actualResult = interpreter.Evaluate(expr);
		Assert.Equal(expectedResult, actualResult);
	}
}
