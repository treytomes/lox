namespace Lox;

public abstract record Expr;

public record AssignExpr(Token Name, Expr Value) : Expr;
public record BinaryExpr(Expr Left, Token Operator, Expr Right) : Expr;
public record CallExpr(Expr Callee, Token Paren, List<Expr> Arguments) : Expr;
public record GetExpr(Expr Object, Token Name) : Expr;
public record GroupingExpr(Expr Expression) : Expr;
public record IfExpr(Expr Condition, Expr IfTrue, Expr IfFalse) : Expr;
public record ListExpr(Expr Left, Expr Right) : Expr;
public record LiteralExpr(object? Value) : Expr;
public record LogicalExpr(Expr Left, Token Operator, Expr Right) : Expr;
public record SetExpr(Expr Object, Token Name, Expr Value) : Expr;
public record SuperExpr(Token Keyword, Token Method) : Expr;
public record ThisExpr(Token Keyword) : Expr;
public record UnaryExpr(Token Operator, Expr Right) : Expr;
public record VariableExpr(Token Name) : Expr;
