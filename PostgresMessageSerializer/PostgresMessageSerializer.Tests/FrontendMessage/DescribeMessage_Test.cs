namespace PostgresMessageSerializer.Tests;

using System;
using FluentAssertions;
using Xunit;

public class DescribeMessage_Test
{
  [Theory]
  [InlineData(21)]
  [InlineData(0)]
  [InlineData(111)]
  public void Serialize_Deserialize_roundtrip(byte targetType)
  {
    // arrange
    var sut = new DescribeMessage
    {
      TargetType = targetType,
      TargetName = Guid.NewGuid().ToString(),
    };

    // act
    var data = sut.Serialize();
    var result = new DescribeMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
