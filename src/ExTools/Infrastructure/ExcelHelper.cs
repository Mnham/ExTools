using Microsoft.Office.Interop.Excel;

using System;
using System.Data;
using System.IO;
using System.Linq;

using DataTable = System.Data.DataTable;

namespace ExTools.Infrastructure
{
    public static class ExcelHelper
    {
        public static void FillWorksheet(this Worksheet worksheet, DataTable dataTable)
        {
            string columnName = GetExcelColumnName(dataTable.Columns.Count);
            string[] headers = dataTable.Columns
                .Cast<DataColumn>()
                .Select(c => c.Caption)
                .ToArray();

            worksheet.Range[$"A1:{columnName}1"].Value2 = headers;

            if (dataTable.Rows.Count > 0)
            {
                worksheet.Range[$"A2:{columnName}{dataTable.Rows.Count + 1}"].Value = GetData(dataTable);
            }
        }

        public static Worksheet GetWorksheet(this Workbook workbook, string name)
        {
            Worksheet result;
            try
            {
                result = (Worksheet)workbook.Sheets[name];
            }
            catch
            {
                result = (Worksheet)workbook.Sheets.Add();
                result.Name = name;
            }

            return result;
        }

        public static void SaveWorkbook(this Workbook workbook)
        {
            if (!File.Exists(workbook.FullName))
            {
                Globals.ThisAddIn.Application.DisplayAlerts = false;
                workbook.Save();
                Globals.ThisAddIn.Application.DisplayAlerts = true;
            }
        }

        private static object[,] GetData(DataTable dataTable)
        {
            object[,] array = new object[dataTable.Rows.Count, dataTable.Columns.Count];
            for (int r = 0; r < dataTable.Rows.Count; r++)
            {
                for (int c = 0; c < dataTable.Columns.Count; c++)
                {
                    array[r, c] = dataTable.Rows[r][c];
                }
            }

            return array;
        }

        private static string GetExcelColumnName(int columnNumber)
        {
            string columnName = string.Empty;
            int modulo;
            while (columnNumber > 0)
            {
                modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }
    }
}