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
		var errorReporter = new TestErrorReporter();
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(writer, errorReporter);
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
		var errorReporter = new TestErrorReporter();
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(writer, errorReporter);
		var parser = new Parser(new ParserCursor(), errorReporter);
		var scanner = new Scanner(new ScannerCursor(), errorReporter);
		var tokens = scanner.ScanTokens(sourceText);
		var stmts = parser.Parse(tokens);
		Assert.NotNull(stmts);

		interpreter.Interpret(stmts);
		var env = interpreter.CurrentEnvironment;
		Assert.IsType<double>(env.Get("b"));
		Assert.Equal(5, Convert.ToDouble(env.Get("b")));
	}

	[Fact]
	public void CanMaintainScope()
	{
		// Given
		var errorReporter = new TestErrorReporter();
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(writer, errorReporter);
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
		var env = interpreter.CurrentEnvironment;
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

	[Fact]
	public void CanExecuteConditionalIfStatements()
	{
		// Given
		var errorReporter = new TestErrorReporter();
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(writer, errorReporter);
		var parser = new Parser(new ParserCursor(), errorReporter);
		var scanner = new Scanner(new ScannerCursor(), errorReporter);

		// When
		var sourceText = @"
			var a = 10;
			if (a <= 10) {
				print ""A"";
			} else {
				print ""B"";
			}
			a = 15;
			if (a < 10) {
				print ""A"";
			} else {
				print ""B"";
			}
		";

		// Then
		var tokens = scanner.ScanTokens(sourceText);
		var stmts = parser.Parse(tokens);
		Assert.NotNull(stmts);

		interpreter.Interpret(stmts);
		Assert.Equal("A", writer.Lines[0]);
		Assert.Equal("B", writer.Lines[1]);
	}

	[Fact]
	public void CanExecuteWhileLoops()
	{
		// Given
		var errorReporter = new TestErrorReporter();
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(writer, errorReporter);
		var parser = new Parser(new ParserCursor(), errorReporter);
		var scanner = new Scanner(new ScannerCursor(), errorReporter);

		// When
		var sourceText = @"
			var a = 0;
			while (a < 10) {
				print a;
				a = a + 1;
			}
		";

		// Then
		var tokens = scanner.ScanTokens(sourceText);
		var stmts = parser.Parse(tokens);
		Assert.NotNull(stmts);

		interpreter.Interpret(stmts);
		Assert.Equal("0", writer.Lines[0]);
		Assert.Equal("1", writer.Lines[1]);
		Assert.Equal("2", writer.Lines[2]);
		Assert.Equal("3", writer.Lines[3]);
		Assert.Equal("4", writer.Lines[4]);
		Assert.Equal("5", writer.Lines[5]);
		Assert.Equal("6", writer.Lines[6]);
		Assert.Equal("7", writer.Lines[7]);
		Assert.Equal("8", writer.Lines[8]);
		Assert.Equal("9", writer.Lines[9]);
	}

	[Fact]
	public void CanLookupGlobalsFromNestedScope()
	{
		// Given
		var errorReporter = new TestErrorReporter();
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(writer, errorReporter);
		var parser = new Parser(new ParserCursor(), errorReporter);
		var scanner = new Scanner(new ScannerCursor(), errorReporter);

		// When
		var sourceText = @"
			var a = 0;
			{
				{
					print a;
				}
			}
		";

		// Then
		var tokens = scanner.ScanTokens(sourceText);
		var stmts = parser.Parse(tokens);
		Assert.NotNull(stmts);

		interpreter.Interpret(stmts);
		// Assert.Equal("2", writer.Lines[3]);
		// Assert.Equal("3", writer.Lines[4]);
		// Assert.Equal("5", writer.Lines[5]);
		// Assert.Equal("8", writer.Lines[6]);
		// Assert.Equal("13", writer.Lines[7]);
		// Assert.Equal("21", writer.Lines[8]);
		// Assert.Equal("34", writer.Lines[9]);
	}

	[Fact]
	public void CanExecuteForLoops()
	{
		// Given
		var errorReporter = new TestErrorReporter();
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(writer, errorReporter);
		var parser = new Parser(new ParserCursor(), errorReporter);
		var scanner = new Scanner(new ScannerCursor(), errorReporter);

		// When
		var sourceText = @"
			var a = 0;
			var temp;

			for (var b = 1; a < 10000; b = temp + b) {
				print a;
				temp = a;
				a = b;
			}
		";

		// Then
		var tokens = scanner.ScanTokens(sourceText);
		var stmts = parser.Parse(tokens);
		Assert.NotNull(stmts);

		interpreter.Interpret(stmts);
		Assert.Equal("0", writer.Lines[0]);
		Assert.Equal("1", writer.Lines[1]);
		Assert.Equal("1", writer.Lines[2]);
		Assert.Equal("2", writer.Lines[3]);
		Assert.Equal("3", writer.Lines[4]);
		Assert.Equal("5", writer.Lines[5]);
		Assert.Equal("8", writer.Lines[6]);
		Assert.Equal("13", writer.Lines[7]);
		Assert.Equal("21", writer.Lines[8]);
		Assert.Equal("34", writer.Lines[9]);
	}

	[Fact]
	public void ExpressionListsShouldReturnRightmostResult()
	{
		var sourceText = @"
			var hey = 17;
			print (1,2,3,4,hey);
		";

		var errorReporter = new TestErrorReporter();
		var scanner = new Scanner(new ScannerCursor(), errorReporter);
		var parser = new Parser(new ParserCursor(), errorReporter);
		var writer = new TestOutputWriter();
		var interpreter = new Interpreter(writer, errorReporter);

		var tokens = scanner.ScanTokens(sourceText);
		var stmts = parser.Parse(tokens);
		Assert.NotNull(stmts);
		interpreter.Interpret(stmts);
		Assert.Equal("17", writer.Lines[0]);
	}
}
