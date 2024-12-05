namespace PostgresMessageSerializer.Tests;

using FluentAssertions;
using Xunit;

public class StartupMessage_Test
{
  [Fact]
  public void Serialize_Deserialize_roundtrip()
  {
    // arrange
    var sut = new StartupMessage();
    sut.Parameters.Add(new StartupParameter("user", "trevorde"));
    sut.Parameters.Add(new StartupParameter("password", "really secret"));
    sut.Parameters.Add(new StartupParameter("database", "all_the_info"));

    // act
    var data = sut.Serialize();
    var result = new StartupMessage();
    result.Deserialize(data);

    // assert
    result.Should().BeEquivalentTo(sut);
  }
}
