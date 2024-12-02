using System.Collections.Generic;

namespace PostgresMessageSerializer
{
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
                var rowFieldDescription = new RowFieldDescription();
                rowFieldDescription.FieldName = buffer.ReadString();
                rowFieldDescription.TableOid = buffer.ReadInt32();
                rowFieldDescription.RowAttributeId = buffer.ReadInt16();
                rowFieldDescription.FieldTypeOid = buffer.ReadInt32();
                rowFieldDescription.DataTypeSize = buffer.ReadInt16();
                rowFieldDescription.TypeModifier = buffer.ReadInt32();
                rowFieldDescription.FormatCode = buffer.ReadInt16();

                RowFieldDescriptions.Add(rowFieldDescription);
            }
        }
    }

    public class RowFieldDescription
    {
        public string FieldName { get; set; }
        public int TableOid { get; set; }
        public short RowAttributeId { get; set; }
        public int FieldTypeOid { get; set; }
        public short DataTypeSize { get; set; }
        public int TypeModifier { get; set; }
        public short FormatCode { get; set; }
    }
}