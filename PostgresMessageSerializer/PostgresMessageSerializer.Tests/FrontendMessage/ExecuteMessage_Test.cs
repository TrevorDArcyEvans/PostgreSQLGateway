namespace PostgresMessageSerializer.Tests;

using System;
using FluentAssertions;
using Xunit;

public class ExecuteMessage_Test
{
  [Theory]
  [InlineData(21)]
  [InlineData(0)]
  [InlineData(-111)]
  public void Serialize_Deserialize_roundtrip(int limit)
  {
    // arrange
    var sut = new ExecuteMessage
    {
      Limit = limit,
      PortalName = Guid.NewGuid().ToString(),
    };

    // act
    var data = sut.Serialize();
    var result = new ExecuteMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
