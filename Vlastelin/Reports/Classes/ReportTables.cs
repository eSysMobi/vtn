using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Vlastelin.Data.Model;

namespace Reports.Classes
{
    public static class ReportTables
    {
        public static DataTable SeatTable(List<Seat> seats, DateTime beginDate, DateTime endDate)
        {
            List<Seat> reportData = seats.Where(s => s.TS.DepartureTime.Date >= beginDate.Date && s.TS.DepartureTime.Date <= endDate.Date).ToList();

            DataTable table = new DataTable();
            table.Columns.Add("Time");
            table.Columns.Add("Trip");
            table.Columns.Add("Tickets");
            table.Columns.Add("Sum");
            table.Columns.Add("Operator");

            table.Rows.Add(
                "Итог", 
                String.Empty, 
                reportData.Count(), 
                reportData.Select(s => s.TS.Trip.Price).Sum(), 
                String.Empty);

            List<DateTime> dates = reportData.Select(s => s.TS.DepartureTime.Date).Distinct().ToList();

            foreach (DateTime d in dates)
            {
                table.Rows.Add(
                    d.ToString("dd.MM.yyyy"),
                    String.Empty,
                    reportData.Where(s => s.TS.DepartureTime.Date == d).Count(),
                    reportData.Where(s => s.TS.DepartureTime.Date == d).Select(s => s.TS.Trip.Price).Sum(),
                    String.Empty);

                foreach (Seat seat in reportData.Where(s => s.TS.DepartureTime.Date == d))
                {
                    table.Rows.Add(
                        String.Empty,
                        String.Format("{0} - {1}", seat.TS.Trip.Departure.Name, seat.TS.Trip.Arrival.Name),
                        1,
                        seat.TS.Trip.Price,
                        "Иванов Иван Иванович");
                }
            }
            return table;
        }

        public static DataTable ActTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Number");
            table.Columns.Add("Name");
            table.Columns.Add("CrewCode");
            table.Columns.Add("BillNumber");
            table.Columns.Add("Sum");
            table.Columns.Add("Person");

            for (int i = 1; i < 5; i++)
            {
                table.Rows.Add(new object[] { i, "Имя чего-то", i, i, i, "Иванов И.И" });
            }

            return table;
        }

        public static DataTable StatementTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("SeatNumber");
            table.Columns.Add("BK");
            table.Columns.Add("FullName");
            table.Columns.Add("PersonalInfo");
            table.Columns.Add("ArrivalTown");
            table.Columns.Add("Price");

            for (int i = 1; i < 9; i++)
            {
                table.Rows.Add(new object[] { i, i, "Ivanov Ivan Pertorvish", "63 03 155555", "1500.00" });
            }

            return table;
        }

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

        public static DataTable KMOTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("RKO");
            table.Columns.Add("Sum");

            for (int i = 1; i < 20; i++)
            {
                table.Rows.Add(new object[] { "Расходный кассовый ордер 00000000 (06.06.11)", i * 1000});
            }

            return table;
        }
    }
}
