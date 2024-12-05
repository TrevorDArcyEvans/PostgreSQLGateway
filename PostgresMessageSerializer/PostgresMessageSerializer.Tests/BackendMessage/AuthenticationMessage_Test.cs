namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class AuthenticationMessage_Test
{
  [Theory]
  [InlineData(21)]
  [InlineData(0)]
  [InlineData(-21)]
  public void Serialize_Deserialize_roundtrip(int authRes)
  {
    // arrange
    var sut = new AuthenticationMessage();
    sut.AuthResult = authRes;

    // act
    var data = sut.Serialize();
    var result = new AuthenticationMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
