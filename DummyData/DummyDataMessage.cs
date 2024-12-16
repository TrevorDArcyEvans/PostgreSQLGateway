namespace DummyData;

using PostgresMessageSerializer;

public class DummyDataMessage : DataRowMessage
{
  public int Id { get; set; }
  public string Name { get; set; }

  public DummyDataMessage(int id, string name)
  {
    Id = id;
    Name = name;
  }

  public override void Update()
  {
    Rows.Clear();
    // Id
    var idField = new RowField
    {
      // WTF?  Have to serialise int32 as string
      Value = SerializerCore.Serialize(Id.ToString())
    };
    Rows.Add(idField);

    // Name
    var nameField = new RowField
    {
      Value = SerializerCore.Serialize(Name)
    };
    Rows.Add(nameField);
  }
}
