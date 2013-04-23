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
    /// Interaction logic for WindowTowns.xaml
    /// </summary>
    public partial class WindowTowns : WindowBase
    {
        public WindowTowns()
        {
            InitializeComponent();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void FilterList(IEnumerable source, String name)
		{
			ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(source);

			// filter for listbox
			view.Filter = delegate(object obj)
			{
				TownVM item = obj as TownVM;
				return item == null ? false : item.FilterCondition(name);
			};
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
			this.FilterList(this.listBoxTowns.ItemsSource, this.textBoxTownFilterName.Text);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void listBoxTowns_Loaded(object sender, RoutedEventArgs e)
		{
            if (this.listBoxTowns.ItemsSource != null)
            {
				this.FilterList(this.listBoxTowns.ItemsSource, String.Empty);

				ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxTowns.ItemsSource);
                view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            }
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as TownWindowVM).ChangeItem += new ChangeItemEventHandler(Window_ChangeItem);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_ChangeItem(object sender, ChangeItemEventArgs e)
        {
            this.listBoxTowns.SelectedItem = e.ChangedItem;
        }
    }
}
