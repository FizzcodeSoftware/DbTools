﻿using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public class Sheet
{
    public Sheet(ExcelPackage excelPackage, string name, Color? tabColor = null)
    {
        ExcelWorksheet = excelPackage.Workbook.Worksheets.Add(name);
        if (tabColor.HasValue)
            ExcelWorksheet.TabColor = tabColor.Value;

        LastRow = 1;
        LastColumn = 1;
    }

    public Sheet(ExcelPackage excelPackage, string name)
    {
        ExcelWorksheet = excelPackage.Workbook.Worksheets.Add(name);
        LastRow = 1;
        LastColumn = 1;
    }

    public ExcelWorksheet ExcelWorksheet { get; }

    public int LastRow { get; set; }
    public int LastColumn { get; set; }

    public void SetValue(object? value, Color? backgroundColor = null)
    {
        ExcelWorksheet.SetValue(LastRow, LastColumn, value);
        if (backgroundColor.HasValue)
        {
            ExcelWorksheet.Cells[LastRow, LastColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ExcelWorksheet.Cells[LastRow, LastColumn].Style.Fill.BackgroundColor.SetColor(backgroundColor.Value);
        }
    }

    public void SetLink(string text, string sheetName, Color? backgroundColor = null)
    {
        ExcelWorksheet.Cells[LastRow, LastColumn].Hyperlink = new ExcelHyperLink($"#{sheetName}!A1", text);
        if (backgroundColor.HasValue)
        {
            ExcelWorksheet.Cells[LastRow, LastColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ExcelWorksheet.Cells[LastRow, LastColumn].Style.Fill.BackgroundColor.SetColor(backgroundColor.Value);
        }
    }
}
