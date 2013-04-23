using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Windows.Input;
using VlastelinClient.Commands;
using Vlastelin.Data.Model;
using System.Collections.ObjectModel;
using VlastelinClient.Util;

namespace VlastelinClient.ViewModel.WindowsViewModel
{
	public class DeparturedBusesWindowVM : BaseViewModel
	{
		#region Свойства

		/// <summary>
		/// список отправленных рейсов
		/// </summary>
		public ObservableCollection<TripScheduleFact> FactTripSchedules { get; set; }

		/// <summary>
		/// дата рейсов
		/// </summary>
		public DateTime DepartureDate { get; set; }

		#endregion

		#region Команды

		public ICommand CancelDepartureCommand
		{
			get
			{
				return new RelayCommand(this.CancelDepartureExecute, this.CancelDepartureCanExecute);
			}
		}

		#endregion

		/// <summary>
		/// загрузка данных из бд
		/// </summary>
		public void LoadData()
		{
			this.FactTripSchedules = new ObservableCollection<TripScheduleFact>(UtilManager.Instance.Client.TripScheduleFactGetByDate(this.DepartureDate.Date).OrderBy(t => t.FactDepartureTime));
		}

		/// <summary>
		/// обработчик команды отмены отправки автобуса
		/// </summary>
		/// <param name="param">рейс</param>
		private void CancelDepartureExecute(object param)
		{
			TripScheduleFact tsf = param as TripScheduleFact;
			if (UtilManager.Instance.MessageProvider.ShowYesNoDialogWindow("Вы действительно хотите отменить отправку автобуса?"))
			{
				FuncExec.Execute(() =>
					{
						UtilManager.Instance.Client.TripScheduleFactDelete(tsf);
						this.FactTripSchedules.Remove(tsf);
						UtilManager.Instance.MessageProvider.ShowInformationWindow(String.Format("Рейс автобуса \"{0} {1}\" отменен", tsf.FactBus.Model, tsf.FactBus.RegNumber));
					});
			}
		}

		/// <summary>
		/// проверка команды отмены отправки автобуса
		/// </summary>
		/// <param name="param">рейс</param>
		private bool CancelDepartureCanExecute(object param)
		{
			return param != null && param is TripScheduleFact;
		}
	}
}
