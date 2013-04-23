using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reports.Windows;
using Vlastelin.Data.Model;
using Reports.Documents;
using VlastelinClient.Util;
using System.Globalization;
using Vlastelin.Common;
using VlastelinClient.Reports.Documents;
using Vlastelin.KKM;
using System.Data;
using System.Windows.Controls;

namespace Reports.Classes
{
    public static class ReportsExecutor
    {
		public static DateTime? IntervalFrom { get; set; }
		public static DateTime? IntervalTo { get; set; }
		
		/// <summary>
		/// отчет - анализ продаж
		/// </summary>
		/// <param name="data">данные отчета</param>
		/// <param name="from">начало интервала</param>
		/// <param name="to">конец интервала</param>
		public static void ReportSalesAnalysis(DataTable data, DateTime? from, DateTime? to, BaseItem item = null)
		{
			ReportSalesAnalysis page = new ReportSalesAnalysis();
			ReportWindow window = new ReportWindow();

			IntervalFrom = from;
			IntervalTo = to;

			page.textBlockHeader.Text = String.Format("Анализ продаж {0}", IntervalDateString(from, to));

			if (item != null)
			{
				String text = String.Empty;
				if (item is Bus)
				{
					text = String.Format("ТС - {0} ({1})", (item as Bus).Manufacter, (item as Bus).RegNumber);
				}
				if (item is Trip)
				{
					text = String.Format("Маршрут ({0})", (item as Trip).NameString);
				}
				page.textBlockAddInfo.Text = text;
				page.textBlockAddInfo.Visibility = System.Windows.Visibility.Visible;
			}

			page.SetData(ReportTables.TableSales(data));
			DocumentHelper.SetPageContent(page, window.documentViewer);

			window.ShowDialog();
		}

        /// <summary>
        /// отчет - справка кассира-операциониста
        /// печатается при закрытии смены в конце рабочего дня
        /// </summary>
        public static void Report_CashierBill()
        {
            UtilManager.Instance.NumbersManager.SetLastNumber();
            
            ReportCashierBill page = new ReportCashierBill();
            ReportWindow window = new ReportWindow();

            page.textBlockOrg.Text = UtilManager.Instance.ServerSettings.OrganizationName;
            page.textBlockKKMModel.Text = UtilManager.Instance.KKMManager.KKMModel;

            page.textBlockINN.Text = UtilManager.Instance.ServerSettings.OrganizationINN;
            page.textBlockKKMRegNum.Text = UtilManager.Instance.KKMManager.RegistrationNum;
            page.textBlockKKMSerNum.Text = UtilManager.Instance.KKMManager.SerialNum;
           
            page.textBlockDocNum.Text = UtilManager.Instance.NumbersManager.Number.ToString();
            page.textBlockDocDate.Text = DateTime.Now.ToString(Ct.ReportShortDateFormat, Ct.RussianCulture);

            page.textBlockIncrementNumber.Text = UtilManager.Instance.KKMManager.ControlCounterValue.ToString();
            page.textBlockControlCounter.Text = UtilManager.Instance.KKMManager.IncrementalControlCounterValue.ToString("F2");
            page.textBlockSumBeginDay.Text = UtilManager.Instance.KKMManager.AccMoneyCounterValue.ToString();
            page.textBlockEndDay.Text = (UtilManager.Instance.KKMManager.AccMoneyCounterValue + UtilManager.Instance.KKMManager.TotalRevenue).ToString();
            page.textBlockSumDay.Text = UtilManager.Instance.KKMManager.TotalRevenue.ToString();
            page.textBlockSumTotal.Text = UtilManager.Instance.KKMManager.TotalRevenue.ToString();
            page.textBlockSumReverted.Text = UtilManager.Instance.KKMManager.TotalReturn.ToString();
            page.textBlockOperator1.Text = UtilManager.Instance.CurrentOperator.Surname;

			page.textBlockSumText.Text = NumByWords.RurPhrase(UtilManager.Instance.KKMManager.TotalRevenue);
            page.textBlockOperator2.Text = UtilManager.Instance.CurrentOperator.InitialName;

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }

