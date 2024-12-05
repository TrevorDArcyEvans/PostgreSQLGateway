namespace PostgresMessageSerializer.Tests;

using System;
using FluentAssertions;
using Xunit;

public class QueryMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new QueryMessage
    {
      Query = Guid.NewGuid().ToString()
    };

    // act
    var data = sut.Serialize();
    var result = new QueryMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
