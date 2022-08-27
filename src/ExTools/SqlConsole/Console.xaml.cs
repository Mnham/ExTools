using ExTools.SqlConsole.Services;

using ICSharpCode.AvalonEdit.Search;

using System.Windows.Controls;

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