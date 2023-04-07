namespace Core.Parser
{
  public class Parser
  {
    public Tokenizer Tokenizer = null;
    public int Index;
    public bool TolerantMode = false;
    public ErrorContext? ErrorContext = null;

    public SyntaxTree Parse(string text)
    {
      Tokenizer t = new Tokenizer();
      t.Tokenize(text);
      return Parse(t);
    }
    public SyntaxTree Parse(Tokenizer tokenizer)
    {
      this.Index = 0;
      Tokenizer = tokenizer;
      var result = new SyntaxTree();
      while(true)
      {
        var statement = Statement();
        if (statement == null)
          break;
        result.Statements.Add(statement);
      }
      return result;
    }
    bool CanRead() => Index < Tokenizer.Tokens.Count;
    void SkipWhitespace()
    {
      while (AcceptRaw(TokenType.Whitespace)) { };
    }
    void SkipNewlineAndWhitespace()
    {
      while (AcceptRaw(TokenType.Whitespace) || AcceptRaw(TokenType.Newline)) { };
    }
    void RecoverFromError()
    {
      while (CanRead() && !AcceptRaw(TokenType.Newline))
        ++Index;
    }
    bool AcceptRaw(TokenType tokenType)
    {
      if(CanRead() && Tokenizer.Tokens[Index].Type == tokenType)
      {
        ++Index;
        return true;
      }
      return false;
    }
    bool Accept(TokenType tokenType)
    {
      SkipWhitespace();
      if (CanRead() && Tokenizer.Tokens[Index].Type == tokenType)
      {
        ++Index;
        return true;
      }
      return false;
    }
    bool Accept(TokenType tokenType, out Token result)
    {
      SkipWhitespace();
      if (CanRead() && Tokenizer.Tokens[Index].Type == tokenType)
      {
        result = Tokenizer.Tokens[Index];
        ++Index;
        return true;
      }
      result = null;
      return false;
    }
    void ReportError(Location? location, string message)
    {
      if(!TolerantMode)
        throw new Exception();
      ErrorContext?.AddError(location, message);
    }
    Location? LastLocation => CanRead() ? Tokenizer.Tokens[Index].Location : null;
    bool Expect(bool state, string message)
    {
      if (!state)
        ReportError(LastLocation, message);
      return state;
    }
    bool Expect(TokenType tokenType, string message = null)
    {
      bool result = Accept(tokenType);
      if (!result)
        ReportError(LastLocation, message ?? $"Expected Token '{tokenType}'");
      return result;
    }
    T? Expect<T>(T? node, string message = null) where T : INode
    {
      Expect(node != null, message);
      return node;
    }
    StatementNode? Statement()
    {
      // statement: statement0 | statement1 | statement2 ;
      // statement0: id '=' OpType id* comment? ;
      // statement1: OpType id* comment? ;
      // statement2: comment ;
      SkipNewlineAndWhitespace();
      var result = Statement0();
      if (result != null)
        return result;
      result = Statement1();
      if (result != null)
        return result;
      result = Statement2();
      return result;
    }
    StatementNode? Statement0()
    {
      // statement0: id '=' OpType id* comment? ;
      var resultIdentifier = Identifier();
      if (resultIdentifier == null)
        return null;

      var result = new StatementNode();
      result.ResultIdNode = resultIdentifier;
      if (!Expect(TokenType.Equal))
      {
        RecoverFromError();
        return result;
      }

      result.EqualToken = Tokenizer.Tokens[Index - 1];
      result.OpTypeNode = Expect(OpType(), "Expected OpCode");
      ParseIdArguments(result);
      result.CommentNode = Comment();
      if (!Expect(TokenType.Newline))
        RecoverFromError();
      return result;
    }
    StatementNode? Statement1()
    {
      //// statement1: OpType id* comment? ;
      var opTypeNode = OpType();
      if (opTypeNode == null)
        return null;

      var result = new StatementNode();
      result.OpTypeNode = opTypeNode;
      ParseIdArguments(result);
      result.CommentNode = Comment();
      if(!Expect(TokenType.Newline))
        RecoverFromError();
      return result;
    }
    StatementNode? Statement2()
    {
      //// statement1: OpType id* comment? ;
      var commentNode = Comment();
      if (commentNode == null)
        return null;

      var result = new StatementNode();
      result.CommentNode = commentNode;
      if(!Expect(TokenType.Newline))
        RecoverFromError();
      return result;
    }
    void ParseIdArguments(StatementNode node)
    {
      node.ArgumentNodes = new List<INode>();
      while (true)
      {
        INode? argument = null;
        argument = Identifier();
        if (argument != null)
        {
          node.ArgumentNodes.Add(argument);
          continue;
        }
        argument = Literal();
        if (argument != null)
        {
          node.ArgumentNodes.Add(argument);
          continue;
        }
        break;
      }
    }
    IdentifierNode? Identifier()
    {
      if(!Accept(TokenType.Identifier, out var token))
        return null;
      return new IdentifierNode { Token = token };
    }
    LiteralNode? Literal()
    {
      Token token = null;
      bool isLiteral = Accept(TokenType.String, out token) || Accept(TokenType.Number, out token) || Accept(TokenType.Word, out token);
      if (!isLiteral)
        return null;
      return new LiteralNode { Token = token };
    }
    OpTypeNode? OpType()
    {
      if (!Accept(TokenType.Word, out var token))
        return null;
      return new OpTypeNode { Token = token };
    }
    CommentNode? Comment()
    {
      if (!Accept(TokenType.Comment, out var token))
        return null;
      return new CommentNode { Token = token };
    }
  }
}
