namespace Lox;

public abstract record Stmt;

public record BlockStmt(IList<Stmt> Statements) : Stmt;
public record ClassStmt(Token Name, VariableExpr SuperClass, IList<FunctionStmt> Methods) : Stmt;
public record ExpressionStmt(Expr Expression) : Stmt;
public record FunctionStmt(Token Name, IList<Token> Params, IList<Stmt> Body) : Stmt;
public record IfStmt(Expr Condition, Stmt ThenBranch, Stmt? ElseBranch) : Stmt;
public record PrintStmt(Expr Expression) : Stmt;
public record ReturnStmt(Token Keyword, Expr Value) : Stmt;
public record VarStmt(Token Name, Expr? Initializer) : Stmt;
public record WhileStmt(Expr Condition, Stmt Body) : Stmt;
