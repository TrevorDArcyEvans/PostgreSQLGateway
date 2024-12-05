using FluentAssertions;
using Xunit;

namespace PostgresMessageSerializer.Tests;

public class DescribeMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new DescribeMessage();

    // act
    var data = sut.Serialize();
    var result = new DescribeMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}