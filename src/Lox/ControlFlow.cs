namespace Lox;

public abstract record ControlFlow
{
	public sealed record Next : ControlFlow;
	public sealed record Break : ControlFlow;
	public sealed record Continue : ControlFlow;
	public sealed record Return(object? Value) : ControlFlow;

	public static readonly ControlFlow NextFlow = new Next();
	public static readonly ControlFlow BreakFlow = new Break();
	public static readonly ControlFlow ContinueFlow = new Continue();
}
