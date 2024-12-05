using FluentAssertions;
using Xunit;

namespace PostgresMessageSerializer.Tests;

public class CancelRequestMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new CancelRequestMessage();

    // act
    var data = sut.Serialize();
    var result = new CancelRequestMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}