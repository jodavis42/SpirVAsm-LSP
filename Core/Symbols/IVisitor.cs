namespace Core.Symbols
{
  public enum VisitResult { Continue, Stop }
  public class IVisitor
  {
    public virtual VisitResult Visit(ISymbol symbol) => VisitResult.Continue;
    public virtual VisitResult Visit(OpCodeSymbol symbol) => Visit(symbol as ISymbol);
    public virtual VisitResult Visit(IdSymbol symbol) => Visit(symbol as ISymbol);
    public virtual VisitResult Visit(ArgumentSymbol symbol) => Visit(symbol as ISymbol);
    public virtual VisitResult Visit(StatementSymbol symbol) => Visit(symbol as ISymbol);
    public virtual VisitResult Visit(SymbolTable symbol) => VisitResult.Continue;
  }
}
