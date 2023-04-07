namespace Core.Parser
{
  public class ErrorContext
  {
    public class Item
    {
      public Location? Location;
      public string Message;
    }
    public List<Item> Errors = new List<Item>();
    public void AddError(Location? location, string message)
    {
      Errors.Add(new Item { Location = location, Message = message });
    }
  }
}
