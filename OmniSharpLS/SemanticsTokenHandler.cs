using Core;
using Core.Parser;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharpLS;

namespace SampleServer
{
#pragma warning disable 618
  public class SemanticTokensHandler : SemanticTokensHandlerBase
  {
    private readonly ILogger _logger;
    DocumentManager DocumentManager;
    
    public SemanticTokensHandler(ILogger<SemanticTokensHandler> logger, DocumentManager documentManager)
    {
      _logger = logger;
      DocumentManager = documentManager;
    }

    public override async Task<SemanticTokens?> Handle(SemanticTokensParams request, CancellationToken cancellationToken)
    {
      var result = await base.Handle(request, cancellationToken).ConfigureAwait(false);
      return result;
    }

    public override async Task<SemanticTokens?> Handle(SemanticTokensRangeParams request, CancellationToken cancellationToken)
    {
      var result = await base.Handle(request, cancellationToken).ConfigureAwait(false);
      return result;
    }

    public override async Task<SemanticTokensFullOrDelta?> Handle(SemanticTokensDeltaParams request, CancellationToken cancellationToken)
    {
      var result = await base.Handle(request, cancellationToken).ConfigureAwait(false);
      return result;
    }

    protected override async Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken)
    {
      var visitor = new SemanticsTokenVisitor() { builder = builder };
      var entry = DocumentManager.Find(identifier.TextDocument.Uri);
      entry.Tree?.Walk(visitor);
      await Task.Yield();
    }

    protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken)
    {
      return Task.FromResult(new SemanticTokensDocument(RegistrationOptions.Legend));
    }

    protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability, ClientCapabilities clientCapabilities)
    {
      return new SemanticTokensRegistrationOptions
      {
        DocumentSelector = DocumentSelector.ForLanguage("spvasm"),
        Legend = new SemanticTokensLegend
        {
          TokenModifiers = capability.TokenModifiers,
          TokenTypes = capability.TokenTypes
        },
        Full = new SemanticTokensCapabilityRequestFull
        {
          Delta = true,
        },
      };
    }

    class SemanticsTokenVisitor : IVisitor
    {
      public SemanticTokensBuilder builder;
      SemanticTokenModifier? modifier = null;
      void Push(Core.Location location, SemanticTokenType tokenType)
      {
        var length = location.ColumnEnd - location.ColumnStart;
        builder.Push(location.LineStart, location.ColumnStart, length, tokenType, modifier);
      }
      public override VisitResult Visit(IdentifierNode node)
      {
        Push(node.Token.Location, SemanticTokenType.Variable);
        return VisitResult.Continue;
      }
      public override VisitResult Visit(OpTypeNode node)
      {
        Push(node.Token.Location, SemanticTokenType.Keyword);
        return VisitResult.Continue;
      }
      public override VisitResult Visit(LiteralNode node)
      {
        SemanticTokenType semanticType;
        if (node.Token.Type == Core.TokenType.String)
          semanticType = SemanticTokenType.String;
        else
          semanticType = SemanticTokenType.Number;
        Push(node.Token.Location, semanticType);
        return VisitResult.Continue;
      }
      public override VisitResult Visit(CommentNode node)
      {
        Push(node.Token.Location, SemanticTokenType.Comment);
        return VisitResult.Continue;
      }
    }
  }
#pragma warning restore 618
}
