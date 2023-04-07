using Core.Parser;

namespace Core.Symbols
{
  public class StatementSymbol : ISymbol
  {
    public IdSymbol? ResultId;
    public OpCodeSymbol? OpType;
    public List<ArgumentSymbol>? Arguments = null;
    public StatementSymbol(StatementNode node)
    {
      Node = node;
    }
    public override void Walk(IVisitor visitor, bool visit = true)
    {
      if (visit && visitor.Visit(this) == VisitResult.Stop)
        return;

      ResultId?.Walk(visitor);
      OpType?.Walk(visitor);

      if (Arguments != null)
      {
        foreach (var argument in Arguments)
          argument.Walk(visitor);
      }
    }
    public void BuildLocation()
    {
      
      var firstLocation = ResultId?.Location ?? OpType?.Location ?? Arguments?.FirstOrDefault()?.Location;
      var lastLocation = Arguments?.LastOrDefault()?.Location ?? OpType?.Location ?? ResultId?.Location;
      Location = new Location();
      if (firstLocation == null || lastLocation == null)
        return;
      Location.LineStart = firstLocation.LineStart;
      Location.ColumnStart = firstLocation.ColumnStart;
      Location.LineEnd = firstLocation.LineEnd;
      Location.ColumnEnd = firstLocation.ColumnEnd;
    }
  }
}
