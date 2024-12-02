using System.Collections.Generic;

namespace PostgresMessageSerializer
{
    public class BindMessage : FrontendMessage
    {
        public static byte MessageTypeId = (byte)'B';

        public string PortalName { get; set; } = string.Empty;

        public string PreparedStatementName { get; set; } = string.Empty;

        public short ParameterFormatCodeCount { get { return (short)ParameterFormatCodes.Count; } }

        public IList<short> ParameterFormatCodes { get; set; } = new List<short>(0);

        public short ParameterValueCount { get { return (short)ParameterValues.Count; } }

        public IList<Parameter> ParameterValues { get; set; } = new List<Parameter>();

        public short RowFormatCode { get; set; } = 0;

        public override byte[] Serialize()
        {
            var buffer = new PostgresProtocolStream();

            buffer.Write(PortalName);
            buffer.Write(PreparedStatementName);
            buffer.Write(ParameterFormatCodeCount);

            for (var i = 0; i < ParameterFormatCodeCount; i++)
            {
                buffer.Write(ParameterFormatCodes[i]);
            }

            buffer.Write(ParameterValueCount);

            for (var i = 0; i < ParameterValueCount; i++)
            {
                buffer.Write(ParameterValues[i].Length);
                buffer.Write(ParameterValues[i].Value);
            }

            buffer.Write(RowFormatCode);

            return buffer.ToArray();
        }
    }


    public class Parameter
    {
        public int Length { get { return Value.Length; } }
        public byte[] Value { get; set; }
    }
}