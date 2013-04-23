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
using VlastelinClient.ViewModel.WindowsViewModel;
using VlastelinClient.ViewModel;
using System.Collections;

namespace VlastelinClient
{
    /// <summary>
    /// Interaction logic for WindowOperators.xaml
    /// </summary>
    public partial class WindowOperators : WindowBase
    {
        public WindowOperators()
        {
            InitializeComponent();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void FilterList(IEnumerable source, String name, String surname)
		{
			ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(source);

			// filter for listbox
			view.Filter = delegate(object obj)
			{
				OperatorVM item = obj as OperatorVM;
				return item == null ? false : item.FilterCondition(name, surname);
			};
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
			this.FilterList(this.listBoxOperators.ItemsSource, this.textBoxFilterName.Text, this.textBoxFilterSurname.Text);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void SortList()
		{
			if (this.listBoxOperators.ItemsSource != null)
			{
				this.FilterList(this.listBoxOperators.ItemsSource, String.Empty, String.Empty);

				ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.listBoxOperators.ItemsSource);
				view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Surname", System.ComponentModel.ListSortDirection.Ascending));
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void listBoxOperators_Loaded(object sender, RoutedEventArgs e)
		{
			this.SortList();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as OperatorWindowVM).ChangeItem += new ChangeItemEventHandler(Window_ChangeItem);
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_ChangeItem(object sender, ChangeItemEventArgs e)
        {
			this.SortList();
			this.listBoxOperators.SelectedItem = e.ChangedItem;
			this.passwordBoxPassword.Password = String.Empty;
        }
    }
}
