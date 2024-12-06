namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class RowDescriptionMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new RowDescriptionMessage();
    for (var i = 1; i <= 3; i++)
    {
      var rowFieldDescr = new RowFieldDescription
      {
        FieldName = $"Field{i}",
        TableOid = i,
        RowAttributeId = (short) (2 * i),
        FieldTypeOid = 3 * i,
        DataTypeSize = (short) (4 * i),
        TypeModifier = 5 * i,
        FormatCode = (short) (6 * i)
      };
      sut.RowFieldDescriptions.Add(rowFieldDescr);
    }

    // act
    var data = sut.Serialize();
    var result = new RowDescriptionMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
