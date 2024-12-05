namespace PostgresMessageSerializer;

using System.Collections.Generic;

public class RowDescriptionMessage : BackendMessage
{
  public static byte MessageTypeId = (byte) 'T';

  public short FieldsCount { get; set; }

  public IList<RowFieldDescription> RowFieldDescriptions { get; } = new List<RowFieldDescription>();

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(FieldsCount);

    for (var i = 0; i < FieldsCount; i++)
    {
      var rowFieldDescr = RowFieldDescriptions[i];

      buffer.Write(rowFieldDescr.FieldName);
      buffer.Write(rowFieldDescr.TableOid);
      buffer.Write(rowFieldDescr.RowAttributeId);
      buffer.Write(rowFieldDescr.FieldTypeOid);
      buffer.Write(rowFieldDescr.DataTypeSize);
      buffer.Write(rowFieldDescr.TypeModifier);
      buffer.Write(rowFieldDescr.FormatCode);
    }

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    FieldsCount = buffer.ReadInt16();

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
