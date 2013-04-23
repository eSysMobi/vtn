using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VlastelinClient.Commands;
using VlastelinClient.Util;
using System.Deployment.Application;
using Vlastelin.Common;
using VlastelinClient.Windows;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using System.ServiceModel;
using VlastelinClient.ViewModel.WindowsViewModel;
using Reports.Classes;
using System.Data;
using System.Xml;
using System.Reflection;
using VlastelinClient.ServiceReference1;
using System.Diagnostics;
using Vlastelin.Data.Model.ExportedData;
using System.IO;
using System.Net;

namespace VlastelinClient.ViewModel
{
    /// <summary>
    /// вьюмодель главного окна программы, содержит остальные элементы
    /// </summary>
    public class MainWindowVM : BaseViewModel
	{
		#region Индикаторы загрузки данных

		protected bool isBusy;
		protected String _busyContent;

		/// <summary>
		/// для индикатора загрузки
		/// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }
            set
            {
                this.isBusy = value;
                this.OnPropertyChanged("IsBusy");
            }
        }

		public String BusyContent
		{
			get
			{
				return this._busyContent;
			}
			set
			{
				this._busyContent = value;
				this.OnPropertyChanged("BusyContent");
			}
		}

		public bool IsWindowEnabled
		{
			get
			{
				return UtilManager.Instance.StateManager.IsWindowEnabled;
			}
		}

		public bool IsServerEnabled
		{
			get
			{
				return UtilManager.Instance.StateManager.IsServerEnabled;
			}
		}

		/// <summary>
		/// если залогинен администратор
		/// </summary>
		public bool FlagIsAdmin
		{
			get
			{
				return this.CurrentOperator.Role == Roles.Administrator;
			}
		}

		/// <summary>
		/// строка состояния для серверса и ККМ
		/// </summary>

		public String ServerState
		{
			get
			{
				//this.OnCatalogChanged("IsWindowEnabled");
				//this.OnCatalogChanged("IsServerEnabled");
				return UtilManager.Instance.StateManager.ServerStateString;
			}
		}

		public String KKMState
		{
			get
			{
				return UtilManager.Instance.StateManager.KKMStateString;
			}
		}

		public String VersionState { get; set; }

		#endregion

        #region Объекты вьюмоделей

        /// <summary>
        /// вьюмодель окна городов
        /// </summary>
        public TownWindowVM townVM { get; private set; }

        /// <summary>
        /// вьюмодель окна автобусов
        /// </summary>
        public BusWindowVM busVM { get; private set; }

        /// <summary>
        /// вьюмодель для окна водителей
        /// </summary>
        public DriverWindowVM driverVM { get; private set; }

        /// <summary>
        /// вьюмодель для окна владельцев
        /// </summary>
        public OwnerWindowVM ownerVM { get; private set; }

        /// <summary>
        /// вьюмодель для туров
        /// </summary>
        public TripWindowVM tripVM { get; private set; }

        /// <summary>
        /// вьюмодель для филиалов
        /// </summary>
        public BranchWindowVM branchVM { get; private set; }

        /// <summary>
        /// вьюмодель для расписания
        /// </summary>
        public TripSheduleWindowVM tripSheduleVM { get; private set; }

        /// <summary>
        /// вьюмодель для формы отправления автобуса в рейс
        /// </summary>
        public BusDepWindowVM busDepVM { get; private set; }

		/// <summary>
		/// вьюмодель для окна отправленных автобусов
		/// </summary>
		public DeparturedBusesWindowVM departuredBusesVM { get; set; }

        /// <summary>
        /// вьюмодель для операторов
        /// </summary>
        public OperatorWindowVM operatorVM { get; private set; }

        /// <summary>
        /// вьюмодель для мест
        /// </summary>
        public SeatTableVM seatVM { get; private set; }

		/// <summary>
		/// вьюмодель для окна обмена билета
		/// </summary>
		public ChangeTicketWindowVM changeTicketVM { get; private set; }

		/// <summary>
		/// вьюмодель для окна смены автобуса
		/// </summary>
		public ChangeBusWindowVM changeBusVM { get; private set; }

		/// <summary>
		/// вьюмодель для отчетов
		/// </summary>
		public ReportVM reportVM { get; set; }

        #endregion

        #region Свойства

        /// <summary>
        /// текущий оператор системы
        /// </summary>
		public Operator CurrentOperator
		{
			get
			{
				return UtilManager.Instance.CurrentOperator;
			}
		}

        /// <summary>
        /// выделенный подмаршрут
        /// </summary>
        private TripPriceVM _selectedTripPrice;
        public TripPriceVM SelectedTripPrice
        {
            get
            {
                return this._selectedTripPrice;
            }
            set
            {
                this._selectedTripPrice = value;
                this.OnPropertyChanged("SelectedTripPrice");
            }
        }

		/// <summary>
		/// для показа подмаршрутов в комбобоксе
		/// </summary>
		public TripVM SelectedTrip { get; set; }
		public IEnumerable<TownVM> RouteTowns1 { get; set; }
		public IEnumerable<TownVM> RouteTowns2 { get; set; }

		/// <summary>
		/// заголовок окна
		/// </summary>
		public String WindowTitle { get; set; }

        #endregion

		#region Списки

		/// <summary>
		/// список видов дополнительных услуг
		/// </summary>
		private IEnumerable<SalesKind> SalesKinds;

		/// <summary>
		/// список маршрутов, используется при продаже билетов
		/// </summary>
		//private ObservableCollection<TripVM> _trips;
		public ObservableCollection<TripVM> Trips
		{
			get
			{
				return this.tripVM != null ? this.tripVM.Trips : null;
			}
		}

		/// <summary>
		/// список туров, используется при продаже билетов
		/// </summary>
		public ObservableCollection<StationScheduleVM> TripShedules
		{
			get
			{
				return this.tripSheduleVM != null ? this.tripSheduleVM.Shedules : null;
			}
		}

		#endregion

		#region События

		/// <summary>
		/// для обновления данных
		/// </summary>
        public event PropertyChangedEventHandler CatalogChanged;

