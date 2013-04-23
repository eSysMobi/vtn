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
using VlastelinClient.ViewModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Collections;

namespace VlastelinClient
{
    /// <summary>
    /// Interaction logic for WindowBusesNew.xaml
    /// </summary>
    public partial class WindowBusesNew : WindowBase
    {
        public WindowBusesNew()
        {
            InitializeComponent();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void FilterList(IEnumerable source, String manf, String model, String pass)
		{
			ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(source);

			// filter for listbox
			view.Filter = delegate(object obj)
			{
				BusVM item = obj as BusVM;
				return item == null ? false : item.FilterCondition(manf, model, pass);
			};
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
			this.FilterList(this.listBoxBuses.ItemsSource, this.textBoxBusFilterManufacter.Text, this.textBoxBusFilterModel.Text, this.textBoxBusFilterPassengers.Text);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void textBoxBusFilterPassengers_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
			this.FilterList(this.listBoxBuses.ItemsSource, this.textBoxBusFilterManufacter.Text, this.textBoxBusFilterModel.Text, this.textBoxBusFilterPassengers.Text);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void listBoxBuses_Loaded(object sender, RoutedEventArgs e)
		{
            if (this.listBoxBuses.ItemsSource != null)
            {
				this.FilterList(this.listBoxBuses.ItemsSource, String.Empty, String.Empty, String.Empty);
				
				ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxBuses.ItemsSource);
                view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Manufacter", System.ComponentModel.ListSortDirection.Ascending));
            }
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as BusWindowVM).ChangeItem += new ChangeItemEventHandler(Window_ChangeItem);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_ChangeItem(object sender, ChangeItemEventArgs e)
        {
            this.listBoxBuses.SelectedItem = e.ChangedItem;
        }
    }
}
