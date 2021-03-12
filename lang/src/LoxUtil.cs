using System;

namespace Lox
{
  public class LoxUtil
  {
    public static void Error(int line, string message)
    {
      LoxUtil.Report(line, "", message);
    }

    public static void Report(int line, string where, string message)
    {
      Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
      Lox.hadError = true;
    }
  }
}
