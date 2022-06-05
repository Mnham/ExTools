using ExTools.Connection.Models;
using ExTools.Infrastructure;

using System.Data.Common;

using Vertica.Data.VerticaClient;

namespace ExTools.SqlConsole.QueryExecutor
{
    public sealed class VerticaQueryExecutor : QueryExecutorBase
    {
        public override QueryExecutorBase CreateQueryExecutor() => new VerticaQueryExecutor();

        protected override DbConnection CreateConnection(ConnectionOptions options)
        {
            VerticaConnectionStringBuilder builder = new()
            {
                Host = options.Host,
                Port = options.Port,
                User = options.User,
                Password = StringCipher.Decrypt(options.SecurePassword),
                Database = options.Database
            };

            return new VerticaConnection(builder.ToString());
        }
    }
}