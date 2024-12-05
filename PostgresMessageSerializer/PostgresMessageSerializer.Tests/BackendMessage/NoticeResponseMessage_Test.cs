namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class NoticeResponseMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new NoticeResponseMessage();
    for (var i = 1; i <= 3; i++)
    {
      var msgField = new MessageField();
      msgField.Id = (byte)(2 * i);
      msgField.Value = (7 * i).ToString();

      sut.Fields.Add(msgField);
    }

    // act
    var data = sut.Serialize();
    var result = new NoticeResponseMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
