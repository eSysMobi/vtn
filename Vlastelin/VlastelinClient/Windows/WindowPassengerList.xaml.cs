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
using System.ComponentModel;
using Reports.Classes;

namespace VlastelinClient.Windows
{
    /// <summary>
    /// Interaction logic for WindowPassengerList.xaml
    /// </summary>
    public partial class WindowPassengerList : WindowBase
    {
		private PassengersListWindowVM viewModel
		{
			get
			{
				return this.DataContext as PassengersListWindowVM;
			}
		}
		
		public WindowPassengerList()
        {
            InitializeComponent();
			this.datePickerFrom.SelectedDate = ReportsExecutor.IntervalFrom;
			this.datePickerTo.SelectedDate = ReportsExecutor.IntervalTo;
        }

		//private void Filter_TextChanged(object sender, TextChangedEventArgs e)
		//{
		//    ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.dataGridPassengers.ItemsSource);

		//    // filter for listbox
		//    view.Filter = delegate(object obj)
		//    {
		//        PassengerVM item = obj as PassengerVM;
		//        return item == null ? false : item.FilterCondition(this.textBoxName.Text, this.textBoxSurName.Text, this.textBoxPatronymic.Text);
		//    };
		//}

		///// <summary>
		///// обработка события двойного клика на список пассажиров
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//private void dataGridPassengers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		//{
		//    PassengersListWindowVM vm = DataContext as PassengersListWindowVM;
		//    if (vm != null)
		//    {
		//        // формируем список параметров: пассажир и период времени
		//        List<object> list = new List<object>()
		//        {
		//            this.dataGridPassengers.SelectedItem as PassengerVM,
		//            this.datePickerFrom.SelectedDate,
		//            this.datePickerTo.SelectedDate
		//        };
		//        vm.ShowReportCommandExecute(list);
		//    }
		//}

		/// <summary>
		/// обработка события изменения вьюмодели (для закрытия диалога после печати отчета)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewModel_ViewModelNotify(object sender, PropertyChangedEventArgs e)
		{
			this.Close();
		}

		private void WindowBase_Loaded(object sender, RoutedEventArgs e)
		{
			this.viewModel.ViewModelNotify += new System.ComponentModel.PropertyChangedEventHandler(ViewModel_ViewModelNotify);
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
