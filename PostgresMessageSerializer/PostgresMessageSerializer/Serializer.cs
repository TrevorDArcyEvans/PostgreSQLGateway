namespace PostgresMessageSerializer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class Serializer
{
  private static Dictionary<byte, Type> _frontEndMsgTypeIdToTypeMap = new();
  private static Dictionary<byte, Type> _backEndMsgTypeIdToTypeMap = new();

  static Serializer()
  {
    var customTypes = Assembly.GetExecutingAssembly().GetTypes().ToList();
    foreach (var customType in customTypes)
    {
      var field = customType.GetField("MessageTypeId");
      if (field == null)
      {
        continue;
      }

      var messageTypeId = (byte)field.GetValue(null);

      if (customType.BaseType == typeof(FrontendMessage))
      {
        _frontEndMsgTypeIdToTypeMap.Add(messageTypeId, customType);
      }

      if (customType.BaseType == typeof(BackendMessage))
      {
        _backEndMsgTypeIdToTypeMap.Add(messageTypeId, customType);
      }
    }
  }

  public static byte[] Serialize(Message message)
  {
    var messageTypeId = message.GetType().GetField("MessageTypeId")?.GetValue(message);
    var payload = message.Serialize();

    var buffer = new List<byte>();

    if (messageTypeId != null)
    {
      buffer.Add((byte)messageTypeId);
    }

    buffer.AddRange(SerializerCore.Serialize(payload.Length + sizeof(int)));
    buffer.AddRange(payload);

    return buffer.ToArray();
  }

  public static Message Deserialize(byte[] bytes)
  {
    var stream = new MemoryStream(bytes);
    return Deserialize(stream);
  }

  private static Message Deserialize(Stream stream)
  {
    var messageTypeId = (byte)stream.ReadByte();

    // prioritise front end messages since that is what we will be receiving
    if (!_frontEndMsgTypeIdToTypeMap.TryGetValue(messageTypeId, out var messageType))
    {
      if (!_backEndMsgTypeIdToTypeMap.TryGetValue(messageTypeId, out messageType))
      {
        throw new ArgumentException($"invalid message type: {messageTypeId}", nameof(stream));
      }
    }

    var payloadSizeField = new byte[sizeof(int)];
    stream.Read(payloadSizeField, 0, sizeof(int));

    var payloadSize = SerializerCore.DeserializeInt32(payloadSizeField) - sizeof(int);
    if (payloadSize < 0)
    {
      throw new ArgumentException("invalid payload size", nameof(stream));
    }

    var payload = new byte[payloadSize];
    var readSize = stream.Read(payload, 0, payloadSize);

    if (readSize != payloadSize)
    {
      throw new ArgumentException("invalid payload size", nameof(stream));
    }

    var message = (Message)Activator.CreateInstance(messageType);
    message.Deserialize(payload);
    return message;
  }
}
