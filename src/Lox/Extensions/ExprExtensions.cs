namespace Lox;

public static class ExprExtensions
{
	public static string ToLispString(this Expr expr) => expr switch
	{
		LiteralExpr { Value: null } => "nil",
		LiteralExpr { Value: var v } => v.ToString() ?? "nil",
		VariableExpr e => e.Name.Lexeme,

		UnaryExpr e => $"({e.Operator.Lexeme} {e.Right.ToLispString()})",
		BinaryExpr e => $"({e.Operator.Lexeme} {e.Left.ToLispString()} {e.Right.ToLispString()})",

		GroupingExpr e => $"(group {e.Expression.ToLispString()})",
		AssignExpr e => $"(set! {e.Name.Lexeme} {e.Value.ToLispString()})",

		LogicalExpr e => $"({e.Operator.Lexeme} {e.Left.ToLispString()} {e.Right.ToLispString()})",
		CallExpr e => $"({e.Callee.ToLispString()} {string.Join(" ", e.Arguments.Select(a => a.ToLispString()))})",

		GetExpr e => $"(get {e.Object.ToLispString()} {e.Name.Lexeme})",
		SetExpr e => $"(set {e.Object.ToLispString()} {e.Name.Lexeme} {e.Value.ToLispString()})",

		ListExpr e => $"({e.Left.ToLispString()} {e.Right.ToLispString()})",

		ThisExpr => "this",
		SuperExpr e => $"(super {e.Method.Lexeme})",

		_ => throw new ArgumentException($"Unknown expression type: {expr.GetType()}")
	};
}
