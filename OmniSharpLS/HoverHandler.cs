using Core.Spec;
using Core.Symbols;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Text;

namespace OmniSharpLS
{
  internal class HoverHandler : IHoverHandler
  {
    DocumentManager DocumentManager;
    public HoverHandler(DocumentManager documentManager)
    {
      DocumentManager = documentManager;
    }
    public HoverRegistrationOptions GetRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities)
    {
      return new HoverRegistrationOptions
      {
        DocumentSelector = DocumentSelector.ForLanguage("spvasm"),
      };
    }

    public Task<Hover?> Handle(HoverParams request, CancellationToken cancellationToken)
    {
      var uri = request.TextDocument.Uri;
      var entry = DocumentManager.Find(uri);
      LocationSearchingVisitor searcher = new LocationSearchingVisitor();
      var resultSymbol = searcher.Find(entry.Symbols, request.Position.Line, request.Position.Character);
      Hover? result = null;
      if(resultSymbol != null)
      {
        if(resultSymbol is ArgumentSymbol argSymbol)
        {
          result = new Hover
          {
            Range = argSymbol.Location.ToRange(),
            Contents = new MarkedStringsOrMarkupContent(ToString(argSymbol?.StatementSymbol)),
          };
        }
        else if (resultSymbol is OpCodeSymbol opTypeSymbol)
        {
          OpTypeRegistry registry = new OpTypeRegistry();
          registry.Initialize();
          var opCodeInfo = registry.Find(opTypeSymbol.Name);
          string content = opTypeSymbol.Name;
          if (opCodeInfo != null)
            content = opCodeInfo.ToString();

          result = new Hover
          {
            Range = opTypeSymbol.Location.ToRange(),
            Contents = new MarkedStringsOrMarkupContent(content),
          };
        }
        else if (resultSymbol is StatementSymbol statementSymbol)
        {
          result = new Hover
          {
            Range = statementSymbol.Location.ToRange(),
            Contents = new MarkedStringsOrMarkupContent(ToString(statementSymbol)),
          };
        }
      }

      return Task.FromResult(result);
    }
    public string ToString(StatementSymbol? symbol)
    {
      return symbol?.Node?.ToString() ?? string.Empty;
    }
  }
}
