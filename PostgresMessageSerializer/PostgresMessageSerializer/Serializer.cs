namespace PostgresMessageSerializer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class Serializer
{
  private static readonly Dictionary<byte, Type> _frontEndMsgTypeIdToTypeMap = new();
  private static readonly Dictionary<byte, Type> _backEndMsgTypeIdToTypeMap = new();

  static Serializer()
  {
    var msgTypes = Assembly
      .GetExecutingAssembly()
      .GetTypes()
      .Where(x =>
        x.BaseType == typeof(FrontendMessage) ||
        x.BaseType == typeof(BackendMessage));
    foreach (var msgType in msgTypes)
    {
      var message = (Message) Activator.CreateInstance(msgType);

      if (msgType.BaseType == typeof(FrontendMessage))
      {
        _frontEndMsgTypeIdToTypeMap.Add(message.MessageTypeId, msgType);
      }

      if (msgType.BaseType == typeof(BackendMessage))
      {
        _backEndMsgTypeIdToTypeMap.Add(message.MessageTypeId, msgType);
      }
    }
  }

  public static byte[] Serialize(Message message)
  {
    var buffer = new List<byte>();
    var payload = message.Serialize();

    buffer.Add(message.MessageTypeId);
    buffer.AddRange(SerializerCore.Serialize(payload.Length + sizeof(int)));
    buffer.AddRange(payload);

    return buffer.ToArray();
  }

  public static Message DeserializeFrontEnd(byte[] bytes)
  {
    var stream = new MemoryStream(bytes);
    return Deserialize(stream, _frontEndMsgTypeIdToTypeMap);
  }

  public static Message DeserializeBackEnd(byte[] bytes)
  {
    var stream = new MemoryStream(bytes);
    return Deserialize(stream, _backEndMsgTypeIdToTypeMap);
  }

  private static Message Deserialize(MemoryStream stream, Dictionary<byte, Type> messageTypeMap)
  {
    var messageTypeId = (byte) stream.ReadByte();

    if (!messageTypeMap.TryGetValue(messageTypeId, out var messageType))
    {
      throw new ArgumentException($"invalid message type: {messageTypeId}", nameof(stream));
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

    var message = (Message) Activator.CreateInstance(messageType);
    message.Deserialize(payload);
    return message;
  }
}
