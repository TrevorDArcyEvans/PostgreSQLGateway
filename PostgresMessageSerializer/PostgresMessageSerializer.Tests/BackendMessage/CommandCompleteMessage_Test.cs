namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class CommandCompleteMessage_Test
{
  [Theory]
  [InlineData("abcde")]
  [InlineData("")]
  [InlineData("fghi")]
  public void Serialize_Deserialize_roundtrip(string commTag)
  {
    // arrange
    var sut = new CommandCompleteMessage();
    sut.CommandTag = commTag;

    // act
    var data = sut.Serialize();
    var result = new CommandCompleteMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
