using ExTools.Connection.Models;

using System.Data.Common;
using System.Data.OleDb;

namespace ExTools.SqlConsole.QueryExecutor
{
    public sealed class ExcelQueryExecutor : QueryExecutorBase
    {
        public override QueryExecutorBase CreateQueryExecutor() => new ExcelQueryExecutor();

        protected override DbConnection CreateConnection(ConnectionOptions options) =>
            new OleDbConnection($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={options.DataSource};Extended Properties='Excel 12.0;HDR=YES';");
    }
}