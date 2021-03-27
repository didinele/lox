using System.Text;

namespace Lox.Syntax
{
  public class AstPrinter : Expr.IVisitor<string>
  {
    private string Parenthesize(string name, params Expr[] expressions)
    {
      var builder = new StringBuilder($"({name}", 100);
      foreach (var expr in expressions)
        builder.Append($" {expr.Accept(this)}");

      builder.Append(')');

      return builder.ToString();
    }

    public string Print(Expr expr) => expr.Accept(this);

    // Visitors
    string Expr.IVisitor<string>.Visit(Expr.Binary binary) => this.Parenthesize(binary.Opr.Lexeme, binary.Left, binary.Right);

    string Expr.IVisitor<string>.Visit(Expr.Grouping grouping) => this.Parenthesize("group", grouping.Expr);

    string Expr.IVisitor<string>.Visit(Expr.Literal literal) => literal.Value == null ? "nil" : literal.Value.ToString();

    string Expr.IVisitor<string>.Visit(Expr.Unary unary) => this.Parenthesize(unary.Opr.Lexeme, unary.Right);
  }
}
