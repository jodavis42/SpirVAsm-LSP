using Core.Parser;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharpLS;

namespace SampleServer
{
  internal class FoldingRangeHandler : IFoldingRangeHandler
  {
    DocumentManager DocumentManager;
    public FoldingRangeHandler(DocumentManager documentManager)
    {
      DocumentManager = documentManager;
    }

    public FoldingRangeRegistrationOptions GetRegistrationOptions()
    {
      return new FoldingRangeRegistrationOptions
      {
        DocumentSelector = DocumentSelector.ForLanguage("spvasm")
      };
    }
    public FoldingRangeRegistrationOptions GetRegistrationOptions(FoldingRangeCapability capability, ClientCapabilities clientCapabilities) => GetRegistrationOptions();

    public async Task<Container<FoldingRange>?> Handle(FoldingRangeRequestParam request, CancellationToken cancellationToken)
    {
      var entry = DocumentManager.Find(request.TextDocument.Uri);
      var visitor = new Visitor();
      entry?.Tree?.Walk(visitor);
      return new Container<FoldingRange>(visitor.Ranges);
    }

    public class Visitor : IVisitor
    {
      public List<FoldingRange> Ranges = new List<FoldingRange>();
      Core.Location Start;
      public override VisitResult Visit(StatementNode node)
      {
        var tokenType = node.OpTypeNode?.Token?.ToString();
        if (tokenType == "OpFunction")
        {
          Start = node.OpTypeNode.Token.Location;
        }
        if (tokenType == "OpFunctionEnd" && Start != null)
        {
          var end = node.OpTypeNode.Token.Location;
          Ranges.Add(new FoldingRange()
          {
            StartLine = Start.LineStart,
            EndLine = end.LineEnd,
            StartCharacter = Start.ColumnStart,
            EndCharacter = end.ColumnEnd,
            Kind = FoldingRangeKind.Region,
          });
        }
        return VisitResult.Continue;
      }
    }
  }
}
