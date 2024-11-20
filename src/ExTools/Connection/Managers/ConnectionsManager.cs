#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using ExTools.Connection.Models;
using ExTools.Infrastructure;
using ExTools.SqlConsole.Models;
using ExTools.SqlConsole.Services;

namespace ExTools.Connection.Managers
{
    public sealed class ConnectionsManager(ConfigurationsProvider configurationsProvider, ConsoleViewModelProvider consoleViewModelProvider) : ObservableObject
    {
        private ObservableCollection<ConnectionOptionsViewModel> _connections = [];
        private FileInfo _connectionsFile = null!;

        public IEnumerable<ConnectionOptionsViewModel> Connections => _connections;

        public ConnectionOptionsViewModel AddConnection(ConnectionOptions сonnectionOptions)
        {
            ConnectionOptionsViewModel connection = CreateConnection(сonnectionOptions);
            _connections.Add(connection);

            Save();

            return connection;
        }

        public void Initialize()
        {
            DirectoryInfo userFolder = new(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            string addInName = Assembly.GetExecutingAssembly().GetName().Name;
            DirectoryInfo addInFolder = CreateDirectory(userFolder, addInName);

            _connectionsFile = new FileInfo(Path.Combine(addInFolder.FullName, "connections"));
            if (_connectionsFile.Exists)
            {
                ConnectionOptions[] connections = _connectionsFile.DeserializeJson<ConnectionOptions[]>();
                List<ConnectionOptionsViewModel> viewModels = connections.Select(CreateConnection).ToList();
                _connections = new ObservableCollection<ConnectionOptionsViewModel>(viewModels);
            }
        }

        public void RemoveConnection(ConnectionOptionsViewModel vm)
        {
            vm.ModelChanged -= ConnectionModelChangedHandler;

            _connections.Remove(vm);

            Save();
        }

        private void ConnectionModelChangedHandler(object sender, EventArgs e) => Save();

        private ConnectionOptionsViewModel CreateConnection(ConnectionOptions connectionOptions)
        {
            ConsoleConfiguration config = configurationsProvider.GetConfiguration(connectionOptions.ConnectionType);
            ConnectionOptionsViewModel connection = new(connectionOptions, this, consoleViewModelProvider)
            {
                AccentColor = config.AccentColor
            };

            connection.ModelChanged += ConnectionModelChangedHandler;

            return connection;
        }

        private DirectoryInfo CreateDirectory(DirectoryInfo root, string name)
        {
            DirectoryInfo directory = new(Path.Combine(root.FullName, name));
            if (!directory.Exists)
            {
                directory.Create();
            }

            return directory;
        }

        private void Save()
        {
            ConnectionOptions[] connections = _connections.Select(c => c.Model).ToArray();
            connections.SerializeJson(_connectionsFile);
        }
    }
}