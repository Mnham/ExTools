#nullable enable

using System.Data.Common;
using ExTools.Connection.Models;
using ExTools.Infrastructure;
using Npgsql;

namespace ExTools.SqlConsole.QueryExecutor
{
    public sealed class PostgreSQLQueryExecutor : QueryExecutorBase
    {
        public override QueryExecutorBase CreateQueryExecutor() => new PostgreSQLQueryExecutor();

        protected override DbConnection CreateConnection(ConnectionOptions options)
        {
            NpgsqlConnectionStringBuilder builder = new()
            {
                Host = options.Host,
                Port = options.Port,
                Username = options.User,
                Password = StringCipher.Decrypt(options.SecurePassword),
                Database = options.Database
            };

            return new NpgsqlConnection(builder.ToString());
        }
    }
}