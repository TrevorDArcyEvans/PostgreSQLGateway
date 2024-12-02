using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostgresMessageSerializer
{
    internal static class SerializerCore
    {
        internal static int DeserializeInt32(byte[] value)
        {
            return BitConverter.ToInt32(value.Reverse().ToArray());
        }

        internal static short DeserializeInt16(byte[] value)
        {
            return BitConverter.ToInt16(value.Reverse().ToArray());
        }

        internal static string DeserializeString(byte[] value)
        {
            return Encoding.UTF8.GetString(value.ToArray());
        }

        internal static byte[] Serialize(string value)
        {
            var bytes = new List<byte>();

            bytes.AddRange(Encoding.UTF8.GetBytes(value));
            bytes.Add(0);

            return bytes.ToArray();
        }

        internal static byte[] Serialize(int value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        internal static byte[] Serialize(short value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        internal static byte[] Serialize(byte value)
        {
            return new[] { value };
        }
    }
}