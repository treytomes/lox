namespace Lox;

static class ObjectExtensions
{
	public static bool IsTruthy(this object? @this)
	{
		if (@this == null) return false;
		if (@this is bool) return (bool)@this;
		return true;
	}

	public static bool IsFalsey(this object? @this)
	{
		return !@this.IsTruthy();
	}

	public static bool IsEqual(this object? a, object? b)
	{
		if (a == null && b == null) return true;
		if (a == null) return false;

		return a.Equals(b);
	}

	public static string Stringify(this object? @this)
	{
		if (@this == null) return "nil";

		if (@this is double)
		{
			var text = @this.ToString();
			if (text == null) throw new NullReferenceException("This shouldn't have happened.");
			if (text.EndsWith(".0"))
			{
				text = text.Substring(0, text.Length - 2);
			}
			return text;
		}
		else if (@this is string)
		{
			return $"\"{@this}\"";
		}

		return @this.ToString()?.ToLower() ?? string.Empty;
	}
}
