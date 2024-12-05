namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class ExecuteMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new ExecuteMessage();

    // act
    var data = sut.Serialize();
    var result = new ExecuteMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
