namespace PostgresMessageSerializer.Tests;

using System;
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
    var sut = new CommandCompleteMessage(commTag);
    sut.CommandTag = commTag;

    // act
    var data = sut.Serialize();
    var result = new CommandCompleteMessage(Guid.NewGuid().ToString());
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