		/// <summary>
		/// авт о возврате денежных сумм покупателям по неиспользованным кассовым чекам
		/// печатается при закрытии смены в конце дня
		/// </summary>
		public static void ReportReturnAct(DataTable table, DateTime date)
        {
			UtilManager.Instance.NumbersManager.SetLastNumber();
			
			ReportWindow window = new ReportWindow();
			ReportReturnAct page = new ReportReturnAct();

			page.textBlockOrganization.Text = UtilManager.Instance.ServerSettings.OrganizationName;
			page.textBlockOrganization2.Text = Ct.InitialName(UtilManager.Instance.ServerSettings.OrganizationDirName, UtilManager.Instance.ServerSettings.OrganizationDirSurname, UtilManager.Instance.ServerSettings.OrganizationDirPatronymic);
			page.textBlockDirPosition.Text = "ИП";

			page.textBlockINN.Text = UtilManager.Instance.ServerSettings.OrganizationINN;
			page.textBlockRegNum.Text = UtilManager.Instance.KKMManager.RegistrationNum;
			page.textBlockManfNum.Text = UtilManager.Instance.KKMManager.SerialNum;

			page.textBlockDocNum.Text = UtilManager.Instance.NumbersManager.Number.ToString();
			page.textBlockDocDate.Text = date.ToString(Ct.ReportShortDateFormat, Ct.RussianCulture);
			page.textBlockDocDateLong.Text = date.ToString("dd MMMM yyyy г.", Ct.RussianCulture);

			page.textBlockOperator1.Text = UtilManager.Instance.CurrentOperator.Surname;
			page.textBlockOperator2.Text = UtilManager.Instance.CurrentOperator.InitialName;

			page.textBlockAttachment.Text = table.Rows.Count.ToString();

			double sum = table.Select().Where(r => r["FactPrice"] != DBNull.Value).Sum(r => double.Parse(r["FactPrice"].ToString()));
			page.textBlockSum.Text = sum.ToString();
			page.textBlockSumText.Text = NumByWords.RurPhrase((decimal)sum);

            page.SetData(ReportTables.ReturnActTable(table));

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }

        /// <summary>
        /// отчет - ведомость №2
        /// печатается при отправке автобуса в рейс
        /// содержит список пассажиров, едущих на данном автобусе
        /// </summary>
        /// <param name="receipts">список чеков</param>
        public static void ReportStatement(DataTable data, StationSchedule ss, DateTime dt, double totalSum)
        {
            ReportWindow window = new ReportWindow();
            ReportStatement page = new ReportStatement();

            page.SetData(ReportTables.SortTable(data, "SeatNumber"));
            page.textBlockBus.Text = "Автобус № " + ss.TS.Bus.RegNumber;
            page.textBlockDate.Text = dt.ToString(Ct.ReportLongDateFormat, Ct.RussianCulture);
            page.textBlockTrip.Text = "Список пассажиров, следующих по маршруту   " + ss.TS.Trip.NameString;
            page.textBlockTime.Text = "Время отправления:   " + ss.DepartureTime.ToString(Ct.ReportTimeFormat);
            page.textBlockSum.Text = totalSum.ToString();

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }

