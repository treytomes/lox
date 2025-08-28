using Lox.Parsing;
using Lox.Scanning;
using Lox.Statements;
using Lox.Text;
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
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(env, writer, errorReporter);
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

	[Fact]
	public void CanAssignVariables()
	{
		var sourceText = "var b = 5;";
		var env = new Environment();
		var errorReporter = new TestErrorReporter();
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(env, writer, errorReporter);
		var parser = new Parser(new ParserCursor(), errorReporter);
		var scanner = new Scanner(new ScannerCursor(), errorReporter);
		var tokens = scanner.ScanTokens(sourceText);
		var stmts = parser.Parse(tokens);
		Assert.NotNull(stmts);

		interpreter.Interpret(stmts);
		Assert.IsType<double>(env.Get("b"));
		Assert.Equal(5, Convert.ToDouble(env.Get("b")));
	}

	[Fact]
	public void CanMaintainScope()
	{
		// Given
		var env = new Environment();
		var errorReporter = new TestErrorReporter();
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(env, writer, errorReporter);
		var parser = new Parser(new ParserCursor(), errorReporter);
		var scanner = new Scanner(new ScannerCursor(), errorReporter);

		// When
		var sourceText = @"
			var a = 10;
			print a;
			{
				a = 15;
				print a;
				var a = 20;
				print a;

				{
					a = 13;
					print a;
					var a = 18;
					print a;
				}
				print a;
			}
			print a;
			a = 12;
			print a;
		";

		// Then
		var tokens = scanner.ScanTokens(sourceText);
		var stmts = parser.Parse(tokens);
		Assert.NotNull(stmts);

		interpreter.Interpret(stmts);
		Assert.IsType<double>(env.Get("a"));
		Assert.Equal(12, Convert.ToDouble(env.Get("a")));
		Assert.Equal("10", writer.Lines[0]);
		Assert.Equal("15", writer.Lines[1]);
		Assert.Equal("20", writer.Lines[2]);
		Assert.Equal("13", writer.Lines[3]);
		Assert.Equal("18", writer.Lines[4]);
		Assert.Equal("13", writer.Lines[5]);
		Assert.Equal("15", writer.Lines[6]);
		Assert.Equal("12", writer.Lines[7]);
	}
}
