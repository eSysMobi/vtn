using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Data;

namespace Reports.Classes
{
    /// <summary>
    /// обертка над созданием документа
    /// </summary>
    public static class DocumentBuilder
    {

        /// <summary>
        /// устанавливает стиль текста в параграфе
        /// </summary>
        /// <param name="p">параграф</param>
        /// <param name="style">стиль</param>
        private static void SetTextStyle(Paragraph p, TextStyle style)
        {
            p.FontSize = style.FontSize;
            p.Foreground = style.Foreground;
            p.TextAlignment = style.Alignment;
            p.FontWeight = style.FontWeight;
            p.FontFamily = new FontFamily("Times New Roman");
        }

        /// <summary>
        /// создает параграф из строки с заданным стилем
        /// </summary>
        /// <param name="text">входная строка</param>
        /// <param name="style">стиль</param>
        /// <returns>полученый парагарф</returns>
        public static Paragraph CreateText(String text, TextStyle style)
        {
            Paragraph p = new Paragraph(new Run(text));
            SetTextStyle(p, style);

            return p;
        }

        /// <summary>
        /// устанавливает стиль ячейки таблицы
        /// </summary>
        /// <param name="cell">ячейка</param>
        /// <param name="style">стиль</param>
        /// <param name="header">заголовок это или нет</param>
        private static void SetCellStyle(TableCell cell, TableStyle style, bool header)
        {
            cell.BorderBrush = style.CellBorderBrush;
            cell.BorderThickness = style.CellBorder;

            if (header)
            {
                cell.Background = style.HeaderBackGround;
                //cell.Foreground = style.HeaderTextStyle.Foreground;
                //cell.FontWeight = style.HeaderTextStyle.FontWeight;
                //cell.FontSize = style.HeaderTextStyle.FontSize;
                //cell.TextAlignment = style.HeaderTextStyle.Alignment;
            }
            else
            {
                cell.Background = style.DataBackground;
                //cell.Foreground = style.DataTextStyle.Foreground;
                //cell.FontWeight = style.DataTextStyle.FontWeight;
                //cell.FontSize = style.DataTextStyle.FontSize;
                //cell.TextAlignment = style.DataTextStyle.Alignment;
            }
        }

        /// <summary>
        /// создает строку таблицы из списка данных
        /// </summary>
        /// <param name="data">массив данных</param>
        /// <param name="style">стиль</param>
        /// <param name="header">флаг заголовка</param>
        /// <returns>строка табилцы</returns>
        private static TableRow CreateRow(IList<String> data, TableStyle style, bool header)
        {
            TableRow row = new TableRow();
            TableCell cell = new TableCell(); 


            foreach (var text in data)
            {
                if (header)
                {
                    cell = new TableCell(CreateText(text, style.HeaderTextStyle));
                }
                else
                {
                    cell = new TableCell(CreateText(text, style.DataTextStyle));
                }
                SetCellStyle(cell, style, header);

                row.Cells.Add(cell);
            }
            return row;
        }

        /// <summary>
        /// создает таблицу с данными
        /// </summary>
        /// <param name="data">таблица данных</param>
        /// <param name="style">стиль</param>
        /// <returns>полученная таблица</returns>
        public static Table CreateTable(DataTable data, TableStyle style)
        {
            Table table = new Table();
            
            table.BorderThickness = style.TableBorder;
            table.BorderBrush = style.TableBorderBrush;
            table.CellSpacing = 1;

            // формирование заголовка таблицы
            List<String> headers = new List<string>();
            foreach (DataColumn col in data.Columns)
            {
                headers.Add(col.Caption);
                table.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            }
            TableRow header = CreateRow(headers, style, true);
            TableRowGroup hgroup = new TableRowGroup();
            hgroup.Rows.Add(header);
            table.RowGroups.Add(hgroup);

            // формирование списка данных
            TableRowGroup dataGroup = new TableRowGroup();
            foreach (DataRow row in data.Rows)
            {
                dataGroup.Rows.Add(CreateRow(row.ItemArray.Select(r => r.ToString()).ToList(), style, false));
            }
            table.RowGroups.Add(dataGroup);

            return table;
        }

    }
}
