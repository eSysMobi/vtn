using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Vlastelin.Data.Model;
using Vlastelin.Common;

namespace Reports.Classes
{
    public static class ReportTables
    {
		//////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// сортировка таблицы
        /// </summary>
        /// <param name="data">таблица</param>
        /// <param name="sortColumn">столбец для сортировки</param>
        /// <returns></returns>
        public static DataTable SortTable(DataTable data, String sortColumn)
        {
            DataTable table = data.Copy();
            table.Clear();

            foreach (DataRow row in data.Select().OrderBy(r => r[sortColumn]))
            {
                table.Rows.Add(row.ItemArray);
            }

            return table;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// делит таблицу на несколько таблиц
        /// </summary>
        /// <param name="data">таблица</param>
        /// <param name="partSize">размер одной части</param>
        /// <returns></returns>
        public static List<DataTable> SplitTable(DataTable data, int partSize)
        {
            List<DataTable> list = new List<DataTable>();
            DataTable partTable= data.Copy();
            partTable.Clear();
            int i = 0;

            foreach (DataRow row in data.Select())
            {
                partTable.Rows.Add(row.ItemArray);
                i++;

                if (i % partSize == 0 || i >= data.Rows.Count)
                {
                    list.Add(partTable.Copy());
                    partTable.Clear();
                }
            }

            return list;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
        /// преобразует исходную таблицу с отчетом в вид, показываемый на печатной форме отчета
        /// </summary>
        /// <param name="data">исходная табилца</param>
        /// <returns>получившаяся таблица</returns>
		public static DataTable TableSales(DataTable data)
        {
			DataTable table = new DataTable();

			// копируем столбцы таблицы, чтобы совпадали со столбцами исхоной таблицы
			String columnDt = "dt";
			String columnItem = "item";
			String columnCheckCount = "checkCount";
			String columnSalesSum = "salesSum";
			String columnOperator = "operator";
            
            table.Columns.Add(columnDt);
            table.Columns.Add(columnItem);
            table.Columns.Add(columnCheckCount);
			table.Columns.Add(columnSalesSum);
            table.Columns.Add(columnOperator);

			// список всех строк исходной таблицы
			IList<DataRow> rows = data.Select().ToList();
			
			// список строк таблицы по определенной дате
			IList<DataRow> rowsByDate;

			// список дат отчета
			IList<DateTime> dates = rows.Select(r => (DateTime)r[columnDt]).Distinct().ToList();
			DataRow row;

			// формируем итоговую строку
			row = table.NewRow();

			row[columnDt] = "Итог";
			row[columnItem] = String.Empty;
			row[columnCheckCount] = rows.Where(rw => rw[columnCheckCount] != DBNull.Value).Select(r => double.Parse(r[columnCheckCount].ToString())).Sum();
			row[columnSalesSum] = rows.Where(rw => rw[columnSalesSum] != DBNull.Value).Select(r => double.Parse(r[columnSalesSum].ToString())).Sum();
			row[columnOperator] = String.Empty;

			table.Rows.Add(row);

			// проходим по датам отчета
			foreach (DateTime date in dates.OrderBy(d => d))
			{
				// формируем итоговую строку для даты
				row = table.NewRow();
				rowsByDate = rows.Where(r => (DateTime)r[columnDt] == date).ToList();
				
				row[columnDt] = date.ToString(Ct.ReportShortDateFormat, Ct.RussianCulture);
				row[columnItem] = String.Empty;
				row[columnCheckCount] = rowsByDate.Where(rw => rw[columnCheckCount] != DBNull.Value).Select(r => double.Parse(r[columnCheckCount].ToString())).Sum();
				row[columnSalesSum] = rowsByDate.Where(rw => rw[columnSalesSum] != DBNull.Value).Select(r => double.Parse(r[columnSalesSum].ToString())).Sum();
				row[columnOperator] = String.Empty;

				table.Rows.Add(row);
				
				// проходим по всем строкам за конкретную дату
				foreach (DataRow rw in rowsByDate)
				{
					row = table.NewRow();

					// заполняем остальные строки
					row[columnDt] = String.Empty;
					row[columnItem] = rw[columnItem];
					row[columnCheckCount] = rw[columnCheckCount];
					row[columnSalesSum] = rw[columnSalesSum];
					row[columnOperator] = rw[columnOperator];

					table.Rows.Add(row);
				}				
			}

            return table;
        }

		//////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// формирование таблицы для отчета акт о возврате денежных средств
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
        public static DataTable ReturnActTable(DataTable data)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Number");
            table.Columns.Add("Name");
            table.Columns.Add("CrewCode");
            table.Columns.Add("ReturnedCheckNum");
            table.Columns.Add("FactPrice");
            table.Columns.Add("Operator");

			// строка с номерами полей (есть в отчете)
			table.Rows.Add(1, 2, 3, 4, 5, 6);

			// формируем таблицу отчета
			int number = 1;
			foreach (DataRow row in data.Select().OrderBy(dr => dr["ReturnedCheckNum"]))
			{
				table.Rows.Add(
					number++,
					1,
					String.Empty,
					row["ReturnedCheckNum"],
					row["FactPrice"],
					row["Operator"]
					);
			}

            return table;
        }

		//////////////////////////////////////////////////////////////////////////////////////////////////

        public static DataTable MonthTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Number");
            table.Columns.Add("Date");
            table.Columns.Add("RegNumber");
            table.Columns.Add("Sum");

            for (int i = 1; i < 3; i++)
            {
                table.Rows.Add(new object[] { i, DateTime.Now.ToShortDateString(), "AH 800", i * 1000 });
            }

            return table;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// таблица для отчета по поездкам пассажира
		/// </summary>
		/// <param name="data">данные отчета</param>
		/// <returns></returns>
        public static DataTable PassengerTripsTable(DataTable data)
        {
            DataTable table = new DataTable();
            table.Columns.Add("dt");
            table.Columns.Add("trip");
            table.Columns.Add("SeatNumber");
            table.Columns.Add("FactPrice");
			table.Columns.Add("Operator");

			foreach (DataRow row in data.Select().OrderByDescending(r => r["dt"]))
			{
				table.Rows.Add(
					((DateTime)row["dt"]).ToString("dd.MM.yyyy HH:mm", Ct.RussianCulture),
					row["trip"],
					row["SeatNumber"],
					row["FactPrice"],
					row["Operator"]
					);
			}

            return table;
        }
    }
}
