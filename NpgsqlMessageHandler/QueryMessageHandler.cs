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

    // version
    stream.Write(Serializer.Serialize(VersionInfo.Instance));

    stream.Write(Serializer.Serialize(new CommandCompleteMessage("SELECT 1")));


    // SELECT ns.nspname, t.oid, t.typname, t.typtype, t.typnotnull, t.elemtypoid
    // TODO   OIDType
    stream.Write(Serializer.Serialize(new OIDTypeDescription()));
    stream.Write(Serializer.Serialize(new CommandCompleteMessage("SELECT 1")));


    // -- Load field definitions for (free-standing) composite types
    // TODO   FieldDefinition


    // -- Load enum fields
    // TODO   EnumField


    stream.Write(Serializer.Serialize(new ReadyForQueryMessage()));

    return stopProcessing;
  }
}
