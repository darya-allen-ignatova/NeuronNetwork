using NeuronNetworkTest.Data;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace DataAccessApp
{
    internal static class ExcelDataProvider
    {
        public static List<InputData> GetData(string path, int worksheetIndex)
        {
            List<InputData> data = new List<InputData>();
            FileInfo existingFile = new FileInfo(path);
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[worksheetIndex];
                int colCount = worksheet.Dimension.End.Column;
                int rowCount = worksheet.Dimension.End.Row;
                for (int row = 2; row <= rowCount; row++)
                {
                    data.Add(new InputData(worksheet.Cells[row, 1].Value.ToString(), worksheet.Cells[row, 2].Value.ToString()));
                }
            }

            return data;
        }
    }
}
