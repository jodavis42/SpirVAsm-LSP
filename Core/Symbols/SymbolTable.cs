using Core.Parser;

namespace Core.Symbols
{
  public class SymbolTable
  {
    public List<StatementSymbol> Symbols = new List<StatementSymbol>();
    public Dictionary<string, StatementSymbol> SymbolMap = new Dictionary<string, StatementSymbol>();
    public Dictionary<INode, StatementSymbol> NodeToSymbolMap = new Dictionary<INode, StatementSymbol>();

    public void Walk(IVisitor visitor, bool visit = true)
    {
      if (visit && visitor.Visit(this) == VisitResult.Stop)
        return;

      if (Symbols != null)
      {
        foreach (var symbol in Symbols)
          symbol.Walk(visitor);
      }
    }
  }
}
