namespace Lox;

public abstract record ControlFlow
{
	public sealed record Next : ControlFlow;
	public sealed record Break(Token Keyword) : ControlFlow;
	public sealed record Continue(Token Keyword) : ControlFlow;
	public sealed record Return(object? Value) : ControlFlow;

	public static readonly ControlFlow NextFlow = new Next();
}
