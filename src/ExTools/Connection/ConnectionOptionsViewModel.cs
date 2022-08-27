using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ExTools.Connection.Helpers;
using ExTools.Connection.Managers;
using ExTools.Connection.Models;
using ExTools.SqlConsole;
using ExTools.SqlConsole.Services;

using MaterialDesignThemes.Wpf;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ExTools.Connection
{
    public sealed class ConnectionOptionsViewModel : ObservableObject
    {
        private readonly ConnectionsManager _connectionsManager;
        private readonly ConsoleViewModelProvider _consoleViewModelProvider;
        private ConnectionOptions _model;
        public string AccentColor { get; set; }

        public AsyncRelayCommand ConnectCommand { get; }

        public string ConnectionType => $"[{Model.ConnectionType}]";

        public ConnectionOptions Model
        {
            get => _model;
            set
            {
                _model = value;

                ModelChanged?.Invoke(this, EventArgs.Empty);

                OnPropertyChanged(nameof(Name));
            }
        }

        public string Name => Model.DataSource + Model.Database;

        public RelayCommand RemoveCommand { get; }

        public event EventHandler ModelChanged;

        public ConnectionOptionsViewModel(
            ConnectionOptions model,
            ConnectionsManager connectionsManager,
            ConsoleViewModelProvider consoleViewModelProvider)
        {
            _model = model;
            _connectionsManager = connectionsManager;
            _consoleViewModelProvider = consoleViewModelProvider;

            ConnectCommand = new AsyncRelayCommand(ConnectAsync);
            RemoveCommand = new RelayCommand(Remove);
        }

        private async Task ConnectAsync()
        {
            IInputElement focusedElement = Keyboard.FocusedElement;

            bool connectionIsAvailable = await ConnectionHelper.TestConnectionAsync(Model);
            if (connectionIsAvailable)
            {
                ConsoleViewModel consoleViewModel = _consoleViewModelProvider.GetConsoleViewModel();
                consoleViewModel.UpdateConnectionOptions(Model);

                DrawerHost.CloseDrawerCommand.Execute(null, focusedElement);
            }
        }

        private void Remove() => _connectionsManager.RemoveConnection(this);
    }
}