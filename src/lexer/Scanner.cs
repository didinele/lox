using System.Collections.Generic;

namespace Lox.Lexer
{
  class Scanner
  {
    private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
    {
      { "and", TokenType.AND },
      { "class", TokenType.CLASS },
      { "else", TokenType.ELSE },
      { "false", TokenType.FALSE },
      { "for", TokenType.FOR },
      { "fun", TokenType.FUN },
      { "if", TokenType.IF },
      { "nil", TokenType.NIL },
      { "or", TokenType.OR },
      { "print", TokenType.PRINT },
      { "return", TokenType.RETURN },
      { "super", TokenType.SUPER },
      { "this", TokenType.THIS },
      { "true", TokenType.TRUE },
      { "var", TokenType.VAR },
      { "while", TokenType.WHILE }
    };

    private readonly string Source;
    private readonly List<Token> Tokens = new List<Token>();

    private int start = 0;
    private int current = 0;
    private int line = 1;

    public Scanner(string source)
    {
      this.Source = source;
    }

    private bool Ended
    {
      get => this.current >= this.Source.Length;
    }

    private char Advance() => this.Source[this.current++];

    private bool MatchesAhead(char expected)
    {
      if (this.Ended) return false;
      if (this.Source[this.current] != expected) return false;

      current++;
      return true;
    }

    private char Peek()
    {
      if (this.Ended) return '\0';
      return this.Source[current];
    }

    private char PeekNext()
    {
      if (this.current + 1 >= this.Source.Length) return '\0';
      return this.Source[current + 1];
    }

    private void HandleString()
    {
      while (this.Peek() != '"' && !this.Ended)
      {
        if (this.Peek() == '\n') this.line++;
        this.Advance();
      }

      if (this.Ended)
      {
        LoxUtil.Error(line, "Undetermined string");
        return;
      }

      // Closing "
      this.Advance();

      // Trim the surrounding quotes
      this.AddToken(TokenType.STRING, this.Source[(this.start + 1)..(this.current - 1)]);
    }

    private bool IsDigit(char c) => c >= '0' && c <= '9';

    private void HandleNumber()
    {
      while (this.IsDigit(this.Peek()))
        this.Advance();

      // Look for a fractional part.
      if (this.Peek() == '.' && this.IsDigit(this.PeekNext()))
      {
        // Consume the "."
        this.Advance();

        while (this.IsDigit(this.Peek()))
          this.Advance();
      }

      this.AddToken(
        TokenType.NUMBER,
        double.Parse(this.Source[this.start..this.current])
      );
    }

    private bool IsAlpha(char c) => (c >= 'a' && c < 'z') || (c >= 'A' && c <= 'Z') || c == '_';

    private bool IsAlphaNumeric(char c) => this.IsAlpha(c) || this.IsDigit(c);

    private void HandleIdentifier()
    {
      while (this.IsAlphaNumeric(this.Peek()))
        this.Advance();

      this.AddToken(
        Scanner.Keywords.GetValueOrDefault(
          this.Source[this.start..this.current],
          TokenType.IDENTIFIER
        )
      );
    }

    private void AddToken(TokenType type) => this.AddToken(type, null);

    private void AddToken(TokenType type, object literal)
    {
      string text = this.Source[this.start..this.current];
      this.Tokens.Add(new Token(type, text, literal, this.line));
    }

    private void ScanToken()
    {
      char c = this.Advance();
      switch (c)
      {
        case '(': this.AddToken(TokenType.LEFT_PAREN); break;
        case ')': this.AddToken(TokenType.RIGHT_PAREN); break;
        case '{': this.AddToken(TokenType.LEFT_BRACE); break;
        case '}': this.AddToken(TokenType.RIGHT_BRACE); break;
        case ',': this.AddToken(TokenType.COMMA); break;
        case '.': this.AddToken(TokenType.DOT); break;
        case '-': this.AddToken(TokenType.MINUS); break;
        case '+': this.AddToken(TokenType.PLUS); break;
        case ';': this.AddToken(TokenType.SEMICOLON); break;
        case '*': this.AddToken(TokenType.STAR); break;

        case '!': this.AddToken(this.MatchesAhead('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
        case '=': this.AddToken(this.MatchesAhead('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
        case '<': this.AddToken(this.MatchesAhead('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
        case '>': this.AddToken(this.MatchesAhead('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;

        case '/':
          if (this.MatchesAhead('/'))
            while (this.Peek() != '\n' && !this.Ended) this.Advance();
          else
            this.AddToken(TokenType.SLASH);
          break;

        // Ignore whitespace.
        case ' ':
        case '\r':
        case '\t':
          break;
        case '\n': this.line++; break;

        case '"': this.HandleString(); break;

        default:
          if (this.IsDigit(c))
            this.HandleNumber();
          else if (this.IsAlpha(c))
            this.HandleIdentifier();
          else
            LoxUtil.Error(line, $"Unexpected character: {c}");

          break;
      }
    }

    public List<Token> ScanTokens()
    {
      while (!this.Ended)
      {
        this.start = this.current;
        this.ScanToken();
      }

      this.Tokens.Add(new Token(TokenType.EOF, "", null, line));
      return this.Tokens;
    }
  }
}
