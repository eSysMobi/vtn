using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using VlastelinClient.Util;
using System.Windows.Input;
using VlastelinClient.Commands;
using Reports.Classes;
using Vlastelin.Data.Model;
using Vlastelin.Common;
using System.Data;

namespace VlastelinClient.ViewModel.WindowsViewModel
{
    /// <summary>
    /// вьюмодель для отправления автобуса
    /// </summary>
    public class BusDepWindowVM : BaseViewModel
    {
        // расписания автобусов и водители, места
        private IEnumerable<DriverVM> _drivers;

        public IEnumerable<DriverVM> Drivers
        {
            get
            {
                return this._drivers;
            }
            set
            {
                this._drivers = value;
                this.OnPropertyChanged("Drivers");
            }
        }

		public IEnumerable<DriverVM> Drivers1 { get; set; }
        public IEnumerable<DriverVM> Drivers2 { get; set; }

        /// <summary>
        /// выделенное расписание (используется для количества свободных мест и других функций)
        /// </summary>
        public StationScheduleVM SS { get; set; }

		/// <summary>
		/// флаг для отложенной печати РКО
		/// </summary>
		public bool IsRKOPostponed { get; set; }

		/// <summary>
		/// сумма проданных билетов (для печати РКО)
		/// </summary>
		public decimal Sum { get; set; }

        /// <summary>
        /// текущий город, в котором запущена программа
        /// 
        /// </summary>
        public TownVM CurrentTown
        {
            get
            {
                return new TownVM(UtilManager.Instance.CurrentOperator.Branch.Town);
            }
        }

		/// <summary>
		/// текущая дата
		/// </summary>
		public String CurrentDate
		{
			get
			{
				return DateTime.Today.ToString(Ct.ReportShortDateFormat, Ct.RussianCulture);
			}
		}

		/// <summary>
		/// количество занятых мест
		/// </summary>
		public int SeatsTakenCount { get; set; }

        /// <summary>
        /// команда для отправки автобуса в рейс
        /// </summary>
        public ICommand DepartureBusCommand
        {
            get
            {
                return new RelayCommand(this.DepartureBusCommandExecute);
            }
        }

        public BusDepWindowVM()
        {
			this.IsRKOPostponed = true;
        }

		/// <summary>
		/// загрузка водителей с имеющимися доверенностями от владельца автобуса из бд
		/// </summary>
		public void LoadData()
		{
			this.Drivers = UtilManager.Instance.Client.DriversGetByBus(this.SS.TS.Bus.bus).Select(d => new DriverVM(d)).OrderBy(dr => dr.Surname);
		}

        /// <summary>
        /// обновляем список водителей в зависимости от наличия у них доверенности для данного владельца автобуса
        /// </summary>
        /// <param name="bus">автобус</param>
        public void UpdateDriverList()
        {
			this.Drivers1 = this.Drivers;
			this.OnPropertyChanged("Drivers1");
        }

        /// <summary>
        /// обновляет список водителей во втором комбобоксе, чтобы нельзя было выбрать двух одинаковых водителей
        /// </summary>
        /// <param name="driver">выделенный водитель в первом комбобоксе</param>
        public void UpdateDriversComboList(DriverVM driver)
        {
            this.Drivers2 = this.Drivers1.Where(dr => !dr.Equals(driver));
            this.OnPropertyChanged("Drivers2");
        }

		/// <summary>
		/// обработчик команды отправления автобуса в рейс
		/// </summary>
		/// <param name="param"></param>
		private void DepartureBusExecute(object param)
		{
			IList<object> list = param as IList<object>;
            if (list != null)
            {
				bool isDriver2 = (bool)list[0];
				DriverVM driver1 = list[1] as DriverVM;
                DriverVM driver2 = isDriver2 ? list[2] as DriverVM : null;
				double sum = (double)list[3];

				if ((driver1 == null) || (driver2 == null && isDriver2))
                {
					UtilManager.Instance.MessageProvider.ShowInformationWindow("Не задан водитель");
					return;
				}

				//формируем объект Фактическое отправление
                TripScheduleFact tsf = new TripScheduleFact()
                {
					TS = this.SS.TS.ts,
					FactBus = this.SS.TS.Bus.bus,
                    FactDriver1 = driver1.driver,
                    FactDriver2 = isDriver2 ? driver2.driver : null,
                    Operator = UtilManager.Instance.CurrentOperator,
					FactDepartureTime = DateTime.Today.SetTime(this.SS.DepartureTime),
                    DepartureTown = UtilManager.Instance.CurrentOperator.Branch.Town,
                    OperationTime = DateTime.Now
                };

				// формирование репорта РКО (расчетный кассовый ордер)
				tsf.Id = UtilManager.Instance.Client.TripScheduleFactAdd(tsf);

				DataTable reportStmt = UtilManager.Instance.Client.ReportStatement(this.SS.ss, tsf.FactDepartureTime);
					
				UtilManager.Instance.NumbersManager.SetLastNumber();
				RKO rko = new RKO()
				{
					DocDate = DateTime.Now,
					Number = UtilManager.Instance.NumbersManager.Number,
					Operator = UtilManager.Instance.CurrentOperator,
					TSF = tsf
				};
				rko.Sum = (decimal)sum;
				rko.Id = UtilManager.Instance.Client.RKOAdd(rko);

				//if (!this.IsRKOPostponed)
				//{
					ReportsExecutor.ReportCashOrder(rko);
				//}

                //// формирование репорта Ведомость №2               
				ReportsExecutor.ReportStatement(reportStmt, this.SS.ss, DateTime.Now.Date, (double)this.Sum);
				this.OnNotify(String.Empty);
            }
		}

        /// <summary>
        /// обработчик команды отправления автобуса в рейс (с проверкой на ошибки)
        /// </summary>
        /// <param name="param"></param>
        private void DepartureBusCommandExecute(object param)
        {
			FuncExec.Execute(() => this.DepartureBusExecute(param));
        }
    }
}
