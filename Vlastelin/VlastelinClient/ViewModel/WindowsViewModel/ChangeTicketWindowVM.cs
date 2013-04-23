using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Windows.Input;
using VlastelinClient.Commands;
using Vlastelin.Data.Model;
using Vlastelin.Common;
using VlastelinClient.Util;

namespace VlastelinClient.ViewModel.WindowsViewModel
{
	/// <summary>
	/// вьюмодель для обмена билета
	/// </summary>
	public class ChangeTicketWindowVM : BaseViewModel
	{
		#region Свойства

		/// <summary>
		/// список расписаний
		/// </summary>
		public IEnumerable<StationScheduleVM> StationSchedules { get; set; }

		/// <summary>
		/// текущий маршрут
		/// </summary>
		public TripVM CurrentTrip { get; set; }

		/// <summary>
		/// текущий подмаршрут
		/// </summary>
		public TripPriceVM CurrentTripPrice { get; set; }

		/// <summary>
		/// выбранная дата маршрута
		/// </summary>
		public DateTime CurrentDate { get; set; }

		/// <summary>
		/// измененное место
		/// </summary>
		public SeatVM ChangedSeat { get; set; }

		#endregion

		#region Команды

		public ICommand ChangeTicketCommand
		{
			get
			{
				return new RelayCommand(this.ChangeTicketExecute, this.ChangeTicketCanExecute);
			}
		}

		#endregion

		public ChangeTicketWindowVM()
		{
		}

		/// <summary>
		/// обработчик команды обмена билета
		/// </summary>
		/// <param name="param"></param>
		private void ChangeTicketExecute(object param)
		{
			//IList<object> list = param as IList<object>;
			//StationScheduleVM ss = list[0] as StationScheduleVM;
			//DateTime date = (DateTime)list[1];

			//Tuple<Seat[], decimal> result = UtilManager.Instance.Client.SeatsGet(ss.ss, this.CurrentTripPrice.tripPrice, date.SetTime(ss.DepartureTime));
			//List<long> numbers = new List<long>();

			//for (int i = 1; i <= ss.TS.Bus.PassengersCount; i++) numbers.Add(i);
			//long seatNumber = numbers.Where(s => !result.Item1.Select(st => st.SeatNumber).Contains(s)).Min();
		}

		/// <summary>
		/// проверка команды обмена билета
		/// </summary>
		/// <param name="param"></param>
		private bool ChangeTicketCanExecute(object param)
		{
			IList<object> list = param as IList<object>;
			if (list == null) return false;

			return list[0] is StationScheduleVM && list[1] is DateTime;
		}
	}
}
