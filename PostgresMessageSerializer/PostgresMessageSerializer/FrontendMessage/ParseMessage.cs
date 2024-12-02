using System.Collections.Generic;

namespace PostgresMessageSerializer
{
    public class ParseMessage : FrontendMessage
    {
        public static byte MessageTypeId = (byte)'P';

        public string PreparedStatementName { get; set; } = string.Empty;

        public string Query { get; set; } = string.Empty;

        public short ParameterDataTypeOidsCount { get { return (short)ParameterDataTypeOids.Count; } }

        public IList<int> ParameterDataTypeOids { get; set; } = new List<int>();

        public override byte[] Serialize()
        {
            var buffer = new PostgresProtocolStream();

            buffer.Write(PreparedStatementName);
            buffer.Write(Query);
            buffer.Write(ParameterDataTypeOidsCount);

            for (var i = 0; i < ParameterDataTypeOidsCount; i++)
            {
                buffer.Write(ParameterDataTypeOids[i]);
            }

            return buffer.ToArray();
        }
    }
}