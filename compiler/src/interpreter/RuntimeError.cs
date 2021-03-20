using System;
using Lox.Lexer;

namespace Lox.Interpreter
{
  public class RuntimeError : Exception
  {
    public readonly Token Token;

    public RuntimeError(Token token, string message) : base(message)
    {
      this.Token = token;
    }
  }
}
