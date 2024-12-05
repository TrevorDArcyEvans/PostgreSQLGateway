namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class ReadyForQueryMessage_Test
{
  [Theory]
  [InlineData(0)]
  [InlineData(11)]
  [InlineData(122)]
  public void Serialize_Deserialize_roundtrip(byte status)
  {
    // arrange
    var sut = new ReadyForQueryMessage
    {
      TransactionStatus = status
    };

    // act
    var data = sut.Serialize();
    var result = new ReadyForQueryMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
