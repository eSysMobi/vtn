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
using VlastelinClient.ServiceReference1;
using Vlastelin.Common;
using VlastelinClient.Windows;

namespace VlastelinClient.ViewModel
{
    /// <summary>
    /// вьюмодель главного окна программы, содержит остальные элементы
    /// </summary>
    public class MainWindowVM : BaseViewModel
    {

        #region fields

        /// <summary>
        /// вьюмодель окна городов
        /// </summary>
        private TownWindowVM townVM;

        /// <summary>
        /// вьюмодель окна автобусов
        /// </summary>
        private BusWindowVM busVM;

        /// <summary>
        /// вьюмодель для окна водителей
        /// </summary>
        private DriverWindowVM driverVM;

        /// <summary>
        /// вьюмодель для окна владельцев
        /// </summary>
        private OwnerWindowVM ownerVM;

        /// <summary>
        /// вьюмодель для туров
        /// </summary>
        private TripWindowVM tripVM;

        /// <summary>
        /// вьюмодель для филиалов
        /// </summary>
        private BranchWindowVM branchVM;

        /// <summary>
        /// вьюмодель для расписания
        /// </summary>
        private TripSheduleWindowVM tripSheduleVM;

        /// <summary>
        /// содержит сервисные данные и вспмогательные модули
        /// </summary>
        private Utilities utilite;

        #endregion

        #region properties

        /// <summary>
        /// текущий оператор системы
        /// </summary>
        public Operator CurrentOperator { get; private set; }

        /// <summary>
        /// определяет зависимость видимости интерфейса от роли оператора
        /// </summary>
        public bool VisibilityByRole
        {
            get
            {
                return this.CurrentOperator.Role == Roles.Administrator;
            }
        }

        #endregion

        /// <summary>
        /// команды для меню и иных действий
        /// </summary>
        #region commands

        public ICommand CatalogTownsCommand
        {
            get 
            {
                return new RelayCommand(this.CatalogTownsExecute);
            }
        }

        public ICommand CatalogBusesCommand
        {
            get
            {
                return new RelayCommand(this.CatalogBusesExecute);
            }
        }

        public ICommand CatalogDriversCommand
        {
            get
            {
                return new RelayCommand(this.CatalogDriversExecute);
            }
        }

        public ICommand CatalogOwnersCommand
        {
            get
            {
                return new RelayCommand(this.CatalogOwnersExecute);
            }
        }

        public ICommand CatalogTripsCommand
        {
            get
            {
                return new RelayCommand(this.CatalogTripsExecute);
            }
        }

        public ICommand CatalogBranchesCommand
        {
            get
            {
                return new RelayCommand(this.CatalogBranchesExecute);
            }
        }

        public ICommand CatalogTripShedulesCommand
        {
            get
            {
                return new RelayCommand(this.CatalogTripShedulesExecute);
            }
        }
        #endregion

        public MainWindowVM(Utilities ut)
        {
            this.utilite = ut;

            this.Init();
            this.LoadInitData();
        }

        /// <summary>
        /// инициализация вьюмоделей
        /// </summary>
        private void Init()
        {
            this.ownerVM = new OwnerWindowVM(this.utilite);
            this.townVM = new TownWindowVM(this.utilite);
            this.busVM = new BusWindowVM(this.utilite, this.ownerVM.Owners);
            this.driverVM = new DriverWindowVM(this.utilite);
            this.tripVM = new TripWindowVM(this.utilite, this.townVM.Towns);
            this.tripSheduleVM = new TripSheduleWindowVM(this.utilite, this.tripVM.Trips, this.busVM.Buses);

            this.Seats = SampleData.Seats;
        }

        /// <summary>
        /// загрузка первоначальных данных
        /// </summary>
        private void LoadInitData()
        {
            this.townVM.LoadDataWithLog();
            this.ownerVM.LoadDataWithLog();
            this.busVM.LoadDataWithLog();
            this.tripVM.LoadDataWithLog();
            this.tripSheduleVM.LoadDataWithLog();
        }

        /// <summary>
        /// обработчик команды показа окна городов
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogTownsExecute(object param)
        {
            WindowTowns window = new WindowTowns();
            townVM.LoadDataWithLog();
            window.DataContext = townVM;

            window.ShowDialog();
        }

        /// <summary>
        /// обработчик команды показа окна автобусов
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogBusesExecute(object param)
        {
            WindowBusesNew window = new WindowBusesNew();

            this.ownerVM.LoadDataWithLog();
            busVM.LoadDataWithLog();
            busVM.Owners = this.ownerVM.Owners;

            window.DataContext = busVM;
            window.ShowDialog();
        }

        /// <summary>
        /// обработчик команды показа окна водителей
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogDriversExecute(object param)
        {
            WindowDriver window = new WindowDriver();
            this.driverVM.LoadDataWithLog();
            window.DataContext = driverVM;

            window.ShowDialog();
        }

        /// <summary>
        /// обработчик команды показа окна владельцев
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogOwnersExecute(object param)
        {
            WindowOwners window = new WindowOwners();
            this.ownerVM.LoadDataWithLog();
            window.DataContext = ownerVM;

            window.ShowDialog();
        }

        /// <summary>
        /// обработчик команды показа окна туров
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogTripsExecute(object param)
        {
            WindowTrips window = new WindowTrips();
            this.tripVM.LoadDataWithLog();
            window.DataContext = tripVM;

            window.ShowDialog();
        }

        /// <summary>
        /// обработчик команды показа окна филиалов
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogBranchesExecute(object param)
        {
            //BranchWindow window = new BranchWindow();
            //this.branchViewModel.LoadData();
            //window.DataContext = branchViewModel;

            //window.ShowDialog();
        }

                /// <summary>
        /// обработчик команды показа окна расписания
        /// </summary>
        /// <param name="param">не используется</param>
        public void CatalogTripShedulesExecute(object param)
        {
            WindowTripShedules window = new WindowTripShedules();
            this.tripSheduleVM.LoadDataWithLog();
            window.DataContext = tripSheduleVM;

            window.ShowDialog();
        }

        #region Продажа билетов

        public ObservableCollection<TripVM> Trips
        {
            get
            {
                return this.tripVM != null ? this.tripVM.Trips : null;
            }
        }

        public ObservableCollection<TripSheduleVM> TripShedules
        {
            get
            {
                return this.tripSheduleVM != null ? this.tripSheduleVM.Shedules : null;
            }
        }

        public ObservableCollection<SeatVM> Seats { get; private set; }

        #endregion


    }
}
