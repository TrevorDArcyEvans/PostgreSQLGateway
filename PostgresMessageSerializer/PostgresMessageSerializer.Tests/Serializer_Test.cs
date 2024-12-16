namespace PostgresMessageSerializer.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

public class Serializer_Test
{
  [Theory]
  [MemberData(nameof(FrontEndMessageTypes))]
  public void DeserializeFrontEnd_roundtrip(Type msgType)
  {
    // arrange
    var original = (Message) Activator.CreateInstance(msgType);
    var data = Serializer.Serialize(original);

    // act
    var copy = Serializer.DeserializeFrontEnd(data);

    // assert
    original.Should().BeEquivalentTo(copy, options => options.RespectingRuntimeTypes());
  }

  [Theory]
  [MemberData(nameof(BackEndMessageTypes))]
  public void DeserializeBackEnd_roundtrip(Type msgType)
  {
    // arrange
    var original = (Message) Activator.CreateInstance(msgType);
    var data = Serializer.Serialize(original);

    // act
    var copy = Serializer.DeserializeBackEnd(data);

    // assert
    original.Should().BeEquivalentTo(copy, options => options.RespectingRuntimeTypes());
  }

  public static IEnumerable<object[]> FrontEndMessageTypes()
  {
    var msgTypes = typeof(FrontendMessage)
      .Assembly
      .GetTypes()
      .Where(x => x.BaseType == typeof(FrontendMessage));
    foreach (var msgType in msgTypes)
    {
      yield return [msgType];
    }
  }

  public static IEnumerable<object[]> BackEndMessageTypes()
  {
    var msgTypes = typeof(BackendMessage)
      .Assembly
      .GetTypes()
      .Where(x => x.BaseType == typeof(BackendMessage) && !x.IsAbstract);
    foreach (var msgType in msgTypes)
    {
      yield return [msgType];
    }
  }
}
