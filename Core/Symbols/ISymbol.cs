using Core.Parser;

namespace Core.Symbols
{
  public class ISymbol
  {
    public Location Location;
    public INode Node;

    public virtual void Walk(IVisitor visitor, bool visit = true) { }
  }
}
