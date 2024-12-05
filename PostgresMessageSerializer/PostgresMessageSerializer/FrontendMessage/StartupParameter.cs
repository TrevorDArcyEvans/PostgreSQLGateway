namespace PostgresMessageSerializer;

public class StartupParameter
{
  public string Name { get; set; }
  public string Value { get; set; }

  public StartupParameter(string name, string value)
  {
    Name = name;
    Value = value;
  }
}
