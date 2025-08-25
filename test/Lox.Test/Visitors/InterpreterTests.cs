using Lox.Parsing;
using Lox.Scanning;
using Lox.Statements;
using Lox.Visitors;

namespace Lox.Test.Visitors;

public class InterpreterTests
{
	[Fact]
	public void CanConcatenateStrings()
	{
		var sourceText = "\"Hello\" + \" \" + \"world!\";";
		var env = new Environment();
		var errorReporter = new TestErrorReporter();
		var interpreter = new Interpreter(env, errorReporter);
		var parser = new Parser(new ParserCursor(), errorReporter);
		var scanner = new Scanner(new ScannerCursor(), errorReporter);
		var tokens = scanner.ScanTokens(sourceText);
		var stmts = parser.Parse(tokens);
		Assert.NotNull(stmts);
		Assert.Single(stmts);
		Assert.IsType<ExpressionStmt>(stmts.Single());

		var expr = (stmts.Single() as ExpressionStmt)!.Expression;

		var expectedResult = "Hello world!";
		var actualResult = interpreter.Evaluate(expr);
		Assert.Equal(expectedResult, actualResult);
	}
}
