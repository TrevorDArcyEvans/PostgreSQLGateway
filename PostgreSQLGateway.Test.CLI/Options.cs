namespace PostgreSQLGateway.Test.CLI;

using CommandLine;

internal sealed class Options
{
  [Value(index: 0, Required = true, HelpText = "User name")]
  public string User { get; set; }

  [Value(index: 1, Required = true, HelpText = "Password")]
  public string Password { get; set; }

  [Value(index: 2, Required = true, HelpText = "Database")]
  public string Database { get; set; }
}
