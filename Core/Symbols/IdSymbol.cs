using Core.Parser;

namespace Core.Symbols
{
  public class IdSymbol : ISymbol
  {
    public StatementSymbol? StatementSymbol;
    public IdSymbol(IdentifierNode node)
    {
      Node = node;
      Location = node.Token.Location;
    }
    public override void Walk(IVisitor visitor, bool visit = true)
    {
      if (visit && visitor.Visit(this) == VisitResult.Stop)
        return;
    }
    public override string ToString() => Node.ToString();
  }
}
