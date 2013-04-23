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
using Vlastelin.Common;
using VlastelinClient.Util;
using VlastelinClient.ViewModel.WindowsViewModel;
using System.ComponentModel;

namespace VlastelinClient.Windows
{
	/// <summary>
	/// Interaction logic for WindowChangeBus.xaml
	/// </summary>
	public partial class WindowChangeBus : WindowBase
	{		
		public WindowChangeBus()
		{
			InitializeComponent();
		}	

		/// <summary>
		/// обработчик события кнопки отмены
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

		private void WindowBase_Loaded(object sender, RoutedEventArgs e)
		{
			(DataContext as ChangeBusWindowVM).ViewModelNotify+=new System.ComponentModel.PropertyChangedEventHandler(WindowChangeBus_ViewModelNotify);
		}

		private void WindowChangeBus_ViewModelNotify(object sender, PropertyChangedEventArgs e)
		{
			// this.Close();
		}
	}
}
