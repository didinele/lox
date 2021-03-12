// Generated automatically using tools/generate_ast on 3/12/2021 1:49:07 PM UTC

using Lox.Lexer;

namespace Lox.Syntax
{
  public abstract class Expr
  {
    public interface IVisitor<T>
    {
      T Visit(Binary binary);
      T Visit(Grouping grouping);
      T Visit(Literal literal);
      T Visit(Unary unary);
    }

    public class Binary : Expr
    {
      public readonly Expr Left;
      public readonly Token Opr;
      public readonly Expr Right;

      public Binary(Expr left, Token opr, Expr right)
      {
        this.Left = left;
        this.Opr = opr;
        this.Right = right;
      }

      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public class Grouping : Expr
    {
      public readonly Expr Expression;

      public Grouping(Expr expression)
      {
        this.Expression = expression;
      }

      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public class Literal : Expr
    {
      public readonly object Value;

      public Literal(object value)
      {
        this.Value = value;
      }

      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public class Unary : Expr
    {
      public readonly Token Opr;
      public readonly Expr Right;

      public Unary(Token opr, Expr right)
      {
        this.Opr = opr;
        this.Right = right;
      }

      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public abstract T Accept<T>(IVisitor<T> visitor);
  }
}
