namespace PostgresMessageSerializer.Tests;

using System;
using FluentAssertions;
using Xunit;

public class Serializer_Test
{
  [Theory]
  [InlineData(typeof(AuthenticationMessage))]
  [InlineData(typeof(BackendKeyDataMessage))]
  public void Serialize_Deserialize_roundtrip(Type msgType)
  {
    // arrange
    var message1 = (Message)Activator.CreateInstance(msgType);
    var data = Serializer.Serialize(message1);

    // act
    var message2 = (Message)Serializer.Deserialize(data);

    // assert
    message1.Should().BeEquivalentTo(message2, options => options.RespectingRuntimeTypes());
  }
}
