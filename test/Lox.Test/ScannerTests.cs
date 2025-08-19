namespace Lox.Test;

public class ScannerTests
{
	public void CanScanSourceTextIntoTokens()
	{
		var errorReporter = new TestErrorReporter();
		var scannerCursor = new ScannerCursor();

		var sourceText = "-123 * (45.67)";

		var scanner = new Scanner(scannerCursor, errorReporter);
		var actualTokens = scanner.ScanTokens(sourceText);

		List<Token> expectedTokens = [
			new Token(TokenType.MINUS, "-", null, 0),
			new Token(TokenType.NUMBER, "123", 123, 0),
			new Token(TokenType.STAR, "*", null, 0),
			new Token(TokenType.LEFT_PAREN, "(", null, 0),
			new Token(TokenType.NUMBER, "45.67", 45.67, 0),
			new Token(TokenType.RIGHT_PAREN, ")", null, 0),
		];

		Assert.Equal(expectedTokens, actualTokens);
	}
}
