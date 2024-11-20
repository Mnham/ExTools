#nullable enable

using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;

namespace ExTools.SqlConsole.Services
{
    public sealed class ConsoleViewModelProvider
    {
        private readonly Dictionary<Workbook, ConsoleViewModel> _viewModels = new();

        public ConsoleViewModel GetConsoleViewModel()
        {
            Workbook activeWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;

            if (_viewModels.TryGetValue(activeWorkbook, out ConsoleViewModel viewModel))
            {
                return viewModel;
            }
            else
            {
                ReleaseViewModel(activeWorkbook);

                viewModel = ThisAddIn.GetService<ConsoleViewModel>();
                _viewModels[activeWorkbook] = viewModel;

                return viewModel;
            }
        }

        private void ReleaseViewModel(Workbook workbook)
        {
            workbook.BeforeClose += WorkbookBeforeCloseHandler;

            void WorkbookBeforeCloseHandler(ref bool cancel)
            {
                workbook.BeforeClose -= WorkbookBeforeCloseHandler;
                _viewModels.Remove(workbook);
            }
        }
    }
}