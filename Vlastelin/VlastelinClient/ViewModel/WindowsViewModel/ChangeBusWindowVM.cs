using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Common;
using System.Windows.Input;
using VlastelinClient.Commands;
using VlastelinClient.Util;
using System.ComponentModel;

namespace VlastelinClient.ViewModel.WindowsViewModel
{
	/// <summary>
	/// вьюмодель для окна смены автобуса
	/// </summary>
	public class ChangeBusWindowVM : BaseViewModel
	{
		/// <summary>
		/// текущий рейс
		/// </summary>
		public StationScheduleVM SS { get; set; }

		/// <summary>
		/// дата рейса
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// список доступных автобусов
		/// </summary>
		public IEnumerable<BusVM> AvailableBuses { get; set; }

		/// <summary>
		/// стрковое представление даты отправления (для форматированного ыввода)
		/// </summary>
		public String DepartureTime
		{
			get
			{
				return this.SS != null ? this.Date.SetTime(this.SS.DepartureTime).ToString("dd.MM.yyyy HH:mm", Ct.RussianCulture) : String.Empty;
			}
		}

		/// <summary>
		/// количество купленных мест
		/// </summary>
		public int BoughtSeats { get; set; }

		/// <summary>
		/// команда изменения автобуса
		/// </summary>
		public ICommand ChangeBusCommand
		{
			get
			{
				return new RelayCommand(this.ChangeBusExecute, this.ChangeBusCanExecute);
			}
		}

		/// <summary>
		/// загрузка автобусов
		/// </summary>
		/// <param name="ss"></param>
		/// <param name="date"></param>
		public void LoadData(StationScheduleVM ss, DateTime date, int seatsCount)
		{
			this.SS = ss;
			this.Date = date;
			this.BoughtSeats = seatsCount;

			this.AvailableBuses = UtilManager.Instance.Client.GetAvailableBuses(ss.ss, date, seatsCount).OrderBy(bus => bus.PassengersCount).ThenBy(bs => bs.Manufacter).Select(b => new BusVM(b));
		}

		/// <summary>
		/// обработчик команды изменения автобуса
		/// </summary>
		/// <param name="param">новый автобус</param>
		private void ChangeBusExecute(object param)
		{
			BusVM bus = param as BusVM;
			FuncExec.Execute(() =>
				{
					UtilManager.Instance.Client.OperatorChangeBus(SS.TS.ts, bus.bus, DateTime.Today);
					UtilManager.Instance.MessageProvider.ShowInformationWindow(String.Format("Автобус \"{0}\" изменен на \"{1}\"", this.SS.TS.Bus.DisplayName, bus.DisplayName));
					this.OnNotify(String.Empty);
				});
		}

		/// <summary>
		/// проверка возможности изменения автобуса
		/// </summary>
		/// <param name="param">новый автобус</param>
		private bool ChangeBusCanExecute(object param)
		{
			BusVM bus = param as BusVM;
			return bus != null;
		}
	}
}
