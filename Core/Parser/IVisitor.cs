namespace Core.Parser
{
  public enum VisitResult { Continue, Stop }
  public class IVisitor
  {
    public virtual VisitResult Visit(INode node) => VisitResult.Continue;
    public virtual VisitResult Visit(IdentifierNode node) => Visit(node as INode);
    public virtual VisitResult Visit(OpTypeNode node) => Visit(node as INode);
    public virtual VisitResult Visit(LiteralNode node) => Visit(node as INode);
    public virtual VisitResult Visit(CommentNode node) => Visit(node as INode);
    public virtual VisitResult Visit(StatementNode node) => Visit(node as INode);
    public virtual VisitResult Visit(SyntaxTree node) => VisitResult.Continue;
  }
}
