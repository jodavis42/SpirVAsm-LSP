using Core.Parser;

namespace Core.Symbols
{
  public class SemanticAnalysis
  {
    public SymbolTable Table = new SymbolTable();
    public SymbolTable Run(SyntaxTree tree)
    {
      var preVisitor = new PreVisitor(Table);
      tree.Walk(preVisitor);
      var visitor = new Visitor(Table);
      tree.Walk(visitor);
      return Table;
    }

    class PreVisitor : Core.Parser.IVisitor
    {
      public SymbolTable Table;
      public PreVisitor(SymbolTable table)
      {
        Table = table;
      }

      public override Core.Parser.VisitResult Visit(StatementNode node)
      {
        if (node.OpTypeNode != null)
        {
          var symbol = new StatementSymbol(node);
          if (node.ResultIdNode != null)
            symbol.ResultId = new IdSymbol(node.ResultIdNode);
          //if (node.ResultIdNode != null)
          //  symbol.Location = node.ResultIdNode.Token.Location;
          //else if (node.OpTypeNode != null)
          //  symbol.Location = node.OpTypeNode.Token.Location;
          //else if (node.CommentNode != null)
          //  symbol.Location = node.CommentNode.Token.Location;
          symbol.OpType = new OpCodeSymbol(node.OpTypeNode);

          Table.Symbols.Add(symbol);
          Table.NodeToSymbolMap.Add(node, symbol);
          if (symbol.ResultId != null)
            Table.SymbolMap.Add(symbol.ResultId.ToString(), symbol);
        }
        return Core.Parser.VisitResult.Continue;
      }
    }

    class Visitor : Core.Parser.IVisitor
    {
      public SymbolTable Table;
      StatementSymbol CurrentSymbol = null;
      public Visitor(SymbolTable table)
      {
        Table = table;
      }
      public override Core.Parser.VisitResult Visit(IdentifierNode node)
      {
        if (Table.SymbolMap.TryGetValue(node.Token.ToString(), out var symbol))
        {
          if (CurrentSymbol.Arguments == null)
            CurrentSymbol.Arguments = new List<ArgumentSymbol>();
          var argumentSymbol = new ArgumentSymbol();
          argumentSymbol.Location = node.Token.Location;
          argumentSymbol.Node = node;
          argumentSymbol.StatementSymbol = symbol;
          CurrentSymbol.Arguments.Add(argumentSymbol);
        }
        return Core.Parser.VisitResult.Stop;
      }
      public override Core.Parser.VisitResult Visit(StatementNode node)
      {
        if(!Table.NodeToSymbolMap.TryGetValue(node, out CurrentSymbol))
          return Core.Parser.VisitResult.Stop;
        if (node.ArgumentNodes != null)
        {
          foreach (var argument in node.ArgumentNodes)
            argument.Walk(this);
        }
        CurrentSymbol.BuildLocation();
        return Core.Parser.VisitResult.Stop;
      }
    }
  }
}
