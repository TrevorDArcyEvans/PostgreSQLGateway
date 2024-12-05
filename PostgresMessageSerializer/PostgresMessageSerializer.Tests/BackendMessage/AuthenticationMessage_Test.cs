namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class AuthenticationMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new AuthenticationMessage();
    sut.AuthResult = 21;

    // act
    var data = sut.Serialize();
    var result = new AuthenticationMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
