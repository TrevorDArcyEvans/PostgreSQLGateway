using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace PostgresMessageSerializer.ConsoleApp;

public static class Program
{
  /// <summary>
  ///  
  /// </summary>
  /// <param name="host">host address</param>
  /// <param name="port">port number</param>
  public static void Main(string host = "127.0.0.1", string port = "5432")
  {
    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    var endPoint = new IPEndPoint(IPAddress.Parse(host), int.Parse(port));
    socket.Connect(endPoint);

    var stream = new NetworkStream(socket);
    var serializer = new Serializer();

    Console.WriteLine("connected.");

    while (true)
    {
      Console.Write("pg_msg =# ");

      var command = Console.ReadLine();

      if (string.IsNullOrWhiteSpace(command))
      {
        continue;
      }

      if (!Parser.TryParse(command, out (IList<FrontendMessage> sendMessages, int expectCount, bool exit) input))
      {
        Console.WriteLine("parse failed...");
        continue;
      }

      if (input.exit)
      {
        break;
      }

      Send(stream, serializer, input.sendMessages);
      Receive(stream, serializer, input.expectCount);
    }
  }

  private static void Send(Stream stream, Serializer serializer, IList<FrontendMessage> messages)
  {
    var sendBytes = new List<byte>();
    foreach (var message in messages)
    {
      var (name, bodyJson) = Format(message);
      Console.WriteLine("<--    " + name + " " + bodyJson);

      sendBytes.AddRange(Serializer.Serialize(message));
    }

    stream.Write(sendBytes.ToArray());
  }

  private static void Receive(Stream stream, Serializer serializer, int expectCount)
  {
    var receiveCount = 0;

    while (receiveCount < expectCount)
    {
      var message = serializer.Deserialize(stream);

      var (name, bodyJson) = Format(message);
      Console.WriteLine("   --> " + name + " " + bodyJson);

      if (typeof(ReadyForQueryMessage) == message.GetType())
      {
        receiveCount++;
      }
    }
  }

  private static (string name, string bodyJson) Format(object message)
  {
    var messageType = message.GetType();
    var body = JsonSerializer.Serialize(message, messageType);
    var messageName = messageType.Name.Replace("Message", "");

    return (messageName, body);
  }
}