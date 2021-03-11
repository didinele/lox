﻿using System;
using System.IO;
using System.Collections.Generic;

namespace Lox
{
  class Lox
  {
    public static bool hadError = false;

    public static void Main(string[] args)
    {
      if (args.Length > 1)
      {
        Console.WriteLine("Usage: jlox [script]");
        Environment.Exit(64);
      }
      else if (args.Length == 1)
      {
        Lox.RunFile(args[0]);
      }
      else
      {
        Lox.RunPrompt();
      }
    }

    private static void RunFile(string path)
    {
      if (!File.Exists(path))
      {
        Console.WriteLine("File not found");
        Environment.Exit(66);
      }

      Lox.Run(File.ReadAllText(path));
      if (Lox.hadError) Environment.Exit(65);
    }

    private static void RunPrompt()
    {
      while (true)
      {
        Console.Write('>');
        string line = Console.ReadLine();
        if (line == null) break;
        Lox.Run(line);
        Lox.hadError = false;
      }
    }

    private static void Run(string source)
    {
      var scanner = new Lexer.Scanner(source);
      List<Lexer.Token> tokens = scanner.ScanTokens();

      foreach (var token in tokens)
      {
        Console.WriteLine(token);
      }
    }
  }
}
