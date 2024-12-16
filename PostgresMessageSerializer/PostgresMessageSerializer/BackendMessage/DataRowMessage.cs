namespace PostgresMessageSerializer;

using System.Collections.Generic;

public abstract class DataRowMessage : BackendMessage
{
  public override byte MessageTypeId => (byte)'D';

  public short ColumnCount => (short) Rows.Count;
  public IList<RowField> Rows { get; } = new List<RowField>();

  public abstract void Update();

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(ColumnCount);

    for (var i = 0; i < ColumnCount; i++)
    {
      var rowField = Rows[i];
      buffer.Write(rowField.Length);

      for (var j = 0; j < rowField.Length; j++)
      {
        buffer.Write(rowField.Value[j]);
      }
    }

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    var columnCount = buffer.ReadInt16();

    for (var i = 0; i < columnCount; i++)
    {
      var rowField = new RowField();
      var rowFieldLen = buffer.ReadInt32();

      var rowValueBytes = new List<byte>();
      for (var j = 0; j < rowFieldLen; j++)
      {
        rowValueBytes.Add((byte)buffer.ReadByte());
      }

      rowField.Value = rowValueBytes;

      Rows.Add(rowField);
    }
  }
}
