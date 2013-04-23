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
using VlastelinClient.ViewModel;
using System.Collections;

namespace VlastelinClient.Windows
{
    /// <summary>
    /// Interaction logic for WindowTrips.xaml
    /// </summary>
    public partial class WindowTrips : WindowBase
    {
        public WindowTrips()
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

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
			this.FilterList(this.listBoxTrips.ItemsSource, this.textBoxTripFilterDeparture.Text, this.textBoxTripFilterArrivalTown.Text);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void listBoxTrips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TripWindowVM vm = DataContext as TripWindowVM;
            TripVM trip = this.listBoxTrips.SelectedItem as TripVM;
            if (vm != null && trip != null)
            {
                this.comboBoxRouteTowns.SelectedIndex = 0;
            }
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.dataGridTowns.SelectedIndex = -1;
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void listBoxTrips_Loaded(object sender, RoutedEventArgs e)
		{
			this.FilterList(this.listBoxTrips.ItemsSource, String.Empty, String.Empty);
			this.SortTrips();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as TripWindowVM).ChangeItem+=new ChangeItemEventHandler(Window_ChangeItem);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_ChangeItem(object sender, ChangeItemEventArgs e)
        {
			this.listBoxTrips.SelectedItem = e.ChangedItem;
            this.SortTrips();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void SortTrips()
		{
			if (this.listBoxTrips.ItemsSource != null)
			{
				ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxTrips.ItemsSource);
				view.SortDescriptions.Add(new System.ComponentModel.SortDescription("NameString", System.ComponentModel.ListSortDirection.Ascending));
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
