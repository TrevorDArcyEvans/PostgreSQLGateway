namespace PostgresMessageSerializer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class SerializerCore
{
  public static int DeserializeInt32(byte[] value)
  {
    return BitConverter.ToInt32(value.Reverse().ToArray());
  }

  public static short DeserializeInt16(byte[] value)
  {
    return BitConverter.ToInt16(value.Reverse().ToArray());
  }

  public static string DeserializeString(byte[] value)
  {
    return Encoding.UTF8.GetString(value.ToArray());
  }

  public static byte[] Serialize(string value)
  {
    return Encoding.UTF8.GetBytes(value);
  }

  public static byte[] Serialize(int value)
  {
    return BitConverter.GetBytes(value).Reverse().ToArray();
  }

  public static byte[] Serialize(short value)
  {
    return BitConverter.GetBytes(value).Reverse().ToArray();
  }

  public static byte[] Serialize(byte value)
  {
    return [value];
  }
}
