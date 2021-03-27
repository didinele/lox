// Generated automatically using tools/generate_ast on 3/27/2021 2:00:27 PM UTC

using Lox.Lexer;
using System.Collections.Generic;

namespace Lox.Syntax
{
  public abstract class Expr
  {
    public interface IVisitor<T>
    {
      T Visit(Assign assign);
      T Visit(Binary binary);
      T Visit(Grouping grouping);
      T Visit(Literal literal);
      T Visit(Unary unary);
      T Visit(Variable variable);
    }

    public class Assign : Expr
    {
      public readonly Token Name;
      public readonly Expr Value;

      public Assign(Token name, Expr value)
      {
        this.Name = name;
        this.Value = value;
      }

      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
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
      public readonly Expr Expr;

      public Grouping(Expr expr)
      {
        this.Expr = expr;
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

    public class Variable : Expr
    {
      public readonly Token Name;

      public Variable(Token name)
      {
        this.Name = name;
      }

      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public abstract T Accept<T>(IVisitor<T> visitor);
  }
}
