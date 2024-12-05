using FluentAssertions;
using Xunit;

namespace PostgresMessageSerializer.Tests;

public class BindMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new BindMessage();

    // act
    var data = sut.Serialize();
    var result = new BindMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}