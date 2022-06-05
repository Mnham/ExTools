using ExTools.Connection.Helpers;
using ExTools.Connection.Managers;
using ExTools.Connection.Models;
using ExTools.SqlConsole.Models;
using ExTools.SqlConsole.Services;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;

using System.Threading.Tasks;

namespace ExTools.Connection
{
    public sealed class ConnectionEditorViewModel : ObservableObject
    {
        private readonly ConfigurationsProvider _configurationsProvider;
        private readonly ConnectionsManager _connectionsManager;
        private string _database;
        private string _dataSource;
        private string _host;
        private int _port;
        private string _primaryHueMidColor;
        private string _securePassword;
        private ConnectionOptionsViewModel _selectedConnection;
        private ConnectionType _selectedConnectionType;
        private string _user;

        public AsyncRelayCommand AddConnectionCommand { get; }

        public string Database
        {
            get => _database;
            set => SetProperty(ref _database, value);
        }

        public string DataSource
        {
            get => _dataSource;
            set => SetProperty(ref _dataSource, value);
        }

        public string Host
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string PrimaryHueMidColor
        {
            get => _primaryHueMidColor;
            private set => SetProperty(ref _primaryHueMidColor, value);
        }

        public RelayCommand SaveChangesCommand { get; }

        public ConnectionOptionsViewModel SelectedConnection
        {
            get => _selectedConnection;
            set
            {
                SetProperty(ref _selectedConnection, value);

                UpdateFields(value?.Model);

                if (value is not null)
                {
                    _selectedConnectionType = value.Model.ConnectionType;
                    OnPropertyChanged(nameof(SelectedConnectionTypeIndex));

                    UpdateConfiguration(_selectedConnectionType);
                }

                SaveChangesCommand.NotifyCanExecuteChanged();
            }
        }

        public int SelectedConnectionTypeIndex
        {
            get => (int)_selectedConnectionType;
            set
            {
                _selectedConnectionType = (ConnectionType)value;

                SelectedConnection = null;

                UpdateConfiguration(_selectedConnectionType);
            }
        }

        public RelayCommand SelectExcelFileCommand { get; }

        public string User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public ConnectionEditorViewModel(ConfigurationsProvider configurationsProvider, ConnectionsManager connectionsManager)
        {
            _configurationsProvider = configurationsProvider;
            _connectionsManager = connectionsManager;

            AddConnectionCommand = new AsyncRelayCommand(AddConnectionAsync);
            SaveChangesCommand = new RelayCommand(SaveChanges, () => SelectedConnection is not null);
            SelectExcelFileCommand = new RelayCommand(SelectExcelFile);

            UpdateConfiguration(ConnectionType.Excel);
        }

        public void SetSecurePassword(string securePassword) => _securePassword = securePassword;

        private async Task AddConnectionAsync()
        {
            ConnectionOptions сonnectionOptions = CreateConnectionOptions();

            bool connectionIsAvailable = await ConnectionHelper.TestConnectionAsync(сonnectionOptions);
            if (connectionIsAvailable)
            {
                SelectedConnection = _connectionsManager.AddConnection(сonnectionOptions);
            }
        }

        private ConnectionOptions CreateConnectionOptions() => new()
        {
            ConnectionType = _selectedConnectionType,
            Database = Database,
            DataSource = DataSource,
            Host = Host,
            Port = Port,
            SecurePassword = _securePassword,
            User = User
        };

        private void SaveChanges() => SelectedConnection.Model = CreateConnectionOptions();

        private void SelectExcelFile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Excel Files|*.xlsx;*.xlsm;*.xlsb;*.xls",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DataSource = openFileDialog.FileName;
            }
        }

        private void UpdateConfiguration(ConnectionType type)
        {
            ConsoleConfiguration config = _configurationsProvider.GetConfiguration(type);
            PrimaryHueMidColor = config.AccentColor;
        }

        private void UpdateFields(ConnectionOptions connection)
        {
            Database = connection?.Database;
            DataSource = connection?.DataSource;
            Host = connection?.Host;
            Port = connection?.Port ?? default;
            User = connection?.User;
            _securePassword = null;
        }
    }
}