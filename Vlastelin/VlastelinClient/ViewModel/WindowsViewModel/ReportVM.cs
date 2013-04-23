using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using VlastelinClient.Commands;
using VlastelinClient.Windows;
using VlastelinClient.Util;
using Vlastelin.Common;
using System.Data;
using Reports.Classes;
using System.ComponentModel;
using VlastelinClient.ViewModel.ObjectsViewModel;

namespace VlastelinClient.ViewModel.WindowsViewModel
{
	/// <summary>
	/// вьюмодель для отчетов
	/// </summary>
	public class ReportVM
	{
		/// <summary>
		/// вьюмодель главного окна
		/// </summary>
		private MainWindowVM mainVM;

		/// <summary>
		/// вьюмодель для формы отчета по пассажирам
		/// </summary>
		private PassengersListWindowVM passListVM;

		#region Команды

		public ICommand ReportPassengerCommand
		{
			get
			{
				return new RelayCommand(this.ReportPassengerExecute, this.ReportPassengerCanExecute);
			}
		}

		public ICommand ReportSalesAnalysisCommand
		{
			get
			{
				return new RelayCommand(this.ReportSalesAnalysisExecute, this.ReportSalesAnalysisCanExecute);
			}
		}

		public ICommand ReportStatementCommand
		{
			get
			{
				return new RelayCommand(this.ReportStatementExecute, this.ReportStatementCanExecute);
			}
		}

		public ICommand ReportRKOCommand
		{
			get
			{
				return new RelayCommand(this.ReportRKOExecute, this.ReportRKOCanExecute);
			}
		}

		public ICommand ReportPKOCommand
		{
			get
			{
				return new RelayCommand(this.ReportPKOExecute, this.ReportPKOCanExecute);
			}
		}
		#endregion

		public ReportVM(MainWindowVM vm)
		{
			this.mainVM = vm;
            this.passListVM = new PassengersListWindowVM();
		}

		#region Отчет по анализу продаж
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обрабочик команды печати ведомости
		/// </summary>
		/// <param name="param">не используется</param>
		private void ReportStatementExecute(object param)
		{
			IList<object> list = param as IList<object>;
			StationScheduleVM ss = list[0] as StationScheduleVM;
			DateTime date = (DateTime)list[1];

			FuncExec.Execute(() =>
			{
				// получаем данные ведомости по рейсу и дате
				DataTable reportStmt = UtilManager.Instance.Client.ReportStatement(ss.ss, date.SetTime(ss.DepartureTime));

				// показываем отчет
				ReportsExecutor.ReportStatement(reportStmt, ss.ss, date, reportStmt.Select().Sum(r => double.Parse(r["FactPrice"].ToString())));
			});
		}

