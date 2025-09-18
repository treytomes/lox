using Lox.Expressions;
using Lox.Parsing;
using Lox.Scanning;

namespace Lox.Test;

public class ParserTests
{
	[Fact]
	public void CanParseTokensIntoExpression()
	{
		var expectedStatement = new ExpressionStmt(
			new BinaryExpr(
				new UnaryExpr(
					new Token(TokenType.MINUS, "-", null, 0),
					new LiteralExpr(123)),
				new Token(TokenType.STAR, "*", null, 0),
				new GroupingExpr(
					new LiteralExpr(45.67))
			)
		);

		var sourceText = "-123 * (45.67);";
		var errorReporter = new TestErrorReporter();
		var scanner = new Scanner(new ScannerCursor(), errorReporter);
		var tokens = scanner.ScanTokens(sourceText);
		var parserCursor = new ParserCursor();
		var parser = new Parser(parserCursor, errorReporter);
		var actualStatement = parser.Parse(tokens);

		Assert.NotNull(actualStatement);
		Assert.Single(actualStatement);
		Assert.Equal(expectedStatement.ToString(), actualStatement.Single().ToString());
	}

	[Fact]
	public void CanParseExpressionLists()
	{
		var expectedStatement = new ExpressionStmt(
			new ListExpr(
				new ListExpr(
					new ListExpr(
						new LiteralExpr(1),
						new LiteralExpr(2)
					),
					new LiteralExpr(3)
				),
				new LiteralExpr(4)
			)
		);

		var sourceText = "1, 2, 3, 4;";
		var errorReporter = new TestErrorReporter();
		var scanner = new Scanner(new ScannerCursor(), errorReporter);
		var tokens = scanner.ScanTokens(sourceText);
		var parserCursor = new ParserCursor();
		var parser = new Parser(parserCursor, errorReporter);
		var actualStatement = parser.Parse(tokens);

		Assert.False(errorReporter.HadError);
		Assert.False(errorReporter.HadRuntimeError);
		Assert.Empty(errorReporter.Errors);
		Assert.Empty(errorReporter.RuntimeErrors);

		Assert.NotNull(actualStatement);
		Assert.Single(actualStatement);
		Assert.Equal(expectedStatement.ToString(), actualStatement.Single().ToString());
	}
}
