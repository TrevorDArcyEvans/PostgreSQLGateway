namespace DummyData;

using PostgresMessageSerializer;

public class DummyDataDescription : RowDescriptionMessage<DummyDataMessage>
{
  public DummyDataDescription()
  {
    RowFieldDescriptions.Add(new RowFieldDescription(typeof(DummyDataMessage), nameof(DummyDataMessage.Id)));
    RowFieldDescriptions.Add(new RowFieldDescription(typeof(DummyDataMessage), nameof(DummyDataMessage.Name)));
  }
}
