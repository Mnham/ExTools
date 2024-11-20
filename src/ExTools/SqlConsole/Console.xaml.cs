#nullable enable

using System.Windows.Controls;
using ExTools.SqlConsole.Services;
using ICSharpCode.AvalonEdit.Search;

namespace ExTools.SqlConsole
{
    public sealed partial class Console : UserControl
    {
        public Console()
        {
            ConsoleViewModelProvider viewModelProvider = ThisAddIn.GetService<ConsoleViewModelProvider>();
            DataContext = viewModelProvider.GetConsoleViewModel();
            InitializeComponent();
            SearchPanel.Install(Editor);
        }
    }
}