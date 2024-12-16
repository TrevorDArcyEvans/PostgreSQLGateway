namespace PostgresMessageSerializer;

public class CommandCompleteMessage : BackendMessage
{
  public override byte MessageTypeId => (byte)'C';

  /// <summary>
  /// The command tag. This is usually a single word that identifies which SQL command was completed.
  ///
  /// For an INSERT command, the tag is INSERT oid rows, where rows is the number of rows inserted.
  /// oid used to be the object ID of the inserted row if rows was 1 and the target table had OIDs,
  /// but OIDs system columns are not supported anymore; therefore oid is always 0.
  ///
  /// For a DELETE command, the tag is DELETE rows where rows is the number of rows deleted.
  ///
  /// For an UPDATE command, the tag is UPDATE rows where rows is the number of rows updated.
  ///
  /// For a MERGE command, the tag is MERGE rows where rows is the number of rows inserted, updated,
  /// or deleted.
  ///
  /// For a SELECT or CREATE TABLE AS command, the tag is SELECT rows where rows is the number
  /// of rows retrieved.
  ///
  /// For a MOVE command, the tag is MOVE rows where rows is the number of rows the cursor's
  /// position has been changed by.
  ///
  /// For a FETCH command, the tag is FETCH rows where rows is the number of rows that have been
  /// retrieved from the cursor.
  ///
  /// For a COPY command, the tag is COPY rows where rows is the number of rows copied.
  /// (Note: the row count appears only in PostgreSQL 8.2 and later.)
  /// </summary>
  public string CommandTag { get; set; } = string.Empty;

  public CommandCompleteMessage() :
    this(string.Empty)
  {
  }

  public CommandCompleteMessage(string cmdTag)
  {
    CommandTag = cmdTag;
  }

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(CommandTag);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    CommandTag = buffer.ReadString();
  }
}
