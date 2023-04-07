using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniRange = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace OmniSharpLS
{
  public static class Extensions
  {
    public static OmniRange ToRange(this Core.Location location)
    {
      return new OmniRange
      {
        Start = new Position { Line = location.LineStart, Character = location.ColumnStart },
        End = new Position { Line = location.LineEnd, Character = location.ColumnEnd },
      };
    }
  }
}
