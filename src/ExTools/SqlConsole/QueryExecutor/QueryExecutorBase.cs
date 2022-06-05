using ExTools.Connection.Models;

using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ExTools.SqlConsole.QueryExecutor
{
    public abstract class QueryExecutorBase
    {
        private readonly Stopwatch _stopwatch = new();

        public long RequestExecutionMilliseconds => _stopwatch.ElapsedMilliseconds;

        public abstract QueryExecutorBase CreateQueryExecutor();

        public async Task<DataTable> ExecuteAsync(string commandText, ConnectionOptions connectionOptions) =>
            await Task.Run(async () =>
            {
                _stopwatch.Reset();

                using DbConnection connection = CreateConnection(connectionOptions);
                await connection.OpenAsync();

                using DbCommand command = connection.CreateCommand();
                command.CommandText = commandText;

                _stopwatch.Start();
                using DbDataReader dataReader = await command.ExecuteReaderAsync();
                _stopwatch.Stop();

                DataTable dataTable = new();
                dataTable.Load(dataReader);

                return dataTable;
            });

        public async Task TestConnectionAsync(ConnectionOptions connectionOptions)
        {
            string testSelect = "SELECT 1=1";
            await ExecuteAsync(testSelect, connectionOptions);
        }

        protected abstract DbConnection CreateConnection(ConnectionOptions connectionOptions);
    }
}