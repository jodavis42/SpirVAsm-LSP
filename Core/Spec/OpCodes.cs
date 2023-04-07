using System.Text;

namespace Core.Spec
{
  public enum OpCodes { Unknown, OpName, OpMemberName, OpTypeFloat, OpTypeVector, OpAccessChain, OpFunctionParameter }
  public enum ArgumentType { ResultId, Id, Literal, OpType}
  public class OpCode
  {
    public class Element 
    { 
      public ArgumentType Type; public string Name; public bool AllowMultiple; }
    public bool HasResultId;
    public OpCodes opCode;
    public List<Element> Arguments;

    public OpCode() 
    {
      HasResultId = false;
      opCode = OpCodes.Unknown;
      Arguments = new List<Element>();
    }
    public OpCode(OpCodes opCode, params Element[] arguments) : this(true, opCode, arguments) { }
    public OpCode(bool hasResultId, OpCodes opCode, params Element[] arguments)
    {
      HasResultId = hasResultId;
      this.opCode = opCode;
      Arguments = arguments.ToList();
    }
    public OpCode AddArg(ArgumentType argType, string name, bool allowMultiple = false)
    {
      Arguments.Add(new Element { Type = argType, Name = name, AllowMultiple = allowMultiple });
      return this;
    }
    public override string ToString()
    {
      var builder = new StringBuilder();
      if(HasResultId)
        builder.Append("Id ");
      builder.Append(opCode.ToString());
      builder.Append("(");
      for(var i = 0; i < Arguments.Count; i++)
      {
        var arg = Arguments[i];
        if(i != 0)
          builder.Append(", ");
        builder.Append(arg.Type.ToString());
        if (arg.AllowMultiple)
          builder.Append("[]");
        builder.Append(" ");
        builder.Append(arg.Name);
      }
      builder.Append(")");
      return builder.ToString();
    }
  }
  public class OpTypeRegistry
  {
    public List<OpCode> OpCodeList = new List<OpCode>();
    public Dictionary<OpCodes, OpCode> OpCodeMap = new Dictionary<OpCodes, OpCode>();

    public void Register(OpCode opCode)
    {
      OpCodeList.Add(opCode);
      OpCodeMap.Add(opCode.opCode, opCode);
    }
    public OpCode Find(string opCodeName)
    {
      foreach(var opCode in OpCodeList)
      {
        if (opCode.opCode.ToString() == opCodeName)
          return opCode;
      }
      return null;
    }
    public void Initialize()
    {
      Register(new OpCode(false, OpCodes.OpName).AddArg(ArgumentType.Id, "target").AddArg(ArgumentType.Literal, "name"));
      Register(new OpCode(false, OpCodes.OpMemberName).AddArg(ArgumentType.Id, "type").AddArg(ArgumentType.Literal, "member").AddArg(ArgumentType.Literal, "name"));
      Register(new OpCode(OpCodes.OpTypeFloat).AddArg(ArgumentType.Literal, "width"));
      Register(new OpCode(OpCodes.OpTypeVector).AddArg(ArgumentType.Id, "componentType").AddArg(ArgumentType.Literal, "componentCount"));
      Register(new OpCode(OpCodes.OpAccessChain).AddArg(ArgumentType.Id, "base").AddArg(ArgumentType.Id, "indices", true));
    }
  }
}
