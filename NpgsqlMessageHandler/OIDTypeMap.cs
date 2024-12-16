namespace NpgsqlMessageHandler;

using System.Globalization;
using CsvHelper.Configuration;

public class OIDTypeMap : ClassMap<OIDType>
{
  public OIDTypeMap()
  {
    AutoMap(CultureInfo.InvariantCulture);
    Map(m => m.elemtypoid).Default(0);
  }
}
