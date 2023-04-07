using System.Diagnostics;

namespace Core
{
  public enum TokenType
  {
    Semicolon,
    Colon,
    Comma,
    Equal,
    Word,
    Number,
    Identifier,
    String,
    Whitespace,
    Newline,
    Comment,
  }
  public class Location
  {
    public int LineStart;
    public int LineEnd;
    public int ColumnStart;
    public int ColumnEnd;
  }


  [DebuggerDisplay("{ToString()}")]
  public class Token
  {
    public TokenType Type;
    public int Start;
    public int Length;
    public Location Location;
    internal Tokenizer tokenizer;
    public override string ToString()
    {
      return tokenizer.Text.Substring(Start, Length);
    }
  }
  public class Tokenizer
  {
    public string Text;
    public int Index = 0;
    public List<Token> Tokens = new List<Token>();

    public void Tokenize(string text)
    {
      Text = text;
      Index = 0;
      Tokens.Clear();
      while(Index < Text.Length)
      {
        if (ParseNewline()) continue;
        if (ParseComment()) continue;
        if (ParseWhitespace()) continue;
        if (ParseIdentifier()) continue;
        if (ParseString()) continue;
        if (ParseSingleToken()) continue;
        if (ParseWord()) continue;
        if (ParseNumber()) continue;
        else
          throw new Exception("Unexpeted Token");
      }
      var line = 0;
      var column = 0;
      foreach(var token in Tokens)
      {
        token.Location = new Location();
        token.Location.LineStart = token.Location.LineEnd = line;
        token.Location.ColumnStart = column;
        token.Location.ColumnEnd = column + token.Length;
        column += token.Length;
        if(token.Type == TokenType.Newline)
        {
          line++;
          column = 0;
        }
      }
    }
    static bool IsNumeric(char c) => c >= '0' && c <= '9';
    static bool IsAlpha(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsNumeric(c);
    bool IsChar(char c) => Text[Index] == c;
    bool CanRead() => Index < Text.Length;
    bool Peek(char c) => CanRead() && IsChar(c);
    bool Accept(char c)
    {
      bool result = Peek(c);
      if(result)
        ++Index;
      return result;
    }
    bool IsNumeric() => CanRead() && IsNumeric(Text[Index]);
    bool IsAlpha() => CanRead() && IsAlpha(Text[Index]);
    bool IsAlphaNumeric() => CanRead() && IsAlphaNumeric(Text[Index]);

    void AddToken(Token token)
    {
      token.tokenizer = this;
      Tokens.Add(token);
    }
    void AddToken(TokenType type, int start, int length) => AddToken(new Token { Type = type, Start = start, Length = length});
    void AddToken(TokenType type, int start) => AddToken(type, start, Index - start);
    
    public bool ParseNewline()
    {
      var start = Index;
      if (Accept('\r'))
      {
        Accept('\n');
        AddToken(TokenType.Newline, start);
        return true;
      }
      if(Accept('\n'))
      {
        AddToken(TokenType.Newline, start);
        return true;
      }
      return false;
    }
    public bool ParseComment()
    {
      var start = Index;
      if (!Accept(';'))
        return false;

      while (CanRead())
      {
        if (IsChar('\r') || IsChar('\n'))
          break;

        ++Index;
      }

      AddToken(TokenType.Comment, start);
      return true;
    }
    public bool ParseWhitespace()
    {
      var start = Index;
      if (!Accept(' '))
        return false;

      while (Accept(' ')) { }

      AddToken(TokenType.Whitespace, start);
      return true;
    }
    public bool ParseSingleToken()
    {
      List<(TokenType Type, char c)> chars = new List<(TokenType Type, char c)>()
      {
        (TokenType.Semicolon, ';'),
        (TokenType.Colon, ':'),
        (TokenType.Comma, ','),
        (TokenType.Equal, '='),
      };
      foreach(var p in chars)
      {
        if (ParseSingleToken(p.Type, p.c))
          return true;
      }
      return false;
    }

    public bool ParseSingleToken(TokenType type, char c)
    {
      var start = Index;
      if (!Accept(c))
        return false;

      AddToken(type, start, 1);
      return true;
    }

    bool ParseNumber()
    {
      var start = Index;
      if (!IsNumeric())
        return false;

      ++Index;
      while (IsNumeric())
        ++Index;
      if(Accept('.'))
      {
        while (IsNumeric())
          ++Index;
      }
      AddToken(TokenType.Number, start);
      return true;
    }

    bool ParseString()
    {
      var start = Index;
      if (!Accept('"'))
        return false;

      while(CanRead())
      {
        if (Accept('"'))
          break;
        ++Index;
      }
      AddToken(TokenType.String, start);
      return true;
    }

    bool ParseWord()
    {
      var start = Index;
      if (!IsAlpha())
        return false;
      ++Index;
      while (CanRead())
      {
        var c = Text[Index];
        if (IsAlphaNumeric(c) || c == '_')
          ++Index;
        else
          break;
      }
      AddToken(TokenType.Word, start);
      return true;
    }

    bool ParseIdentifier()
    {
      var start = Index;
      if (!Accept('%'))
        return false;
      ++Index;
      while (CanRead())
      {
        var c = Text[Index];
        if (IsAlphaNumeric(c) || c == '_')
          ++Index;
        else
          break;
      }
      AddToken(TokenType.Identifier, start);
      return true;
    }
  }
}
