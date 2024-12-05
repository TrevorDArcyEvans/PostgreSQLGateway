namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class SyncMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new SyncMessage();

    // act
    var data = sut.Serialize();
    var result = new SyncMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
