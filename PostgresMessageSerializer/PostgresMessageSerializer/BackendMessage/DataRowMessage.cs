using System.Collections.Generic;

namespace PostgresMessageSerializer
{
    public class DataRowMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'D';

        public short ColumnCount { get; set; }
        public IList<RowField> Rows { get; set; }

        public override void Deserialize(byte[] payload)
        {
            var buffer = new PostgresProtocolStream(payload);

            ColumnCount = buffer.ReadInt16();

            Rows = new List<RowField>();

            for (var i = 0; i < ColumnCount; i++)
            {
                var rowField = new RowField();
                rowField.Length = buffer.ReadInt32();

                var rowValueBytes = new List<byte>();
                for (var j = 0; j < rowField.Length; j++)
                {
                    rowValueBytes.Add((byte)buffer.ReadByte());
                }

                rowField.Value = rowValueBytes;

                Rows.Add(rowField);
            }
        }
    }

    public class RowField
    {
        public int Length { get; set; }

        public IList<byte> Value { get; set; }
    }
}