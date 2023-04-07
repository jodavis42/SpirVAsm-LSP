using Core.Symbols;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharpLS
{
  public class DefinitionHandler : IDefinitionHandler
  {
    DocumentManager DocumentManager;
    public DefinitionHandler(DocumentManager documentManager)
    {
      DocumentManager = documentManager;
    }

    public DefinitionRegistrationOptions GetRegistrationOptions(DefinitionCapability capability, ClientCapabilities clientCapabilities)
    {
      return new DefinitionRegistrationOptions
      {
        DocumentSelector = DocumentSelector.ForLanguage("spvasm")
      };
    }
    public Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken)
    {
      var location = FindLocation(request.TextDocument.Uri, request.Position);
      var results = new List<LocationOrLocationLink>();
      if (location != null)
        results.Add(location);
      var result = new LocationOrLocationLinks(results);
      return Task.FromResult(result);
    }
    Location? FindLocation(DocumentUri uri, Position position)
    {
      Location? resultLocation = null;
      
      var entry = DocumentManager.Find(uri);
      if (entry?.Symbols == null)
        return resultLocation;

      LocationSearchingVisitor searcher = new LocationSearchingVisitor();
      var resultSymbol = searcher.Find(entry.Symbols, position.Line, position.Character);
      if (resultSymbol == null)
        return resultLocation;

      if(resultSymbol is ArgumentSymbol argSymbol)
      {
        var location = argSymbol.StatementSymbol?.Location;
        if(location != null)
        {
          resultLocation = new Location()
          {
            Uri = uri,
            Range = location.ToRange(),
          };
        }
      }
      return resultLocation;
    }
  }
}
