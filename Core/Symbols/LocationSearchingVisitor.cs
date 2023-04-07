namespace Core.Symbols
{
  public class LocationSearchingVisitor : IVisitor
  {
    int Line = 0;
    int Column = 0;
    List<ISymbol> Symbols = new List<ISymbol>();
    ISymbol? Result = null;
    public ISymbol? Find(SymbolTable table, int line, int column)
    {
      Line = line;
      Column = column;
      table?.Walk(this);
      return Result;
    }
    public override VisitResult Visit(OpCodeSymbol symbol) => TrySetSymbol(symbol);
    public override VisitResult Visit(IdSymbol symbol) => TrySetSymbol(symbol);
    public override VisitResult Visit(ArgumentSymbol symbol) => TrySetSymbol(symbol);
    public override VisitResult Visit(StatementSymbol symbol)
    {
      if (Contains(symbol.Location))
      {
        Symbols.Add(symbol);
        Result = symbol;
//        return VisitResult.Stop;
      }
      //symbol.Walk(this, false);
      return VisitResult.Continue;
    }
    VisitResult TrySetSymbol(ISymbol symbol)
    {
      if (Contains(symbol.Location))
      {
        Symbols.Add(symbol);
        Result = symbol;
        return VisitResult.Stop;
      }
      return VisitResult.Continue;
    }

    bool Contains(Location location)
    {
      if (location == null)
        return false;

      if (Line < location.LineStart)
        return false;
      if (Line > location.LineEnd)
        return false;

      if(location.LineStart == location.LineEnd)
      {
        return location.ColumnStart <= Column && Column <= location.ColumnEnd;
      }
      else
      {
        if (location.LineStart == Line)
          return location.ColumnStart <= Column;
        if (location.LineEnd == Line)
          return Column <= location.ColumnEnd;
        return true;
      }
    }
  }
}
