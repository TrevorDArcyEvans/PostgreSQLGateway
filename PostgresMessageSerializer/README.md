# PostgresMessageSerializer

![.NET Core](https://github.com/kbth/toybox/workflows/.NET%20Core/badge.svg)

This library allows you to (de-)serialize bytes of [PostgreSQL](https://www.postgresql.org/) protocol messages.

## Usage

### Serializing

```cs
var message = new QueryMessage();
message.Query = "SELECT * FROM table1";

var bytes = serializer.Serialize(message); // Byte sequence conforming to PostgreSQL protocol
```

### Deserializing

```cs
var bytes = new [] { (byte)'Z', (byte)'I' };

var message = serializer.Deserialize(bytes); // PostgreSQL message (ReadyForQueryMessage)
```

There is also a Deserialize method that takes a stream as argument.

## Support

For PoC purposes only, this supports only simple queries, extended queries, and messages used around them. This does not support replication or copying.