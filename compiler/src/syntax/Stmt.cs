// Generated automatically using tools/generate_ast on 3/27/2021 2:00:27 PM UTC

using Lox.Lexer;
using System.Collections.Generic;

namespace Lox.Syntax
{
  public abstract class Stmt
  {
    public interface IVisitor<T>
    {
      T Visit(Block block);
      T Visit(Expression expression);
      T Visit(Print print);
      T Visit(Var var);
    }

    public class Block : Stmt
    {
      public readonly List<Stmt> Statements;

      public Block(List<Stmt> statements)
      {
        this.Statements = statements;
      }

      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public class Expression : Stmt
    {
      public readonly Expr Expr;

      public Expression(Expr expr)
      {
        this.Expr = expr;
      }

      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public class Print : Stmt
    {
      public readonly Expr Expr;

      public Print(Expr expr)
      {
        this.Expr = expr;
      }

      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public class Var : Stmt
    {
      public readonly Token Name;
      public readonly Expr Initializer;

      public Var(Token name, Expr initializer)
      {
        this.Name = name;
        this.Initializer = initializer;
      }

      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }

    public abstract T Accept<T>(IVisitor<T> visitor);
  }
}
