using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.Util;
using System.Collections.ObjectModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Windows.Input;
using VlastelinClient.Commands;
using Reports.Classes;
using Vlastelin.Data.Model;
using System.Data;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.WindowsViewModel
{
    /// <summary>
    /// вьюмодель для окна списка пассажиров
    /// </summary>
    public class PassengersListWindowVM : BaseViewModel
    {
        /// <summary>
        /// список пассажиров
        /// </summary>
        public IList<PassengerVM> Passengers { get; set; }

        /// <summary>
        /// команда для показа отчета по поездкам пассажиров
        /// </summary>
        public ICommand ShowReportCommand
        {
            get
            {
				return new RelayCommand(this.ShowReportCommandExecute, this.ShowReportCommandCanExecute);
            }
        }

        /// <summary>
        /// инициализация
        /// </summary>
        public void Init()
        { 
        }

        public PassengersListWindowVM() 
        {
        }

		/// <summary>
		/// загрузка данных из бд
		/// </summary>
        public void LoadData()
        {
            this.Passengers = UtilManager.Instance.Client.PassengersGet(null).Select(p => new PassengerVM(p)).OrderBy(p => p.FullName).ToList();
        }

        /// <summary>
        /// обработчик команды показа отчета
        /// </summary>
        /// <param name="param"></param>
        public void ShowReportCommandExecute(object param)
        {
            IList<object> list = (IList<object>)param;

            // получаем выбранного пассажира и дату начала и конца периода отчета
            String surname = list[0].ToString();
			String name = list[1].ToString();
			String patronymic = list[2].ToString();

            DateTime? from = (DateTime?)list[3];
            DateTime? to = (DateTime?)list[4];

			FuncExec.Execute(() => 
			{
				// получаем данные отчета по введенному пассажиру
				DataTable data = UtilManager.Instance.Client.ReportPassengers(from, to, surname, name, patronymic);
				ReportsExecutor.ReportPassengerTrip(Ct.FullName(name, surname, patronymic), data, from, to);
				
				// передаем окну уведомление, чтобы оно закрылось после печати отчета
				this.OnNotify(String.Empty);
			});
        }

		/// <summary>
        /// проверка команды показа отчета
        /// </summary>
        /// <param name="param"></param>
		private bool ShowReportCommandCanExecute(object param)
		{
			return true; 
		}
    }
}
