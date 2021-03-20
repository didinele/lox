using Lox.Syntax;
using Lox.Lexer;

namespace Lox.Interpreter
{
  public class Interpreter : Expr.IVisitor<object>
  {
    public void Interpret(Expr expression)
    {
      try {
        object value = this.Evaluate(expression);
        System.Console.WriteLine(this.Stringify(value));
      } catch (RuntimeError error) {
        LoxUtil.RuntimeError(error);
      }
    }

    private object Evaluate(Expr expr) => expr.Accept(this);

    private string Stringify(object value)
    {
      if (value == null) return "nil";

      if (value is double num)
      {
        var text = num.ToString();
        if (text.EndsWith(".0"))
          text = text[0..(text.Length - 2)];

        return text;
      }
      else if (value is bool boolean)
      {
        if (boolean) return "true";
        return "false";
      }

      return value.ToString();
    }

    private bool IsTruthy(object value)
    {
      if (value == null) return false;
      if (value is bool v) return v;
      return true;
    }

    private void CheckNumberOperands(Token opr, bool multiple, params object[] operands)
    {
      bool badOperand = false;

      foreach (var operand in operands)
      {
        if (!(operand is double))
        {
          badOperand = true;
          break;
        }
      }

      if (badOperand) throw new RuntimeError(opr, multiple ? "Operands must be numbers" : "Operand must be a number");
    }

    // Visitors
    object Expr.IVisitor<object>.Visit(Expr.Grouping grouping) => this.Evaluate(grouping.Expression);

    object Expr.IVisitor<object>.Visit(Expr.Literal literal) => literal.Value;

    object Expr.IVisitor<object>.Visit(Expr.Binary binary)
    {
      var left = this.Evaluate(binary.Left);
      var right = this.Evaluate(binary.Right);

      switch (binary.Opr.Type)
      {
        case TokenType.GREATER:
          this.CheckNumberOperands(binary.Opr, true, left, right);
          return (double)left > (double)right;
        case TokenType.GREATER_EQUAL:
          this.CheckNumberOperands(binary.Opr, true, left, right);
          return (double)left >= (double)right;
        case TokenType.LESS:
          this.CheckNumberOperands(binary.Opr, true, left, right);
          return (double)left < (double)right;
        case TokenType.LESS_EQUAL:
          this.CheckNumberOperands(binary.Opr, true, left, right);
          return (double)left <= (double)right;

        case TokenType.EQUAL_EQUAL: return left == right;
        case TokenType.BANG_EQUAL: return left != right;

        case TokenType.MINUS:
          this.CheckNumberOperands(binary.Opr, false, right);
          return (double)left - (double)right;
        case TokenType.PLUS:
          if (left is double lNum && right is double rNum)
            return lNum + rNum;

          if (left is string lStr && right is string rStr)
            return lStr + rStr;

          throw new RuntimeError(binary.Opr, "Operands must be two numbers or two strings");
        case TokenType.SLASH:
          this.CheckNumberOperands(binary.Opr, true, left, right);
          return (double)left / (double)right;
        case TokenType.STAR:
          this.CheckNumberOperands(binary.Opr, true, left, right);
          return (double)left * (double)right;
        // Unreachable
        default: return null;
      }
    }

    object Expr.IVisitor<object>.Visit(Expr.Unary unary)
    {
      var right = this.Evaluate(unary.Right);

      switch (unary.Opr.Type)
      {
        case TokenType.MINUS: return -(double)right;
        case TokenType.BANG: return !this.IsTruthy(right);
        // Unreachable
        default: return null;
      }
    }
  }
}
