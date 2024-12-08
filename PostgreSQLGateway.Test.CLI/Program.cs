namespace PostgreSQLGateway.Test.CLI;

using CommandLine;
using Npgsql;

internal static class Program
{
  public static async Task Main(string[] args)
  {
    var result = await Parser.Default.ParseArguments<Options>(args)
      .WithParsedAsync(Run);
    await result.WithNotParsedAsync(HandleParseError);
  }

  private static async Task Run(Options opt)
  {
    var connectionString = $"Host=localhost;Username={opt.User};Password={opt.Password};Database={opt.Database}";
    await using var dataSource = NpgsqlDataSource.Create(connectionString);
    await using var command = dataSource.CreateCommand("SELECT firstname, lastname FROM customers");
    await using var reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
      Console.WriteLine($"{reader.GetString(0)} {reader.GetString(1)}");
    }
  }

  private static Task HandleParseError(IEnumerable<Error> errs)
  {
    if (errs.IsVersion())
    {
      Console.WriteLine("Version Request");
      return Task.CompletedTask;
    }

    if (errs.IsHelp())
    {
      Console.WriteLine("Help Request");
      return Task.CompletedTask;
    }

    Console.WriteLine("Parser Fail");

    return Task.CompletedTask;
  }
}
