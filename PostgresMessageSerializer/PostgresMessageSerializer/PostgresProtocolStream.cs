﻿namespace PostgresMessageSerializer;

using System.Collections.Generic;
using System.IO;

public class PostgresProtocolStream : MemoryStream
{
  public PostgresProtocolStream() : base()
  {
  }

  public PostgresProtocolStream(byte[] bytes) : base(bytes)
  {
  }

  public short ReadInt16()
  {
    var buffer = new byte[sizeof(short)];
    base.Read(buffer, 0, sizeof(short));

    return SerializerCore.DeserializeInt16(buffer);
  }

  public int ReadInt32()
  {
    var buffer = new byte[sizeof(int)];
    base.Read(buffer, 0, sizeof(int));

    return SerializerCore.DeserializeInt32(buffer);
  }

  public string ReadString()
  {
    var buffer = new List<byte>();

    // look for null string terminator
    int b;
    while ((b = base.ReadByte()) != 0)
    {
      buffer.Add((byte)b);
    }

    return SerializerCore.DeserializeString(buffer.ToArray());
  }

  public void Write(byte value)
  {
    var buffer = SerializerCore.Serialize(value);
    base.Write(buffer, 0, buffer.Length);
  }

  public void Write(short value)
  {
    var buffer = SerializerCore.Serialize(value);
    base.Write(buffer, 0, buffer.Length);
  }

  public void Write(int value)
  {
    var buffer = SerializerCore.Serialize(value);
    base.Write(buffer, 0, buffer.Length);
  }

  public void Write(string value)
  {
    var buffer = SerializerCore.Serialize(value);

    // add null string terminator
    var bytes = new List<byte>(buffer) { 0 };

    base.Write(bytes.ToArray(), 0, bytes.Count);
  }
}
