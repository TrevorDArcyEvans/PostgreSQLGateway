namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class BackendKeyDataMessage_Test
{
  [Theory]
  [InlineData(21, 22)]
  [InlineData(0, 0)]
  [InlineData(-21, -22)]
  public void Serialize_Deserialize_roundtrip(int pid, int key)
  {
    // arrange
    var sut = new BackendKeyDataMessage();
    sut.ProcessId = pid;
    sut.SecretKey = key;

    // act
    var data = sut.Serialize();
    var result = new BackendKeyDataMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