        /// <summary>
        /// создание расходного кассового ордера
        /// </summary>
        /// <param name="rko">объект ордера с данными для отчета</param>
        public static void ReportCashOrder(RKO rko)
        {           
            ReportCashOrder page = new ReportCashOrder();
            ReportWindow window = new ReportWindow();

            // установка параметров отчета
			page.textBlockBus.Text = String.Format("Автобус: {0} {1}",  rko.Bus.Model, rko.Bus.RegNumber);
			page.textBlockTrip.Text = String.Format("Рейс: {0}", rko.TSF.TS.Trip.NameString);
			page.textBlockDeparture.Text = String.Format("Отправление: {0} {1}", rko.TSF.TS.Trip.NameString, rko.DepTime.ToString(Ct.LongDateTimeFormat));
            page.textBlockDate.Text = DateTime.Now.ToString(Ct.ReportShortDateFormat);
			page.textBlockReportNum.Text = rko.Number;
            page.textBlockDirector1.Text = UtilManager.Instance.ServerSettings.OrganizationName;
            page.textBlockDriver.Text = String.Format("{0} ч/з вод. {1}", rko.Owner.Name, rko.TSF.FactDriver1.InitialName);
            page.textBlockSumNumber.Text = rko.Sum.ToString();
            page.textBlockSumText1.Text = NumByWords.RurPhrase((Decimal)rko.Sum);
            page.textBlockLondDate.Text = DateTime.Now.ToString(Ct.ReportShortDateFormat, Ct.RussianCulture);
            page.textBlockDirector2.Text = Ct.InitialName(UtilManager.Instance.ServerSettings.OrganizationDirName, UtilManager.Instance.ServerSettings.OrganizationDirSurname, UtilManager.Instance.ServerSettings.OrganizationDirPatronymic);
			page.textBlockOperator.Text = rko.Operator.InitialName;
			
			page.textBlockDocument1.Text = String.Format("паспорт сер. {0} № {1} выдан: {2}", rko.TSF.FactDriver1.PassportSer, rko.TSF.FactDriver1.PassportNum, rko.TSF.FactDriver1.PassportDate.ToString(Ct.ReportShortDateFormat));
			page.textBlockDocument2.Text = rko.TSF.FactDriver1.PassportIssuer;

			DocumentHelper.SetPageContent(page, window.documentViewer);

			window.ShowDialog();
        }

