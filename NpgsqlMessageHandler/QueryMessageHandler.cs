namespace NpgsqlMessageHandler;

using System.Net.Sockets;
using PostgresMessageSerializer;
using PostgreSQLGateway.Interfaces;

public class QueryMessageHandler : IMessageHandler<QueryMessage>
{
  public bool Process(NetworkStream stream, StartupMessage startupMsg, QueryMessage query)
  {
    if (!query.Query.StartsWith("SELECT version();") ||
        !query.Query.Contains("SELECT ns.nspname, t.oid, t.typname, t.typtype, t.typnotnull, t.elemtypoid") ||
        !query.Query.Contains("-- Arrays have typtype=b - this subquery identifies them by their typreceive and converts their typtype to a") ||
        !query.Query.Contains("-- We first do this for the type (innerest-most subquery), and then for its element type") ||
        !query.Query.Contains("-- This also returns the array element, range subtype and domain base type as elemtypoid"))
    {
      return false;
    }

    var stopProcessing = false;

    // SELECT version();
    stream.Write(Serializer.Serialize(new VersionInfoDescription()));
    stream.Write(Serializer.Serialize(VersionInfo.Instance.Value));
    stream.Write(Serializer.Serialize(new CommandCompleteMessage("SELECT 1")));


    // SELECT ns.nspname, t.oid, t.typname, t.typtype, t.typnotnull, t.elemtypoid
    stream.Write(Serializer.Serialize(new OIDTypeDescription()));
    foreach (var val in OIDType.Instance.Value)
    {
      //stream.Write(Serializer.Serialize(val));
    }

    // TODO   remove once OIDType marshalled correctly
    stream.Write(Serializer.Serialize(OIDType.Instance.Value.First()));

    stream.Write(Serializer.Serialize(new CommandCompleteMessage($"SELECT {OIDType.Instance.Value.Count}")));


    // -- Load field definitions for (free-standing) composite types
    stream.Write(Serializer.Serialize(new FieldDefinitionDescription()));
    foreach (var val in FieldDefinition.Instance.Value)
    {
      stream.Write(Serializer.Serialize(val));
    }
    stream.Write(Serializer.Serialize(new CommandCompleteMessage($"SELECT {FieldDefinition.Instance.Value.Count}")));


    // -- Load enum fields
    stream.Write(Serializer.Serialize(new EnumFieldDescription()));
    foreach (var val in EnumField.Instance.Value)
    {
      stream.Write(Serializer.Serialize(val));
    }
    stream.Write(Serializer.Serialize(new CommandCompleteMessage($"SELECT {EnumField.Instance.Value.Count}")));


    stream.Write(Serializer.Serialize(new ReadyForQueryMessage()));

    return stopProcessing;
  }
}
