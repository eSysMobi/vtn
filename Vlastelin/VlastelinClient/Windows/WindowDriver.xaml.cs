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
    /// Interaction logic for WindowDriver.xaml
    /// </summary>
    public partial class WindowDriver : WindowBase
    {
        public WindowDriver()
        {
            InitializeComponent();
            this.datePickerAuthDate.SelectedDate = DateTime.Now.Date;
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void FilterList(IEnumerable source, String name, String surname)
		{
			ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(source);

			// filter for listbox
			view.Filter = delegate(object obj)
			{
				DriverVM item = obj as DriverVM;
				return item == null ? false : item.FilterCondition(name, surname);
			};
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
			this.FilterList(this.listBoxDrivers.ItemsSource, this.textBoxDriverFilterName.Text, this.textBoxDriverFilterSurname.Text);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void listBoxDrivers_Loaded(object sender, RoutedEventArgs e)
		{
            if (this.listBoxDrivers.ItemsSource != null)
            {
				this.FilterList(this.listBoxDrivers.ItemsSource, String.Empty, String.Empty);
				
				ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxDrivers.ItemsSource);
                view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Surname", System.ComponentModel.ListSortDirection.Ascending));
            }
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as DriverWindowVM).ChangeItem += new ChangeItemEventHandler(Window_ChangeItem);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_ChangeItem(object sender, ChangeItemEventArgs e)
        {
            this.listBoxDrivers.SelectedItem = e.ChangedItem;
        }
    }
}
