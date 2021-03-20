using System;
using Lox.Interpreter;

namespace Lox
{
  public class LoxUtil
  {
    public static void Error(int line, string message)
    {
      LoxUtil.Report(line, "", message);
    }

    public static void RuntimeError(RuntimeError error)
    {
      Console.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
      Lox.hadRuntimeError = true;
    }

    public static void Report(int line, string where, string message)
    {
      var w = where.Length > 0 ? $" {where}" : "";
      Console.Error.WriteLine($"[line {line}] Error{w}: {message}");
      Lox.hadError = true;
    }
  }
}
