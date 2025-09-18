namespace Lox.Test.Visitors;

public class InterpreterTests
{
	[Fact]
	public void CanConcatenateStrings()
	{
		var sourceText = "\"Hello\" + \" \" + \"world!\";";

		var fixture = new TestFixture();

		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);
		Assert.Single(stmts);
		Assert.IsType<ExpressionStmt>(stmts.Single());

		var expr = (stmts.Single() as ExpressionStmt)!.Expression;

		var expectedResult = "Hello world!";
		var actualResult = fixture.Interpreter.Evaluate(expr);
		Assert.Equal(expectedResult, actualResult);
	}

	[Fact]
	public void CanAssignVariables()
	{
		var sourceText = "var b = 5;";

		var fixture = new TestFixture();

		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);

		fixture.Interpreter.Interpret(stmts);
		var env = fixture.Interpreter.CurrentEnvironment;
		Assert.IsType<double>(env.Get("b"));
		Assert.Equal(5, Convert.ToDouble(env.Get("b")));
	}

	[Fact]
	public void CanMaintainScope()
	{
		// Given
		var fixture = new TestFixture();

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
		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);

		fixture.Interpreter.Interpret(stmts);
		var env = fixture.Interpreter.CurrentEnvironment;
		Assert.IsType<double>(env.Get("a"));
		Assert.Equal(12, Convert.ToDouble(env.Get("a")));
		Assert.Equal("10", fixture.Writer.Lines[0]);
		Assert.Equal("15", fixture.Writer.Lines[1]);
		Assert.Equal("20", fixture.Writer.Lines[2]);
		Assert.Equal("13", fixture.Writer.Lines[3]);
		Assert.Equal("18", fixture.Writer.Lines[4]);
		Assert.Equal("13", fixture.Writer.Lines[5]);
		Assert.Equal("15", fixture.Writer.Lines[6]);
		Assert.Equal("12", fixture.Writer.Lines[7]);
	}

	[Fact]
	public void CanExecuteConditionalIfStatements()
	{
		// Given
		var fixture = new TestFixture();

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
		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);

		fixture.Interpreter.Interpret(stmts);
		Assert.Equal("A", fixture.Writer.Lines[0]);
		Assert.Equal("B", fixture.Writer.Lines[1]);
	}

	[Fact]
	public void CanExecuteWhileLoops()
	{
		// Given
		var fixture = new TestFixture();

		// When
		var sourceText = @"
			var a = 0;
			while (a < 10) {
				print a;
				a = a + 1;
			}
		";

		// Then
		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);

		fixture.Interpreter.Interpret(stmts);
		Assert.Equal("0", fixture.Writer.Lines[0]);
		Assert.Equal("1", fixture.Writer.Lines[1]);
		Assert.Equal("2", fixture.Writer.Lines[2]);
		Assert.Equal("3", fixture.Writer.Lines[3]);
		Assert.Equal("4", fixture.Writer.Lines[4]);
		Assert.Equal("5", fixture.Writer.Lines[5]);
		Assert.Equal("6", fixture.Writer.Lines[6]);
		Assert.Equal("7", fixture.Writer.Lines[7]);
		Assert.Equal("8", fixture.Writer.Lines[8]);
		Assert.Equal("9", fixture.Writer.Lines[9]);
	}

	[Fact]
	public void CanLookupGlobalsFromNestedScope()
	{
		// Given
		var fixture = new TestFixture();

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
		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);

		fixture.Interpreter.Interpret(stmts);
		Assert.Equal("0", fixture.Writer.Lines[0]);
	}

	[Fact]
	public void CanExecuteForLoops()
	{
		// Given
		var fixture = new TestFixture();

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
		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);

		fixture.Interpreter.Interpret(stmts);
		Assert.Equal("0", fixture.Writer.Lines[0]);
		Assert.Equal("1", fixture.Writer.Lines[1]);
		Assert.Equal("1", fixture.Writer.Lines[2]);
		Assert.Equal("2", fixture.Writer.Lines[3]);
		Assert.Equal("3", fixture.Writer.Lines[4]);
		Assert.Equal("5", fixture.Writer.Lines[5]);
		Assert.Equal("8", fixture.Writer.Lines[6]);
		Assert.Equal("13", fixture.Writer.Lines[7]);
		Assert.Equal("21", fixture.Writer.Lines[8]);
		Assert.Equal("34", fixture.Writer.Lines[9]);
	}

	[Fact]
	public void ExpressionListsShouldReturnRightmostResult()
	{
		var fixture = new TestFixture();

		var sourceText = @"
			var hey = 17;
			print (1,2,3,4,hey);
		";

		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);
		fixture.Interpreter.Interpret(stmts);
		Assert.Equal("17", fixture.Writer.Lines[0]);
	}

	[Fact]
	public void ExpressionStatementShouldOutputResult()
	{
		var fixture = new TestFixture();

		var sourceText = "3*12-5;";

		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);
		fixture.Interpreter.Interpret(stmts);
		Assert.IsType<double>(fixture.Interpreter.LastResult);
		Assert.Equal(31, Convert.ToDouble(fixture.Interpreter.LastResult));
	}

	[Fact]
	public void LastResultIsCaptured()
	{
		var sourceText = "3*12-5;";

		var fixture = new TestFixture();

		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);
		fixture.Interpreter.Interpret(stmts);
		Assert.IsType<double>(fixture.Interpreter.LastResult);
		Assert.Equal(31, Convert.ToDouble(fixture.Interpreter.LastResult));

		sourceText = "_;";

		tokens = fixture.Scanner.ScanTokens(sourceText);
		stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);
		fixture.Interpreter.Interpret(stmts);
		Assert.IsType<double>(fixture.Interpreter.LastResult);
		Assert.Equal(31, Convert.ToDouble(fixture.Interpreter.LastResult));
	}

	[Fact]
	public void FinalStatementDoesNotRequireSemicolon()
	{
		var sourceText = @"
			print ""1"";
			print ""2"";
			print ""3"";
			3*12-5
		";

		var fixture = new TestFixture();

		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);
		fixture.Interpreter.Interpret(stmts);
		Assert.IsType<double>(fixture.Interpreter.LastResult);
		Assert.Equal(31, Convert.ToDouble(fixture.Interpreter.LastResult));
	}

	[Fact]
	public void CanInterpretTernaryExpressions()
	{
		// Given
		var fixture = new TestFixture();

		// When
		var sourceText = "(10 > 5) ? \"hello\" : \"hola\"";

		// Then
		var tokens = fixture.Scanner.ScanTokens(sourceText);
		var stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);
		fixture.Interpreter.Interpret(stmts);
		Assert.IsType<string>(fixture.Interpreter.LastResult);
		Assert.Equal("hello", Convert.ToString(fixture.Interpreter.LastResult));

		// When
		sourceText = "(10 < 5) ? \"hello\" : \"hola\"";

		// Then
		tokens = fixture.Scanner.ScanTokens(sourceText);
		stmts = fixture.Parser.Parse(tokens);
		Assert.NotNull(stmts);
		fixture.Interpreter.Interpret(stmts);
		Assert.IsType<string>(fixture.Interpreter.LastResult);
		Assert.Equal("hola", Convert.ToString(fixture.Interpreter.LastResult));
	}
}
