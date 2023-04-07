using Core.Parser;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharpLS;
using System.Collections.Immutable;

namespace SampleServer
{
  internal class TextDocumentHandler : TextDocumentSyncHandlerBase
  {
    private readonly ILogger<TextDocumentHandler> _logger;
    DocumentManager DocumentManager;
    ILanguageServerFacade Facade;

    private readonly DocumentSelector _documentSelector = new DocumentSelector(new DocumentFilter { Pattern = "**/*.spvasm" });

    public TextDocumentHandler(ILogger<TextDocumentHandler> logger, DocumentManager documentManager, ILanguageServerFacade facade)
    {
      _logger = logger;
      Facade = facade;
      DocumentManager = documentManager;
    }
    protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities)
    {
      return new TextDocumentSyncRegistrationOptions()
      {
        DocumentSelector = _documentSelector,
        Change = TextDocumentSyncKind.Full,
        Save = new SaveOptions() { IncludeText = true }
      };
    }
    public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) => new TextDocumentAttributes(uri, "spvasm");

    public override Task<Unit> Handle(DidChangeTextDocumentParams notification, CancellationToken token)
    {
      var entry = DocumentManager.Load(notification.TextDocument.Uri, notification.ContentChanges.First().Text);
      PublishErrors(notification.TextDocument.Uri, notification.TextDocument.Version, entry.ErrorContext);
      return Unit.Task;
    }

    public override async Task<Unit> Handle(DidOpenTextDocumentParams notification, CancellationToken token)
    {
      await Task.Yield();
      var entry = DocumentManager.Load(notification.TextDocument.Uri, notification.TextDocument.Text);
      PublishErrors(notification.TextDocument.Uri, notification.TextDocument.Version, entry.ErrorContext);
      return Unit.Value;
    }

    void PublishErrors(DocumentUri uri, int? version, ErrorContext errorContext)
    {
      //var diagnostics = ImmutableArray<Diagnostic>.Empty.ToBuilder();
      var diagnostics = new List<Diagnostic>();
      foreach (var error in errorContext.Errors)
      {
        diagnostics.Add(new Diagnostic()
        {
          Code = "ErrorCode_001",
          Severity = DiagnosticSeverity.Error,
          Message = error.Message,
          Range = error.Location.ToRange(),
          Source = "XXX",
          Tags = new Container<DiagnosticTag>(new DiagnosticTag[] { DiagnosticTag.Unnecessary })
        });
      }
      Facade.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams()
      {
        Diagnostics = diagnostics,
        Uri = uri,
        Version = version,
      });
    }
    public override Task<Unit> Handle(DidCloseTextDocumentParams notification, CancellationToken token) => Unit.Task;
    public override Task<Unit> Handle(DidSaveTextDocumentParams notification, CancellationToken token) => Unit.Task;
  }
}
