using Microsoft.Extensions.DependencyInjection;
using Microsoft.Office.Interop.Excel;

using System.Collections.Generic;

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

                viewModel = ThisAddIn.Services.GetService<ConsoleViewModel>();
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