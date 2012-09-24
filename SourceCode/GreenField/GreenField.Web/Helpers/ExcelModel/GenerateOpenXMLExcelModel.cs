﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DAL;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using System.Drawing;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;

namespace GreenField.Web.ExcelModel
{
    public static class GenerateOpenXMLExcelModel
    {

        /// <summary>
        /// Method to Generate Byte[] for the Excel File
        /// </summary>
        /// <param name="financialData"></param>
        /// <param name="consensusData"></param>
        /// <returns></returns>
        public static byte[] GenerateExcel(List<FinancialStatementData> financialData, List<ModelConsensusEstimatesData> consensusData, string currencyReuters, string currencyConsensus)
        {
            try
            {
                string fileName = GetFileName();

                // Create a spreadsheet document by supplying the filepath.
                // By default, AutoSave = true, Editable = true, and Type = xlsx.
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                    Create(fileName, SpreadsheetDocumentType.Workbook))
                {
                    UInt32 sheetId;
                    // Add a WorkbookPart to the document.
                    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();

                    // Add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>("rId3");
                    worksheetPart.Worksheet = new Worksheet();

                    // add styles to sheet
                    WorkbookStylesPart wbsp = workbookpart.AddNewPart<WorkbookStylesPart>();                   
                    wbsp.Stylesheet = CreateStylesheet();
                    wbsp.Stylesheet.Save();

                    // Add Sheets to the Workbook.
                    spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                    sheetId = 1;
                    spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                            {
                                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                                SheetId = sheetId,
                                Name = "Reuters Repoted"
                            });
                    GenerateReutersHeaders(worksheetPart, financialData, currencyReuters);
                    InsertValuesInWorksheet(worksheetPart, financialData);
                    workbookpart.Workbook.Save();

                    // Add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPartConsensus = workbookpart.AddNewPart<WorksheetPart>("rId2");
                    worksheetPartConsensus.Worksheet = new Worksheet();
                    worksheetPartConsensus.Worksheet.Save();
                    sheetId = 2;

                    spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPartConsensus),
                        SheetId = sheetId,
                        Name = "Consensus Data"
                    });
                    GenerateConsensusHeaders(worksheetPartConsensus, consensusData, currencyConsensus);
                    InsertConsensusDataInWorksheet(worksheetPartConsensus, consensusData);

                    workbookpart.Workbook.Save();
                    workbookpart.Workbook.Save();

                    // Close the document.
                    spreadsheetDocument.Close();
                    return GetBytsForFile(fileName);
                }
            }
            catch (Exception ex)
            {
                //ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worksheetPart"></param>
        /// <param name="consensusData"></param>
        private static void InsertConsensusDataInWorksheet(WorksheetPart worksheetPart, List<ModelConsensusEstimatesData> consensusData)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            //SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            var row = new Row { RowIndex = 2 };
            sheetData.Append(row);
            int rowIndex = 2;
            List<string> dataDescriptors = consensusData.Select(a => a.ESTIMATE_DESC).Distinct().ToList();
            var maxRowCount = dataDescriptors.Count + 2;
            List<int> financialPeriodYears = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).Distinct().ToList();

            int firstYear = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).FirstOrDefault();
            int lastYear = consensusData.Select(a => a.PERIOD_YEAR).OrderByDescending(a => a).FirstOrDefault();
            int numberOfYears = lastYear - firstYear;

            while (row.RowIndex < maxRowCount)
            {
                foreach (string item in dataDescriptors)
                {
                    firstYear = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).FirstOrDefault();
                    var cell = CreateNumberCell(Convert.ToDecimal(consensusData.Where(a => a.ESTIMATE_DESC == item).Select(a => a.ESTIMATE_ID).FirstOrDefault()));
                    row.InsertAt(cell, 0);
                    cell = new Cell();
                    cell = CreateTextCell(Convert.ToString(consensusData.Where(a => a.ESTIMATE_DESC == item).Select(a => a.ESTIMATE_DESC).FirstOrDefault()));
                    row.InsertAt(cell, 1);
                    for (int i = 0; i <= numberOfYears * 5; i = i + 5)
                    {
                        cell = new Cell();
                        cell = CreateNumberCell(consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q1").
                            Select(a => a.AMOUNT).FirstOrDefault());
                        row.InsertAt(cell, i + 2);


                        cell = new Cell();
                        cell = CreateNumberCell(consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q2")
                            .Select(a => a.AMOUNT).FirstOrDefault());
                        row.InsertAt(cell, i + 3);


                        cell = new Cell();
                        cell = CreateNumberCell(consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q3")
                            .Select(a => a.AMOUNT).FirstOrDefault());
                        row.InsertAt(cell, i + 4);


                        cell = new Cell();
                        cell = CreateNumberCell(consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q4")
                            .Select(a => a.AMOUNT).FirstOrDefault());
                        row.InsertAt(cell, i + 5);


                        cell = new Cell();
                        cell = CreateNumberCell(consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "A")
                            .Select(a => a.AMOUNT).FirstOrDefault());
                        row.InsertAt(cell, i + 6);


                        firstYear++;
                    }

                    ++rowIndex;
                    row = new Row { RowIndex = Convert.ToUInt32(rowIndex) };
                    sheetData.Append(row);
                }
            }
        }

        /// <summary>
        /// Insert Financial Values in WorkSheet
        /// </summary>
        /// <param name="worksheetPart"></param>
        /// <param name="financialData"></param>
        private static void InsertValuesInWorksheet(WorksheetPart worksheetPart, List<FinancialStatementData> financialData)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            //SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            var row = new Row { RowIndex = 2 };
            sheetData.Append(row);
            List<int> dataIds = financialData.Select(a => Convert.ToInt32((a.DataId))).Distinct().ToList();
            var maxRowCount = dataIds.Count + 2;
            int rowIndex = 2;
            List<int> financialPeriodYears = financialData.Select(a => a.PeriodYear).OrderBy(a => a).Distinct().ToList();

            int firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
            int lastYear = financialData.Select(a => a.PeriodYear).OrderByDescending(a => a).FirstOrDefault();

            int numberOfYears = lastYear - firstYear;
            while (row.RowIndex < maxRowCount)
            {
                foreach (int item in dataIds)
                {
                    firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
                    var cell = CreateNumberCell(Convert.ToDecimal(financialData.Where(a => a.DataId == item).Select(a => a.DataId).FirstOrDefault()));
                    row.InsertAt(cell, 0);
                    cell = new Cell();
                    cell = CreateTextCell(financialData.Where(a => a.DataId == item).Select(a => a.Description).FirstOrDefault());
                    row.InsertAt(cell, 1);

                    for (int i = 0; i <= numberOfYears * 5; i = i + 5)
                    {
                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.DataId == item && a.PeriodType.Trim() == "Q1").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 2);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.DataId == item && a.PeriodType.Trim() == "Q2").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 3);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.DataId == item && a.PeriodType.Trim() == "Q3").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 4);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.DataId == item && a.PeriodType.Trim() == "Q4").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 5);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.DataId == item && a.PeriodType.Trim() == "A").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 6);
                        firstYear++;
                    }
                    ++rowIndex;
                    row = new Row { RowIndex = Convert.ToUInt32(rowIndex) };
                    sheetData.Append(row);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worksheetPart"></param>
        /// <param name="financialData"></param>
        private static void GenerateReutersHeaders(WorksheetPart worksheetPart, List<FinancialStatementData> financialData, string currency)
        {
            var worksheet = worksheetPart.Worksheet;
            SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            SheetFormatProperties sheetFormatProperties1;
            var row = new Row { RowIndex = 1 };
            sheetData.Append(row);
            int firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
            int lastYear = financialData.Select(a => a.PeriodYear).OrderByDescending(a => a).FirstOrDefault();
            int numberOfYears = lastYear - firstYear;

            var maxLength = financialData.Max(s => s.Description.Length);

            var maxLengthStr = financialData.FirstOrDefault(s => s.Description.Length == maxLength).Description;

            DoubleValue maxWidth = GetColumnWidth(maxLengthStr);

            Column firstColumn = new Column() { Min = 2U, Max = 2U, Width = maxWidth };

            Columns sheetColumns = new Columns();
            Column mergeColumns;

            sheetColumns.Append(firstColumn);

            var cell = new Cell();

            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateHeaderCell("Data In " + Convert.ToString(currency) + " (Millions)");
            row.InsertAt(cell, 1);

            for (int i = 0; i <= numberOfYears * 5; i = i + 5)
            {
                Columns column = new DocumentFormat.OpenXml.Spreadsheet.Columns();
                mergeColumns = new DocumentFormat.OpenXml.Spreadsheet.Column();

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q1");
                row.InsertAt(cell, i + 2);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q2");
                row.InsertAt(cell, i + 3);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q3");
                row.InsertAt(cell, i + 4);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q4");
                row.InsertAt(cell, i + 5);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " A");
                row.InsertAt(cell, i + 6);
                firstYear++;

                mergeColumns = new Column() { Min = Convert.ToUInt32(i + 3), Max = Convert.ToUInt32(i + 6), CustomWidth = true, OutlineLevel = 1, Hidden = false };
                sheetColumns.Append(mergeColumns);
            }
            sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, OutlineLevelColumn = 1, DyDescent = 0.25D };
            worksheet.Append(sheetFormatProperties1);
            worksheet.Append(sheetColumns);
            worksheet.Append(sheetData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worksheetPart"></param>
        /// <param name="consensusData"></param>
        private static void GenerateConsensusHeaders(WorksheetPart worksheetPart, List<ModelConsensusEstimatesData> consensusData, string currency)
        {
            var worksheet = worksheetPart.Worksheet;
            //var sheetData = worksheet.GetFirstChild<SheetData>();
            SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            var row = new Row { RowIndex = 1 };
            SheetFormatProperties sheetFormatProperties1;
            sheetData.Append(row);

            Columns sheetColumns = new Columns();
            Column mergeColumns;

            var maxLength = consensusData.Max(s => s.ESTIMATE_DESC.Length);

            var maxLengthStr = consensusData.FirstOrDefault(s => s.ESTIMATE_DESC.Length == maxLength).ESTIMATE_DESC;

            DoubleValue maxWidth = GetColumnWidth(maxLengthStr);

            Column firstColumn = new Column() { Min = 2U, Max = 2U, Width = maxWidth };

            sheetColumns.Append(firstColumn);

            int firstYear = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).FirstOrDefault();
            int lastYear = consensusData.Select(a => a.PERIOD_YEAR).OrderByDescending(a => a).FirstOrDefault();
            int numberOfYears = lastYear - firstYear;

            var cell = new Cell();

            cell = CreateHeaderCell("Data Id");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateHeaderCell("Data in " + Convert.ToString(currency) + " (Millions)");
            row.InsertAt(cell, 1);

            for (int i = 0; i <= numberOfYears * 5; i = i + 5)
            {
                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q1");
                row.InsertAt(cell, i + 2);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q2");
                row.InsertAt(cell, i + 3);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q3");
                row.InsertAt(cell, i + 4);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q4");
                row.InsertAt(cell, i + 5);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " A");
                row.InsertAt(cell, i + 6);
                firstYear++;
                mergeColumns = new Column() { Min = Convert.ToUInt32(i + 3), Max = Convert.ToUInt32(i + 6), CustomWidth = true, OutlineLevel = 1, Hidden = false };
                sheetColumns.Append(mergeColumns);
            }
            sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, OutlineLevelColumn = 1, DyDescent = 0.25D };
            worksheet.Append(sheetFormatProperties1);
            worksheet.Append(sheetColumns);
            worksheet.Append(sheetData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        private static Cell CreateTextCell(string cellValue)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(cellValue);
            return cell;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        private static Cell CreateNumberCell(Decimal? cellValue)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.Number;
            cell.CellValue = new CellValue(Convert.ToString(cellValue));
            return cell;
        }

        /// <summary>
        /// Creating Header Cell
        /// </summary>
        /// <param name="cellValue">value to fill in teh cell</param>
        /// <returns></returns>
        private static Cell CreateHeaderCell(String cellValue)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(cellValue);
            cell.StyleIndex = (UInt32Value)2U;
            return cell;
        }

        /// <summary>
        /// Get Width of the column for longest string
        /// </summary>
        /// <param name="sILT">longest String</param>
        /// <returns>width</returns>
        private static DoubleValue GetColumnWidth(string sILT)
        {
            double fSimpleWidth = 0.0f;
            double fWidthOfZero = 0.0f;
            double fDigitWidth = 0.0f;
            double fMaxDigitWidth = 0.0f;
            double fTruncWidth = 0.0f;

            System.Drawing.Font drawfont = new System.Drawing.Font("Calibri", 11);
            // I just need a Graphics object. Any reasonable bitmap size will do.
            Graphics g = Graphics.FromImage(new Bitmap(200, 200));
            fWidthOfZero = (double)g.MeasureString("0", drawfont).Width;
            fSimpleWidth = (double)g.MeasureString(sILT, drawfont).Width;
            fSimpleWidth = fSimpleWidth / fWidthOfZero;

            for (int i = 0; i < 10; ++i)
            {
                fDigitWidth = (double)g.MeasureString(i.ToString(), drawfont).Width;
                if (fDigitWidth > fMaxDigitWidth)
                {
                    fMaxDigitWidth = fDigitWidth;
                }
            }
            g.Dispose();

            // Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}] / {Maximum Digit Width} * 256) / 256
            fTruncWidth = Math.Truncate((sILT.ToCharArray().Count() * fMaxDigitWidth + 5.0) / fMaxDigitWidth * 256.0) / 256.0;

            return fTruncWidth;

        }

        /// <summary>
        /// StyleSheet for the excel File
        /// </summary>
        /// <returns></returns>
        private static Stylesheet CreateStylesheet()
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)1U, KnownFonts = true };

            DocumentFormat.OpenXml.Spreadsheet.Font font1 = new DocumentFormat.OpenXml.Spreadsheet.Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            DocumentFormat.OpenXml.Spreadsheet.Color color1 = new DocumentFormat.OpenXml.Spreadsheet.Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            DocumentFormat.OpenXml.Spreadsheet.Font font2 = new DocumentFormat.OpenXml.Spreadsheet.Font();
            FontSize fontSize2 = new FontSize() { Val = 11D };
            DocumentFormat.OpenXml.Spreadsheet.Color color2 = new DocumentFormat.OpenXml.Spreadsheet.Color() { Theme = (UInt32Value)1U };
            FontName fontName2 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };
            Bold bold2 = new Bold();

            font2.Append(fontSize2);
            font2.Append(color2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);
            font2.Append(fontScheme2);
            font2.Append(bold2);

            fonts1.Append(font1);
            fonts1.Append(font2);

            Fills fills1 = new Fills() { Count = (UInt32Value)5U };

            // FillId = 0
            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };
            fill1.Append(patternFill1);

            // FillId = 1
            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };
            fill2.Append(patternFill2);

            // FillId = 2,RED
            Fill fill3 = new Fill();
            PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FFFF0000" };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill3.Append(foregroundColor1);
            patternFill3.Append(backgroundColor1);
            fill3.Append(patternFill3);

            // FillId = 3,BLUE
            Fill fill4 = new Fill();
            PatternFill patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor2 = new ForegroundColor() { Theme = (UInt32Value)3U, Tint = 0.79998168889431442D };// { Rgb = "FF0070C0" };
            BackgroundColor backgroundColor2 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill4.Append(foregroundColor2);
            patternFill4.Append(backgroundColor2);
            fill4.Append(patternFill4);

            // FillId = 4,YELLO
            Fill fill5 = new Fill();
            PatternFill patternFill5 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor3 = new ForegroundColor() { Rgb = "FFFFFF00" };
            BackgroundColor backgroundColor3 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill5.Append(foregroundColor3);
            patternFill5.Append(backgroundColor3);
            fill5.Append(patternFill5);

            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);
            fills1.Append(fill5);

            Borders borders1 = new Borders() { Count = (UInt32Value)1U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            borders1.Append(border1);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)4U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)4U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);
            cellFormats1.Append(cellFormat5);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleMedium9" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append(slicerStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);
            return stylesheet1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetFileName()
        {
            string fileName = Path.GetTempPath() + Guid.NewGuid() + "_Model.xlsx";
            return fileName;
        }

        /// <summary>
        /// Generate byte-Array from Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static byte[] GetBytsForFile(string filePath)
        {
            try
            {
                FileStream fileStream;
                byte[] fileByte;
                using (fileStream = File.OpenRead(filePath))
                {
                    fileByte = new byte[fileStream.Length];
                    fileStream.Read(fileByte, 0, Convert.ToInt32(fileStream.Length));
                }
                return fileByte;
            }
            catch (Exception ex)
            {
                //ExceptionTrace.LogException(ex);
                return null;
            }
        }

    }
}