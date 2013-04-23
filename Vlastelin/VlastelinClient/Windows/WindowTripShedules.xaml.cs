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
using System.Windows.Shapes;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Data;
using VlastelinClient.ViewModel;
using System.ComponentModel;
using Vlastelin.Data.Model;
using Vlastelin.Common;
using System.Collections;

namespace VlastelinClient.Windows
{
    /// <summary>
    /// Interaction logic for WindowTripShedules.xaml
    /// </summary>
    public partial class WindowTripShedules : WindowBase
    {
		TripSheduleWindowVM viewModel
		{
			get
			{
				return this.DataContext as TripSheduleWindowVM;
			}
		}
		
		public WindowTripShedules()
        {
            InitializeComponent();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void FilterList(IEnumerable source, String dep, String arr)
		{
			ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(source);

			// filter for listbox
			view.Filter = delegate(object obj)
			{
				TripVM item = obj as TripVM;
				return item == null ? false : item.FilterCondition(dep, arr);
			};
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
			this.listBoxTrips.SelectedIndex = -1;
        }

		private void Filter_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.FilterList(this.listBoxTrips.ItemsSource, this.textBoxTripFilterDepartureTown.Text, this.textBoxTripFilterArrivalTown.Text);
		}

        /// <summary>
        /// обновляем выделенный маршрут
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxTrips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TripVM trip = this.listBoxTrips.SelectedItem as TripVM;
            if (trip != null)
            {
                viewModel.UpdateSelectedTrip(trip);

				if (this.dataGridMatrix.ItemsSource != null)
				{
					DataView view = this.dataGridMatrix.ItemsSource as DataView;
					view.Sort = view.Table.Columns[0].ColumnName;
				}
			}
        }

		/// <summary>
		/// обработчик события изменения выделенного ТС в матрице времени
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridMatrix_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.viewModel.UpdateSelectedStationSchedule(this.dataGridMatrix.SelectedItem as DataRowView);
		}

		private void listBoxTrips_Loaded(object sender, RoutedEventArgs e)
		{
			this.FilterList(this.listBoxTrips.ItemsSource, String.Empty, String.Empty);
			this.listBoxTrips.SelectedIndex = 0;
		}

		private void WindowBase_Loaded(object sender, RoutedEventArgs e)
		{
			(DataContext as TripSheduleWindowVM).ChangeItem+=new ChangeItemEventHandler(Window_ChangeItem);
		}

		private void Window_ChangeItem(object sender, ChangeItemEventArgs e)
		{
			if (e.ChangedItem == null)
			{
				this.dataGridMatrix.SelectedItem = null;
				this.viewModel.UpdateSelectedStationSchedule(null);
                this.datePickerDate.SelectedDate = DateTime.Today;
			}
			else
			{
				if (this.viewModel.SelectedSchedule != null)
				{
					foreach (DataRowView row in this.dataGridMatrix.Items)
					{
						if (row.Row[0].ToString().Equals(this.viewModel.SelectedSchedule.DepartureTime.GetTimeString()))
						{
							this.dataGridMatrix.SelectedItem = row;
							break;
						}
					}
				}
			} 
		}
    }
}
