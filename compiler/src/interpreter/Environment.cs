using System.Collections.Generic;
using Lox.Lexer;

namespace Lox.Interpreter
{
  public class Environment
  {
    private readonly Environment Parent;

    private readonly Dictionary<string, object> Values = new Dictionary<string, object>();

    public Environment() => this.Parent = null;

    public Environment(Environment parent) => this.Parent = parent;

    public void Define(string name, object value) => this.Values.Add(name, value);

    public void Assign(Token name, object value)
    {
      if (this.Values.ContainsKey(name.Lexeme))
      {
        this.Values.Remove(name.Lexeme);
        this.Values.Add(name.Lexeme, value);

        return;
      }

      if (this.Parent != null)
      {
        this.Parent.Assign(name, value);
        return;
      }

      throw new RuntimeError(name, $"Undefined variable \"{name.Lexeme}\".");
    }

    public object Get(Token name)
    {
      if (this.Values.TryGetValue(name.Lexeme, out var value))
        return value;

      if (this.Parent != null)
        return this.Parent.Get(name);

      throw new RuntimeError(name, $"Undefined variable \"{name.Lexeme}\".");
    }
  }
}
