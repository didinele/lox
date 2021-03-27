 using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Lox.Tools.GenerateAst
{
  class AstGenerator
  {
    public static void Main(string[] args)
    {
      if (args.Length != 1)
      {
        Console.WriteLine("Usage: generate_ast <output directory>");
        Environment.Exit(64);
      }

      AstGenerator.DefineAst(args[0], "Expr", new List<string>{
        "Assign   : Token name, Expr value",
        "Binary   : Expr left, Token opr, Expr right",
        "Grouping : Expr expr",
        "Literal  : object value",
        "Unary    : Token opr, Expr right",
        "Variable : Token name"
      });

      AstGenerator.DefineAst(args[0], "Stmt", new List<string>{
        "Block      : List<Stmt> statements",
        "Expression : Expr expr",
        "Print      : Expr expr",
        "Var        : Token name, Expr initializer"
      });
    }

    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
      string path = $"{outputDir}/{baseName}.cs";

      if (File.Exists(path))
        File.Delete(path);

      using (FileStream fs = File.Create(path))
      {
        AstGenerator.DefineMeta(fs);

        AstGenerator.AddText(fs, "namespace Lox.Syntax\n");
        AstGenerator.AddText(fs, "{\n");

        // Base class
        AstGenerator.AddText(fs, $"  public abstract class {baseName}\n");
        AstGenerator.AddText(fs, "  {\n");

        AstGenerator.DefineVisitor(fs, types);

        foreach (var type in types)
        {
          string[] split = type.Split(':');

          var className = split[0].Trim();
          var fieldList = split[1].Trim();

          AstGenerator.DefineType(fs, baseName, className, fieldList);
        }

        // Base class ending
        AstGenerator.AddText(fs, "\n");
        AstGenerator.AddText(fs, "    public abstract T Accept<T>(IVisitor<T> visitor);\n");
        AstGenerator.AddText(fs, "  }\n");

        // Namespace ending
        AstGenerator.AddText(fs, "}\n");
    }
    }

    private static string UpperCaseFirstChar(string str) => $"{char.ToUpper(str[0])}{str[1..(str.Length)]}";

    private static void AddText(FileStream fs, string value)
    {
      byte[] info = new UTF8Encoding(true).GetBytes(value);
      fs.Write(info, 0, info.Length);
    }

    private static void DefineMeta(FileStream fs)
    {
      AstGenerator.AddText(fs, $"// Generated automatically using tools/generate_ast on {DateTime.UtcNow.ToString()} UTC\n\n");
      AstGenerator.AddText(fs, "using Lox.Lexer;\n");
      AstGenerator.AddText(fs, "using System.Collections.Generic;\n\n");
    }

    private static void DefineVisitor(FileStream fs, List<string> types)
    {
        AstGenerator.AddText(fs, "    public interface IVisitor<T>\n");
        AstGenerator.AddText(fs, "    {\n");
        foreach (var type in types)
        {
          var typeName = type.Split(':')[0].Trim();
          AstGenerator.AddText(fs, $"      T Visit({typeName} {typeName.ToLower()});\n");
        }

        AstGenerator.AddText(fs, "    }\n");
    }

    private static void DefineType(FileStream fs, string baseName, string className, string fieldList)
    {
      AstGenerator.AddText(fs, $"\n    public class {className} : {baseName}\n");
      AstGenerator.AddText(fs, "    {\n");

      // Fields
      var fields = new List<string>(fieldList.Split(", "));
      foreach (var field in fields)
      {
        string[] split = field.Split(' ');

        var fieldType = split[0];
        var fieldName = AstGenerator.UpperCaseFirstChar(split[1]);

        AstGenerator.AddText(fs, $"      public readonly {fieldType} {fieldName};\n");
      }

      AstGenerator.AddText(fs, "\n");

      // Constructor
      AstGenerator.AddText(fs, $"      public {className}({fieldList})\n");
      AstGenerator.AddText(fs, "      {\n");

      foreach (var field in fields)
      {
        var name = field.Split(' ')[1];
        AstGenerator.AddText(fs, $"        this.{AstGenerator.UpperCaseFirstChar(name)} = {name};\n");
      }

      // Constructor ending
      AstGenerator.AddText(fs, "      }\n\n");

      AstGenerator.AddText(fs, "      override public T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);\n");

      // Class ending
      AstGenerator.AddText(fs, "    }\n");
    }
  }
}
