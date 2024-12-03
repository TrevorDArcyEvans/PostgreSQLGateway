using CommandLine;

namespace PostgreSQLGateway;

internal sealed class Options
{
  [Value(index: 0, Required = false, Default = 5432, HelpText = "Port on which to listen to")]
  public int Port { get; set; } = 5432;
}
