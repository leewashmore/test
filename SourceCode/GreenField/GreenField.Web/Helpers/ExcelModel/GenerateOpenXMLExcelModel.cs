﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DAL;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Drawing;

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
                    var cell = CreateTextCell(Convert.ToString(consensusData.Where(a => a.ESTIMATE_DESC == item).Select(a => a.ESTIMATE_ID).FirstOrDefault()));
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
            List<string> dataDescriptors = financialData.Select(a => a.Description).Distinct().ToList();
            var maxRowCount = dataDescriptors.Count + 2;
            int rowIndex = 2;
            List<int> financialPeriodYears = financialData.Select(a => a.PeriodYear).OrderBy(a => a).Distinct().ToList();

            int firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
            int lastYear = financialData.Select(a => a.PeriodYear).OrderByDescending(a => a).FirstOrDefault();

            int numberOfYears = lastYear - firstYear;
            while (row.RowIndex < maxRowCount)
            {
                foreach (string item in dataDescriptors)
                {
                    firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
                    var cell = CreateTextCell(Convert.ToString(financialData.Where(a => a.Description == item).Select(a => a.DataId).FirstOrDefault()));
                    row.InsertAt(cell, 0);
                    cell = new Cell();
                    cell = CreateTextCell(financialData.Where(a => a.Description == item).Select(a => a.Description).FirstOrDefault());
                    row.InsertAt(cell, 1);

                    for (int i = 0; i <= numberOfYears * 5; i = i + 5)
                    {
                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q1").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 2);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q2").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 3);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q3").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 4);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q4").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 5);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "A").
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

            cell = CreateTextCell(" ");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateTextCell("Data In " + Convert.ToString(currency) + " (Millions)");
            row.InsertAt(cell, 1);

            for (int i = 0; i <= numberOfYears * 5; i = i + 5)
            {
                Columns column = new DocumentFormat.OpenXml.Spreadsheet.Columns();
                mergeColumns = new DocumentFormat.OpenXml.Spreadsheet.Column();

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q1");
                row.InsertAt(cell, i + 2);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q2");
                row.InsertAt(cell, i + 3);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q3");
                row.InsertAt(cell, i + 4);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q4");
                row.InsertAt(cell, i + 5);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " A");
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

            Column firstColumn = new Column() { Min = 2U, Max = 2U, Width = maxWidth};

            sheetColumns.Append(firstColumn);

            int firstYear = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).FirstOrDefault();
            int lastYear = consensusData.Select(a => a.PERIOD_YEAR).OrderByDescending(a => a).FirstOrDefault();
            int numberOfYears = lastYear - firstYear;

            var cell = new Cell();

            cell = CreateTextCell("Data Id");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateTextCell("Data in " + Convert.ToString(currency) + " (Millions)");
            row.InsertAt(cell, 1);

            for (int i = 0; i <= numberOfYears * 5; i = i + 5)
            {
                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q1");
                row.InsertAt(cell, i + 2);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q2");
                row.InsertAt(cell, i + 3);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q3");
                row.InsertAt(cell, i + 4);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q4");
                row.InsertAt(cell, i + 5);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " A");
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