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
    /// Interaction logic for WindowOwners.xaml
    /// </summary>
    public partial class WindowOwners : WindowBase
    {
        public WindowOwners()
        {
            InitializeComponent();
            this.datePickerAuthDate.SelectedDate = DateTime.Now.Date;
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void FilterList(IEnumerable source, String name, String numsv)
		{
			ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(source);

			// filter for listbox
			view.Filter = delegate(object obj)
			{
				OwnerVM item = obj as OwnerVM;
				return item == null ? false : item.FilterCondition(name, numsv);
			};
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
			this.FilterList(this.listBoxOwners.ItemsSource, this.textBoxOwnerFilterName.Text, this.textBoxOwnerFilterNumSv.Text);
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

		private void listBoxOwners_Loaded(object sender, RoutedEventArgs e)
		{
            if (this.listBoxOwners.ItemsSource != null)
            {
				this.FilterList(this.listBoxOwners.ItemsSource, String.Empty, String.Empty);
				
				ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxOwners.ItemsSource);
                view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            }
		}

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as OwnerWindowVM).ChangeItem += new ChangeItemEventHandler(Window_ChangeItem);
        }

        private void Window_ChangeItem(object sender, ChangeItemEventArgs e)
        {
            this.listBoxOwners.SelectedItem = e.ChangedItem;
        }
    }
}
