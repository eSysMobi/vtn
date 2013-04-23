using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Vlastelin.Data.Model;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace Reports.Classes
{
    public class ReportLoader
    {
        private TextStyle textStyle;
        private TableStyle tableStyle;

        private List<Seat> seats;

        public ReportLoader(List<Seat> sts)
        {
            this.seats = sts;
            
            TextStyle header = new TextStyle(Brushes.White, FontWeights.Bold, 16, TextAlignment.Center);
            TextStyle data = new TextStyle(Brushes.Black, FontWeights.Normal, 13, TextAlignment.Left);

            this.textStyle = data;
            this.tableStyle = new TableStyle(header, data);
        }

        public DataTable LoadReportData()
        {
            DataTable table = new DataTable("ReportTable");

            table.Columns.Add("Date", typeof(string));
            table.Columns.Add("Trip", typeof(string));
            table.Columns.Add("Tickets", typeof(int));
            table.Columns.Add("Sum", typeof(double));
            table.Columns.Add("Operator", typeof(string));

            IEnumerable<DateTime> dates = seats.Select(s => s.TS.DepartureTime).Distinct().OrderBy(s => s);
            object[] row = null;

            row = new object[]
                {
                    "Итог",
                    String.Empty,
                    seats.Count(),
                    seats.Sum(s => s.TS.Trip.Price),
                    String.Empty
                };
            table.Rows.Add(row);

            foreach (var date in dates)
            {
                row = new object[]
                {
                    date.ToShortDateString(),
                    String.Empty,
                    seats.Where(seat => seat.TS.DepartureTime.Date == date).Count(),
                    seats.Where(seat => seat.TS.DepartureTime.Date == date).Sum(s => s.TS.Trip.Price),
                    String.Empty
                };
                table.Rows.Add(row);

                List<Seat> sts = seats.Where(s => s.TS.DepartureTime.Date == date).ToList();

                foreach (var seat in sts)
                {
                    row = new object[]
                    {
                        String.Empty,
                        String.Format("{0} - {1}", seat.TS.Trip.DepartureTown.Name, seat.TS.Trip.ArrivalTown.Name),
                        seats.Where(s => s.TS.DepartureTime.Date == date && s.TS == seat.TS).Count(),
                        seats.Where(s => s.TS.DepartureTime.Date == date && s.TS == seat.TS).Sum(s => s.TS.Trip.Price),
                        String.Empty
                    };
                    table.Rows.Add(row);
                }
            }

            return table;
        }

        public IDocumentPaginatorSource GenerateDocument()
        {
            FlowDocument doc = new FlowDocument();

            doc.Blocks.Add(DocumentBuilder.CreateText("Анализ продаж за период", this.textStyle));
            doc.Blocks.Add(DocumentBuilder.CreateTable(this.LoadReportData(), this.tableStyle));

            return doc;
        }

    }
}
