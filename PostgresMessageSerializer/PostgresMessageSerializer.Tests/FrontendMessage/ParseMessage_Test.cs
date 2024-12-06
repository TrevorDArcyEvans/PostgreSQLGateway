namespace PostgresMessageSerializer.Tests;

using System;
using FluentAssertions;
using Xunit;

public class ParseMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new ParseMessage
    {
      PreparedStatementName = Guid.NewGuid().ToString(),
      Query = Guid.NewGuid().ToString()
    };
    sut.ParameterDataTypeOids.Add(-10);
    sut.ParameterDataTypeOids.Add(0);
    sut.ParameterDataTypeOids.Add(11);
    sut.ParameterDataTypeOids.Add(210);

    // act
    var data = sut.Serialize();
    var result = new ParseMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
