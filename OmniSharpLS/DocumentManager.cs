using Core.Parser;
using Core.Symbols;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharpLS
{
  public class DocumentManager
  {
    public class Entry
    {
      public ErrorContext ErrorContext = new ErrorContext();
      public DocumentUri? DocumentUri;
      public SyntaxTree? Tree;
      public SymbolTable? Symbols;
    }
    Dictionary<DocumentUri, Entry> EntryMap = new Dictionary<DocumentUri, Entry>();

    public Entry Load(DocumentUri documentURI, string text)
    {
      if(!EntryMap.TryGetValue(documentURI, out var entry))
      {
        entry = new Entry();
        entry.DocumentUri = documentURI;
        EntryMap.Add(documentURI, entry);
      }
      entry.ErrorContext = new ErrorContext();
      var parser = new Parser();
      parser.ErrorContext = entry.ErrorContext;
      parser.TolerantMode = true;
      entry.Tree = parser.Parse(text);
      var semanticAnalysis = new SemanticAnalysis();
      entry.Symbols = semanticAnalysis.Run(entry.Tree);
      return entry;
    }
    public void Unload(DocumentUri documentURI) => EntryMap.Remove(documentURI);
    public Entry? Find(DocumentUri documentURI) => EntryMap.GetValueOrDefault(documentURI);
  }
}
