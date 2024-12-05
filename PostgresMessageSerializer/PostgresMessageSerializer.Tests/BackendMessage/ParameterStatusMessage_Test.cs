namespace PostgresMessageSerializer.Tests;

using System;
using FluentAssertions;
using Xunit;

public class ParameterStatusMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new ParameterStatusMessage
    {
      Name = Guid.NewGuid().ToString(),
      Value = Guid.NewGuid().ToString()
    };

    // act
    var data = sut.Serialize();
    var result = new ParameterStatusMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