		/// <summary>
		/// проверка команды печати ведомости
		/// </summary>
		/// <param name="param">не используется</param>
		private bool ReportStatementCanExecute(object param)
		{
			IList<object> list = param as IList<object>;
			if (list == null) return false;

			return
				list[0] != null && list[0] is StationScheduleVM &&
				list[1] != null &&
				UtilManager.Instance.StateManager.IsServerEnabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Отчет по анализу продаж
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обрабочик команды показа отчета по пассажиру
		/// </summary>
		/// <param name="param">не используется</param>
		public void ReportPassengerExecute(object param)
		{
			WindowPassengerList window = new WindowPassengerList();

			this.mainVM.CatalogCommonExecute(window, () =>
			{
				this.passListVM.LoadData();
			},
			passListVM);
		}

		/// <summary>
		/// обрабочик команды показа отчета по пассажиру
		/// </summary>
		/// <param name="param">не используется</param>
		public bool ReportPassengerCanExecute(object param)
		{
			return UtilManager.Instance.StateManager.IsServerEnabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Отчет по анализу продаж
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обрабочик команды показа отчета по анализу автоубсов
		/// </summary>
		/// <param name="param">не используется</param>
		public void ReportSalesAnalysisExecute(object param)
		{
			WindowCriteriaReportAnalysis window = new WindowCriteriaReportAnalysis();
			BackgroundWorker worker = new BackgroundWorker();

			// процесс, работающий в фоновом режиме
			worker.DoWork += (o, ea) =>
			{
				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.ShowSplash();
				});

				FuncExec.Execute(() =>
				{
					this.mainVM.busVM.LoadData();
					this.mainVM.tripVM.LoadDataExecute();
				});
			};

			// обработчик по окончанию фонового процесса
			worker.RunWorkerCompleted += (o, ea) =>
			{
				UtilManager.Instance.CloseSplash();
				DispatchService.Invoke(() =>
				{
					window.Buses = this.mainVM.busVM.Buses.OrderBy(b => b.Manufacter).ThenBy(b => b.RegNumber);
					window.Trips = this.mainVM.tripVM.Trips.OrderBy(t => t.NameString);

					if (window.ShowDialog() == true)
					{
						DataTable data = null;

						DateTime from = window.IntervalFrom.HasValue ? window.IntervalFrom.Value : DateTime.MinValue;
						DateTime to = window.IntervalTo.HasValue ? window.IntervalTo.Value : DateTime.MaxValue;

						// формируем различные типы отчета в зависимости от того, котоырй был выбран в окне
						switch (window.ReportType)
						{
							case ReportTypes.SalesReportFull:
								{
									data = UtilManager.Instance.Client.ReportGet(ReportTypes.SalesReportFull, from, to, null, null);
									ReportsExecutor.ReportSalesAnalysis(data, window.IntervalFrom, window.IntervalTo, null);
									break;
								}
							case ReportTypes.SalesReportTickets:
								{
									data = UtilManager.Instance.Client.ReportGet(ReportTypes.SalesReportTickets, from, to, null, null);
									ReportsExecutor.ReportSalesAnalysis(data, window.IntervalFrom, window.IntervalTo, null);
									break;
								}
							case ReportTypes.SalesReportNonTickets:
								{
									data = UtilManager.Instance.Client.ReportGet(ReportTypes.SalesReportNonTickets, from, to, null, null);
									ReportsExecutor.ReportSalesAnalysis(data, window.IntervalFrom, window.IntervalTo, null);
									break;
								}
							case ReportTypes.SalesReportByBus:
								{
									data = UtilManager.Instance.Client.ReportGet(ReportTypes.SalesReportByBus, from, to, window.Bus.bus, null);
									ReportsExecutor.ReportSalesAnalysis(data, window.IntervalFrom, window.IntervalTo, window.Bus.bus);
									break;
								}
							case ReportTypes.SalesReportByTrip:
								{
									data = UtilManager.Instance.Client.ReportGet(ReportTypes.SalesReportByTrip, from, to, window.Trip.trip, null);
									ReportsExecutor.ReportSalesAnalysis(data, window.IntervalFrom, window.IntervalTo, window.Trip.trip);
									break;
								}
						}
					}
				});
				UtilManager.Instance.TimerManager.TimerMonitorConnection.Start();
				UtilManager.Instance.TimerManager.TimerMonitorConnection.Start();
			};

			UtilManager.Instance.TimerManager.TimerMonitorConnection.Stop();
			UtilManager.Instance.TimerManager.TimerMonitorConnection.Stop();

			// стартуем фоновый процесс
			worker.RunWorkerAsync();
		}

		/// <summary>
		/// проверка команды показа отчета по анализу автоубсов
		/// </summary>
		/// <param name="param">не используется</param>
		public bool ReportSalesAnalysisCanExecute(object param)
		{
			return UtilManager.Instance.StateManager.IsServerEnabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Отчет РКО
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обрабочик команды показа отчета по РКО
		/// </summary>
		/// <param name="param">не используется</param>
		public void ReportRKOExecute(object param)
		{
			WindowDateInterval window = new WindowDateInterval();
			UtilManager.Instance.TimerManager.TimerMonitorConnection.Stop();
			if (window.ShowDialog() == true)
			{
				DataTable data = UtilManager.Instance.Client.ReportGet(ReportTypes.RKOReport, window.IntervalFrom.HasValue ? window.IntervalFrom.Value : DateTime.MinValue, window.IntervalTo.HasValue ? window.IntervalTo.Value : DateTime.MaxValue, null, null);
                ReportsExecutor.ReportRKO(data, window.IntervalFrom, window.IntervalTo);
			}
			UtilManager.Instance.TimerManager.TimerMonitorConnection.Start();
		}

		/// <summary>
		/// проверка команды показа отчета по РКО
		/// </summary>
		/// <param name="param">не используется</param>
		public bool ReportRKOCanExecute(object param)
		{
			return UtilManager.Instance.StateManager.IsServerEnabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Отчет ПКО
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обрабочик команды показа отчета по ПКО
		/// </summary>
		/// <param name="param">не используется</param>
		public void ReportPKOExecute(object param)
		{
			WindowDateInterval window = new WindowDateInterval();
			UtilManager.Instance.TimerManager.TimerMonitorConnection.Stop();
			if (window.ShowDialog() == true)
			{
				DataTable data = UtilManager.Instance.Client.ReportGet(ReportTypes.PKOReport, window.IntervalFrom.HasValue ? window.IntervalFrom.Value : DateTime.MinValue, window.IntervalTo.HasValue ? window.IntervalTo.Value : DateTime.MaxValue, null, null);
				ReportsExecutor.ReportPKO(data, window.IntervalFrom, window.IntervalTo);
			}
			UtilManager.Instance.TimerManager.TimerMonitorConnection.Start();
		}

		/// <summary>
		/// проверка команды показа отчета по ПКО
		/// </summary>
		/// <param name="param">не используется</param>
		public bool ReportPKOCanExecute(object param)
		{
			return UtilManager.Instance.StateManager.IsServerEnabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion
	}
}
