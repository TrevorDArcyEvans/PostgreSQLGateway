namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class ParseMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new ParseMessage();

    // act
    var data = sut.Serialize();
    var result = new ParseMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
