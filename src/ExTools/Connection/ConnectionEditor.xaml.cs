using ExTools.Infrastructure;
using ExTools.SqlConsole;

using System.Windows;
using System.Windows.Controls;

namespace ExTools.Connection
{
    public sealed partial class ConnectionEditor : UserControl
    {
        public ConnectionEditor() => InitializeComponent();

        private void SetPassword(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                return;
            }

            string securePassword = StringCipher.Encrypt(PasswordBox.Password);
            PasswordBox.Password = string.Empty;

            ConsoleViewModel console = (ConsoleViewModel)DataContext;
            console.ConnectionEditor.SetSecurePassword(securePassword);
        }
    }
}