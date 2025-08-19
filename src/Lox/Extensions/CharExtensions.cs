namespace Lox;

public static class CharExtensions
{
	public static bool IsAlpha(this char @this)
	{
		return (@this >= 'a' && @this <= 'z') ||
			(@this >= 'A' && @this <= 'Z') ||
			@this == '_';
	}

	public static bool IsDigit(this char @this)
	{
		return @this >= '0' && @this <= '9';
	}

	public static bool IsAlphaNumeric(this char @this)
	{
		return @this.IsAlpha() || @this.IsDigit();
	}
}
