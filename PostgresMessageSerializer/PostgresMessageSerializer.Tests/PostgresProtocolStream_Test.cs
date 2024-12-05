namespace PostgresMessageSerializer.Tests;

using Xunit;

public class PostgresProtocolStream_Test
{
  [Fact]
  public void ReadInt16_PostgresProtocolBytes_ToInt16()
  {
    // arrange
    var test = new byte[] { 39, 16 }; // 39 * 256 + 16 = 10000
    var stream = new PostgresProtocolStream(test);

    // act
    var result = stream.ReadInt16();

    // assert
    Assert.Equal(10000, result);
  }

  [Fact]
  public void ReadInt32_PostgresProtocolBytes_ToInt32()
  {
    // arrange
    var test = new byte[] { 0, 0, 39, 16 }; // 39 * 256 + 16 = 10000
    var stream = new PostgresProtocolStream(test);

    // act
    var result = stream.ReadInt32();

    // assert
    Assert.Equal(10000, result);
  }

  [Fact]
  public void ReadString_PostgresProtocolBytes_ToString()
  {
    // arrange
    var bytes = new byte[] { (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0 };
    var stream = new PostgresProtocolStream(bytes);

    // act
    var result = stream.ReadString();

    // assert
    Assert.Equal("test", result);
  }

  [Fact]
  public void Write_String_ToPostgresProtocolBytes()
  {
    // arrange
    var stream = new PostgresProtocolStream();
    stream.Write("test");
    stream.Position = 0;

    // act
    var result = new byte[5];
    stream.Read(result, 0, 5);

    // assert
    var expect = new byte[] { (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0 };
    Assert.Equal(expect, result);
  }

  [Fact]
  public void Write_Int32_ToPostgresProtocolBytes()
  {
    // arrange
    var stream = new PostgresProtocolStream();
    stream.Write(39 * 256 + 16); // 10000
    stream.Position = 0;

    // act
    var result = new byte[sizeof(int)];
    stream.Read(result, 0, sizeof(int));

    // assert
    var expected = new byte[] { 0, 0, 39, 16 };
    Assert.Equal(expected, result);
  }

  [Fact]
  public void Write_Int16_ToPostgresProtocolBytes()
  {
    // arrange
    var stream = new PostgresProtocolStream();
    stream.Write((short)(39 * 256 + 16)); // 10000
    stream.Position = 0;

    // act
    var result = new byte[sizeof(short)];
    stream.Read(result, 0, sizeof(short));

    // assert
    var expected = new byte[] { 39, 16 };
    Assert.Equal(expected, result);
  }
}
