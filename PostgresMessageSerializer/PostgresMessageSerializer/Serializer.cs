using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PostgresMessageSerializer
{
    public class Serializer
    {
        public IList<Type> CustomTypes { get; } = Assembly.GetExecutingAssembly().GetTypes().ToList();

        public byte[] Serialize(FrontendMessage message)
        {
            var messageTypeId = message.GetType().GetField("MessageTypeId")?.GetValue(message);
            var payload = message.Serialize();

            var buffer = new List<byte>();

            if (messageTypeId != null) buffer.Add((byte)messageTypeId);
            buffer.AddRange(SerializerCore.Serialize(payload.Length + sizeof(int)));
            buffer.AddRange(payload);

            return buffer.ToArray();
        }

        public BackendMessage Deserialize(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            return Deserialize(stream);
        }

        public BackendMessage Deserialize(Stream stream)
        {
            var messageTypeId = (byte)stream.ReadByte();

            Type messageType = null;

            foreach (var customType in CustomTypes)
            {
                var field = customType.GetField("MessageTypeId");
                if (field == null) continue;

                if ((byte)field.GetValue(null) == messageTypeId)
                {
                    messageType = customType;
                    break;
                }
            }

            if (messageType == null)
                throw new ArgumentException("invalid message type", nameof(stream));

            var payloadSizeField = new byte[sizeof(int)];
            stream.Read(payloadSizeField, 0, sizeof(int));

            var payloadSize = SerializerCore.DeserializeInt32(payloadSizeField) - sizeof(int);
            if (payloadSize < 0)
                throw new ArgumentException("invalid payload size", nameof(stream));

            var payload = new byte[payloadSize];
            var readSize = stream.Read(payload, 0, payloadSize);

            if (readSize != payloadSize)
                throw new ArgumentException("invalid payload size", nameof(stream));

            return Deserialize(payload, messageType);
        }

        public BackendMessage Deserialize(byte[] payload, Type type)
        {
            var message = (BackendMessage)Activator.CreateInstance(type);
            message.Deserialize(payload);
            return message;
        }
    }
}