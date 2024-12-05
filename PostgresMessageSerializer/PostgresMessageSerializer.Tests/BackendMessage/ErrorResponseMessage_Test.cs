using FluentAssertions;
using Xunit;

namespace PostgresMessageSerializer.Tests;

public class ErrorResponseMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new ErrorResponseMessage();
    for (var i = 1; i <= 3; i++)
    {
      var msgField = new MessageField();
      msgField.Id = (byte)(2 * i);
      msgField.Value = (7 * i).ToString();

      sut.Fields.Add(msgField);
    }

    // act
    var data = sut.Serialize();
    var result = new ErrorResponseMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
