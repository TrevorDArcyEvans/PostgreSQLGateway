namespace DummyData;

using PostgresMessageSerializer;

public class DummyDataMessage : DataRowMessage
{
  public int Id { get; }
  public string Name { get; }

  public DummyDataMessage(int id, string name)
  {
    // Id
    var idField = new RowField
    {
      // WTF?  Have to serialise int32 as string
      Value = SerializerCore.Serialize(id.ToString())
    };
    Rows.Add(idField);

    // Name
    var nameField = new RowField
    {
      Value = SerializerCore.Serialize(name)
    };
    Rows.Add(nameField);
  }
}
