using ExTools.Connection.Models;
using ExTools.Dialog;
using ExTools.SqlConsole.Models;
using ExTools.SqlConsole.QueryExecutor;
using ExTools.SqlConsole.Services;

using System;
using System.Threading.Tasks;

namespace ExTools.Connection.Helpers
{
    public static class ConnectionHelper
    {
        private static readonly ConfigurationsProvider _configurationsProvider = ThisAddIn.GetService<ConfigurationsProvider>();
        private static readonly ConsoleViewModelProvider _consoleViewModelProvider = ThisAddIn.GetService<ConsoleViewModelProvider>();

        public static async Task<bool> TestConnectionAsync(ConnectionOptions сonnectionOptions)
        {
            DialogHostViewModel dialogHost = _consoleViewModelProvider.GetConsoleViewModel().DialogHost;

            try
            {
                dialogHost.ShowProgress();

                ConsoleConfiguration config = _configurationsProvider.GetConfiguration(сonnectionOptions.ConnectionType);
                QueryExecutorBase queryExecutor = config.QueryExecutorPool.Get();
                await queryExecutor.TestConnectionAsync(сonnectionOptions);
                config.QueryExecutorPool.Return(queryExecutor);

                dialogHost.Close();

                return true;
            }
            catch (Exception ex)
            {
                await dialogHost.ShowExceptionAsync(ex);
            }

            return false;
        }
    }
}