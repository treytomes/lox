namespace Lox;

static class ObjectExtensions
{
	public static bool IsTruthy(this object @this)
	{
		if (@this == null) return false;
		if (@this is bool) return (bool)@this;
		return true;
	}

	public static bool IsFalsey(this object @this)
	{
		return !@this.IsTruthy();
	}

	public static bool IsEqual(this object a, object b)
	{
		if (a == null && b == null) return true;
		if (a == null) return false;

		return a.Equals(b);
	}
}
