using System;
using System.IO;
using Xunit;

namespace PostgresMessageSerializer.Tests;

public class SerializerTest
{
  [Fact]
  public void Deserialize_PostgresProtocolBytes_ToBackendMessage()
  {
    // assert
    var bytes = new byte[]
    {
      MockBackendMessage.MessageTypeId,
      0, 0, 0, 9, // message size (including this property)
      (byte) 't', (byte) 'e', (byte) 's', (byte) 't', 0, // 'test'(encoded utf-8) + 0(end of string)
    };

    var serializer = new Serializer();
    serializer.CustomTypes.Add(typeof(MockBackendMessage));

    // act
    var message = (MockBackendMessage) serializer.Deserialize(new MemoryStream(bytes));

    // assert
    Assert.Equal("test", message.PropertyString);
  }

  [Fact]
  public void Deserialize_InvalidPostgresProtocolBytes_ThrowArgumentException()
  {
    // assert
    var bytes = new byte[]
    {
      MockBackendMessage.MessageTypeId,
      0, 0, 0, 2, // invalid message size
      (byte) 'a'
    };

    var serializer = new Serializer();
    serializer.CustomTypes.Add(typeof(MockBackendMessage));

    // act & assert
    Assert.Throws<ArgumentException>(() => serializer.Deserialize(new MemoryStream(bytes)));
  }

  [Fact]
  public void Serialize_FrontendMessage_ToPostgresProtocolBytes()
  {
    // arrange
    var message = new MockFrontendMessage();
    message.PropertyString = "test";

    var serializer = new Serializer();
    serializer.CustomTypes.Add(typeof(MockFrontendMessage));

    // act
    var serialized = serializer.Serialize(message);

    // assert
    var expect = new byte[]
    {
      MockFrontendMessage.MessageTypeId,
      0, 0, 0, 9, // message size (including itself)
      (byte) 't', (byte) 'e', (byte) 's', (byte) 't', 0, // 'test'(encoded utf-8) + 0(end of string)
    };
    Assert.Equal(expect, serialized);
  }

  class MockFrontendMessage : FrontendMessage
  {
    public static byte MessageTypeId = (byte) '0';

    public string PropertyString { get; set; }

    public override byte[] Serialize()
    {
      var buffer = new PostgresProtocolStream();

      buffer.Write(PropertyString);

      return buffer.ToArray();
    }
  }

  class MockBackendMessage : BackendMessage
  {
    public static byte MessageTypeId = (byte) '0';

    public string PropertyString { get; set; }

    public override void Deserialize(byte[] payload)
    {
      var buffer = new PostgresProtocolStream(payload);

      PropertyString = buffer.ReadString();
    }
  }
}
