using FluentAssertions;
using Xunit;

namespace PostgresMessageSerializer.Tests;

public class CancelRequestMessage_Test
{
  [Theory]
  [InlineData(21, 23)]
  [InlineData(0, -11)]
  [InlineData(-21, 0)]
  [InlineData(-21, 10)]
  public void Serialize_Deserialize_roundtrip(int pid, int key)
  {
    // arrange
    var sut = new CancelRequestMessage
    {
      ProcessId = pid,
      SecretKey = key
    };

    // act
    var data = sut.Serialize();
    var result = new CancelRequestMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
