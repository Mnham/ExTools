using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Text;
using System.Threading.Tasks;

namespace ExTools.Dialog
{
    public sealed class DialogHostViewModel : ObservableObject
    {
        private TaskCompletionSource<bool> _completionSource;
        private bool _isMessageOpen;
        private bool _isProgressOpen;
        private bool _isShowing;
        private string _message;
        private string _title;

        public RelayCommand CloseCommand { get; }

        public bool IsMessageOpen
        {
            get => _isMessageOpen;
            private set => SetProperty(ref _isMessageOpen, value);
        }

        public bool IsProgressOpen
        {
            get => _isProgressOpen;
            private set => SetProperty(ref _isProgressOpen, value);
        }

        public bool IsShowing
        {
            get => _isShowing;
            private set => SetProperty(ref _isShowing, value);
        }

        public string Message
        {
            get => _message;
            private set => SetProperty(ref _message, value);
        }

        public string Title
        {
            get => _title;
            private set => SetProperty(ref _title, value);
        }

        public DialogHostViewModel() => CloseCommand = new RelayCommand(Close);

        public void Close()
        {
            IsShowing = false;

            _completionSource?.SetResult(true);
            _completionSource = null;
        }

        public async Task ShowExceptionAsync(Exception exception) => await ShowMessageAsync("ОШИБКА", GetExceptionMessage(exception));

        public async Task ShowMessageAsync(string title, string message)
        {
            IsProgressOpen = false;

            Title = title;
            Message = message;
            IsMessageOpen = true;

            IsShowing = true;

            _completionSource = new TaskCompletionSource<bool>();
            await _completionSource.Task;
        }

        public void ShowProgress()
        {
            IsMessageOpen = false;

            IsProgressOpen = true;

            IsShowing = true;
        }

        private static string GetExceptionMessage(Exception exception)
        {
            StringBuilder message = new();

            while (exception is not null)
            {
                message.AppendLine(exception.Message);
                exception = exception.InnerException;
            }

            return message.ToString();
        }
    }
}