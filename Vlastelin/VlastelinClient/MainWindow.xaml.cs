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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VlastelinClient.ViewModel;
//using VlastelinClient.ServiceReference1;
using VlastelinClient.Util;
using Vlastelin.Data.Model;
using Vlastelin.Common;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Collections.ObjectModel;
using VlastelinClient.Windows;
using System.ComponentModel;
using System.Data;
using System.Windows.Threading;
using System.Threading;
using VlastelinClient.ServiceReference1;
//using VlastelinClient.ServiceReference3;

namespace VlastelinClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// вьюмодель главного окна программы
        /// </summary>
		public MainWindowVM mainViewModel { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		#region Инициализация

		public MainWindow()
        {
            InitializeComponent();
        }
		
        /// <summary>
        /// иниализация основных параметров перед вызовом окна
        /// </summary>
        private void Init()
        {
            this.DataContext = mainViewModel;
			this.datePickerDepartureDate.SelectedDate = DateTime.Today;

			if (this.listBoxTrips.ItemsSource != null)
			{
                //ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxTrips.ItemsSource);
                //view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Departure", System.ComponentModel.ListSortDirection.Descending));
			}

            this.ListTripsSelected(null);
            this.ListShedulesSelected(null, null);

            this.mainViewModel.CatalogChanged+=new PropertyChangedEventHandler(MainViewModel_CatalogChanged);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// загрузка окна программы
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.Init();
			UtilManager.Instance.TimerManager.Init();
			UtilManager.Instance.TimerManager.TimerMonitorConnection.Tick += TimerMonitorConnection_Tick;
			UtilManager.Instance.TimerManager.TimerMonitorAppVersion.Tick += TimerMonitorAppVersion_Tick;
			UtilManager.Instance.TimerManager.StartTimers();
		}

		#endregion

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		#region События изменения выделенного элемента в списках

		/// <summary>
        /// обработчик события изменения содержимого списков
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">измененный список</param>
        private void MainViewModel_CatalogChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Trips")
            {
                this.mainViewModel.UpdateTripHasScheduleFlag();
                this.listBoxTrips.ItemsSource = this.mainViewModel.Trips;
				this.ListTripsSelected(this.mainViewModel.SelectedTripPrice);
            }
            if (e.PropertyName == "TripShedules")
            {
                this.mainViewModel.UpdateTripHasScheduleFlag();
                this.listBoxTrips.ItemsSource = this.mainViewModel.Trips;
				this.listBoxTripShedules.ItemsSource = this.mainViewModel.TripShedules;
                this.ListTripsSelected(this.mainViewModel.SelectedTripPrice);
				this.ListShedulesSelected(null, null);
            }
			if (e.PropertyName == "Seats")
			{
				this.ListShedulesSelected(this.listBoxTripShedules.SelectedItem as StationScheduleVM, this.mainViewModel.SelectedTripPrice);
			}
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик события изменения выделенного элемента в списке маршрутов
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listBoxTrips_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TripVM trip = this.listBoxTrips.SelectedItem as TripVM;
			this.mainViewModel.UpdateSubTripComboBox(trip);
			this.comboBoxDeparture.SelectedIndex = 0;
			this.mainViewModel.UpdateSubTripComboBox(this.comboBoxDeparture.SelectedItem as TownVM, trip);
			this.comboBoxArrival.SelectedItem = trip.Arrival;

			this.ComboBoxUpdate();

			if (this.listBoxTripShedules.ItemsSource != null)
			{
				ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxTripShedules.ItemsSource);
				view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Time", System.ComponentModel.ListSortDirection.Ascending));
			}
			this.ListTripsSelected(this.mainViewModel.SelectedTripPrice);
			this.ListShedulesSelected(null, null);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик события изменения выбранного расписания маршрутов
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listBoxTripShedules_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UtilManager.Instance.ModifiedTables[ModifiedObjects.Seats] = DateTime.MinValue;
			this.ListShedulesSelected(this.listBoxTripShedules.SelectedItem as StationScheduleVM, this.mainViewModel.SelectedTripPrice);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void comboBoxDeparture_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TripVM trip = this.listBoxTrips.SelectedItem as TripVM;
			if (trip != null)
			{
				this.mainViewModel.UpdateSubTripComboBox(this.comboBoxDeparture.SelectedItem as TownVM, trip);
				if (this.comboBoxArrival.SelectedItem == null)
				{
					this.comboBoxArrival.SelectedItem = trip.Arrival;
				}
				this.ComboBoxUpdate();
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void comboBoxArrival_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.ComboBoxUpdate();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		#endregion

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		#region События других элементов

		/// <summary>
		/// обработчик события двойного клика на списке мест
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridSeats_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.SellSeat();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик события выхода по кнопке меню
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			this.CloseProgram();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработсчик события загрузки списка маршрутов
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listBoxTrips_Loaded(object sender, RoutedEventArgs e)
		{
            //ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxTrips.ItemsSource);
            //view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Departure", System.ComponentModel.ListSortDirection.Ascending));
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обрабочтик события обновления списка мест
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonSeatUpdate_Click(object sender, RoutedEventArgs e)
		{
			UtilManager.Instance.ModifiedTables[ModifiedObjects.Seats] = DateTime.MinValue;
			this.ListShedulesSelected(this.listBoxTripShedules.SelectedItem as StationScheduleVM, this.mainViewModel.SelectedTripPrice);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ///// <summary>
        ///// установка фокуса листбоксов
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ItemsControl_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    ItemsControl list = sender as ItemsControl;
        //    if (list != null)
        //    {
        //        list.Focus();
        //    }
        //}

		#endregion

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		#region Методы

        /// <summary>
        /// обработчик списка маршрутов
        /// </summary>
        /// <param name="listbox"></param>
        private void ListTripsSelected(TripPriceVM price)
        {
            ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxTripShedules.ItemsSource);
            if (view != null)
            {
                // filter for listbox
                view.Filter = delegate(object obj)
                {
                    StationScheduleVM item = obj as StationScheduleVM;
                    if (item == null || price == null)
                    {
                        return false;
                    }
					bool res = item == null ? false : item.FilterTripCondition(this.datePickerDepartureDate.SelectedDate, price, this.listBoxTrips.SelectedItem as TripVM);
                    return res;
                };
            }
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик выделения списка расписания
        /// </summary>
        private void ListShedulesSelected(StationScheduleVM shedule, TripPriceVM price)
        {
            if (shedule == null)
            {
                this.listBoxTripShedules.SelectedIndex = -1;
            }
			this.mainViewModel.UpdateSeatsExecute(shedule, price, this.datePickerDepartureDate.SelectedDate);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// продажа билета
        /// </summary>
        private void SellSeat()
        {
            StationScheduleVM shedule = this.listBoxTripShedules.SelectedItem as StationScheduleVM;
            SeatVM seat = this.dataGridSeats.SelectedItem as SeatVM;

            if (shedule != null && seat != null && this.mainViewModel.SelectedTripPrice != null)
            {
                if (seat.SS == null)
                {
                    seat.SS = shedule;
                }
                if (seat.TripPrice == null)
                {
                    seat.TripPrice = this.mainViewModel.SelectedTripPrice;
                }
				UtilManager.Instance.TimerManager.TimerMonitorConnection.Stop();
				this.mainViewModel.SeatSellExecute(seat);
				UtilManager.Instance.TimerManager.TimerMonitorConnection.Start();
            }
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик изменения выбора подмаршрута в комбобоксе
		/// </summary>
		private void ComboBoxUpdate()
		{
			TripVM trip = this.listBoxTrips.SelectedItem as TripVM;
			if (trip != null && trip.Prices != null)
			{
				this.mainViewModel.SelectedTripPrice = trip.Prices.FirstOrDefault(tr => tr.Departure.Equals(this.comboBoxDeparture.SelectedItem) && tr.Arrival.Equals(this.comboBoxArrival.SelectedItem));
				this.ListTripsSelected(this.mainViewModel.SelectedTripPrice);
				this.ListShedulesSelected(null, null);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// выход из программы
		/// </summary>
		private void CloseProgram()
		{
			if (UtilManager.Instance.MessageProvider.ShowYesNoDialogWindow("Выйти из программы?"))
			{
				Application.Current.Shutdown();
			}
		}

		#endregion

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		#region Таймеры и проверки

        /// <summary>
        /// обработчик события тика таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void TimerMonitorConnection_Tick(object sender, EventArgs e)
        {
			this.mainViewModel.MonitorConnection_TimerTick(this.listBoxTripShedules.SelectedItem as StationScheduleVM, this.datePickerDepartureDate.SelectedDate);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// обработчик события тика таймера для проверки обновлений данных
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimerMonitorAppVersion_Tick(object sender, EventArgs e)
		{
			this.mainViewModel.CheckApplicationVersion();
		}

		#endregion

		private void datePickerDepartureDate_SelectedDateChanged_1(object sender, SelectionChangedEventArgs e)
		{
			if (this.mainViewModel != null)
			{
				this.ListTripsSelected(this.mainViewModel.SelectedTripPrice);
				UtilManager.Instance.ModifiedTables[ModifiedObjects.Seats] = DateTime.MinValue;
				this.ListShedulesSelected(null, null);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}
