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
using VlastelinClient.ViewModel.WindowsViewModel;

namespace VlastelinClient.Windows
{
	/// <summary>
	/// Interaction logic for WindowRKO.xaml
	/// </summary>
	public partial class WindowRKO : WindowBase
	{
		private ListRKOWindowVM vm;
		
		public WindowRKO()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void dataGridRKO_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (this.dataGridRKO.SelectedItem != null)
			{
				vm.PrintExecute(this.dataGridRKO.SelectedItem);
			}
		}

		private void WindowBase_Loaded(object sender, RoutedEventArgs e)
		{
			this.vm = this.DataContext as ListRKOWindowVM;
		}
	}
}
