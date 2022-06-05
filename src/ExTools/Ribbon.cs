using ExTools.SqlConsole;

using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools;
using Microsoft.Office.Tools.Ribbon;

using System.Collections.Generic;

namespace ExTools
{
    public sealed partial class Ribbon
    {
        private readonly Dictionary<Workbook, CustomTaskPane> TaskPanes = new();

        private void ReleaseTaskPane(Workbook workbook)
        {
            workbook.BeforeClose += WorkbookBeforeCloseHandler;

            void WorkbookBeforeCloseHandler(ref bool cancel)
            {
                workbook.BeforeClose -= WorkbookBeforeCloseHandler;

                if (TaskPanes.Count > 1)
                {
                    CustomTaskPane taskPane = TaskPanes[workbook];
                    TaskPanes.Remove(workbook);
                    Globals.ThisAddIn.RemoveTaskPane(taskPane);
                }
            }
        }

        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private void ShowConsoleHandler(object sender, RibbonControlEventArgs e)
        {
            Workbook activeWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;

            if (TaskPanes.TryGetValue(activeWorkbook, out CustomTaskPane taskPane))
            {
                if (taskPane.Visible)
                {
                    taskPane.Visible = false;
                }
                else
                {
                    taskPane.DockPosition = Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionBottom;
                    taskPane.Visible = true;
                }
            }
            else
            {
                ReleaseTaskPane(activeWorkbook);

                taskPane = Globals.ThisAddIn.AddTaskPane(new ConsoleHost(), "Console");
                taskPane.DockPosition = Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionBottom;
                taskPane.Visible = true;

                TaskPanes[activeWorkbook] = taskPane;
            }
        }
    }
}