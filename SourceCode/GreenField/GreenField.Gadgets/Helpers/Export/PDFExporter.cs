﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Telerik.Windows.Data;
using System.Collections;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Helper class for Exporting Grid as PDF
    /// </summary>
    public static class PDFExporter
    {
        /// <summary>
        /// FontSize of Document
        /// </summary>
        private static int fontSizePDF = 6;

        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void btnExportPDF_Click(RadGridView dataGrid, int fontSize = 6)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "*.pdf";
            dialog.Filter = "Adobe PDF Document (*.pdf)|*.pdf";

            fontSizePDF = fontSize;

            if (dialog.ShowDialog() == true)
            {
                RadDocument document = CreateDocument(dataGrid);

                document.LayoutMode = DocumentLayoutMode.Paged;
                document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));

                PdfFormatProvider provider = new PdfFormatProvider();
                using (Stream output = dialog.OpenFile())
                {
                    provider.Export(document, output);
                }
            }

        }

        /// <summary>
        /// Helper Function for PDF
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private static RadDocument CreateDocument(RadGridView grid)
        {
            List<GridViewBoundColumnBase> columns = (from c in grid.Columns.OfType<GridViewBoundColumnBase>()
                                                     orderby c.DisplayIndex
                                                     select c).ToList();
            Table table = new Table();
            RadDocument document = new RadDocument();
            Telerik.Windows.Documents.Model.Section section = new Telerik.Windows.Documents.Model.Section();
            section.Blocks.Add(table);
            document.Sections.Add(section);

            if (grid.ShowColumnHeaders)
            {
                TableRow headerRow = new TableRow();
                if (grid.GroupDescriptors.Count() > 0)
                {
                    TableCell indentCell = new TableCell();
                    indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count() * 20);
                    indentCell.Background = Colors.Gray;
                    headerRow.Cells.Add(indentCell);
                }

                for (int i = 0; i < columns.Count(); i++)
                {
                    TableCell cell = new TableCell();
                    cell.Background = Color.FromArgb(255, 159, 29, 33);
                    AddCellValue(cell, columns[i].UniqueName);
                    cell.PreferredWidth = new TableWidthUnit((float)columns[i].ActualWidth);
                    headerRow.Cells.Add(cell);
                }

                table.Rows.Add(headerRow);
            }

            if (grid.Items.Groups != null)
            {
                for (int i = 0; i < grid.Items.Groups.Count(); i++)
                {
                    AddGroupRow(table, grid.Items.Groups[i] as QueryableCollectionViewGroup, columns, grid);
                }
            }
            else
            {
                AddDataRows(table, grid.Items, columns, grid);
            }

            return document;
        }

        /// <summary>
        /// Helper Method for PDF Export
        /// </summary>
        /// <param name="table"></param>
        /// <param name="items"></param>
        /// <param name="columns"></param>
        /// <param name="grid"></param>
        private static void AddDataRows(Table table, IList items, IList<GridViewBoundColumnBase> columns, RadGridView grid)
        {
            for (int i = 0; i < items.Count; i++)
            {
                TableRow row = new TableRow();

                if (grid.GroupDescriptors.Count() > 0)
                {
                    TableCell indentCell = new TableCell();
                    indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count() * 20);
                    indentCell.Background = Colors.White;
                    row.Cells.Add(indentCell);
                }

                for (int j = 0; j < columns.Count(); j++)
                {
                    TableCell cell = new TableCell();

                    object value = columns[j].GetValueForItem(items[i]);

                    AddCellValue(cell, value != null ? value.ToString() : string.Empty);

                    cell.PreferredWidth = new TableWidthUnit((float)columns[j].ActualWidth);
                    cell.Background = Colors.White;

                    row.Cells.Add(cell);
                }

                table.Rows.Add(row);
            }
        }

        /// <summary>
        /// Helper method for Export PDF
        /// </summary>
        /// <param name="table"></param>
        /// <param name="group"></param>
        /// <param name="columns"></param>
        /// <param name="grid"></param>
        private static void AddGroupRow(Table table, QueryableCollectionViewGroup group, IList<GridViewBoundColumnBase> columns, RadGridView grid)
        {
            TableRow row = new TableRow();

            int level = GetGroupLevel(group);
            if (level > 0)
            {
                TableCell cell = new TableCell();
                cell.PreferredWidth = new TableWidthUnit(level * 20);
                cell.Background = Colors.Red;
                row.Cells.Add(cell);
            }

            TableCell aggregatesCell = new TableCell();
            aggregatesCell.Background = Colors.White;
            aggregatesCell.ColumnSpan = columns.Count() + (grid.GroupDescriptors.Count() > 0 ? 1 : 0) - (level > 0 ? 1 : 0);

            AddCellValue(aggregatesCell, group.Key != null ? group.Key.ToString() : string.Empty);

            foreach (AggregateResult result in group.AggregateResults)
            {
                AddCellValue(aggregatesCell, result.FormattedValue != null ? result.FormattedValue.ToString() : string.Empty);
            }

            row.Cells.Add(aggregatesCell);

            table.Rows.Add(row);

            if (group.HasSubgroups)
            {
                for (int i = 0; i < group.Subgroups.Count(); i++)
                {
                    AddGroupRow(table, group.Subgroups[i] as QueryableCollectionViewGroup, columns, grid);
                }
            }
            else
            {
                for (int i = 0; i < group.ItemCount; i++)
                {
                    AddDataRows(table, group.Items, columns, grid);
                }
            }
        }

        /// <summary>
        /// Helper Method for PDF Export
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="value"></param>
        private static void AddCellValue(TableCell cell, string value)
        {
            Telerik.Windows.Documents.Model.Paragraph paragraph = new Telerik.Windows.Documents.Model.Paragraph();
            cell.Blocks.Add(paragraph);
            Telerik.Windows.Documents.Model.Span span = new Telerik.Windows.Documents.Model.Span();
            if (value == "")
                value = " ";
            span.Text = value;
            span.FontFamily = new System.Windows.Media.FontFamily("Arial");
            span.FontSize = fontSizePDF;
            paragraph.Inlines.Add(span);
        }

        /// <summary>
        /// Helper Method for PDF Export
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private static int GetGroupLevel(IGroup group)
        {
            int level = 0;
            IGroup parent = group.ParentGroup;
            while (parent != null)
            {
                level++;
                parent = parent.ParentGroup;
            }
            return level;
        }
    }
}