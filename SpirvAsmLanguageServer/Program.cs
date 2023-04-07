// See https://aka.ms/new-console-template for more information
using Core;
using Core.Parser;
using Core.Symbols;

Console.WriteLine("Hello, World!");
Tokenizer t = new Tokenizer();
t.Tokenize(@"OpEntryPoint Vertex %EntryPoint_Main ""EntryPoint_Main"" %Uv %Out
");

t.Tokenize(File.ReadAllText("C:\\Users\\Josh\\source\\repos\\SpirvAsmLanguageServer\\Examples\\Test.spvasm"));
var parser = new Parser();
parser.ErrorContext = new ErrorContext();
parser.TolerantMode = true;
var tree = parser.Parse(t);
var c = tree.Statements.Count;
SemanticAnalysis s = new SemanticAnalysis();
s.Run(tree);
var table = s.Table;
var searcher = new LocationSearchingVisitor();
searcher.Find(table, 999, 0);