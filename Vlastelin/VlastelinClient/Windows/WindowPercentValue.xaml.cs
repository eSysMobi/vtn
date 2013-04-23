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
using VlastelinClient.Util;

namespace VlastelinClient.Windows
{
	/// <summary>
	/// Interaction logic for WindowPercentValue.xaml
	/// </summary>
	public partial class WindowPercentValue : WindowBase
	{
		public double Percent
		{
			get
			{
				return (double)this.doubleUpDownSum.Value * 100;
			}
		}
		
		public WindowPercentValue()
		{
			InitializeComponent();
		}

		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			if (this.doubleUpDownSum.Value == null || this.doubleUpDownSum.Value < 0)
			{
				UtilManager.Instance.MessageProvider.ShowInformationWindow("Значение должно быть положительным");
			}
			else
			{
				this.DialogResult = true;
				this.Close();
			}
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void WindowBase_Loaded(object sender, RoutedEventArgs e)
		{
			this.doubleUpDownSum.Value = (double)UtilManager.Instance.ServerSettings.ReturnedCommission / 100;
			this.doubleUpDownSum.Focus();
		}
	}
}
