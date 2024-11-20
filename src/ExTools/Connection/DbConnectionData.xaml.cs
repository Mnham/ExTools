#nullable enable

using System.Windows.Controls;
using ExTools.Infrastructure;

namespace ExTools.Connection
{
    public partial class DbConnectionData : UserControl
    {
        public DbConnectionData() => InitializeComponent();

        public void SetPassword()
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                return;
            }

            string securePassword = StringCipher.Encrypt(PasswordBox.Password);
            PasswordBox.Password = string.Empty;

            var vm = (ConnectionEditorViewModel)DataContext;
            vm.SetSecurePassword(securePassword);
        }
    }
}