using System.Collections.Generic;

namespace PostgresMessageSerializer;

public class RowDescriptionMessage : BackendMessage
{
  public static byte MessageTypeId = (byte)'T';

  public short FieldsCount { get; set; }

  public IList<RowFieldDescription> RowFieldDescriptions { get; set; }

  public override void Deserialize(byte[] payload)
  {
    var buffer = new PostgresProtocolStream(payload);

    FieldsCount = buffer.ReadInt16();

    RowFieldDescriptions = new List<RowFieldDescription>();

    for (var i = 0; i < FieldsCount; i++)
    {
      var rowFieldDescription = new RowFieldDescription
      {
        FieldName = buffer.ReadString(),
        TableOid = buffer.ReadInt32(),
        RowAttributeId = buffer.ReadInt16(),
        FieldTypeOid = buffer.ReadInt32(),
        DataTypeSize = buffer.ReadInt16(),
        TypeModifier = buffer.ReadInt32(),
        FormatCode = buffer.ReadInt16()
      };

      RowFieldDescriptions.Add(rowFieldDescription);
    }
  }
}
