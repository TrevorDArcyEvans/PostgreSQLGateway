namespace PostgresMessageSerializer.Tests;

using System;
using FluentAssertions;
using Xunit;

public class BindMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new BindMessage
    {
      PortalName = Guid.NewGuid().ToString(),
      PreparedStatementName = Guid.NewGuid().ToString()
    };
    sut.ParameterFormatCodes.Add(-1);
    sut.ParameterFormatCodes.Add(0);
    sut.ParameterFormatCodes.Add(21);
    sut.ParameterFormatCodes.Add(64);
    sut.ParameterValues.Add(new Parameter());
    sut.ParameterValues.Add(new Parameter { Value = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9] });
    sut.ParameterValues.Add(new Parameter { Value = [20, 21, 32, 43] });

    // act
    var data = sut.Serialize();
    var result = new BindMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