        protected virtual void OnCatalogChanged(string propertyName)
        {
            if (this.CatalogChanged != null)
            {
                this.CatalogChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

		#endregion

        #region Команды

		/// <summary>
		/// команды для меню и иных действий
		/// </summary>
        public ICommand CatalogTownsCommand
        {
            get 
            {
                return new RelayCommand(this.CatalogTownsExecute, this.CatalogCanExecute);
            }
        }

        public ICommand CatalogBusesCommand
        {
            get
            {
				return new RelayCommand(this.CatalogBusesExecute, this.CatalogCanExecute);
            }
        }

        public ICommand CatalogDriversCommand
        {
            get
            {
				return new RelayCommand(this.CatalogDriversExecute, this.CatalogCanExecute);
            }
        }

        public ICommand CatalogOwnersCommand
        {
            get
            {
				return new RelayCommand(this.CatalogOwnersExecute, this.CatalogCanExecute);
            }
        }

        public ICommand CatalogTripsCommand
        {
            get
            {
				return new RelayCommand(this.CatalogTripsExecute, this.CatalogCanExecute);
            }
        }

        public ICommand CatalogBranchesCommand
        {
            get
            {
				return new RelayCommand(this.CatalogBranchesExecute, this.CatalogCanExecute);
            }
        }

        public ICommand CatalogTripShedulesCommand
        {
            get
            {
				return new RelayCommand(this.CatalogTripShedulesExecute, this.CatalogCanExecute);
            }
        }

        public ICommand BusDepartureCommand
        {
            get
            {
				return new RelayCommand(this.BusDepartureExecute, this.BusDepartureCanExecute);
            }
        }

		public ICommand DeparturedBusesCommand
		{
			get
			{
				return new RelayCommand(this.DeparturedBusesExecute, this.DeparturedBusesCanExecute);
			}
		}

        public ICommand CatalogOperatorsCommand
        {
            get
            {
				return new RelayCommand(this.CatalogOperatorsCommandExecute, this.CatalogCanExecute);
            }
        }

        public ICommand ShowSettingsCommand
        {
            get
            {
                return new RelayCommand(this.ShowSettingsExecute);
            }
        }

        public ICommand ShowKKMSettingsCommand
        {
            get
            {
				return new RelayCommand(this.ShowKKMSettingsExecute, this.ShowKKMSettingsCanExecute);
            }
        }

		public ICommand SessionReportCommand
		{
			get
			{
				return new RelayCommand(this.SessionReportExecute, this.SessionReportCanExecute);
			}
		}

        public ICommand CloseSessionCommand
        {
            get
            {
				return new RelayCommand(this.CloseSessionExecute, this.CloseSessionCanExecute);
            }
        }

        public ICommand ResetKKMStateCommand
        {
            get
            {
                return new RelayCommand(this.ResetKKMStateExecute, this.ResetKKMStateCanExecute);
            }
        }

		public ICommand SeatSellCommand
		{
			get
			{
				return new RelayCommand(this.SeatSellExecute, this.SeatSellCanExecute);
			}
		}
		public ICommand SeatReserveCommand
        {
            get
            {
				return new RelayCommand(this.SeatReserveExecute, this.SeatReserveCanExecute);
            }
        }

		public ICommand SeatUnReserveCommand
		{
			get
			{
				return new RelayCommand(this.SeatUnReserveExecute, this.SeatUnReserveCanExecute);
			}
		}

		public ICommand SeatReturnCommand
		{
			get
			{
				return new RelayCommand(this.SeatReturnExecute, this.SeatReturnCanExecute);
			}
		}

		//public ICommand ChangeTicketCommand
		//{
		//    get
		//    {
		//        return new RelayCommand(this.ChangeTicketExecute, this.ChangeTicketCanExecute);
		//    }
		//}

		public ICommand ChangeBusCommand
		{
			get
			{
				return new RelayCommand(this.ChangeBusExecute, this.ChangeBusCanExecute);
			}
		}

        public ICommand AdditionalServicesCommand
        {
            get
            {
				return new RelayCommand(this.AdditionalServicesExecute, this.AdditionalServicesCanExecute);
            }
        }

        public ICommand TurnOnKKMCommand
        {
            get
            {
                return new RelayCommand(this.TurnOnKKMExecute, this.TurnOnKKMCanExecute);
            }
        }

		public ICommand DepositMoneyCommand
		{
			get
			{
				return new RelayCommand(this.DepositMoneyExecute, this.DepositMoneyCanExecute);
			}
		}

		public ICommand WithdrawMoneyCommand
		{
			get
			{
				return new RelayCommand(this.WithdrawMoneyExecute, this.WithdrawMoneyCanExecute);
			}
		}

        public ICommand ConnectToServerCommand
        {
            get
            {
                return new RelayCommand(this.ConnectToServerExecute);
            }
        }

        public ICommand ExportRKOCommand
        {
            get
            {
                return new RelayCommand(this.ExportRKOExecute, this.ExportRKOCanExecute);
            }
        }

        public ICommand ExporPKOCommand
        {
            get
            {
                return new RelayCommand(this.ExportPKOExecute, this.ExportPKOCanExecute);
            }
        }

		public ICommand ListRKOCommand
        {
            get
            {
				return new RelayCommand(this.ListRKOExecute, this.ListRKOCanExecute);
            }
        }

		public ICommand SendErrorReportCommand
		{
			get
			{
				return new RelayCommand(this.SendErrorReportExecute);
			}
		}

		public ICommand ShowAboutWindowCommand
		{
			get
			{
				return new RelayCommand(this.ShowAboutWindowExecute);
			}
		}	
   
        #endregion

		#region Конструкторы и инициализация

		public MainWindowVM()
        {
            this.Init();
        }

        /// <summary>
        /// инициализация вьюмоделей
        /// </summary>
        private void Init()
        {
            this.ownerVM = new OwnerWindowVM();
            this.townVM = new TownWindowVM();
            this.branchVM = new BranchWindowVM(this.townVM);
            this.operatorVM = new OperatorWindowVM(this.branchVM);
            this.busVM = new BusWindowVM(this.ownerVM);
            this.driverVM = new DriverWindowVM(this.ownerVM);
            this.tripVM = new TripWindowVM(this.townVM);
			this.tripSheduleVM = new TripSheduleWindowVM(this.tripVM, this.busVM);
            this.seatVM = new SeatTableVM();
            this.busDepVM = new BusDepWindowVM();
			this.departuredBusesVM = new DeparturedBusesWindowVM();
			this.changeTicketVM = new ChangeTicketWindowVM();
			this.changeBusVM = new ChangeBusWindowVM();
            
            this.ownerVM.driverVM = this.driverVM;
			UtilManager.Instance.StateManager.StateChanged += new PropertyChangedEventHandler(StateChanged);

			this.reportVM = new ReportVM(this);
			ReportsExecutor.IntervalFrom = DateTime.Today;
			ReportsExecutor.IntervalTo = DateTime.Today.AddYears(1);
        }

		#endregion

		#region Начальная загрузка данных

		/// <summary>
		/// проверка версии приложения
		/// </summary>
		public void CheckApplicationVersion()
		{
			// если в конфигах установлено, что версию приложения не проверять, то выходим
			if (!Properties.Settings.Default.FlagCheckApplicationVersion)
			{
				return;
			}

			try
			{
				String versionCurrent = UtilManager.Instance.CurrentAppVersion.ToString();
				this.VersionState = versionCurrent;

				var webClient = new System.Net.WebClient();
				string html = webClient.DownloadString("http://office24.pro/vlastelin-client/VlastelinClient.application");
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(html);

				String versionAvailable = doc.ChildNodes[1]["assemblyIdentity"].Attributes["version"].Value;

				IList<int> vAvailable = versionAvailable.Split('.').Select(s => int.Parse(s)).ToList();
				IList<int> vCurrent = versionCurrent.Split('.').Select(s => int.Parse(s)).ToList();

				// 4 количество цифр в номере версии, например 1.0.0.10
				// сравниваем текущую версию и доступную
				for (int i = 0; i < 4; i++)
				{
					if (vAvailable[i] > vCurrent[i])
					{
						this.VersionState = String.Format("{0} (доступно обновление до версии {1})", versionCurrent, versionAvailable);
						break;
					}
				}
			}
			catch (Exception ex)
			{
				UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
				this.VersionState = "ошибка при определении версии";
			}
			this.OnPropertyChanged("VersionState");
		}

		/// <summary>
        /// загрузка дополнительных списков - видов услуг, настроек итд
        /// </summary>
		public void LoadInitDataSettings()
        {
			UtilManager.Instance.ServerSettings = UtilManager.Instance.Client.MainSettingsGet();
			this.SalesKinds = UtilManager.Instance.Client.SalesKindsGet(null);

			String serverType = UtilManager.Instance.Client.Endpoint.Address.Uri.Host.ToLower().Contains("localhost") ? "(Тестовый сервер)" : String.Empty;
			this.WindowTitle = String.Format("Властелин {0}", serverType);
        }

		/// <summary>
		/// загрузка маршрутов
		/// </summary>
		public void LoadInitDataTrips()
		{
			this.tripVM.LoadDataExecute();
		}

		/// <summary>
		/// загрузка расписания
		/// </summary>
		public void LoadInitDataTripSchedules()
		{
			this.tripSheduleVM.LoadData();
			foreach (TripVM trip in this.Trips)
			{
				trip.SetHasSchedule(this.TripShedules);
			}
		}

		/// <summary>
		/// загрузка расписания
		/// </summary>
		public void LoadInitDataSearchKKM()
		{
			// подключение ККМ и создание объекта
            UtilManager.Instance.KKMManager.CreateKKMObject();
			
            // проверка на состояние ККМ
			if (UtilManager.Instance.StateManager.KKMState == KKMStates.Disabled || UtilManager.Instance.StateManager.KKMState == KKMStates.Faulted)
            {
                DispatchService.Invoke(() =>
                {
                    UtilManager.Instance.CloseSplash();
                    if (!UtilManager.Instance.MessageProvider.ShowYesNoDialogWindow("Возникла ошибка при подключении ККМ! Продолжить дальнейшую работу программы?"))
                    {
                        Application.Current.Shutdown();
                    }
                    UtilManager.Instance.ShowSplash();
                });
				return;
            }

            // проверка соответствия текущего времени и времени ККМ
			if (!UtilManager.Instance.KKMManager.IsKKMTimeCorrect())
			{
				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.CloseSplash();
					
                    // если время расходится, устанавливаем его в зависимости от выбора пользователя
                    if (UtilManager.Instance.MessageProvider.ShowYesNoDialogWindow("Возникло расхождение между текущим временем и временем ККМ. Установить время ККМ?"))
					{
						UtilManager.Instance.KKMManager.InternalTime = DateTime.Now;
					}
					UtilManager.Instance.ShowSplash();
				});
			}

            // проверяем состояние ККМ на предмет, закрыта ли 24часовая смена
			if (UtilManager.Instance.KKMManager.IsKKMSessionClosed())
			{
				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.CloseSplash();
					
                    // закрываем смену или выходим из программы в зависимости от выбора пользователя
                    if (!UtilManager.Instance.MessageProvider.ShowYesNoDialogWindow("Смена ККМ не закрыта, работа ККМ не может быть продолжена. Закрыть смену или выйти из программы?"))
					{
						Application.Current.Shutdown();
					}
					this.CloseSessionExecute(null);
					UtilManager.Instance.ShowSplash();
				});
			}
		}

		#endregion

		/// <summary>
		/// обработчик события изменения состояния сервера или ккм
		/// </summary>
		/// <param name="propertyName">название свойства</param>
		private void StateChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName);
		}
		/// <summary>
        /// функция для вызова каталога с показом сплэш-скрина(унифицированная)
        /// </summary>
        /// <param name="wnd">окно каталога</param>
        /// <param name="loadProcess">функция загрузки данных</param>
        /// <param name="vm">вьюмодель каталога</param>
        public void CatalogCommonExecute(Window wnd, Action loadProcess, BaseViewModel vm, Action updateDataAction = null)
        {
            // активируем класс для работы с фоновым потоком
            BackgroundWorker worker = new BackgroundWorker();

            // процесс, работающий в фоновом режиме
            worker.DoWork += (o, ea) =>
            {
                // показываем сплэш-скрин в потоке UI-интерфейса
                DispatchService.Invoke(() =>
                {
                    UtilManager.Instance.ShowSplash();
                });

				// загружаем данные
                loadProcess();

                // записываем полученные данные в датаконтест окна
                DispatchService.Invoke(() =>
                {
                    wnd.DataContext = vm;
                });
            };

            // обработчик по окончанию фонового процесса
            worker.RunWorkerCompleted += (o, ea) =>
            {
                // закрывает сплэш-скрин и показывает диалоговое окно
				DispatchService.Invoke(() => 
				{ 
					UtilManager.Instance.CloseSplash();
				});

                if (ea.Error == null)
                {
                    if (wnd.ShowDialog() == true)
                    {
                        // обновляем список полученных объектов
                        if (updateDataAction != null)
                        {
                            DispatchService.Invoke(updateDataAction);
                        }
                    }

                    // стартуем таймер обновления данных
                    UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
                }
                else
                {
                     DispatchService.Invoke(() => FuncExec.ProcessException(ea.Error));
                }
            };

			// останавливаем таймер обновления данных и опроса сервера
			UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

            // стартуем фоновый процесс
            worker.RunWorkerAsync();
        }

		#region Каталоги
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик команды показа окна городов
		/// </summary>
		/// <param name="param">не используется</param>
		public bool CatalogCanExecute(object param)
		{
			return this.IsServerEnabled;
		}

        /// <summary>
        /// обработчик команды показа окна городов
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogTownsExecute(object param)
        {
            WindowTowns window = new WindowTowns();
            this.CatalogCommonExecute(window, () => townVM.LoadData(), this.townVM);
        }

        /// <summary>
        /// обработчик команды показа окна автобусов
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogBusesExecute(object param)
        {
            WindowBusesNew window = new WindowBusesNew();
            this.CatalogCommonExecute(window, () =>
            {
                ownerVM.LoadData();
                busVM.LoadData();
            },
            this.busVM);
        }

        /// <summary>
        /// обработчик команды показа окна водителей
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogDriversExecute(object param)
        {          
            WindowDriver window = new WindowDriver();
            this.CatalogCommonExecute(window, () =>
                {
                    this.ownerVM.LoadData();
                    this.driverVM.LoadData();
                }, this.driverVM);
        }

        /// <summary>
        /// обработчик команды показа окна владельцев
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogOwnersExecute(object param)
        {         
            WindowOwners window = new WindowOwners();
            this.CatalogCommonExecute(window, () =>
                {
                    this.driverVM.LoadData();
                    this.ownerVM.LoadData();
                }, ownerVM);         
        }

        /// <summary>
        /// обработчик команды показа окна туров
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogTripsExecute(object param)
        {          
            WindowTrips window = new WindowTrips();
            this.CatalogCommonExecute(window, () =>
                {
                    this.townVM.LoadData();
                    this.tripVM.LoadDataExecute();
                }, 
                tripVM,
                () => this.OnCatalogChanged("Trips"));
        }

        /// <summary>
        /// обработчик команды показа окна филиалов
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogBranchesExecute(object param)
        {
            WindowBranches window = new WindowBranches();
            this.CatalogCommonExecute(window, () =>
            {
                this.townVM.LoadData();
                this.branchVM.LoadData();
            },
            branchVM); 
        }

        /// <summary>
        /// обработчик команды показа окна расписания
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogTripShedulesExecute(object param)
        {
            WindowTripShedules window = new WindowTripShedules();
            this.CatalogCommonExecute(window, () =>
                {
                    this.busVM.LoadData();
					this.tripVM.LoadDataExecute();
                    this.tripSheduleVM.LoadData();
                }, 
                tripSheduleVM,
                () => this.OnCatalogChanged("TripShedules"));
        }

        /// <summary>
        /// обработчик команды показа окна операторов
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogOperatorsCommandExecute(object param)
        {
            WindowOperators window = new WindowOperators();
            this.CatalogCommonExecute(window, () =>
                {
                    this.branchVM.LoadData();
                    this.operatorVM.LoadData();
                }, 
                operatorVM);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Отправка автобуса
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обрабочик команды отправки автобуса в рейс
        /// </summary>
        /// <param name="param">не используется</param>
        private void BusDepartureExecute(object param)
        {
            BusDepartureWindow window = new BusDepartureWindow();
			StationScheduleVM ss = param as StationScheduleVM;

            this.CatalogCommonExecute(window, () =>
            {
                this.busDepVM.SS = ss;
				this.busDepVM.Sum = this.seatVM.SeatsSoldSum;
				this.busDepVM.SeatsTakenCount = this.SeatsTakenCount;
				this.busDepVM.LoadData();
            }, 
            busDepVM);
        }

		/// <summary>
		/// проверка команды отправки автобуса в рейс
		/// </summary>
		/// <param name="param"></param>
		private bool BusDepartureCanExecute(object param)
		{
			return 
				param != null && param is StationScheduleVM &&
				this.IsServerEnabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Отмена отправки автобусов
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
        /// обрабочик команды показа списка отправленных автобусов
        /// </summary>
        /// <param name="param">не используется</param>
		private void DeparturedBusesExecute(object param)
        {
            WindowDeparturedBuses window = new WindowDeparturedBuses();

            this.CatalogCommonExecute(window, () =>
            {
                this.departuredBusesVM.DepartureDate = DateTime.Today;
				this.departuredBusesVM.LoadData();
            },
			this.departuredBusesVM);
        }

		/// <summary>
		/// обрабочик команды показа списка отправленных автобусов
		/// </summary>
		/// <param name="param">не используется</param>
		private bool DeparturedBusesCanExecute(object param)
		{
			return this.IsServerEnabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion	

		#region Параметры организации
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды показа настроек организации
        /// </summary>
        /// <param name="param"></param>
        public void ShowSettingsExecute(object param)
        {
            WindowServerSettings window = new WindowServerSettings();
            window.Settings = UtilManager.Instance.ServerSettings;

			window.ShowDialog();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Настройки ККМ
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды показа настроек ККМ
        /// </summary>
        /// <param name="param"></param>
        public void ShowKKMSettingsExecute(object param)
        {
            if (!UtilManager.Instance.KKMManager.IsKKMEnabled)
			{
				UtilManager.Instance.MessageProvider.ShowErrorWindow("ККМ не подключен");
			}
			else
			{
				UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();
				WindowKKMSettings window = new WindowKKMSettings();
				window.ShowDialog();
				UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
			}
        }

		/// <summary>
        /// проверка команды показа настроек ККМ
        /// </summary>
        /// <param name="param"></param>
		private bool ShowKKMSettingsCanExecute(object param)
		{
			return 
                UtilManager.Instance.StateManager.KKMState == KKMStates.Enabled || 
                UtilManager.Instance.StateManager.KKMState == KKMStates.UnClosedSession;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Сменный отчет без гашения
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик команды сменного отчета без гашения
		/// </summary>
		/// <param name="param"></param>
		private void SessionReportExecute(object param)
		{
			FuncExec.Execute(() =>
			{
				UtilManager.Instance.KKMManager.KKM_SessionReport();
			});
		}

		/// <summary>
		/// проверка команды сменного отчета без гашения
		/// </summary>
		/// <param name="param">не используется</param>
		private bool SessionReportCanExecute(object param)
		{
			return UtilManager.Instance.StateManager.KKMState == KKMStates.Enabled;
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Закрытие смены
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды закрытия сессии
        /// </summary>
        /// <param name="param"></param>
        private void CloseSessionExecute(object param)
        {
			UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();
			
			FuncExec.Execute(() => 
			{
				// печатается справка кассира-операциониста
				ReportsExecutor.Report_CashierBill();

				// создается и печатается приходный кассовый ордер и добавляется в бд
				UtilManager.Instance.NumbersManager.SetLastNumber();
				PKO pko = new PKO()
				{
					Operator = UtilManager.Instance.CurrentOperator,
					DocDate = DateTime.Today,
					DocNum = UtilManager.Instance.NumbersManager.Number,
					Sum = (double)UtilManager.Instance.KKMManager.TotalRevenue
				};
				UtilManager.Instance.Client.PKOAdd(pko);
				ReportsExecutor.ReportIncomeCashOrder(pko);

				// печатается акт о возврате денежных средств
				DataTable table = UtilManager.Instance.Client.ReportGet(ReportTypes.ReturnedTickets, DateTime.Today, DateTime.Today, null, null);
				ReportsExecutor.ReportReturnAct(table, DateTime.Today);

				// на ККМ печатается Z-отчет закрытия смены
				UtilManager.Instance.KKMManager.KKM_CloseSession();				
			});

			UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
        }

		/// <summary>
		/// проверка команды закрытия сессии
		/// </summary>
		/// <param name="param">не используется</param>
		private bool CloseSessionCanExecute(object param)
		{
			return 
				this.IsServerEnabled && 
				(UtilManager.Instance.StateManager.KKMState == KKMStates.Enabled || UtilManager.Instance.StateManager.KKMState == KKMStates.UnClosedSession);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion

        #region Сброс состояния ККМ
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды сброса состояния ККМ
        /// </summary>
        /// <param name="param"></param>
        private void ResetKKMStateExecute(object param)
        {
            UtilManager.Instance.KKMManager.KKM_Reset();
        }

        /// <summary>
        /// проверка команды сброса состояния ККМ
        /// </summary>
        /// <param name="param"></param>
        private bool ResetKKMStateCanExecute(object param)
        {
            return 
                UtilManager.Instance.KKMManager.IsKKMEnabled &&
                Properties.Settings.Default.FlagUseKKM;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion		

		#region Дополнительные услуги
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды показал окна дополнительных сервисов
        /// </summary>
        /// <param name="param">не используется</param>
        private void AdditionalServicesExecute(object param)
        {
			if (!UtilManager.Instance.KKMManager.IsKKMEnabled)
			{
				UtilManager.Instance.MessageProvider.ShowErrorWindow("ККМ не подключен. Продажа услуг недоступна");
				return;
			}

			UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

			int cc = int.Parse(param.ToString());
			WindowAdditionalServices window = new WindowAdditionalServices();
            
			window.SalesKinds = this.SalesKinds;
			window.CurrentKind = cc == 0 ? this.SalesKinds.FirstOrDefault(s => s.Name.Equals("Туалет")) : this.SalesKinds.FirstOrDefault(s => s.Name.Equals("Камера хранения"));
            window.ShowDialog();

			UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
        }

		/// <summary>
        /// проверка команды показал окна дополнительных сервисов
        /// </summary>
        /// <param name="param">не используется</param>
		private bool AdditionalServicesCanExecute(object param)
		{
			return 
                this.IsServerEnabled && 
                UtilManager.Instance.StateManager.KKMState == KKMStates.Enabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Подключение ККМ
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды подключения ККМ
        /// </summary>
        /// <param name="param">не используется</param>
        private void TurnOnKKMExecute(object param)
        {
			// активируем класс для работы с фоновым потоком
			BackgroundWorker worker = new BackgroundWorker();

			// процесс, работающий в фоновом режиме
			worker.DoWork += (o, ea) =>
			{
				// показываем сплэш-скрин в потоке UI-интерфейса
				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.ShowSplash();
					UtilManager.Instance.ChangeSplashString("Поиск ККМ...");
				});

				// ищем ККМ
                this.LoadInitDataSearchKKM();
			};

			// обработчик по окончанию фонового процесса
			worker.RunWorkerCompleted += (o, ea) =>
			{
				// закрывает сплэш-скрин и показывает диалоговое окно
				UtilManager.Instance.CloseSplash();

				if (ea.Error == null)
				{
					if (UtilManager.Instance.KKMManager.IsKKMEnabled)
					{
						UtilManager.Instance.MessageProvider.ShowInformationWindow("ККМ подключен");
					}
				}
			};

			// стартуем фоновый процесс
			worker.RunWorkerAsync();				
        }

        /// <summary>
        /// обработчик команды подключения ККМ
        /// </summary>
        /// <param name="param">не используется</param>
        private bool TurnOnKKMCanExecute(object param)
        {
            return Properties.Settings.Default.FlagUseKKM;
        }
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Внесение денег в кассу
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
        /// обработчик команды внесения наличных
        /// </summary>
        /// <param name="param">не используется</param>
        private void DepositMoneyExecute(object param)
        {
            if (!UtilManager.Instance.KKMManager.IsKKMEnabled)
			{
				UtilManager.Instance.MessageProvider.ShowErrorWindow("ККМ не подключен. Функция недоступна");
				return;
			}

			UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();
			WindowDepositMoney window = new WindowDepositMoney();
			if (window.ShowDialog() == true)
			{
				FuncExec.Execute(() =>
				{
					UtilManager.Instance.NumbersManager.SetLastNumber();

					PKO pko = new PKO()
					{
						Operator = UtilManager.Instance.CurrentOperator,
						DocDate = DateTime.Now.Date,
						DocNum = UtilManager.Instance.NumbersManager.Number,
						Sum = window.Sum
					};
					UtilManager.Instance.Client.PKOAdd(pko);
					ReportsExecutor.ReportIncomeCashOrder(pko);

					UtilManager.Instance.KKMManager.KKM_CashIncome(window.Sum);
				});
			}
			UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
        }

		/// <summary>
        /// првоерка команды внесения наличных
        /// </summary>
        /// <param name="param">не используется</param>
		private bool DepositMoneyCanExecute(object param)
		{
			return this.IsServerEnabled && UtilManager.Instance.StateManager.KKMState == KKMStates.Enabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Изъятие денег из кассы
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
        /// обработчик команды изъятия наличных
        /// </summary>
        /// <param name="param">не используется</param>
		private void WithdrawMoneyExecute(object param)
        {
            if (!UtilManager.Instance.KKMManager.IsKKMEnabled)
			{
				UtilManager.Instance.MessageProvider.ShowErrorWindow("ККМ не подключен. Функция недоступна");
				return;
			}

			UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();
			WindowDepositMoney window = new WindowDepositMoney();
			if (window.ShowDialog() == true)
			{
				FuncExec.Execute(() =>
				{
					UtilManager.Instance.KKMManager.KKM_CashOutcome(window.Sum);
				});
			}
			UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
        }

		/// <summary>
		/// првоерка команды внесения наличных
		/// </summary>
		/// <param name="param">не используется</param>
		private bool WithdrawMoneyCanExecute(object param)
		{
			return this.IsServerEnabled && UtilManager.Instance.StateManager.KKMState == KKMStates.Enabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

        #region Подключение к серверу
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды подключения к серверу
        /// </summary>
        /// <param name="param">не используется</param>
        private void ConnectToServerExecute(object param)
        {
			//UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();
            this.ResetConnection();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion	

        #region Экспорт РКО
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды экспорта РКО
        /// </summary>
        /// <param name="param">не используется</param>
        private void ExportRKOExecute(object param)
        {
            UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

            WindowDateInterval window = new WindowDateInterval();
            
            if (window.ShowDialog() == true)
            {
				FuncExec.Execute(() => 
				{ 
					// создаем диалог сохранения файла, забиваем параметры файла РКО
					Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
					dlg.FileName = String.Format("Расчетный кассовый ордер_{0:ddMMyyyy}", DateTime.Today); 
					dlg.DefaultExt = ".txt"; 
					dlg.Filter = "Текстовые файлы (.txt)|*.txt";

					Nullable<bool> result = dlg.ShowDialog();

					// если файл выбран
					if (result == true)
					{
						// получаем строку, содержащую данные рко
						String fileText = ExportUtil.GetExportegRKOFile(UtilManager.Instance.Client.ExportRKO(window.IntervalFrom, window.IntervalTo).ToList());
						// получаем имя файла
						string path = dlg.FileName;

						// сохраняем файл
                        File.WriteAllText(path, fileText, Encoding.GetEncoding(1251));

						UtilManager.Instance.MessageProvider.ShowInformationWindow(String.Format("РКО успешно экспортирован в файл {0}", path));
					}
				});
            }

            UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
        }

        /// <summary>
        /// проверка команды экспорта РКО
        /// </summary>
        /// <param name="param">не используется</param>
        private bool ExportRKOCanExecute(object param)
        {
            return this.IsServerEnabled;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion	

        #region Экспорт ПКО
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды экспорта ПКО
        /// </summary>
        /// <param name="param">не используется</param>
        private void ExportPKOExecute(object param)
        {
            UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

            WindowDateInterval window = new WindowDateInterval();

            if (window.ShowDialog() == true)
            {
            }

            UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
        }

        /// <summary>
        /// проверка команды экспорта ПКО
        /// </summary>
        /// <param name="param">не используется</param>
        private bool ExportPKOCanExecute(object param)
        {
            return this.IsServerEnabled;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion	

		#region Печать РКО
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик команды печати РКО
		/// </summary>
		/// <param name="param">не используется</param>
		private void ListRKOExecute(object param)
		{
			UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

			WindowRKO window = new WindowRKO();
			ListRKOWindowVM vm = new ListRKOWindowVM();

			this.CatalogCommonExecute(window, () => vm.LoadData(), vm);

			UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
		}

		/// <summary>
		/// проверка команды экспорта ПКО
		/// </summary>
		/// <param name="param">не используется</param>
		private bool ListRKOCanExecute(object param)
		{
			return this.IsServerEnabled;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion	

		#region Отчет об ошибке
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды отправки отчета об ошибке
        /// </summary>
        /// <param name="param">не используется</param>
		private void SendErrorReportExecute(object param)
        {
            UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

			try
			{
				WindowEnterMessage window = new WindowEnterMessage();
				if (window.ShowDialog() == true)
				{
					UtilManager.Instance.SendErrorNotification(window.Description);
					UtilManager.Instance.MessageProvider.ShowInformationWindow("Отчет об ошибке успешно отправлен");
				}
			}
			catch (IOException ex)
			{
				UtilManager.Instance.MessageProvider.ShowErrorWindow("Возникла ошибка работы с файлами логов");
				UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
			}
			catch (WebException ex)
			{
				UtilManager.Instance.MessageProvider.ShowErrorWindow("Не удалось выполнить отправку отчета об ошибке. Проблемы при соединении с сервером");
				UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
			}
			catch (Exception ex)
			{
				UtilManager.Instance.MessageProvider.ShowErrorWindow(ClientMsg.ErrorUnknown);
				UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
			}
				
            UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion	
	
		#region О программе
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды отправки отчета об ошибке
        /// </summary>
        /// <param name="param">не используется</param>
		private void ShowAboutWindowExecute(object param)
        {
            UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

			WindowAbout window = new WindowAbout();
			window.ShowDialog();
				
            UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion	

        #region Продажа билетов

        /// <summary>
        /// список мест в автобусе для выбранного тура
        /// </summary>
        private ObservableCollection<SeatVM> _seats;
        public ObservableCollection<SeatVM> Seats 
        {
            get
            {
                return this._seats;
            }
            set
            {
                this._seats = value;
                this.OnPropertyChanged("Seats");
				this.OnPropertyChanged("SeatsTakenCount");
				this.OnPropertyChanged("SeatsSoldSum");
            }
        }

		/// <summary>
		/// количество проданных мест
		/// </summary>
		public int SeatsTakenCount
		{
			get
			{
				return this.Seats != null ? this.Seats.Count(s => s.State != SeatState.Free) : 0;
			}
		}

		/// <summary>
		/// сумма проданных мест
		/// </summary>
		public String SeatsSoldSum
		{
			get
			{
				return String.Format(Ct.RussianCulture, "{0:F2} р.", this.seatVM.SeatsSoldSum);
			}
		}

        /// <summary>
        /// обновление списка мест в датагриде
        /// </summary>
        /// <param name="param">поездка</param>
        public void UpdateSeatsExecute(StationScheduleVM item, TripPriceVM price, DateTime? date)
        {         
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if (date != null && item != null)
                {
                    this.seatVM.LoadData(item, price, date.Value);
                    this.seatVM.UpdateSeatsList(item, price, date.Value);
                }
                else
                {
                    this.seatVM.ListSeats = null;
                }
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.IsBusy = false;
                if (ea.Error != null)
                {
                    DispatchService.Invoke(() => FuncExec.ProcessException(ea.Error));
                }
                else
                {
                    DispatchService.Invoke(() => this.Seats = this.seatVM.ListSeats);
                    UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
                }
            };

            // останавливаем таймер обновления данных и опроса сервера
            UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

            this.IsBusy = true; 
            worker.RunWorkerAsync();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		#endregion

		#region Продажа мест (обработчики команд)

		/// <summary>
        /// продажа билета
        /// </summary>
        /// <param name="seat"></param>
		public void SeatSellExecute(object param)
        {
            //if (!UtilManager.Instance.KKMManager.IsKKMEnabled)
            //{
            //    UtilManager.Instance.MessageProvider.ShowWarningnWindow("ККМ не подключен. Место не может быть продано");
            //    return;
            //}

            UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

			SeatVM seat = param as SeatVM;

			// если место не задано или не пустое, то оно не может быть обработано
			if (seat == null)
			{
				UtilManager.Instance.MessageProvider.ShowInformationWindow("Место не выбрано");
				return;
			}
			if (seat.TripDate < DateTime.Now)
			{
				UtilManager.Instance.MessageProvider.ShowInformationWindow("Нельзя продать место задним числом");
				return;
			}
			if (seat.State != SeatState.Free && seat.State != SeatState.Reserved)
			{
				UtilManager.Instance.MessageProvider.ShowInformationWindow("Это место уже куплено или забронировано");
				return;
			}
			FuncExec.Execute(
                () =>
                {
                    this.seatVM.ProcessSeat(param as SeatVM);
					// seatVM.LoadData(seat.SS, seat.TripPrice, seat.TripDate);
					UtilManager.Instance.UpdateModifiedObject(ModifiedObjects.Seats);
                });
				this.OnPropertyChanged("SeatsTakenCount");
				this.OnPropertyChanged("SeatsSoldSum");
			
			UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();  
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// проверка продажи билета
		/// </summary>
		/// <param name="seat"></param>
		public bool SeatSellCanExecute(object param)
		{
			SeatVM seat = param as SeatVM;
			if (seat == null) return false;
			return (seat.State == SeatState.Free || seat.State == SeatState.Reserved) && seat.TripDate >= DateTime.Today;/* && UtilManager.Instance.KKMManager.IsKKMEnabled*/;

		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды бронирования места
        /// </summary>
        /// <param name="param"></param>
		private void SeatReserveExecute(object param)
        {
			UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

			SeatVM seat = param as SeatVM;

			// получаем то место, которое изменяем
			seat = this.Seats.FirstOrDefault(s => s.Equals(seat));
			FuncExec.Execute(
				() =>
				{
					// передаем его на сервис
					UtilManager.Instance.Client.SeatReserve(seat.seat);
					UtilManager.Instance.UpdateModifiedObject(ModifiedObjects.Seats);
					seat.State = SeatState.Reserved;
					// seatVM.LoadData(seat.SS, seat.TripPrice, seat.TripDate);
				});
			this.OnPropertyChanged("SeatsTakenCount");
			this.OnPropertyChanged("SeatsSoldSum");

			UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// проверка резервирования билета
		/// </summary>
		/// <param name="seat"></param>
		public bool SeatReserveCanExecute(object param)
		{
			SeatVM seat = param as SeatVM;
			if (seat == null) return false;
			return seat.State == SeatState.Free && seat.TripDate > DateTime.Now;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик команды разбронирования места
		/// </summary>
		/// <param name="param"></param>
		public void SeatUnReserveExecute(object param)
		{
			UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

			SeatVM seat = param as SeatVM;

			//if (seat != null && seat.State == SeatState.Reserved)
			//{
				// получаем то место, которое изменяем
				seat = this.Seats.FirstOrDefault(s => s.Equals(seat));
				FuncExec.Execute(
					() =>
					{
						seat.Passenger = null;
						// передаем его на сервис
						UtilManager.Instance.Client.SeatReserveCancel(seat.seat);
						UtilManager.Instance.UpdateModifiedObject(ModifiedObjects.Seats);
						seat.State = SeatState.Free;
					});
				this.OnPropertyChanged("SeatsTakenCount");
				this.OnPropertyChanged("SeatsSoldSum");
				//this.Seats = this.seatVM.ListSeats;
			//}
			//else
			//{
			//    UtilManager.Instance.MessageProvider.ShowInformationWindow("Это место не зарезервировано");
			//}
			UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// проверка разрезервирования билета
		/// </summary>
		/// <param name="seat"></param>
		public bool SeatUnReserveCanExecute(object param)
		{
			SeatVM seat = param as SeatVM;
			if (seat == null) return false;
			return seat.State == SeatState.Reserved;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик команды возврата места
		/// </summary>
		/// <param name="param"></param>
		public void SeatReturnExecute(object param)
		{
			UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();
			WindowPercentValue window = new WindowPercentValue();

			FuncExec.Execute(() =>
			{
				if (window.ShowDialog() == true)
				{
					this.seatVM.SeatReturn(param as SeatVM, window.Percent);
					UtilManager.Instance.UpdateModifiedObject(ModifiedObjects.Seats);
				}
			});
			this.OnPropertyChanged("SeatsTakenCount");
			this.OnPropertyChanged("SeatsSoldSum");

			UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		/// <summary>
		/// проверка разрезервирования билета
		/// </summary>
		/// <param name="seat"></param>
		public bool SeatReturnCanExecute(object param)
		{
			SeatVM seat = param as SeatVM;
			if (seat == null) return false;

			return seat.State == SeatState.Sold && UtilManager.Instance.KKMManager.IsKKMEnabled;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик команды возврата места
		/// </summary>
		/// <param name="param"></param>
		public void ChangeTicketExecute(object param)
		{
			//WindowChangeTicket window = new WindowChangeTicket();
			//BackgroundWorker worker = new BackgroundWorker();

			//// процесс, работающий в фоновом режиме
			//worker.DoWork += (o, ea) =>
			//{
			//    DispatchService.Invoke(() =>
			//    {
			//        UtilManager.Instance.ShowSplash();
			//    });

			//    FuncExec.Execute(() =>
			//    {
			//        this.tripSheduleVM.LoadDataExecute();
			//    });
			//};

			//// обработчик по окончанию фонового процесса
			//worker.RunWorkerCompleted += (o, ea) =>
			//{
			//    UtilManager.Instance.CloseSplash();
			//    this.changeTicketVM.ChangedSeat = param as SeatVM;
			//    this.changeTicketVM.CurrentDate = DateTime.Today;
			//    this.changeTicketVM.CurrentTrip = this.SelectedTrip;
			//    this.changeTicketVM.CurrentTripPrice = this.SelectedTripPrice;
			//    this.changeTicketVM.StationSchedules = this.tripSheduleVM.Shedules.OrderBy(sh => sh.Time);

			//    DispatchService.Invoke(() =>
			//    {
			//        window.DataContext = this.changeTicketVM;

			//        if (window.ShowDialog() == true)
			//        {
						
			//        }
			//    });
			//    UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
			//};

			//UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

			//// стартуем фоновый процесс
			//worker.RunWorkerAsync();	
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		#endregion

		#region Смена автобуса
		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик команды смены автобуса
		/// </summary>
		/// <param name="param"></param>
		private void ChangeBusExecute(object param)
		{
			IList<object> list = param as IList<object>;
			StationScheduleVM ss = list[0] as StationScheduleVM;
			DateTime date = (DateTime)list[1];

			WindowChangeBus window = new WindowChangeBus();

			this.CatalogCommonExecute(
				window,
				() => {
					FuncExec.Execute(() =>
						{
							this.changeBusVM.LoadData(ss, date, this.seatVM.LoadedSeats.Count);
						});
				},
				this.changeBusVM,
				() => {
					FuncExec.Execute(() =>
						{
							this.tripSheduleVM.LoadData();
							this.OnCatalogChanged("TripShedules");
							this.OnCatalogChanged("Seats");
						});
				});
		}

		/// <summary>
		/// проверка команды смены автобуса
		/// </summary>
		/// <param name="param"></param>
		private bool ChangeBusCanExecute(object param)
		{
			IList<object> list = param as IList<object>;
			if (list == null) return false;

			return
				list[0] != null && list[0] is StationScheduleVM && // выбран рейс
				list[1] != null && // выбрана дата
				this.IsServerEnabled; // сервер доступен
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Выбор подмаршрута
		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// получаем список городов второго комбобокса в зависимости от наличия расписаний
		/// </summary>
		/// <param name="town"></param>
		private IEnumerable<TownVM> GetRouteTowns2(TownVM town, TripVM selectedTrip)
		{
			if (town == null || selectedTrip == null)
			{
				return new List<TownVM>();
			}
			else
			{
				int index = selectedTrip.RouteTowns.IndexOf(town);
				return selectedTrip.RouteTowns.Where(t => selectedTrip.RouteTowns.IndexOf(t) > index);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обновляет список городов первого комбобокса в зависимости от наличия расписаний
		/// </summary>
		/// <param name="town"></param>
		public void UpdateSubTripComboBox(TripVM selectedTrip)
		{
			if (selectedTrip == null)
			{
				this.RouteTowns1 = new List<TownVM>();
			}else
			this.RouteTowns1 = selectedTrip.RouteTowns.Where(t => !t.EqualsById(selectedTrip.Arrival));
			this.OnPropertyChanged("RouteTowns1");
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обновляет список городов второго комбобокса в зависимости от наличия расписаний
		/// </summary>
		/// <param name="town"></param>
		public void UpdateSubTripComboBox(TownVM town, TripVM selectedTrip)
		{
			this.RouteTowns2 = this.GetRouteTowns2(town, selectedTrip);
			this.OnPropertyChanged("RouteTowns2");
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion

		#region Мониторинг обновлений

		/// <summary>
		/// обработчик тика таймера для проверки обновлений
		/// </summary>
		public void MonitorConnection_TimerTick(StationScheduleVM shedule, DateTime? date)
		{
			if (!UtilManager.Instance.TimerManager.BackgroundWorkerMonitorConnection.IsBusy && // если фоновый процесс не занят
				UtilManager.Instance.StateManager.IsServerEnabled && // сервер доступен
				UtilManager.Instance.StateManager.IsWindowEnabled && // не происходит загрузка данных
				Properties.Settings.Default.FlagMonitorConnectionState) //в конфигах установлено, что нужно мониторить соединение
			{
				// проверяем соединение и данные
				UtilManager.Instance.TimerManager.BackgroundWorkerMonitorConnection = new BackgroundWorker();
				this.StartMonitorConnection(shedule, date);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// переподключение к серверу
        /// </summary>
        public void ResetConnection()
        {
            UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

            UtilManager.Instance.Client = new VlastelinSrvClient();
			if (!LoginManager.AttemptLogin(LoginManager.Login, LoginManager.GetPassword()))
            {
                LoginWindow loginWnd = new LoginWindow();

                // если успешна попытка логина
                if (loginWnd.ShowDialog() == true)
                {
                    StartMonitorConnection(null, null);
                    UtilManager.Instance.MessageProvider.ShowInformationWindow("Подключение к серверу выполнено успешно");
                }
            }
            else
            {
                StartMonitorConnection(null, null);
                UtilManager.Instance.MessageProvider.ShowInformationWindow("Подключение к серверу выполнено успешно");
            }
        }

		/// <summary>
		/// мониторит обновления данных
		/// </summary>
		private void StartMonitorConnection(StationScheduleVM shedule, DateTime? date)
		{
			// процесс, работающий в фоновом режиме
			UtilManager.Instance.TimerManager.BackgroundWorkerMonitorConnection.DoWork += (o, ea) =>
			{
				// вначале проверяем соединение с сервером
				UtilManager.Instance.Client.TestConnection();

				// затем проверяем обновления
                this.CheckUpdateDataAailable();
                if (date.HasValue && shedule != null) this.CheckUpdateSeatAailable(shedule, date.Value);
			};

			UtilManager.Instance.TimerManager.BackgroundWorkerMonitorConnection.RunWorkerCompleted += (o, ea) =>
            {
                // обрабатываем ошибки
				if (ea.Error != null)
					{
						DispatchService.Invoke(() =>
						{
							UtilManager.Instance.MessageProvider.ShowWarningnWindow("Отсутствует связь с сервером! Попробуйте подключиться позднее");
							UtilManager.Instance.StateManager.ServerState = ServerStates.Disconnected;
						});
						UtilManager.Instance.logger.LogMessage(LogEventType.Error, ea.ToString());
					}
					else
					{
						UtilManager.Instance.StateManager.ServerState = ServerStates.Connected;
						UtilManager.Instance.TimerManager.Start_TimerMonitorConnection();
					}
            };
			UtilManager.Instance.TimerManager.Stop_TimerMonitorConnection();

			// стартуем фоновый процесс
			UtilManager.Instance.TimerManager.BackgroundWorkerMonitorConnection.RunWorkerAsync();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// проверяет доступность данных
		/// </summary>
		private void CheckUpdateDataAailable()
		{
			// проверка обновления маршрутов
			if (UtilManager.Instance.NeedLoad(ModifiedObjects.Trips))
			{				
				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.StateManager.ServerState = ServerStates.Updating;
				});
				
				this.tripVM.LoadMainLists();
				this.tripVM.UpdateData();
					
				DispatchService.Invoke(() =>
					{
						this.OnCatalogChanged("Trips");
					});
			}

			// проверка обновления расписания
			if (UtilManager.Instance.NeedLoad(ModifiedObjects.TripsSchedule))
			{
				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.StateManager.ServerState = ServerStates.Updating;
				});

				this.tripSheduleVM.LoadDataExecute();

				DispatchService.Invoke(() =>
				{
					this.OnCatalogChanged("TripShedules");
				});
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// проверяет доступность данных
		/// </summary>
		private void CheckUpdateSeatAailable(StationScheduleVM shedule, DateTime date)
		{
			// загрузка мест
			if (this.seatVM.LoadData(shedule, this.SelectedTripPrice, date))
			{
				DispatchService.Invoke(() =>
				{
					this.OnCatalogChanged("Seats");
				});
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		#endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обновляет флаг наличия расписаний у маршрута
        /// </summary>
        public void UpdateTripHasScheduleFlag()
        {
            foreach (var trip in this.Trips)
            {
                trip.SetHasSchedule(this.TripShedules);
            }
        }
	}
}
