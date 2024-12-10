namespace PostgreSQLGateway;

public class MessageHandlerConfig
{
  /// <summary>
  /// Fully qualified path to assembly (including extension)
  /// </summary>
  public string Assembly { get; set; }

  /// <summary>
  /// Type of class containing message handler
  /// </summary>
  public string Type { get; set; }

  /// <summary>
  /// Order in which to call message handler.
  /// This must be unique.
  /// </summary>
  public int Order { get; set; }
}
