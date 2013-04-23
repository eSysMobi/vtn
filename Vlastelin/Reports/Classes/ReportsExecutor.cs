using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reports.Windows;
using Vlastelin.Data.Model;
using Reports.Documents;

namespace Reports.Classes
{
    public static class ReportsExecutor
    {
        public static void ReportBOD(List<Seat> seats)
        {
            ReportBeginDay page = new ReportBeginDay();
            ReportWindow window = new ReportWindow();

            page.SetData (ReportTables.SeatTable(seats, new DateTime(2012,05,01),new DateTime(2012,08,01)), DateTime.Now, DateTime.Now);
            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }

        public static void ReportEODOne(List<Seat> seats)
        {
            ReportKM3 page = new ReportKM3();
            ReportWindow window = new ReportWindow();

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog(); 
        }

        public static void ReportEODTwo()
        {
            ReportWindow window = new ReportWindow();
            ReportKM3_2 page = new ReportKM3_2();

            page.SetData(ReportTables.ActTable());

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }

        public static void ReportStatement()
        {
            ReportWindow window = new ReportWindow();
            ReportStatement page = new ReportStatement();

            page.SetData(ReportTables.StatementTable());

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }

        public static void ReportCashOrder()
        {
            ReportCashOrder page = new ReportCashOrder();
            ReportWindow window = new ReportWindow();

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }

        public static void ReportIncomeCashOrder()
        {
            ReportIncomeCashOrder page = new ReportIncomeCashOrder();
            ReportWindow window = new ReportWindow();

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }

        public static void ReportMonth()
        {
            ReportMonth page = new ReportMonth();
            ReportWindow window = new ReportWindow();
            page.SetData(ReportTables.MonthTable()); 

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }

        public static void ReportRKO()
        {
            ReportRKO page = new ReportRKO();
            ReportWindow window = new ReportWindow();
            page.SetData(ReportTables.KMOTable());

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }
    }
}
