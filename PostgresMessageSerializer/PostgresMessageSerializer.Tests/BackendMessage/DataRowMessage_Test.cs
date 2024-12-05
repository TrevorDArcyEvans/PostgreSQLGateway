namespace PostgresMessageSerializer.Tests;

using System.Collections.Generic;
using FluentAssertions;
using Xunit;

public class DataRowMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new DataRowMessage();
    sut.ColumnCount = 3;
    for (var i = 1; i <= sut.ColumnCount; i++)
    {
      var rowField = new RowField();
      rowField.Length = 2 * i;

      var rowValueBytes = new List<byte>(rowField.Length);
      for (var j = 1; j <= rowField.Length; j++)
      {
        rowValueBytes.Add((byte)(7 * j));
      }

      rowField.Value = rowValueBytes;

      sut.Rows.Add(rowField);
    }

    // act
    var data = sut.Serialize();
    var result = new DataRowMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
