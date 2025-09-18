namespace Lox;

public static class ExprExtensions
{
	public static string ToLispString(this Expr expr) => expr switch
	{
		AssignExpr e => $"(set! {e.Name.Lexeme} {e.Value.ToLispString()})",
		BinaryExpr e => $"({e.Operator.Lexeme} {e.Left.ToLispString()} {e.Right.ToLispString()})",
		CallExpr e => $"({e.Callee.ToLispString()} {string.Join(" ", e.Arguments.Select(a => a.ToLispString()))})",
		GetExpr e => $"(get {e.Object.ToLispString()} {e.Name.Lexeme})",
		GroupingExpr e => $"(group {e.Expression.ToLispString()})",
		IfExpr e => $"(if {e.Condition.ToLispString()} ? {e.IfTrue.ToLispString()} : {e.IfFalse.ToLispString()})",
		ListExpr e => $"({e.Left.ToLispString()} {e.Right.ToLispString()})",
		LiteralExpr { Value: null } => "nil",
		LiteralExpr { Value: var v } => v.ToString() ?? "nil",
		LogicalExpr e => $"({e.Operator.Lexeme} {e.Left.ToLispString()} {e.Right.ToLispString()})",
		SetExpr e => $"(set {e.Object.ToLispString()} {e.Name.Lexeme} {e.Value.ToLispString()})",
		SuperExpr e => $"(super {e.Method.Lexeme})",
		ThisExpr => "this",
		UnaryExpr e => $"({e.Operator.Lexeme} {e.Right.ToLispString()})",
		VariableExpr e => e.Name.Lexeme,

		_ => throw new ArgumentException($"Unknown expression type: {expr.GetType()}")
	};
}
