using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PostgresMessageSerializer.ConsoleApp;

internal static class Parser
{
  private static readonly Serializer serializer = new Serializer();

  public static bool TryParse(string input, out (IList<FrontendMessage>, int, bool) output)
  {
    output = (new List<FrontendMessage>(), 1, false);
    try
    {
      output = Parse(input);
      return true;
    }
    catch (Exception ex)
    {
      Console.Write(ex);
      return false;
    }
  }

  public static (IList<FrontendMessage>, int, bool) Parse(string rawInput)
  {
    var json = string.Empty;
    var exit = false;

    var input = rawInput.Trim();
    if (input.StartsWith("\\i"))
    {
      var filePath = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];
      if (!File.Exists(filePath))
      {
        throw new ArgumentException("file not found", nameof(rawInput));
      }

      json = File.ReadAllText(filePath);
    }
    else if (input.StartsWith("\\q"))
    {
      exit = true;
    }
    else
    {
      json = input;
    }


    var parsed = ParseJson(json);
    return (parsed.Item1, parsed.Item2, exit);
  }

  private static (IList<FrontendMessage>, int) ParseJson(string input)
  {
    var sendMessages = new List<FrontendMessage>();

    var inputJson = JsonDocument.Parse(input).RootElement;

    if (!inputJson.TryGetProperty("Messages", out JsonElement messagesElement))
    {
      throw new ArgumentException("must have messages element", nameof(input));
    }

    var waitCount = 1;
    if (inputJson.TryGetProperty("WaitCount", out JsonElement waitCountElement))
    {
      waitCount = waitCountElement.GetInt32();
    }

    foreach (var message in messagesElement.EnumerateArray())
    {
      var messageName = message.GetProperty("Name").GetString() + "Message";
      var messageBody = message.GetProperty("Body").ToString();

      Type messageType = null;
      foreach (var type in serializer.CustomTypes)
      {
        if (type.Name == messageName)
        {
          messageType = type;
          break;
        }
      }

      if (messageType == null)
      {
        throw new NotSupportedException("message name is not exist.");
      }

      var options = new JsonSerializerOptions();
      options.Converters.Add(new ByteConverter());
      options.PropertyNameCaseInsensitive = true;
      var deserializedMessage = (FrontendMessage) JsonSerializer.Deserialize(messageBody, messageType, options);

      sendMessages.Add(deserializedMessage);
    }

    return (sendMessages, waitCount);
  }
}

public class ByteConverter : JsonConverter<byte>
{
  public override byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Encoding.UTF8.GetBytes(reader.GetString())[0];
  }

  public override void Write(Utf8JsonWriter writer, byte value, JsonSerializerOptions options)
  {
    throw new NotImplementedException();
  }
}
