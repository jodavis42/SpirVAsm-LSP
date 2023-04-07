using Core.Parser;

namespace Core.Symbols
{
  public class ArgumentSymbol : ISymbol
  {
    public StatementSymbol? StatementSymbol;

    public override void Walk(IVisitor visitor, bool visit = true)
    {
      if (visit && visitor.Visit(this) == VisitResult.Stop)
        return;
    }
  }
}
