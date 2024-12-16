using System;

namespace PostgresMessageSerializer;

public class RowFieldDescription
{
  /// <summary>
  /// The field name.
  /// </summary>
  public string FieldName { get; set; }

  /// <summary>
  /// If the field is a column of a specific table, 
  /// the objectID of the table, else zero.
  /// </summary>
  public int TableOid { get; set; }

  /// <summary>
  /// If the field can be identified as a column of a specific table, 
  /// the attribute number of the column; otherwise zero.
  /// </summary>
  public short RowAttributeId { get; set; }

  /// <summary>
  /// The object ID of the field's data type.
  /// </summary>
  public int FieldTypeOid { get; set; }

  /// <summary>
  /// The data type size. Note that negative values denote variable-width types.
  /// </summary>
  public short DataTypeSize { get; set; }

  /// <summary>
  /// The type modifier(see pg_attribute.atttypmod). 
  /// The meaning of the modifier is type-specific.
  /// </summary>
  public int TypeModifier { get; set; }

  /// <summary>
  /// The format code being used for the field. 
  /// Currently, will be zero (text) or one (binary). 
  /// In a RowDescription returned from the statement variant of Describe, 
  /// the format code is not yet known and will always be zero.
  /// </summary>
  public short FormatCode { get; set; }

  public RowFieldDescription()
  {
  }

  public RowFieldDescription(Type type, string propName)
  {
    FieldName = propName;
    TableOid = 0;
    RowAttributeId = 0;
    FieldTypeOid = (int)ColumnTypeExtensions.TypeToColumnTypeMap[type.GetProperty(propName).PropertyType];
    DataTypeSize = ColumnTypeExtensions.DataTypeSize(ColumnTypeExtensions.TypeToColumnTypeMap[type.GetProperty(propName).PropertyType]);
    TypeModifier = -1;
    FormatCode = 0;
  }
}
