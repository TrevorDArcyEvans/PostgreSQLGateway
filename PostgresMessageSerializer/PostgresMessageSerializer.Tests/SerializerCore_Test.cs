namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class SerializerCore_Test
{
  [Theory]
  [InlineData(21)]
  [InlineData(0)]
  [InlineData(-21)]
  public void Serialize_Deserialize_Int32_roundtrip(int number)
  {
    // arrange
    var data = SerializerCore.Serialize(number);

    // act
    var copy = SerializerCore.DeserializeInt32(data);

    // assert
    copy.Should().Be(number);
  }

  [Theory]
  [InlineData(21)]
  [InlineData(0)]
  [InlineData(-21)]
  public void Serialize_Deserialize_Int16_roundtrip(short number)
  {
    // arrange
    var data = SerializerCore.Serialize(number);

    // act
    var copy = SerializerCore.DeserializeInt16(data);

    // assert
    copy.Should().Be(number);
  }

  [Theory]
  [InlineData("")]
  [InlineData("abcdefghijklmnopqrstuv")]
  [InlineData("12345678")]
  [InlineData("!£$%^&*()_+-={}|[]")]
  public void Serialize_Deserialize_String_roundtrip(string msg)
  {
    // arrange
    var data = SerializerCore.Serialize(msg);

    // act
    var copy = SerializerCore.DeserializeString(data);

    // assert
    copy.Should().Be(msg);
  }
}
