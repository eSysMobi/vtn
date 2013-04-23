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
using Reports.Classes;

namespace VlastelinClient.Windows
{
	/// <summary>
	/// Interaction logic for WindowDateInterval.xaml
	/// </summary>
	public partial class WindowDateInterval : WindowBase
	{
		/// <summary>
		/// начало интервала
		/// </summary>
		public DateTime? IntervalFrom
		{
			get
			{
				return this.datePickerFrom.SelectedDate;
			}
		}

		/// <summary>
		/// конец интервала
		/// </summary>
		public DateTime? IntervalTo
		{
			get
			{
				return this.datePickerTo.SelectedDate;
			}
		}
		
		public WindowDateInterval()
		{
			InitializeComponent();

			this.datePickerFrom.SelectedDate = ReportsExecutor.IntervalFrom;
			this.datePickerTo.SelectedDate = ReportsExecutor.IntervalTo;
		}

		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}
	}
}
