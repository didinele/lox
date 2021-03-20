using System.Collections.Generic;
using Lox.Lexer;
using Lox.Syntax;

namespace Lox.Parser
{
  public class Parser
  {
    private readonly List<Token> Tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
      this.Tokens = tokens;
    }

    public Expr Parse()
    {
      try {
        return this.Expression();
      } catch {
        return null;
      }
    }

    // Utils
    private bool Ended => this.Peek().Type == TokenType.EOF;

    private Token Peek() => this.Tokens[this.current];

    private Token Advance()
    {
      if (!this.Ended) this.current++;
      return this.Previous();
    }

    private Token Previous() => this.Tokens[this.current - 1];

    private bool Check(TokenType type)
    {
      if (this.Ended) return false;
      return this.Peek().Type == type;
    }

    private bool Match(params TokenType[] types)
    {
      foreach (var type in types)
      {
        if (this.Check(type))
        {
          this.Advance();
          return true;
        }
      }

      return false;
    }

    private ParseError Error(Token token, string message)
    {
      if (token.Type == TokenType.EOF)
        LoxUtil.Report(token.Line, "at end", message);
      else
        LoxUtil.Report(token.Line, $"at \"{token.Lexeme}\"", message);

      return new ParseError();
    }

    private Token Consume(TokenType type, string message)
    {
      if (this.Check(type)) return this.Advance();

      throw this.Error(this.Peek(), message);
    }

    // Impls
    private void Synchronize()
    {
      this.Advance();

      while (!this.Ended)
      {
        if (this.Previous().Type == TokenType.SEMICOLON) return;

        switch (this.Peek().Type)
        {
          case TokenType.CLASS:
          case TokenType.FUN:
          case TokenType.VAR:
          case TokenType.FOR:
          case TokenType.IF:
          case TokenType.WHILE:
          case TokenType.PRINT:
          case TokenType.RETURN:
            return;
        }

        this.Advance();
      }
    }

    private Expr Expression() => this.Equality();

    private Expr Equality()
    {
      Expr expr = this.Comparison();

      while (this.Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
      {
        Token opr = this.Previous();
        Expr right = this.Comparison();
        expr = new Expr.Binary(expr, opr, right);
      }

      return expr;
    }

    private Expr Comparison()
    {
      Expr expr = this.Term();

      while (this.Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
      {
        Token opr = this.Previous();
        Expr right = this.Term();
        expr = new Expr.Binary(expr, opr, right);
      }

      return expr;
    }

    private Expr Term()
    {
      Expr expr = this.Factor();

      while (this.Match(TokenType.MINUS, TokenType.PLUS))
      {
        Token opr = this.Previous();
        Expr right = this.Factor();
        expr = new Expr.Binary(expr, opr, right);
      }

      return expr;
    }

    private Expr Factor()
    {
      Expr expr = this.Unary();

      while (this.Match(TokenType.SLASH, TokenType.STAR))
      {
        Token opr = this.Previous();
        Expr right = this.Unary();
        expr = new Expr.Binary(expr, opr, right);
      }

      return expr;
    }

    private Expr Unary()
    {
      if (this.Match(TokenType.BANG, TokenType.MINUS))
      {
        Token opr = this.Previous();
        Expr right = this.Unary();
        return new Expr.Unary(opr, right);
      }

      return this.Primary();
    }

    private Expr Primary()
    {
      if (this.Match(TokenType.FALSE)) return new Expr.Literal(false);
      if (this.Match(TokenType.TRUE)) return new Expr.Literal(true);
      if (this.Match(TokenType.NIL)) return new Expr.Literal(null);

      if (this.Match(TokenType.NUMBER, TokenType.STRING))
        return new Expr.Literal(this.Previous().Literal);

      if (this.Match(TokenType.LEFT_PAREN))
      {
        Expr expr = this.Expression();
        this.Consume(TokenType.RIGHT_PAREN, "Expected \")\" after expression");
        return new Expr.Grouping(expr);
      }

      throw this.Error(this.Peek(), "Expected an expression");
    }
  }
}
