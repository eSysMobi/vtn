using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Common;
using Reports.Classes;

namespace VlastelinClient.Windows
{
	/// <summary>
	/// тип анализа продаж
	/// </summary>
	public enum ReportAnalysisType
	{
		Common = 0,
		ByTickets = 1,
		ByAdditionalServices = 2,
		ByBus = 3,
		ByTrip = 4
	}

	/// <summary>
	/// Interaction logic for WindowDateIntervalBus.xaml
	/// </summary>
	public partial class WindowCriteriaReportAnalysis : WindowBase
	{
		/// <summary>
		/// автобусы и маршруты для комбобоксов
		/// </summary>
		public IEnumerable<BusVM> Buses { get; set; }
		public IEnumerable<TripVM> Trips { get; set; }
		
		/// <summary>
		/// начало интервала
		/// </summary>
		public DateTime? IntervalFrom
		{
			get
			{
				return this.datePickerFrom.SelectedDate;
			}
		}

		/// <summary>
		/// конец интервала
		/// </summary>
		public DateTime? IntervalTo
		{
			get
			{
				return this.datePickerTo.SelectedDate;
			}
		}

		/// <summary>
		/// выбранный автобус
		/// </summary>
		public BusVM Bus
		{
			get
			{
				return this.comboBoxBuses.SelectedItem as BusVM;
			}
		}

		/// <summary>
		/// выбранный маршрут
		/// </summary>
		public TripVM Trip
		{
			get 
			{
				return this.comboBoxTrip.SelectedItem as TripVM;
			}
		}

		/// <summary>
		/// тип отчета
		/// </summary>
		public ReportTypes ReportType { get; set; }

		public WindowCriteriaReportAnalysis()
		{
			InitializeComponent();

			this.datePickerFrom.SelectedDate = ReportsExecutor.IntervalFrom;
			this.datePickerTo.SelectedDate = ReportsExecutor.IntervalTo;
		}

		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			if (this.radioButtonCommon.IsChecked == true) this.ReportType = ReportTypes.SalesReportFull;
			if (this.radioButtonTicket.IsChecked == true) this.ReportType = ReportTypes.SalesReportTickets;
			if (this.radioButtonAdditionalServices.IsChecked == true) this.ReportType = ReportTypes.SalesReportNonTickets;
			if (this.radioButtonBus.IsChecked == true) this.ReportType = ReportTypes.SalesReportByBus;
			if (this.radioButtonTrip.IsChecked == true) this.ReportType = ReportTypes.SalesReportByTrip;

			this.DialogResult = true;
			this.Close();
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void WindowBase_Loaded(object sender, RoutedEventArgs e)
		{
			this.DataContext = this;
		}

		private void radioButton_Checked(object sender, RoutedEventArgs e)
		{

		}
	}
}
