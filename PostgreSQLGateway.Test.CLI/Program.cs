using Npgsql;

var connectionString = "Host=localhost;Username=mylogin;Password=mypass;Database=mydatabase";
await using var dataSource = NpgsqlDataSource.Create(connectionString);
await using var command = dataSource.CreateCommand("SELECT some_field FROM some_table");
await using var reader = await command.ExecuteReaderAsync();

while (await reader.ReadAsync())
{
  Console.WriteLine(reader.GetString(0));
}
