#nullable enable

using System;
using System.Windows.Forms;
using ExTools.Connection.Managers;
using ExTools.SqlConsole;
using ExTools.SqlConsole.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Office.Tools;

namespace ExTools
{
    public sealed partial class ThisAddIn
    {
        private static IServiceProvider _services = null!;

        public static T GetService<T>() where T : class => _services.GetRequiredService<T>();

        public CustomTaskPane AddTaskPane(UserControl userControl, string title) =>
            CustomTaskPanes.Add(userControl, title);

        public bool RemoveTaskPane(CustomTaskPane taskPane) =>
            CustomTaskPanes.Remove(taskPane);

        private static void ConfigureServices() => _services = new ServiceCollection()
            .AddSingleton<ConsoleViewModelProvider>()
            .AddSingleton<ConfigurationsProvider>()
            .AddSingleton<ConnectionsManager>()
            .AddTransient<ConsoleViewModel>()
            .BuildServiceProvider();

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
        }

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            ConfigureServices();

            ConnectionsManager connectionsManager = GetService<ConnectionsManager>();
            connectionsManager.Initialize();
        }
    }
}