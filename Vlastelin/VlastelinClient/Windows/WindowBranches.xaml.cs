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

namespace VlastelinClient
{
    /// <summary>
    /// Interaction logic for WindowBranches.xaml
    /// </summary>
    public partial class WindowBranches : WindowBase
    {
        public WindowBranches()
        {
            InitializeComponent();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void FilterList(IEnumerable source, String name, String town)
		{
			ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(source);

            // filter for listbox
            view.Filter = delegate(object obj)
            {
                BranchVM item = obj as BranchVM;
                return item == null ? false : item.FilterCondition(name, town);
			};
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
			this.FilterList(this.listBoxBranches.ItemsSource, this.textBoxFilterName.Text, this.textBoxFilterTown.Text);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void listBoxBranches_Loaded(object sender, RoutedEventArgs e)
		{
            if (this.listBoxBranches.ItemsSource != null)
            {
				this.FilterList(this.listBoxBranches.ItemsSource, String.Empty, String.Empty);

				ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxBranches.ItemsSource);
                view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            }
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as BranchWindowVM).ChangeItem += new ChangeItemEventHandler(Window_ChangeItem);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_ChangeItem(object sender, ChangeItemEventArgs e)
        {
            this.listBoxBranches.SelectedItem = e.ChangedItem;
        }
    }
}
