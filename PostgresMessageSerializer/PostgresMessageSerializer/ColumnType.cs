namespace PostgresMessageSerializer;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

/// <summary>
/// stolen from:
///     https://jdbc.postgresql.org/documentation/publicapi/constant-values.html
/// </summary>
public enum ColumnType
{
  BIT = 1560,
  BIT_ARRAY = 1561,
  BOOL = 16,
  BOOL_ARRAY = 1000,
  BOX = 603,
  BPCHAR = 1042,
  BPCHAR_ARRAY = 1014,
  BYTEA = 17,
  BYTEA_ARRAY = 1001,
  CHAR = 18,
  CHAR_ARRAY = 1002,
  CIDR = 650,
  CIRCLE = 718,
  DATE = 1082,
  DATE_ARRAY = 1182,
  FLOAT4 = 700,
  FLOAT4_ARRAY = 1021,
  FLOAT8 = 701,
  FLOAT8_ARRAY = 1022,
  INET = 869,
  INT2 = 21,
  INT2_ARRAY = 1005,
  INT4 = 23,
  INT4_ARRAY = 1007,
  INT8 = 20,
  INT8_ARRAY = 1016,
  INTERVAL = 1186,
  INTERVAL_ARRAY = 1187,
  JSON = 114,
  JSON_ARRAY = 199,
  JSONB = 3802,
  JSONB_ARRAY = 3807,
  LINE = 628,
  LSEG = 601,
  MACADDR = 829,
  MACADDR8 = 774,
  MONEY = 790,
  MONEY_ARRAY = 791,
  NAME = 19,
  NAME_ARRAY = 1003,
  NUMERIC = 1700,
  NUMERIC_ARRAY = 1231,
  OID = 26,
  OID_ARRAY = 1028,
  PATH = 602,
  POINT = 600,
  POINT_ARRAY = 1017,
  POLYGON = 604,
  REF_CURSOR = 1790,
  REF_CURSOR_ARRAY = 2201,
  TEXT = 25,
  TEXT_ARRAY = 1009,
  TIME = 1083,
  TIME_ARRAY = 1183,
  TIMESTAMP = 1114,
  TIMESTAMP_ARRAY = 1115,
  TIMESTAMPTZ = 1184,
  TIMESTAMPTZ_ARRAY = 1185,
  TIMETZ = 1266,
  TIMETZ_ARRAY = 1270,
  TSQUERY = 3615,
  TSVECTOR = 3614,
  UNSPECIFIED = 0,
  UUID = 2950,
  UUID_ARRAY = 2951,
  VARBIT = 1562,
  VARBIT_ARRAY = 1563,
  VARCHAR = 1043,
  VARCHAR_ARRAY = 1015,
  VOID = 2278,
  XML = 142,
  XML_ARRAY = 143,

  // some esoteric ones
  REG_PROC = 24,
  XID = 28,
  OIDVECTOR = 30,
  PG_NODE_TREE = 194,
  UNKNOWN = 705,
  ACL_ITEM = 1034,
}

public static class ColumnTypeExtensions
{
  public static short DataTypeSize(this ColumnType colType)
    => colType switch
    {
      ColumnType.BOOL => 1,
      ColumnType.CHAR => 1,
      ColumnType.CHAR_ARRAY => -1,
      ColumnType.DATE => 4,
      ColumnType.FLOAT4 => 4,
      ColumnType.FLOAT8 => 8,
      ColumnType.INT2 => 2,
      ColumnType.INT4 => 4,
      ColumnType.INT8 => 8,
      ColumnType.INT8_ARRAY => -1,
      ColumnType.NAME => 64,
      ColumnType.NUMERIC => -1,
      ColumnType.OID => 4,
      ColumnType.OID_ARRAY => -1,
      ColumnType.TEXT => -1,
      ColumnType.TEXT_ARRAY => -1,
      ColumnType.TIME => 8,
      ColumnType.TIMESTAMP => 8,
      ColumnType.UNSPECIFIED => -1,
      ColumnType.UUID => 16,
      ColumnType.VARCHAR => -1,

      // some esoteric ones
      ColumnType.REG_PROC => 4,
      ColumnType.XID => 4,
      ColumnType.OIDVECTOR => -1,
      ColumnType.PG_NODE_TREE => -1,
      ColumnType.UNKNOWN => -1,
      ColumnType.ACL_ITEM => -1,
      _ => -1
    };

  // Stolen from:
  //      https://stackoverflow.com/questions/845458/postgresql-and-c-sharp-datatypes
  //      https://www.npgsql.org/doc/types/basic.html
  /*
      Postgresql  NpgsqlDbType System.DbType Enum .NET System Type
      ----------  ------------ ------------------ ----------------
      int8        Bigint       Int64              Int64 aka long
      bool        Boolean      Boolean            Boolean aka bool
      bytea       Bytea        Binary             Byte[]
      date        Date         Date               DateTime
      float8      Double       Double             Double aka double
      int4        Integer      Int32              Int32 aka int
      money       Money        Decimal            Decimal aka decimal
      numeric     Numeric      Decimal            Decimal aka decimal
      float4      Real         Single             Single aka float
      int2        Smallint     Int16              Int16 aka short
      text        Text         String             String aka string
      time        Time         Time               DateTime
      timetz      Time         Time               DateTime
      timestamp   Timestamp    DateTime           DateTime
      timestamptz TimestampTZ  DateTime           DateTime
      interval    Interval     Object             TimeSpan
      varchar     Varchar      String             String aka string
      inet        Inet         Object             IPAddress
      bit         Bit          Boolean            Boolean aka bool
      uuid        Uuid         Guid               Guid
      array       Array        Object             Array

      int8        ???          ???                uint aka UInt32
      int2        ???          ???                byte aka Byte
  */
  public static ReadOnlyDictionary<Type, ColumnType> TypeToColumnTypeMap = new(new Dictionary<Type, ColumnType>
  {
    { typeof(bool), ColumnType.BOOL },
    { typeof(char), ColumnType.CHAR },
    { typeof(DateTime), ColumnType.DATE },
    { typeof(double), ColumnType.FLOAT8 },
    { typeof(byte), ColumnType.INT2 },
    { typeof(sbyte), ColumnType.INT2 },
    { typeof(short), ColumnType.INT2 },
    { typeof(int), ColumnType.INT4 },
    { typeof(ulong), ColumnType.INT8 },
    { typeof(long), ColumnType.INT8 },
    { typeof(uint), ColumnType.INT8 },
    { typeof(ulong[]), ColumnType.INT8_ARRAY },
    { typeof(float), ColumnType.FLOAT4 },
    { typeof(decimal), ColumnType.NUMERIC },
    { typeof(Array), ColumnType.TEXT_ARRAY },
    { typeof(string[]), ColumnType.TEXT_ARRAY },
    { typeof(object), ColumnType.UNKNOWN },
    { typeof(Guid), ColumnType.UUID },
    { typeof(string), ColumnType.VARCHAR },
    { typeof(IPAddress), ColumnType.VARCHAR },
  });
}
