using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Data.Model;
//using VlastelinClient.ServiceReference1;
using VlastelinClient.Util;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using VlastelinClient.Commands;
using System.Data;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel
{
    /// <summary>
    /// вьюмодель для расписания
    /// </summary>
    public class TripSheduleWindowVM : BaseWindowVM
    {
		private BusWindowVM busWindowVM;
		private String noneTime = String.Empty;
		
		/// <summary>
		/// типы расписания
		/// </summary>
		public ICollection<String> ScheduleTypes
		{
			get
			{
				return Ct.GetEnumDescriptionValues(typeof(TripScheduleType));
			}
		}
		
		/// <summary>
        /// список времени для ввода
        /// </summary>
        public List<String> ConstantTimeList { get; private set; }
        
        /// <summary>
        /// выбранный маршрут и подмаршрут
        /// </summary>
        public TripVM SelectedTrip { get; set; }

        /// <summary>
        /// выделенное расписание
        /// </summary>
        public StationScheduleVM SelectedSchedule { get; set; }

        /// <summary>
        /// используется для списка маршрутов
        /// </summary>
        private TripWindowVM tripWindowVM;

        /// <summary>
        /// список расписаний
        /// </summary>
        public ObservableCollection<StationScheduleVM> Shedules { get;  set; }

		/// <summary>
		/// список автобусов, используется для добавления и редактирования маршрутов
		/// </summary>
		public IEnumerable<BusVM> Buses
		{
			get
			{
				return this.busWindowVM.Buses != null ? this.busWindowVM.Buses.OrderBy(b => b.Manufacter) : null;
			}
		}

        /// <summary>
        /// список расписаниеов, используется для редактирования расписания
        /// </summary>
        public ObservableCollection<TripVM> Trips
        {
            get
            {
                return this.tripWindowVM.Trips;
            }  
        }

        /// <summary>
        /// матрица времени (для расписаний маршрута)
        /// </summary>
        public DataTable TimeMatrix { get; set; }

        /// <summary>
        /// словарь по городам и времени, используется для добавления расписания сразу в целый маршрут
        /// </summary>
        public ObservableCollection<TownTimePair> TripTimeDictionary { get; set; }

        #region Команды

		public ICommand AddTimeCommand
		{
			get
			{
				return new RelayCommand(this.AddTimeExecute, this.AddTimeCanExecute);
			}
		}

        public ICommand SaveTimeCommand
        {
            get
            {
                return new RelayCommand(this.SaveTimeExecute);
            }
        }

        public ICommand DeleteTimeCommand
        {
            get
            {
				return new RelayCommand(this.DeleteTimeExecute, this.DeleteTimeCanExecute);
            }
        }

        #endregion

        public TripSheduleWindowVM(TripWindowVM tripVM, BusWindowVM busVM)
        {
            this.tripWindowVM = tripVM;
			this.busWindowVM = busVM;
        }

        /// <summary>
        /// инициализация начальных данных
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.ModifiedObj = Vlastelin.Common.ModifiedObjects.TripsSchedule;

            // заполняет комбобокс для ввода времени значениями
            // 00:00 - 23:55
            this.ConstantTimeList = new List<string>();
			this.ConstantTimeList.Add(noneTime);
            DateTime time = DateTime.MinValue;
            while (time < DateTime.MinValue.AddDays(1))
            {
                this.ConstantTimeList.Add(time.GetTimeString());
                time = time.AddMinutes(5);
            }
        }

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public override void LoadDataExecute()
        {
            base.LoadDataExecute();
            this.Shedules = new ObservableCollection<StationScheduleVM>(UtilManager.Instance.Client.StationScheduleGet(null).Select(shedule => new StationScheduleVM(shedule)));

			foreach (var trip in this.Trips)
			{
				trip.SetHasSchedule(this.Shedules);
			}
			this.OnPropertyChanged("Shedules");
        }

        #region Работа с интерфейсом матрицы времени

        /// <summary>
        /// обновляет словарь городов и времени
        /// </summary>
        private void UpdateTimeDictionary()
        {
            // словарь имеет следующий вид
            // Саратов 12:00
            // Балаково 14:00
            // Ртищево 16:00
            //
            // пункт назначения не выводится в словарь, потому что из него нет подмаршрутов и автобус дальше не едет

            this.TripTimeDictionary = new ObservableCollection<TownTimePair>();

			if (this.SelectedSchedule == null)
			{
				// проходим по всем промежуточным пунктам маршрута, кроме пункта прибытия и формируем словарь
				foreach (TownVM town in this.SelectedTrip.RouteTowns.Take(this.SelectedTrip.RouteTowns.Count - 1))
				{
					this.TripTimeDictionary.Add(new TownTimePair(town, this.noneTime));
				}
			}
			else
			{
				IList<StationScheduleVM> ssList = this.Shedules.Where(s => s.TS.Equals(this.SelectedSchedule.TS)).ToList();
				StationScheduleVM ss;
				foreach (TownVM town in this.SelectedTrip.RouteTowns.Take(this.SelectedTrip.RouteTowns.Count - 1))
				{
					ss = this.Shedules.FirstOrDefault(s => s.TS.Equals(this.SelectedSchedule.TS) && s.Town.Equals(town));
					this.TripTimeDictionary.Add(new TownTimePair(town, ss != null ? ss.Time : this.noneTime));
				}
			}
            this.OnPropertyChanged("TripTimeDictionary");
        }

        /// <summary>
        /// инициализирует матрицу и создает столбцы
        /// </summary>
        private void CreateMatrix()
        {
            if (this.TimeMatrix == null)
            {
                this.TimeMatrix = new DataTable();

                //формируем столбцы матрицы
                foreach (TownVM town in this.SelectedTrip.RouteTowns.Take(this.SelectedTrip.RouteTowns.Count - 1))
                {
                    this.TimeMatrix.Columns.Add(town.Name);
                }
            }
        }

        /// <summary>
        /// обновляет матрицу времени в зависимости от расписаний для выделенного маршрута
        /// </summary>
        private void UpdateTimeMatrix()
        {
            this.TimeMatrix = null;

            //получаем расписания для выделенного маршрута, чтобы отобразить их на интерфейсе
            List<StationScheduleVM> shList = this.Shedules.Where(sh => sh.TS.Trip.Equals(this.SelectedTrip) && !sh.Town.Equals(this.SelectedTrip.Arrival)).ToList();

            if (shList.Count > 0)
            {
                this.CreateMatrix();
                DataRow row;

                // проходим по каждому из времени выезда
                foreach (TripScheduleVM ts in shList.Select(s => s.TS).Distinct())
                {
                    row = this.TimeMatrix.NewRow();

                    //в пределах расписаний для выбранного маршрута ищем остальные маршруты по ключу - времени, и затем формируем строку матрицы
                    foreach (StationScheduleVM shedule in shList.Where(sh => sh.TS.Equals(ts)))
                    {
                        row[shedule.Town.Name] = shedule.DepartureTime.GetTimeString();
                    }
                    this.TimeMatrix.Rows.Add(row);
                }
            }

            this.OnPropertyChanged("TimeMatrix");
        }

#endregion

        /// <summary>
        /// обработчик команды для изменения расписания
        /// </summary>
        /// <param name="param">объект расписания</param>
        protected override void EditItem(object param)
        {
        }

		/// <summary>
		/// обновляем выделенный маршрут
		/// </summary>
		/// <param name="trip"></param>
        public void UpdateSelectedTrip(TripVM trip)
        {
            this.SelectedTrip = trip;
			this.flagTypeEdit = false;
            this.UpdateTimeDictionary();
            this.UpdateTimeMatrix();
            this.OnPropertyChanged("SheduleType");
        }

		/// <summary>
		/// обновляем выделенное расписание
		/// </summary>
		public void UpdateSelectedStationSchedule(DataRowView row)
		{
			if (row != null)
			{
				this.SelectedSchedule = this.GetCurrentFirstSS(row.Row);
				this.flagTypeEdit = true;
			}
			else this.SelectedSchedule = null;
			this.UpdateTimeDictionary();
			this.OnPropertyChanged("SelectedSchedule");
		}

		/// <summary>
		/// обработчик команды добавления расписания
		/// </summary>
		/// <param name="param"></param>
		private void AddTimeExecute(object param)
		{
			this.flagTypeEdit = false;
			this.OnChangeItem(null);
		}

        /// <summary>
        /// обработчик команды добавления времени
        /// </summary>
        /// <param name="param">параметр</param>
        private void SaveTimeExecute(object param)
        {
            // поолучаем три параметра и проверяем их на правильность
			IList<object> paramList = param as IList<object>;
			if (paramList == null)
			{
				throw new ArgumentException("TripScheduleAdd params");
			}

			// список городов и времени отправления
			IEnumerable<TownTimePair> list = paramList[0] as IEnumerable<TownTimePair>;
			
			// дата расписания (для одноразовых)
			DateTime date = paramList[3] != null ? (DateTime)paramList[3] : DateTime.MinValue;

			if (this.SelectedTrip == null)
			{
				UtilManager.Instance.MessageProvider.ShowInformationWindow("Не выбран маршрут!");
				return;
			}

			if (paramList[1] == null)
			{
				UtilManager.Instance.MessageProvider.ShowInformationWindow("Не задан тип расписания!");
				return;
			}

			// получаем тип расписания из комбобокса (передается как параметр)
			TripScheduleType scheduleType = (TripScheduleType)Ct.GetEnumFromDescription(typeof(TripScheduleType), paramList[1].ToString());

			if (paramList[2] == null)
			{
				UtilManager.Instance.MessageProvider.ShowInformationWindow("Не задан автообус!");
				return;
			}

			// получаем автобус
			BusVM bus = paramList[2] as BusVM;

			// проверяем на прваильность для одноразовых раписаний
			if (paramList[2] == null && scheduleType == TripScheduleType.Once)
			{
				UtilManager.Instance.MessageProvider.ShowInformationWindow("Для одноразового типа расписания необходимо задать дату!");
				return;
			}
            
			if (list != null && list.Count() > 0)
            {
                List<object> row = new List<object>();
                List<String> timeList = list.Select(p => p.Time).ToList();

                this.CreateMatrix();

                // формируем список времен выезда из первого города маршрута
                List<String> depTimeList = new List<String>();
                foreach (DataRow item in this.TimeMatrix.Rows)
                {
                    depTimeList.Add(item[0].ToString());
                }

				// проверка на то, чтобы время задавалось хотя бы для одного города
				if (timeList.All(t => String.IsNullOrEmpty(t)))
				{
					UtilManager.Instance.MessageProvider.ShowInformationWindow("Время отправления не задано ни для одного из городов!");
					return;
				}

				// проверка на задание времени расписания первого города
				if (String.IsNullOrEmpty(timeList.First()))
				{
					UtilManager.Instance.MessageProvider.ShowInformationWindow("Время отправления для города отправления должно быть задано!");
					return;
				}

                // проверяем новое расписание на то, существует ли оно уже в текущем расписании
				String fTime = list.Select(p => p.Time).First();
                if (depTimeList.Contains(fTime) && (this.SelectedSchedule != null ? this.SelectedSchedule.Time != fTime : false))
                {
					UtilManager.Instance.MessageProvider.ShowInformationWindow("Такое время выезда автобуса уже существует в расписании!");
                    return;
                }

                // проверяем расписание на предмет упорядоченности времени выезда
				IEnumerable<String> tmpList = timeList.Where(t => !String.IsNullOrEmpty(t));
				if (!tmpList.OrderBy(i => i).ArrayEquals(tmpList) || tmpList.Count() != tmpList.Distinct().Count())
                {
					UtilManager.Instance.MessageProvider.ShowInformationWindow("Время выезда из следующего пункта должно быть больше, чем из предыдущего!");
                    return;
                }

				IList<StationSchedule> ssList = new List<StationSchedule>();

				// если расписание добавляют, то создаем новые расписания и добавляем их в базу
				if (!this.flagTypeEdit)
				{
					// создаем ТС для нового расписания
					TripSchedule ts = new TripSchedule()
					{
						Trip = this.SelectedTrip.trip,
						ScheduleType = scheduleType,
						Bus = bus.bus,
						StartTime = DateTime.MinValue,
						EndTime = DateTime.MaxValue
					};

					// выполняем функции севриса для дообавления списка расписаний
					FuncExec.Execute(() =>
						{
							// получаем ид ТС
							// создаем расписания для этого ТС
							ts.Id = UtilManager.Instance.Client.TripScheduleAdd(ts);
							ssList = list.Where(p => !String.IsNullOrEmpty(p.Time)).Select(pair => new StationSchedule()
							{
								TS = ts,
								Town = pair.Town.town,
								DepartureTime = scheduleType == TripScheduleType.Once ? date.SetTime(Ct.GetTimeFromTimeString(pair.Time)) : Ct.GetTimeFromTimeString(pair.Time)
							}).ToList();

							// добавляем их в базу данных
							foreach (var ss in ssList)
							{
								ss.Id = UtilManager.Instance.Client.StationScheduleAdd(ss);
							};
							row.AddRange(timeList);
							this.TimeMatrix.Rows.Add(row.ToArray());
							this.OnPropertyChanged("TimeMatrix");
							this.LoadDataExecute();

							this.SelectedSchedule = new StationScheduleVM(ssList.FirstOrDefault());
							this.OnChangeItem(this.SelectedSchedule);
							UtilManager.Instance.MessageProvider.ShowInformationWindow("Расписание сохранено");
						});
				}
				else
				{
					// иначе находим изменяемое расписание, задаем ему время, тип и автобус
					FuncExec.Execute(() =>
						{
							this.SelectedSchedule.TS.ScheduleType = scheduleType;
							this.SelectedSchedule.TS.Bus = bus;
							UtilManager.Instance.Client.TripScheduleEdit(this.SelectedSchedule.TS.ts);
												
							// изменяем расписания
							//StationScheduleVM ss;
							//foreach (var p in list)
							//{
							//    ss = this.Shedules.FirstOrDefault(s => s.TS.Equals(this.SelectedSchedule.TS) && s.Town.Equals(p.Town));
							//    if (String.IsNullOrEmpty(p.Time))
							//    {
							//        UtilManager.Instance.Client.StationScheduleDelete(ss.ss);
							//    }
							//    else
							//    {
							//        ss.DepartureTime = scheduleType == TripScheduleType.Once ? date.SetTime(Ct.GetTimeFromTimeString(p.Time)) : Ct.GetTimeFromTimeString(p.Time);
							//        UtilManager.Instance.Client.StationScheduleEdit(ss.ss);
							//    }
							//};
							this.LoadDataExecute();
							//this.UpdateTimeMatrix();

							this.OnChangeItem(this.SelectedSchedule);
							UtilManager.Instance.MessageProvider.ShowInformationWindow("Расписание сохранено");
						});
				}
            }
        }

		/// <summary>
		/// проверка добавления расписания
		/// </summary>
		/// <param name="param">параметр</param>
		private bool AddTimeCanExecute(object param)
		{
			return this.SelectedTrip != null;
		}

		/// <summary>
		/// проверка удаления времени
		/// </summary>
		/// <param name="param">параметр</param>
		private bool DeleteTimeCanExecute(object param)
		{
			DataRow row = param as DataRow;
			if (row == null) return false;
			return this.GetCurrentTS(row) != null;
		}

        /// <summary>
        /// обработчик команды удаления времени
        /// </summary>
        /// <param name="param">параметр</param>
        private void DeleteTimeExecute(object param)
        {
			DataRow row = param as DataRow;
            if (row != null)
            {
				TripScheduleVM ts = this.GetCurrentTS(row);
				if (ts != null)
				{
					if (UtilManager.Instance.MessageProvider.ShowYesNoDialogWindow("Удалить расписание?"))
					{
						FuncExec.Execute(() =>
						{
							// удалит все расписания для этой ТС автоматически
							UtilManager.Instance.Client.TripScheduleDelete(ts.ts);
							UtilManager.Instance.UpdateModifiedObject(this.ModifiedObj);

							this.TimeMatrix.Rows.Remove(row);
							this.OnPropertyChanged("TimeMatrix");
							this.LoadDataExecute();

							this.OnChangeItem(null);
						});
					}
				}								
            }
        }

		/// <summary>
		/// получает текущий ключ - ТС по выделенной строке в матрице времени
		/// </summary>
		/// <param name="row">строка</param>
		/// <returns>тс</returns>
		public TripScheduleVM GetCurrentTS(DataRow row)
		{
			if (row != null && row.ItemArray.Count() > 0)
            {
				StationScheduleVM schedule = this.Shedules.Where(s => s.DepartureTime.GetTimeString().Equals(row[0])).FirstOrDefault(ss => ss.TS.Trip.Equals(this.SelectedTrip));
				return schedule != null ? schedule.TS : null;
			}
			return null;
		}

        /// <summary>
        /// получает выделенное расписание
        /// </summary>
        /// <param name="row">строка</param>
        /// <returns>тс</returns>
        public StationScheduleVM GetCurrentFirstSS(DataRow row)
        {
            TripScheduleVM ts = this.GetCurrentTS(row);
            if (ts != null)
            {
                return this.Shedules.FirstOrDefault(ss => ss.TS.Equals(ts) && ss.TS.Trip.Equals(this.SelectedTrip));               
            }
            return null;
        }

        /// <summary>
        /// обработчик команды для обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected override void UpdateData(object param)
        {
            this.LoadData();
        }

        /// <summary>
        /// обновление расписаний
        /// </summary>
        protected override void RefreshItems()
        {
            base.RefreshItems();
            this.OnPropertyChanged("Shedules");
        }
    }

}
