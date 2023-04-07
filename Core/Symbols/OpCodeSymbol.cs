using Core.Parser;

namespace Core.Symbols
{
  public class OpCodeSymbol : ISymbol
  {
    public string? Name;

    public OpCodeSymbol() { }
    public OpCodeSymbol(OpTypeNode node)
    {
      Node = node;
      Location = node.Token.Location;
      Name = node.Token.ToString();
    }
    public override void Walk(IVisitor visitor, bool visit = true)
    {
      if (visit && visitor.Visit(this) == VisitResult.Stop)
        return;
    }
  }
}
