using System.Text;

namespace Core.Parser
{
  public class INode
  {
    public virtual void Walk(IVisitor visitor, bool visitNode = true) { }
  }
  public class IdentifierNode : INode
  {
    public Token? Token;
    public override string ToString() => Token?.ToString() ?? string.Empty;
    public override void Walk(IVisitor visitor, bool visitNode = true)
    {
      if (visitNode && visitor.Visit(this) == VisitResult.Stop)
        return;
    }
  }
  public class OpTypeNode : INode
  {
    public Token? Token;
    public override string ToString() => Token?.ToString() ?? string.Empty;
    public override void Walk(IVisitor visitor, bool visitNode = true)
    {
      if (visitNode && visitor.Visit(this) == VisitResult.Stop)
        return;
    }
  }
  public class LiteralNode : INode
  {
    public Token? Token;
    public override string ToString() => Token?.ToString() ?? string.Empty;
    public override void Walk(IVisitor visitor, bool visitNode = true)
    {
      if (visitNode && visitor.Visit(this) == VisitResult.Stop)
        return;
    }
  }
  public class CommentNode : INode
  {
    public Token? Token;
    public override string ToString() => Token?.ToString() ?? string.Empty;
    public override void Walk(IVisitor visitor, bool visitNode = true)
    {
      if (visitNode && visitor.Visit(this) == VisitResult.Stop)
        return;
    }
  }
  public class StatementNode : INode
  {
    public IdentifierNode? ResultIdNode;
    public Token? EqualToken;
    public OpTypeNode? OpTypeNode;
    public List<INode>? ArgumentNodes;
    public CommentNode? CommentNode;
    public override string ToString()
    {
      var builder = new StringBuilder();
      if(ResultIdNode != null)
      {
        builder.Append(ResultIdNode.ToString());
        builder.Append(" = ");
      }
      if(OpTypeNode != null) 
      {
        builder.Append(OpTypeNode?.ToString());
        builder.Append(" ");
      }
      if(ArgumentNodes != null)
      {
        foreach(var argNode in ArgumentNodes)
        {
          builder.Append(argNode.ToString());
          builder.Append(" ");
        }
      }
      return builder.ToString();
    }

    public override void Walk(IVisitor visitor, bool visitNode = true)
    {
      if (visitNode && visitor.Visit(this) == VisitResult.Stop)
        return;

      ResultIdNode?.Walk(visitor, true);
      OpTypeNode?.Walk(visitor, true);
      if(ArgumentNodes != null)
      {
        foreach (var node in ArgumentNodes)
          node.Walk(visitor, true);
      }
      ResultIdNode?.Walk(visitor, true);
      CommentNode?.Walk(visitor, true);
    }
  }

  public class SyntaxTree
  {
    public List<StatementNode> Statements = new List<StatementNode>();

    public void Walk(IVisitor visitor, bool visitNode = true) 
    {
      if (visitNode && visitor.Visit(this) == VisitResult.Stop)
        return;

      if(Statements != null)
      {
        foreach (var node in Statements)
          node.Walk(visitor, true);
      }
    }
  }
}