		/// <summary>
		/// приходный кассовый ордер
		/// </summary>
        public static void ReportIncomeCashOrder(PKO pko)
        {
            ReportIncomeCashOrder page = new ReportIncomeCashOrder();
            ReportWindow window = new ReportWindow();

			page.textBlockDocNumber1.Text = page.textBlockDocNumber2.Text = pko.DocNum;
			page.textBlockDocDate.Text = pko.DocDate.ToString(Ct.ReportShortDateFormat, Ct.RussianCulture);
			page.textBlockOrgName1.Text = page.textBlockOrgName2.Text = UtilManager.Instance.ServerSettings.OrganizationName;
			page.textBlockOperator1.Text = page.textBlockOperator2.Text = pko.Operator.InitialName;
			page.textBlockNDC1.Text = page.textBlockNDC2.Text = "без НДС";
			page.textBlockSumText1.Text = page.textBlockSumText2.Text = NumByWords.RurPhrase((decimal)pko.Sum);
			page.textBlockAttachment.Text = String.Format("справка кассира-операциониста от {0}", pko.DocDate.ToString(Ct.ReportShortDateFormat, Ct.RussianCulture));
			page.textBlockSum.Text = pko.Sum.ToString();
			page.textBlockSum2.Text = ((int)pko.Sum).ToString(); // получение рублей
			page.textBlockSumCop.Text = Math.Round(((Math.Round(pko.Sum, 2) - (int)pko.Sum)) * 100, 0).ToString(); // получение копеек

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

        /// <summary>
        /// многостраничный отчет, для больших таблиц
        /// </summary>
        /// <param name="getPage">функция создания экземпляра страницы</param>
        /// <param name="data">таблица с очетом</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void MultiPageReport(Func<DataTable, DateTime?, DateTime?, Page> getPage, DataTable data, String sortColumn, DateTime? from, DateTime? to)
        {
            ReportWindow window = new ReportWindow();

            List<Page> pages = new List<Page>();
            Page page;

            foreach (DataTable part in ReportTables.SplitTable(ReportTables.SortTable(data, sortColumn), 10))
            {
                page = getPage(part, from, to);
                pages.Add(page);
            }

            DocumentHelper.SetPageMultiContent(pages, window.documentViewer);
            window.ShowDialog();
        }

		/// <summary>
		/// репорт РКО (расчетный кассовый ордер)
		/// </summary>
		/// <param name="data">данные</param>
		/// <param name="from">начала интервала</param>
		/// <param name="to">конец интервала</param>
        public static void ReportRKO(DataTable data, DateTime? from, DateTime? to)
        {
            ReportRKO page = new ReportRKO();
            ReportWindow window = new ReportWindow();

			IntervalFrom = from;
			IntervalTo = to;

            page.SetData(data);

			page.textBlockHeader.Text = String.Format("Отчет по РКО {0}", IntervalDateString(from, to));
			page.textBlockOperator.Text = String.Format("Кассир : {0}", UtilManager.Instance.CurrentOperator.FullName);
			page.textBlockTotalSum.Text = data.Select().Sum(r => double.Parse(r["Сумма"].ToString())).ToString();

            DocumentHelper.SetPageContent(page, window.documentViewer);
            window.ShowDialog();
        }

		/// <summary>
		/// репорт ПКО (приходный кассовый ордер)
		/// </summary>
		/// <param name="data">данные</param>
		/// <param name="from">начала интервала</param>
		/// <param name="to">конец интервала</param>
		public static void ReportPKO(DataTable data, DateTime? from, DateTime? to)
		{
			ReportRKO page = new ReportRKO();
			ReportWindow window = new ReportWindow();

			IntervalFrom = from;
			IntervalTo = to;

            page.SetData(ReportTables.SortTable(data, "РКО"));

			page.textBlockHeader.Text = String.Format("Отчет по ПКО {0}", IntervalDateString(from, to));
            page.textBlockOperator.Text = String.Format("Кассир : {0}", UtilManager.Instance.CurrentOperator.FullName);
			page.textBlockTotalSum.Text = data.Select().Sum(r => double.Parse(r["Сумма"].ToString())).ToString();

			DocumentHelper.SetPageContent(page, window.documentViewer);

			window.ShowDialog();
		}

		/// <summary>
		/// формирует строку вида: за период от 12.01.2012 до 12.02.2012, в зависимости от выбранных или не выбранных дат
		/// </summary>
		/// <param name="from">начало интервала</param>
		/// <param name="to">конец интервала</param>
		/// <returns>результирующая строка</returns>
		private static String IntervalDateString(DateTime? from, DateTime? to)
		{
			String dateString = String.Empty;
			if (from != null && to != null)
				dateString = String.Format("за период от {0:dd.MM.yyyy} до {1:dd.MM.yyyy}", from.Value, to.Value);
			else
				if (from == null && to != null)
					dateString = String.Format("до {0:dd.MM.yyyy}", to.Value);
				else
					if (from != null && to == null) dateString = String.Format("от {0:dd.MM.yyyy}", from.Value);
			return dateString;
		}

        /// <summary>
        /// отчет - поездки пассажира
        /// </summary>
        /// <param name="pass">пассажир</param>
        /// <param name="receipts">список чеков</param>
        /// <param name="from">начало интервала отчета (дата)</param>
        /// <param name="to">конец интервала отчета (дата)</param>
        public static void ReportPassengerTrip(String fullname, DataTable data, DateTime? from, DateTime? to)
        {
            ReportPassengerTrips page = new ReportPassengerTrips();
            ReportWindow window = new ReportWindow();

			IntervalFrom = from;
			IntervalTo = to;

			page.textBlockHeader.Text = String.Format("Список поездок пассажира {0} {1}", fullname, IntervalDateString(from, to));
            page.SetData(ReportTables.PassengerTripsTable(data));

            DocumentHelper.SetPageContent(page, window.documentViewer);

            window.ShowDialog();
        }
    }
}
