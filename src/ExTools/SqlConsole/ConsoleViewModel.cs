using ExTools.Connection;
using ExTools.Connection.Managers;
using ExTools.Connection.Models;
using ExTools.Dialog;
using ExTools.Infrastructure;
using ExTools.SqlConsole.Models;
using ExTools.SqlConsole.QueryExecutor;
using ExTools.SqlConsole.Services;

using ICSharpCode.AvalonEdit.Highlighting;

using Microsoft.Office.Interop.Excel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using DataTable = System.Data.DataTable;

namespace ExTools.SqlConsole
{
    public sealed class ConsoleViewModel : ObservableObject
    {
        private const string FILE_FILTER = "Extl files (*.extl)|*.extl";
        private readonly ConfigurationsProvider _configurationsProvider;
        private ConsoleConfiguration _configuration;
        private ConnectionOptions _connectionOptions;
        private RequestData _requestData = new();
        private long _requestExecutionMilliseconds;
        private FileInfo _requestFile;

        public string AccentColor => _configuration?.AccentColor;

        public ConnectionEditorViewModel ConnectionEditor { get; }

        public ConnectionsManager ConnectionsManager { get; }

        public string ConnectionType => _connectionOptions is null ? null : $"[{_connectionOptions.ConnectionType}]";

        public RelayCommand ConnectToActiveWorkbookCommand { get; }

        public RelayCommand CopyScriptForVbaCommand { get; }

        public RelayCommand CreateNewRequestCommand { get; }

        public string DatabaseName => _connectionOptions is null ? null : $"<{_connectionOptions.DataSource}{_connectionOptions.Database}>";

        public DialogHostViewModel DialogHost { get; } = new();

        public RelayCommand OpenRequestCommand { get; }

        public long RequestExecutionMilliseconds
        {
            get => _requestExecutionMilliseconds;
            set => SetProperty(ref _requestExecutionMilliseconds, value);
        }

        public string RequestFileName => RequestFile?.Name ?? "Новый файл";

        public string ResultSheetName
        {
            get => _requestData.ResultSheetName;
            set
            {
                _requestData.ResultSheetName = value;

                RunScriptCommand.NotifyCanExecuteChanged();
                RunSelectedScriptCommand.NotifyCanExecuteChanged();
            }
        }

        public AsyncRelayCommand RunScriptCommand { get; }

        public AsyncRelayCommand RunSelectedScriptCommand { get; }

        public RelayCommand SaveAsRequestCommand { get; }

        public RelayCommand SaveRequestCommand { get; }

        public string Script
        {
            get => _requestData.Script;
            set => _requestData.Script = value;
        }

        public string SelectedScript { get; set; }

        public IHighlightingDefinition SyntaxHighlighting => _configuration?.SyntaxHighlighting;

        private FileInfo RequestFile
        {
            get => _requestFile;
            set
            {
                _requestFile = value;
                OnPropertyChanged(nameof(RequestFileName));
            }
        }

        public ConsoleViewModel(ConfigurationsProvider configurationsProvider, ConnectionsManager connectionsManager)
        {
            _configurationsProvider = configurationsProvider;
            ConnectionsManager = connectionsManager;

            ConnectionEditor = new ConnectionEditorViewModel(configurationsProvider, connectionsManager);

            ConnectToActiveWorkbookCommand = new RelayCommand(ConnectToActiveWorkbook);
            CopyScriptForVbaCommand = new RelayCommand(CopyScriptForVba);
            CreateNewRequestCommand = new RelayCommand(CreateNewRequest);
            OpenRequestCommand = new RelayCommand(OpenRequest);
            RunScriptCommand = new AsyncRelayCommand(() => RunScriptAsync(Script), CanExecuteQuery);
            RunSelectedScriptCommand = new AsyncRelayCommand(() => RunScriptAsync(SelectedScript), CanExecuteQuery);
            SaveAsRequestCommand = new RelayCommand(SaveAsRequest);
            SaveRequestCommand = new RelayCommand(SaveRequest);
        }

        public void UpdateConnectionOptions(ConnectionOptions connectionOptions)
        {
            _connectionOptions = connectionOptions;
            UpdateConfiguration(connectionOptions.ConnectionType);

            OnPropertyChanged(nameof(ConnectionType));
            OnPropertyChanged(nameof(DatabaseName));

            RunScriptCommand.NotifyCanExecuteChanged();
            RunSelectedScriptCommand.NotifyCanExecuteChanged();
        }

        private bool CanExecuteQuery() =>
            !string.IsNullOrWhiteSpace(ResultSheetName)
            && _connectionOptions is not null;

        private void ConnectToActiveWorkbook()
        {
            Workbook activeWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            activeWorkbook?.SaveWorkbook();

            ConnectionOptions connectionOptions = new()
            {
                ConnectionType = Connection.Models.ConnectionType.Excel,
                DataSource = activeWorkbook?.FullName
            };

            UpdateConnectionOptions(connectionOptions);
        }

        private void CopyScriptForVba()
        {
            IEnumerable<string> lines = Script
                .Replace("\"", "\"\"")
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => $"\"{l} \" & _");

            Clipboard.SetText(string.Join("\n", lines));
        }

        private void CreateNewRequest()
        {
            RequestFile = null;
            UpdateRequestData(new RequestData());
        }

        private void OpenRequest()
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Открыть файл",
                Filter = FILE_FILTER
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RequestFile = new FileInfo(openFileDialog.FileName);
                RequestData requestData = SerializeExtensions.DeserializeJson<RequestData>(RequestFile);
                UpdateRequestData(requestData);
            }
        }

        private async Task RunScriptAsync(string script)
        {
            Workbook activeWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            if (activeWorkbook is null)
            {
                return;
            }

            try
            {
                DialogHost.ShowProgress();

                QueryExecutorBase queryExecutor = _configuration.QueryExecutorPool.Get();

                DataTable dataTable = await queryExecutor.ExecuteAsync(script, _connectionOptions);

                Worksheet resultSheet = activeWorkbook.GetWorksheet(ResultSheetName);
                resultSheet.Cells.Clear();
                resultSheet.FillWorksheet(dataTable);
                resultSheet.UsedRange.Cells.Borders.LineStyle = XlLineStyle.xlContinuous;
                resultSheet.Activate();

                RequestExecutionMilliseconds = queryExecutor.RequestExecutionMilliseconds;

                _configuration.QueryExecutorPool.Return(queryExecutor);

                DialogHost.Close();
            }
            catch (Exception ex)
            {
                await DialogHost.ShowExceptionAsync(ex);
            }
        }

        private void Save() => SerializeExtensions.SerializeJson(_requestData, RequestFile);

        private void SaveAsRequest()
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Сохранить файл",
                Filter = FILE_FILTER,
                OverwritePrompt = false
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                RequestFile = new(saveFileDialog.FileName);
                Save();
            }
        }

        private void SaveRequest()
        {
            if (RequestFile is null)
            {
                SaveAsRequest();
            }
            else
            {
                Save();
            }
        }

        private void UpdateConfiguration(ConnectionType type)
        {
            _configuration = _configurationsProvider.GetConfiguration(type);

            OnPropertyChanged(nameof(AccentColor));
            OnPropertyChanged(nameof(SyntaxHighlighting));
        }

        private void UpdateRequestData(RequestData requestData)
        {
            _requestData = requestData;

            OnPropertyChanged(nameof(ResultSheetName));
            OnPropertyChanged(nameof(Script));

            RunScriptCommand.NotifyCanExecuteChanged();
            RunSelectedScriptCommand.NotifyCanExecuteChanged();
        }
    }
}