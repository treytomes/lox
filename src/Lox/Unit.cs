namespace Lox;

public readonly record struct Unit
{
	public static readonly Unit Value = new();

	// Make it more convenient to use
	public static implicit operator Unit(ValueTuple _) => Value;
}
