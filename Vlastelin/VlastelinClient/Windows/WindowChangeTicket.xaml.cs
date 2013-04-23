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

namespace VlastelinClient.Windows
{
	/// <summary>
	/// Interaction logic for WindowChangeTicket.xaml
	/// </summary>
	public partial class WindowChangeTicket : WindowBase
	{
		public ChangeTicketWindowVM vm
		{
			get
			{
				return this.DataContext as ChangeTicketWindowVM;
			}
		}
		
		public WindowChangeTicket()
		{
			InitializeComponent();
		}

		private void calendarDepartureDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
		{
			ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.comboBoxSchedules.ItemsSource);
			if (view != null)
			{
				// filter for listbox
				view.Filter = delegate(object obj)
				{
					StationScheduleVM item = obj as StationScheduleVM;
					if (item == null || vm.CurrentTripPrice == null)
					{
						return false;
					}
					bool res = item == null ? false : item.FilterTripCondition(this.calendarDepartureDate.SelectedDate, vm.CurrentTripPrice, vm.CurrentTrip);
					return res;
				};
			}
		}
	}
}
