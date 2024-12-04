# PostgreSQL Gateway


## TODO

* support front end messages
  * SSL request
  * GSS API encryption request
* support SSL
  * https://learn.microsoft.com/en-us/dotnet/api/system.net.security.sslstream?view=net-9.0&redirectedfrom=MSDN 
* support various front ends
  * [psql](https://www.postgresguide.com/utilities/psql/)
  * [Npgsql](https://www.npgsql.org/)
  * [DBeaver](https://dbeaver.io/)
* support PostgreSQL protocols
  * simple
  * extended
* multi-threaded server
* routing of queries
  * probably via plugins aka extension point/s


## Further information
<details>

* [PostgreSQL wire protocol](https://www.postgresql.org/docs/current/protocol.html)
* [PSQL wire protocol in go](https://github.com/jeroenrinzema/psql-wire)
* [Building a PostgreSQL Wire Protocol Server using Vanilla, Modern Java 21](https://gavinray97.github.io/blog/postgres-wire-protocol-jdk-21)
* [Demo implementations of the Postgres Wire Protocol](https://github.com/rgwood/odbc/tree/main)
* [Postgres on the wire - A look at the PostgreSQL wire protocol](https://beta.pgcon.org/2014/schedule/attachments/330_postgres-for-the-wire.pdf)
  * [local copy (pdf)](docs/330_postgres-for-the-wire.pdf)
* [PostgresMessageSerializer](https://github.com/kbth/PostgresMessageSerializer/tree/master)
* [PostgreSQL System Catalogs](https://www.postgresql.org/docs/current/catalogs.html)
* [Slon - A modern high performance PostgreSQL protocol implementation for .NET](https://github.com/NinoFloris/Slon)
* [RaveDB PostgreSQL integration](https://github.com/ravendb/ravendb/tree/v5.4/src/Raven.Server/Integrations/PostgreSQL)
  * [Integrations - Would like PostgreSQL integration to support DBeaver](https://github.com/ravendb/ravendb/issues/17037)
  * [GNU Affero General Public License](https://www.gnu.org/licenses/agpl-3.0.en.html)
  * [GNU Affero General Public License on Wikipedia](https://en.wikipedia.org/wiki/GNU_Affero_General_Public_License)
  * [What is the AGPL license? Top questions answered](https://snyk.io/learn/agpl-license/)
  * [Open Source Software Licenses 101: The AGPL License](https://fossa.com/blog/open-source-software-licenses-101-agpl-license/)
* [ClickHouse PostgreSQL driver (C++)](https://github.com/ClickHouse/ClickHouse/tree/master/src/Databases/PostgreSQL)

</details>
